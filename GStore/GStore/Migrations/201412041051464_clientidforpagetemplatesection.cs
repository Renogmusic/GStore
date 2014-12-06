namespace GStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class clientidforpagetemplatesection : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PageTemplateSections", "UniqueRecord");
            AddColumn("dbo.PageTemplateSections", "ClientId", c => c.Int(nullable: false));
			Sql("update PageTemplateSections set clientid = (select min(clientid) from clients)");
            CreateIndex("dbo.PageTemplateSections", new[] { "ClientId", "PageTemplateId", "Name" }, unique: true, name: "UniqueRecord");
            AddForeignKey("dbo.PageTemplateSections", "ClientId", "dbo.Clients", "ClientId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PageTemplateSections", "ClientId", "dbo.Clients");
            DropIndex("dbo.PageTemplateSections", "UniqueRecord");
            DropColumn("dbo.PageTemplateSections", "ClientId");
            CreateIndex("dbo.PageTemplateSections", new[] { "PageTemplateId", "Name" }, unique: true, name: "UniqueRecord");
        }
    }
}
