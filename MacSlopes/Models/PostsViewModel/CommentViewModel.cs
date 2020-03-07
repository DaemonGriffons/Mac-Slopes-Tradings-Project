using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.PostsViewModel
{
    public class CommentViewModel
    {
        [Required]
        public string PostId { get; set; }
        [Required]
        public string Message { get; set; }

        public string CommentorImage { get; set; }
    }
}