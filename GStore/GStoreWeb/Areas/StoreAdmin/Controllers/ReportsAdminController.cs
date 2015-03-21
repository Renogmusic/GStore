using System;
using System.Web.Mvc;
using GStoreData.Identity;

namespace GStoreWeb.Areas.StoreAdmin.Controllers
{
	public class ReportsAdminController : AreaBaseController.StoreAdminAreaBaseController
    {
		public ActionResult AA_Status_Incomplete()
		{
			throw new NotImplementedException();
		}

		[AuthorizeGStoreAction(GStoreAction.Reports_Manager)]
		public ActionResult Manager()
        {
			return View("Manager", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.Reports_AbandonedCarts_View)]
		public ActionResult AbandonedCartsReport()
		{
			return View("AbandonedCartsReport", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Reports_MonthlySales_View)]
		public ActionResult MonthlySalesReport()
        {
			return View("MonthlySalesReport", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.Reports_MonthlyVisitors_View)]
		public ActionResult MonthlyVisitorsReport()
        {
			return View("MonthlyVisitorsReport", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.Reports_MonthlyPageViews_View)]
		public ActionResult MonthlyPageViewsReport()
		{
			return View("MonthlyPageViewsReport", this.StoreAdminViewModel);
		}
	}
}
