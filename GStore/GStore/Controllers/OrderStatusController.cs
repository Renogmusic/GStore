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

		public ActionResult Download(string id, string email, int? orderItemId)
		{
			if (string.IsNullOrEmpty(id) || id == "0")
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Bad Url. OrderId missing", false, orderNumber: id, orderItemId: orderItemId);
				return HttpBadRequest("Order ID cannot be null or zero");
			}

			if (!orderItemId.HasValue || orderItemId == 0)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Bad Url. Order Item Id missing", false, orderNumber: id, orderItemId: orderItemId);
				return HttpBadRequest("Order Item ID cannot be null or zero");
			}

			Order order = CurrentStoreFrontOrThrow.Orders.Where(o => o.OrderNumber == id && o.Email.ToLower() == email.ToLower()).SingleOrDefault();
			if (order == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Order not found by Id and email. Order Id: " + id + " Email: " + email, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Order not found. Order Id: " + id + " Email: " + email);
			}

			OrderItem orderItem = order.OrderItems.Where(oi => oi.OrderItemId == orderItemId.Value).SingleOrDefault();
			if (orderItem == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Order Item not found by id. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Order Item not found by id. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value);
			}

			if (!orderItem.Product.DigitalDownload)
			{
				//no longer digital download
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Product is not set for Digital Download. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Product is not set for Digital Download. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value);
			}

			Product product = orderItem.Product;
			string filePath = product.DigitalDownloadFilePath(Request.ApplicationPath, RouteData, Server);
			string mimeType = MimeMapping.GetMimeMapping(filePath);
			FilePathResult result = new FilePathResult(filePath, mimeType);
			result.FileDownloadName = product.DigitalDownloadFileName;

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Success, id, true, orderNumber: id, orderItemId: orderItemId.Value);

			return result;
			//return new FilePathResult(filePath, mimeType) { FileDownloadName = XXX}

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
