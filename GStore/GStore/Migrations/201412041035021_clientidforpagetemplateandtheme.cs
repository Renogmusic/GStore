namespace GStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clientidforpagetemplateandtheme : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Themes", "UniqueRecord");
            DropIndex("dbo.PageTemplates", "UniqueRecord");
            AddColumn("dbo.Themes", "ClientId", c => c.Int(nullable: false));
            AddColumn("dbo.PageTemplates", "ClientId", c => c.Int(nullable: false));
			Sql("update pageTemplates set clientid = (select min(clientid) from clients)");
			Sql("update themes set clientid = (select min(clientid) from clients)");
			CreateIndex("dbo.Themes", new[] { "ClientId", "Name" }, unique: true, name: "UniqueRecord");
            CreateIndex("dbo.PageTemplates", new[] { "ClientId", "Name" }, unique: true, name: "UniqueRecord");
            AddForeignKey("dbo.PageTemplates", "ClientId", "dbo.Clients", "ClientId");
            AddForeignKey("dbo.Themes", "ClientId", "dbo.Clients", "ClientId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Themes", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.PageTemplates", "ClientId", "dbo.Clients");
            DropIndex("dbo.PageTemplates", "UniqueRecord");
            DropIndex("dbo.Themes", "UniqueRecord");
            DropColumn("dbo.PageTemplates", "ClientId");
            DropColumn("dbo.Themes", "ClientId");
            CreateIndex("dbo.PageTemplates", "Name", unique: true, name: "UniqueRecord");
            CreateIndex("dbo.Themes", "Name", unique: true, name: "UniqueRecord");
        }
    }
}
