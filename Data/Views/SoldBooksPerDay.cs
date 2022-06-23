using Microsoft.EntityFrameworkCore;

namespace Data.Views
{
    [Keyless]
    public class SoldBooksPerDay
    {
        public DateTime CreationDate { get; set; }
        public int SoldCount { get; set; }
    }
}
