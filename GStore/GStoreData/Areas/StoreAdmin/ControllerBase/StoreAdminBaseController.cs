using System;
using System.Web.Mvc;
using GStoreData.Areas.StoreAdmin.ViewModels;
using GStoreData.Models;

namespace GStoreData.Areas.StoreAdmin.ControllerBase
{
	[Identity.AuthorizeGStoreAction(Identity.GStoreAction.Admin_StoreAdminArea)]
	public abstract class StoreAdminBaseController : GStoreData.ControllerBase.BaseController
	{
		public StoreAdminBaseController(IGstoreDb dbContext)
			: base(dbContext)
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		public StoreAdminBaseController()
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		protected override string Area
		{
			get { return "StoreAdmin"; }
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrAny.AdminTheme.FolderName;
			}
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			try
			{
				StoreFront storeFront = GStoreDb.GetCurrentStoreFront(Request, true, false, true);
				if (storeFront == null)
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

		public StoreAdminViewModel StoreAdminViewModel
		{
			get
			{
				return new StoreAdminViewModel(CurrentStoreFrontConfigOrAny, CurrentUserProfileOrThrow);
			}
		}


	}
}