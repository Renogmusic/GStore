using GStore.Identity;
using GStore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
	public class PageAdminController : BaseClasses.StoreAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Pages_Manager)]
		public ActionResult Manager()
		{
			return View("Manager", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Pages_Create)]
		public ActionResult Create()
		{
			return View("Create", this.StoreAdminViewModel);
		}

	}
}