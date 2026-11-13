
using System;
using System.Collections.Generic;

namespace CloudHub.Models.ViewModels
{
    public class HostedApplicationViewModel
    {
        public int DeploymentId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Framework { get; set; }
        public string DeploymentMethod { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class HostedApplicationsPageViewModel
    {
        public List<HostedApplicationViewModel> Applications { get; set; }
            = new List<HostedApplicationViewModel>();
    }
}
