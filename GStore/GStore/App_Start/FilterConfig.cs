using GStore.Controllers.BaseClass;
using System.Web;
using System.Web.Mvc;
using GStore.Data;

namespace GStore
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			if (Properties.Settings.Current.AppEnablePageViewLog)
			{
				filters.Add(new PageLogger());
			}

		}
	}

	public class PageLogger : ActionFilterAttribute
	{
		/// <summary>
		/// Note: only controllers that inherit from base controllers can log page views automatically
		/// </summary>
		/// <param name="context"></param>
		public override void OnResultExecuting(ResultExecutingContext context)
		{
			if (!Properties.Settings.Current.AppEnablePageViewLog)
			{
				base.OnResultExecuting(context);
				return;
			}

			if (context.Controller is BaseController)
			{
				BaseController controller = (BaseController)context.Controller;
				if (!controller.LogActionsAsPageViews)
				{
					base.OnResultExecuting(context);
					return;
				}

				Models.Client client = controller.CurrentClientOrNull;
				bool enablePageViewLog = true;
				if (client != null)
				{
					enablePageViewLog = client.EnablePageViewLog;
				}

				if (enablePageViewLog)
				{
					//log page view
					Data.IGstoreDb db = controller.GStoreDb;
					if (db == null)
					{
						db = Data.RepositoryFactory.SystemWideRepository(context.HttpContext.User);
					}

					db.LogPageViewEvent(context);
				}
			}

			base.OnResultExecuting(context);
		}
	}
}
