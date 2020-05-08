using BookCatalogue.ElasticSearch.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookCatalogue.Contracts.ElasticSearch.Services
{
    public interface IBookElasticService
    {
        Task AddBook(Book book);
        Task RemoveBook(int book);
        Task UpdateBook(Book book);
        Task<IEnumerable<Book>> SearchBooks(string name);
    }
}
