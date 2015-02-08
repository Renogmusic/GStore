using GStore.Models;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Controllers
{
    public class GStoreController : BaseClass.BaseController
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

		public ActionResult AppError(Exception exception, GStore.Exceptions.ErrorPage? errorPage, int? httpStatusCode)
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
