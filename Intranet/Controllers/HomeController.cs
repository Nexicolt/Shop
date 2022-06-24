using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(DatabaseContext databaseContext) : base(databaseContext) { }

        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            return RedirectPermanent("/Pages");
        }

    }
}