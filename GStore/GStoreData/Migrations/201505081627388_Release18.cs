namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductCategory", "ShowTop10ChildProductsInMenu", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductCategory", "ShowTop10ChildProductsInMenu");
        }
    }
}
