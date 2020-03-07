using ClientNotifications;
using ClientNotifications.Helpers;
using MacSlopes.Entities;
using MacSlopes.Models.PhotosViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using PagedList.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Controllers
{
    [Authorize(Roles="Admin")]
    [Route("[controller]/[action]")]
    public class PhotosController : Controller
    {
        private readonly IPhoto _photoRepo;
        private readonly ICategoryReporitory _categoryRepo;
        private readonly IClientNotification _clientNotification;
        private readonly IFileManager _fileManager;
        private readonly PhotoSettings _options;

        public PhotosController(IPhoto photoRepo,
            ICategoryReporitory categoryRepo, IClientNotification clientNotification,
            IFileManager fileManager, IOptionsSnapshot<PhotoSettings> options)
        {
            _photoRepo = photoRepo;
            _categoryRepo = categoryRepo;
            _clientNotification = clientNotification;
            _fileManager = fileManager;
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var photos = _photoRepo.GetAllPhotos()
                .Select(photo => new PhotosIndexViewModel
                {
                    Id = photo.Id,
                    Name = photo.Name,
                    Description = photo.Description,
                    Category = photo.Category,
                    PhotoUrl = photo.PhotoUrl,
                    DateCreated = photo.DateCreated.ToLongDateString(),
                    FaceBookLink = photo.FaceBookLink,
                    InstagramLink = photo.InstagramLink,
                    TwitterLink = photo.TwitterLink
                });

            //var model = new PagedList<PhotosIndexViewModel>(photos, pageNumber, 12);

            var model = new PhotosListViewModel
            {
                Photos = new PagedList<PhotosIndexViewModel>(photos, pageNumber, 12)
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _categoryRepo.GetCategories().Select(cat => new SelectListItem
            {
                Text = cat.Name,
                Value = cat.Name,
            }).ToList();

            var model = new PhotoCreateViewModel
            {
                Categories = categories
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhotoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image == null)
                {
                    _clientNotification.AddToastNotification("You Have to Upload An Image!", NotificationHelper.NotificationType.error, new ToastNotificationOption
                    {
                        NewestOnTop = true,
                        CloseButton = true,
                        PositionClass = "toast-top-right",
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
                        PositionClass = "toast-top-right",
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
                        PositionClass = "toast-top-right",
                        PreventDuplicates = true
                    });
                }
                else
                {
                    var photo = new Photo
                    {
                        Id= Guid.NewGuid().ToString().Replace("-", string.Empty),
                        Name = model.Name,
                        Description = model.Description,
                        Category = model.Category,
                        PhotoUrl = _fileManager.SaveImage(model.Image),
                        FaceBookLink = model.FacebookLink,
                        InstagramLink = model.InstagramLink,
                        TwitterLink = model.TwitterLink
                    };
                    await _photoRepo.AddPhoto(photo);
                    await _photoRepo.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            _clientNotification.AddToastNotification("You Have Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
            {
                NewestOnTop = true,
                CloseButton = true,
                PositionClass = "toast-top-right",
                PreventDuplicates = true
            });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditPhoto(string Id)
        {
            var photo = await _photoRepo.GetPhoto(Id);

            var categories = _categoryRepo.GetCategories().Select(cat => new SelectListItem
            {
                Text = cat.Name,
                Value = cat.Name
            }).ToList();

            if (photo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = new PhotoEditViewModel
            {
                Name = photo.Name,
                Description = photo.Description,
                PhotoUrl = photo.PhotoUrl,
                Category = photo.Category,
                FacebookLink = photo.FaceBookLink,
                InstagramLink = photo.InstagramLink,
                TwitterLink = photo.TwitterLink,
                Categories = categories
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPhoto(PhotoEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var photo = new Photo
                {
                    Id=model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Category = model.Category,
                    FaceBookLink = model.FacebookLink,
                    InstagramLink = model.InstagramLink,
                    TwitterLink = model.TwitterLink
                };
                if (model.Image == null)
                {
                    photo.PhotoUrl = model.PhotoUrl;
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(model.PhotoUrl))
                    {
                        _fileManager.RemoveImage(model.PhotoUrl);
                    }
                    photo.PhotoUrl = _fileManager.SaveImage(model.Image);
                    
                }

                await _photoRepo.UpdatePhoto(photo);
                if(await _photoRepo.SaveChangesAsync())
                {
                    return RedirectToAction(nameof(Index));
                }
                _clientNotification.AddToastNotification("Could not Update Photo Item", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-right",
                    PreventDuplicates = true
                });
                return View(model);
            }
            _clientNotification.AddToastNotification("You Have Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
            {
                NewestOnTop = true,
                CloseButton = true,
                PositionClass = "toast-top-right",
                PreventDuplicates = true
            });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PhotoDetails(string Id)
        {
            var photo = await _photoRepo.GetPhoto(Id);
            if (photo == null)
                return RedirectToAction(nameof(Index));
            var model = new PhotosIndexViewModel
            {
                Name = photo.Name,
                Description = photo.Description,
                Category = photo.Category,
                DateCreated = photo.DateCreated.ToLongDateString(),
                FaceBookLink = photo.FaceBookLink,
                InstagramLink = photo.InstagramLink,
                TwitterLink = photo.TwitterLink,
                PhotoUrl = photo.PhotoUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(string Id)
        {
            var photo = await _photoRepo.GetPhoto(Id);
            _fileManager.RemoveImage(photo.PhotoUrl);
            await _photoRepo.RemovePhoto(photo);
            await _photoRepo.SaveChangesAsync();
                
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Search([FromQuery] string Search, [FromQuery] int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var photos= _photoRepo.SearchPhotos(Search).Select(photo => new PhotosIndexViewModel
            {
                Id = photo.Id,
                Name = photo.Name,
                Description = photo.Description,
                Category = photo.Category,
                PhotoUrl = photo.PhotoUrl,
                DateCreated = photo.DateCreated.ToLongDateString(),
                FaceBookLink = photo.FaceBookLink,
                InstagramLink = photo.InstagramLink,
                TwitterLink = photo.TwitterLink
            });

            var model = new PhotosListViewModel
            {
                Search = Search,
                Photos = new PagedList<PhotosIndexViewModel>(photos, pageNumber, 12)
            };

            return View(model);
        }
    }
}