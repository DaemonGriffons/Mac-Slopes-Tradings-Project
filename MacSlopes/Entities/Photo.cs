using System;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Entities
{
    public class Photo
    {
        public Photo()
        {
            DateCreated = DateTime.Now;
        }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        [MaxLength(256)]
        public string Description { get; set; }

        [Required]
        [MaxLength(256)]
        public string Category { get; set; }

        [Required]
        public DateTime DateCreated { get; private set; }

        [Required]
        [MaxLength(1024)]
        public string PhotoUrl { get; set; }

        [Required]
        [MaxLength(256)]
        public string FaceBookLink { get; set; }
        [Required]
        [MaxLength(256)]
        public string InstagramLink { get; set; }
        [Required]
        [MaxLength(256)]
        public string TwitterLink { get; set; }
    }
}
