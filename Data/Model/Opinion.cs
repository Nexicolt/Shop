using Data.Model.Base;

namespace Data.Model
{
    public class Opinion:FingerPrintEntity
    {
        public virtual Book OpiniedBook { get; set; }
        public int Stars { get; set; }
        public string Description { get; set; }
    }
}
