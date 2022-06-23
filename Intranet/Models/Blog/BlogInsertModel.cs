using System.ComponentModel.DataAnnotations;

namespace Intranet.Models.Blog
{
    public class BlogInsertModel : BaseModel
    {
        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        public string HTML { get; set; }

        [Required(ErrorMessage = "Pole jest wymagane")]
        public IFormFile Thumbnail { get; set; }
    }

    public class BlogEditModel : BlogInsertModel
    {
        public new IFormFile? Thumbnail { get; set; }

        public bool IsActive { get; set; }
    }
}
