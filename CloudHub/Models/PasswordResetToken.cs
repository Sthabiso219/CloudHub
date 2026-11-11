
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudHub.Models
{
    [Table("PasswordResetTokens")]
    public class PasswordResetToken
    {
        [Key]
        public int PasswordResetTokenId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(64)]
        [Index("IX_PasswordResetTokenHash", IsUnique = true)]
        public string TokenHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
