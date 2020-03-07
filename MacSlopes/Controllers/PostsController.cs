using ClientNotifications;
using ClientNotifications.Helpers;
using MacSlopes.Entities;
using MacSlopes.Models.PostsViewModel;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MacSlopes.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICategoryReporitory _categoryReporitory;
        private readonly IFileManager _fileManager;
        private readonly UserManager<User> _userManager;
        private readonly IClientNotification _clientNotification;
        private readonly PhotoSettings _options;

        public PostsController(IBlogRepository blogRepository,
            ICategoryReporitory categoryReporitory,
            IFileManager fileManager,
            UserManager<User> userManager,
            IOptionsSnapshot<PhotoSettings> options,
            IClientNotification clientNotification)
        {
            _blogRepository = blogRepository;
            _categoryReporitory = categoryReporitory;
            _fileManager = fileManager;
            _userManager = userManager;
            _clientNotification = clientNotification;
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var host = Request.Scheme + "://" + Request.Host;
            var posts = _blogRepository.GetPosts().Select(x => new PostsIndexViewModel
            {
                Id = x.Id,
                Author = x.Author,
                AuthorImage=x.User.ImageUrl,
                Title = x.Title,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Link = host + _blogRepository.GetLink(x.Slug),
                DatePublished = x.DateCreated,
                CommentCount = x.Comments.Count
            });
            var model = new PostListViewModel
            {
                PostsIndexViewModels = new PagedList<PostsIndexViewModel>(posts, pageNumber, 20)
            };
            return View(model);
        }


        [HttpGet]
        public IActionResult CreatePost()
        {
            var model = new PostCreateViewModel
            {
                Categories = _categoryReporitory.GetCategories().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Name
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image == null)
                {
                    _clientNotification.AddToastNotification("You Have to Upload An Image!", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                    return View(model);
                }

                if (model.Image.Length > _options.MaxBytes)
                {
                    _clientNotification.AddToastNotification("Image File size Exceeded", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                    return View(model);
                }

                if (!_options.IsSupported(model.Image.FileName))
                {
                    _clientNotification.AddToastNotification("Invalid File Type", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-full-width",
                        PreventDuplicates = true
                    });
                }
                else
                {
                    var user = await _userManager.GetUserAsync(User);
                    var post = new Post
                    {
                        Id = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLowerInvariant(),
                        Author = user.Name + " " + user.Surname,
                        Title = model.Title,
                        Description = model.Description,
                        Slug = _blogRepository.CreateSlug(model.Title),
                        ImageUrl = _fileManager.SaveImage(model.Image),
                        CategoryId = model.Tag,
                        Body = model.Body,
                        UserId = user.Id
                    };
                    _blogRepository.AddPost(post);
                    await _blogRepository.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            _clientNotification.AddToastNotification("You Have Errors", NotificationHelper.NotificationType.error,new ToastNotificationOption
            {
                NewestOnTop = true,
                CloseButton = true,
                PositionClass = "toast-top-full-width",
                PreventDuplicates = true
            });
            return View(model);
        }


        [HttpGet]
        public IActionResult Post(string Id)
        {
            var post = _blogRepository.GetPost(Id);
            if (post == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var model = new PostDetailsViewModel
            {
                Id = post.Id,
                Author = post.Author,
                AuthorImage=post.User.ImageUrl,
                Title = post.Title,
                Description = post.Description,
                Image = post.ImageUrl,
                Body = post.Body,
                DatePosted = post.DateCreated,
                Comments = post.Comments,
                Category = post.CategoryId
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            var categories = _categoryReporitory.GetCategories().Select(z => new SelectListItem
            {
                Value = z.Name.ToString(),
                Text = z.Name
            }).ToList();
            var post = _blogRepository.GetPost(Id);
            var model = new PostEditViewModel()
            {
                Id = post.Id,
                CurrentImage = post.ImageUrl,
                Title = post.Title,
                Description = post.Description,
                Body = post.Body,
                CategoryId = post.CategoryId,
                Categories = categories
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var post = new Post
                {
                    Id = model.Id,
                    Author = user.Name + " " + user.Surname,
                    Title = model.Title,
                    Description = model.Description,
                    Slug = _blogRepository.CreateSlug(model.Title),
                    CategoryId = model.CategoryId,
                    UserId = user.Id
                };

                if (model.NewImage==null)
                {
                    post.ImageUrl = model.CurrentImage;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(model.CurrentImage))
                    {
                        _fileManager.RemoveImage(model.CurrentImage);
                    }

                    post.ImageUrl = _fileManager.SaveImage(model.NewImage);
                }

                _blogRepository.UpdatePost(post);
                if (await _blogRepository.SaveChangesAsync())
                {
                    return RedirectToAction(nameof(Index));
                }

                _clientNotification.AddToastNotification("Could not Update post", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
                return View(model);
            }
            _clientNotification.AddToastNotification("You Have Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
            {
                NewestOnTop = true,
                CloseButton = true,
                PositionClass = "toast-top-full-width",
                PreventDuplicates = true
            });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            _blogRepository.RemovePost(Id);
            await _blogRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Post), new {id = model.PostId});
            }

            var post = _blogRepository.GetPost(model.PostId);
            var user = await _userManager.GetUserAsync(User);

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
            _blogRepository.UpdatePost(post);
            await _blogRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Post), new {id = model.PostId});
        }


        [HttpGet]
        public IActionResult Search([FromQuery]string search,[FromQuery]int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var host = Request.Scheme + "://" + Request.Host;
            if (String.IsNullOrWhiteSpace(search))
            {
                return RedirectToAction(nameof(Index));
            }

            var result = _blogRepository.SearchPosts(search).Select(x => new PostsIndexViewModel
            {
                Author = x.Author,
                AuthorImage=x.User.ImageUrl,
                Body = x.Body,
                CommentCount = x.Comments.Count,
                DatePublished = x.DateCreated,
                Description = x.Description,
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                Link = host + _blogRepository.GetLink(x.Slug),
                Title = x.Title
            });

            var model = new PostListViewModel
            {
                Search = search,
                PostsIndexViewModels = new PagedList<PostsIndexViewModel>(result, pageNumber, 15)
            };

            return View(model);
        }

        [HttpPost]
        [Route("/Posts/Post/{PostId}/{CommentId}")]
        public async Task<IActionResult> DeleteComment(string PostId, string CommentId)
        {
            var comment = _blogRepository.GetPost(PostId).Comments.FirstOrDefault(c => c.Id.Equals(CommentId));
            if (comment==null)
            {
                return RedirectToAction(nameof(Post), new { Id = PostId });
            }
            _blogRepository.RemoveComment(comment);
            await _blogRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Post), new { Id = PostId });
        }


        [HttpPost]
        public IActionResult Upload(IFormFile imageUpload)
        {
            var filename = _fileManager.SaveImage(imageUpload);
            return base.Json(new
            {
                path = "/Uploads/" + filename + "/"
            });
        }
    }
}