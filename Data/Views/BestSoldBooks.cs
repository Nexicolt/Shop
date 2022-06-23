using Microsoft.EntityFrameworkCore;

namespace Data.Views
{
    [Keyless]
    public class BestSoldBooks
    {
        public string Name { get; set; }
        public int SoldCount { get; set; }
    }
}
