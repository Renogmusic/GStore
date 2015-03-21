using System;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreWeb.Areas.StoreAdmin.Controllers
{
	public class StoreAdminController : AreaBaseController.StoreAdminAreaBaseController
    {
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

		public ActionResult TestPayPal()
		{
			bool result = CurrentStoreFrontConfigOrThrow.TestPayPal(this);
			return RedirectToAction("Index");
		}
	}
}
