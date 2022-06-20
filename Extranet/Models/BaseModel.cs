namespace Extranet.Models
{
    public abstract class BaseModel
    {
        //Strony do nawigacji
        public Dictionary<long, string> Pages { get; set; }
        public int CartItemsCount { get; set; }

    }
}
