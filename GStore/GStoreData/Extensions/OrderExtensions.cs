using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.ControllerBase;
using GStoreData.Exceptions;
using GStoreData.Models;
using GStoreData.PayPal.Models;

namespace GStoreData
{
	public static class OrderExtensions
	{
		public static Order CreateOrderFromCartAndSave(this Cart cart, StoreFrontConfiguration config, Payment payment, IGstoreDb db)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			Order order = db.Orders.Create();

			lock (orderNumberLock)
			{
				order.OrderNumber = cart.StoreFront.GetAndIncrementOrderNumber(db).ToString();
			}

			order.OriginalCartId = cart.CartId;
			order.Client = cart.Client;
			order.ClientId= cart.ClientId;
			order.CreateDateTimeUtc = DateTime.UtcNow;
			order.CreatedBy= cart.CreatedBy;
			order.CreatedBy_UserProfileId= cart.CreatedBy_UserProfileId;
			order.Discount= cart.Discount;
			order.DiscountCode= cart.DiscountCode;
			order.DiscountCodesAttempted= cart.DiscountCodesAttempted;
			order.DiscountCodesFailed= cart.DiscountCodesFailed;
			order.DiscountId= cart.DiscountId;
			order.Email= cart.Email;
			order.FullName = cart.FullName;
			order.EndDateTimeUtc= DateTime.UtcNow.AddYears(100);
			order.EntryDateTimeUtc= cart.EntryDateTimeUtc;
			order.EntryRawUrl= cart.EntryRawUrl;
			order.EntryReferrer= cart.EntryReferrer;
			order.EntryUrl= cart.EntryUrl;
			order.Handling= cart.Handling;
			order.IPAddress= cart.IPAddress;
			order.IsPending= false;
			order.ItemCount= cart.ItemCount;
			order.OrderDiscount= cart.OrderDiscount;
			order.LoginOrGuestWebFormResponse = cart.LoginOrGuestWebFormResponse;
			order.PaymentInfoWebFormResponse = cart.CartPaymentInfo == null ? null : cart.CartPaymentInfo.WebFormResponse;
			order.ConfirmOrderWebFormResponse = cart.ConfirmOrderWebFormResponse;
			order.DeliveryInfoDigital = cart.DeliveryInfoDigital;
			order.DeliveryInfoShipping = cart.DeliveryInfoShipping;
			order.AllItemsAreDigitalDownload = cart.AllItemsAreDigitalDownload;
			order.DigitalDownloadItemCount = cart.DigitalDownloadItemCount;
			order.ShippingItemCount = cart.ShippingItemCount;

			order.RefundedAmount = 0;
			order.SessionId= cart.SessionId;
			order.Shipping= cart.Shipping;

			order.StartDateTimeUtc= DateTime.UtcNow.AddMinutes(-1);
			order.StoreFront= cart.StoreFront;
			order.StoreFrontId= cart.StoreFrontId;
			order.Subtotal= cart.Subtotal;
			order.Tax= cart.Tax;
			order.Total= cart.Total;
			order.UpdateDateTimeUtc= DateTime.UtcNow;
			order.UpdatedBy= cart.UpdatedBy;
			order.UpdatedBy_UserProfileId= cart.UpdatedBy_UserProfileId;
			order.UserAgent= cart.UserAgent;
			order.UserProfile= cart.UserProfile;
			order.UserProfileId= cart.UserProfileId;

			order = db.Orders.Add(order);
			db.SaveChanges();

			//note: payment can be null in a PO or invoicing scenario
			if (payment == null)
			{
				order.StatusOrderPaymentProcessed = false;
			}
			else
			{
				if (payment.IsProcessed)
				{
					order.StatusOrderPaymentProcessed = true;
					if (config.Orders_AutoAcceptPaid)
					{
						order.StatusOrderAccepted = true;
					}
					else if (order.AllItemsAreDigitalDownload)
					{
						//always auto-accept digital download orders because they are available immediately
						order.StatusOrderAccepted = true;
					}
				}
				//link payment to order
				payment.Order = order;
				payment.OrderId = order.OrderId;

				payment = db.Payments.Update(payment);
			}

			foreach (CartBundle cartBundle in cart.CartBundles.AsQueryable().ApplyDefaultSort())
			{
				OrderBundle orderBundle = db.OrderBundles.Create();
				orderBundle.ProductBundleId = cartBundle.ProductBundleId;

				orderBundle.Client = cartBundle.Client;
				orderBundle.ClientId = cartBundle.ClientId;
				orderBundle.StoreFront = cartBundle.StoreFront;
				orderBundle.StoreFrontId = cartBundle.StoreFrontId;
				orderBundle.CartBundleId = cartBundle.CartBundleId;
				orderBundle.Order = order;

				orderBundle.Quantity = cartBundle.Quantity;
				orderBundle.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				orderBundle.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				orderBundle.IsPending = false;

				orderBundle = db.OrderBundles.Add(orderBundle);
			}

			if (cart.CartBundles != null && cart.CartBundles.Count != 0)
			{
				db.SaveChanges();
			}


			//add order items;
			foreach (CartItem item in cart.CartItems.AsQueryable().ApplyDefaultSort())
			{
				OrderItem orderItem = db.OrderItems.Create();
				orderItem.CartItem = item;
				orderItem.CartItemId = item.CartItemId;
				orderItem.Client = item.Client;
				orderItem.ClientId = item.ClientId;
				orderItem.CreateDateTimeUtc = DateTime.UtcNow;
				orderItem.CreatedBy = item.CreatedBy;
				orderItem.CreatedBy_UserProfileId = item.CreatedBy_UserProfileId;
				orderItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				orderItem.IsPending = false;
				orderItem.ItemRefundedAmount = 0;
				orderItem.IsDigitalDownload = item.Product.DigitalDownload;
				if (payment != null && payment.IsProcessed)
				{
					orderItem.StatusItemPaymentReceived = true;
				}
				else
				{
					orderItem.StatusItemPaymentReceived = false;
				}
				orderItem.StatusItemPaymentReceived = order.StatusOrderPaymentProcessed;
				orderItem.StatusItemAccepted = order.StatusOrderAccepted;
				orderItem.LineDiscount = item.LineDiscount;
				orderItem.LineTotal = item.LineTotal;
				orderItem.ListPrice = item.ListPrice;
				orderItem.ListPriceExt = item.ListPriceExt;
				orderItem.Order = order;
				orderItem.OrderId = order.OrderId;
				orderItem.Product = item.Product;
				orderItem.ProductId = item.ProductId;
				orderItem.ProductVariantInfo = item.ProductVariantInfo;
				orderItem.Quantity = item.Quantity;
				orderItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				orderItem.StoreFront = item.StoreFront;
				orderItem.StoreFrontId = item.StoreFrontId;
				orderItem.UnitPrice = item.UnitPrice;
				orderItem.UnitPriceExt = item.UnitPriceExt;
				orderItem.UpdateDateTimeUtc = DateTime.UtcNow;
				orderItem.UpdatedBy = item.UpdatedBy;
				orderItem.UpdatedBy_UserProfileId = item.UpdatedBy_UserProfileId;
				orderItem.ProductBundleItemId = item.ProductBundleItemId;

				if (item.CartBundleId.HasValue)
				{
					OrderBundle orderBundle = order.OrderBundles.Single(ob => ob.CartBundleId == item.CartBundleId);
					orderItem.OrderBundleId = orderBundle.OrderBundleId;
				}
				orderItem = db.OrderItems.Add(orderItem);
			}

