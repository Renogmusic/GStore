namespace GStoreData.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class release2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ProductBundleItem", "BaseUnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ProductBundleItem", "BaseListPrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductBundleItem", "BaseListPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.ProductBundleItem", "BaseUnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
