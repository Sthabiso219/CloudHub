using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudHub.Models
{
    // SRS section 10 statuses.
    public enum SubscriptionStatus
    {
        Pending = 0,
        Active = 1,
        Expired = 2,
        Cancelled = 3,
        Suspended = 4
    }

    [Table("Subscriptions")]
    public class Subscription
    {
        [Key]
        public int SubscriptionId { get; set; }

        public int CustomerId { get; set; }

        public int HostingPlanId { get; set; }

        public SubscriptionStatus Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? RenewalDate { get; set; }       // null for the free plan

        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("HostingPlanId")]
        public virtual HostingPlan HostingPlan { get; set; }
    }

    // SRS rule: plan changes must be recorded.
    // Plan ids and names are copied here, not linked, so history survives plan edits.
    [Table("SubscriptionHistories")]
    public class SubscriptionHistory
    {
        [Key]
        public int SubscriptionHistoryId { get; set; }

        public int CustomerId { get; set; }

        public int? FromPlanId { get; set; }

        [StringLength(50)]
        public string FromPlanName { get; set; }

        public int ToPlanId { get; set; }

        [Required]
        [StringLength(50)]
        public string ToPlanName { get; set; }

        [StringLength(200)]
        public string Reason { get; set; }

        public DateTime ChangedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}