using Microsoft.EntityFrameworkCore;

namespace Data.Views
{
    [Keyless]
    public class SoldByCategory
    {
        public string Name { get; set; }
        public int SoldCount { get; set; }
    }
}
