using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.ManageViewModel
{
    public class IndexViewModel
    {
        [Required,MaxLength(256)]
        [ProtectedPersonalData]
        public string Name { get; set; }
        [Required, MaxLength(256)]
        [ProtectedPersonalData]
        public string Surname { get; set; }
        [Required]
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public bool IsPhoneConfirmed { get; set; }
    }
}
