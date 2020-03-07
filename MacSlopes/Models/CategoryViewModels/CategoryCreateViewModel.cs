using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
