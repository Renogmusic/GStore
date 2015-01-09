using GStore.Models;
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
				return HttpBadRequest("Order ID cannot be null or zero");
			}

			Order order = CurrentStoreFrontOrThrow.Orders.Where(o => o.OrderNumber == id && o.Email.ToLower() == email.ToLower()).SingleOrDefault();
			if (order == null)
			{
				return HttpNotFound("Order not found. Order Id: " + id + " Email: " + email);
			}

			return View("View", order);
		}


		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.OrderStatusLayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.OrderStatusTheme.FolderName;
			}
		}


	}
}
