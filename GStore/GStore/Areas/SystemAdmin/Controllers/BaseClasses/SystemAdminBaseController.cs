using GStore.Areas.StoreAdmin.Models.ViewModels;
using GStore.Controllers.BaseClass;
using GStore.Data;
using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.SystemAdmin.Controllers.BaseClasses
{
	[Authorize(Roles="SystemAdmin")]
	public class SystemAdminBaseController : GStore.Controllers.BaseClass.BaseController
	{
		public SystemAdminBaseController(IGstoreDb dbContext): base(dbContext)
		{
			this._throwErrorIfUserProfileNotFound = false;
			this._throwErrorIfStoreFrontNotFound = false;
		}

		public SystemAdminBaseController()
		{
			this._throwErrorIfUserProfileNotFound = false;
			this._throwErrorIfStoreFrontNotFound = false;
		}

		protected override string LayoutName
		{
			get
			{
				return Properties.Settings.Current.AppDefaultLayoutName;
			}
		}


	}
}