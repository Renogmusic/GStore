using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Data;
using GStore.Models.ViewModels;
using GStore.AppHtmlHelpers;
using GStore.Models;
using GStore.Identity;

namespace GStore.Controllers
{
    public class CartController : BaseClass.BaseController
	{
		#region Public Cart Actions

		public ActionResult Index()
		{
			if (!CurrentStoreFrontConfigOrThrow.UseShoppingCart)
			{
				RedirectToAction("Index", "Checkout");
			}

			if (!CheckAccess())
			{
				return BounceToLogin();
			}

			Cart cart = CurrentStoreFrontOrThrow.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			CartConfigViewModel cartConfig = CurrentStoreFrontConfigOrThrow.CartConfigViewModel(false, false);

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_View, "", true, (cart == null ? (int?)null : cart.CartId));

			ViewData.Add("CartConfig", cartConfig);
			return View("Index", cart);
		}

		public ActionResult Add(string id, int? qty, string type, bool? Login)
		{
			//remove old item and add new item
			if (!CheckAccess())
			{
				return BounceToLogin();
			}

			int quantity = 1;
			if (qty.HasValue && qty.Value > 0 && qty.Value < 10000)
			{
				quantity = qty.Value;
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Cart cart = storeFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);

			if (string.IsNullOrWhiteSpace(id))
			{
				AddUserMessage("Add to Cart Error", "Item not found. Please try again.", UserMessageType.Danger);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_AddToCartFailure, "Bad Url", false, cartId: (cart == null ? (int?)null : cart.CartId), productUrlName: id);
				return RedirectToAction("Index");
			}

