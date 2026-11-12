using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudHub.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProjectName { get; set; }

        [Required]
        [StringLength(50)]
        public string Framework { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Created";

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    }
}
