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
					if (Properties.Settings.Current.InitializeEFCodeFirstMigrateLatest)
					{
						System.Data.Entity.Database.SetInitializer(new Data.EntityFrameworkCodeFirstProvider.GStoreDbContextInitializerMigrateLatest());
					}
					else if (Properties.Settings.Current.InitializeEFCodeFirstDropCreate)
					{
						System.Data.Entity.Database.SetInitializer(new Data.EntityFrameworkCodeFirstProvider.GStoreEFDbContextInitializerDropCreate());
					}
					else
					{
						System.Data.Entity.Database.SetInitializer(new Data.EntityFrameworkCodeFirstProvider.GStoreDbContextInitializerCreateIfNotExists());
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
			try
			{
				if (ex is HttpException)
				{
					Exceptions.ExceptionHandler.HandleHttpException(ex as HttpException, true, HttpContext.Current, null, null);
				}
				else if (ex is ApplicationException)
				{
					Exceptions.ExceptionHandler.HandleAppException(ex as ApplicationException, true, HttpContext.Current, null, null);
				}
				else if (ex is InvalidOperationException)
				{
					Exceptions.ExceptionHandler.HandleInvalidOperationException(ex as InvalidOperationException, true, HttpContext.Current, null, null);
				}
				else
				{
					Exceptions.ExceptionHandler.HandleUnknownException(ex, true, HttpContext.Current, null, null);
				}
			}
			catch (Exception exInHandler)
			{
				string errorInHandler = exInHandler.Message;
				throw exInHandler;
			}
		}
	}

}
