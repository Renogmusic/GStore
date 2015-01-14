﻿using GStore.Areas.CatalogAdmin.ViewModels;
using GStore.Data;

namespace GStore.Areas.CatalogAdmin.Controllers.BaseClasses
{
	[Identity.AuthorizeGStoreAction(Identity.GStoreAction.Admin_CatalogAdminArea)]
	public class CatalogAdminBaseController : GStore.Controllers.BaseClass.BaseController
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

		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontConfigOrAny.CatalogAdminLayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrAny.CatalogAdminTheme.FolderName;
			}
		}

		public CatalogAdminViewModel CatalogAdminViewModel
		{
			get
			{
				return new CatalogAdminViewModel(CurrentStoreFrontConfigOrAny, CurrentUserProfileOrThrow);
			}
		}


	}
}