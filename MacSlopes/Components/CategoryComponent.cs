using MacSlopes.Models.CategoryViewModels;
using MacSlopes.Models.ViewComponentsViewModels;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MacSlopes.Components
{
    public class CategoryComponent : ViewComponent
    {
        private readonly ICategoryReporitory _repo;

        public CategoryComponent(ICategoryReporitory repo)
        {
            _repo = repo;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _repo.GetCategories().Select(cat => new CategoryIndexViewModel
            {
                Id = cat.Id,
                Name = cat.Name
            }).Take(30).ToList();

            return View(categories);
        }
    }
}
