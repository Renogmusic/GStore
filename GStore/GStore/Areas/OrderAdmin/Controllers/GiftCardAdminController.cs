using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;

namespace GStore.Areas.OrderAdmin.Controllers
{
	public class GiftCardAdminController : BaseClasses.OrderAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.GiftCards_Manager)]
        public ActionResult Index()
        {
			return View("Index", this.OrderAdminViewModel);
        }
	}
}