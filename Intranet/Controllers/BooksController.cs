using AutoMapper;
using Core.Flash;
using Data;
using Data.Model;
using Extranet.Models;
using Intranet.Models.Books;
using Intranet.Models.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Controllers
{
    public class BooksController : BaseController
    {
        private readonly IFlasher _flasher;
        private readonly IMapper _mapper;
        public BooksController(DatabaseContext databaseContext, IFlasher flasher, IMapper mapper) : base(databaseContext)
        {
            _flasher = flasher;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(CancellationToken cancelationToken)
        {
            var books = await _dbContext.AllActive<Book>().Include(row => row.Category).ToListAsync(cancelationToken);
            return View("Index", new BooksModel{ Books = books});
        }

        [Route("/Edit")]
        public async Task<IActionResult> Edit(CancellationToken cancelationToken, long id)
        {
            var entity = await _dbContext.Book.Include(x=>x.Category).FirstOrDefaultAsync(row=>row.Id == id, cancelationToken);
            if (entity == null)
            {
                return Error("Książka o podanym identyfikatorze nie istnieje");
            }

            var model = _mapper.Map<BookEditModel>(entity);
            model.CateogoriesList = GetCategoriesAsSelectList(model.CategoryId);

            return View("Edit", model);
        }

        [Route("/Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(CancellationToken cancelationToken, BookEditModel editModel)
        {
            if (ModelState.IsValid)
            {
                bool categoryByGivenNameAlreadyExists = await _dbContext.Book.AnyAsync(row => row.Name == editModel.Name && row.Id != editModel.Id, cancelationToken);
                if (categoryByGivenNameAlreadyExists)
                {
                    ModelState.AddModelError("Name", "Książka o takiej nazwie już istnieje");
                }
                else
                {
                    var editedBook = await _dbContext.Book.FindAsync(new object[] { editModel.Id }, cancelationToken);
                    if (editedBook == null)
                    {
                        return Error("Książka o podanym identyfikatorze nie istnieje");
                    }

                    string? pathToFile = editModel.Icon != null ? UploadFile(editModel.Icon, "books_images") : null;

                    editedBook.Name = editModel.Name;
                    editedBook.IconUrl = pathToFile ?? editedBook.IconUrl; //Podmień ścieżkę, tylko jesli przesłano nową ikonę
                    editedBook.Author = editModel.Author;
                    editedBook.Pages = editModel.Pages;
                    editedBook.Price = editModel.Price;
                    editedBook.Description = editModel.Description;
                    editedBook.Available = editModel.Available;
                    editedBook.Discount = editModel.Discount;
                    editedBook.Category =
                        await _dbContext.Category.FindAsync(new object[] { editModel.CategoryId}, cancelationToken);

                    try
                    {
                        await _dbContext.SaveChangesAsync(cancelationToken);
                        _flasher.Success("Książka została edytowana poprawnie", true);
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        _flasher.Danger("Wystapił błąd, przy próbie edycji kategorii", true);
                    }

                }
            }
            return View("Edit", editModel);
        }

        /// <summary>
        /// Uzupełnia listę kategorii, do dropdowna
        /// </summary>
        /// <param name="categoriesList"></param>
        private SelectList GetCategoriesAsSelectList(object selectedValue)
        {
            var categories = _dbContext.AllActive<Category>().ToList();
            return  new SelectList(categories, "Id", "Name", selectedValue);
        }

        [Route("/New")]
        public  IActionResult New()
        {
            var model = new BookInsertModel();
            model.CateogoriesList = GetCategoriesAsSelectList(null);
            return View("New", model);
        }

        [HttpPost]
        [Route("/New")]
        public async Task<IActionResult> New(CancellationToken cancelationToken, BookInsertModel insertModel)
        {
            if (ModelState.IsValid)
            {
                bool categoryByGivenNameAlreadyExists = await _dbContext.Book.AnyAsync(row => row.Name == insertModel.Name, cancelationToken);
                if (categoryByGivenNameAlreadyExists)
                {
                    ModelState.AddModelError("Name", "Książka o takiej nazwie już istnieje");
                }
                else
                {
                    string pathToFile = UploadFile(insertModel.Icon, "books_images");

                    var newBook = _mapper.Map<Book>(insertModel);
                    newBook.Category = await _dbContext.Category.FindAsync(new object[] {insertModel.CategoryId}, cancelationToken);
                    newBook.IconUrl = pathToFile;

                    try
                    {
                        await _dbContext.AddAsync(newBook, cancelationToken);
                        await _dbContext.SaveChangesAsync(cancelationToken);
                        _flasher.Success("Książka została dodana poprawnie", true);
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        _flasher.Danger("Wystapił błąd, przy próbie dodania Książka", true);
                    }

                }
            }
            insertModel.CateogoriesList = GetCategoriesAsSelectList(insertModel.CategoryId);
            return View("New", insertModel);
        }

        /// <summary>
        /// Usunięcie kategorii
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Remove(CancellationToken cancellationToken, long id)
        {
            //Bazowa funkcja do usuwania
            var deleteResult = TryDelete<Category>(cancellationToken, id, out string? message);
            if (deleteResult)
            {
                _flasher.Success("Kategoria została usunięta poprawnie", true);
            }
            else
            {
                _flasher.Danger("Wystapił błąd przy próbie usunięcia kategorii", true);
            }
            return RedirectToAction("Index");

        }

        /// <summary>
        /// Metoda pozwaljąca na usunięcie kategorii
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveByPost(CancellationToken cancellationToken, long id)
        {
            //Bazowa funkcja do usuwania
            var deleteResult = TryDelete<Category>(cancellationToken, id, out string? message);

            return Json(new
            {
                Success = deleteResult,
                Message = message ?? "Kategoria została usunięta poprawnie" //Message != null, gdy zawiera komunikat błędu
            });
        }
    }
}

