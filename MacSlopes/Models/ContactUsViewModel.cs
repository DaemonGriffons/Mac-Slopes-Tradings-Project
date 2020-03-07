using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MacSlopes.Models
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage ="Your name is required")]
        [Display(Name ="Your Name")]
        public string Name { get; set; }

        [Required(ErrorMessage ="Your Email is required")]
        [Display(Name ="Email Address")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage ="Your Messge title is required")]
        [Display(Name = "Message Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Your Message is required")]
        [Display(Name = "Your Message")]
        public string Message { get; set; }
    }
}
