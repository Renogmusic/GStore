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
	public static class CartExtensions
	{
		public static Cart GetCart(this StoreFront storeFront, string sessionId, UserProfile userProfileOrNull)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			IQueryable<Cart> query = null;
			if (userProfileOrNull == null)
			{
				query = storeFront.Carts.AsQueryable().Where(c =>
					c.StoreFrontId == storeFront.StoreFrontId && c.ClientId == storeFront.ClientId
					&& c.OrderId == null && c.UserProfileId == null
					&& c.SessionId == sessionId);
			}
			else
			{
				int userProfileId = userProfileOrNull.UserProfileId;
				query = storeFront.Carts.AsQueryable().Where(c =>
					c.StoreFrontId == storeFront.StoreFrontId && c.ClientId == storeFront.ClientId
					&& c.OrderId == null && c.UserProfileId == userProfileId);
			}

			return query.SingleOrDefault();
		}

		public static Cart GetPreviewCart(this StoreFront storeFront, string sessionId, int quantity, string discountCode, UserProfile userProfileOrNull)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			int? userProfileId = userProfileOrNull == null ? (int?)null : userProfileOrNull.UserProfileId;

			Product product = storeFront.Products.AsQueryable().CanAddToCart(storeFront).ApplyDefaultSort().FirstOrDefault();
			if (storeFront.Products.Count == 0)
			{
				//no products for this store, create a sample one
				product = new Product();
				product.Name = "Sample Product Name";
				product.ProductId = 0;
				product.ImageName = "SampleProductImage";
				product.MaxQuantityPerOrder = 9;
				product.Order = 100;
				product.StoreFront = storeFront;
				product.StoreFrontId = storeFront.StoreFrontId;
				product.UrlName = "SampleProduct";
				if (storeFront.ProductCategories.Count > 0)
				{
					product.Category = storeFront.ProductCategories.AsQueryable().WhereIsActive().ApplyDefaultSort().FirstOrDefault();
					if (product.Category == null)
					{
						product.Category = storeFront.ProductCategories.AsQueryable().ApplyDefaultSort().FirstOrDefault();
					}
				}
				if (product.Category != null)
				{
					product.ProductCategoryId = product.Category.ProductCategoryId;
				}
			}
			else
			{
				product = storeFront.Products.AsQueryable().CanAddToCart(storeFront).ApplyDefaultSort().FirstOrDefault();
				if (product == null)
				{
					//no products can add to cart, grab the first active one
					product = storeFront.Products.AsQueryable().WhereIsActive().ApplyDefaultSort().FirstOrDefault();
				}
				if (product == null)
				{
					//no products are active, grab first inactive one
					product = storeFront.Products.AsQueryable().ApplyDefaultSort().FirstOrDefault();
				}
			}

			if (quantity < 1)
			{
				quantity = 1;
			}
			else if (quantity > product.MaxQuantityPerOrder)
			{
				quantity = product.MaxQuantityPerOrder;
			}

			string productVariantInfo = "Color: Red";

			Cart cart = new Cart();
			CartItem cartItem = new CartItem();
			cartItem.SetDefaults(userProfileOrNull);
			cartItem.Cart = cart;
			cartItem.CartId = cart.CartId;
			cartItem.Client = storeFront.Client;
			cartItem.ClientId = storeFront.ClientId;
			cartItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			cartItem.IsPending = false;
			cartItem.ListPrice = product.ListPrice(quantity);
			cartItem.ListPriceExt = product.ListPriceExt(quantity);
			cartItem.Product = product;
			cartItem.ProductId = product.ProductId;
			cartItem.ProductVariantInfo = productVariantInfo;
			cartItem.Quantity = quantity;
			cartItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			cartItem.StoreFront = storeFront;
			cartItem.StoreFrontId = storeFront.StoreFrontId;
			cartItem.UnitPrice = product.UnitPrice(quantity);
			cartItem.UnitPriceExt = product.UnitPriceExt(quantity);
			cartItem.LineDiscount = product.LineDiscount(quantity);
			cartItem.LineTotal = cartItem.CalculateLineTotal();

			List<CartItem> cartItems = new List<CartItem>();
			cartItems.Add(cartItem);

			cart.CartItems = cartItems;

			cart.CartId = 0;
			cart.Client = storeFront.Client;
			cart.ClientId = storeFront.ClientId;
			cart.SessionId = sessionId;
			cart.StoreFront = storeFront;
			cart.StoreFrontId = storeFront.StoreFrontId;
			cart.UserProfile = userProfileOrNull;
			cart.UserProfileId = userProfileId;
			if (!string.IsNullOrEmpty(discountCode))
			{
				cart.PreviewUpdateDiscountCode(discountCode, storeFront.CurrentConfig(), null);
			}
			cart.RecalcNoSave();

			return cart;
		}

		/// <summary>
		/// Adds an item to the cart. Creates the cart if it does not exist (cart == null)
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="storeFront"></param>
		/// <param name="db"></param>
		/// <param name="session"></param>
		/// <param name="userProfileOrNull"></param>
		/// <returns></returns>
		public static CartItem AddToCart(this Cart cart, Product product, int quantity, string productVariantInfo, Controllers.BaseClass.BaseController controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			StoreFront storeFront = controller.CurrentStoreFrontOrThrow;
			StoreFrontConfiguration storeFrontConfig = controller.CurrentStoreFrontConfigOrThrow;
			IGstoreDb db = controller.GStoreDb;
			HttpSessionStateBase session = controller.Session;
			HttpRequestBase request = controller.Request;
			UserProfile userProfileOrNull = controller.CurrentUserProfileOrNull;

			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be from 1 to 10,000");
			}
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			if (!storeFront.Products.AsQueryable().WhereIsActive().Any(p => p.ProductId == product.ProductId))
			{
				throw new ApplicationException("Cannot add product id: " + product.ProductId + " to cart. Not found in store front active products for Store Front '" + storeFrontConfig.Name + "' [" + storeFront.StoreFrontId + "].");
			}

			if (cart == null)
			{
				cart = controller.NewCart();
			}

			CartItem cartItem = db.CartItems.Create();
			cartItem.SetDefaults(userProfileOrNull);
			cartItem.Cart = cart;
			cartItem.CartId = cart.CartId;
			cartItem.Client = storeFront.Client;
			cartItem.ClientId = storeFront.ClientId;
			cartItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			cartItem.IsPending = false;
			cartItem.ListPrice = product.ListPrice(quantity);
			cartItem.ListPriceExt = product.ListPriceExt(quantity);
			cartItem.Product = product;
			cartItem.ProductId = product.ProductId;
			cartItem.ProductVariantInfo = productVariantInfo;
			cartItem.Quantity = 0;
			cartItem.UpdateQuantityNoSave(quantity, controller);
			cartItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			cartItem.StoreFront = storeFront;
			cartItem.StoreFrontId = storeFront.StoreFrontId;
			cartItem.UnitPrice = product.UnitPrice(quantity);
			cartItem.UnitPriceExt = product.UnitPriceExt(quantity);
			cartItem.LineDiscount = product.LineDiscount(quantity);
			cartItem.LineTotal = cartItem.CalculateLineTotal();

			
			cartItem = db.CartItems.Add(cartItem);
			db.SaveChanges();

			cartItem.Cart.RecalcAndSave(db, true);
			return cartItem;
		}

		/// <summary>
		/// Adds an item to the cart. Creates the cart if it does not exist (cart == null), returns user messages if quantity is over the limit
		/// Controller is only user for user messages. if no used messages wanted, set controller = null
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="storeFront"></param>
		/// <param name="db"></param>
		/// <param name="session"></param>
		/// <param name="userProfileOrNull"></param>
		/// <returns></returns>
		public static CartItem UpdateQuantityAndSave(this CartItem cartItem, IGstoreDb db, int quantity, BaseController controller)
		{
			cartItem = cartItem.UpdateQuantityNoSave(quantity, controller);
			cartItem = db.CartItems.Update(cartItem);
			cartItem.Cart.RecalcAndSave(db, true);
			return cartItem;
		}

		/// <summary>
		/// Updates the quantity on a cart item and adjusts for if over max and sets user messages if user is over quantity
		/// Controller is only user for user messages. if no used messages wanted, set controller = null
		/// </summary>
		/// <param name="cartItem"></param>
		/// <param name="quantity"></param>
		/// <param name="controller"></param>
		/// <returns></returns>
		public static CartItem UpdateQuantityNoSave(this CartItem cartItem, int quantity, BaseController controller)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be from 1 to 10,000. For 0 quantity call RemoveFromCart");
			}
			if (cartItem == null)
			{
				throw new ArgumentNullException("cartItem");
			}

			if ((cartItem.Product.MaxQuantityPerOrder != 0) && (quantity > cartItem.Product.MaxQuantityPerOrder))
			{
				//user wants more than the max allowed per order
				bool atMaxBefore = (cartItem.Quantity >= cartItem.Product.MaxQuantityPerOrder);
				if (atMaxBefore)
				{
					//was already at the max qty. give a message the user is at the max allowed per order, cannot add more
					if (controller != null)
					{
						controller.AddUserMessage("Quantity Limit:", "This item is limited to " + cartItem.Product.MaxQuantityPerOrder + " per order and you are at the limit.", UserMessageType.Danger);
					}
					return cartItem;
				}

				if (quantity > cartItem.Product.MaxQuantityPerOrder)
				{
					//was not at max qty before, adjust user to the max and give info message
					quantity = cartItem.Product.MaxQuantityPerOrder;
					if (controller != null)
					{
						controller.AddUserMessage("Quantity Limit:", "This item is limited to " + cartItem.Product.MaxQuantityPerOrder + " per order. You now have " + cartItem.Product.MaxQuantityPerOrder + " of them in your cart.", UserMessageType.Info);
					}
				}

			}

			cartItem.Quantity = quantity;
			return cartItem;

		}

		public static bool RemoveFromCart(this CartItem cartItemToRemove, IGstoreDb db)
		{
			if (cartItemToRemove == null)
			{
				throw new ArgumentNullException("cartItemToRemove");
			}
			Cart cart = cartItemToRemove.Cart;
						
			bool result = db.CartItems.Delete(cartItemToRemove);
			cart.RecalcAndSave(db, false);
			return result;
		}

		private static Cart NewCart(this Controllers.BaseClass.BaseController controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			IGstoreDb db = controller.GStoreDb;
			HttpSessionStateBase session = controller.Session;
			UserProfile userProfileOrNull = controller.CurrentUserProfileOrNull;
			StoreFront storeFront = controller.CurrentStoreFrontOrThrow;
			HttpRequestBase request = controller.Request;

			string discountCode = null;
			if (session["DiscountCode"] != null)
			{
				//add discount code
				discountCode = session["DiscountCode"] as string;
			}
			Cart newCart = db.Carts.Create();
			newCart.SetDefaults(userProfileOrNull);
			newCart.SessionId = session.SessionID;
			newCart.UserProfileId = (userProfileOrNull == null ? (int?)null : userProfileOrNull.UserProfileId);
			newCart.EntryDateTimeUtc = session.EntryDateTime().Value;
			newCart.EntryRawUrl = session.EntryRawUrl();
			newCart.EntryReferrer = session.EntryReferrer();
			newCart.EntryUrl = session.EntryUrl();
			newCart.IsPending = false;
			newCart.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newCart.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			newCart.ClientId = storeFront.ClientId;
			newCart.Client = storeFront.Client;
			newCart.StoreFront = storeFront;
			newCart.StoreFrontId = storeFront.StoreFrontId;
			newCart.UserAgent = request.UserAgent;
			newCart.IPAddress = request.UserHostAddress;
			newCart = db.Carts.Add(newCart);
			db.SaveChanges();

			newCart.UpdateDiscountCode(discountCode, controller);

			return newCart;

		}

		private static Cart NewPreviewCart(this Controllers.BaseClass.BaseController controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			HttpSessionStateBase session = controller.Session;
			UserProfile userProfileOrNull = controller.CurrentUserProfileOrNull;
			StoreFront storeFront = controller.CurrentStoreFrontOrThrow;
			HttpRequestBase request = controller.Request;

			string discountCode = null;
			if (session["DiscountCode"] != null)
			{
				//add discount code
				discountCode = session["DiscountCode"] as string;
			}
			Cart newCart = new Cart();
			newCart.SetDefaults(userProfileOrNull);
			newCart.SessionId = session.SessionID;
			newCart.UserProfileId = (userProfileOrNull == null ? (int?)null : userProfileOrNull.UserProfileId);
			newCart.EntryDateTimeUtc = session.EntryDateTime().Value;
			newCart.EntryRawUrl = session.EntryRawUrl();
			newCart.EntryReferrer = session.EntryReferrer();
			newCart.EntryUrl = session.EntryUrl();
			newCart.IsPending = false;
			newCart.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newCart.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			newCart.ClientId = storeFront.ClientId;
			newCart.Client = storeFront.Client;
			newCart.StoreFront = storeFront;
			newCart.StoreFrontId = storeFront.StoreFrontId;
			newCart.UserAgent = request.UserAgent;
			newCart.IPAddress = request.UserHostAddress;
			//newCart.UpdateDiscountCode(discountCode, controller);

			return newCart;

		}

		public static void CopyValuesFromCartConfigViewModel(this StoreFrontConfiguration storeFrontConfig, CartConfigViewModel cartConfig)
		{
			if (cartConfig == null)
			{
				return;
			}
			storeFrontConfig.UseShoppingCart = cartConfig.UseShoppingCart;
			storeFrontConfig.CartNavShowCartToAnonymous = cartConfig.CartNavShowCartToAnonymous;
			storeFrontConfig.CartNavShowCartToRegistered = cartConfig.CartNavShowCartToRegistered;
			storeFrontConfig.CartNavShowCartWhenEmpty = cartConfig.CartNavShowCartWhenEmpty;
			storeFrontConfig.CartRequireLogin = cartConfig.CartRequireLogin;
			storeFrontConfig.CartLayoutName = cartConfig.CartLayoutName;
			storeFrontConfig.CartThemeId = cartConfig.CartThemeId;

			storeFrontConfig.CartEmptyMessage = cartConfig.CartEmptyMessage;
			storeFrontConfig.CartItemColumnLabel = cartConfig.CartItemColumnLabel;
			storeFrontConfig.CartItemDiscountColumnLabel = cartConfig.CartItemDiscountColumnLabel;
			storeFrontConfig.CartItemDiscountColumnShow = cartConfig.CartItemDiscountColumnShow;
			storeFrontConfig.CartItemListPriceColumnLabel = cartConfig.CartItemListPriceColumnLabel;
			storeFrontConfig.CartItemListPriceColumnShow = cartConfig.CartItemListPriceColumnShow;
			storeFrontConfig.CartItemListPriceExtColumnLabel = cartConfig.CartItemListPriceExtColumnLabel;
			storeFrontConfig.CartItemListPriceExtColumnShow = cartConfig.CartItemListPriceExtColumnShow;
			storeFrontConfig.CartItemQuantityColumnLabel = cartConfig.CartItemQuantityColumnLabel;
			storeFrontConfig.CartItemQuantityColumnShow = cartConfig.CartItemQuantityColumnShow;
			storeFrontConfig.CartItemTotalColumnLabel = cartConfig.CartItemTotalColumnLabel;
			storeFrontConfig.CartItemTotalColumnShow = cartConfig.CartItemTotalColumnShow;
			storeFrontConfig.CartItemUnitPriceColumnLabel = cartConfig.CartItemUnitPriceColumnLabel;
			storeFrontConfig.CartItemUnitPriceColumnShow = cartConfig.CartItemUnitPriceColumnShow;
			storeFrontConfig.CartItemUnitPriceExtColumnLabel = cartConfig.CartItemUnitPriceExtColumnLabel;
			storeFrontConfig.CartItemUnitPriceExtColumnShow = cartConfig.CartItemUnitPriceExtColumnShow;
			storeFrontConfig.CartItemVariantColumnLabel = cartConfig.CartItemVariantColumnLabel;
			storeFrontConfig.CartItemVariantColumnShow = cartConfig.CartItemVariantColumnShow;
			storeFrontConfig.CartOrderDiscountCodeApplyButtonText = cartConfig.CartOrderDiscountCodeApplyButtonText;
			storeFrontConfig.CartOrderDiscountCodeLabel = cartConfig.CartOrderDiscountCodeLabel;
			storeFrontConfig.CartOrderDiscountCodeRemoveButtonText = cartConfig.CartOrderDiscountCodeRemoveButtonText;
			storeFrontConfig.CartOrderDiscountCodeSectionShow = cartConfig.CartOrderDiscountCodeSectionShow;
			storeFrontConfig.CartOrderDiscountLabel = cartConfig.CartOrderDiscountLabel;
			storeFrontConfig.CartOrderDiscountShow = cartConfig.CartOrderDiscountShow;
			storeFrontConfig.CartOrderHandlingLabel = cartConfig.CartOrderHandlingLabel;
			storeFrontConfig.CartOrderHandlingShow = cartConfig.CartOrderHandlingShow;
			storeFrontConfig.CartOrderItemCountLabel = cartConfig.CartOrderItemCountLabel;
			storeFrontConfig.CartOrderItemCountShow = cartConfig.CartOrderItemCountShow;
			storeFrontConfig.CartOrderShippingLabel = cartConfig.CartOrderShippingLabel;
			storeFrontConfig.CartOrderShippingShow = cartConfig.CartOrderShippingShow;
			storeFrontConfig.CartOrderSubtotalLabel = cartConfig.CartOrderSubtotalLabel;
			storeFrontConfig.CartOrderSubtotalShow = cartConfig.CartOrderSubtotalShow;
			storeFrontConfig.CartOrderTaxLabel = cartConfig.CartOrderTaxLabel;
			storeFrontConfig.CartOrderTaxShow = cartConfig.CartOrderTaxShow;
			storeFrontConfig.CartOrderTotalLabel = cartConfig.CartOrderTotalLabel;
			storeFrontConfig.CartPageHeading = cartConfig.CartPageHeading;
			storeFrontConfig.CartPageTitle = cartConfig.CartPageTitle;
		}

		public static void DumpCartAndSave(this StoreFront storeFront, IGstoreDb db, Cart cart)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (cart != null)
			{
				db.Carts.Delete(cart);
				db.SaveChanges();
			}
		}

		/// <summary>
		/// Merges items in the users cart with items from their anonymous cart
		/// after items are moved, the old user profile cart is dumped
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="db"></param>
		/// <param name="cart"></param>
		/// <param name="userProfile"></param>
		/// <returns></returns>
		public static Cart MigrateCartToProfile(this StoreFront storeFront, IGstoreDb db, Cart cart, UserProfile userProfile, BaseController controller)
		{
			if (cart == null)
			{
				return null;
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (storeFront.Carts.Any(c => c.UserProfileId == userProfile.UserProfileId))
			{
				//pick up all the items from the old cart and move it to the new cart
				Cart oldUserCart = storeFront.Carts.Single(c => c.UserProfileId == userProfile.UserProfileId && c.Order == null);

				cart.StatusEmailedConfirmation = (cart.StatusEmailedConfirmation || oldUserCart.StatusEmailedConfirmation);
				cart.StatusEmailedContents = (cart.StatusEmailedConfirmation || oldUserCart.StatusEmailedContents);

				cart.DiscountCodesAttempted += oldUserCart.DiscountCodesAttempted + cart.DiscountCodesAttempted.AndDelimiter(", ");
				cart.DiscountCodesFailed += oldUserCart.DiscountCodesFailed + cart.DiscountCodesFailed.AndDelimiter(", ");

				if (cart.Discount == null && oldUserCart.Discount != null)
				{
					cart.Discount = oldUserCart.Discount;
					cart.DiscountCode = oldUserCart.DiscountCode;
					cart.DiscountId = oldUserCart.DiscountId;
				}

				foreach (CartItem item in oldUserCart.CartItems)
				{
					if (cart.CartItems.Any(c => c.ProductId == item.ProductId && c.ProductVariantInfo == item.ProductVariantInfo))
					{
						//item exists in old and new cart; update quantity and ensure not over max
						CartItem existingItem = cart.CartItems.Single(c => c.ProductId == item.ProductId);
						existingItem.UpdateQuantityNoSave(existingItem.Quantity + item.Quantity, null);
					}
					else
					{
						cart.AddToCart(item.Product, item.Quantity, item.ProductVariantInfo, controller);
					}
				}
				cart.UserProfileId = userProfile.UserProfileId;
				cart.Email = userProfile.Email;
				cart = db.Carts.Update(cart);

				db.SaveChangesDirect();

				storeFront.DumpCartAndSave(db, oldUserCart);

			}
			else
			{
				//user has no cart, so move this one to their login
				cart.UserProfileId = userProfile.UserProfileId;
				cart.Email = userProfile.Email;
				cart = db.Carts.Update(cart);
				db.SaveChangesDirect();
			}
			return cart;
		}

		public static Cart MigrateCartToAnonymous(this StoreFront storeFront, IGstoreDb db, Cart cart)
		{
			cart.UserProfileId = null;
			cart = db.Carts.Update(cart);
			db.SaveChangesDirect();
			return cart;
		}

		/// <summary>
		/// Recalculates the cart total, and recalcs line items if recalcItems = true (default is false)
		/// DOES NOT SAVE CHANGES to CartItem or Cart (useful for preview cart)
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="db"></param>
		/// <param name="recalcItems"></param>
		public static void RecalcNoSave(this Cart cart, bool recalcItems = false)
		{
			if (cart == null)
			{
				return;
			}
			if (recalcItems)
			{
				foreach (CartItem cartItem in cart.CartItems)
				{
					cartItem.RecalcNoSave();
				}
			}

			cart.ItemCount = cart.CalculateItemCount();
			cart.Subtotal = cart.CalculateSubtotal();
			cart.Shipping = cart.CalculateShipping();
			cart.Handling = cart.CalculateHandling();
			cart.OrderDiscount = cart.CalculateOrderDiscount();
			cart.Tax = cart.CalculateTax();
			cart.Total = cart.CalculateTotal();

			cart.DigitalDownloadItemCount = cart.CartItems.Count(ci => ci.Product.DigitalDownload);
			cart.ShippingItemCount = cart.CartItems.Count(ci => !ci.Product.DigitalDownload);
			if (cart.ShippingItemCount == 0)
			{
				cart.AllItemsAreDigitalDownload = true;
			}

		}

		/// <summary>
		/// Recalculates the cart total, and recalcs line items if recalcItems = true (default is false)
		/// calls SaveChanges after the recalc
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="db"></param>
		/// <param name="recalcItems"></param>
		public static void RecalcAndSave(this Cart cart, IGstoreDb db, bool recalcItems = false)
		{
			if (db == null)
			{
				throw new ArgumentNullException("db");
			}
			if (cart == null)
			{
				return;
			}
			if (recalcItems)
			{
				foreach (CartItem cartItem in cart.CartItems)
				{
					cartItem.RecalcNoSave();
					db.CartItems.Update(cartItem);
				}
			}

			cart.ItemCount = cart.CalculateItemCount();
			cart.Subtotal = cart.CalculateSubtotal();
			cart.Shipping = cart.CalculateShipping();
			cart.Handling = cart.CalculateHandling();
			cart.OrderDiscount = cart.CalculateOrderDiscount();
			cart.Tax = cart.CalculateTax();
			cart.Total = cart.CalculateTotal();

			cart.DigitalDownloadItemCount = cart.CartItems.Count(ci => ci.Product.DigitalDownload);
			cart.ShippingItemCount = cart.CartItems.Count(ci => !ci.Product.DigitalDownload);
			if (cart.ShippingItemCount == 0)
			{
				cart.AllItemsAreDigitalDownload = true;
			}

			db.Carts.Update(cart);
			db.SaveChanges();
		}

		public static int CalculateItemCount(this Cart cart)
		{
			if (cart == null || cart.CartItems == null || cart.CartItems.Count == 0)
			{
				return 0;
			}

			return cart.CartItems.Sum(ci => ci.Quantity);
		}

		public static decimal CalculateSubtotal(this Cart cart)
		{
			if (cart == null || cart.CartItems == null || cart.CartItems.Count == 0)
			{
				return 0M;
			}
			
			return cart.CartItems.Sum(ci => ci.LineTotal);
		}

		public static decimal CalculateShipping(this Cart cart)
		{
			if (cart == null)
			{
				return 0M;
			}
			return 0M;
		}

		public static decimal CalculateHandling(this Cart cart)
		{
			if (cart == null)
			{
				return 0M;
			}
			return 0M;
		}

		public static decimal CalculateOrderDiscount(this Cart cart)
		{
			if (cart == null)
			{
				return 0M;
			}

			if (cart.DiscountCode == null)
			{
				return 0M;
			}

			Discount discount = cart.Discount;
			if (cart.Subtotal < discount.MinSubtotal)
			{
				//DiscountCode minimum not met
				return 0M;
			}

			if ((discount.MaxUses != 0) && (discount.UseCount >= discount.MaxUses))
			{
				//discount usage is used up
				return 0M;
			}

			if (discount.FreeShipping)
			{
				cart.Shipping = 0M;
			}

			decimal discountAmount = discount.FlatDiscount;

			if (discount.PercentOff > 0)
			{
				discountAmount += cart.Subtotal * (discount.PercentOff / 100);
			}

			if (discountAmount > cart.Subtotal)
			{
				discountAmount = cart.Subtotal;
			}
			return discountAmount;

		}

		public static decimal CalculateTax(this Cart cart)
		{
			if (cart == null)
			{
				return 0M;
			}
			return 0M;
		}

		public static decimal CalculateTotal(this Cart cart)
		{
			if (cart == null)
			{
				return 0M;
			}

			return cart.Subtotal + cart.Shipping + cart.Handling - cart.OrderDiscount + cart.Tax;
		}

		/// <summary>
		/// Re-calculates unit price, ext price, line discount, and line total for a cart line item
		/// </summary>
		/// <param name="cartItem"></param>
		public static void RecalcNoSave(this CartItem cartItem)
		{
			cartItem.UnitPrice = cartItem.Product.UnitPrice(cartItem.Quantity);
			cartItem.UnitPriceExt = cartItem.Product.UnitPriceExt(cartItem.Quantity);
			cartItem.ListPrice = cartItem.Product.ListPrice(cartItem.Quantity);
			cartItem.ListPriceExt = cartItem.Product.ListPriceExt(cartItem.Quantity);
			cartItem.LineDiscount = cartItem.Product.LineDiscount(cartItem.Quantity);
			cartItem.LineTotal = cartItem.CalculateLineTotal();
		}

		public static decimal CalculateLineTotal(this CartItem cartItem)
		{
			if (cartItem == null)
			{
				return 0M;
			}
			return cartItem.UnitPriceExt - cartItem.LineDiscount;
		}

		public static Cart UpdateDiscountCode(this Cart cart, string discountCode, Controllers.BaseClass.BaseController controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			ApplyDiscountCodeHelper(cart, discountCode, controller.CurrentStoreFrontConfigOrThrow, true, controller.GStoreDb, controller);
			return cart;
		}

		public static Cart PreviewUpdateDiscountCode(this Cart cart, string discountCode, StoreFrontConfiguration storeFrontConfig, Controllers.BaseClass.BaseController controller)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}

			ApplyDiscountCodeHelper(cart, discountCode, storeFrontConfig, false, null, controller);
			return cart;
		}

		/// <summary>
		/// Returns true if successful, false if not
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="discountCode"></param>
		/// <param name="controller">if null, no user messages will be created, and if cart is null an error will be thrown when trying to create the cart</param>
		/// <returns></returns>
		private static bool ApplyDiscountCodeHelper(this Cart cart, string discountCode, StoreFrontConfiguration storeFrontConfig, bool saveToDb, IGstoreDb db, Controllers.BaseClass.BaseController controller)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			if (saveToDb && db == null)
			{
				throw new ArgumentNullException("db", "Database is needed when SaveToDb = true");
			}

			StoreFront storeFront = storeFrontConfig.StoreFront;
			if (cart == null && string.IsNullOrWhiteSpace(discountCode))
			{
				//no discountCode and no cart, do nothing
				return false;
			}
			if (cart != null && (cart.DiscountCode ?? string.Empty).Trim().ToLower() == (discountCode ?? string.Empty).Trim().ToLower())
			{
				//discount is already in use, do nothing
				return false;
			}

			if (cart == null)
			{
				if (controller == null)
				{
					throw new ArgumentNullException("controller", "controller is needed to create a new cart");
				}
				cart = controller.NewCart();
			}

			string discountCodeLabel = storeFrontConfig.CartOrderDiscountCodeLabel.OrDefault("Discount Code");

			if (string.IsNullOrWhiteSpace(discountCode))
			{
				//remove the discount code
				string oldDiscountCode = cart.DiscountCode;
				cart.DiscountCode = null;
				cart.Discount = null;
				cart.DiscountId = null;
				if (saveToDb)
				{
					cart = db.Carts.Update(cart);
					db.SaveChanges();
					cart.RecalcAndSave(db, true);
				}
				else
				{
					cart.RecalcNoSave(true);
				}
				if (controller != null)
				{
					controller.AddUserMessage(discountCodeLabel + " Removed.", "'" + oldDiscountCode.ToHtml() + "' is removed from your cart.", UserMessageType.Success);
				}
				return true;
			}

			Discount discount = storeFront.Discounts.SingleOrDefault(c => c.Code.ToLower() == discountCode.Trim().ToLower());
			if (discount == null)
			{
				//discount code not found
				cart.DiscountCodesAttempted = cart.DiscountCodesAttempted.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				cart.DiscountCodesFailed = cart.DiscountCodesFailed.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";

				if (saveToDb)
				{
					cart = db.Carts.Update(cart);
					db.SaveChanges();
				}
				else
				{
					cart.RecalcNoSave(true);
				}

				if (controller != null)
				{
					controller.AddUserMessage("Invalid " + discountCodeLabel + ".", "'" + discountCode.ToHtml() + "' is invalid. Please check your " + discountCodeLabel + ".", UserMessageType.Danger);
				}
				return false;
			}
			if (discount.IsPending)
			{
				//discount marked as pending by an admin
				cart.DiscountCodesAttempted = cart.DiscountCodesAttempted.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				cart.DiscountCodesFailed = cart.DiscountCodesFailed.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				if (saveToDb)
				{
					cart = db.Carts.Update(cart);
					db.SaveChanges();
				}

				if (controller != null)
				{
					controller.AddUserMessage("Invalid " + discountCodeLabel + ".", "'" + discountCode.ToHtml() + "' is not active. Please use another " + discountCodeLabel + ".", UserMessageType.Danger);
				}
				return false;
			}
			if (discount.StartDateTimeUtc > DateTime.UtcNow)
			{
				//future discount
				cart.DiscountCodesAttempted = cart.DiscountCodesAttempted.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				cart.DiscountCodesFailed = cart.DiscountCodesFailed.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				if (saveToDb)
				{
					cart = db.Carts.Update(cart);
					db.SaveChanges();
				}

				if (controller != null)
				{
					controller.AddUserMessage("Invalid " + discountCodeLabel + ".", "'" + discountCode.ToHtml() + "' is not available yet. Please try after " + discount.StartDateTimeUtc.ToLocalTime().ToShortDateString() + " or use another " + discountCodeLabel + ".", UserMessageType.Danger);
				}
				return false;
			}

			if (discount.EndDateTimeUtc < DateTime.UtcNow)
			{
				//past discount
				cart.DiscountCodesAttempted = cart.DiscountCodesAttempted.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				cart.DiscountCodesFailed = cart.DiscountCodesFailed.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				if (saveToDb)
				{
					cart = db.Carts.Update(cart);
					db.SaveChanges();
				}

				if (controller != null)
				{
					controller.AddUserMessage("Invalid " + discountCodeLabel + ".", "'" + discountCode.ToHtml() + "' expired on " + discount.EndDateTimeUtc.ToLocalTime().ToShortDateString() + ". Please use another " + discountCodeLabel + ".", UserMessageType.Danger);
				}
				return false;
			}

			if ((discount.MaxUses != 0) && (discount.UseCount >= discount.MaxUses))
			{
				//discount used up
				cart.DiscountCodesAttempted = cart.DiscountCodesAttempted.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				cart.DiscountCodesFailed = cart.DiscountCodesFailed.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
				if (saveToDb)
				{
					cart = db.Carts.Update(cart);
					db.SaveChanges();
				}

				if (controller != null)
				{
					controller.AddUserMessage("Invalid " + discountCodeLabel + ".", "'" + discountCode.ToHtml() + "' is no longer available. Please use another " + discountCodeLabel + ".", UserMessageType.Danger);
				}
				return false;
			}

			cart.DiscountCodesAttempted = cart.DiscountCodesAttempted.AndDelimiter(", ") + "'" + discountCode.Replace("'", "''") + "'";
			cart.DiscountCode = discountCode;
			cart.Discount = discount;
			cart.DiscountId = discount.DiscountId;
			if (saveToDb)
			{
				cart = db.Carts.Update(cart);
				db.SaveChanges();
				cart.RecalcAndSave(db, true);
			}
			else
			{
				cart.RecalcNoSave(true);
			}

			if (controller != null)
			{
				controller.AddUserMessage(discountCodeLabel + " Applied.", "'" + discountCode.ToHtml() + "' is applied to your cart.", UserMessageType.Success);
			}
			return true;

		}

		/// <summary>
		/// filters to products that can be added to the shopping cart by storefront rules
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<Product> CanAddToCart(this IQueryable<Product> query, StoreFront storeFront)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			//todo: add storefront add to cart rules (in inventory, backorder, etc)

			return query.WhereIsActive();
		}

		public static CartItem FindItemInCart(this Cart cart, Product product, string productVariantInfo)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}

			if (cart == null)
			{
				return null;
			}

			if (cart.CartItems == null || cart.CartItems.Count == 0)
			{
				return null;
			}

			return cart.CartItems.SingleOrDefault(ci => ci.ProductId == product.ProductId && ci.ProductVariantInfo == productVariantInfo);
		}

		public static bool ItemExistsInCart(this Cart cart, Product product, string productVariantInfo)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}

			if (cart == null)
			{
				return false;
			}

			if (cart.CartItems == null || cart.CartItems.Count == 0)
			{
				return false;
			}

			return cart.CartItems.Any(ci => ci.ProductId == product.ProductId && ci.ProductVariantInfo == productVariantInfo);
		}

		public static bool IsEmpty(this Cart cart)
		{
			if (cart == null || cart.ItemCount == 0)
			{
				return true;
			}
			return false;
		}

		public static IEnumerable<SelectListItem> QuantityList(this CartItem cartItem)
		{
			int currentQty = cartItem.Quantity;

			int maxQty = 99;
			if (cartItem != null && cartItem.Product != null && cartItem.Product.MaxQuantityPerOrder > 0 && cartItem.Product.MaxQuantityPerOrder < 99)
			{
				maxQty = cartItem.Product.MaxQuantityPerOrder;
			}

			int[] values = new int[maxQty];

			for (int i = 0; i < maxQty; i++)
			{
				values[i] = i + 1;
			}

			IEnumerable<SelectListItem> items = values.Select(i => new SelectListItem(){ Text = i.ToString(), Value = i.ToString(), Selected = (i == currentQty) });
			
			
			return items;
		}

		public static Models.ViewModels.CartConfigViewModel CartConfigViewModel(this StoreFrontConfiguration config, bool isViewPage = false, bool isEditPage = false)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			return new Models.ViewModels.CartConfigViewModel(config, isEditPage, isViewPage);
		}

		/// <summary>
		/// Returns the editor, validation message, and help label for a cart configuration field
		/// </summary>
		/// <typeparam name="Cart"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString CartFieldEditorFor<TModel, TShowField, TLabelField>(this HtmlHelper<TModel> htmlHelper, bool isReadOnly, Expression<Func<TModel, TShowField>> showField, Expression<Func<TModel, TLabelField>> labelField, bool leftToRight = false) where TModel : Cart
		{
			if (showField == null && labelField == null)
			{
				return new MvcHtmlString(string.Empty);
			}

			StringBuilder html = new StringBuilder();
			html.AppendLine("<div class='bg-warning'>");
			if (showField != null)
			{
				html.AppendLine(htmlHelper.CartFieldEditorHelper(isReadOnly, showField, leftToRight));
			}

			if (labelField != null)
			{
				html.AppendLine(htmlHelper.CartFieldEditorHelper(isReadOnly, labelField, leftToRight));
			}
			html.AppendLine("</div>");

			return new MvcHtmlString(html.ToString());
		}

		public static MvcHtmlString CartFieldEditorFor<TModel, TField>(this HtmlHelper<TModel> htmlHelper, bool isReadOnly, Expression<Func<TModel, TField>> field, bool leftToRight = false) where TModel : Cart
		{
			if (field == null)
			{
				return new MvcHtmlString(string.Empty);
			}

			StringBuilder html = new StringBuilder();
			html.AppendLine("<div class='bg-warning'>");
			html.AppendLine(htmlHelper.CartFieldEditorHelper(isReadOnly, field));
			html.AppendLine("</div>");

			return new MvcHtmlString(html.ToString());
		}

		private static string CartFieldEditorHelper<TModel, TField>(this HtmlHelper<TModel> htmlHelper, bool isReadOnly, Expression<Func<TModel, TField>> field, bool leftToRight = false) where TModel : Cart
		{
			if (field == null)
			{
				return string.Empty;
			}

			ModelMetadata fieldMetadata = ModelMetadata.FromLambdaExpression(field, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(field);

			StringBuilder html = new StringBuilder();
			html.AppendLine(htmlHelper.LabelFor(field, new { @class = "control-label" }).ToHtmlString());
			if (leftToRight)
			{
				html.AppendLine("&nbsp;");
			}
			else
			{
				html.AppendLine("<br/>");
			}
			RouteValueDictionary htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = "form-control" });
			if (isReadOnly)
			{
				if (typeof(TField) == typeof(bool))
				{
					htmlAttributes.Add("disabled", "disabled");
				}
				htmlAttributes.Add("readonly", "readonly");
			}
			html.AppendLine(htmlHelper.EditorFor(field, new { htmlAttributes = htmlAttributes}).ToHtmlString());
			if (leftToRight)
			{
				html.AppendLine("&nbsp;");
			}
			else
			{
				html.AppendLine("<br/>");
			}
			html.AppendLine(htmlHelper.HelpLabelFor(field, new { @class = "help-label" }).ToHtmlString());

			return html.ToString();
		}



	}
}