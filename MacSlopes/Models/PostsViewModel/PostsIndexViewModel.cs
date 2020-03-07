using System;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.PostsViewModel
{
    public class PostsIndexViewModel
    {
        [Key]
        public string Id { get; set; }

        public string Author { get; set; }

        public string AuthorImage { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public string ImageUrl { get; set; }

        public string Body { get; set; }

        public string Link { get; set; }

        public DateTime DatePublished { get; set; }

        public bool IsPublished { get; set; }

        public int CommentCount { get; set; }

    }
}
