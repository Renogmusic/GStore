namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreFrontConfiguration", "BlogAdminThemeId", c => c.Int(nullable: true));
			Sql("update storefrontconfiguration set BlogAdminThemeId = BlogThemeId");
			AlterColumn("dbo.StoreFrontConfiguration", "BlogAdminThemeId", c => c.Int(nullable: false));
            CreateIndex("dbo.StoreFrontConfiguration", "BlogAdminThemeId");
            AddForeignKey("dbo.StoreFrontConfiguration", "BlogAdminThemeId", "dbo.Theme", "ThemeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreFrontConfiguration", "BlogAdminThemeId", "dbo.Theme");
            DropIndex("dbo.StoreFrontConfiguration", new[] { "BlogAdminThemeId" });
            DropColumn("dbo.StoreFrontConfiguration", "BlogAdminThemeId");
        }
    }
}
