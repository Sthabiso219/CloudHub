
using System.ComponentModel.DataAnnotations;

namespace CloudHub.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}
