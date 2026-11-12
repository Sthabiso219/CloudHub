using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudHub.Models
{
    [Table("Customers")]
    public class Customer
    {
        public Customer()
        {
            Subscriptions = new HashSet<Subscription>();
        }

        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Index("IX_Customer_Email", IsUnique = true)]
        public string EmailAddress { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsEmailVerified { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        // ---- Registration wizard ----

        [Required]
        [StringLength(20)]
        public string AccountType { get; set; }          // Individual, Student or Business

        [StringLength(100)]
        public string OrganisationName { get; set; }

        [StringLength(50)]
        public string PrimaryUse { get; set; }

        [StringLength(50)]
        public string PreferredTechnology { get; set; }

        [StringLength(50)]
        public string PreferredDeploymentMethod { get; set; }

        public bool MarketingConsent { get; set; }

        public DateTime? TermsAcceptedDate { get; set; }

        [StringLength(20)]
        public string TermsVersion { get; set; }          // e.g. 2026-11-12

        public virtual ICollection<Subscription> Subscriptions { get; set; }
    }
}