
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudHub.Models
{
    [Table("EmailVerificationTokens")]
    public class EmailVerificationToken
    {
        [Key]
        public int EmailVerificationTokenId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(64)]
        public string TokenHash { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UsedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
