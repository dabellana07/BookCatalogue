using BookCatalogue.Contracts.Data.Repositories;
using BookCatalogue.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BookCatalogue.Data.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly BookCatalogueContext _context;

        public BookRepository(BookCatalogueContext context) : base(context)
        {
            _context = context;
        }

        public override Task<Book> GetAsync(int id)
        {
            return _context.Books.Where(b => b.Id == id).AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
