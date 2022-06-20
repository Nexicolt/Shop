namespace Data.Model.Base
{
    public class TimeStampEntity : BaseEntity
    {

        public DateTime CreateDate { get; set; }

        public DateTime? EditDate { get; set; }
    }
}
