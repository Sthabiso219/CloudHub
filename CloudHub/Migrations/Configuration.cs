namespace CloudHub.Migrations
{
    using CloudHub.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CloudHub.Models.CloudHubDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CloudHub.Models.CloudHubDbContext context)
        {
            if (context.HostingPlans.Any()) return;

            DateTime now = DateTime.UtcNow;

            context.HostingPlans.AddRange(new[]
            {
        new HostingPlan
        {
            Name = "Starter", Tagline = "Try CloudHub for free",
            MonthlyPrice = 0m,
            MaxProjects = 1,                 // SRS: one small project
            StorageMb = 500,                // (assumed) SRS: limited storage
            MaxDatabases = 0,
            HasSsl = false, HasDeploymentHistory = false,
            MonitoringLevel = 1,            // basic resource monitoring
            DisplayOrder = 1, IsActive = true, CreatedAt = now
        },
        new HostingPlan
        {
            Name = "Developer", Tagline = "For students and side projects",
            MonthlyPrice = 99m,
            MaxProjects = 2, StorageMb = 2048,
            MaxDatabases = 0,
            HasSsl = true, HasDeploymentHistory = true,
            MonitoringLevel = 0,
            DisplayOrder = 2, IsActive = true, CreatedAt = now
        },
        new HostingPlan
        {
            Name = "Professional", Tagline = "For serious applications",
            MonthlyPrice = 249m,
            MaxProjects = 5, StorageMb = 10240,
            MaxDatabases = 1,               // (assumed) SRS: database allowance
            HasSsl = true, HasDeploymentHistory = true,
            MonitoringLevel = 2,            // application monitoring
            IsFeatured = true,
            DisplayOrder = 3, IsActive = true, CreatedAt = now
        },
        new HostingPlan
        {
            Name = "Business", Tagline = "For growing businesses",
            MonthlyPrice = 499m,
            MaxProjects = 20,               // (assumed) SRS: higher project limits
            StorageMb = 25600,
            MaxDatabases = 3,               // (assumed) SRS: database allowance
            HasSsl = true, HasDeploymentHistory = true,
            MonitoringLevel = 2,
            HasBackups = true, HasPrioritySupport = true,
            DisplayOrder = 4, IsActive = true, CreatedAt = now
        }
    });

            context.SaveChanges();
        }
    }
}
