
using System;
using System.Collections.Generic;

namespace CloudHub.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public int? TotalProjects { get; set; }
        public int? TotalDeployments { get; set; }
        public decimal? StorageUsedGB { get; set; }
        public decimal? StorageLimitGB { get; set; }
        public string CurrentPlan { get; set; }

        public List<DashboardActivityViewModel> RecentActivities
        { get; set; } = new List<DashboardActivityViewModel>();

        public decimal? StoragePercentage
        {
            get
            {
                if (!StorageUsedGB.HasValue ||
                    !StorageLimitGB.HasValue ||
                    StorageLimitGB.Value <= 0)
                {
                    return null;
                }

                return Math.Min(100,
                    Math.Round(
                        StorageUsedGB.Value /
                        StorageLimitGB.Value * 100, 0));
            }
        }
    }

    public class DashboardActivityViewModel
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActivityType { get; set; }
    }
}
