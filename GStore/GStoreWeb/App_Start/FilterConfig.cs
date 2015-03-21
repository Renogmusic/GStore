using System.Web.Mvc;
using GStoreData;
using GStoreData.ControllerBase;
using GStoreData.Models;

namespace GStoreWeb
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			//filters.Add(new HandleErrorAttribute());
			if (Settings.AppEnablePageViewLog)
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
			if (!Settings.AppEnablePageViewLog)
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

				Client client = controller.CurrentClientOrNull;
				bool enablePageViewLog = true;
				if (client != null)
				{
					enablePageViewLog = client.EnablePageViewLog;
				}

				if (enablePageViewLog)
				{
					//log page view
					IGstoreDb db = controller.GStoreDb;
					if (db == null)
					{
						db = RepositoryFactory.SystemWideRepository(context.HttpContext.User);
					}

					db.LogPageViewEvent(context);
				}
			}

			base.OnResultExecuting(context);
		}
	}
}
