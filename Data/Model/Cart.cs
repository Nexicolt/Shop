using Data.Model.Base;

namespace Data.Model
{
    public class Cart : TimeStampEntity
    {
        public virtual User Owner { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

    }
}
