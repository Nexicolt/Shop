using Data.Model.Base;

namespace Data.Model
{
    public class Visit : TimeStampEntity
    {
        public virtual Post Post { get; set; }

    }
}
