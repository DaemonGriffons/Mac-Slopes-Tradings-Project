using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MacSlopes.Entities;
using MacSlopes.Models.PostsViewModel;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace MacSlopes.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _repository;
        private readonly UserManager<User> _userManager;

        public BlogController(IBlogRepository repository, UserManager<User> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Index([FromQuery]int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;

            var host = Request.Scheme + "://" + Request.Host;
            var posts = _repository.GetPosts().Select(x => new PostsIndexViewModel
            {
                Id = x.Id,
                Author = x.Author,
                AuthorImage=x.User.ImageUrl,
                Title = x.Title,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Category = x.CategoryId,
                Link = host + _repository.GetLink(x.Slug),
                DatePublished = x.DateCreated,
                CommentCount = x.Comments.Count
            });
            var model = new PostListViewModel
            {
                PostsIndexViewModels = new PagedList<PostsIndexViewModel>(
                    posts,
                    pageNumber,
                    12)
            };
            return View(model);
        }

        [HttpGet]
        [Route("/Blog/Categories/{Category}/")]
        [Route("/Blog/Categories/{Category}/{page:int?}")]
        public IActionResult Categories([FromQuery]string Category, [FromQuery]int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var host = Request.Scheme + "://" + Request.Host;

            //if (String.IsNullOrWhiteSpace(Category))
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            var posts = _repository.GetCategoryPosts(Category).Select(x => new PostsIndexViewModel
            {
                Id = x.Id,
                Author = x.Author,
                AuthorImage=x.User.ImageUrl,
                Title = x.Title,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Category = x.CategoryId,
                Link = host + _repository.GetLink(x.Slug),
                DatePublished = x.DateCreated,
                CommentCount = x.Comments.Count
            });
            var model = new PostListViewModel
            {
                PostsIndexViewModels = new PagedList<PostsIndexViewModel>(
                   posts,
                   pageNumber,
                   20)
            };
            ViewBag.Category = Category;
            return View(model);
        }

        [Route("/post/{slug}")]
        [HttpGet]
        public IActionResult Redirects(string slug)
        {
            return LocalRedirectPermanent($"/Blog/{slug}");
        }


        [HttpGet]
        [Route("/Blog/{slug}")]
        public IActionResult Post(string slug)
        {
            var post = _repository.GetPostBySlug(slug);
            if (post == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var model = new PostDetailsViewModel
            {
                Id = post.Id,
                Author = post.Author,
                AuthorImage = post.User.ImageUrl,
                Title = post.Title,
                Slug=post.Slug,
                Description = post.Description,
                Image = post.ImageUrl,
                DatePosted = post.DateCreated,
                Body = post.Body,
                //I wanted to make the CategoryId
                //Pull all the Categories with the Id that matched this one
                //Unfortunately due to me not yet knowing Linq I Could not
                //Thus I put the category name in place of category Id
                Category = post.CategoryId,
                Comments = post.Comments.OrderBy(c => c.DatePosted).ToList()
            };
            return View(model);

        }


        [HttpGet]
        [Route("Blog/Search")]
        public IActionResult Search([FromQuery]string Search,[FromQuery] int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;

            if (String.IsNullOrWhiteSpace(Search))
            {
                return RedirectToAction(nameof(Index));
            }

            var posts = _repository.SearchPosts(Search).Select(post => new PostsIndexViewModel
            {
                Id = post.Id,
                Author = post.Author,
                Title = post.Title,
                Description = post.Description,
                ImageUrl = post.ImageUrl,
                Link = _repository.GetLink(post.Slug),
                Category = post.CategoryId,
                DatePublished = post.DateCreated,
                CommentCount = post.Comments.Count,
                Body = post.Body,
                AuthorImage = post.User.ImageUrl
            });
            var model = new PostListViewModel
            {
                Search = Search,
                PostsIndexViewModels = new PagedList<PostsIndexViewModel>(posts, pageNumber, 12)
            };
            return View(model);
        }

        


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(CommentViewModel model)
        {
           
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Post), new { slug = model.PostId });
            
            var user = await _userManager.GetUserAsync(User);
            var post = _repository.GetPostBySlug(model.PostId);
            //Sanitizing User Input
            //Because someone might try to inject a script
            //That may execute on the client
            //And that would be very bad
            var encodedComment = HtmlEncoder.Create().Encode(model.Message);
            post.Comments = post.Comments ?? new List<Comment>();
            post.Comments.Add(new Comment
            {
                Id = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLowerInvariant(),
                Message = encodedComment,
                DatePosted = DateTime.Now,
                Username = user.UserName,
                Gravator = user.ImageUrl
            });
            _repository.UpdatePost(post);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Post), new { slug=post.Slug });
        }
    }
}