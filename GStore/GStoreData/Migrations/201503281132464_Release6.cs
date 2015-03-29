namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StoreFrontConfiguration", "CheckoutOrderMinimum", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.PageTemplateSection", "DefaultStringValue", c => c.String());
            AddColumn("dbo.PageSection", "StringValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PageSection", "StringValue");
            DropColumn("dbo.PageTemplateSection", "DefaultStringValue");
            DropColumn("dbo.StoreFrontConfiguration", "CheckoutOrderMinimum");
        }
    }
}
