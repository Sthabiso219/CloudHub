

namespace CloudHub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectSoftDelete : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "IsArchived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Projects", "ArchivedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "ArchivedAt");
            DropColumn("dbo.Projects", "IsArchived");
        }
    }
}
