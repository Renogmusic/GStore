using System.Data.Entity.Migrations;

namespace GStoreData.Migrations
{

	/// <summary>
	/// Migration configuration for Code First Migrations
	/// Data loss allowed and automatic migrations enabled
	/// </summary>
	public class Configuration : DbMigrationsConfiguration<EntityFrameworkCodeFirstProvider.GStoreEFDbContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(EntityFrameworkCodeFirstProvider.GStoreEFDbContext context)
		{
			SeedDataExtensions.AddSeedData(context);
		}
	}
}
