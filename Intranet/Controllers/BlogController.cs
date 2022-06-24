using AutoMapper;
using Core.Flash;
using Data;
using Data.Model;
using Intranet.Models.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intranet.Controllers
{
    public class BlogController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IFlasher _flasher;
        public BlogController(DatabaseContext databaseContext, IMapper mapper, IFlasher flasher) : base(databaseContext)
        {
            _mapper = mapper;
            _flasher = flasher;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var posts = await _dbContext.AllActive<Post>()
                .Include(row => row.Vitsits)
                .Include(row => row.Comments)
                .ToListAsync(cancellationToken);
            var model = _mapper.Map<List<PostModel>>(posts);
            return View("Index", model);
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> New(CancellationToken cancelationToken)
        {
            return View("New", new BlogInsertModel());
        
        }

        [Route("[controller]/[action]")]
        [HttpPost]
        public async Task<IActionResult> Insert(CancellationToken cancelationToken,  BlogInsertModel insertModel)
        {
            if (ModelState.IsValid)
            {
                string pathToFile = UploadFile(insertModel.Thumbnail, "post_images");


                var newPost = new Post
                {
                    Name = insertModel.Name,
                    Description = insertModel.Description,
                    IsLocked = false,
                    HTML = insertModel.HTML,
                    Icon = pathToFile
                };

                _dbContext.Add(newPost);
                await _dbContext.SaveChangesAsync(cancelationToken);
                _flasher.Success("Post został dodany", true);
                return RedirectToAction("Index");

            }
            return View("New", insertModel);
        }

        public async Task<IActionResult> Edit(CancellationToken cancelationToken, long id)
        {
            var postToEdit = await _dbContext.AllActive<Post>().FirstOrDefaultAsync(row => row.Id == id, cancelationToken);
            if(postToEdit is null)
            {
                _flasher.Danger("Post o podanym identyfikatorze, nie istnieje", true);
                return RedirectToAction("Index");
            }

            var model = _mapper.Map<BlogEditModel>(postToEdit);
            return View("Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CancellationToken cancelationToken, BlogEditModel editModel)
        {
            var actuallPost = await _dbContext.Post.AsNoTracking().FirstOrDefaultAsync(row => row.Id == editModel.Id, cancelationToken);
            actuallPost.HTML = editModel.HTML;
            actuallPost.Name = editModel.Name;
            actuallPost.Description = editModel.Description;
            actuallPost.IsLocked = !editModel.IsActive;

            _dbContext.Update(actuallPost);
            await _dbContext.SaveChangesAsync(cancelationToken);
            _flasher.Success("Post został edytowany poprawnie", true);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Remove(CancellationToken cancelationToken, long id)
        {
            var post = await _dbContext.Post.FirstOrDefaultAsync(row => row.Id == id, cancelationToken);
            post.IsLocked = true;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync(cancelationToken);
            _flasher.Success("Post został usunięty", true);

            return RedirectToAction("Index");
        }

        [Route("[controller]/[action]/{postId}")]
        public async Task<IActionResult> Comments(CancellationToken cancelationToken, long postId)
        {
            var model = await _dbContext.Comment.Include(row => row.CreateBy).Where(row => row.Post.Id == postId).ToListAsync(cancelationToken);

            return View("Comments", model);
        }

        [Route("[controller]/[action]/{commentId}")]
        public async Task<IActionResult> AproveComment(CancellationToken cancelationToken, long commentId)
        {
            var comment = await _dbContext.Comment.Include(row => row.Post).FirstOrDefaultAsync(row => row.Id == commentId, cancelationToken);
            comment.Aproved = true;

            _dbContext.Update(comment);
            await _dbContext.SaveChangesAsync(cancelationToken);

            _flasher.Success("Komentarz został zaakceptowany");


            return RedirectToAction("Comments", new { postId=comment.Post.Id });
        }

        [Route("[controller]/[action]/{commentId}")]
        public async Task<IActionResult> RemoveComment(CancellationToken cancelationToken, long commentId)
        {
            var comment = await _dbContext.Comment.Include(row => row.Post).FirstOrDefaultAsync(row => row.Id == commentId, cancelationToken);
            comment.IsLocked = true;
            _dbContext.Update(comment);
            await _dbContext.SaveChangesAsync(cancelationToken);

            _flasher.Success("Komentarz został odrzucony");


            return RedirectToAction("Comments", new { postId = comment.Post.Id });
        }



    }
}
