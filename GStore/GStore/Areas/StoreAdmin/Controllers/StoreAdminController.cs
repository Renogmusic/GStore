using GStore.Areas.StoreAdmin.ViewModels;
using GStore.Controllers.BaseClass;
using GStore.Identity;
using GStore.Models;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Areas.StoreAdmin.Controllers
{
	public class StoreAdminController : BaseClasses.StoreAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Admin_StoreAdminArea)]
		public ActionResult Index(DashboardDateTimeRange? dateTimeRange)
        {
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null && !Session.StoreAdminVisitLogged())
			{
				profile.LastStoreAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
				Session.StoreAdminVisitLogged(true);
			}

			ViewData.Add("DashboardDateTimeRange", dateTimeRange);
			return View("Index", this.StoreAdminViewModel);
        }
	}
}