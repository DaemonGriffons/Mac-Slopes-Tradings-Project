using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.CategoryViewModels
{
    public class CategoryListViewModel
    {

        public string Search { get; set; }

        [Required(ErrorMessage = "A Category Tag is needed")]
        [Remote("ValidateCategory", "Categories")]
        public string CategoryCreate { get; set; }
        public PagedList<CategoryIndexViewModel> PagingCategories { get; set; }
    }
}
