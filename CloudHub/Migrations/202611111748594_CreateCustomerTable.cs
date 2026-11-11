namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCustomerTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        EmailAddress = c.String(nullable: false, maxLength: 150),
                        PasswordHash = c.String(nullable: false),
                        IsEmailVerified = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerId)
                .Index(t => t.EmailAddress, unique: true, name: "IX_Customer_Email");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Customers", "IX_Customer_Email");
            DropTable("dbo.Customers");
        }
    }
}
