using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStoreData;
using GStoreData.Models;
using GStoreData.AppHtmlHelpers;

namespace GStoreWeb.Controllers
{
	public class OrderStatusController : AreaBaseController.RootAreaBaseController
    {
		[Authorize]
		public ActionResult Index()
		{
			UserProfile profile = CurrentUserProfileOrThrow;
			return View("Index", profile);
		}

		[ActionName("View")]
		public ActionResult ViewOrder(string id, string email)
		{
			if (string.IsNullOrEmpty(id) || id == "0")
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_View, "Bad Url - no order id", false);
				return HttpBadRequest("Order ID cannot be null or zero");
			}

			UserProfile profile = CurrentUserProfileOrNull;
			Order order = null;
			if (profile == null)
			{
				if (string.IsNullOrWhiteSpace(email))
				{
					GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_View, "Bad Url - no email", false);
					return HttpBadRequest("No Email specified");
				}
				order = CurrentStoreFrontOrThrow.Orders.Where(o => o.OrderNumber == id && o.Email.ToLower() == email.ToLower()).SingleOrDefault();
				if (order == null)
				{
					GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_NotFound, id, false, orderNumber: id);
					return HttpNotFound("Order not found. Order Id: " + id + " Email: " + email);
				}

			}
			else
			{
				if (profile.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					order = CurrentStoreFrontOrThrow.Orders.Where(o => o.OrderNumber == id).SingleOrDefault();
					if (order == null)
					{
						GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_NotFound, "SysAdmin Order id: " + id, false, orderNumber: id);
						return HttpNotFound("SysAdmin Order not found. Order Id: " + id);
					}
				}
				else if (CurrentStoreFrontConfigOrThrow.OrderAdmin_UserProfileId == profile.UserProfileId)
				{
					order = CurrentStoreFrontOrThrow.Orders.Where(o => o.OrderNumber == id).SingleOrDefault();
					if (order == null)
					{
						GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_NotFound, "Order Admin Order id: " + id, false, orderNumber: id);
						return HttpNotFound("Order Admin Order not found. Order Id: " + id);
					}
				}
				else
				{
					order = CurrentStoreFrontOrThrow.Orders.Where(o => o.OrderNumber == id && o.UserProfileId == profile.UserProfileId).SingleOrDefault();
					if (order == null)
					{
						GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_NotFound, id, false, orderNumber: id);
						return HttpNotFound("Order not found. Order Id: " + id + " User Profile Id: " + profile.UserProfileId);
					}

				}
			}

			if (order.UserProfile != null)
			{
				if (CurrentUserProfileOrNull == null)
				{
					GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_View, "Not logged in", false, orderNumber: id);
					AddUserMessage("Login Required", "You must log in to view your order.", GStoreData.AppHtmlHelpers.UserMessageType.Info);
					return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
				}
				else if (CurrentUserProfileOrThrow.UserProfileId != order.UserProfileId)
				{
					//order is for another user, ask for login
					GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_View, "Order is for another user", false, orderNumber: id);
					AddUserMessage("Login Required", "This order was placed by another user. Please log in to view.", GStoreData.AppHtmlHelpers.UserMessageType.Info);
					return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
				}
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_View, id, true, orderNumber: id);

			return View("View", order);
		}

		public ActionResult Download(string id, string email, int? orderItemId)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;

			IGstoreDb db = GStoreDb;
			if (string.IsNullOrEmpty(id) || id == "0")
			{
				db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Bad Url. OrderId missing", false, orderNumber: id, orderItemId: orderItemId);
				return HttpBadRequest("Order ID cannot be null or zero");
			}

			if (!orderItemId.HasValue || orderItemId == 0)
			{
				db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Bad Url. Order Item Id missing", false, orderNumber: id, orderItemId: orderItemId);
				return HttpBadRequest("Order Item ID cannot be null or zero");
			}

			Order order = config.StoreFront.Orders.Where(o => o.OrderNumber == id && o.Email.ToLower() == email.ToLower()).SingleOrDefault();
			if (order == null)
			{
				db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Order not found by Id and email. Order Id: " + id + " Email: " + email, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Order not found. Order Id: " + id + " Email: " + email);
			}

			OrderItem orderItem = order.OrderItems.Where(oi => oi.OrderItemId == orderItemId.Value).SingleOrDefault();
			if (orderItem == null)
			{
				db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Order Item not found by id. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Order Item not found by id. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value);
			}

			Product product = orderItem.Product;
			if (!product.DigitalDownload)
			{
				//no longer digital download
				db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Product '" + product.UrlName + "' [" + product.ProductId + "] is not set for Digital Download. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Product '" + product.UrlName + "' [" + product.ProductId + "] is not set for Digital Download. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value);
			}

			if (string.IsNullOrEmpty(product.DigitalDownloadFileName))
			{
				//no file name
				string errorMessage = "There is no file set for digital download for Product '" + product.Name.ToHtmlLines() + "' [" + product.ProductId + "]";
				db.CreateDigitalDownloadErrorNotificationToOrderAdminAndSave(config, CurrentUserProfileOrNull, orderItem, Url, errorMessage);

				db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Product '" + product.UrlName + "' [" + product.ProductId + "] has no file set for Digital Download. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Product '" + product.UrlName + "' [" + product.ProductId + "] has no file set for Digital Download. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value);
			}

			string filePath = product.DigitalDownloadFilePath(Request.ApplicationPath, RouteData, Server);
			if (string.IsNullOrEmpty(filePath))
			{
				string errorMessage = "File Not Found '" + filePath.ToHtmlLines()  + "' for digital download for Product '" + product.Name + "' [" + product.ProductId + "]";
				db.CreateDigitalDownloadErrorNotificationToOrderAdminAndSave(config, CurrentUserProfileOrNull, orderItem, Url, errorMessage);


				//file not found
				db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Failure, "Product '" + product.UrlName + "' [" + product.ProductId + "] digital download file '" + product.DigitalDownloadFileName + "' is not found. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value, false, orderNumber: id, orderItemId: orderItemId.Value);
				return HttpNotFound("Product '" + product.UrlName + "' [" + product.ProductId + "] digital download file '" + product.DigitalDownloadFileName + "' is not found. Order Id: " + id + " Email: " + email + " Order Item Id: " + orderItemId.Value);
			}

			string mimeType = MimeMapping.GetMimeMapping(filePath);
			FilePathResult result = new FilePathResult(filePath, mimeType);
			result.FileDownloadName = product.DigitalDownloadFileName;

			db.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_DigitalDownload_Success, id, true, orderNumber: id, orderItemId: orderItemId.Value);

			//update order item status and order status
			if (!orderItem.StatusItemDownloaded)
			{
				orderItem.StatusItemDownloaded = true;
				if (order.OrderItems.Any(oi => !oi.StatusItemDownloaded))
				{
					order.StatusOrderDownloaded = false;
				}
				else
				{
					//all items are downloaded
					order.StatusOrderDownloaded = true;
				}
				db.SaveChanges();
			}

			return result;
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
