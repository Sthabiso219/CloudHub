using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudHub.Models.ViewModels
{
    public class ProjectFormViewModel
    {
        public int ProjectId { get; set; }
        [Required, StringLength(100)]
        [Display(Name = "Project name")]
        public string ProjectName { get; set; }
        [Required, StringLength(50)]
        public string Framework { get; set; }
    }
    public class ProjectListItemViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Framework { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DeploymentCount { get; set; }
        public string LatestDeploymentStatus { get; set; }
        public DateTime? LastDeploymentAt { get; set; }
    }
    public class ProjectsPageViewModel
    {
        public string Search { get; set; }
        public List<ProjectListItemViewModel> Projects { get; set; } = new List<ProjectListItemViewModel>();
    }
    public class ProjectDetailsViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Framework { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ProjectDeploymentItemViewModel> Deployments { get; set; } = new List<ProjectDeploymentItemViewModel>();
    }
    public class ProjectDeploymentItemViewModel
    {
        public int DeploymentId { get; set; }
        public string DeploymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
