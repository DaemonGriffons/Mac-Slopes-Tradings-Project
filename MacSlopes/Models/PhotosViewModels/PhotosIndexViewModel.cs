using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.PhotosViewModels
{
    public class PhotosIndexViewModel
    {
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


        public string DateCreated { get; set; }

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
