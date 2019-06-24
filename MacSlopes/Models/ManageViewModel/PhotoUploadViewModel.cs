using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MacSlopes.Models.ManageViewModel
{
    public class PhotoUploadViewModel
    {
        public string Thumbnail { get; set; }

        [Required(ErrorMessage = "Upload an Image First!...")]
        public IFormFile Image { get; set; }
    }
}
