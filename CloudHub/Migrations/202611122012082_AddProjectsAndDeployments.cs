namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectsAndDeployments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Deployments",
                c => new
                    {
                        DeploymentId = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        DeploymentMethod = c.String(nullable: false, maxLength: 30),
                        RepositoryName = c.String(maxLength: 250),
                        BranchName = c.String(maxLength: 150),
                        PackagePath = c.String(maxLength: 250),
                        Status = c.String(nullable: false, maxLength: 30),
                        CreatedAt = c.DateTime(nullable: false),
                        CompletedAt = c.DateTime(),
                        DeploymentLog = c.String(),
                    })
                .PrimaryKey(t => t.DeploymentId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        ProjectName = c.String(nullable: false, maxLength: 100),
                        Framework = c.String(nullable: false, maxLength: 50),
                        CreatedAt = c.DateTime(nullable: false),
                        Status = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.ProjectId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deployments", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Projects", new[] { "CustomerId" });
            DropIndex("dbo.Deployments", new[] { "ProjectId" });
            DropTable("dbo.Projects");
            DropTable("dbo.Deployments");
        }
    }
}