			db.SaveChanges();

			return order;

		}

		/// <summary>
		/// Uses controller for StoreFrontConfig, AddUserMessage, and UserProfile, and GStoreDb
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="controller"></param>
		/// <returns></returns>
		public static ActionResult ProcessOrderAndPayment(this Cart cart, BaseController controller, string orderEmailHtmlPartialViewName = "_OrderEmailHtmlPartial", string orderEmailTextPartialViewName = "_OrderEmailTextPartial")
		{
			//process order
			if (controller == null)
			{
				throw new ArgumentNullException("controller", "controller needed to process an order and payment.");
			}
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (cart.OrderId.HasValue)
			{
				throw new ApplicationException("cart.OrderId.HasValue is set. This cart already has been processed and converted into an order. Order Id: " + cart.OrderId.Value);
			}
			if (cart.StatusPlacedOrder)
			{
				throw new ApplicationException("cart.StatusPlacedOrder = true. This cart already has been processed and converted into an order.");
			}

			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrThrow;
			IGstoreDb db = controller.GStoreDb;
			UserProfile userProfile = controller.CurrentUserProfileOrNull;

			Payment payment = null;
			if (config.PaymentMethod_PayPal_Enabled)
			{
				try
				{
					payment = cart.ProcessPayPalPaymentForOrderAndSavePayment(config, db);
				}
				catch (PayPalExceptionOAuthFailed exOAuth)
				{
					string message = "Sorry, this store's configuration for PayPal OAuth is not operational. Please contact us for other payment options."
						+ (exOAuth.IsSandbox ? "\nError in Sandbox Config." : "\nError in Live Config");
					controller.AddUserMessage("PayPal Error", message, UserMessageType.Danger);

					if (userProfile != null && userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						string adminMessage = exOAuth.ToString()
							+ "\n\nHTTP Response:\n" + exOAuth.ResponseString
							+ "\n\nHTTP Headers:\n" + exOAuth.ResponseHeaders;
						controller.AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
					}

					cart.StatusPaymentInfoConfirmed = false;
					db.Carts.Update(cart);
					db.SaveChanges();

					return controller.RedirectToActionResult("PaymentInfo", "Checkout");
				}
				catch (PayPalExceptionCreatePaymentFailed exPaymentFailed)
				{
					string message = "Sorry, there was an error sending your order to PayPal for payment. Please contact us for other payment options."
						+ (exPaymentFailed.IsSandbox ? "\nError in Sandbox." : "\nError in Live Site.");

					controller.AddUserMessage("PayPal Error", message, UserMessageType.Danger);

					if (userProfile != null && userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						string adminMessage = exPaymentFailed.ToString()
							+ "\n\nHTTP Response:\n" + exPaymentFailed.ResponseString
							+ "\n\nHTTP Headers:\n" + exPaymentFailed.ResponseHeaders;
						controller.AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
					}

					cart.StatusPaymentInfoConfirmed = false;
					db.Carts.Update(cart);
					db.SaveChanges();

					return controller.RedirectToActionResult("PaymentInfo", "Checkout");
				}
				catch (Exception ex)
				{
					string message = "Sorry, there was an error starting starting your order with PayPal. Please contact us for other payment options.";
					controller.AddUserMessage("PayPal Error", message, UserMessageType.Danger);

					if (userProfile != null && userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						string adminMessage = "Exception: " + ex.ToString();
						controller.AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
					}

					cart.StatusPaymentInfoConfirmed = false;
					db.Carts.Update(cart);
					db.SaveChanges();

					return controller.RedirectToActionResult("PaymentInfo", "Checkout");
				}

				if (payment.PaymentFailed)
				{
					//fire the user back to "enter payment info"
					cart.StatusPaymentInfoConfirmed = false;
					db.Carts.Update(cart);
					db.SaveChanges();

					controller.AddUserMessage("Payment Failed!", "Sorry, there was a problem processing your payment with PayPal. Please try again or contact us if you continue to get this error.", UserMessageType.Danger);

					if (userProfile != null && userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						string adminMessage = "PayPal Payment Failed see JSON\n" + payment.Json;
						controller.AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
					}
					return controller.RedirectToActionResult("PaymentInfo", "Checkout");
				}
			}
			else
			{
				//no payment method in store, continue with pay after order flow
			}

			Order order = cart.CreateOrderFromCartAndSave(config, payment, db);
			cart.OrderId = order.OrderId;
			cart.StatusPlacedOrder = true;
			cart = db.Carts.Update(cart);

			if (payment != null)
			{
				payment.OrderId = order.OrderId;
				payment = db.Payments.Update(payment);
			}

			Discount discount = cart.Discount;
			if (discount != null)
			{
				discount.UseCount++;
				discount = db.Discounts.Update(discount);
			}
			db.SaveChanges();

			db.LogUserActionEvent(controller.HttpContext, controller.RouteData, controller, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_PlacedOrder, "", true, cartId: cart.CartId, orderNumber: order.OrderNumber);

			db.CreateNewOrderNotificationToOrderAdminAndSave(config, userProfile, order, controller.Url);

			string userMessage;
			bool emailResult = order.SendOrderReceipt(controller, out userMessage, orderEmailHtmlPartialViewName, orderEmailTextPartialViewName);

			if (emailResult)
			{
				controller.AddUserMessage("Your Order is Placed!", userMessage, UserMessageType.Info);
			}
			else
			{
				controller.AddUserMessage("Your Order is Placed", userMessage, UserMessageType.Danger);
			}

			
			return new RedirectResult(controller.Url.Action(actionName: "View", controllerName: "OrderStatus", routeValues: new { id = order.OrderNumber, Email = order.Email }));
		}

