using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MacSlopes.Models.PostsViewModel
{
    public class PostCreateViewModel
    {
        [Required(ErrorMessage = "Post Title Is Required")]
        [MaxLength(120, ErrorMessage = "Post Title Must not Be Larger that {0} characters long!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Post Description is required")]
        [MaxLength(255, ErrorMessage = "Post Description Must not Be Larger than 255 characters long!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Post Description is required")]
        [MaxLength(5000000, ErrorMessage = "Post Body Must not Be Larger that {0} characters long!")]
        public string Body { get; set; }

        [Required(ErrorMessage = "Post Category is Required")]
        public string tag { get; set; }

        [Required(ErrorMessage ="Post Banner/Image is required")]
        public IFormFile Image { get; set; }

        [Display(Name="Publish Blog Post ?")]
        public bool Publish { get; set; }
    }
}
