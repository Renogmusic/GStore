using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Areas.OrderAdmin.Controllers
{
	public class OrderAdminController : BaseClasses.OrderAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Admin_OrderAdminArea)]
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
	}
}