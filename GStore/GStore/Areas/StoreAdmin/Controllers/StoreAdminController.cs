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

namespace GStore.Areas.StoreAdmin.Controllers
{
	public class StoreAdminController : BaseClasses.StoreAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Admin_StoreAdminArea)]
        public ActionResult Index()
        {
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null)
			{
				profile.LastStoreAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
			}

			return View("Index", this.StoreAdminViewModel);
        }
	}
}