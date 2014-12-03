namespace GStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class movethemes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.StoreFronts", new[] { "ThemeId" });
            RenameColumn(table: "dbo.StoreFronts", name: "ThemeId", newName: "Theme_ThemeId");
            AddColumn("dbo.StoreFronts", "DefaultNewPageThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.StoreFronts", "AccountThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.StoreFronts", "ProfileThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.StoreFronts", "NotificationsThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.StoreFronts", "CatalogThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.StoreFronts", "AdminThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.Pages", "ThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.Pages", "MetaKeywords", c => c.String());
			Sql("update storefronts set DefaultNewPageThemeId = Theme_ThemeId, AccountThemeId = Theme_ThemeId, ProfileThemeId = Theme_ThemeId, NotificationsThemeId = Theme_ThemeId, CatalogThemeId = Theme_ThemeId, AdminThemeId = Theme_ThemeId");
			Sql("update pages set ThemeId = (select sf.Theme_ThemeId from StoreFronts sf where sf.storefrontid = pages.storefrontid ) ");

            CreateIndex("dbo.StoreFronts", "DefaultNewPageThemeId");
            CreateIndex("dbo.StoreFronts", "AccountThemeId");
            CreateIndex("dbo.StoreFronts", "ProfileThemeId");
            CreateIndex("dbo.StoreFronts", "NotificationsThemeId");
            CreateIndex("dbo.StoreFronts", "CatalogThemeId");
            CreateIndex("dbo.StoreFronts", "AdminThemeId");
            CreateIndex("dbo.Pages", "ThemeId");
            AddForeignKey("dbo.StoreFronts", "AccountThemeId", "dbo.Themes", "ThemeId");
            AddForeignKey("dbo.StoreFronts", "AdminThemeId", "dbo.Themes", "ThemeId");
            AddForeignKey("dbo.StoreFronts", "CatalogThemeId", "dbo.Themes", "ThemeId");
            AddForeignKey("dbo.StoreFronts", "DefaultNewPageThemeId", "dbo.Themes", "ThemeId");
            AddForeignKey("dbo.Pages", "ThemeId", "dbo.Themes", "ThemeId");
            AddForeignKey("dbo.StoreFronts", "NotificationsThemeId", "dbo.Themes", "ThemeId");
            AddForeignKey("dbo.StoreFronts", "ProfileThemeId", "dbo.Themes", "ThemeId");
            DropColumn("dbo.StoreFrontConfigViewModels", "ThemeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StoreFrontConfigViewModels", "ThemeId", c => c.Int(nullable: false));
            DropForeignKey("dbo.StoreFronts", "ProfileThemeId", "dbo.Themes");
            DropForeignKey("dbo.StoreFronts", "NotificationsThemeId", "dbo.Themes");
            DropForeignKey("dbo.Pages", "ThemeId", "dbo.Themes");
            DropForeignKey("dbo.StoreFronts", "DefaultNewPageThemeId", "dbo.Themes");
            DropForeignKey("dbo.StoreFronts", "CatalogThemeId", "dbo.Themes");
            DropForeignKey("dbo.StoreFronts", "AdminThemeId", "dbo.Themes");
            DropForeignKey("dbo.StoreFronts", "AccountThemeId", "dbo.Themes");
            DropIndex("dbo.Pages", new[] { "ThemeId" });
            DropIndex("dbo.StoreFronts", new[] { "Theme_ThemeId" });
            DropIndex("dbo.StoreFronts", new[] { "AdminThemeId" });
            DropIndex("dbo.StoreFronts", new[] { "CatalogThemeId" });
            DropIndex("dbo.StoreFronts", new[] { "NotificationsThemeId" });
            DropIndex("dbo.StoreFronts", new[] { "ProfileThemeId" });
            DropIndex("dbo.StoreFronts", new[] { "AccountThemeId" });
            DropIndex("dbo.StoreFronts", new[] { "DefaultNewPageThemeId" });
            AlterColumn("dbo.StoreFronts", "Theme_ThemeId", c => c.Int(nullable: false));
            DropColumn("dbo.Pages", "MetaKeywords");
            DropColumn("dbo.Pages", "ThemeId");
            DropColumn("dbo.StoreFronts", "AdminThemeId");
            DropColumn("dbo.StoreFronts", "CatalogThemeId");
            DropColumn("dbo.StoreFronts", "NotificationsThemeId");
            DropColumn("dbo.StoreFronts", "ProfileThemeId");
            DropColumn("dbo.StoreFronts", "AccountThemeId");
            DropColumn("dbo.StoreFronts", "DefaultNewPageThemeId");
            RenameColumn(table: "dbo.StoreFronts", name: "Theme_ThemeId", newName: "ThemeId");
            CreateIndex("dbo.StoreFronts", "ThemeId");
        }
    }
}
