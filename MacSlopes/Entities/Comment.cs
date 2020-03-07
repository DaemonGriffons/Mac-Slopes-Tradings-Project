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
}