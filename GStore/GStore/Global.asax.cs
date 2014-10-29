using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GStore
{
	public class MvcApplication : System.Web.HttpApplication
	{

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			switch (Data.RepositoryFactory.RepositoryProvider())
			{
				case Data.RepositoryProviderEnum.EntityFrameworkCodeFirstProvider:
					if (Properties.Settings.Default.InitializeEFCodeFirstMigrateLatest)
					{
						System.Data.Entity.Database.SetInitializer(new Data.EntityFrameworkCodeFirstProvider.GStoreDbContextInitializerMigrateLatest());
					}
					else if (Properties.Settings.Default.InitializeEFCodeFirstDropCreate)
					{
						System.Data.Entity.Database.SetInitializer(new Data.EntityFrameworkCodeFirstProvider.GStoreEFDbContextInitializerDropCreate());
					}
					break;
				case Data.RepositoryProviderEnum.ListProvider:
					//can't really set an initializer on a list, so it's done at repository creation
				default:
					break;
			}
		}

		protected void Application_Error()
		{
			Exception ex = Server.GetLastError();
			if (ex is HttpException)
			{
				GStore.ExceptionHandler.HandleHttpException(ex as HttpException, true, HttpContext.Current, null, null);
			}
			else if (ex is ApplicationException)
			{
				GStore.ExceptionHandler.HandleAppException(ex as ApplicationException, true, HttpContext.Current, null, null);
			}
			else
			{
				GStore.ExceptionHandler.HandleUnknownException(ex, true, HttpContext.Current, null, null);
			}
		}
	}

}
