using System;
using System.Web.Mvc;
using GStoreData.Areas.CatalogAdmin.ViewModels;
using GStoreData.Identity;
using GStoreData.Models;

namespace GStoreData.Areas.CatalogAdmin.ControllerBase
{
	[AuthorizeGStoreAction(Identity.GStoreAction.Admin_CatalogAdminArea)]
	public abstract class CatalogAdminBaseController : GStoreData.ControllerBase.BaseController
	{
		public CatalogAdminBaseController(IGstoreDb dbContext): base(dbContext)
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		public CatalogAdminBaseController()
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		protected override string Area
		{
			get { return "CatalogAdmin"; }
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrAny.CatalogAdminTheme.FolderName;
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

		public CatalogAdminViewModel CatalogAdminViewModel
		{
			get
			{
				if (_catalogAdminViewModel != null)
				{
					return _catalogAdminViewModel;
				}
				_catalogAdminViewModel = new CatalogAdminViewModel(CurrentStoreFrontConfigOrAny, CurrentUserProfileOrThrow);
				return _catalogAdminViewModel;
			}
		}
		protected CatalogAdminViewModel _catalogAdminViewModel = null;

	}
}