		/// <summary>
		/// Processes paypal payment on an order if it has not been done already
		/// Saves Payment to cart, does not mark cart status
		/// Exceptions thrown if cart.StatusPlacedOrder = true or cart.OrderId.HasValue
		/// Otherwise, payment will be processed and if failed, a record with failure code will be returned
		/// </summary>
		/// <returns></returns>
		public static Payment ProcessPayPalPaymentForOrderAndSavePayment(this Cart cart, StoreFrontConfiguration storeFrontConfig, IGstoreDb db)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (!cart.StatusPaymentInfoConfirmed)
			{
				throw new ApplicationException("cart.StatusPaymentInfoConfirmed = false. Be sure cart is updated with payment info status = true when payment is selected.");
			}
			if (cart.CartPaymentInfo == null)
			{
				throw new ArgumentNullException("cart.PaymentInfo");
			}
			if (cart.OrderId.HasValue)
			{
				throw new ApplicationException("cart.OrderId.HasValue is set. This cart already has been processed and converted into an order. Order Id: " + cart.OrderId.Value);
			}
			if (cart.StatusPlacedOrder)
			{
				throw new ApplicationException("cart.StatusPlacedOrder = true. This cart already has been processed and converted into an order.");
			}
			if (cart.CartPaymentInfo.PayPalPaymentId.ToLower() != cart.CartPaymentInfo.PayPalReturnPaymentId.ToLower())
			{
				throw new ApplicationException("PayPal Payment id mismatch. PaymentId: " + cart.CartPaymentInfo.PayPalPaymentId + " ReturnPaymentId: " + cart.CartPaymentInfo.PayPalReturnPaymentId);
			}
			if (cart.CartPaymentInfo.PaymentSource != Models.BaseClasses.GStorePaymentSourceEnum.PayPal)
			{
				throw new ApplicationException("Payment Source not supported: " + cart.CartPaymentInfo.PaymentSource.ToString());
			}

			Payment payment = db.Payments.Create();
			payment.SetDefaultsForNew(storeFrontConfig);
			payment.PaymentSource = cart.CartPaymentInfo.PaymentSource;
			payment.ProcessDateTimeUtc = DateTime.UtcNow;
			payment.CartId = cart.CartId;

			PayPalPaymentData response = new PayPal.Models.PayPalPaymentData();
			PayPal.PayPalPaymentClient paypalClient = new PayPal.PayPalPaymentClient();
			try
			{
				response = paypalClient.ExecutePayPalPayment(storeFrontConfig, cart);
				if (response.transactions == null || response.transactions.Count() == 0)
				{
					throw new ApplicationException("PayPal transactions missing from response. JSON: " + response.Json);
				}
				payment.SetFromPayPalPayment(response);
				payment.PaymentFailed = false;
				payment.FailureException = null;
				payment.IsProcessed = true;
			}
			catch (Exceptions.PayPalExceptionOAuthFailed exOAuth)
			{
				payment.PaymentFailed = true;
				payment.FailureException = exOAuth.ToString();
				payment.Json = response.Json;
				payment.AmountPaid = 0M;
				payment.TransactionId = null;
				payment.IsProcessed = false;
			}
			catch (Exceptions.PayPalExceptionExecutePaymentFailed exPaymentFailed)
			{
				payment.PaymentFailed = true;
				payment.FailureException = exPaymentFailed.ToString();
				payment.Json = response.Json;
				payment.AmountPaid = 0M;
				payment.TransactionId = null;
				payment.IsProcessed = false;
			}
			catch (Exception ex)
			{
				payment.PaymentFailed = true;
				payment.FailureException = ex.ToString();
				payment.Json = response.Json;
				payment.AmountPaid = 0M;
				payment.TransactionId = null;
				payment.IsProcessed = false;
			}

			db.Payments.Add(payment);
			db.SaveChanges();

