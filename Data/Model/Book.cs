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
        public decimal PriceAfterDiscount
        {
            get => (Price/100) * (100 - Discount);
        }
        public uint Pages { get; set; }
        public uint Discount { get; set; }
        public bool Available { get; set; }


    }
}
