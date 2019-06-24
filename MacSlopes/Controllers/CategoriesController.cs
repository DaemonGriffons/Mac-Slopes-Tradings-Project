using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientNotifications;
using ClientNotifications.Helpers;
using MacSlopes.Entities;
using MacSlopes.Models.CategoryViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

namespace MacSlopes.Controllers
{
    [Authorize(Roles ="Admin,SuperUser")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryReporitory _repository;
        private readonly IClientNotification _clientNotification;

        public CategoriesController(ICategoryReporitory repository,IClientNotification clientNotification)
        {
            _repository = repository;
            _clientNotification = clientNotification;
        }

        [HttpGet]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var categories = _repository.GetCategories().Select(x => new CategoryIndexViewModel
            {
                Id = x.Id,
                Name = x.Name
            });

            var model = new CategoryListViewModel
            {
                PagingCategories = new PagedList<CategoryIndexViewModel>(categories, pageNumber, 10)
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Search() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Categories/Search/{search}/{page:int?}")]
        public IActionResult Search(string Search, [FromRoute]int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            if (String.IsNullOrWhiteSpace(Search))
            {
                return RedirectToAction(nameof(Index));
            }
            var results = _repository.Search(Search).Select(x => new CategoryIndexViewModel
            {
                Id = x.Id,
                Name = x.Name
            });
            var model = new CategoryListViewModel
            {
                Search = Search,
                PagingCategories = new PagedList<CategoryIndexViewModel>(results, pageNumber, 10)
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string CategoryCreate)
        {
            if (String.IsNullOrWhiteSpace(CategoryCreate))
            {
                return RedirectToAction(nameof(Index));
            }

            if (_repository.VerifyName(CategoryCreate))
            {
                ModelState.AddModelError("Error","Category name already exists");
                return RedirectToAction(nameof(Index));
            }
            var model = new Category
            {
                Id = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLowerInvariant(),
                Name = CategoryCreate
            };

            _repository.AddCategory(model);
            await _repository.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var category = _repository.GetCategory(id);
            if(category is null)
            {
                return RedirectToAction(nameof(Index));
            }
            var model = new CategoryIndexViewModel()
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
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Id = model.Id,
                    Name = model.Name
                };
                _repository.UpdateCategory(category);
                await _repository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            _clientNotification.AddToastNotification("You Have Made Error", NotificationHelper.NotificationType.error,
                new ToastNotificationOption
                {
                    NewestOnTop = true,
                    CloseButton = true,
                    PositionClass = "toast-top-full-width",
                    PreventDuplicates = true
                });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(string id)
        {
            try
            {
                var category = _repository.GetCategory(id);
                if(category is null)
                {
                    return NotFound();
                }
                _repository.DeleteCategory(category);
                await _repository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult ValidateCategory(string name)
        {
            if (_repository.VerifyName(name))
            {
                return Json($"The category named {name} alrealdy exists.try another one");
            }
            return Json(true);
        }
    }
}
