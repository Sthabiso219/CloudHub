
using System;
using System.ComponentModel.DataAnnotations;

namespace CloudHub.Models.ViewModels
{
    public class CustomerProfileViewModel
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [StringLength(100)]
        [Display(Name = "Organisation Name")]
        public string OrganisationName { get; set; }

        [StringLength(50)]
        [Display(Name = "Primary Use")]
        public string PrimaryUse { get; set; }

        [StringLength(50)]
        [Display(Name = "Preferred Technology")]
        public string PreferredTechnology { get; set; }

        [StringLength(50)]
        [Display(Name = "Preferred Deployment Method")]
        public string PreferredDeploymentMethod { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