			return payment;
		}

		private static object orderNumberLock = new Object();

		public static int GetAndIncrementOrderNumber(this StoreFront storeFront, IGstoreDb db)
		{
			lock (orderNumberLock)
			{
				IGstoreDb newDb = db.NewContext();
				int orderNumber = 1001;
				StoreFront safeStoreFront = newDb.StoreFronts.Single(s => s.StoreFrontId == storeFront.StoreFrontId);
				if (safeStoreFront.NextOrderNumber > 0)
				{
					orderNumber = safeStoreFront.NextOrderNumber;
				}

				safeStoreFront.NextOrderNumber = orderNumber + 1;
				safeStoreFront = newDb.StoreFronts.Update(safeStoreFront);
				newDb.SaveChangesDirect();

				storeFront = db.Refresh<StoreFront>(storeFront);
				return orderNumber;
			}
		}

		public static string ToOrderDetails(this OrderItem data)
		{
			if (data == null)
			{
				return "(none)";
			}

			return data.Product.Name + " [" + data.ProductId + "]"
				+ (!string.IsNullOrEmpty(data.ProductVariantInfo) ? " Variant: " + data.ProductVariantInfo : "")
				+ " Qty: " + data.Quantity
				+ " Line Total: $" + data.LineTotal.ToString("N2")
				+ " Unit Price: $" + data.UnitPrice.ToString("N2")
				+ " Unit Price Ext: $" + data.UnitPriceExt.ToString("N2")
				+ " Line Discount: $" + data.LineDiscount.ToString("N2")
				+ (data.IsDigitalDownload ? " Digital Download-Yes" : "")
				+ (data.IsDigitalDownload ? " File: " + data.Product.DigitalDownloadFileName : "");
		}

		public static string ToOrderDetails(this DeliveryInfoShipping data, bool includeCustomFormFields)
		{
			if (data == null)
			{
				return "(none)";
			}

			string text = "Ship via " + data.ShippingDeliveryMethod.ToDisplayName();
			if (data.DeliveryMethodWebFormResponse != null)
			{
				text += data.DeliveryMethodWebFormResponse.ToSimpleTextForCheckout(true);
			}

			text+="\nShip To: " + data.FullName
				+ "\nAddress: " + data.AdddressL1
				+ (string.IsNullOrWhiteSpace(data.AdddressL2) ? "" : "\nAddress Line 2: " + data.AdddressL2)
				+ "\nCity/State/Zip: " + data.City + ", " + data.State + " " + data.PostalCode
				+ "\nEmail: " + data.EmailAddress;

			if (data.WebFormResponse != null && includeCustomFormFields)
			{
				text += data.WebFormResponse.ToSimpleTextForCheckout(true);
			}
			return text;
		}

		public static string ToOrderDetails(this DeliveryInfoDigital data, bool includeCustomFormFields)
		{
			if (data == null)
			{
				return "(none)";
			}
			string text = "Email to " + data.EmailAddress;

			if (data.WebFormResponse != null && includeCustomFormFields)
			{
				text += data.WebFormResponse.ToSimpleTextForCheckout(true);
			}
			return text;
		}

		public static void CreateNewOrderNotificationToOrderAdminAndSave(this IGstoreDb db, StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, Order order, UrlHelper urlHelper)
		{
			//user profile is null if user purchased as a guest
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			if (order == null)
			{
				throw new ArgumentNullException("order");
			}
			if (urlHelper == null)
			{
				throw new ArgumentNullException("urlHelper");
			}

			Uri currentUrl = urlHelper.RequestContext.HttpContext.Request.Url;
			string orderAdminUrl = urlHelper.Action("View", "Orders", new { area = "OrderAdmin", id = order.OrderNumber });
			string userViewOrderStatusUrl = urlHelper.Action("View", "OrderStatus", new { area = "", id = order.OrderNumber, Email = order.Email });

			UserProfile orderAdmin = storeFrontConfig.OrderAdmin;
			Notification notification = db.Notifications.Create();

			notification.StoreFront = storeFrontConfig.StoreFront;
			notification.StoreFront = storeFrontConfig.StoreFront;
			notification.ClientId = storeFrontConfig.ClientId;
			notification.Client = storeFrontConfig.Client;
			notification.From = "GStore Order Admin";
			notification.FromUserProfileId = orderAdmin.UserProfileId;
			notification.OrderId = order.OrderId;
			notification.ToUserProfileId = orderAdmin.UserProfileId;
			notification.To = orderAdmin.FullName;

			notification.Importance = "Normal";
			notification.Subject = "New Order #" + order.OrderNumber + " placed on " + currentUrl.Host + " at " + order.CreateDateTimeUtc.ToStoreDateTimeString(storeFrontConfig, storeFrontConfig.Client);

			notification.UrlHost = currentUrl.Host;
			if (!currentUrl.IsDefaultPort)
			{
				notification.UrlHost += ":" + currentUrl.Port;
			}
			notification.BaseUrl = urlHelper.Action("Details", "Notifications", new { area = "", id = "" });
			notification.Message = notification.Subject
				+ "\n\nOrder Number: " + order.OrderNumber
				+ "\nStore Front: " + storeFrontConfig.Name + " [" + storeFrontConfig.StoreFrontId + "]"
				+ "\nClient: " + order.Client.Name + " [" + order.ClientId + "]"
				+ "\nDate/Time: " + order.CreateDateTimeUtc.ToStoreDateTimeString(storeFrontConfig, storeFrontConfig.Client)
				+ "\n"
				+ "\nUser: " + (order.UserProfile == null ? "(guest)" : order.UserProfile.FullName + " <" + order.UserProfile.Email + "> [" + order.UserProfileId + "]")
				+ "\nEmail: " + order.Email;

			if (order.LoginOrGuestWebFormResponse != null)
			{
				//add custom fields from LoginOrGuestWebFormResponse
				notification.Message += order.LoginOrGuestWebFormResponse.ToSimpleTextForCheckout(true);
			}

			notification.Message += "\n"
				+ "\nTax: $" + order.Tax.ToString("N2")
				+ "\nShipping: $" + order.Shipping.ToString("N2")
				+ "\nHandling: $" + order.Handling.ToString("N2")
				+ "\nDiscount Code: " + order.DiscountCode.OrDefault("(none)")
				+ "\nDiscount: " + order.Discount.ToStringDetails()
				+ "\nOrder Discount: $" + order.OrderDiscount.ToString("N2")
				+ "\nSubtotal: $" + order.Subtotal.ToString("N2")
				+ "\nTotal: $" + order.Total.ToString("N2")
				+ "\nPaid: $" + (order.Payments.Sum(p => p.AmountPaid).ToString("N2"));

			if (order.PaymentInfoWebFormResponse != null)
			{
				//add custom fields from PaymentInfoWebFormResponse
				notification.Message += order.PaymentInfoWebFormResponse.ToSimpleTextForCheckout(true);
			}
			notification.Message += "\n";
			notification.Message += "\nOrder Confirmed";
			if (order.ConfirmOrderWebFormResponse != null)
			{
				//add custom fields from LoginOrGuestWebFormResponse
				notification.Message += order.ConfirmOrderWebFormResponse.ToSimpleTextForCheckout(true);
			}

			notification.Message += "\n";
			if (order.DeliveryInfoShippingId.HasValue)
			{
				notification.Message += "\nShip To: " + order.DeliveryInfoShipping.ToOrderDetails(true) + "\n";
			}

			if (order.DeliveryInfoDigitalId.HasValue)
			{
				notification.Message += "\nDigital Delivery To: " + order.DeliveryInfoDigital.ToOrderDetails(true) + "\n";
			}

			notification.Message += "\nItem Count: " + order.ItemCount
				+ "\nAll Items are Digital Download: " + (order.AllItemsAreDigitalDownload ? "Yes" : "No")
				+ "\nItems to Ship: " + order.ShippingItemCount.ToString("N0")
				+ "\nDigital Download Item Count: " + order.DigitalDownloadItemCount
				+ "\n\nOrder Items: " + order.OrderItems.Count;

			int counter = 0;
			foreach (OrderItem item in order.OrderItems.OrderBy(oi => oi.OrderItemId))
			{
				counter++;
				notification.Message += "\nOrder Item " + counter + " " + item.ToOrderDetails();
			}

			notification.Message += "\n\nOrder Id: " + order.OrderId
				+ "\nCart Id: " + order.OriginalCartId
				+ "\nUser Agent: " + order.UserAgent
				+ "\nSession Id: " + order.SessionId
				+ "\nIP Address: " + order.IPAddress
				+ "\nEntry Url: " + order.EntryUrl
				+ "\nEntry Referrer: " + order.EntryReferrer
				+ "\nEntry Raw Url: " + order.EntryRawUrl
				+ "\nEntry Date Time: " + order.EntryDateTimeUtc.ToStoreDateTimeString(storeFrontConfig, storeFrontConfig.Client)
				+ "\nDiscount Codes Attempted: " + order.DiscountCodesAttempted.OrDefault("(none)")
				+ "\nDiscount Codes Failed: " + order.DiscountCodesFailed.OrDefault("(none)");

			notification.IsPending = false;
			notification.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			notification.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			List<NotificationLink> links = new List<NotificationLink>();

			NotificationLink link1 = db.NotificationLinks.Create();
			
			link1.SetDefaultsForNew(notification);
			link1.Order = 1;
			link1.LinkText = "Admin Order #" + order.OrderNumber;
			link1.Url = orderAdminUrl;
			link1.IsExternal = false;
			links.Add(link1);

			NotificationLink link2 = db.NotificationLinks.Create();
			link2.SetDefaultsForNew(notification);
			link2.Order = 2;
			link2.LinkText = "Order Status Link for User";
			link2.Url = userViewOrderStatusUrl;
			link2.IsExternal = false;
			links.Add(link2);

			notification.NotificationLinks = links;
			db.Notifications.Add(notification);

			db.SaveChanges();
		}


		/// <summary>
		/// Shows order status (not order items)
		/// </summary>
		/// <param name="order"></param>
		/// <param name="allStatus"></param>
		/// <returns></returns>
		public static string StatusTextString(this Order order, string delimiter = ", ")
		{
			if (order == null)
			{
				throw new ArgumentNullException("item");
			}
			List<string> status = new List<string>();
			if (!order.StatusOrderAccepted)
			{
				status.Add("Order Awaiting Processing");
			}
			else
			{
				status.Add("Order Accepted");
			}

			if (order.StatusOrderPaymentProcessed)
			{
				status.Add("Order Paid");
			}
			if (order.StatusOrderShipped)
			{
				status.Add("Order Shipped");
			}
			if (order.StatusOrderDelivered)
			{
				status.Add("Order Delivered");
			}
			if (order.StatusOrderDownloaded)
			{
				status.Add("Order Downloaded");
			}
			if (order.StatusOrderFeedbackReceived)
			{
				status.Add("Order Feedback Received");
			}

			if (order.StatusOrderCancelledByMerchant)
			{
				status.Add("Order Canceled by Seller");
			}
			if (order.StatusOrderCancelledByUser)
			{
				status.Add("Order Canceled by Buyer");
			}
			if (order.StatusOrderEditedByMerchant)
			{
				status.Add("Order Edited by Seller");
			}
			if (order.StatusOrderEditedByUser)
			{
				status.Add("Order Editied by Buyer");
			}
			if (order.StatusOrderItemsReturned)
			{
				status.Add("Order Items Returned");
			}

			return string.Join(delimiter, status);
		}


		/// <summary>
		/// Shows item status
		/// </summary>
		/// <param name="item"></param>
		/// <param name="allStatus"></param>
		/// <returns></returns>
		public static string StatusTextString(this OrderItem item, string delimiter = ", ")
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			List<string> status = new List<string>();
			if (!item.StatusItemAccepted)
			{
				status.Add("Awaiting Processing");
			}
			else
			{
				status.Add("Accepted");
			}

			if (item.StatusItemPaymentReceived)
			{
				status.Add("Paid");
			}
			if (item.StatusItemShipped)
			{
				status.Add("Shipped");
			}
			if (item.StatusItemDelivered)
			{
				status.Add("Delivered");
			}
			if (item.StatusItemDownloaded)
			{
				status.Add("Downloaded");
			}
			if (item.StatusItemFeedbackReceived)
			{
				status.Add("Feedback Received");
			}

			if (item.StatusItemCancelledByMerchant)
			{
				status.Add("Canceled by Seller");
			}
			if (item.StatusItemCancelledByUser)
			{
				status.Add("Canceled by Buyer");
			}
			if (item.StatusItemEditedByMerchant)
			{
				status.Add("Edited by Seller");
			}
			if (item.StatusItemEditedByUser)
			{
				status.Add("Editied by Buyer");
			}
			if (item.StatusItemReturned)
			{
				status.Add("Returned");
			}

			return string.Join(delimiter, status);
		}

		public static Cart OriginalCart(this Order order)
		{
			if (order== null)
			{
				throw new ArgumentNullException("order");
			}
			if (order.OriginalCartId == null)
			{
				return null;
			}
			return order.Carts.Where(c => c.CartId == order.OriginalCartId).SingleOrDefault();

		}

		public static void SetFromPayPalPayment(this Payment payment, PayPalPaymentData response)
		{
			if (response.transactions == null)
			{
				throw new ArgumentNullException("response.transactions");
			}
			if (response.transactions.Count() != 1)
			{
				throw new ArgumentOutOfRangeException("response.transactions", "response.transactions count is invalid in the response. count must be 1. current count: " + response.transactions.Count());
			}

			PayPalTransactionData transaction = response.transactions.SingleOrDefault();
			PayPalPayerData payer = response.payer;
			if (!response.payer.payer_info.HasValue)
			{
				throw new ArgumentNullException("response.payer.payer_info");
			}
			PayPalPayerInfoData payer_info = response.payer.payer_info.Value;

			if (payer.funding_instruments != null && payer.funding_instruments.Count() != 1 && payer.funding_instruments.Count() != 0)
			{
				//wrong number of funding instruments, only 1 is supported
				throw new ArgumentOutOfRangeException("response.payer.funding_instruments", "funding_instruments count is invalid in the response. count must be 1 or 0 (or null). current count: " + payer.funding_instruments.Count());
			}
			PayPalCreditCardData? directCreditCard = null;
			if (payer.funding_instruments != null && payer.funding_instruments.Count() == 1)
			{
				directCreditCard = payer.funding_instruments[0].credit_card;
			}

			if (response.links == null || response.links.Count() == 0)
			{
				throw new ArgumentNullException("response.links");
			}
			if (!response.links.Any(l => l.rel == "self"))
			{
				throw new ArgumentException("Response is invalid. response.links[rel = self] not found in response.");
			}
			PayPalLinkData linkToSelf = response.links.Single(l => l.rel == "self");

			if (transaction.related_resources == null)
			{
				throw new ArgumentNullException("response.transactions[0].related_resources");
			}
			if (transaction.related_resources.Count() != 1)
			{
				throw new ArgumentOutOfRangeException("response", "response.transactions[0].related_resources count is invalid in the response. count must be 1. current count: " + transaction.related_resources.Count());
			}
			PayPalSaleData sale = transaction.related_resources[0].sale;

			if (!payer_info.shipping_address.HasValue)
			{
				throw new ArgumentNullException("response.payer.payer_info.shipping_address");
			}
			PayPalShippingAddressData payerAddress = payer_info.shipping_address.Value;

			if (!transaction.item_list.HasValue)
			{
				throw new ArgumentNullException("response.transactions[0].item_list");
			}
			if (!transaction.item_list.Value.shipping_address.HasValue)
			{
				throw new ArgumentNullException("response.transactions[0].item_list.shipping_address");
			}
			PayPalShippingAddressData shippingAddress = transaction.item_list.Value.shipping_address.Value;

			if (sale.links == null)
			{
				throw new ArgumentNullException("response.transactions[0].related_resources[0].sale.links");
			}

			if (!sale.links.Any(l => l.rel == "self"))
			{
				throw new ArgumentException("Response is invalid. response.transactions[0].related_resources[0].sale.links[rel = self] not found in response.");
			}
			PayPalLinkData linkToSale = sale.links.Single(l => l.rel == "self");

			if (!sale.links.Any(l => l.rel == "refund"))
			{
				throw new ArgumentException("Response is invalid. response.transactions[0].related_resources[0].sale.links[rel = refund] not found in response.");
			}
			PayPalLinkData linkToRefund = sale.links.Single(l => l.rel == "refund");

			if (!sale.links.Any(l => l.rel == "parent_payment"))
			{
				throw new ArgumentException("Response is invalid. response.transactions[0].related_resources[0].sale.links[rel = parent_payment] not found in response.");
			}
			PayPalLinkData linkToParentPayment = sale.links.Single(l => l.rel == "parent_payment");

			//some doubt to the right id to put here
			payment.TransactionId = sale.id;

			payment.AmountPaid = transaction.amount.ToDecimal();
			payment.Json = response.Json;

			//[Display(Name = "PayPal Payment Id", Description = "PayPal Payment Resource from PayPal payment (response.id)")]
			payment.PayPalPaymentResource = response.id;

			//[Display(Name = "PayPal Payment State", Description = "PayPal Payment State from PayPal payment (response.state")]
			payment.PayPalState = response.state;

			//[Display(Name = "PayPal Payment Intent", Description = "PayPal Payment Intent from PayPal payment (response.intent")]
			payment.PayPalIntent = response.intent;

			//[Display(Name = "PayPal Payment Create Time", Description = "Time payment was started from PayPal payment (response.create_time")]
			payment.PayPalCreateTime = response.create_time;

			//[Display(Name = "PayPal Payment Update Time", Description = "Time payment was completed from PayPal payment (response.update_time")]
			payment.PayPalUpdateTime = response.update_time;

			//[Display(Name = "PayPal Payment Method", Description = "Payment method from PayPal payment (response.payer.payment_method")]
			payment.PayPalPaymentMethod = payer.payment_method;

			if (!directCreditCard.HasValue)
			{
				//[Display(Name = "PayPal Payment Is a Direct Credit Card payment", Description = "Checked if this is a PayPal direct credit card payment from PayPal payment (response.payer.funding_instruments.Any()")]
				payment.PayPalIsDirectCreditCardPayment = false;
			}
			if (payer.funding_instruments != null && payer.funding_instruments.Count() == 1)
			{
				//[Display(Name = "PayPal Payment Is a Direct Credit Card payment", Description = "Checked if this is a PayPal direct credit card payment from PayPal payment (response.payer.funding_instruments.Any()")]
				payment.PayPalIsDirectCreditCardPayment = true;

				//[Display(Name = "PayPal Direct Credit Card Number", Description = "Credit card number for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.number")]
				payment.PayPalDirectCreditCardNumber = directCreditCard.Value.number;

				//[Display(Name = "PayPal Direct Credit Card Type", Description = "Credit card type for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.type")]
				payment.PayPalDirectCreditCardType = directCreditCard.Value.type;

				//[Display(Name = "PayPal Direct Credit Card Expire Month", Description = "Expiration month for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.expire_month")]
				payment.PayPalDirectCreditCardExpireMonth = directCreditCard.Value.expire_month;

				//[Display(Name = "PayPal Direct Credit Card Expire Year", Description = "Expiration year for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.expire_year")]
				payment.PayPalDirectCreditCardExpireYear = directCreditCard.Value.expire_year;

				//[Display(Name = "PayPal Direct Credit Card First Name", Description = "First Name for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.first_name")]
				payment.PayPalDirectCreditCardFirstName = directCreditCard.Value.first_name;

				//[Display(Name = "PayPal Direct Credit Card First Name", Description = "Last Name for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.last_name")]
				payment.PayPalDirectCreditCardLastName = directCreditCard.Value.last_name;

			}


			//[Display(Name = "PayPal Payer Email", Description = "Email address entered from PayPal payment (response.payer.payer_info.email")]
			payment.PayPalPayerEmail = payer_info.email;

			//[Display(Name = "PayPal Payer First Name", Description = "first name entered from PayPal payment (response.payer.payer_info.first_name")]
			payment.PayPalPayerFirstName = payer_info.first_name;

			//[Display(Name = "PayPal Payer Last Name", Description = "last name entered from PayPal payment (response.payer.payer_info.last_name")]
			payment.PayPalPayerLastName = payer_info.last_name;

			//[Display(Name = "PayPal Payer Id", Description = "Payer Id from PayPal payment (response.payer.payer_info.payer_id")]
			payment.PayPalPayerId = payer_info.payer_id;

			//[Display(Name = "PayPal Payment Resource API Link", Description = "Payment API Link from PayPal payment (response.links.[rel=self].href")]
			payment.PayPalPaymentResourceLink = linkToSelf.href;

			//[Display(Name = "PayPal Transaction Total", Description = "Transaction Total from PayPal payment (response.transactions.[0].amount.total")]
			payment.PayPalTransactionTotal = transaction.amount.total;

			//[Display(Name = "PayPal Transaction Currency", Description = "Transaction currency from PayPal payment (response.transactions.[0].amount.currency")]
			payment.PayPalTransactionCurrency = transaction.amount.currency;

			//[Display(Name = "PayPal Transaction Description", Description = "Transaction description from PayPal payment (response.transactions.[0].description")]
			payment.PayPalTransactionDescription = transaction.description;

			//[Display(Name = "PayPal Sale Transaction Id", Description = "PayPal Transaction Id from PayPal payment (response.transactions.[0].related_resources.[0].sale.id")]
			payment.PayPalSaleId = sale.id;

			//[Display(Name = "PayPal Sale Create Time", Description = "Time sale was started from PayPal payment (response.transactions.[0].related_resources.[0].sale.create_time")]
			payment.PayPalSaleCreateTime = sale.create_time;

			//[Display(Name = "PayPal Sale Update Time", Description = "Time sale was completed from PayPal payment (response.transactions.[0].related_resources.[0].sale.update_time")]
			payment.PayPalSaleUpdateTime = sale.update_time;

			//[Display(Name = "PayPal Sale Total", Description = "PayPal Sale Total from PayPal payment (response.transactions.[0].related_resources.[0].sale.amount.total")]
			payment.PayPalSaleAmountTotal = sale.amount.total;

			//[Display(Name = "PayPal Sale Currency", Description = "PayPal Sale Currency from PayPal payment (response.transactions.[0].related_resources.[0].sale.amount.currency")]
			payment.PayPalSaleAmountCurrency = sale.amount.currency;

			//[Display(Name = "PayPal Sale Payment Mode", Description = "PayPal Sale Payment Mode from PayPal payment (response.transactions.[0].related_resources.[0].sale.payment_mode")]
			payment.PayPalSalePaymentMode = sale.payment_mode;

			//[Display(Name = "PayPal Sale Status", Description = "PayPal Sale status from PayPal payment (response.transactions.[0].related_resources.[0].sale.state")]
			payment.PayPalSaleState = sale.state;

			//[Display(Name = "PayPal Sale Protection Eligibility", Description = "PayPal Sale protection eligibility from PayPal payment (response.transactions.[0].related_resources.[0].sale.protection_eligibility")]
			payment.PayPalSaleProtectionEligibility = sale.protection_eligibility;

			//[Display(Name = "PayPal Sale Protection Eligibility Type", Description = "PayPal Sale protection eligibility type from PayPal payment (response.transactions.[0].related_resources.[0].sale.protection_eligibility_type")]
			payment.PayPalSaleProtectionEligibilityType = sale.protection_eligibility_type;

			//[Display(Name = "PayPal Sale Transaction Fee", Description = "PayPal Sale transaction fee from PayPal payment (response.transactions.[0].related_resources.[0].sale.transaction_fee.value")]
			payment.PayPalSaleTransactionFeeValue = sale.transaction_fee.value ;

			//[Display(Name = "PayPal Sale Transaction Fee Currency", Description = "PayPal Sale transaction fee currency from PayPal payment (response.transactions.[0].related_resources.[0].sale.transaction_fee.currency")]
			payment.PayPalSaleTransactionFeeCurrency = sale.transaction_fee.currency;

			//[Display(Name = "PayPal Sale API Link to Sale", Description = "PayPal API Link to sale from PayPal payment (response.transactions.[0].related_resources.[0].sale.links.[rel=self]")]
			payment.PayPalSaleAPILinkToSelf = linkToSale.href;

			//[Display(Name = "PayPal Sale API Link to Refund", Description = "PayPal API Link to refund from PayPal payment (response.transactions.[0].related_resources.[0].sale.links.[rel=refund]")]
			payment.PayPalSaleAPILinkToRefund = linkToRefund.href;

			//[Display(Name = "PayPal Sale API Link to Refund", Description = "PayPal API Link to refund from PayPal payment (response.transactions.[0].related_resources.[0].sale.links.[rel=parent_payment]")]
			payment.PayPalSaleAPILinkToParentPayment = linkToParentPayment.href;

			//[Display(Name = "PayPal Shipping Address Recipient Name", Description = "Shipping recipient name from PayPal payment (response.transactions.[0].item_list.shipping_address.recipient_name")]
			payment.PayPalShippingAddressRecipientName = shippingAddress.recipient_name;

			//[Display(Name = "PayPal Shipping Address Line 1", Description = "Shipping address line 1 from PayPal payment (response.transactions.[0].item_list.shipping_address.line1")]
			payment.PayPalShippingAddressLine1 = shippingAddress.line1;

			//[Display(Name = "PayPal Shipping Address Line 2", Description = "Shipping address line 2 from PayPal payment (response.transactions.[0].item_list.shipping_address.line2")]
			payment.PayPalShippingAddressLine2 = shippingAddress.line2;

			//[Display(Name = "PayPal Shipping Address City", Description = "Shipping City from PayPal payment (response.transactions.[0].item_list.shipping_address.city")]
			payment.PayPalShippingAddressCity = shippingAddress.city;

			//[Display(Name = "PayPal Shipping Address State", Description = "Shipping State from PayPal payment (response.transactions.[0].item_list.shipping_address.state")]
			payment.PayPalShippingAddressState = shippingAddress.state;

			//[Display(Name = "PayPal Shipping Address Postal Code", Description = "Shipping (ZIP) Postal code from PayPal payment (response.transactions.[0].item_list.shipping_address.postal_code")]
			payment.PayPalShippingAddressPostalCode = shippingAddress.postal_code;

			//[Display(Name = "PayPal Shipping Country Code", Description = "Shipping Country Code from PayPal payment (response.transactions.[0].item_list.shipping_address.country_code")]
			payment.PayPalShippingAddressCountryCode = shippingAddress.country_code;

			//[Display(Name = "PayPal Shipping Address Recipient Name", Description = "Shipping recipient name from PayPal payment (response.payer.payer_info.shipping_address.recipient_name")]
			payment.PayPalPayerShippingAddressRecipientName = payerAddress.recipient_name;

			//[Display(Name = "PayPal Shipping Address Line 1", Description = "Shipping address line 1 from PayPal payment (response.payer.payer_info.shipping_address.line1")]
			payment.PayPalPayerShippingAddressLine1 = payerAddress.line1;

			//[Display(Name = "PayPal Shipping Address Line 2", Description = "Shipping address line 2 from PayPal payment (response.payer.payer_info.shipping_address.line2")]
			payment.PayPalPayerShippingAddressLine2 = payerAddress.line2;

			//[Display(Name = "PayPal Shipping Address City", Description = "Shipping City from PayPal payment (response.payer.payer_info.shipping_address.city")]
			payment.PayPalPayerShippingAddressCity = payerAddress.city;

			//[Display(Name = "PayPal Shipping Address State", Description = "Shipping State from PayPal payment (response.payer.payer_info.shipping_address.state")]
			payment.PayPalPayerShippingAddressState = payerAddress.state;

			//[Display(Name = "PayPal Shipping Address Postal Code", Description = "Shipping (ZIP) Postal code from PayPal payment (response.payer.payer_info.shipping_address.postal_code")]
			payment.PayPalPayerShippingAddressPostalCode = payerAddress.postal_code;

			//[Display(Name = "PayPal Shipping Country Code", Description = "Shipping Country Code from PayPal payment (response.payer.payer_info.shipping_address.country_code")]
			payment.PayPalPayerShippingAddressCountryCode = payerAddress.country_code;

		}

		/// <summary>
		/// Migrates anonymous orders to a new user profile; used when user signs up to bring their anonymous orders with them
		/// saves changes when done
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="db"></param>
		/// <param name="userProfile"></param>
		/// <param name="controller"></param>
		public static void MigrateOrdersToNewProfile(this StoreFront storeFront, IGstoreDb db, UserProfile userProfile, BaseController controller)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			List<Order> orders = storeFront.Orders.Where(o => o.UserProfileId == null && o.Email.ToLower() == userProfile.Email.ToLower()).ToList();
			foreach (Order order in orders)
			{
				order.UserProfileId = userProfile.UserProfileId;
				db.Orders.Update(order);
			}
			if (orders.Count != 0)
			{
				db.SaveChanges();
				controller.AddUserMessage("Orders Available", "Your orders (" + orders.Count.ToString("N0") + ") are now available on your profile page.", UserMessageType.Info);
			}

		}

		public static string ToSimpleTextForCheckout(this WebFormResponse response, bool htmlEncode)
		{
			if (response == null)
			{
				return string.Empty;
			}

			string text = string.Empty;
			foreach (WebFormFieldResponse fieldResponse in response.WebFormFieldResponses.OrderBy(wffr => wffr.WebFormFieldOrder))
			{
				if (htmlEncode)
				{
					text += "\n" + fieldResponse.WebFormField.LabelText.ToHtmlLines() + ": " + fieldResponse.ValueText().ToHtmlLines();
				}
				else
				{
					text += "\n" + fieldResponse.WebFormField.LabelText + ": " + fieldResponse.ValueText();
				}
			}

			return text;
		}

		public static bool SendOrderReceipt(this Order order, BaseController controller, out string userMessage, string orderEmailHtmlPartialViewName, string orderEmailTextPartialViewName, string emailAddressOverride = null, string emailNameOverride = null)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (order == null)
			{
				throw new ArgumentNullException("order");
			}
			if (string.IsNullOrEmpty(orderEmailTextPartialViewName))
			{
				throw new ArgumentNullException("orderEmailTextPartialViewName");
			}
			if (string.IsNullOrEmpty(orderEmailHtmlPartialViewName))
			{
				throw new ArgumentNullException("orderEmailHtmlPartialViewName");
			}

			Client client = controller.CurrentClientOrThrow;

			string emailName = order.FullName;
			string emailAddress = order.Email;

			if (!string.IsNullOrEmpty(emailNameOverride))
			{
				emailName = emailNameOverride;
			}
			if (!string.IsNullOrEmpty(emailAddressOverride))
			{
				emailAddress = emailAddressOverride;
			}
			string subject = order.ReceiptSubject(controller.CurrentStoreFrontConfigOrThrow);

			string orderEmailHtml = null;
			try
			{
				orderEmailHtml = controller.RenderPartialView(orderEmailHtmlPartialViewName, order);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error running order email HTML partial view: " + orderEmailHtmlPartialViewName, ex);
			}

			string orderEmailText = null;
			try
			{
				orderEmailText = controller.RenderPartialView(orderEmailTextPartialViewName, order);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error running order email Text partial view: " + orderEmailTextPartialViewName, ex);
			}

			bool emailResult;
			try
			{
				emailResult = controller.SendEmail(emailAddress, emailName, subject, orderEmailText, orderEmailHtml);
			}
			catch (Exception ex)
			{
				if (controller.CurrentUserProfileOrNull != null && controller.CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					controller.AddUserMessage("Email failed", "Order Email failed with exception: " + ex.Message + "\n" + ex.ToString(), UserMessageType.Danger);
				}
				emailResult = false;
			}

			IGstoreDb db = controller.GStoreDb;

			db.LogUserActionEvent(controller.HttpContext, controller.RouteData, controller, UserActionCategoryEnum.Orders, UserActionActionEnum.Orders_ReceiptSentToEmail, "Order Receipt", emailResult, orderNumber: order.OrderNumber);

			if (emailResult)
			{
				userMessage = "Receipt for Order #" + order.OrderNumber + " was emailed to " + emailAddress;
			}
			else
			{
				if (!Settings.AppEnableEmail)
				{
					userMessage = "Sorry, we cannot Email an order receipt because Email is not enabled for this server.";
					return false;
				}

				if (!client.UseSendGridEmail)
				{
					userMessage = "Sorry, we cannot Email an order receipt because Email is not enabled for this store front.";
					return false;
				}
				userMessage = "There was an error emailing receipt for Order #" + order.OrderNumber + " to " + emailAddress + ". Please Contact us if you do not receive it.";
				return false;

			}
			return emailResult;
		}

		public static string ReceiptSubject(this Order order, StoreFrontConfiguration config)
		{
			if (order == null)
			{
				throw new ArgumentNullException("order");
			}
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			return "Your Order #" + order.OrderNumber + " at " + config.Name + " - " + config.PublicUrl;

		}

		public static decimal? UnitPrice(this OrderBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.OrderItems == null || bundle.OrderItems.Count == 0)
			{
				return null;
			}

			return bundle.OrderItems.Sum(ci => ci.UnitPrice);
		}

		public static decimal? UnitPriceExt(this OrderBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.OrderItems == null || bundle.OrderItems.Count == 0)
			{
				return null;
			}

			return bundle.OrderItems.Sum(ci => ci.UnitPriceExt);
		}

		public static decimal? LineDiscount(this OrderBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.OrderItems == null || bundle.OrderItems.Count == 0)
			{
				return null;
			}

			return bundle.OrderItems.Sum(ci => ci.LineDiscount);
		}

		public static decimal? LineTotal(this OrderBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.OrderItems == null || bundle.OrderItems.Count == 0)
			{
				return null;
			}

			return bundle.OrderItems.Sum(ci => ci.LineTotal);
		}


							
	}
}