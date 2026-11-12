namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHostingPlansAndSubscriptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        HostingPlanId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        RenewalDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.HostingPlans", t => t.HostingPlanId)
                .Index(t => t.CustomerId)
                .Index(t => t.HostingPlanId);
            
            CreateTable(
                "dbo.HostingPlans",
                c => new
                    {
                        HostingPlanId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Tagline = c.String(maxLength: 120),
                        MonthlyPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxProjects = c.Int(nullable: false),
                        StorageMb = c.Int(nullable: false),
                        MaxDatabases = c.Int(nullable: false),
                        HasSsl = c.Boolean(nullable: false),
                        HasDeploymentHistory = c.Boolean(nullable: false),
                        MonitoringLevel = c.Int(nullable: false),
                        HasBackups = c.Boolean(nullable: false),
                        HasPrioritySupport = c.Boolean(nullable: false),
                        IsFeatured = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.HostingPlanId)
                .Index(t => t.Name, unique: true, name: "IX_HostingPlan_Name");
            
            CreateTable(
                "dbo.SubscriptionHistories",
                c => new
                    {
                        SubscriptionHistoryId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        FromPlanId = c.Int(),
                        FromPlanName = c.String(maxLength: 50),
                        ToPlanId = c.Int(nullable: false),
                        ToPlanName = c.String(nullable: false, maxLength: 50),
                        Reason = c.String(maxLength: 200),
                        ChangedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionHistoryId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            AddColumn("dbo.Customers", "TermsVersion", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionHistories", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Subscriptions", "HostingPlanId", "dbo.HostingPlans");
            DropForeignKey("dbo.Subscriptions", "CustomerId", "dbo.Customers");
            DropIndex("dbo.SubscriptionHistories", new[] { "CustomerId" });
            DropIndex("dbo.HostingPlans", "IX_HostingPlan_Name");
            DropIndex("dbo.Subscriptions", new[] { "HostingPlanId" });
            DropIndex("dbo.Subscriptions", new[] { "CustomerId" });
            DropColumn("dbo.Customers", "TermsVersion");
            DropTable("dbo.SubscriptionHistories");
            DropTable("dbo.HostingPlans");
            DropTable("dbo.Subscriptions");
        }
    }
}
