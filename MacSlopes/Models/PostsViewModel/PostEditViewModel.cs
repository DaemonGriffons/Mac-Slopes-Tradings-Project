using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MacSlopes.Models.PostsViewModel
{
    public class PostEditViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CurrentImage { get; set; }

        public IFormFile NewImage { get; set; }
        public string Body { get; set; }

        public string CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
