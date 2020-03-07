using MacSlopes.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.PostsViewModel
{
    public class PostDetailsViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Author { get; set; }

        public string AuthorImage { get; set; }

        public string Slug { get; set; }

        [Required]
        [MaxLength(120)]
        public string Title { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        [MaxLength(500000)]
        public string Body { get; set; }

        public string Link { get; set; }


        [Required]
        public string Image { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        public string Category { get; set; }

        public ICollection<Comment> Comments { get; set; }

    }
}
