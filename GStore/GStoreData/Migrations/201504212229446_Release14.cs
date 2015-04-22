namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreFrontConfiguration", "ShowBlogInMenu", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreFrontConfiguration", "ShowBlogInMenu");
        }
    }
}
