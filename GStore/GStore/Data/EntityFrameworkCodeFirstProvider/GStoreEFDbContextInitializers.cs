using System.Data.Entity;

namespace GStore.Data.EntityFrameworkCodeFirstProvider
{
	public class GStoreEFDbContextInitializerDropCreate : DropCreateDatabaseAlways<GStoreEFDbContext>
	{
		protected override void Seed(GStoreEFDbContext context)
		{
			SeedDataExtensions.AddSeedData(context.GStoreDb);
			base.Seed(context);
		}
	}

	class GStoreDbContextInitializerMigrateLatest : MigrateDatabaseToLatestVersion<GStoreEFDbContext, Migrations.Configuration>
	{
		//migration configuration will automatically call Seed
	}

	class GStoreDbContextInitializerCreateIfNotExists:System.Data.Entity.CreateDatabaseIfNotExists<GStoreEFDbContext>
	{
		protected override void Seed(GStoreEFDbContext context)
		{
			SeedDataExtensions.AddSeedData(context.GStoreDb);
			base.Seed(context);
		}
	}

}
