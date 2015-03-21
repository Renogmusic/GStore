using System.Web.Mvc;
using GStoreData.Identity;

namespace GStoreWeb.Areas.OrderAdmin.Controllers
{
	public class GiftCardAdminController : AreaBaseController.OrderAdminAreaBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.GiftCards_Manager)]
        public ActionResult Index()
        {
			return View("Index", this.OrderAdminViewModel);
        }

	}
}
