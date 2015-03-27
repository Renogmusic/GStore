namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Product", "BaseUnitPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.Product", "BaseListPrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "BaseListPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Product", "BaseUnitPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