			Product product = storeFront.Products.AsQueryable().CanAddToCart(storeFront).SingleOrDefault(p => p.UrlName.ToLower() == id.ToLower());
			if (product == null)
			{
				AddUserMessage("Add to Cart Error", "Item '" + id.ToHtml() + "' could not be found to add to your cart. Please try again.", UserMessageType.Danger);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_AddToCartFailure, "Item Not Found", false, cartId: (cart == null ? (int?)null : cart.CartId), productUrlName: id);
				return RedirectToPreviousPageOrCartIndex();
			}

			//if item with same variant is already added, increment the quantity
			if (!CurrentStoreFrontConfigOrThrow.UseShoppingCart)
			{
				if (cart != null && cart.CartItems.Count > 0)
				{
					//if storefront is not set to use a cart, dump previous items and start with a new cart.
					CurrentStoreFrontOrThrow.DumpCartAndSave(GStoreDb, cart);
					cart = storeFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
				}
			}

			CartItem cartItemExisting = cart.FindItemInCart(product, type);
			if (cartItemExisting != null)
			{
				int newQty = cartItemExisting.Quantity + quantity;
				cartItemExisting = cartItemExisting.UpdateQuantityAndSave(GStoreDb, newQty, this);

				if (newQty <= cartItemExisting.Product.MaxQuantityPerOrder)
				{
					AddUserMessage("Item Added to Cart", "'" + cartItemExisting.Product.Name.ToHtml() + "' was added to your cart. Now you have " + cartItemExisting.Quantity + " of them in your cart.<br/><a href=" + Url.Action("Index", "Cart") + ">Click here to view your cart.</a>", UserMessageType.Success);
					cart.CancelCheckout(GStoreDb);
				}
				else
				{
					//quantity is over max, user messages are already set
				}
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_AddToCartSuccess, "Added to Existing", true, cartId: cart.CartId, productUrlName: id);

				return RedirectToPreviousPageOrCartIndex();
			}
			CartItem cartItem = cart.AddToCart(product, quantity, type, this);

			AddUserMessage("Item Added to Cart", "'" + product.Name.ToHtml() + "' is now in your shopping cart.<br/><a href=" + Url.Action("Index", "Cart") + ">Click here to view your cart.</a>", UserMessageType.Success);

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_AddToCartSuccess, "Added", true, cartId: cartItem.CartId, productUrlName: id);

			cart.CancelCheckout(GStoreDb);

			return RedirectToPreviousPageOrCartIndex();
		}

		public ActionResult Remove(string id, string type)
		{
			if (!CheckAccess())
			{
				return BounceToLogin();
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Cart cart = storeFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);

			if (string.IsNullOrWhiteSpace(id))
			{
				AddUserMessage("Remove from Cart Error", "Item not found. Please try again.", UserMessageType.Danger);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_RemoveFromCart, "Bad Url", false, cartId: (cart == null ? (int?)null : cart.CartId), productUrlName: id);
				return RedirectToAction("Index");
			}

			Product product = storeFront.Products.AsQueryable().CanAddToCart(storeFront).SingleOrDefault(p => p.UrlName.ToLower() == id.ToLower());
			if (product == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_RemoveFromCart, "Product not found in catalog", false, cartId: (cart == null ? (int?)null : cart.CartId), productUrlName: id);
				AddUserMessage("Remove From Cart Error", "Item '" + id.ToHtml() + "' could not be found. Please try again.", UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			CartItem cartItemExisting = cart.FindItemInCart(product, type);
			if (cartItemExisting == null)
			{
				AddUserMessage("Item Not Found in Cart", "'" + id.ToHtml() + "' was already removed from your cart.", UserMessageType.Success);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_RemoveFromCart, "Item not found in cart.", false, cartId: (cart == null ? (int?)null : cart.CartId), productUrlName: id);
				return RedirectToAction("Index");
			}

			bool result = cartItemExisting.RemoveFromCart(GStoreDb);
			cart.CancelCheckout(GStoreDb);

			AddUserMessage("Item Removed from Cart", "'" + product.Name.ToHtml() + "' was removed from your shopping cart.", UserMessageType.Success);
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_RemoveFromCart, "Success", true, cartId: (cart == null ? (int?)null : cart.CartId), productUrlName: id);

			return RedirectToPreviousPageOrCartIndex();
		}

		[HttpGet]
		public ActionResult UpdateDiscountCode()
		{
			//stub for URL hacks
			Cart cart = CurrentStoreFrontOrThrow.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_ApplyDiscountCodeFailure, "Bad Url", false, cartId: (cart == null ? (int?)null : cart.CartId));
			return RedirectToAction("Index");
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult UpdateDiscountCode(string discountCode)
		{
			if (!CheckAccess())
			{
				return BounceToLogin();
			}
			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Cart cart = CurrentStoreFrontOrThrow.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			bool success = false;

			cart = cart.UpdateDiscountCode(discountCode, this, out success);

			if (success)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_ApplyDiscountCodeSuccess, "", success, discountCode: discountCode, cartId: (cart == null ? (int?)null : cart.CartId));
			}
			else
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Cart, UserActionActionEnum.Cart_ApplyDiscountCodeFailure, "", success, discountCode: discountCode, cartId: (cart == null ? (int?)null : cart.CartId));
			}

			cart.CancelCheckout(GStoreDb);
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult UpdateQty()
		{
			//stub for URL hacks
			return RedirectToAction("Index");
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult UpdateQty(string id, int? quantity, string type)
		{
			if (!CheckAccess())
			{
				return BounceToLogin();
			}
			int newQuantity = 1;
			if (quantity.HasValue && quantity.Value > 0 && quantity.Value < 10000)
			{
				newQuantity = quantity.Value;
			}
			if (string.IsNullOrWhiteSpace(id))
			{
				AddUserMessage("Update Quantity Error", "Item '' not found. Please try again.", UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;

			Product product = storeFront.Products.AsQueryable().CanAddToCart(storeFront).SingleOrDefault(p => p.UrlName.ToLower() == id.ToLower());
			if (product == null)
			{
				AddUserMessage("Update Quantity Error", "Item '" + id.ToHtml() + "' is not addable to your cart. Please try again.", UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			Cart cart = CurrentStoreFrontOrThrow.GetCart(Session.SessionID, CurrentUserProfileOrNull);

			CartItem cartItemExisting = cart.FindItemInCart(product, type);
			if (cartItemExisting == null)
			{
				AddUserMessage("Update Quantity Error", "Item '" + id.ToHtml() + "' is not in your cart. Please try again.", UserMessageType.Success);
				return RedirectToAction("Index");
			}

			cartItemExisting.UpdateQuantityAndSave(GStoreDb, newQuantity, this);
			cart.CancelCheckout(GStoreDb);

			return RedirectToPreviousPageOrCartIndex();
		}

		#endregion

		#region Preview Actions

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Cart_Preview)]
		public ActionResult Preview(int? id, int? quantity, string discountCode)
		{
			StoreFrontConfiguration config = GetConfigAndAccessCheck(id);

			if (config == null)
			{
				return HttpBadRequest("No config found for store front config id [" + (id ?? 0) + "]");
			}

			if (!quantity.HasValue)
			{
				quantity = 1;
			}

			Cart cart = config.StoreFront.GetPreviewCart(Session.SessionID, quantity.Value, discountCode, CurrentUserProfileOrNull);
			CartConfigViewModel cartConfig = config.CartConfigViewModel(false, false);
			ViewData.Add("CartConfig", cartConfig);
			ViewData.Add("IsPreview", true);
			return View("Index", cart);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Cart_Preview)]
		public ActionResult PreviewUpdateDiscountCode(int? id, string discountCode, string originalDiscountCode, int? quantity)
		{
			StoreFrontConfiguration config = GetConfigAndAccessCheck(id);

			if (config == null)
			{
				return HttpBadRequest("No config found for store front config id [" + (id ?? 0) + "]");
			}

			if (!quantity.HasValue)
			{
				quantity = 1;
			}

			Cart cart = config.StoreFront.GetPreviewCart(Session.SessionID, quantity.Value, originalDiscountCode, CurrentUserProfileOrNull);
			cart = cart.PreviewUpdateDiscountCode(discountCode, config, this);
			CartConfigViewModel cartConfig = config.CartConfigViewModel(false, false);
			ViewData.Add("CartConfig", cartConfig);
			ViewData.Add("IsPreview", true);
			return View("Index", cart);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Cart_Preview)]
		public ActionResult PreviewUpdateQty(int? id, string discountCode, int? quantity)
		{
			StoreFrontConfiguration config = GetConfigAndAccessCheck(id);

			if (config == null)
			{
				return HttpBadRequest("No config found for store front config id [" + (id ?? 0) + "]");
			}

			if (!quantity.HasValue)
			{
				quantity = 1;
			}

			Cart cart = config.StoreFront.GetPreviewCart(Session.SessionID, quantity.Value, discountCode, CurrentUserProfileOrNull);
			CartConfigViewModel cartConfig = config.CartConfigViewModel(false, false);
			ViewData.Add("CartConfig", cartConfig);
			ViewData.Add("IsPreview", true);
			return View("Index", cart);
		}

		#endregion

		#region View/Edit Configuration Actions

		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_StoreFrontConfig_Cart_Edit, GStoreAction.ClientConfig_StoreFrontConfig_Cart_View)]
		public ActionResult ViewConfig(int? id)
		{
			StoreFrontConfiguration config = GetConfigAndAccessCheck(id);

			if (config == null)
			{
				return HttpBadRequest("No config found for store front config id [" + (id ?? 0) + "]");
			}
			Cart cart = config.StoreFront.GetPreviewCart(Session.SessionID, 1, null, CurrentUserProfileOrNull);
			CartConfigViewModel cartConfig = config.CartConfigViewModel(false, false);
			ViewData.Add("CartConfig", cartConfig);
			ViewData.Add("IsViewConfig", true);
			return View("Index", cart);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Cart_Edit)]
		public ActionResult EditConfigApplyDefault(int? id)
		{
			StoreFrontConfiguration config = GetConfigAndAccessCheck(id);

			if (config == null)
			{
				return HttpBadRequest("No config found for store front config id [" + (id ?? 0) + "]");
			}

			config.ApplyDefaultCartConfig();
			config = GStoreDb.StoreFrontConfigurations.Update(config);
			GStoreDb.SaveChanges();

			AddUserMessage("Cart Configuration updated.", "Default cart configuration was applied successfully to configuration '" + config.ConfigurationName.ToHtml() + "' [" + config.StoreFrontConfigurationId + "]", UserMessageType.Success);

			return RedirectToAction("ViewConfig", new { id = config.StoreFrontConfigurationId });
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Cart_Edit)]
		public ActionResult EditConfig(int? id)
		{
			StoreFrontConfiguration config = GetConfigAndAccessCheck(id);

			if (config == null)
			{
				return HttpBadRequest("No config found for store front config id [" + (id ?? 0) + "]");
			}
			Cart cart = config.StoreFront.GetPreviewCart(Session.SessionID, 1, null, CurrentUserProfileOrNull);
			CartConfigViewModel cartConfig = config.CartConfigViewModel(false, false);
			ViewData.Add("CartConfig", cartConfig);
			ViewData.Add("IsEditConfig", true);
			return View("Index", cart);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Cart_Edit)]
		public ActionResult EditConfig(int? id, CartConfigViewModel config)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("store front config id (ID) is null");
			}

			if (id.Value != config.StoreFrontConfigurationId)
			{
				return HttpBadRequest("store front configuration id mismatch. Url id: " + id.Value + " view model value: " + config.StoreFrontConfigurationId);
			}

			StoreFrontConfiguration configToEdit = GetConfigAndAccessCheck(id);

			if (config == null)
			{
				return HttpBadRequest("No config found for store front config id [" + (id ?? 0) + "]");
			}

			if (ModelState.IsValid)
			{
				configToEdit.CopyValuesFromCartConfigViewModel(config);
				GStoreDb.StoreFrontConfigurations.Update(configToEdit);
				GStoreDb.SaveChanges();

				AddUserMessage("Cart Configuration updated.", "Your changes have been saved for configuration '" + configToEdit.ConfigurationName.ToHtml() + "' [" + configToEdit.StoreFrontConfigurationId + "]", UserMessageType.Success);

				return RedirectToAction("Preview", new { id = configToEdit.StoreFrontConfigurationId });
			}

			AddUserMessage("Form Error.", "There was an error in your form values. Please correct the error below.", UserMessageType.Warning);

			Cart cart = configToEdit.StoreFront.GetPreviewCart(Session.SessionID, 1, null, CurrentUserProfileOrNull);
			CartConfigViewModel cartConfig = configToEdit.CartConfigViewModel(false, false);
			ViewData.Add("CartConfig", cartConfig);
			ViewData.Add("IsEditConfig", true);
			return View("Index", cart);
		}

		#endregion

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.CartTheme.FolderName;
			}
		}

		protected StoreFrontConfiguration GetConfigAndAccessCheck(int? id)
		{
			if (!id.HasValue)
			{
				return CurrentStoreFrontConfigOrAny;
			}
			else
			{
				StoreFront currentStoreFront = CurrentStoreFrontOrThrow;

				StoreFront configStoreFront = currentStoreFront.Client.StoreFronts.Where(sf => sf.StoreFrontConfigurations.Any(sfc => sfc.StoreFrontConfigurationId == id.Value)).FirstOrDefault();
				if (configStoreFront == null)
				{
					AddUserMessage("Configuration not found.", "Store Front Configuration id [" + id.Value + "] was not found for any store fronts for this client. Here is the default configuration for the current store front.", UserMessageType.Info);
					return currentStoreFront.CurrentConfigOrAny();
				}
				else
				{
					if (configStoreFront.StoreFrontId == currentStoreFront.StoreFrontId)
					{
						//current store front, user is authorized
						return configStoreFront.StoreFrontConfigurations.Single(sfc => sfc.StoreFrontConfigurationId == id.Value);
					}
					else
					{
						if (configStoreFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, true, GStoreAction.ClientConfig_StoreFrontConfig_Cart_View, GStoreAction.ClientConfig_StoreFrontConfig_Cart_Edit))
						{
							//authorized
							return configStoreFront.StoreFrontConfigurations.Single(sfc => sfc.StoreFrontConfigurationId == id.Value);
						}
						else
						{
							//access denied
							AddUserMessage("Access Denied.", "You do not have permission to view the cart for configuration '" + configStoreFront + "' [" + id.Value + "]. Here is the default configuration for the current store front.", UserMessageType.Info);
							return currentStoreFront.CurrentConfigOrAny();
						}
					}
				}
			}
		}

		protected bool CheckAccess()
		{
			if ((CurrentStoreFrontConfigOrThrow.CartRequireLogin) && (CurrentUserProfileOrNull == null))
			{
				return false;
			}
			return true;
		}
		protected ActionResult BounceToLogin()
		{
			AddUserMessage("Login required.", "You must log in to use your shopping cart.", UserMessageType.Info);
			return RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl });
		}

		protected ActionResult RedirectToPreviousPageOrCartIndex()
		{

			if (!CurrentStoreFrontConfigOrThrow.UseShoppingCart)
			{
				return RedirectToAction("Index", "Checkout");
			}

			if ((!string.IsNullOrEmpty(Request.QueryString["status"])) && (Request.QueryString["status"].ToLower() == "login"))
			{
				return RedirectToAction("Index");
			}

			if (Request.UrlReferrer != null)
			{
				if (Request.UrlReferrer.Host.ToLower() == Request.Url.Host.ToLower())
				{
					string url = Request.UrlReferrer.ToString();
					if (!url.Contains("status=Cart"))
					{
						if (url.Contains('?'))
						{
							url += "&status=Cart";
						}
						else
						{
							url += "?status=Cart";
						}
					}
					return Redirect(url);
				}
			}

			return RedirectToAction("Index");
		}
    }
}