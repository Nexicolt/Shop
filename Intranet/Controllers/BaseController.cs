using Data;
using Data.Model.Base;
using Intranet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Intranet.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected readonly DatabaseContext _dbContext;

        protected BaseController(DatabaseContext databaseContext)
        {
            _dbContext = databaseContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _dbContext.UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            base.OnActionExecuting(context);
        }


        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string error = "")
        {
            return View("Error", new ErrorModel { ErrorMessage = error });
        }

        /// <summary>
        /// Głowna metoda do usuwania obiektów z bazy 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cancellationToken"></param>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost]
        public bool  TryDelete<T>(CancellationToken cancellationToken, long id, out string? message) where T : BaseEntity
        {
            bool success = true;


            var pageToRemove = _dbContext.Set<T>().Find(new object[] { id });
            if (pageToRemove == null)
            {
                success = false;
                message = "Zasób o podany identyfikatorze nie istnieje";
                goto END;
            }

            try
            {
                pageToRemove?.Delete();
                _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                success = false;
                message = "Wystapił błąd przy próbie usunięcia rekordu";
                goto END;

            }

            message = null;

        END:
            return success;
        }

        /// <summary>
        /// Uploaduje plik, przekazany z foormularza
        /// </summary>
        /// <param name="fileToUpload"></param>
        protected  string UploadFile(IFormFile fileToUpload, string whereToMove)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"../Extranet/wwwroot/upload/{whereToMove}");

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //get file extension
            FileInfo fileInfo = new FileInfo(fileToUpload.FileName);
            string fileName = RandomString(12) + fileInfo.Extension;

            string fileNameWithPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                fileToUpload.CopyTo(stream);
            }

            return $"upload/{whereToMove}/{fileName}";
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }



}
