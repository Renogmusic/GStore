namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreFrontColForBundleItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreFrontConfiguration", "CatalogProductBundleItemColLg", c => c.Int(nullable: false, defaultValue: 3));
            AddColumn("dbo.StoreFrontConfiguration", "CatalogProductBundleItemColMd", c => c.Int(nullable: false, defaultValue: 4));
            AddColumn("dbo.StoreFrontConfiguration", "CatalogProductBundleItemColSm", c => c.Int(nullable: false, defaultValue: 6));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StoreFrontConfiguration", "CatalogProductBundleItemColSm");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogProductBundleItemColMd");
            DropColumn("dbo.StoreFrontConfiguration", "CatalogProductBundleItemColLg");
        }
    }
}
