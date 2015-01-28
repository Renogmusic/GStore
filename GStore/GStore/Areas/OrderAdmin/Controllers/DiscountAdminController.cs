using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;

namespace GStore.Areas.OrderAdmin.Controllers
{
	public class DiscountAdminController : BaseClasses.OrderAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Discounts_Manager)]
        public ActionResult Index()
        {
			return View("Index", this.OrderAdminViewModel);
        }
	}
}