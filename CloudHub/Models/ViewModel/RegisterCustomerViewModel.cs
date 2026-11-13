using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudHub.Models.ViewModels
{
    public class RegisterCustomerViewModel
    {
        public RegisterCustomerViewModel()
        {
            AccountType = "Individual";
        }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$",
            ErrorMessage = "Password needs upper and lowercase letters, a number and a special character.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        // ---- Plan and hosting step ----

        [Range(1, int.MaxValue, ErrorMessage = "Choose a plan.")]
        public int SelectedPlanId { get; set; }

        [Required(ErrorMessage = "Select an account type.")]
        [RegularExpression("^(Individual|Student|Business)$",
            ErrorMessage = "Select a valid account type.")]
        public string AccountType { get; set; }

        // Required for Business accounts; checked in the controller.
        [StringLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string OrganisationName { get; set; }

        [StringLength(50)]
        public string PrimaryUse { get; set; }

        [StringLength(50)]
        public string PreferredTechnology { get; set; }

        [StringLength(50)]
        public string PreferredDeploymentMethod { get; set; }

        // ---- Review step ----

        [Range(typeof(bool), "true", "true",
            ErrorMessage = "You must accept the Terms and Conditions.")]
        [Display(Name = "I accept the Terms and Conditions")]
        public bool AcceptTerms { get; set; }

        public bool MarketingConsent { get; set; }

        // ---- Not saved ----

        // Honeypot: hidden from people, so only bots fill it in.
        public string Website { get; set; }

        // Plans shown on the page, loaded from the database.
        public IList<HostingPlan> AvailablePlans { get; set; }
    }

    public class ResendVerificationViewModel
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}