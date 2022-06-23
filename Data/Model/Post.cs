using Data.Model.Base;

namespace Data.Model
{
    public class Post : FingerPrintEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string HTML { get; set; }

        public string Icon { get; set; }

        public ICollection<Visit> Vitsits { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
