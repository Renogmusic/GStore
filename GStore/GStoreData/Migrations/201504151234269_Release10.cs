namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreFrontConfiguration", "ShowAboutGStoreMenu", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreFrontConfiguration", "ShowAboutGStoreMenu");
        }
    }
}
