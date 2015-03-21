using System;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.Identity;
using GStoreData.Models;

namespace GStoreWeb.Areas.CatalogAdmin.Controllers
{
	public class CatalogAdminController : AreaBaseController.CatalogAdminAreaBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Admin_CatalogAdminArea)]
        public ActionResult Index()
        {
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null && !Session.CatalogAdminVisitLogged())
			{
				profile.LastCatalogAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
				Session.CatalogAdminVisitLogged(true);
			}

			return View("Index", this.CatalogAdminViewModel);
        }
	}
}
