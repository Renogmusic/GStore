using GStore.Areas.StoreAdmin.Models.ViewModels;
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
			return View("Index", this.StoreAdminViewModel);
        }
	}
}