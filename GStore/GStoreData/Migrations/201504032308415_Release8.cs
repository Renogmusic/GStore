namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release8 : DbMigration
    {
        public override void Up()
        {
			Sql("delete from storebinding");
            DropIndex("dbo.StoreBinding", "UniqueRecord");
            AddColumn("dbo.StoreBinding", "StoreFrontConfigurationId", c => c.Int(nullable: false));
            CreateIndex("dbo.StoreBinding", new[] { "ClientId", "StoreFrontId", "StoreBindingId", "StoreFrontConfigurationId" }, unique: true, name: "UniqueRecord");
            AddForeignKey("dbo.StoreBinding", "StoreFrontConfigurationId", "dbo.StoreFrontConfiguration", "StoreFrontConfigurationId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreBinding", "StoreFrontConfigurationId", "dbo.StoreFrontConfiguration");
            DropIndex("dbo.StoreBinding", "UniqueRecord");
            DropColumn("dbo.StoreBinding", "StoreFrontConfigurationId");
            CreateIndex("dbo.StoreBinding", new[] { "ClientId", "StoreFrontId", "StoreBindingId" }, unique: true, name: "UniqueRecord");
        }
    }
}
