
using Data.Model;
using Extranet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Extranet.Controllers
{
    public class ShopController : BaseController
    {
        public ShopController(Data.DatabaseContext databaseContext) : base(databaseContext)
        {
        }

        public async Task<IActionResult> Index(CancellationToken cancelationToken)
        {
            var model = new CategoryModel()
            {
                Categories = await _dbContext.AllActive<Category>().ToListAsync(cancelationToken)
            };


            return BaseView("Index", model);
        }

        /// <summary>
        /// Widok wyświetlający ksiązki, w podanej kategori
        /// </summary>
        /// <param name="cancelationToken"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/books/{categoryId}")]
        public async Task<IActionResult> BooksByCategory(CancellationToken cancelationToken, long? categoryId)
        {
            var books = await _dbContext.AllActive<Book>().Where(row => row.Category.Id == categoryId).ToListAsync(cancelationToken);

            var model = new BooksModel
            {
                Books = books
            };
            return BaseView("BooksList", model);
        }

        private Cart getUserCart()
        {
            var exsistCart = _dbContext.AllActive<Cart>().FirstOrDefault(row => row.Owner.Id == _loggedUser.Id);
            if (exsistCart == null)
            {
                var newCart = new Cart
                {
                    Owner = _loggedUser
                };
                _dbContext.Add(newCart);
                _dbContext.SaveChanges();
            }
            return _dbContext.AllActive<Cart>()
                .Include(row => row.CartItems)
                .ThenInclude(row=>row.Book)
                .FirstOrDefault(row => row.Owner.Id == _loggedUser.Id);

        }

        [HttpPost]
        public async Task<JsonResult> AddItemToCart(CancellationToken cancelationToken, long bookId)
        {
            bool newItemAdded = false;
            var userCart = getUserCart();
            var itemInCart = userCart.CartItems.FirstOrDefault(x => x.Book.Id == bookId);
            if (itemInCart == null)
            {
                var book = _dbContext.Book.FirstOrDefault(row => row.Id == bookId);
                var newItemToCart = new CartItem()
                {
                    Book = book,
                    Cart = _dbContext.Cart.FirstOrDefault(row => row.Id == userCart.Id),
                    Quantity = 1,
                    Price = book.PriceAfterDiscount
                };
                newItemAdded = true;
                await _dbContext.AddAsync(newItemToCart, cancelationToken);
            }
            else
            {
                itemInCart.Quantity += 1;
                _dbContext.Update(itemInCart);
            }

            try
            {
                await _dbContext.SaveChangesAsync(cancelationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //Zwraca info, czy dodano nowy przedmiot, czy zwiększono ilość już istniejącego
            return Json(
            new {
                item_added = newItemAdded
            });

        }

        public async Task<IActionResult> Cart(CancellationToken cancelationToken)
        {
            var model = new CartModel();
            model.CartItems = getUserCart().CartItems.ToList();
            return BaseView("Cart", model);
        }


        public async Task<JsonResult> DecreaseQuantity(CancellationToken cancelationToken, long bookId)
        {
            var userCart = getUserCart();
            var itemInCart = userCart.CartItems.FirstOrDefault(x => x.Book.Id == bookId);
            if (itemInCart?.Quantity == 1)
            {
                return Json(new
                {
                    succesfull = false,
                    error = "Nie można ustawić ilośc, poniżej 1"
                });
            }
            else
            {
                itemInCart.Quantity -= 1;
                _dbContext.Update(itemInCart);
                await _dbContext.SaveChangesAsync(cancelationToken);
                return Json(new
                {
                    succesfull = true
                });
            }
        }

        public async Task<string> RemoveItemFromCart(CancellationToken cancelationToken, long bookId)
        {
            var usrCart = getUserCart();
            var item = usrCart.CartItems.FirstOrDefault(x => x.Book.Id == bookId);
            item?.Delete();
            if(item != null)
                _dbContext.Remove(item);
            await _dbContext.SaveChangesAsync(cancelationToken);
            return "Ok";
        }

    }
}
