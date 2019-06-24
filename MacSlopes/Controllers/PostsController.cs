using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

namespace MacSlopes.Controllers
{
    [Authorize(Roles = "Admin,SuperUser")]
    public class PostsController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ICategoryReporitory _categoryReporitory;
        private readonly IFileManager _fileManager;
        private readonly UserManager<User> _userManager;
        private readonly IClientNotification _clientNotification;
        private PhotoSettings _options;

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
        [Route("/Posts/Index/{Search}")]
        [Route("/Posts/Index")]
        [Route("/Posts/Index/{page:int?}/")]
        [Route("Posts/{Search}/{page:int?}")]
        public IActionResult Index(int? page,string Search)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var host = Request.Scheme + "://" + Request.Host;
            var posts= string.IsNullOrWhiteSpace(Search)? _blogRepository.GetPosts().Select(x => new PostsIndexViewModel
            {
                Id = x.Id,
                Author = x.Author,
                Title = x.Title,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
                Link = host + _blogRepository.GetLink(x.Slug),
                DatePublished = x.DatePublished,
                IsPublished = x.IsPublished,
                CommentCount = x.MainComments.Count
            })
            : _blogRepository.SearchPosts(Search).Select(x => new PostsIndexViewModel
            {
                Author = x.Author,
                Body = x.Body,
                CommentCount = x.MainComments.Count,
                DatePublished = x.DatePublished,
                Description = x.Description,
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                IsPublished = x.IsPublished,
                Link = host + _blogRepository.GetLink(x.Slug),
                Title = x.Title
            });
            var model = new PostListViewModel
            {
                PostsIndexViewModels = new PagedList<PostsIndexViewModel>(posts, pageNumber, 20)
            };
            return View(model);
        }


        [HttpGet]
        [Route("/Posts/CreatePost")]
        public IActionResult CreatePost()
        {
            ViewBag.Categories = _categoryReporitory.GetCategories().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });
            return View();
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
                        CategoryId = model.tag,
                        Body = model.Body,
                        IsPublished = model.Publish
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
        [Route("/Posts/Post/{Id}")]
        public IActionResult Post(string Id)
        {
            var post = _blogRepository.GetPost(Id);
            var model = new PostDetailsViewModel
            {
                Id = post.Id,
                Author = post.Author,
                Title = post.Title,
                Description = post.Description,
                Image = post.ImageUrl,
                Body = post.Body,
                DatePosted = post.DatePublished,
                MainComments = post.MainComments,
                Categories = post.Categories.Where(x => x.Id.Equals(post.CategoryId))
                
            };
            return View(model);
        }

        [HttpGet]
        [Route("/Posts/Edit/{Id}")]
        public IActionResult Edit(string Id)
        {
            ViewBag.Categories = _categoryReporitory.GetCategories().Select(z => new SelectListItem
            {
                Value = z.Id.ToString(),
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
                Publish = post.IsPublished,
                CategoryId = post.CategoryId
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
                    IsPublished = model.Publish,
                    DatePublished = DateTime.Now,
                    CategoryId = model.CategoryId
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
        [ValidateAntiForgeryToken]
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
                return RedirectToAction(nameof(Post), new {id = model.PostId});

            var post = _blogRepository.GetPost(model.PostId);
            var user = await _userManager.GetUserAsync(User);
            var encodedComment = HtmlEncoder.Create().Encode(model.Message);
            if (model.MainCommentId==0)
            {
                post.MainComments = post.MainComments ?? new List<MainComment>();
                post.MainComments.Add(new MainComment
                {
                    Id = Guid.NewGuid().ToString().Replace("-",string.Empty).ToLowerInvariant(),
                    Message = encodedComment,
                    DatePosted = DateTime.Now,
                    Username = user.UserName,
                    Gravator = user.ImageUrl
                });
                _blogRepository.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    Id = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLowerInvariant(),
                    MainCommentId = model.MainCommentId,
                    Message = encodedComment,
                    DatePosted = DateTime.Now,
                    Username = user.UserName,
                    Gravator = user.ImageUrl,
                };
                _blogRepository.AddSubComment(subComment: comment);
            }

            await _blogRepository.SaveChangesAsync();
            return RedirectToAction(nameof(Post), new {id = model.PostId});
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Posts/Search/{search}/{page:int?}")]
        public IActionResult Search(string search,int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var host = Request.Scheme + "://" + Request.Host;
            if (String.IsNullOrWhiteSpace(search))
            {
                _clientNotification.AddToastNotification("You Have to Upload An Image!", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
                return View();
            }

            var result = _blogRepository.SearchPosts(search).Select(x => new PostsIndexViewModel
            {
                Author = x.Author,
                Body = x.Body,
                CommentCount = x.MainComments.Count,
                DatePublished = x.DatePublished,
                Description = x.Description,
                Id = x.Id,
                ImageUrl = x.ImageUrl,
                IsPublished = x.IsPublished,
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