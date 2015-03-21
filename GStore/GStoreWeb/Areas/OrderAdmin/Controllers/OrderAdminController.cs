using System;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreWeb.Areas.OrderAdmin.Controllers
{
	public class OrderAdminController : AreaBaseController.OrderAdminAreaBaseController
    {
        public ActionResult Index()
        {
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null && !Session.OrderAdminVisitLogged())
			{
				profile.LastOrderAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
				Session.OrderAdminVisitLogged(true);
			}

			return View("Index", this.OrderAdminViewModel);
        }

		public ActionResult TestPayPal()
		{
			bool result = CurrentStoreFrontConfigOrThrow.TestPayPal(this);
			return RedirectToAction("Index");
		}

	}
}
