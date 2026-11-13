
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CloudHub.Models.ViewModels
{
    public class ZipDeploymentViewModel
    {
        [Required(ErrorMessage = "Please select a project.")]
        public int? ProjectId { get; set; }

        [Required(ErrorMessage = "Please select a ZIP file.")]
        public HttpPostedFileBase ZipFile { get; set; }
    }
}
