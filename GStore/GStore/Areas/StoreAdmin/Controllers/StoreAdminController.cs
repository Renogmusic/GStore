using GStore.Areas.StoreAdmin.Models.ViewModels;
using GStore.Controllers.BaseClass;
using GStore.Models;
using GStore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
	[UserHasAnyAdminPermission]
    public class StoreAdminController : BaseClasses.AdminBaseController
    {
        // GET: StoreAdmin/Home
        public ActionResult Index()
        {
			AdminHomeViewModel model = new AdminHomeViewModel(GStoreDb, CurrentStoreFront, CurrentUserProfile);
            return View(model);
        }
    }

}