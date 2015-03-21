using System;
using System.Web.Mvc;

namespace GStoreWeb.Areas.SystemAdmin.Controllers
{
	public class GStoreController : AreaBaseController.SystemAdminAreaBaseController
    {
		public ActionResult Index()
		{
			return About();
		}
		public ActionResult About()
		{
			this.BreadCrumbsFunc = htmlHelper => this.GStoreAboutBreadcrumb(htmlHelper, false);
			return View("About");
		}

		public ActionResult SystemInfo()
		{
			this.BreadCrumbsFunc = htmlHelper => this.GStoreSystemInfoBreadcrumb(htmlHelper, false);
			return View("SystemInfo");
		}

		public ActionResult Settings()
		{
			this.BreadCrumbsFunc = htmlHelper => this.GStoreSettingsBreadcrumb(htmlHelper, false);
			return View("Settings");
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			if (RouteData.Values["action"].ToString().ToLower() == "apperror")
			{
				throw new ApplicationException("Exception in error handler. " + filterContext.Exception.Message, filterContext.Exception);
			}
			base.OnException(filterContext);
		}

		public ActionResult AppError(Exception exception, GStoreData.Exceptions.ErrorPage? errorPage, int? httpStatusCode)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("Exception");
			}
			if (!errorPage.HasValue)
			{
				throw new ArgumentNullException("ErrorPage");
			}
			if (!httpStatusCode.HasValue)
			{
				throw new ArgumentNullException("httpStatusCode");
			}

			TryDisplayErrorView(exception, errorPage.Value, httpStatusCode.Value, true);
			return null;
		}

	}
}
