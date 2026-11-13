namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _new : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "AccountType", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.Customers", "OrganisationName", c => c.String(maxLength: 100));
            AddColumn("dbo.Customers", "PrimaryUse", c => c.String(maxLength: 50));
            AddColumn("dbo.Customers", "PreferredTechnology", c => c.String(maxLength: 50));
            AddColumn("dbo.Customers", "PreferredDeploymentMethod", c => c.String(maxLength: 50));
            AddColumn("dbo.Customers", "MarketingConsent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Customers", "TermsAcceptedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "TermsAcceptedDate");
            DropColumn("dbo.Customers", "MarketingConsent");
            DropColumn("dbo.Customers", "PreferredDeploymentMethod");
            DropColumn("dbo.Customers", "PreferredTechnology");
            DropColumn("dbo.Customers", "PrimaryUse");
            DropColumn("dbo.Customers", "OrganisationName");
            DropColumn("dbo.Customers", "AccountType");
        }
    }
}
