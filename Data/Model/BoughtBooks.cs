using Data.Model.Base;

namespace Data.Model
{
    public class BoughtBooks : TimeStampEntity
    {
        public virtual Book Book { get; set; }
        public virtual User Customer{ get; set; }
    }
}
