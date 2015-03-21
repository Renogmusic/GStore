using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Exceptions;
using GStoreData.Models;
using GStoreData.PayPal;
using GStoreData.PayPal.Models;
using GStoreData.ViewModels;

namespace GStoreWeb.Controllers
{
	public class CheckoutController : AreaBaseController.RootAreaBaseController
    {
		public ActionResult Index()
		{
			
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (cart.StatusStartedCheckout)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);
			cart.StatusStartedCheckout = true;
			cart = GStoreDb.Carts.Update(cart);
			GStoreDb.SaveChanges();
			
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_Started, "", true, cartId: cart.CartId);

			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null)
			{
				return RedirectToAction("DeliveryInfo");
			}
			return RedirectToAction("LogInOrGuest");
		}

		[HttpGet]
		public ActionResult LogInOrGuest(bool? ContinueAsGuest, bool? ContinueAsLogin)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}

			cart = cart.ValidateCartAndSave(this);

			if (ContinueAsGuest ?? false)
			{
				if (!cart.StatusSelectedLogInOrGuest)
				{
					cart.StatusSelectedLogInOrGuest = true;
					GStoreDb.Carts.Update(cart);
					GStoreDb.SaveChanges();

					GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_SelectedLogInOrGuest, "", true, cartId: cart.CartId);
					return RedirectToAction("DeliveryInfo");
				}
			}

			UserProfile profile = CurrentUserProfileOrNull;
			if ((ContinueAsLogin ?? false) && (profile != null))
			{
				cart.StatusSelectedLogInOrGuest = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();

				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_SelectedLogInOrGuest, "", true, cartId: cart.CartId);
				return RedirectToAction("DeliveryInfo");
			}

			if (profile != null)
			{
				cart.StatusSelectedLogInOrGuest = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();

				return RedirectToAction("DeliveryInfo");
			}

			CheckoutLogInOrGuestViewModel viewModel = new CheckoutLogInOrGuestViewModel(config, cart, RouteData.Action());
			return View("LogInOrGuest", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogInOrGuest(CheckoutLogInOrGuestViewModel viewModel)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}

			cart = cart.ValidateCartAndSave(this);

			if (config.CheckoutLogInOrGuestWebForm != null)
			{
				FormProcessorExtensions.ValidateFields(this, config.CheckoutLogInOrGuestWebForm);
			}

			if (ModelState.IsValid)
			{
				WebFormResponse webFormResponse = cart.LoginOrGuestProcessWebForm(this);
				if (webFormResponse != null)
				{
					cart.LoginOrGuestWebFormResponseId = webFormResponse.WebFormResponseId;
				}
				cart.StatusSelectedLogInOrGuest = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_SelectedLogInOrGuest, "", true, cartId: cart.CartId);
				return RedirectToAction("DeliveryInfo");
			}

			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("LogInOrGuest", viewModel);
		}

		[HttpGet]
		public ActionResult DeliveryInfo()
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (cart.AllItemsAreDigitalDownload)
			{
				CheckoutDeliveryInfoDigitalOnlyViewModel viewModelDigitalOnly = new CheckoutDeliveryInfoDigitalOnlyViewModel(config, cart, RouteData.Action());
				return View("DeliveryInfoDigitalOnly", viewModelDigitalOnly);
			}
			else
			{
				CheckoutDeliveryInfoShippingViewModel viewModelShipping = new CheckoutDeliveryInfoShippingViewModel(config, cart, RouteData.Action());
				return View("DeliveryInfoShipping", viewModelShipping);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeliveryInfoDigitalOnly(CheckoutDeliveryInfoDigitalOnlyViewModel viewModel)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.AllItemsAreDigitalDownload)
			{
				return RedirectToAction("DeliveryInfo");
			}

			//check if custom form is valid
			if (config.CheckoutDeliveryInfoDigitalOnlyWebForm != null)
			{
				FormProcessorExtensions.ValidateFields(this, config.CheckoutDeliveryInfoDigitalOnlyWebForm);
			}

			if (ModelState.IsValid)
			{
				WebFormResponse webFormResponse = cart.DeliveryInfoDigitalOnlyProcessWebForm(this);

				DeliveryInfoDigital info = null;
				if (cart.DeliveryInfoDigital == null)
				{
					info = GStoreDb.DeliveryInfoDigitals.Create();
					info.SetDefaults(CurrentUserProfileOrNull);
					info.Client = CurrentClientOrThrow;
					info.ClientId = info.Client.ClientId;
					info.StoreFront = CurrentStoreFrontOrThrow;
					info.StoreFrontId = info.StoreFront.StoreFrontId;
					info.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					info.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					info.Cart = cart;
					info.CartId = cart.CartId;

					info.EmailAddress = viewModel.EmailAddress;
					info.FullName = viewModel.FullName;
					if (webFormResponse != null)
					{
						info.WebFormResponseId = webFormResponse.WebFormResponseId;
					}

					info = GStoreDb.DeliveryInfoDigitals.Add(info);
				}
				else
				{
					info = cart.DeliveryInfoDigital;

					info.EmailAddress = viewModel.EmailAddress;
					info.FullName = viewModel.FullName;

					if (webFormResponse != null)
					{
						info.WebFormResponseId = webFormResponse.WebFormResponseId;
					}
					info = GStoreDb.DeliveryInfoDigitals.Update(info);
				}

				cart.DeliveryInfoDigital = info;
				cart.Email = viewModel.EmailAddress;
				cart.FullName = viewModel.FullName;
				cart.StatusCompletedDeliveryInfo = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();

				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_CompletedDeliveryInfo, "", true, cartId: cart.CartId);

				return RedirectToAction("DeliveryMethod");
			}
			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("DeliveryInfoDigitalOnly", viewModel);
		}

		[HttpGet]
		public ActionResult DeliveryInfoDigitalOnly()
		{
			return RedirectToAction("DeliveryInfo");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeliveryInfoShipping(CheckoutDeliveryInfoShippingViewModel viewModel)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (cart.AllItemsAreDigitalDownload)
			{
				return RedirectToAction("DeliveryInfo");
			}

			//check if custom form is valid
			if (config.CheckoutDeliveryInfoShippingWebForm != null)
			{
				FormProcessorExtensions.ValidateFields(this, config.CheckoutDeliveryInfoShippingWebForm);
			}

			if (ModelState.IsValid)
			{
				WebFormResponse webFormResponse = cart.DeliveryInfoShippingProcessWebForm(this);

				DeliveryInfoShipping info = null;
				if (cart.DeliveryInfoShipping == null)
				{
					info = GStoreDb.DeliveryInfoShippings.Create();
					info.SetDefaults(CurrentUserProfileOrNull);
					info.Client = CurrentClientOrThrow;
					info.ClientId = info.Client.ClientId;
					info.StoreFront = CurrentStoreFrontOrThrow;
					info.StoreFrontId = info.StoreFront.StoreFrontId;
					info.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					info.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					info.Cart = cart;
					info.CartId = cart.CartId;

					info.AdddressL1 = viewModel.AdddressL1;
					info.AdddressL2 = viewModel.AdddressL2;
					info.EmailAddress = viewModel.EmailAddress;
					info.FullName = viewModel.FullName;
					info.City = viewModel.City;
					info.State = viewModel.State;
					info.PostalCode = viewModel.PostalCode;
					info.CountryCode = viewModel.CountryCode.Value;
					if (webFormResponse != null)
					{
						info.WebFormResponseId = webFormResponse.WebFormResponseId;
					}
					info = GStoreDb.DeliveryInfoShippings.Add(info);
				}
				else
				{
					info = cart.DeliveryInfoShipping;
					info.AdddressL1 = viewModel.AdddressL1;
					info.AdddressL2 = viewModel.AdddressL2;
					info.EmailAddress = viewModel.EmailAddress;
					info.FullName = viewModel.FullName;
					info.City = viewModel.City;
					info.State = viewModel.State;
					info.PostalCode = viewModel.PostalCode;
					info.CountryCode = viewModel.CountryCode.Value;
					if (webFormResponse != null)
					{
						info.WebFormResponseId = webFormResponse.WebFormResponseId;
					}
					info = GStoreDb.DeliveryInfoShippings.Update(info);
				}

				cart.DeliveryInfoShipping = info;
				cart.Email = viewModel.EmailAddress;
				cart.FullName = viewModel.FullName;
				cart.StatusCompletedDeliveryInfo = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();

				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_CompletedDeliveryInfo, "", true, cartId: cart.CartId);

				return RedirectToAction("DeliveryMethod");
			}

			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("DeliveryInfoShipping", viewModel);
		}

		[HttpGet]
		public ActionResult DeliveryInfoShipping()
		{
			return RedirectToAction("DeliveryInfo");
		}

		[HttpGet]
		public ActionResult DeliveryMethod()
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (cart.AllItemsAreDigitalDownload)
			{
				if (!cart.StatusSelectedDeliveryMethod)
				{
					cart.StatusSelectedDeliveryMethod = true;
					GStoreDb.Carts.Update(cart);
					GStoreDb.SaveChanges();

					GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_SelectedDeliveryMethod, "All Digital", true, cartId: cart.CartId);
				}
				return RedirectToAction("PaymentInfo");
			}

			CheckoutDeliveryMethodShippingViewModel viewModel = new CheckoutDeliveryMethodShippingViewModel(config, cart, RouteData.Action());
			return View("DeliveryMethod", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeliveryMethod(CheckoutDeliveryMethodShippingViewModel viewModel)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}

			if (config.CheckoutDeliveryMethodWebForm != null)
			{
				FormProcessorExtensions.ValidateFields(this, config.CheckoutDeliveryMethodWebForm);
			}

			if (ModelState.IsValid)
			{
				WebFormResponse webFormResponse = cart.DeliveryMethodProcessWebForm(this);
				if (webFormResponse != null)
				{
					cart.DeliveryInfoShipping.DeliveryMethodWebFormResponseId = webFormResponse.WebFormResponseId;
				}
				cart.DeliveryInfoShipping.ShippingDeliveryMethod = viewModel.ShippingDeliveryMethod;
				cart.DeliveryInfoShipping.ShippingDeliveryMethod = viewModel.ShippingDeliveryMethod;
				cart.StatusSelectedDeliveryMethod = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();

				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_SelectedDeliveryMethod, "", true, cartId: cart.CartId);

				return RedirectToAction("PaymentInfo");
			}
			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("DeliveryMethod", viewModel);
		}

		[HttpGet]
		public ActionResult PaymentInfo()
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (!cart.StatusSelectedDeliveryMethod)
			{
				return RedirectToAction("DeliveryMethod");
			}

			CheckoutPaymentInfoViewModel viewModel = new CheckoutPaymentInfoViewModel(config, cart, RouteData.Action());
			return View("PaymentInfo", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult PaymentInfo(CheckoutPaymentInfoViewModel viewModel)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (!cart.StatusSelectedDeliveryMethod)
			{
				return RedirectToAction("DeliveryMethod");
			}

			if (config.CheckoutPaymentInfoWebForm != null)
			{
				FormProcessorExtensions.ValidateFields(this, config.CheckoutPaymentInfoWebForm);
			}

			if (ModelState.IsValid)
			{
				WebFormResponse webFormResponse = cart.PaymentInfoProcessWebForm(this);
				if (config.PaymentMethod_PayPal_Enabled)
				{
					return Payment_PayPalStartPayment(viewModel, webFormResponse);
				}


				//payment with pay after order/no automated processing
				CartPaymentInfo cartPaymentInfo = null;
				if (cart.CartPaymentInfo == null)
				{
					cartPaymentInfo = GStoreDb.CartPaymentInfos.Create();
					cartPaymentInfo.SetDefaults(CurrentUserProfileOrNull);
					cartPaymentInfo.Client = CurrentClientOrThrow;
					cartPaymentInfo.ClientId = cartPaymentInfo.Client.ClientId;
					cartPaymentInfo.StoreFront = CurrentStoreFrontOrThrow;
					cartPaymentInfo.StoreFrontId = cartPaymentInfo.StoreFront.StoreFrontId;
					cartPaymentInfo.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					cartPaymentInfo.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					cartPaymentInfo.Cart = cart;
					cartPaymentInfo.CartId = cart.CartId;
					if (webFormResponse != null)
					{
						cartPaymentInfo.WebFormResponseId = webFormResponse.WebFormResponseId;
					}

					cartPaymentInfo = GStoreDb.CartPaymentInfos.Add(cartPaymentInfo);
				}
				else
				{
					cartPaymentInfo = cart.CartPaymentInfo;
					if (webFormResponse != null)
					{
						cartPaymentInfo.WebFormResponseId = webFormResponse.WebFormResponseId;
					}
					cartPaymentInfo = GStoreDb.CartPaymentInfos.Update(cartPaymentInfo);
				}

				//add/remove/etc
				cart.StatusPaymentInfoConfirmed = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();

				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Checkout, UserActionActionEnum.Checkout_ConfirmedPaymentInfo, "", true, cartId: cart.CartId);

				return RedirectToAction("ConfirmOrder");
			}

			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("PaymentInfo", viewModel);
		}

		public ActionResult PayPalCanceled(string token)
		{
			StoreFrontConfiguration storeFrontConfig = CurrentStoreFrontConfigOrThrow;
			Cart cart = storeFrontConfig.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);

			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (!cart.StatusSelectedDeliveryMethod)
			{
				return RedirectToAction("DeliveryMethod");
			}

			if (cart.CartPaymentInfoId.HasValue)
			{
				GStoreDb.CartPaymentInfos.DeleteById(cart.CartPaymentInfoId.Value);
				cart.CartPaymentInfo = null;
				cart.CartPaymentInfoId = null;
			}
			cart.StatusPaymentInfoConfirmed = false;
			cart = GStoreDb.Carts.Update(cart);
			GStoreDb.SaveChanges();

			AddUserMessage("PayPal payment cancelled", "You have cancelled your PayPal payment. Please select another payment option or try PayPal again.", UserMessageType.Info);
			return RedirectToAction("PaymentInfo");
		}

		public ActionResult PayPalAccountConfirmed(string paymentId, string token, string PayerId)
		{
			StoreFrontConfiguration storeFrontConfig = CurrentStoreFrontConfigOrThrow;
			Cart cart = storeFrontConfig.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);

			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (!cart.StatusSelectedDeliveryMethod)
			{
				return RedirectToAction("DeliveryMethod");
			}
			if (!cart.CartPaymentInfoId.HasValue)
			{
				throw new ApplicationException("Cart.PaymentInfo is null");
			}
			if (string.IsNullOrEmpty(cart.CartPaymentInfo.PayPalPaymentId))
			{
				throw new ArgumentNullException("cart.CartPaymentInfo.PayPalPaymentId", "cart.CartPaymentInfo.PayPalPaymentId = null. Be sure paypalpaymentid is being set before going to PayPal.");
			}
			if (string.IsNullOrWhiteSpace(paymentId))
			{
				return HttpBadRequest("paymentId cannot be null");
			}
			if (string.IsNullOrWhiteSpace(token))
			{
				return HttpBadRequest("token cannot be null");
			}
			if (string.IsNullOrWhiteSpace(PayerId))
			{
				return HttpBadRequest("PayerId cannot be null");
			}
			if (cart.CartPaymentInfo.PayPalPaymentId.ToLower() != paymentId.ToLower())
			{
				return HttpBadRequest("PayPal Payment id mismatch");
			}

			cart.CartPaymentInfo.PayPalReturnPaymentId = paymentId;
			cart.CartPaymentInfo.PayPalReturnPayerId = PayerId;
			cart.CartPaymentInfo.PayPalReturnToken = token;

			cart.StatusPaymentInfoConfirmed = true;
			cart = GStoreDb.Carts.Update(cart);
			GStoreDb.SaveChanges();

			return RedirectToAction("ConfirmOrder");
		}

		/// <summary>
		/// Starts a PayPal payment and returns a redirect result to PayPal (or payment info page if an error occurs)
		/// </summary>
		/// <param name="viewModel"></param>
		/// <returns></returns>
		protected ActionResult Payment_PayPalStartPayment(CheckoutPaymentInfoViewModel viewModel, WebFormResponse webFormResponse)
		{
			StoreFrontConfiguration storeFrontConfig = CurrentStoreFrontConfigOrThrow;
			Cart cart = storeFrontConfig.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);

			Uri returnUri = new Uri(Url.Action("PayPalAccountConfirmed", null, null, Request.Url.Scheme));
			Uri cancelUri = new Uri(Url.Action("PayPalCanceled", null, null, Request.Url.Scheme));

			PayPalPaymentClient paypalClient = new PayPalPaymentClient();

			PayPalPaymentData response;
			try
			{
				response = paypalClient.StartPayPalPayment(storeFrontConfig, cart, returnUri, cancelUri);
			}
			catch(PayPalExceptionOAuthFailed exOAuth)
			{
				string message = "Sorry, this store's configuration for PayPal OAuth is not operational. Please contact us for other payment options."
					+ (exOAuth.IsSandbox ? "\nError in Sandbox Config." : "\nError in Live Config");
				AddUserMessage("PayPal Error", message, UserMessageType.Danger);

				if (CurrentUserProfileOrNull != null && CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					string adminMessage = exOAuth.ToString()
						+ "\n\nHTTP Response:\n" + exOAuth.ResponseString
						+ "\n\nHTTP Headers:\n" + exOAuth.ResponseHeaders;
					AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
				}

				return RedirectToAction("PaymentInfo");
			}
			catch(PayPalExceptionCreatePaymentFailed exPaymentFailed)
			{
				string message = "Sorry, there was an error sending your order to PayPal for payment. Please contact us for other payment options."
					+ (exPaymentFailed.IsSandbox ? "\nError in Sandbox." : "\nError in Live Site.");

				AddUserMessage("PayPal Error", message, UserMessageType.Danger);

				if (CurrentUserProfileOrNull != null && CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					string adminMessage = exPaymentFailed.ToString()
						+ "\n\nHTTP Response:\n" + exPaymentFailed.ResponseString
						+ "\n\nHTTP Headers:\n" + exPaymentFailed.ResponseHeaders;
					AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
				}

				return RedirectToAction("PaymentInfo");
			}
			catch (Exception ex)
			{
				string message = "Sorry, there was an error starting starting your order with PayPal. Please contact us for other payment options.";
				AddUserMessage("PayPal Error", message, UserMessageType.Danger);

				if (CurrentUserProfileOrNull != null && CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					string adminMessage = "Exception: " + ex.ToString();
					AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
				}
				return RedirectToAction("PaymentInfo");
			}

			CartPaymentInfo cartPaymentInfo = cart.CartPaymentInfo;
			if (cartPaymentInfo == null)
			{
				cartPaymentInfo = GStoreDb.CartPaymentInfos.Create();
				cartPaymentInfo.SetFromPayPalResponse(cart, response);
				if (webFormResponse != null)
				{
					cartPaymentInfo.WebFormResponseId = webFormResponse.WebFormResponseId;
				}
				cartPaymentInfo = GStoreDb.CartPaymentInfos.Add(cartPaymentInfo);
			}
			else
			{
				cartPaymentInfo.SetFromPayPalResponse(cart, response);
				if (webFormResponse != null)
				{
					cartPaymentInfo.WebFormResponseId = webFormResponse.WebFormResponseId;
				}
				cartPaymentInfo = GStoreDb.CartPaymentInfos.Update(cartPaymentInfo);
			}

			GStoreDb.SaveChanges();

			cart.CartPaymentInfoId = cartPaymentInfo.CartPaymentInfoId;
			cart.StatusPaymentInfoConfirmed = false;
			cart = GStoreDb.Carts.Update(cart);
			GStoreDb.SaveChanges();

			PayPalLinkData confirmLink = response.links.Where(l => l.rel == "approval_url").SingleOrDefault();
			if (string.IsNullOrEmpty(confirmLink.href))
			{
				string message = "Sorry, there was an error getting your order info from PayPal. Please contact us for other payment options.";
				AddUserMessage("PayPal Error", message, UserMessageType.Danger);
				if (CurrentUserProfileOrNull != null && CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					string adminMessage = "PayPal Response parse error. Cannot find link with method: approval_url";
					AddUserMessage("PayPal Error (admin info)", "Error " + adminMessage, UserMessageType.Danger);
				}
				return RedirectToAction("PaymentInfo");
			}

			return Redirect(confirmLink.href);
		}

		[HttpGet]
		public ActionResult ConfirmOrder()
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (!cart.StatusSelectedDeliveryMethod)
			{
				return RedirectToAction("DeliveryMethod");
			}
			if (!cart.StatusPaymentInfoConfirmed)
			{
				return RedirectToAction("PaymentInfo");
			}

			CheckoutConfirmOrderViewModel viewModel = new CheckoutConfirmOrderViewModel(config, cart, RouteData.Action());
			return View("ConfirmOrder", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ConfirmOrder(CheckoutConfirmOrderViewModel viewModel)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}

			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart = cart.ValidateCartAndSave(this);

			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (!cart.StatusSelectedDeliveryMethod)
			{
				return RedirectToAction("DeliveryMethod");
			}
			if (!cart.StatusPaymentInfoConfirmed)
			{
				return RedirectToAction("PaymentInfo");
			}

			if (config.CheckoutConfirmOrderWebForm != null)
			{
				FormProcessorExtensions.ValidateFields(this, config.CheckoutConfirmOrderWebForm);
			}

			if (ModelState.IsValid)
			{
				WebFormResponse webFormResponse = cart.ConfirmOrderProcessWebForm(this);
				if (webFormResponse != null)
				{
					cart.ConfirmOrderWebFormResponseId = webFormResponse.WebFormResponseId;
					GStoreDb.Carts.Update(cart);
					GStoreDb.SaveChanges();
				}

				return cart.ProcessOrderAndPayment(this);
			}

			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("ConfirmOrder", viewModel);
		}

		[GStoreData.Identity.AuthorizeGStoreAction(GStoreData.Identity.GStoreAction.Checkout_SendReceiptCopy)]
		public ActionResult SendReceiptCopy(string id, string email, string name)
		{
			if (string.IsNullOrEmpty(id))
			{
				return HttpNotFound("Order id is null");
			}

			Order order = CurrentStoreFrontOrThrow.Orders.SingleOrDefault(o => o.OrderNumber == id);
			if (order == null)
			{
				return HttpNotFound("Order not found for Order Id " + id);
			}

			string userMessage;
			bool result = order.SendOrderReceipt(this, out userMessage, "_OrderEmailHtmlPartial", "_OrderEmailTextPartial", email, name);

			return View("Message", model: userMessage);
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.CheckoutTheme.FolderName;
			}
		}

	}
}
