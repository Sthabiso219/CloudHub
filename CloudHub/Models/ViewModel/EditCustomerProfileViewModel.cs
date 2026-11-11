
using System.ComponentModel.DataAnnotations;

namespace CloudHub.Models.ViewModels
{
    public class EditCustomerProfileViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Phone]
        [StringLength(30)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}
