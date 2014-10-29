using GStore.Controllers.BaseClass;
using System.Web;
using System.Web.Mvc;
using GStore.Models.Extensions;

namespace GStore
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new PageLogger());
			if (Properties.Settings.Default.AppEnablePageViewLog)
			{
				filters.Add(new HandleErrorAttribute());
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
			if (context.Controller is BaseController)
			{
				BaseController controller = (BaseController)context.Controller;

				if (Properties.Settings.Default.AppEnablePageViewLog && controller.CurrentClient.EnablePageViewLog)
				{
					//log page view
					controller.GStoreDb.LogPageViewEvent(context);
				}
			}

			base.OnResultExecuting(context);
		}
	}
}
