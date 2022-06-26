using Core.Flash;
using Data;
using Data.Model;
using Extranet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Extranet.Controllers
{
    public class PostsController : BaseController
    {
        private readonly IFlasher _flasher;
        public PostsController(DatabaseContext databaseContext, IFlasher flasher) : base(databaseContext)
        {
            _flasher = flasher;
        }

        [Route("/blog/{id}")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken, long id)
        {
            var post = await _dbContext.Post
                .Include(row => row.Comments)
                .ThenInclude(row => row.CreateBy)
                .FirstOrDefaultAsync(row => row.Id == id);
            if(post == null)
            {
                _flasher.Danger("Post o podanym identyfikatorze nie istnieje", true);
                return RedirectToAction("Index");
            }


            var visit = new Visit
            {
                Post = post
            };

            _dbContext.Add(visit);
            await _dbContext.SaveChangesAsync(cancellationToken);

            post.Comments = post.Comments.Where(x => x.Aproved).ToList();

            var model = new BlogModel
            {
                PostData = post
            };

            return BaseView("Index", model);
        }

        [HttpPost]
        public async Task<JsonResult> AddComment(CancellationToken cancelationToken, string comment, long postId)
        {
            var newComment = new Comment
            {
                Aproved = false,
                Content = comment,
                Post = _dbContext.Post.FirstOrDefault(row => row.Id == postId)
            };

            _dbContext.Add(newComment);
            await _dbContext.SaveChangesAsync(cancelationToken);

            return Json("ok");
        }
    }
}
