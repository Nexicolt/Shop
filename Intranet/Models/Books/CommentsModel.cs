using Data.Model;

namespace Intranet.Models.Books
{
    public class CommentsModel : BaseModel
    {
        public List<Opinion> Opinions{ get; set; }
    }
}
