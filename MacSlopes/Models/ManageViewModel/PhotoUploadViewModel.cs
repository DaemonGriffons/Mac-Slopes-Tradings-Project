using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.ManageViewModel
{
    public class PhotoUploadViewModel
    {
        public string Thumbnail { get; set; }

        [Required(ErrorMessage = "Upload an Image First!...")]
        public IFormFile Image { get; set; }
    }
}
