namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release12 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogEntry",
                c => new
                    {
                        BlogEntryId = c.Int(nullable: false, identity: true),
                        BlogId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        UrlName = c.String(nullable: false, maxLength: 100),
                        ImageName = c.String(),
                        Order = c.Int(nullable: false),
                        ShowInMenu = c.Boolean(nullable: false),
                        DisplayForDirectLinks = c.Boolean(nullable: false),
                        ForRegisteredOnly = c.Boolean(nullable: false),
                        ForAnonymousOnly = c.Boolean(nullable: false),
                        UseDividerBeforeOnMenu = c.Boolean(nullable: false),
                        UseDividerAfterOnMenu = c.Boolean(nullable: false),
                        ThemeId = c.Int(),
                        HeaderHtml = c.String(),
                        FooterHtml = c.String(),
                        MetaDescription = c.String(),
                        MetaKeywords = c.String(),
                        Body1 = c.String(),
                        Body2 = c.String(),
                        Body3 = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BlogEntryId)
                .ForeignKey("dbo.Blog", t => t.BlogId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Theme", t => t.ThemeId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.BlogId, t.UrlName }, unique: true, name: "UniqueRecord")
                .Index(t => t.ThemeId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Blog",
                c => new
                    {
                        BlogId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        UrlName = c.String(nullable: false, maxLength: 100),
                        ImageName = c.String(),
                        Order = c.Int(nullable: false),
                        ShowInMenu = c.Boolean(nullable: false),
                        HideInMenuIfEmpty = c.Boolean(nullable: false),
                        DisplayForDirectLinks = c.Boolean(nullable: false),
                        ForRegisteredOnly = c.Boolean(nullable: false),
                        ForAnonymousOnly = c.Boolean(nullable: false),
                        UseDividerBeforeOnMenu = c.Boolean(nullable: false),
                        UseDividerAfterOnMenu = c.Boolean(nullable: false),
                        ThemeId = c.Int(),
                        ListHeaderHtml = c.String(),
                        ListFooterHtml = c.String(),
                        DefaultEntryHeaderHtml = c.String(),
                        DefaultBlogFooterHtml = c.String(),
                        DefaultMetaDescription = c.String(),
                        DefaultMetaKeywords = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BlogId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Theme", t => t.ThemeId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.UrlName }, unique: true, name: "UniqueRecord")
                .Index(t => t.ThemeId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            AddColumn("dbo.StoreFrontConfiguration", "HomePageUseBlog", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BlogEntry", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.BlogEntry", "ThemeId", "dbo.Theme");
            DropForeignKey("dbo.BlogEntry", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.BlogEntry", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.BlogEntry", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Blog", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Blog", "ThemeId", "dbo.Theme");
            DropForeignKey("dbo.Blog", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Blog", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Blog", "ClientId", "dbo.Client");
            DropForeignKey("dbo.BlogEntry", "BlogId", "dbo.Blog");
            DropIndex("dbo.Blog", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Blog", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Blog", new[] { "ThemeId" });
            DropIndex("dbo.Blog", "UniqueRecord");
            DropIndex("dbo.BlogEntry", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.BlogEntry", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.BlogEntry", new[] { "ThemeId" });
            DropIndex("dbo.BlogEntry", "UniqueRecord");
            DropColumn("dbo.StoreFrontConfiguration", "HomePageUseBlog");
            DropTable("dbo.Blog");
            DropTable("dbo.BlogEntry");
        }
    }
}
