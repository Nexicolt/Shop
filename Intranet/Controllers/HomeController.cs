using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public HomeController(DatabaseContext databaseContext) : base(databaseContext) { }

        [Authorize]
        [HttpGet("main")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View();
        }

    }
}