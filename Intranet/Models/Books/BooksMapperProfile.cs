using Data.Model;

namespace Intranet.Models.Books
{
    public class BooksMapperProfile : AutoMapper.Profile
    {
        public BooksMapperProfile()
        {
            CreateMap<Book, BookEditModel>()
                .ForMember(src => src.CategoryId, opt => opt.MapFrom(dst => dst.Category.Id));
            CreateMap<BookInsertModel, Book>();
        }
    }
}
