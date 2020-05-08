using BookCatalogue.Contracts.ElasticSearch.Services;
using BookCatalogue.ElasticSearch.Documents;
using Nest;
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
        }

        public async Task<IEnumerable<Book>> SearchBooks(string name)
        {
            var searchResponse = await _client.SearchAsync<Book>(s => s
                .Index(IndexName)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Title)
                        .Query(name))));
            return searchResponse.Documents;
        }

        public Task AddBook(Book book)
        {
            return _client.IndexAsync(book, i => i.Index(IndexName));
        }

        public Task RemoveBook(int id)
        {
            return _client.DeleteAsync<Book>(id, i => i.Index(IndexName));
        }

        public Task UpdateBook(Book book)
        {
            return _client.UpdateAsync<Book>(book.Id, b => b
                .Index(IndexName)
                .Doc(book));
        }
    }
}
