using System;
using System.Web.Mvc;
using GStoreData.Areas.BlogAdmin.ViewModels;
using GStoreData.Identity;
using GStoreData.Models;

namespace GStoreData.Areas.BlogAdmin.ControllerBase
{
	[AuthorizeGStoreAction(Identity.GStoreAction.Admin_BlogAdminArea)]
	public abstract class BlogAdminBaseController : GStoreData.ControllerBase.BaseController
	{
		public BlogAdminBaseController(IGstoreDb dbContext): base(dbContext)
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		public BlogAdminBaseController()
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		protected override string Area
		{
			get { return "BlogAdmin"; }
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrAny.BlogAdminTheme.FolderName;
			}
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			try
			{
				StoreFrontConfiguration storeFrontConfig = GStoreDb.GetCurrentStoreFrontConfig(Request, true, false);
				if (storeFrontConfig == null)
				{
					AddUserMessage("Store Front Inactive or not found!", "Sorry, this URL does not point to an active store front. Please contact us for assistance.", AppHtmlHelpers.UserMessageType.Danger);
					filterContext.ExceptionHandled = true;
					RedirectToAction("Index", "Home", new { area = "" }).ExecuteResult(this.ControllerContext);
				}
			}
			catch (Exceptions.StoreFrontInactiveException)
			{
				AddUserMessage("Store Front Inactive!", "Sorry, this URL points to an Inactive Store Front. Please contact us for assistance.", AppHtmlHelpers.UserMessageType.Danger);
				filterContext.ExceptionHandled = true;
				RedirectToAction("Index", "Home", new { area = "" }).ExecuteResult(this.ControllerContext);
			}
			catch (Exceptions.NoMatchingBindingException)
			{
				AddUserMessage("Store Front Not Found!", "Sorry, this URL does not point to an active store front. Please contact us for assistance.", AppHtmlHelpers.UserMessageType.Danger);
				filterContext.ExceptionHandled = true;
				RedirectToAction("Index", "Home", new { area = "" }).ExecuteResult(this.ControllerContext);
			}
			catch (Exception)
			{
				throw;
			}
			base.OnException(filterContext);
		}

		public BlogAdminViewModel BlogAdminViewModel
		{
			get
			{
				if (_blogAdminViewModel != null)
				{
					return _blogAdminViewModel;
				}
				_blogAdminViewModel = new BlogAdminViewModel(CurrentStoreFrontConfigOrAny, CurrentUserProfileOrThrow);
				return _blogAdminViewModel;
			}
		}
		protected BlogAdminViewModel _blogAdminViewModel = null;

	}
}