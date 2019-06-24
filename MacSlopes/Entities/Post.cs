using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Entities
{
    public class Post
    {
        public Post()
        {
            Categories=new List<Category>();
            MainComments=new List<MainComment>();
        }
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Author { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Description { get; set; }

        [Required]
        [MaxLength(5000000)]
        public string Body { get; set; }

        [Required]
        [MaxLength(1024)]
        public string ImageUrl { get; set; }

        [Required]
        public DateTime DatePublished { get; set; }

        public bool IsPublished { get; set; }

        public ICollection<Category> Categories { get; set; }
        public string CategoryId { get; set; }


        public ICollection<MainComment> MainComments { get; set; }

    }
}
