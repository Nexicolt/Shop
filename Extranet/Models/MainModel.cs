using Data.Model;

namespace Extranet.Models
{
    public class MainModel : BaseModel
    {
        public long SiteId { get; set; }
        public string HTML { get; set; }

        public List<Post> LatestPosts { get; set; }
    }
}
