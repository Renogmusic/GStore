namespace GStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class webformid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WebForms", "SuccessPageId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "WebFormId", "dbo.WebForms");
            DropIndex("dbo.Pages", new[] { "WebFormId" });
            DropIndex("dbo.WebForms", new[] { "SuccessPageId" });
            DropColumn("dbo.Pages", "WebFormId");
            DropColumn("dbo.WebForms", "SuccessPageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WebForms", "SuccessPageId", c => c.Int(nullable: false));
            AddColumn("dbo.Pages", "WebFormId", c => c.Int());
            CreateIndex("dbo.WebForms", "SuccessPageId");
            CreateIndex("dbo.Pages", "WebFormId");
            AddForeignKey("dbo.Pages", "WebFormId", "dbo.WebForms", "WebFormId");
            AddForeignKey("dbo.WebForms", "SuccessPageId", "dbo.Pages", "PageId");
        }
    }
}
