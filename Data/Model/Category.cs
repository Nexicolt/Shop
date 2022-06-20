using Data.Model.Base;

namespace Data.Model
{
    public class Category : FingerPrintEntity
    {
        public string Name { get; set; }
        public string IconUrl { get; set; }

        public ICollection<Book> Books{ get; set; }
    }
}
