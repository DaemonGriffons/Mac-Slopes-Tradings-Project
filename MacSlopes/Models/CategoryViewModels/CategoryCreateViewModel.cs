using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MacSlopes.Models.CategoryViewModels
{
    public class CategoryCreateViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "A Category Tag is needed")]
        [Remote("ValidateCategory","Categories")]
        public string Name { get; set; }
    }
}
