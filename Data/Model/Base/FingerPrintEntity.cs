using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Model.Base
{
    public class FingerPrintEntity : TimeStampEntity
    {
        public User CreateBy { get; set; }

        [ForeignKey("CreateBy")]
        public string CreateById { get; set; }

        public User? EditBy { get; set; }

        [ForeignKey("EditBy")]
        public string? EditById { get; set; }

    }
}
