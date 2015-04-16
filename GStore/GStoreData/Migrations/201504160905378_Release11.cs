namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release11 : DbMigration
    {
        public override void Up()
        {

			AddColumn("dbo.StoreFrontConfiguration", "BlogThemeId", c => c.Int(nullable: true));
			AddColumn("dbo.StoreFrontConfiguration", "ChatThemeId", c => c.Int(nullable: true));

			Sql("update storefrontconfiguration set ChatThemeId = CartThemeId, BlogThemeId = CartThemeId");

			AlterColumn("dbo.StoreFrontConfiguration", "BlogThemeId", c => c.Int(nullable: false));
			AlterColumn("dbo.StoreFrontConfiguration", "ChatThemeId", c => c.Int(nullable: false));
            CreateIndex("dbo.StoreFrontConfiguration", "BlogThemeId");
            CreateIndex("dbo.StoreFrontConfiguration", "ChatThemeId");
            AddForeignKey("dbo.StoreFrontConfiguration", "BlogThemeId", "dbo.Theme", "ThemeId");
            AddForeignKey("dbo.StoreFrontConfiguration", "ChatThemeId", "dbo.Theme", "ThemeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreFrontConfiguration", "ChatThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "BlogThemeId", "dbo.Theme");
            DropIndex("dbo.StoreFrontConfiguration", new[] { "ChatThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "BlogThemeId" });
            DropColumn("dbo.StoreFrontConfiguration", "ChatThemeId");
            DropColumn("dbo.StoreFrontConfiguration", "BlogThemeId");
        }
    }
}
