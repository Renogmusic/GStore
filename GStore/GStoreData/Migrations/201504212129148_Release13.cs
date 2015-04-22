namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogEntry", "Description", c => c.String(nullable: false));
            AddColumn("dbo.BlogEntry", "PostDateTimeUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.Blog", "Description", c => c.String(nullable: false));
            AddColumn("dbo.Blog", "AutoDisplayLatestEntry", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserActionEvent", "BlogId", c => c.Int());
            AddColumn("dbo.UserActionEvent", "BlogEntryId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserActionEvent", "BlogEntryId");
            DropColumn("dbo.UserActionEvent", "BlogId");
            DropColumn("dbo.Blog", "AutoDisplayLatestEntry");
            DropColumn("dbo.Blog", "Description");
            DropColumn("dbo.BlogEntry", "PostDateTimeUtc");
            DropColumn("dbo.BlogEntry", "Description");
        }
    }
}
