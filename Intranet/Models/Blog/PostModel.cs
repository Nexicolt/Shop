using Microsoft.AspNetCore.Mvc;

namespace Intranet.Models.Blog
{
    public class PostModel : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string HTML { get; set; }


        public int AllVisits { get; set; }
        public int VisitsInCurrentMonth { get; set; }

        public int CommentsCount { get; set; }
        public bool IsLocked { get; set; }
    }
}
