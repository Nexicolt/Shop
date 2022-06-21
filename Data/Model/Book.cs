using System.ComponentModel.DataAnnotations.Schema;
using Data.Model.Base;

namespace Data.Model
{
    public class Book : FingerPrintEntity
    {
        public string Name { get; set; }
        public string Author { get; set; }

        public string IconUrl { get; set; }
        public string Description { get; set; }
        public virtual Category Category{ get; set; }

        public decimal Price { get; set; }

        [NotMapped]
        public decimal PriceAfterDiscount
        {
            get => Math.Round((Price/100) * (100 - Discount), 2);
        }
        public uint Pages { get; set; }
        public uint Discount { get; set; }
        public bool Available { get; set; }

        public ICollection<Opinion> Opinions { get; set; }

        [NotMapped]
        public int Stars { get; set; }

        [NotMapped]
        public int OpinionsCount { get; set; }


    }
}
