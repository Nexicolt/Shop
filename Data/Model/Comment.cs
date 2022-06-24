using Data.Model.Base;

namespace Data.Model
{
    public class Comment : FingerPrintEntity
    {
        public string Content { get; set; }
        public virtual Post? Post { get; set; }

        public bool Aproved { get; set; }
    }
}
