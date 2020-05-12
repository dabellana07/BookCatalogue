using AutoMapper;
using BookCatalogue.API.DTO;
using BookCatalogue.ElasticSearch.Documents;

namespace BookCatalogue.API.Helpers
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDTO>();
            CreateMap<BookDTO, Book>();
        }
    }
}
