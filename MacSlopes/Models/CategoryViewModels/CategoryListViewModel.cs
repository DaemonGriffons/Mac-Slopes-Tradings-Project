using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PagedList.Core;

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
