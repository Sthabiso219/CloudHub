namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEmailVerificationTokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailVerificationTokens",
                c => new
                    {
                        EmailVerificationTokenId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        TokenHash = c.String(nullable: false, maxLength: 64),
                        ExpiresAt = c.DateTime(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UsedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.EmailVerificationTokenId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailVerificationTokens", "CustomerId", "dbo.Customers");
            DropIndex("dbo.EmailVerificationTokens", new[] { "CustomerId" });
            DropTable("dbo.EmailVerificationTokens");
        }
    }
}
