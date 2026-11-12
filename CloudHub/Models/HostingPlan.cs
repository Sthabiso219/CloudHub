using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudHub.Models
{
    // SRS section 9: hosting plans. Administrators will manage these (UC20 to UC22),
    // so limits live in the database, not in code.
    [Table("HostingPlans")]
    public class HostingPlan
    {
        [Key]
        public int HostingPlanId { get; set; }

        [Required]
        [StringLength(50)]
        [Index("IX_HostingPlan_Name", IsUnique = true)]
        public string Name { get; set; }

        [StringLength(120)]
        public string Tagline { get; set; }

        [Range(0, 100000)]
        public decimal MonthlyPrice { get; set; }          // South African rand

        [Range(1, 1000)]
        public int MaxProjects { get; set; }

        [Range(0, 10000000)]
        public int StorageMb { get; set; }

        [Range(0, 1000)]
        public int MaxDatabases { get; set; }

        public bool HasSsl { get; set; }
        public bool HasDeploymentHistory { get; set; }

        // 0 = none, 1 = basic resource monitoring, 2 = application monitoring
        public int MonitoringLevel { get; set; }

        public bool HasBackups { get; set; }
        public bool HasPrioritySupport { get; set; }

        public bool IsFeatured { get; set; }               // "Most popular" badge
        public bool IsActive { get; set; }                 // inactive plans cannot be chosen
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Subscription> Subscriptions { get; set; }

        // Readable feature lines for registration, pricing and the dashboard.
        [NotMapped]
        public IList<string> FeatureList
        {
            get
            {
                var list = new List<string>();
                list.Add(MaxProjects == 1 ? "1 project" : MaxProjects + " projects");
                list.Add(StorageMb >= 1024
                    ? (StorageMb / 1024) + " GB storage"
                    : StorageMb + " MB storage");
                if (MaxDatabases > 0)
                    list.Add(MaxDatabases == 1 ? "1 database" : MaxDatabases + " databases");
                if (HasSsl) list.Add("SSL support");
                if (HasDeploymentHistory) list.Add("Deployment history");
                if (MonitoringLevel == 1) list.Add("Basic resource monitoring");
                if (MonitoringLevel == 2) list.Add("Application monitoring");
                if (HasBackups) list.Add("Backup allowance");
                if (HasPrioritySupport) list.Add("Priority support");
                return list;
            }
        }
    }
}