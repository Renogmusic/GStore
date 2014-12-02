using GStore.Areas.StoreAdmin.ViewModels;
using GStore.Controllers.BaseClass;
using GStore.Data;
using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers.BaseClasses
{
	[Identity.AuthorizeGStoreAction(Identity.GStoreAction.Admin_StoreAdminArea)]
	public class StoreAdminBaseController : GStore.Controllers.BaseClass.BaseController
	{
		public StoreAdminBaseController(IGstoreDb dbContext): base(dbContext)
		{
			this._throwErrorIfUserProfileNotFound = true;
		}

		public StoreAdminBaseController()
		{
			this._throwErrorIfUserProfileNotFound = true;
		}

		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontOrThrow.AdminLayoutName;
			}
		}

		public GStore.Areas.StoreAdmin.ViewModels.StoreAdminViewModel StoreAdminViewModel
		{
			get
			{
				return new GStore.Areas.StoreAdmin.ViewModels.StoreAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
			}
		}


	}
}