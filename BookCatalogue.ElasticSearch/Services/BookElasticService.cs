using BookCatalogue.Contracts.ElasticSearch.Services;
using BookCatalogue.ElasticSearch.Documents;
using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookCatalogue.ElasticSearch.Services
{
    public class BookElasticService : IBookElasticService
    {
        private const string IndexName = "books";

        private readonly IElasticClient _client;

        public BookElasticService(IElasticClient client)
        {
            _client = client;
            InitializeIndex();
        }

        public async Task<IEnumerable<Book>> SearchBooks(
            string searchValue,
            string genre = "",
            DateTime? startDate = null,
            DateTime? endDate = null,
            int take = 10)
        {
            var filters = new List<Func<QueryContainerDescriptor<Book>, QueryContainer>>();

            if (!string.IsNullOrEmpty(genre))
            {
                filters.Add(fq => fq.MatchPhrase(t => t.Field(f => f.Genre).Query(genre)));
            }

            if (startDate.HasValue)
            {
                filters.Add(fq => fq
                    .DateRange(dr => dr
                        .Field(f => f.PublishDate)
                        .GreaterThanOrEquals(startDate.Value)));
            }

            if (endDate.HasValue)
            {
                filters.Add(fq => fq
                    .DateRange(dr => dr
                        .Field(f => f.PublishDate)
                        .LessThanOrEquals(endDate.Value)));
            }

            var response = await _client.SearchAsync<Book>(s => s
                .Index(IndexName)
                .Take(take)
                .Query(q => q
                    .Bool(bq => bq.Filter(filters)) &&
                    q.MultiMatch(mm => mm
                        .Fields(f => f
                            .Field(f1 => f1.Title)
                            .Field(f1 => f1.Description))
                        .Query(searchValue))));

            return response.Documents;
        }

        public async Task<Book> GetBook(Guid id)
        {
            var response = await _client.GetAsync<Book>(id, s => s
                .Index(IndexName));
            return response.Source;
        }

        public async Task<Book> AddBook(Book book)
        {
            book.Id = Guid.NewGuid();

            var indexResponse = await _client.IndexAsync(book, i => i
                .Index(IndexName)
                .Id(book.Id));

            if (!indexResponse.IsValid)
            {
                throw indexResponse.OriginalException;
            }

            var getResponse = await _client.GetAsync<Book>(indexResponse.Id, g => g
                .Index(IndexName));
            return getResponse.Source;
        }

        public async Task RemoveBook(Guid id)
        {
            var response = await _client.DeleteAsync<Book>(id, i => i.Index(IndexName));

            if (!response.IsValid)
            {
                throw response.OriginalException;
            }
        }

        public async Task UpdateBook(Book book)
        {
            var response = await _client.UpdateAsync<Book>(book.Id, b => b
                .Index(IndexName)
                .Doc(book));

            if (!response.IsValid)
            {
                throw response.OriginalException;
            }
        }

        private void InitializeIndex()
        {
            _client.Indices.Create(IndexName, c => c
                .Map<Book>(m => m
                    .Properties(p => p
                        .Text(t => t
                            .Name(n => n.Title)
                            .Analyzer("custom_latin_transform")
                        )
                        .Text(t1 => t1
                            .Name(n1 => n1.Description)
                            .Analyzer("custom_latin_transform")
                        )
                    )
                )
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(az => az
                            .Custom("latin", azc => azc
                                .Tokenizer("keyword")
                                .Filters(new string[]{"custom_latin_transform"})
                            )
                        )
                        .TokenFilters(tf => tf
                            .IcuTransform("custom_latin_transform", icut => icut
                                .Id("Any-Latin; NFD; [:Nonspacing Mark:] Remove; NFC")
                            )
                        )
                    )
                )
            );
        }
    }
}
