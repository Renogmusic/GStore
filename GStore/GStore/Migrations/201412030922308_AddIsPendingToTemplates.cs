namespace GStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsPendingToTemplates : DbMigration
    {
        public override void Up()
        {
			Sql("update PageTemplates SET IsPending = 0, StartDateTimeUtc = getdate(), EndDateTimeUtc = '12/3/2114'");
			Sql("update PageTemplateSections SET IsPending = 0, StartDateTimeUtc = getdate(), EndDateTimeUtc = '12/3/2114'");
			Sql("update Themes SET IsPending = 0, StartDateTimeUtc = getdate(), EndDateTimeUtc = '12/3/2114'");
		}
        
        public override void Down()
        {
        }
    }
}
