using Core.Flash;
using Data;
using Data.Model;
using Intranet.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Controllers
{
    public class PagesController : BaseController
    {
        private readonly IFlasher _flasher;
        public PagesController(DatabaseContext databaseContext, IFlasher flasher) : base(databaseContext)
        {
            _flasher = flasher;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _dbContext.AllActive<Page>()
                                .Include(row => row.CreateBy)
                                .ToListAsync(cancellationToken);
            return View("Index", model);
        }

        /// <summary>
        /// Widok formularza z edycją strony
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(CancellationToken cancellationToken, long id)
        {
            var pageToEdit = await _dbContext.AllActive<Page>().FirstOrDefaultAsync(row => row.Id == id, cancellationToken);
            if (pageToEdit == null)
            {
                return Error("Strona o podanym identyfikatorze, nie istnieje");
            }

            return View("Edit", pageToEdit);
        }

        /// <summary>
        /// Edycja strony, po wysłąniu formularza
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="editModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(CancellationToken cancellationToken, PageEditModel editModel)
        {

            var editedPage = await _dbContext.Page.FindAsync(new object[] { editModel.Id }, cancellationToken);
            if (editedPage is null)
            {
                return Error("Strona o podanym identyfikatorze, nie istnieje");
            }

            editedPage.Name = editModel.Name;
            editedPage.HTML = editModel.HTML;
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                _flasher.Danger("Wystąpił błąd, podczas próby edycji", true);
                return RedirectToAction("Edit", new { Id = editModel.Id });
            }

            _flasher.Success("Strona została edytowana poprawnie", true);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Widok formularza, do stworzenia nowej strony
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public  IActionResult New() => View("New", new PageInsertModel());

        /// <summary>
        /// Dodawanie nowej strony
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> New(CancellationToken cancellationToken, PageInsertModel insertModel)
        {
            bool pageWithGivenNameAlreadyExists = await _dbContext.Page.AnyAsync(row => row.Name.ToLower() == insertModel.Name.ToLower(), cancellationToken);
            if (pageWithGivenNameAlreadyExists)
            {
                _flasher.Danger("Strona o takiej nazwie, już istnieje", true);
                return View("New", insertModel);
            }

            var insertingPage = new Page
            {
                Name = insertModel.Name,
                HTML = insertModel.HTML,
            };

            try
            {
                await _dbContext.AddAsync(insertingPage, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _flasher.Success("Strona została dodana poprawnie", true);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                _flasher.Danger("Wystąpił błąd, podczas próby dodania strony", true);
                return View("New", insertModel);
            }
        }

        /// <summary>
        /// Usunięcie strony
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Remove(CancellationToken cancellationToken, long id)
        {
            //Bazowa funkcja do usuwania
            var deleteResult = TryDelete<Page>(cancellationToken, id, out string? message);
            if (deleteResult)
            {
                _flasher.Success("Strona została usunięta poprawnie", true);
            }
            else
            {
                _flasher.Danger("Wystapił błąd przy próbie usunięcia strony", true);
            }
            return RedirectToAction("Index");

        }

        /// <summary>
        /// Metoda pozwaljąca na usunięcie strony
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> RemoveByPost(CancellationToken cancellationToken, long id)
        {
            //Bazowa funkcja do usuwania
            var deleteResult = TryDelete<Page>(cancellationToken, id, out string? message);

            return Json(new
            {
                Success = deleteResult,
                Message = message ?? "Strona została usunięta poprawnie" //Message != null, gdy zawiera komunikat błędu
            });
        }


    }
}
