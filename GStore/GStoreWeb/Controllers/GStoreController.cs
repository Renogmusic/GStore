using System;
using System.Web.Mvc;
using GStoreData;
using GStoreData.Models;

namespace GStoreWeb.Controllers
{
	public class GStoreController : AreaBaseController.RootAreaBaseController
    {
		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.DefaultNewPageTheme.FolderName;
			}
		}

		public ActionResult About()
		{
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.GStore, UserActionActionEnum.GStore_ViewAbout, "", true);
			return View("About");
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
