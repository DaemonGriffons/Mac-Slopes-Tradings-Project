using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.PostsViewModel
{
    public class CommentViewModel
    {
        [Required]
        public string PostId { get; set; }
        [Required]
        public int MainCommentId { get; set; }
        [Required]
        public string Message { get; set; }
    }
}