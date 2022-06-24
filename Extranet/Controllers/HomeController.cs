using Core.Flash;
using Data;
using Data.Model;
using Extranet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Extranet.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        protected readonly IFlasher _flasher;

        public HomeController(ILogger<HomeController> logger, DatabaseContext databaseContext, IFlasher flasher) : base(databaseContext)
        {
            _logger = logger;
            _flasher = flasher;
        }

        /// <summary>
        /// Pobiera HTMl strony z bazy i wyświetla użytkownikowi
        /// </summary>
        /// <param name="siteId"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(CancellationToken cancellationToken, int siteId = 4)
        {

            //Pobierz żądaną stronę
            var page = await _dbContext.AllActive<Page>().FirstOrDefaultAsync(row => row.Id == siteId, cancellationToken);
            if (page is null)
            {
                return Error("Strona o podanym identyfikatorze nie istnieje");
            }

            List<Post> latestPosts = new List<Post>();
            if (siteId == 5)
            {
                latestPosts = await _dbContext.AllActive<Post>().Take(2).ToListAsync(cancellationToken);
            }

            return BaseView("Index",
                new MainModel
                {
                    HTML = page.HTML,
                    SiteId = siteId,
                    LatestPosts = latestPosts
                });

        }
    }
}