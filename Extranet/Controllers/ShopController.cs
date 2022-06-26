
using Core.Flash;
using Data.Model;
using Extranet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Extranet.Controllers
{
    public class ShopController : BaseController
    {
        private readonly IFlasher _flasher;
        public ShopController(Data.DatabaseContext databaseContext, IFlasher flasher) : base(databaseContext)
        {
            _flasher = flasher;
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
        public async Task<IActionResult> BooksByCategory(CancellationToken cancelationToken, long? categoryId=null)
        {

            List<Book> books = new List<Book>();
            
            if(categoryId.HasValue)
                books = await _dbContext.AllActive<Book>().Include(row => row.Opinions).Where(row => row.Category.Id == categoryId).ToListAsync(cancelationToken);
            else
                books = await _dbContext.AllActive<Book>().Include(row => row.Opinions).ToListAsync(cancelationToken);

            

            foreach (var book in books)
            {
                var cos = await _dbContext.AllActive<Opinion>().Where(row => row.OpiniedBook.Id == book.Id).ToListAsync(cancelationToken);
                book.Stars = cos.Count == 0 ? 0 : (int)cos.Average(row => row.Stars);
                book.OpinionsCount = cos.Count();
            }

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
                .ThenInclude(row => row.Book)
                .AsNoTracking()
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
            new
            {
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
            if (item != null)
                _dbContext.Remove(item);
            await _dbContext.SaveChangesAsync(cancelationToken);
            return "Ok";
        }

        public async Task<IActionResult> PayForCart(CancellationToken cancelationToken)
        {
            var userCart = getUserCart();
            foreach (var cartItem in userCart.CartItems)
            {
                var savedBought = new BoughtBooks
                {
                    Book = await _dbContext.Book.FirstOrDefaultAsync(row => row.Id == cartItem.Book.Id, cancelationToken),
                    Customer = _loggedUser
                };

                var cartItemTmp = await _dbContext.CartItem.FirstOrDefaultAsync(row => row.Id == cartItem.Id, cancelationToken);
                cartItemTmp?.Delete();
                _dbContext.Add(savedBought);
            }
            var userCartTmp = await _dbContext.Cart.FirstOrDefaultAsync(row => row.Id == userCart.Id, cancelationToken);
            userCartTmp?.Delete();

            try
            {
                await _dbContext.SaveChangesAsync(cancelationToken);
                _flasher.Success("Zapłacono pomyslnie", true); ;
            }
            catch (Exception e)
            {
                _flasher.Danger("Nie udało się dokonać płatnoci", true);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Widok z wszystkimi kupionymi ksiązkami
        /// </summary>
        /// <param name="cancelationToken"></param>
        /// <returns></returns>
        [Route("/BoughtBooks")]
        public async Task<IActionResult> Distinct_AllBoughtBooksView(CancellationToken cancelationToken)
        {
            var alreadyOpiniedBooksByUser = await _dbContext.AllActive<Opinion>().Where(row => row.CreateBy.Id == _loggedUser.Id).Select(row => row.OpiniedBook.Id).ToListAsync(cancelationToken);
            //Każda zakupiona, kiedykolwiek przez użytkowika ksiązka, ale bez powieleń
            var model = new BoughtBooksModel
            {
                Books = _dbContext.AllActive<BoughtBooks>()
                    .Include(row => row.Book)
                    .Include(row => row.Customer)
                    .Where(row => row.Customer.Id == _loggedUser.Id)
                    .Where(row => !alreadyOpiniedBooksByUser.Contains(row.Book.Id))
                    .ToList()
                    .DistinctBy(row => row.Book.Id)
                    .ToList()
            };
            return BaseView("BoughtBooks", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddOpinion(CancellationToken cancelationToken, OpinionModel opinionMdeol)
        {

            var opinion = new Opinion
            {
                Stars = opinionMdeol.Stars,
                Description = opinionMdeol.Description,
                OpiniedBook = await _dbContext.Book.FirstOrDefaultAsync(row => row.Id == opinionMdeol.BookId, cancelationToken)
            };

            _dbContext.Add(opinion);
            await _dbContext.SaveChangesAsync(cancelationToken);

            _flasher.Success("Opinia została dodana");
            return RedirectToAction("Distinct_AllBoughtBooksView");
        }

        [HttpPost]
        public async Task<JsonResult> OpinionsForBook(CancellationToken cancelationToken, long bookId)
        {
            return Json(

                _dbContext.AllActive<Opinion>()
                            .Where(row => row.OpiniedBook.Id == bookId)
                            .Select(row =>
                            new
                            {
                                User = row.CreateBy.UserName,
                                Stars = row.Stars,
                                CreateDate = row.CreateDate,
                                Description = row.Description
                            })
                            .ToArray()
            );
        }

    }
}
