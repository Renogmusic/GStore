using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Areas.CatalogAdmin.Controllers
{
	public class CatalogAdminController : BaseClasses.CatalogAdminBaseController
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
