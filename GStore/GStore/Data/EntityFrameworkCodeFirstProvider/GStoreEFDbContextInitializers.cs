using System.Data.Entity;

namespace GStore.Data.EntityFrameworkCodeFirstProvider
{
	public class GStoreEFDbContextInitializerDropCreate : DropCreateDatabaseAlways<GStoreEFDbContext>
	{
		protected override void Seed(GStoreEFDbContext context)
		{
			Extensions.SeedDataExtensions.AddSeedData(context.GStoreDb);
			base.Seed(context);
		}
	}

	class GStoreDbContextInitializerMigrateLatest : MigrateDatabaseToLatestVersion<GStoreEFDbContext, Migrations.Configuration>
	{
	}

}
