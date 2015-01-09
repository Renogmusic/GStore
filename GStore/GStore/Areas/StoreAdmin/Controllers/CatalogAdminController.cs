using GStore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
    public class CatalogAdminController : BaseClasses.StoreAdminBaseController
    {
		public ActionResult AA_Status_Incomplete()
		{
			throw new NotImplementedException();
		}

		[AuthorizeGStoreAction(GStoreAction.Catalog_Manager)]
		public ActionResult Manager()
		{
			return View("Manager", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Catalog_Categories_Manager)]
		public ActionResult Categories()
		{
			return View("Categories", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Catalog_Products_Manager)]
		public ActionResult Products()
		{
			return View("Products", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Catalog_Categories_Images_Manager)]
		public ActionResult CategoryImages()
		{
			return View("CategoryImages", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Catalog_Products_Images_Manager)]
		public ActionResult ProductImages()
		{
			return View("ProductImages", this.StoreAdminViewModel);
		}
    }
}