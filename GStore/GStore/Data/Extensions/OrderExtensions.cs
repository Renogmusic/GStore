using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using GStore.AppHtmlHelpers;
using System.Web.Mvc;
using GStore.Controllers.BaseClass;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Text;
using GStore.Models.ViewModels;

namespace GStore.Data
{
	public static class OrderExtensions
	{
		public static Order CreateOrderFromCart(this Cart cart, IGstoreDb db)
		{
			Order order = db.Orders.Create();

			lock (orderNumberLock)
			{
				order.OrderNumber = cart.StoreFront.GetAndIncrementOrderNumber(db).ToString();
			}

			order.Cart = cart;
			order.CartId = cart.CartId;
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
			order.DeliveryInfoDigital = cart.DeliveryInfoDigital;
			order.DeliveryInfoShipping = cart.DeliveryInfoShipping;
			order.AllItemsAreDigitalDownload = cart.AllItemsAreDigitalDownload;
			order.DigitalDownloadItemCount = cart.DigitalDownloadItemCount;
			order.ShippingItemCount = cart.ShippingItemCount;

			order.Payment = cart.Payment;
			
			order.RefundedAmount= 0;
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

			//add order items;
			foreach (CartItem item in cart.CartItems)
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
				db.OrderItems.Add(orderItem);
			}

			if (cart.CartItems.Count > 0)
			{
				db.SaveChanges();
			}

			return order;

		}

		private static object orderNumberLock = new Object();

		public static int GetAndIncrementOrderNumber(this StoreFront storeFront, IGstoreDb db)
		{
			lock (orderNumberLock)
			{
				int orderNumber = 1001;
				if (storeFront.NextOrderNumber > 0)
				{
					orderNumber = storeFront.NextOrderNumber;
				}

				storeFront.NextOrderNumber = orderNumber + 1;
				storeFront = db.StoreFronts.Update(storeFront);
				db.SaveChangesDirect();

				return orderNumber;

			}
		}

	}
}