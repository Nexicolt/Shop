using Data.Model;

namespace Intranet.Models.Blog
{
    public class BlogModelsMapper : AutoMapper.Profile
    {
        public BlogModelsMapper()
        {
            CreateMap<Post, PostModel>()
                .ForMember(src => src.AllVisits, opt => opt.MapFrom(dst => dst.Vitsits.Count))
                .ForMember(src => src.VisitsInCurrentMonth, opt => opt.MapFrom(dst => dst.Vitsits.Where(x => x.CreateDate.Month == DateTime.Now.Month).Count()))
                .ForMember(src => src.CommentsCount, opt => opt.MapFrom(dst => dst.Comments.Count));

            CreateMap<Post, BlogEditModel>()
                .ForMember(src => src.IsActive, opt => opt.MapFrom(dst => !dst.IsLocked));


            CreateMap<BlogEditModel, Post>()
                .ForMember(src => src.IsLocked, opt => opt.MapFrom(dst => !dst.IsActive));

        }
    }
}
