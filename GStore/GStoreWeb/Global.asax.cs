using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GStoreData;
using GStoreData.ControllerBase;
using GStoreData.EntityFrameworkCodeFirstProvider;
using GStoreData.Exceptions;

namespace GStoreWeb
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

			switch (RepositoryFactory.RepositoryProvider())
			{
				case RepositoryProviderEnum.EntityFrameworkCodeFirstProvider:
					if (Settings.InitializeEFCodeFirstMigrateLatest)
					{
						System.Data.Entity.Database.SetInitializer(new GStoreDbContextInitializerMigrateLatest());
					}
					else if (Settings.InitializeEFCodeFirstDropCreate)
					{
						System.Data.Entity.Database.SetInitializer(new GStoreEFDbContextInitializerDropCreate());
					}
					else
					{
						System.Data.Entity.Database.SetInitializer(new GStoreDbContextInitializerCreateIfNotExists());
					}
					break;
				case RepositoryProviderEnum.ListProvider:
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
				BaseController gstoreController = GetGStoreControllerFromException(ex);
				gstoreController.InitContext(this.Context, "GStore", "AppError", "", null);
				if (ex is HttpException)
				{
					ExceptionHandler.HandleHttpException(ex as HttpException, true, HttpContext.Current, null, gstoreController);
				}
				else if (ex is ApplicationException)
				{
					ExceptionHandler.HandleAppException(ex as ApplicationException, true, HttpContext.Current, null, gstoreController);
				}
				else if (ex is InvalidOperationException)
				{
					ExceptionHandler.HandleInvalidOperationException(ex as InvalidOperationException, true, HttpContext.Current, null, gstoreController);
				}
				else
				{
					ExceptionHandler.HandleUnknownException(ex, true, HttpContext.Current, null, gstoreController);
				}
			}
			catch (Exception exInHandler)
			{
				string errorInHandler = exInHandler.Message;
				throw;
			}
		}

		protected virtual BaseController GetGStoreControllerFromException(Exception ex)
		{
			BaseController controller = null;
			if (ex is GStoreData.Exceptions.HttpExceptionBase)
			{
				controller = (ex as HttpExceptionBase).GStoreControllerForArea;
			}

			if (controller == null)
			{
				controller = new Controllers.GStoreController();
			}

			return controller;
		}
	}
}
