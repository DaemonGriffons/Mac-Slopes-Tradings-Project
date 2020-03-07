using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.PhotosViewModels
{
    public class PhotoEditViewModel

    { 
        public string Id { get; set; }
        [Required]
        [MaxLength(256, ErrorMessage = "Name cannot exceed {0} characters long")]
        public string Name { get; set; }
        [Required]
        [MaxLength(256, ErrorMessage = "Description cannot exceed {0} characters long")]
        public string Description { get; set; }
        [Required]
        [MaxLength(256, ErrorMessage = "Category cannot exceed {0} characters long")]
        public string Category { get; set; }

        public string PhotoUrl { get; set; }

        public IFormFile Image { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Facebook Handle cannot exceed {0} characters long")]
        public string FacebookLink { get; set; }
        [Required]
        [MaxLength(256, ErrorMessage = "Instagram Handle cannot exceed {0} characters long")]
        public string InstagramLink { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Twitter Handle cannot exceed {0} characters long")]
        public string TwitterLink { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
