using Data;
using Data.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Intranet.Controllers
{
    public class HomeController : BaseController
    {
        private readonly SignInManager<User> _signInManager;
        public HomeController(DatabaseContext databaseContext, SignInManager<User> signInManager) : base(databaseContext)
        {
            _signInManager = signInManager;
        }

        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            return RedirectPermanent("/Pages");
        }

        public async Task<IActionResult> Logout(CancellationToken cancelationToken)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }

    }
}