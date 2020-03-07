using System;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Entities
{
    public class Category
    {
        public Category()
        {
            Id = Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
        [Key]
        public string Id { get; private set; }

        [Required]
        public string Name { get; set; }

    }
}