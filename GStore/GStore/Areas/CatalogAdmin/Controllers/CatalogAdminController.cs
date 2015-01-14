using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;

namespace GStore.Areas.CatalogAdmin.Controllers
{
	public class CatalogAdminController : BaseClasses.CatalogAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Admin_CatalogAdminArea)]
        public ActionResult Index()
        {
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null)
			{
				profile.LastCatalogAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
			}

			return View("Index", this.CatalogAdminViewModel);
        }
	}
}