using System.ComponentModel.DataAnnotations;

namespace MacSlopes.Models.AccountViewModels
{
    public class LoginWithRecoverCodeViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}
