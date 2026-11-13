namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPasswordResetTokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PasswordResetTokens",
                c => new
                    {
                        PasswordResetTokenId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        TokenHash = c.String(nullable: false, maxLength: 64),
                        CreatedAt = c.DateTime(nullable: false),
                        ExpiresAt = c.DateTime(nullable: false),
                        UsedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.PasswordResetTokenId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.TokenHash, unique: true, name: "IX_PasswordResetTokenHash");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PasswordResetTokens", "CustomerId", "dbo.Customers");
            DropIndex("dbo.PasswordResetTokens", "IX_PasswordResetTokenHash");
            DropIndex("dbo.PasswordResetTokens", new[] { "CustomerId" });
            DropTable("dbo.PasswordResetTokens");
        }
    }
}
