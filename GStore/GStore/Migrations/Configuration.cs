namespace GStore.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<GStore.Data.EntityFrameworkCodeFirstProvider.GStoreEFDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
        }
		protected override void Seed(Data.EntityFrameworkCodeFirstProvider.GStoreEFDbContext context)
		{
			GStore.Data.Extensions.SeedDataExtensions.AddSeedData(context.GStoreDb);
			base.Seed(context);
		}
    }
}
