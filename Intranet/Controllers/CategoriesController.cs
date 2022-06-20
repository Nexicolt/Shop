using Core.Flash;
using Data;
using Data.Model;
using Intranet.Models.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly IFlasher _flasher;
        public CategoriesController(DatabaseContext databaseContext, IFlasher flasher) : base(databaseContext)
        {
            _flasher = flasher;
        }

        public async Task<IActionResult> Index(CancellationToken cancelationToken)
        {
            var categories = await _dbContext.AllActive<Category>().ToListAsync(cancelationToken);
            return View("Index", categories);
        }

        public async Task<IActionResult> Edit(CancellationToken cancelationToken, long id)
        {
            var entity = await _dbContext.Category.FindAsync(new object[] {id}, cancelationToken);
            if (entity == null)
            {
                return Error("Kategoria o podanym identyfikatorze nie istnieje");
            }


            return View("Edit", new CategoryEditModel
            {
                Name = entity.Name,
                Id = entity.Id,
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CancellationToken cancelationToken, CategoryEditModel editModel)
        {
            if (ModelState.IsValid)
            {
                bool categoryByGivenNameAlreadyExists = await _dbContext.Category.AnyAsync(row => row.Name == editModel.Name && row.Id != editModel.Id, cancelationToken);
                if (categoryByGivenNameAlreadyExists)
                {
                    ModelState.AddModelError("Name", "Kategoria o takiej nazwie już istnieje");
                }
                else
                {
                    var editedCategory = await _dbContext.Category.FindAsync(new object[] {editModel.Id}, cancelationToken);
                    if (editedCategory == null)
                    {
                        return Error("Kategoria o podanym identyfikatorze nie istnieje");
                    }

                    string? pathToFile = editModel.Icon != null ? UploadFile(editModel.Icon, "category_images") : null;

                    editedCategory.Name = editModel.Name;
                    editedCategory.IconUrl = pathToFile ?? editedCategory.IconUrl; //Podmień ścieżkę, tylko jesli przesłano nową ikonę

                    try
                    {
                        await _dbContext.SaveChangesAsync(cancelationToken);
                        _flasher.Success("Kategoria została edytowana poprawnie", true);
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

        public async Task<IActionResult> New(CancellationToken cancelationToken)
        {
            return View("New", new CategoryInsertModel());
        }

        [HttpPost]
        public async Task<IActionResult> New(CancellationToken cancelationToken, CategoryInsertModel insertModel)
        {
            if (ModelState.IsValid)
            {
                bool categoryByGivenNameAlreadyExists = await _dbContext.Category.AnyAsync(row => row.Name == insertModel.Name, cancelationToken);
                if (categoryByGivenNameAlreadyExists)
                {
                    ModelState.AddModelError("Name", "Kategoria o takiej nazwie już istnieje");
                }
                else
                {
                    string pathToFile = UploadFile(insertModel.Icon, "category_images");
                    var newCategory = new Category
                    {
                        Name = insertModel.Name,
                        IconUrl = pathToFile
                    };
                    try
                    {
                        await _dbContext.AddAsync(newCategory, cancelationToken);
                        await _dbContext.SaveChangesAsync(cancelationToken);
                        _flasher.Success("Kategoria została dodana poprawnie", true);
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        _flasher.Danger("Wystapił błąd, przy próbie dodania kategorii", true);
                    }

                }
            }
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
