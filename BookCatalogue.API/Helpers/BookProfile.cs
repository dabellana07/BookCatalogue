using AutoMapper;
using BookCatalogue.API.DTO;
using BookCatalogue.ElasticSearch.Documents;

namespace BookCatalogue.API.Helpers
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Models.Book, BookDTO>();
            CreateMap<BookDTO, Models.Book>();
            CreateMap<Book, Models.Book>();
            CreateMap<Models.Book, Book>();
            CreateMap<Book, BookDTO>();
        }
    }
}
