
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CloudHub.Models.ViewModels
{
    public class DeploymentPageViewModel
    {
        [Required(ErrorMessage = "Please select a project.")]
        [Display(Name = "Select Project")]
        public int? ProjectId { get; set; }

        [Required(ErrorMessage = "Select a deployment method.")]
        public string DeploymentMethod { get; set; }

        public string RepositoryName { get; set; }

        public string BranchName { get; set; }

        public IEnumerable<SelectListItem> Projects { get; set; }
            = new List<SelectListItem>();
    }
}
