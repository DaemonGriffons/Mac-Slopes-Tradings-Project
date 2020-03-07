using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Enter Email Address")]
        [EmailAddress(ErrorMessage = "Enter Valid Email Address")]
        public string Email { get; set; }
    }
}
