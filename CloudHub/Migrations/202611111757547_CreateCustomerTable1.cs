namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCustomerTable1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Customers", "LastName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Customers", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            DropColumn("dbo.Customers", "FullName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "FullName", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Customers", "PhoneNumber");
            DropColumn("dbo.Customers", "LastName");
            DropColumn("dbo.Customers", "FirstName");
        }
    }
}
