using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Entities
{
    public class Comment
    {
        [Key]
        public string Id { get; set; }

        public string Username { get; set; }

        public string Gravator { get; set; }

        public DateTime DatePosted { get; set; }

        public string Message { get; set; }
    }

    public class MainComment : Comment
    {
        public List<SubComment> SubComments { get; set; }
        
    }
    public class SubComment : Comment
    {
        public int MainCommentId { get; set; }
    }
}