
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CloudHub.Models.ViewModels
{
    public class DeploymentPageViewModel
    {
        [Required(ErrorMessage = "Please select a project.")]
        public int? ProjectId { get; set; }

        public string DeploymentMethod { get; set; } = "GitHub";

        public string RepositoryName { get; set; }

        public string BranchName { get; set; }

        public IEnumerable<SelectListItem> Projects { get; set; }
            = new List<SelectListItem>();
    }
}
