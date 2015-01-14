using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;

namespace GStore.Areas.OrderAdmin.Controllers
{
	public class OrderAdminController : BaseClasses.OrderAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Admin_OrderAdminArea)]
        public ActionResult Index()
        {
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null)
			{
				profile.LastOrderAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
			}

			return View("Index", this.OrderAdminViewModel);
        }
	}
}