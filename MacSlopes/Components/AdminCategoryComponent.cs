using MacSlopes.Models.ViewComponentsViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Components
{
    public class AdminCategoryComponent : ViewComponent
    {
        private readonly ICategoryReporitory _category;

        public AdminCategoryComponent(ICategoryReporitory category)
        {
            _category = category;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _category.GetCategories();
            var model = new UsersViewComponentViewModel
            {
                UserCount = categories.Count()
            };

            return View(model);
        }
    }
}
