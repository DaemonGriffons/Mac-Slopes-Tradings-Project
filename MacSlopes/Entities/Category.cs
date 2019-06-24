using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Entities
{
    public class Category
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}