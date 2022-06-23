namespace Intranet.Models.Charts
{
    public class MainChartsModel
    {
        public List<BooksSoldPerDay> BooksSoldPerDay { get; set; }
        public List<BestDolsBooks> BestSoldBooks { get; set; }
        public List<SoldByCategory> SoldByCategory { get; set; }
        public List<BestReaders> BestReaders { get; set; }
    }

    public class BooksSoldPerDay
    {
        public int SoldCount { get; set; }
        public string Date{ get; set; }
    }

    public class BestDolsBooks
    {
        public int SoldCount { get; set; }
        public string Name { get; set; }
    }

    public class SoldByCategory
    {
        public string Name { get; set; }
        public int SoldCount { get; set; }
    }

    public class BestReaders
    {
        public string Username { get; set; }
        public int BoughtBooks { get; set; }

    }

}
