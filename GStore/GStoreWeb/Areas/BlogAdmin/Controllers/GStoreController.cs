using System;
using System.Web.Mvc;

namespace GStoreWeb.Areas.BlogAdmin.Controllers
{
	public class GStoreController : AreaBaseController.BlogAdminAreaBaseController
    {
		public ActionResult About()
		{
			return View("About", this.BlogAdminViewModel);
		}

		public ActionResult SystemInfo()
		{
			return View("SystemInfo", this.BlogAdminViewModel);
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
