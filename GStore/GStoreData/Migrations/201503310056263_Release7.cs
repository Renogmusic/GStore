namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductCategoryAltProductBundle",
                c => new
                    {
                        ProductCategoryAltProductBundleId = c.Int(nullable: false, identity: true),
                        ProductCategoryId = c.Int(nullable: false),
                        ProductBundleId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ProductCategoryAltProductBundleId)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategoryId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.ProductBundle", t => t.ProductBundleId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.ProductCategoryId, t.ProductBundleId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ProductCategoryAltProduct",
                c => new
                    {
                        ProductCategoryAltProductId = c.Int(nullable: false, identity: true),
                        ProductCategoryId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ProductCategoryAltProductId)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategoryId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.ProductCategoryId, t.ProductId }, unique: true, name: "UniqueRecord")
                .Index(t => t.ProductId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ProductCategoryImage",
                c => new
                    {
                        ProductCategoryImageId = c.Int(nullable: false, identity: true),
                        ProductCategoryId = c.Int(nullable: false),
                        ImageName = c.String(nullable: false),
                        Order = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ProductCategoryImageId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategoryId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.ProductCategoryId)
                .Index(t => new { t.ClientId, t.StoreFrontId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ProductImage",
                c => new
                    {
                        ProductImageId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ImageName = c.String(nullable: false),
                        Order = c.Int(nullable: false),
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
                .PrimaryKey(t => t.ProductImageId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.ProductId)
                .Index(t => new { t.ClientId, t.StoreFrontId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSummaryCaption", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultTopDescriptionCaption", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSampleImageCaption", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultBottomDescriptionCaption", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSampleDownloadCaption", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSampleAudioCaption", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultNoProductsMessageHtml", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductTypeSingle", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductTypePlural", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductBundleTypeSingle", c => c.String());
            AddColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductBundleTypePlural", c => c.String());
            AddColumn("dbo.PageTemplateSection", "IsVariable", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ProductCategory", "ProductTypeSingle", c => c.String(maxLength: 100));
            AlterColumn("dbo.ProductCategory", "ProductTypePlural", c => c.String(maxLength: 100));
            AlterColumn("dbo.ProductCategory", "BundleTypeSingle", c => c.String(maxLength: 100));
            AlterColumn("dbo.ProductCategory", "BundleTypePlural", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductImage", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductImage", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.ProductImage", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductImage", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductImage", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ProductCategoryImage", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategoryImage", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.ProductCategoryImage", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.ProductCategoryImage", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategoryImage", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ProductCategoryAltProduct", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategoryAltProduct", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.ProductCategoryAltProduct", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductCategoryAltProduct", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategoryAltProduct", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ProductCategoryAltProduct", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.ProductCategoryAltProductBundle", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategoryAltProductBundle", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.ProductCategoryAltProductBundle", "ProductBundleId", "dbo.ProductBundle");
            DropForeignKey("dbo.ProductCategoryAltProductBundle", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategoryAltProductBundle", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ProductCategoryAltProductBundle", "ProductCategoryId", "dbo.ProductCategory");
            DropIndex("dbo.ProductImage", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductImage", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductImage", "UniqueRecord");
            DropIndex("dbo.ProductImage", new[] { "ProductId" });
            DropIndex("dbo.ProductCategoryImage", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategoryImage", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategoryImage", "UniqueRecord");
            DropIndex("dbo.ProductCategoryImage", new[] { "ProductCategoryId" });
            DropIndex("dbo.ProductCategoryAltProduct", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategoryAltProduct", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategoryAltProduct", new[] { "ProductId" });
            DropIndex("dbo.ProductCategoryAltProduct", "UniqueRecord");
            DropIndex("dbo.ProductCategoryAltProductBundle", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategoryAltProductBundle", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategoryAltProductBundle", "UniqueRecord");
            AlterColumn("dbo.ProductCategory", "BundleTypePlural", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.ProductCategory", "BundleTypeSingle", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.ProductCategory", "ProductTypePlural", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.ProductCategory", "ProductTypeSingle", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.PageTemplateSection", "IsVariable");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductBundleTypePlural");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductBundleTypeSingle");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductTypePlural");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultProductTypeSingle");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultNoProductsMessageHtml");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSampleAudioCaption");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSampleDownloadCaption");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultBottomDescriptionCaption");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSampleImageCaption");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultTopDescriptionCaption");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogDefaultSummaryCaption");
            DropTable("dbo.ProductImage");
            DropTable("dbo.ProductCategoryImage");
            DropTable("dbo.ProductCategoryAltProduct");
            DropTable("dbo.ProductCategoryAltProductBundle");
        }
    }
}
