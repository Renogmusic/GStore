using System.Web.Mvc;
using GStoreData.Identity;

namespace GStoreWeb.Areas.OrderAdmin.Controllers
{
	public class DiscountAdminController : AreaBaseController.OrderAdminAreaBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Discounts_Manager)]
        public ActionResult Index()
        {
			return View("Index", this.OrderAdminViewModel);
        }

	}
}
