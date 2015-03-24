namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductBundle", "ProductTypeSingle", c => c.String(maxLength: 100));
            AddColumn("dbo.ProductBundle", "ProductTypePlural", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductBundle", "ProductTypePlural");
            DropColumn("dbo.ProductBundle", "ProductTypeSingle");
        }
    }
}
