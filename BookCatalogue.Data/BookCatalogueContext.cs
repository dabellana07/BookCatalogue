using BookCatalogue.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCatalogue.Data
{
    public class BookCatalogueContext : DbContext
    {
        public BookCatalogueContext(DbContextOptions<BookCatalogueContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
