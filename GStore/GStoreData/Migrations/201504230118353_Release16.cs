namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "LastBlogAdminVisitDateTimeUtc", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "LastBlogAdminVisitDateTimeUtc");
        }
    }
}
