namespace GStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class webformid2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Pages", name: "WebForm_WebFormId", newName: "WebFormId");
            RenameIndex(table: "dbo.Pages", name: "IX_WebForm_WebFormId", newName: "IX_WebFormId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Pages", name: "IX_WebFormId", newName: "IX_WebForm_WebFormId");
            RenameColumn(table: "dbo.Pages", name: "WebFormId", newName: "WebForm_WebFormId");
        }
    }
}
