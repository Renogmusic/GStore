using GStore.Models;
using GStore.Data;
using GStore.AppHtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Models.ViewModels;

namespace GStore.Controllers
{
	public class CheckoutController : BaseClass.BaseController
    {
		public ActionResult Index()
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (cart.StatusStartedCheckout)
			{
				return RedirectToAction("LogInOrGuest");
			}

			cart.StatusStartedCheckout = true;
			GStoreDb.Carts.Update(cart);
			GStoreDb.SaveChanges();

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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}

			if (ContinueAsGuest ?? false)
			{
				if (!cart.StatusSelectedLogInOrGuest)
				{
					cart.StatusSelectedLogInOrGuest = true;
					GStoreDb.Carts.Update(cart);
					GStoreDb.SaveChanges();
					return RedirectToAction("DeliveryInfo");
				}
			}

			UserProfile profile = CurrentUserProfileOrNull;
			if ((ContinueAsLogin ?? false) && (profile != null))
			{
				cart.StatusSelectedLogInOrGuest = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();
				return RedirectToAction("DeliveryInfo");
			}

			if (profile != null)
			{
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusStartedCheckout)
			{
				return RedirectToAction("Index");
			}
			if (ModelState.IsValid)
			{
				if (!cart.StatusSelectedLogInOrGuest)
				{
					cart.StatusSelectedLogInOrGuest = true;
					GStoreDb.Carts.Update(cart);
					GStoreDb.SaveChanges();
				}
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}

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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}
			if (!cart.AllItemsAreDigitalDownload)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (ModelState.IsValid)
			{
				Models.DeliveryInfoDigital info = null;
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

					info = GStoreDb.DeliveryInfoDigitals.Add(info);
				}
				else
				{
					info = cart.DeliveryInfoDigital;

					info.EmailAddress = viewModel.EmailAddress;
					
					info = GStoreDb.DeliveryInfoDigitals.Update(info);
				}

				cart.DeliveryInfoDigital = info;
				cart.Email = viewModel.EmailAddress;
				cart.StatusCompletedDeliveryInfo = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusSelectedLogInOrGuest)
			{
				return RedirectToAction("LogInOrGuest");
			}
			if (cart.AllItemsAreDigitalDownload)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (ModelState.IsValid)
			{
				Models.DeliveryInfoShipping info = null;
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
					info.FirstName = viewModel.FirstName;
					info.LastName = viewModel.LastName;
					info.City = viewModel.City;
					info.State = viewModel.State;
					info.ZIPCode = viewModel.ZIPCode;

					info = GStoreDb.DeliveryInfoShippings.Add(info);
				}
				else
				{
					info = cart.DeliveryInfoShipping;
					info.AdddressL1 = viewModel.AdddressL1;
					info.AdddressL2 = viewModel.AdddressL2;
					info.EmailAddress = viewModel.EmailAddress;
					info.FirstName = viewModel.FirstName;
					info.LastName = viewModel.LastName;
					info.City = viewModel.City;
					info.State = viewModel.State;
					info.ZIPCode = viewModel.ZIPCode;
					info = GStoreDb.DeliveryInfoShippings.Update(info);
				}

				cart.DeliveryInfoShipping = info;
				cart.Email = viewModel.EmailAddress;
				cart.StatusCompletedDeliveryInfo = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusCompletedDeliveryInfo)
			{
				return RedirectToAction("DeliveryInfo");
			}
			if (ModelState.IsValid)
			{
				cart.DeliveryInfoShipping.ShippingDeliveryMethod = viewModel.ShippingDeliveryMethod;
				cart.StatusSelectedDeliveryMethod = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusSelectedDeliveryMethod)
			{
				return RedirectToAction("DeliveryMethod");
			}

			if (ModelState.IsValid)
			{
				Models.Payment payment = null;
				if (cart.Payment == null)
				{
					payment = GStoreDb.Payments.Create();
					payment.SetDefaults(CurrentUserProfileOrNull);
					payment.Client = CurrentClientOrThrow;
					payment.ClientId = payment.Client.ClientId;
					payment.StoreFront = CurrentStoreFrontOrThrow;
					payment.StoreFrontId = payment.StoreFront.StoreFrontId;
					payment.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					payment.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					payment.Cart = cart;
					payment.CartId = cart.CartId;

					payment.IsProcessed = false;
					payment.TransactionId = null;

					payment = GStoreDb.Payments.Add(payment);
				}
				else
				{
					payment = cart.Payment;

					payment.IsProcessed = false;
					payment.TransactionId = null;
					
					payment = GStoreDb.Payments.Update(payment);
				}

				cart.Payment = payment;
				cart.StatusEnteredPaymentInfo = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();
				return RedirectToAction("ConfirmOrder");
			}

			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("PaymentInfo", viewModel);
		}

		[HttpGet]
		public ActionResult ConfirmOrder()
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			Cart cart = config.StoreFront.GetCart(Session.SessionID, CurrentUserProfileOrNull);
			if (cart == null || cart.CartItems.Count == 0)
			{
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusEnteredPaymentInfo)
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
				AddUserMessage("Nothing to check out.", "Your cart is empty.", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index", "Cart");
			}
			if (!cart.StatusEnteredPaymentInfo)
			{
				return RedirectToAction("PaymentInfo");
			}

			if (ModelState.IsValid)
			{
				Order order = cart.CreateOrderFromCart(GStoreDb);
				cart.Order = order;
				cart.OrderId = order.OrderId;
				cart.StatusPlacedOrder = true;
				GStoreDb.Carts.Update(cart);
				GStoreDb.SaveChanges();

				AddUserMessage("notifications", "sending notification to store and user email", UserMessageType.Success);
				return RedirectToAction("View", "OrderStatus", new { id = order.OrderNumber, Email = order.Email });
			}

			viewModel.UpdateForRepost(config, cart, RouteData.Action());
			return View("PaymentInfo", viewModel);
		}

		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.CheckoutLayoutName;
			}
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
