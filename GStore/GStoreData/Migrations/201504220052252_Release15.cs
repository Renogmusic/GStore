namespace GStoreData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Release15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blog", "ShowInListEvenIfNoPermission", c => c.Boolean(nullable: false));
            AddColumn("dbo.BlogEntry", "ShowInListEvenIfNoPermission", c => c.Boolean(nullable: false));
            DropColumn("dbo.Blog", "ShowInMenu");
            DropColumn("dbo.Blog", "HideInMenuIfEmpty");
            DropColumn("dbo.Blog", "DisplayForDirectLinks");
            DropColumn("dbo.Blog", "UseDividerBeforeOnMenu");
            DropColumn("dbo.Blog", "UseDividerAfterOnMenu");
            DropColumn("dbo.BlogEntry", "ShowInMenu");
            DropColumn("dbo.BlogEntry", "DisplayForDirectLinks");
            DropColumn("dbo.BlogEntry", "UseDividerBeforeOnMenu");
            DropColumn("dbo.BlogEntry", "UseDividerAfterOnMenu");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BlogEntry", "UseDividerAfterOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.BlogEntry", "UseDividerBeforeOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.BlogEntry", "DisplayForDirectLinks", c => c.Boolean(nullable: false));
            AddColumn("dbo.BlogEntry", "ShowInMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blog", "UseDividerAfterOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blog", "UseDividerBeforeOnMenu", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blog", "DisplayForDirectLinks", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blog", "HideInMenuIfEmpty", c => c.Boolean(nullable: false));
            AddColumn("dbo.Blog", "ShowInMenu", c => c.Boolean(nullable: false));
            DropColumn("dbo.BlogEntry", "ShowInListEvenIfNoPermission");
            DropColumn("dbo.Blog", "ShowInListEvenIfNoPermission");
        }
    }
}
