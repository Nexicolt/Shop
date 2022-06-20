using Data;
using Extranet.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Data.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Extranet.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly DatabaseContext _dbContext;
        protected  User? _loggedUser;


        protected BaseController(DatabaseContext databaseContext )
        {
            _dbContext = databaseContext;

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _loggedUser = _dbContext.User.Find(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            _dbContext.UserId = _loggedUser?.Id ?? null;
            base.OnActionExecuting(context);
        }

        [HttpPost]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string error = "")
        {
            return BaseView("Error", new ErrorModel { ErrorMessage = error });
        }

        private int getCartCount()
        {
            if (_loggedUser == null)
            {
                return 0;
            }

            
            var cart =  _dbContext.AllActive<Cart>().Include(row => row.CartItems)
                .Include(row => row.Owner)
                .Where(row => row.Owner.Id == _loggedUser.Id).FirstOrDefault();
            return cart == null ? 0 : cart.CartItems.Count();

        }

        /// <summary>
        /// Metoda powinna być wywoływana, zamiast wbudowanego "View"
        /// Uzupełnia listę stron, wyświetlanych w gónym menu
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected ViewResult BaseView(string viewName, BaseModel model)
        {

            model.Pages = _dbContext.AllActive<Page>().ToDictionary(key => (key.Id ?? -1), value => value.Name);
            model.CartItemsCount = getCartCount();
            return View(viewName, model);
        }


    }
}
