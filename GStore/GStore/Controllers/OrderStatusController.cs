using GStore.Models;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Controllers
{
	public class OrderStatusController : BaseClass.BaseController
    {
		public ActionResult Index()
		{
			throw new NotImplementedException();
			//go to status page?
		}

		[ActionName("View")]
		public ActionResult ViewOrder(string id, string email)
		{
			if (string.IsNullOrEmpty(id) || id == "0")
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_View, "Bad Url", false);
				return HttpBadRequest("Order ID cannot be null or zero");
			}

			Order order = CurrentStoreFrontOrThrow.Orders.Where(o => o.OrderNumber == id && o.Email.ToLower() == email.ToLower()).SingleOrDefault();
			if (order == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_NotFound, id, false, orderNumber: id);
				return HttpNotFound("Order not found. Order Id: " + id + " Email: " + email);
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_View, id, true, orderNumber: id);

			return View("View", order);
		}


		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.OrdersTheme.FolderName;
			}
		}


	}
}
