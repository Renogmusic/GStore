using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Areas.CatalogAdmin.Controllers
{
	public class ProductAdminController : BaseClasses.CatalogAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Products_Manager)]
        public ActionResult Manager()
        {
			return View("Manager", this.CatalogAdminViewModel);
        }
	}
}
