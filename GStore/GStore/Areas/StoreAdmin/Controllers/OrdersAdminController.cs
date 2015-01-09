using GStore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
	public class OrdersAdminController : BaseClasses.StoreAdminBaseController
    {
		public ActionResult AA_Status_Incomplete()
		{
			throw new NotImplementedException();
		}

		[AuthorizeGStoreAction(GStoreAction.Orders_Manager)]
        public ActionResult Manager()
        {
			return View("Manager", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.Orders_Carts_ViewAbandoned)]
		public ActionResult Carts_ViewAbandoned()
		{
			return View("Carts_ViewAbandoned", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Orders_Carts_ViewCurrent)]
		public ActionResult Carts_ViewCurrent()
		{
			return View("Carts_ViewCurrent", this.StoreAdminViewModel);
		}
		
		[AuthorizeGStoreAction(GStoreAction.Orders_Carts_TransferAbandonedCart)]
		public ActionResult TransferAbandonedCart()
		{
			return View("TransferAbandonedCart", this.StoreAdminViewModel);
		}
		
		[AuthorizeGStoreAction(GStoreAction.Orders_Carts_SendCheckOutLink)]
		public ActionResult Carts_SendCheckOutLink()
		{
			return View("Carts_SendCheckOutLink", this.StoreAdminViewModel);
		}
	}
}
		

