using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientNotifications;
using ClientNotifications.Helpers;
using MacSlopes.Entities;
using MacSlopes.Models.CategoryViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace MacSlopes.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryReporitory _reporitory;
        private readonly IClientNotification _clientNotification;
        private readonly IMemoryCache _memoryCache;

        public CategoriesController(ICategoryReporitory reporitory, IClientNotification clientNotification, IMemoryCache memoryCache)
        {
            _reporitory = reporitory;
            _clientNotification = clientNotification;
            _memoryCache = memoryCache;
        }
        [HttpGet]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;

            var categories = _reporitory.GetCategories().Select(category => new CategoryIndexViewModel
            {
                Id = category.Id,
                Name = category.Name
            });

            if(!_memoryCache.TryGetValue("categories", out categories))
            {
                if (categories == null)
                {
                    categories = _reporitory.GetCategories().Select(category => new CategoryIndexViewModel
                    {
                        Id = category.Id,
                        Name = category.Name
                    });
                }

                _memoryCache.Set("categories", categories);
            }

            var model = new CategoryListViewModel
            {
                PagingCategories = new PagedList<CategoryIndexViewModel>(categories, pageNumber, 10)
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _clientNotification.AddToastNotification("You Have Made Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    PositionClass = "toast-top-right",
                    NewestOnTop = true,
                    PreventDuplicates = true
                });
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name
            };
            _reporitory.AddCategory(category);
            await _reporitory.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.Client, NoStore = false)]
        public IActionResult Edit(string Id)
        {
            var category = _reporitory.GetCategory(Id);
            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var model = new CategoryIndexViewModel
            {
                Id = category.Id,
                Name = category.Name
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryIndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _clientNotification.AddToastNotification("You Have Made Errors", NotificationHelper.NotificationType.error, new ToastNotificationOption
                {
                    PositionClass = "toast-top-right",
                    NewestOnTop = true,
                    PreventDuplicates = true
                });
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name
            };

            _reporitory.UpdateCategory(category);
            await _reporitory.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(string Id)
        {
            var category = _reporitory.GetCategory(Id);
            if (category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _reporitory.DeleteCategory(category);
            await _reporitory.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        [Route("/Categories/Search")]
        public IActionResult Search([FromQuery] string Search, [FromQuery] int? page)
        {
            if (String.IsNullOrWhiteSpace(Search))
            {
                return RedirectToAction(nameof(Index));
            }
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;

            var categories = _reporitory.Search(Search).Select(category => new CategoryIndexViewModel
            {
                Id = category.Id,
                Name = category.Name
            });

            ViewBag.Search = Search;
            var model = new CategoryListViewModel
            {
                Search = Search,
                PagingCategories = new PagedList<CategoryIndexViewModel>(categories, pageNumber, 10)
            };
            return View(model);
        }


        [HttpGet]
        public IActionResult ValidateCategory(string name)
        {
            if (_reporitory.VerifyName(name))
            {
                return Json($"The category named {name} alrealdy exists.try another one");
            }
            return Json(true);
        }
    }
}