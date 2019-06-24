using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MacSlopes.Models.PostsViewModel
{
    public class PostEditViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string CurrentImage { get; set; }

        public IFormFile NewImage { get; set; }

        public bool Publish { get; set; }
        public string Body { get; set; }

        public string CategoryId { get; set; }
    }
}
