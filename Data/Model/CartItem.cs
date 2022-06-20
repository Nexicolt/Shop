using Data.Model.Base;

namespace Data.Model
{
    public class CartItem : BaseEntity
    {
        public virtual Book Book { get; set; }
        public virtual Cart Cart { get; set; }
        public uint Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
