namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreFrontConfiguration", "ChatEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.StoreFrontConfiguration", "ChatRequireLogin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreFrontConfiguration", "ChatRequireLogin");
            DropColumn("dbo.StoreFrontConfiguration", "ChatEnabled");
        }
    }
}
