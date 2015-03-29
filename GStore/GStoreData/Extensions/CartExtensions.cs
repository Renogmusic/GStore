using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GStoreData.AppHtmlHelpers;
using GStoreData.ControllerBase;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData
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

			List<Cart> carts = query.ToList();
			if (carts == null || carts.Count == 0)
			{
				return null;
			}
			if (carts.Count > 1)
			{
				return carts.OrderByDescending(c => c.UpdateDateTimeUtc).First();
			}
			return carts[0];
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
			cartItem.ListPrice = product.ListPrice(quantity) ?? 0M;
			cartItem.ListPriceExt = product.ListPriceExt(quantity) ?? 0M;
			cartItem.Product = product;
			cartItem.ProductId = product.ProductId;
			cartItem.ProductVariantInfo = productVariantInfo;
			cartItem.Quantity = quantity;
			cartItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			cartItem.StoreFront = storeFront;
			cartItem.StoreFrontId = storeFront.StoreFrontId;
			cartItem.UnitPrice = product.UnitPrice(quantity) ?? 0M;
			cartItem.UnitPriceExt = product.UnitPriceExt(quantity) ?? 0M;
			cartItem.LineDiscount = product.LineDiscount(quantity);
			cartItem.LineTotal = cartItem.CalculateLineTotal();

			List<CartItem> cartItems = new List<CartItem>();
			cartItems.Add(cartItem);

			if (storeFront.ProductBundles.AsQueryable().CanAddToCart(storeFront).Any())
			{
				//add a bundle to the cart
				ProductBundle bundle = storeFront.ProductBundles.AsQueryable().CanAddToCart(storeFront).First();
				CartBundle cartBundle = new CartBundle();
				cartBundle.SetDefaults(userProfileOrNull);
				cartBundle.CartBundleId = 0;
				cartBundle.Cart = cart;
				cartBundle.CartId = cart.CartId;
				cartBundle.Client = storeFront.Client;
				cartBundle.ClientId = storeFront.ClientId;
				cartBundle.StoreFront = storeFront;
				cartBundle.StoreFrontId = storeFront.StoreFrontId;
				cartBundle.IsPending = false;
				cartBundle.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				cartBundle.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				cartBundle.ProductBundle = bundle;
				cartBundle.ProductBundleId = bundle.ProductBundleId;

				if ((bundle.MaxQuantityPerOrder != 0) && (quantity > bundle.MaxQuantityPerOrder))
				{
					quantity = bundle.MaxQuantityPerOrder;
				}
				cartBundle.Quantity = quantity;

				cartBundle.CartItems = new List<CartItem>();
				foreach (ProductBundleItem item in bundle.ProductBundleItems)
				{
					CartItem newItem = new CartItem();
					newItem.SetDefaults(userProfileOrNull);
					newItem.Cart = cart;
					newItem.CartId = cart.CartId;
					newItem.Client = storeFront.Client;
					newItem.ClientId = storeFront.ClientId;
					newItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					newItem.IsPending = false;
					newItem.Product = item.Product;
					newItem.ProductId = item.ProductId;
					newItem.ProductVariantInfo = item.ProductVariantInfo;
					newItem.Quantity = cartBundle.Quantity * item.Quantity;
					newItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					newItem.StoreFront = storeFront;
					newItem.StoreFrontId = storeFront.StoreFrontId;
					newItem.ListPrice = item.ListPrice(newItem.Quantity) ?? 0M;
					newItem.ListPriceExt = item.ListPriceExt(newItem.Quantity) ?? 0M;
					newItem.UnitPrice = item.UnitPrice(newItem.Quantity) ?? 0M;
					newItem.UnitPriceExt = item.UnitPriceExt(newItem.Quantity) ?? 0M;
					newItem.LineDiscount = item.LineDiscount(newItem.Quantity);
					newItem.LineTotal = newItem.CalculateLineTotal();
					newItem.CartBundle = cartBundle;
					newItem.CartBundleId = cartBundle.CartBundleId;
					newItem.ProductBundleItem = item;
					newItem.ProductBundleItemId = item.ProductBundleItemId;
					newItem.CartBundleId = cartBundle.CartBundleId;

					cartItems.Add(newItem);
					cartBundle.CartItems.Add(newItem);
				}

				List<CartBundle> cartBundles = new List<CartBundle>();
				cartBundles.Add(cartBundle);
				cart.CartBundles = cartBundles;
			}

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

		public static CartItem AddToCartById(this Cart cart, int productId, int quantity, string productVariantInfo, GStoreData.ControllerBase.BaseController controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			Product product = controller.CurrentStoreFrontOrThrow.Products.SingleOrDefault(p => p.ProductId == productId);
			if (product == null)
			{
				throw new ApplicationException("Product not found in storefront by id: " + productId);
			}
			return cart.AddToCart(product, quantity, productVariantInfo, controller);
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
		public static CartItem AddToCart(this Cart cart, Product product, int quantity, string productVariantInfo, GStoreData.ControllerBase.BaseController controller)
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
			if (!product.BaseUnitPrice.HasValue)
			{
				throw new ApplicationException("product.BaseUnitPrice is null");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			if (!storeFront.Products.AsQueryable().CanAddToCart(storeFront).Any(p => p.ProductId == product.ProductId))
			{
				throw new ApplicationException("Cannot add product id: " + product.ProductId + " to cart. Not found in store front available and active products for Store Front '" + storeFrontConfig.Name + "' [" + storeFront.StoreFrontId + "].");
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
			cartItem.StoreFront = storeFront;
			cartItem.StoreFrontId = storeFront.StoreFrontId;

			cartItem.IsPending = false;
			cartItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			cartItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			cartItem.Product = product;
			cartItem.ProductId = product.ProductId;
			cartItem.ProductVariantInfo = productVariantInfo;

			cartItem.UpdateQuantityNoSave(quantity, controller);
			cartItem.ListPrice = product.ListPrice(cartItem.Quantity) ?? product.UnitPrice(cartItem.Quantity).Value;
			cartItem.ListPriceExt = product.ListPriceExt(cartItem.Quantity) ?? product.UnitPriceExt(cartItem.Quantity).Value;
			cartItem.UnitPrice = product.UnitPrice(cartItem.Quantity).Value;
			cartItem.UnitPriceExt = product.UnitPriceExt(cartItem.Quantity).Value;
			cartItem.LineDiscount = product.LineDiscount(cartItem.Quantity);
			cartItem.LineTotal = cartItem.CalculateLineTotal();

			cartItem = db.CartItems.Add(cartItem);
			db.SaveChanges();

			cartItem.Cart.RecalcAndSave(db, true);
			return cartItem;
		}

		public static CartBundle AddBundleToCartById(this Cart cart, int productBundleId, int quantity, GStoreData.ControllerBase.BaseController controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			ProductBundle bundle = controller.CurrentStoreFrontOrThrow.ProductBundles.SingleOrDefault(pb => pb.ProductBundleId == productBundleId);
			if (bundle == null)
			{
				throw new ApplicationException("Product Bundle not found in storefront by id: " + productBundleId);
			}
			return cart.AddBundleToCart(bundle, quantity, controller);
		}

		/// <summary>
		/// Adds a bundle and all its items to the cart. Creates the cart if it does not exist (cart == null)
		/// </summary>
		/// <returns></returns>
		public static CartBundle AddBundleToCart(this Cart cart, ProductBundle productBundle, int quantity, GStoreData.ControllerBase.BaseController controller)
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
			if (productBundle == null)
			{
				throw new ArgumentNullException("product");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			if (!storeFront.ProductBundles.AsQueryable().CanAddToCart(storeFront).Any(p => p.ProductBundleId == productBundle.ProductBundleId))
			{
				throw new ApplicationException("Cannot add product bundle id: " + productBundle.ProductBundleId + " to cart. Not found in store front active and available bundles for Store Front '" + storeFrontConfig.Name + "' [" + storeFront.StoreFrontId + "].");
			}

			if (cart == null)
			{
				cart = controller.NewCart();
			}

			CartBundle cartBundle = db.CartBundles.Create();
			cartBundle.SetDefaults(userProfileOrNull);
			cartBundle.Cart = cart;
			cartBundle.CartId = cart.CartId;
			cartBundle.Client = storeFront.Client;
			cartBundle.ClientId = storeFront.ClientId;
			cartBundle.StoreFront = storeFront;
			cartBundle.StoreFrontId = storeFront.StoreFrontId;
			cartBundle.IsPending = false;
			cartBundle.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			cartBundle.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			cartBundle.ProductBundle = productBundle;
			cartBundle.ProductBundleId = productBundle.ProductBundleId;

			if ((productBundle.MaxQuantityPerOrder != 0) && (quantity > productBundle.MaxQuantityPerOrder))
			{
				//over quantity
				if (controller != null)
				{
					cartBundle = cartBundle.UpdateQuantityNoSave(quantity, controller);
				}
			}
			cartBundle.Quantity = quantity;
			cartBundle = db.CartBundles.Add(cartBundle);
			db.SaveChanges();

			if (cart == null)
			{
				cart = controller.NewCart();
			}

			foreach (ProductBundleItem item in productBundle.ProductBundleItems.AsQueryable().WhereIsActive().ApplyDefaultSort())
			{
				CartItem cartItem = db.CartItems.Create();
				cartItem.SetDefaults(userProfileOrNull);
				cartItem.Cart = cart;
				cartItem.CartId = cart.CartId;
				cartItem.CartBundleId = cartBundle.CartBundleId;
				cartItem.Client = storeFront.Client;
				cartItem.ClientId = storeFront.ClientId;
				cartItem.StoreFront = storeFront;
				cartItem.StoreFrontId = storeFront.StoreFrontId;

				cartItem.IsPending = false;
				cartItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				cartItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

				cartItem.ProductBundleItem = item;
				cartItem.ProductBundleItemId = item.ProductBundleItemId;

				cartItem.Product = item.Product;
				cartItem.ProductId = item.ProductId;
				cartItem.ProductVariantInfo = item.ProductVariantInfo;

				cartItem.Quantity = cartBundle.Quantity * item.Quantity;
				cartItem.RecalcNoSave();

				cartItem = db.CartItems.Add(cartItem);
			}

			db.SaveChanges();

			cartBundle.RecalcBundleItemsNoSave();

			cart.RecalcAndSave(db, true);

			return cartBundle;
		}

		/// <summary>
		/// Adds an item to the cart. Creates the cart if it does not exist (cart == null), returns user messages if quantity is over the limit
		/// Controller is only used for user messages. if no used messages wanted, set controller = null
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
			if (cartItem.CartBundleId.HasValue)
			{
				throw new ApplicationException("Cannot use UpdateQuantityNoSave on items in a product bundle.");
			}
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

			if (cartItemToRemove.ProductBundleItemId.HasValue)
			{
				throw new ApplicationException("Cart Item cannot be removed because it is part of a bundle. Cart Item Id: " + cartItemToRemove.CartItemId + " Product UrlName: " + cartItemToRemove.ProductBundleItem.Product.UrlName + " Bundle UrlName: " + cartItemToRemove.ProductBundleItem.ProductBundle.UrlName);
			}
			if (cartItemToRemove.CartBundleId.HasValue)
			{
				throw new ApplicationException("Cart Item cannot be removed because it is part of a bundle. Cart Item Id: " + cartItemToRemove.CartItemId + " Cart Bundle Id: " + cartItemToRemove.CartBundle.CartBundleId);
			}

			Cart cart = cartItemToRemove.Cart;
						
			bool result = db.CartItems.Delete(cartItemToRemove);
			cart.RecalcAndSave(db, false);
			return result;
		}

		/// <summary>
		/// Updated the quantity of a bundle in the cart. returns user messages if quantity is over the limit
		/// Controller is only used for user messages. if no used messages wanted, set controller = null
		/// this method also recalculates price and totals for Bundle Items
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="storeFront"></param>
		/// <param name="db"></param>
		/// <param name="session"></param>
		/// <param name="userProfileOrNull"></param>
		/// <returns></returns>
		public static CartBundle UpdateQuantityAndSave(this CartBundle cartBundle, IGstoreDb db, int quantity, BaseController controller)
		{
			cartBundle = cartBundle.UpdateQuantityNoSave(quantity, controller);
			cartBundle = db.CartBundles.Update(cartBundle);
			cartBundle.Cart.RecalcAndSave(db, true);
			return cartBundle;
		}

		/// <summary>
		/// Updates the quantity on a cart bundle and adjusts for if over max and sets user messages if user is over quantity
		/// Controller is only user for user messages. if no used messages wanted, set controller = null
		/// </summary>
		/// <param name="cartItem"></param>
		/// <param name="quantity"></param>
		/// <param name="controller"></param>
		/// <returns></returns>
		public static CartBundle UpdateQuantityNoSave(this CartBundle cartBundle, int quantity, BaseController controller)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be from 1 to 10,000. For 0 quantity call RemoveFromCart");
			}
			if (cartBundle == null)
			{
				throw new ArgumentNullException("cartItem");
			}

			if ((cartBundle.ProductBundle.MaxQuantityPerOrder != 0) && (quantity > cartBundle.ProductBundle.MaxQuantityPerOrder))
			{
				//user wants more than the max allowed per order
				bool atMaxBefore = (cartBundle.Quantity >= cartBundle.ProductBundle.MaxQuantityPerOrder);
				if (atMaxBefore)
				{
					//was already at the max qty. give a message the user is at the max allowed per order, cannot add more
					if (controller != null)
					{
						controller.AddUserMessage("Quantity Limit:", "This bundle is limited to " + cartBundle.ProductBundle.MaxQuantityPerOrder + " per order and you are at the limit.", UserMessageType.Danger);
					}
					return cartBundle;
				}

				if (quantity > cartBundle.ProductBundle.MaxQuantityPerOrder)
				{
					//was not at max qty before, adjust user to the max and give info message
					quantity = cartBundle.ProductBundle.MaxQuantityPerOrder;

					if (controller != null)
					{
						controller.AddUserMessage("Quantity Limit:", "This bundle is limited to " + cartBundle.ProductBundle.MaxQuantityPerOrder + " per order. You now have " + cartBundle.ProductBundle.MaxQuantityPerOrder + " of them in your cart.", UserMessageType.Info);
					}
				}

			}

			cartBundle.Quantity = quantity;

			cartBundle = cartBundle.RecalcBundleItemsNoSave();

			return cartBundle;

		}

		/// <summary>
		/// Recalculates quantity and price for bundled items
		/// </summary>
		/// <param name="cartBundle"></param>
		/// <returns></returns>
		public static CartBundle RecalcBundleItemsAndSave(this CartBundle cartBundle, IGstoreDb db)
		{
			foreach (CartItem cartItem in cartBundle.CartItems)
			{
				cartItem.Quantity = cartBundle.Quantity * cartItem.ProductBundleItem.Quantity;
				cartItem.RecalcNoSave();
				db.CartItems.Update(cartItem);
			}

			db.SaveChanges();

			return cartBundle;
		}

		/// <summary>
		/// Recalculates quantity and price for bundled items
		/// </summary>
		/// <param name="cartBundle"></param>
		/// <returns></returns>
		public static CartBundle RecalcBundleItemsNoSave(this CartBundle cartBundle)
		{
			if (cartBundle == null)
			{
				throw new ArgumentNullException("cartBundle");
			}

			if (cartBundle.Quantity < 1)
			{
				throw new ApplicationException("Cart Bundle quantity is less than Zero for Cart Bundle Id: " + cartBundle.CartBundleId);
			}

			if (cartBundle.CartItems == null)
			{
				return cartBundle;
			}

			foreach (CartItem cartItem in cartBundle.CartItems)
			{
				if (cartItem.ProductBundleItem == null)
				{
					throw new ApplicationException("cartItem.ProductBundleItem == null for Cart Item Id: " + cartItem.CartItemId + " in Cart Bundle Id: " + cartBundle.CartBundleId);
				}
				cartItem.Quantity = cartBundle.Quantity * cartItem.ProductBundleItem.Quantity;
				cartItem.RecalcNoSave();
			}

			return cartBundle;
		}

		public static bool RemoveFromCart(this CartBundle cartBundleToRemove, IGstoreDb db)
		{
			if (cartBundleToRemove == null)
			{
				throw new ArgumentNullException("cartBundleToRemove");
			}

			Cart cart = cartBundleToRemove.Cart;
			List<CartItem> cartItems = cart.CartItems.Where(ci => ci.CartBundleId == cartBundleToRemove.CartBundleId).ToList(); 
			foreach (CartItem item in cartItems)
			{
				db.CartItems.Delete(item);
			}

			bool result = db.CartBundles.Delete(cartBundleToRemove);
			cart.RecalcAndSave(db, false);
			return result;
		}

		private static Cart NewCart(this GStoreData.ControllerBase.BaseController controller)
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
			newCart.UserProfile = userProfileOrNull;
			newCart.UserProfileId = (userProfileOrNull == null ? (int?)null : userProfileOrNull.UserProfileId);
			if (userProfileOrNull != null)
			{
				newCart.FullName = userProfileOrNull.FullName;
				newCart.Email = userProfileOrNull.Email;
			}
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

			bool success = false;
			newCart.UpdateDiscountCode(discountCode, controller, out success);

			return newCart;

		}

		private static Cart NewPreviewCart(this GStoreData.ControllerBase.BaseController controller)
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
			storeFrontConfig.CartThemeId = cartConfig.CartThemeId;

			storeFrontConfig.CartEmptyMessage = cartConfig.CartEmptyMessage;
			storeFrontConfig.CartCheckoutButtonLabel = cartConfig.CartCheckoutButtonLabel;
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
			storeFrontConfig.CartBundleShowIncludedItems = cartConfig.CartBundleShowIncludedItems;
			storeFrontConfig.CartBundleShowPriceOfIncludedItems = cartConfig.CartBundleShowPriceOfIncludedItems;
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

		public static void DumpCartNoSave(this StoreFront storeFront, IGstoreDb db, Cart cart)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (cart != null)
			{
				db.CartBundles.DeleteRange(cart.CartBundles);
				db.CartItems.DeleteRange(cart.CartItems);
				db.Carts.Delete(cart);
			}
		}

		/// <summary>
		/// cancels any checkout in progress. Used after a cart is updated or quantity changed or discount code applied
		/// </summary>
		/// <param name="cart"></param>
		/// <returns></returns>
		public static Cart CancelCheckout(this Cart cart, IGstoreDb db)
		{
			if (cart == null)
			{
				return null;
			}
			if (cart.StatusStartedCheckout)
			{
				cart.StatusStartedCheckout = false;
				cart.StatusSelectedLogInOrGuest = false;
				cart.StatusCompletedDeliveryInfo = false;
				cart.StatusSelectedDeliveryMethod = false;
				cart.StatusPaymentInfoConfirmed = false;
				cart = db.Carts.Update(cart);
				db.SaveChanges();
			}
			return cart;
		}

		/// <summary>
		/// Merges items in the users cart with items from their anonymous cart
		/// after items are moved, the old user profile cart is dumped
		/// This method saves changes to the database after migrating
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
			if (storeFront.Carts.Any(c => c.UserProfileId == userProfile.UserProfileId && c.Order == null))
			{
				//pick up all the items from the old cart and move it to the new cart

				Cart oldUserCart = storeFront.Carts.Single(c => c.UserProfileId == userProfile.UserProfileId && c.Order == null);
				List<CartItem> oldCartItems = oldUserCart.CartItems.AsQueryable().Where(ci => !ci.CartBundleId.HasValue && !ci.ProductBundleItemId.HasValue).ApplyDefaultSort().ToList();
				List<CartBundle> oldCartBundles = oldUserCart.CartBundles.AsQueryable().ApplyDefaultSort().ToList();

				storeFront.DumpCartNoSave(db, oldUserCart);
				db.SaveChanges();

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

				foreach(CartBundle bundle in oldCartBundles)
				{
					if (cart.CartBundles.Any(cb => cb.ProductBundleId == bundle.ProductBundleId))
					{
						//bundle exists in old and new cart; update quantity and ensure not over max
						CartBundle existingBundle = cart.CartBundles.Single(cb => cb.ProductBundleId == bundle.ProductBundleId);
						existingBundle.UpdateQuantityNoSave(existingBundle.Quantity + bundle.Quantity, null);
					}
					else
					{
						//add bundle to cart
						cart.AddBundleToCartById(bundle.ProductBundleId, bundle.Quantity, controller);
					}
				}

				foreach (CartItem item in oldCartItems)
				{
					if (cart.CartItems.Any(c => !c.CartBundleId.HasValue && !c.ProductBundleItemId.HasValue && c.ProductId == item.ProductId && c.ProductVariantInfo == item.ProductVariantInfo))
					{
						//item exists in old and new cart; update quantity and ensure not over max
						CartItem existingItem = cart.CartItems.Single(c => !c.CartBundleId.HasValue && !c.ProductBundleItemId.HasValue && c.ProductId == item.ProductId && c.ProductVariantInfo == item.ProductVariantInfo);
						existingItem.UpdateQuantityNoSave(existingItem.Quantity + item.Quantity, null);
					}
					else
					{
						cart.AddToCartById(item.ProductId, item.Quantity, item.ProductVariantInfo, controller);
					}
				}

				cart.AllItemsAreDigitalDownload = oldUserCart.AllItemsAreDigitalDownload; 
				cart.DeliveryInfoDigitalId = oldUserCart.DeliveryInfoDigitalId;
				cart.CartPaymentInfoId = oldUserCart.CartPaymentInfoId;
				cart.DeliveryInfoShippingId = oldUserCart.DeliveryInfoShippingId;
				cart.Handling = oldUserCart.Handling;
				cart.Shipping = oldUserCart.Shipping;
				cart.ShippingItemCount = oldUserCart.ShippingItemCount;
				cart.StatusCompletedDeliveryInfo = false;
				cart.StatusEmailedConfirmation = false;
				cart.StatusEmailedContents = false;
				cart.StatusPaymentInfoConfirmed = false;
				cart.StatusPlacedOrder = false;
				cart.StatusPrintedConfirmation = false;
				cart.StatusSelectedDeliveryMethod = false;
				cart.StatusSelectedLogInOrGuest = true;
				cart.StatusStartedCheckout = false;
				cart.RecalcNoSave(true);

				cart.UserProfileId = userProfile.UserProfileId;
				cart.Email = userProfile.Email;

				cart = db.Carts.Update(cart);
				db.SaveChanges();
				

			}
			else
			{
				//user has no cart, so move this one to their login
				cart.UserProfileId = userProfile.UserProfileId;
				cart.Email = userProfile.Email;
				cart = db.Carts.Update(cart);
				db.SaveChanges();
			}
			return cart;
		}

		public static Cart MigrateCartToAnonymous(this StoreFront storeFront, IGstoreDb db, Cart cart)
		{
			cart.UserProfileId = null;
			cart = db.Carts.Update(cart);
			cart.StatusCompletedDeliveryInfo = false;
			cart.StatusPaymentInfoConfirmed = false;
			cart.StatusPrintedConfirmation = false;
			cart.StatusSelectedDeliveryMethod = false;
			cart.StatusSelectedLogInOrGuest = false;
			db.SaveChanges();
			return cart;
		}

		/// <summary>
		/// Returns true if order has items and meets the order minimum and any other requirements for checkout
		/// Sets user messages using controller (required)
		/// </summary>
		/// <param name="cart"></param>
		/// <param name="controller">User for user messages (required)</param>
		/// <returns></returns>
		public static bool CartIsValidForCheckout(this Cart cart, BaseController controller)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrNull;
			if (config == null)
			{
				throw new ArgumentNullException("controller.CurrentStoreFrontConfigOrNull");
			}

			if (cart == null || cart.CartItems.Count == 0)
			{
				controller.AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return false;
			}

			if (cart == null || cart.CartItems.Count == 0)
			{
				controller.AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return false;
			}

			if (cart == null || cart.CartItems.Count == 0)
			{
				controller.AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return false;
			}

			if (cart.Total < config.CheckoutOrderMinimum)
			{
				controller.AddUserMessage("Order minimum not met", "Sorry, the minimum order amount is $" + config.CheckoutOrderMinimum.ToString("N2"), UserMessageType.Danger);
				return false;
			}

			return true;
		}

		public static Cart ValidateCartAndSave(this Cart cart, BaseController controller)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (cart.StatusPlacedOrder || cart.OrderId.HasValue)
			{
				throw new ApplicationException("Cart already has an order");
			}

			bool cartItemsUpdated = false;

			foreach(CartBundle bundle in cart.CartBundles)
			{
				int maxQty = bundle.ProductBundle.MaxQuantityPerOrder;
				if (maxQty != 0 && bundle.Quantity > maxQty)
				{
					//adjust quantity and set user messages and recalc cart
					bundle.UpdateQuantityNoSave(bundle.Quantity, controller);
					controller.GStoreDb.CartBundles.Update(bundle);
					cartItemsUpdated = true;
				}
			}
			foreach (CartItem item in cart.CartItems)
			{
				if (!item.CartBundleId.HasValue)
				{
					int maxQty = item.Product.MaxQuantityPerOrder;
					if (maxQty != 0 && item.Quantity > maxQty)
					{
						//adjust quantity and set user messages and recalc cart
						item.UpdateQuantityNoSave(item.Quantity, controller);

						controller.GStoreDb.CartItems.Update(item);
						cartItemsUpdated = true;
					}
				}
			}

			if (cartItemsUpdated)
			{
				cart.StatusCompletedDeliveryInfo = false;
				cart.StatusEmailedConfirmation = false;
				cart.StatusEmailedContents = false;
				cart.StatusPaymentInfoConfirmed = false;
				cart.StatusPrintedConfirmation = false;
				cart.StatusSelectedDeliveryMethod = false;
				cart.StatusPaymentInfoConfirmed = false;
				cart.StatusPrintedConfirmation = false;

			}

			cart.RecalcNoSave(true);

			cart = controller.GStoreDb.Carts.Update(cart);
			controller.GStoreDb.SaveChanges();

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
			else
			{
				cart.AllItemsAreDigitalDownload = false;
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
			if (cartItem.ProductBundleItemId.HasValue)
			{
				if (!cartItem.ProductBundleItem.BaseUnitPrice.HasValue)
				{
					throw new ApplicationException("cartItem.ProductBundleItem.BaseUnitPrice is null");
				}
				
				cartItem.UnitPrice = cartItem.ProductBundleItem.UnitPrice(cartItem.Quantity).Value;
				cartItem.UnitPriceExt = cartItem.ProductBundleItem.UnitPriceExt(cartItem.Quantity).Value;
				cartItem.ListPrice = cartItem.ProductBundleItem.ListPrice(cartItem.Quantity) ?? cartItem.UnitPrice;
				cartItem.ListPriceExt = cartItem.ProductBundleItem.ListPriceExt(cartItem.Quantity) ?? cartItem.UnitPriceExt;
				cartItem.LineDiscount = cartItem.ProductBundleItem.LineDiscount(cartItem.Quantity);
				cartItem.LineTotal = cartItem.CalculateLineTotal();
			}
			else
			{
				if (!cartItem.Product.BaseUnitPrice.HasValue)
				{
					throw new ApplicationException("cartItem.Product.BaseUnitPrice is null");
				}

				cartItem.UnitPrice = cartItem.Product.UnitPrice(cartItem.Quantity) ?? 0M;
				cartItem.UnitPriceExt = cartItem.Product.UnitPriceExt(cartItem.Quantity) ?? 0M;
				cartItem.ListPrice = cartItem.Product.ListPrice(cartItem.Quantity) ?? cartItem.UnitPrice;
				cartItem.ListPriceExt = cartItem.Product.ListPriceExt(cartItem.Quantity) ?? cartItem.UnitPriceExt;
				cartItem.LineDiscount = cartItem.Product.LineDiscount(cartItem.Quantity);
				cartItem.LineTotal = cartItem.CalculateLineTotal();
			}
		}

		public static decimal CalculateLineTotal(this CartItem cartItem)
		{
			if (cartItem == null)
			{
				return 0M;
			}
			return cartItem.UnitPriceExt - cartItem.LineDiscount;
		}

		public static Cart UpdateDiscountCode(this Cart cart, string discountCode, GStoreData.ControllerBase.BaseController controller, out bool success)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			success = ApplyDiscountCodeHelper(cart, discountCode, controller.CurrentStoreFrontConfigOrThrow, true, controller.GStoreDb, controller);
			return cart;
		}

		public static Cart PreviewUpdateDiscountCode(this Cart cart, string discountCode, StoreFrontConfiguration storeFrontConfig, GStoreData.ControllerBase.BaseController controller)
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
		private static bool ApplyDiscountCodeHelper(this Cart cart, string discountCode, StoreFrontConfiguration storeFrontConfig, bool saveToDb, IGstoreDb db, GStoreData.ControllerBase.BaseController controller)
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
			return query.WhereIsActive().Where(p => p.AvailableForPurchase);
		}

		/// <summary>
		/// filters to products that can be added to the shopping cart by storefront rules
		/// Runs a WhereIsActive and where .AvailableForPurchase
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductBundle> CanAddToCart(this IQueryable<ProductBundle> query, StoreFront storeFront)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			//todo: add storefront add to cart rules (in inventory, backorder, etc)
			return query.WhereIsActive().Where(p => p.AvailableForPurchase);
		}

		public static List<CartItem> FindBundleItemsInCart(this Cart cart, ProductBundle productBundle)
		{
			if (cart == null)
			{
				return null;
			}
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}

			if (cart.CartItems == null || cart.CartItems.Count == 0)
			{
				return null;
			}

			return cart.CartItems.Where(ci => ci.CartBundleId.HasValue && ci.CartBundle.ProductBundleId == productBundle.ProductBundleId).ToList();
		}

		public static CartBundle FindBundleInCart(this Cart cart, ProductBundle productBundle)
		{
			if (cart == null)
			{
				return null;
			}
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}

			if (cart.CartBundles == null || cart.CartBundles.Count == 0)
			{
				return null;
			}

			return cart.CartBundles.SingleOrDefault(ci => ci.ProductBundleId == productBundle.ProductBundleId);
		}

		//public static CartItem FindBundleItemInCart(this Cart cart, ProductBundle productBundle, Product product)
		//{
		//	if (product == null)
		//	{
		//		throw new ArgumentNullException("product");
		//	}
		//	if (cart == null)
		//	{
		//		return null;
		//	}
		//	if (productBundle == null)
		//	{
		//		throw new ArgumentNullException("productBundle");
		//	}

		//	if (cart.CartItems == null || cart.CartItems.Count == 0)
		//	{
		//		return null;
		//	}

		//	return cart.CartItems.SingleOrDefault(ci => ci.CartBundleId.HasValue && ci.CartBundle.ProductBundleId == productBundle.ProductBundleId && ci.ProductId == product.ProductId);
		//}

		public static CartItem FindItemInCart(this Cart cart, Product product, string productVariantInfo, bool includeBundles)
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

			if (includeBundles)
			{
				return cart.CartItems.SingleOrDefault(ci => ci.ProductId == product.ProductId && ci.ProductVariantInfo == productVariantInfo);
			}
			return cart.CartItems.SingleOrDefault(ci => ci.ProductId == product.ProductId && ci.ProductVariantInfo == productVariantInfo && !ci.CartBundleId.HasValue && !ci.ProductBundleItemId.HasValue);
		}

		public static bool ItemExistsInCart(this Cart cart, Product product, string productVariantInfo, bool includeBundles)
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

			if (includeBundles)
			{
				return cart.CartItems.Any(ci => ci.ProductId == product.ProductId && ci.ProductVariantInfo == productVariantInfo);
			}
			return cart.CartItems.Any(ci => ci.ProductId == product.ProductId && ci.ProductVariantInfo == productVariantInfo && !ci.CartBundleId.HasValue && !ci.ProductBundleItemId.HasValue);
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

		public static IEnumerable<SelectListItem> QuantityList(this CartBundle bundle)
		{
			int currentQty = bundle.Quantity;

			int maxQty = 99;
			if (bundle != null && bundle.ProductBundle != null && bundle.ProductBundle.MaxQuantityPerOrder > 0 && bundle.ProductBundle.MaxQuantityPerOrder < 99)
			{
				maxQty = bundle.ProductBundle.MaxQuantityPerOrder;
			}

			int[] values = new int[maxQty];

			for (int i = 0; i < maxQty; i++)
			{
				values[i] = i + 1;
			}

			IEnumerable<SelectListItem> items = values.Select(i => new SelectListItem() { Text = i.ToString(), Value = i.ToString(), Selected = (i == currentQty) });


			return items;
		}

		public static ViewModels.CartConfigViewModel CartConfigViewModel(this StoreFrontConfiguration config, bool isViewPage = false, bool isEditPage = false)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			return new ViewModels.CartConfigViewModel(config, isEditPage, isViewPage);
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

		public static void SetFromPayPalResponse(this CartPaymentInfo cartPaymentInfo, Cart cart, PayPal.Models.PayPalPaymentData response)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (cartPaymentInfo == null)
			{
				throw new ArgumentNullException("cartPaymentInfo");
			}
			if (string.IsNullOrEmpty(response.id))
			{
				throw new ArgumentNullException("response.id");
			}

			cartPaymentInfo.Cart = cart;
			cartPaymentInfo.CartId = cart.CartId;
			cartPaymentInfo.Client = cart.Client;
			cartPaymentInfo.ClientId = cart.ClientId;
			cartPaymentInfo.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			cartPaymentInfo.IsPending = false;
			cartPaymentInfo.Json = response.Json;
			cartPaymentInfo.PaymentSource = Models.BaseClasses.GStorePaymentSourceEnum.PayPal;
			cartPaymentInfo.PayPalPaymentId = response.id;
			cartPaymentInfo.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			cartPaymentInfo.StoreFront = cart.StoreFront;
			cartPaymentInfo.StoreFrontId = cart.StoreFrontId;

		}

		public static WebFormResponse LoginOrGuestProcessWebForm(this Cart cart, BaseController controller)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrThrow;
			WebForm webForm = config.CheckoutLogInOrGuestWebForm;
			if (webForm == null)
			{
				return null;
			}
			WebFormResponse oldResponseOrNull = cart.LoginOrGuestWebFormResponse;

			WebFormResponse response = FormProcessorExtensions.ProcessWebFormForCheckout(controller, webForm, oldResponseOrNull);
			return response;
		}

		public static WebFormResponse DeliveryInfoDigitalOnlyProcessWebForm(this Cart cart, BaseController controller)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrThrow;
			WebForm webForm = config.CheckoutDeliveryInfoDigitalOnlyWebForm;
			if (config.CheckoutDeliveryInfoDigitalOnlyWebForm == null)
			{
				return null;
			}

			DeliveryInfoDigital info = cart.DeliveryInfoDigital;
			WebFormResponse oldResponseOrNull = null;
			if (info != null)
			{
				oldResponseOrNull = info.WebFormResponse;
			}

			WebFormResponse response = FormProcessorExtensions.ProcessWebFormForCheckout(controller, webForm, oldResponseOrNull);
			return response;
		}

		public static WebFormResponse DeliveryInfoShippingProcessWebForm(this Cart cart, BaseController controller)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrThrow;
			WebForm webForm = config.CheckoutDeliveryInfoShippingWebForm;
			if (webForm == null)
			{
				return null;
			}

			DeliveryInfoShipping info = cart.DeliveryInfoShipping;
			WebFormResponse oldResponseOrNull = null;
			if (info != null)
			{
				oldResponseOrNull = info.WebFormResponse;
			}

			WebFormResponse response = FormProcessorExtensions.ProcessWebFormForCheckout(controller, webForm, oldResponseOrNull);
			return response;
		}

		public static WebFormResponse DeliveryMethodProcessWebForm(this Cart cart, BaseController controller)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrThrow;
			WebForm webForm = config.CheckoutDeliveryMethodWebForm;
			if (webForm == null)
			{
				return null;
			}

			DeliveryInfoShipping info = cart.DeliveryInfoShipping;
			WebFormResponse oldResponseOrNull = null;
			if (info != null)
			{
				oldResponseOrNull = info.DeliveryMethodWebFormResponse;
			}

			WebFormResponse response = FormProcessorExtensions.ProcessWebFormForCheckout(controller, webForm, oldResponseOrNull);
			return response;
		}

		public static WebFormResponse PaymentInfoProcessWebForm(this Cart cart, BaseController controller)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrThrow;
			WebForm webForm = config.CheckoutPaymentInfoWebForm;
			if (webForm == null)
			{
				return null;
			}

			CartPaymentInfo info = cart.CartPaymentInfo;
			WebFormResponse oldResponseOrNull = null;
			if (info != null)
			{
				oldResponseOrNull = info.WebFormResponse;
			}

			WebFormResponse response = FormProcessorExtensions.ProcessWebFormForCheckout(controller, webForm, oldResponseOrNull);
			return response;
		}

		public static WebFormResponse ConfirmOrderProcessWebForm(this Cart cart, BaseController controller)
		{
			if (cart == null)
			{
				throw new ArgumentNullException("cart");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			StoreFrontConfiguration config = controller.CurrentStoreFrontConfigOrThrow;
			WebForm webForm = config.CheckoutConfirmOrderWebForm;
			if (webForm == null)
			{
				return null;
			}

			WebFormResponse oldResponseOrNull = cart.ConfirmOrderWebFormResponse;

			WebFormResponse response = FormProcessorExtensions.ProcessWebFormForCheckout(controller, webForm, oldResponseOrNull);
			return response;
		}


		public static decimal? ListPrice(this CartBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.CartItems == null || bundle.CartItems.Count == 0)
			{
				return null;
			}

			return bundle.CartItems.Sum(ci => ci.ListPrice * ci.ProductBundleItem.Quantity);
		}

		public static decimal? ListPriceExt(this CartBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.CartItems == null || bundle.CartItems.Count == 0)
			{
				return null;
			}

			return bundle.CartItems.Sum(ci => ci.ListPriceExt);
		}

		public static decimal? UnitPrice(this CartBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.CartItems == null || bundle.CartItems.Count == 0)
			{
				return null;
			}

			return bundle.CartItems.Sum(ci => ci.UnitPrice * ci.ProductBundleItem.Quantity);
		}

		public static decimal? UnitPriceExt(this CartBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.CartItems == null || bundle.CartItems.Count == 0)
			{
				return null;
			}

			return bundle.CartItems.Sum(ci => ci.UnitPriceExt);
		}

		public static decimal? LineDiscount(this CartBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.CartItems == null || bundle.CartItems.Count == 0)
			{
				return null;
			}

			return bundle.CartItems.Sum(ci => ci.LineDiscount);
		}

		public static decimal? LineTotal(this CartBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.CartItems == null || bundle.CartItems.Count == 0)
			{
				return null;
			}

			return bundle.CartItems.Sum(ci => ci.LineTotal);
		}

		
	}
}