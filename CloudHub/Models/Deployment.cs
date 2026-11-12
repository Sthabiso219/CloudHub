using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudHub.Models
{
    public class Deployment
    {
        [Key]
        public int DeploymentId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(30)]
        public string DeploymentMethod { get; set; }

        [StringLength(250)]
        public string RepositoryName { get; set; }

        [StringLength(150)]
        public string BranchName { get; set; }

        [StringLength(250)]
        public string PackagePath { get; set; }

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        public string DeploymentLog { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
