using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MacSlopes.Entities;

namespace MacSlopes.Models.PostsViewModel
{
    public class PostDetailsViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Author { get; set; }

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

        public ICollection<MainComment> MainComments { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}
