using Data;
using Data.Model;
using Intranet.Models.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Controllers
{
    public class ChartsController : BaseController
    {
        private readonly IServiceProvider _serviceProvider;

        public ChartsController(DatabaseContext databaseContext, IServiceProvider serviceProvider) : base(databaseContext)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task<IActionResult> Index(CancellationToken cancelationToken)
        {
            var booksSoldByDay = GetSellingForWholeMonth(cancelationToken);
            var bestSoldBooks = GetBestSoldBooks(cancelationToken);
            var soldByCategory = GetSoldPerCategory(cancelationToken);
            var bestReaders = GetBestReaders(cancelationToken);

            await Task.WhenAll(booksSoldByDay, bestSoldBooks, soldByCategory, bestReaders);

            var mainModel = new MainChartsModel
            {
                BooksSoldPerDay = booksSoldByDay.Result,
                BestSoldBooks = bestSoldBooks.Result,
                SoldByCategory = soldByCategory.Result,
                BestReaders = bestReaders.Result
            };

            return View("Index", mainModel);
        }

        /// <summary>
        /// Zwraca ilośc sprzedanych książek na dzień w zakresie 30 dni wstecz
        /// </summary>
        /// <param name="cancelationToken"></param>
        /// <returns></returns>
        private async Task<List<BooksSoldPerDay>> GetSellingForWholeMonth(CancellationToken cancelationToken)
        {
            var startDate = DateTime.Now.AddDays(-30);

            var selledBooks = await _dbContext.SoldBooksPerDay
                .Where(row => row.CreationDate >= startDate)
                .Select(row => new BooksSoldPerDay { Date = row.CreationDate.ToShortDateString(), SoldCount = row.SoldCount })
                .ToListAsync(cancelationToken);


            return selledBooks;
        }

        /// <summary>
        /// Zwraca ilośc sprzedanych książek na dzień w zakresie 30 dni wstecz
        /// </summary>
        /// <param name="cancelationToken"></param>
        /// <returns></returns>
        private async Task<List<BestDolsBooks>> GetBestSoldBooks(CancellationToken cancelationToken)
        {

            var tmpDbContext = new DatabaseContext(_dbContext.Options);
            return await tmpDbContext.BestSoldBooks
           .Select(row => new BestDolsBooks { Name = row.Name, SoldCount = row.SoldCount })
           .ToListAsync(cancelationToken);

        }
        private async Task<List<SoldByCategory>> GetSoldPerCategory(CancellationToken cancelationToken)
        {

            var tmpDbContext = new DatabaseContext(_dbContext.Options);
            return await tmpDbContext.SoldByCategory
           .Select(row => new SoldByCategory { Name = row.Name, SoldCount = row.SoldCount })
           .ToListAsync(cancelationToken);

        }

        private async Task<List<BestReaders>> GetBestReaders(CancellationToken cancelationToken)
        {

            var tmpDbContext = new DatabaseContext(_dbContext.Options);
            return await tmpDbContext.BestReaders
           .Select(row => new BestReaders { Username = row.Username, BoughtBooks = row.BoughtBooks })
           .ToListAsync(cancelationToken);

        }
    }
}
