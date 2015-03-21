using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.StoreAdmin.ViewModels;
using GStoreData.Identity;
using GStoreData.Models;

namespace GStoreWeb.Areas.StoreAdmin.Controllers
{
	public class ClientConfigAdminController : AreaBaseController.StoreAdminAreaBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Manager)]
		public ActionResult Manager()
		{
			ClientConfigManagerAdminViewModel viewModel = new ClientConfigManagerAdminViewModel(CurrentClientOrThrow, CurrentStoreFrontConfigOrAny, CurrentUserProfileOrThrow);
			return View("Manager", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_Edit, GStoreAction.ClientConfig_View)]
		public ActionResult ClientView(string Tab)
		{
			ClientConfigAdminViewModel viewModel = new ClientConfigAdminViewModel(CurrentClientOrThrow, CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow, Tab);
			return View("ClientView", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_Edit, GStoreAction.ClientConfig_View)]
		public ActionResult ClientViewNoTabs()
		{
			ClientConfigAdminViewModel viewModel = new ClientConfigAdminViewModel(CurrentClientOrThrow, CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow, null);
			return View("ClientViewNoTabs", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Edit)]
		public ActionResult ClientEdit(string Tab)
		{
			ClientConfigAdminViewModel viewModel = new ClientConfigAdminViewModel(CurrentClientOrThrow, CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow, Tab);
			return View("ClientEdit", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Edit)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult ClientEdit(ClientConfigAdminViewModel model)
		{
			if (ModelState.IsValid)
			{
				Client client = CurrentClientOrThrow;
				client.EnableNewUserRegisteredBroadcast = model.EnableNewUserRegisteredBroadcast;
				client.EnablePageViewLog = model.EnablePageViewLog;
				client.Name = model.Name;
				client.TimeZoneId = model.TimeZoneId;
				client.SendGridMailAccount = model.SendGridMailAccount;
				client.SendGridMailFromEmail = model.SendGridMailFromEmail;
				client.SendGridMailFromName = model.SendGridMailFromName;
				client.SendGridMailPassword = model.SendGridMailPassword;
				client.TwilioFromPhone = model.TwilioFromPhone;
				client.TwilioSid = model.TwilioSid;
				client.TwilioSmsFromEmail = model.TwilioSmsFromEmail;
				client.TwilioSmsFromName = model.TwilioSmsFromName;
				client.TwilioToken = model.TwilioToken;
				client.UseSendGridEmail = model.UseSendGridEmail;
				client.UseTwilioSms = model.UseTwilioSms;
				GStoreDb.Clients.Update(client);
				GStoreDb.SaveChanges();
				client.CreateClientFolders(Request.ApplicationPath, Server);
				return RedirectToAction("ClientView", new { Tab = model.ActiveTab });
			}

			return View("ClientEdit", model);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_StoreFrontConfig_Edit, GStoreAction.ClientConfig_StoreFrontConfig_View)]
		public ActionResult StoreFrontView(int? id, int? storeFrontConfigId, string Tab)
		{
			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToView = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				storeFrontToView = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToView == null)
				{
					throw new ApplicationException("StoreFrontToView cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!storeFrontToView.Authorization_IsAuthorized(CurrentUserProfileOrThrow, true, GStoreAction.ClientConfig_StoreFrontConfig_View, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to view configuration for store front: " + storeFrontToView.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToView.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToView = CurrentStoreFrontOrThrow;
			}

			StoreFrontConfiguration storeFrontConfig = null;
			if (storeFrontConfigId.HasValue)
			{
				storeFrontConfig = storeFrontToView.StoreFrontConfigurations.FirstOrDefault(c => c.StoreFrontConfigurationId == storeFrontConfigId.Value);
				if (storeFrontConfig == null)
				{
					AddUserMessage("Configuration not found", "Configuration id [" + storeFrontConfigId.Value  +"] was not found, here is the current store front configuration instead.", UserMessageType.Warning);
					storeFrontConfig = storeFrontToView.CurrentConfigOrAny();
				}
			}
			else
			{
				//show current config if no config id passed in
				storeFrontConfig = storeFrontToView.CurrentConfigOrAny();
			}
			StoreFrontConfigAdminViewModel viewModel = new StoreFrontConfigAdminViewModel(storeFrontConfig, CurrentUserProfileOrThrow, Tab, false, false);

			return View("StoreFrontView", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_StoreFrontConfig_Edit, GStoreAction.ClientConfig_StoreFrontConfig_View)]
		public ActionResult StoreFrontViewNoTabs(int? id, int? storeFrontConfigId)
		{
			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToView = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				storeFrontToView = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToView == null)
				{
					throw new ApplicationException("StoreFrontToView cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!storeFrontToView.Authorization_IsAuthorized(CurrentUserProfileOrThrow, true, GStoreAction.ClientConfig_StoreFrontConfig_View, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to view configuration for store front: " + storeFrontToView.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToView.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToView = CurrentStoreFrontOrThrow;
			}

			StoreFrontConfiguration storeFrontConfig = null;
			if (storeFrontConfigId.HasValue)
			{
				storeFrontConfig = storeFrontToView.StoreFrontConfigurations.FirstOrDefault(c => c.StoreFrontConfigurationId == storeFrontConfigId.Value);
				if (storeFrontConfig == null)
				{
					AddUserMessage("Configuration not found", "Configuration id [" + storeFrontConfigId.Value + "] was not found, here is the current store front configuration instead.", UserMessageType.Warning);
					storeFrontConfig = storeFrontToView.CurrentConfigOrAny();
				}
			}
			else
			{
				//show current config if no config id passed in
				storeFrontConfig = storeFrontToView.CurrentConfigOrAny();
			}
			StoreFrontConfigAdminViewModel viewModel = new StoreFrontConfigAdminViewModel(storeFrontConfig, CurrentUserProfileOrThrow, null, false, false);

			return View("StoreFrontViewNoTabs", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Edit)]
		public ActionResult StoreFrontEdit(int? id, int? storeFrontConfigId, string Tab)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Store Front Id is null");
			}
			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToEdit = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToEdit == null)
				{
					throw new ApplicationException("StoreFrontToEdit cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to edit configuration for store front: " + storeFrontToEdit.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow;
			}
			StoreFrontConfiguration storeFrontConfig = null;
			if (storeFrontConfigId.HasValue)
			{
				storeFrontConfig = storeFrontToEdit.StoreFrontConfigurations.FirstOrDefault(c => c.StoreFrontConfigurationId == storeFrontConfigId.Value);
				if (storeFrontConfig == null)
				{
					AddUserMessage("Configuration not found", "Configuration id [" + storeFrontConfigId.Value + "] was not found, here is the current store front configuration instead.", UserMessageType.Warning);
					storeFrontConfig = storeFrontToEdit.CurrentConfigOrAny();
				}
			}
			else
			{
				//show current config if no config id passed in
				storeFrontConfig = storeFrontToEdit.CurrentConfigOrAny();
			}
			StoreFrontConfigAdminViewModel viewModel = new StoreFrontConfigAdminViewModel(storeFrontConfig, CurrentUserProfileOrThrow, Tab, false, false);


			int clientId = storeFrontToEdit.ClientId;
			int storeFrontId = storeFrontToEdit.StoreFrontId;

			return View("StoreFrontEdit", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Edit)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult StoreFrontEdit(StoreFrontConfigAdminViewModel model)
		{
			//note: cart edits are done in the cart preview/view/edit pages in /cart
			if (model == null)
			{
				return HttpBadRequest("model is null");
			}

			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToEdit = null;
			if (model.StoreFrontId != CurrentStoreFrontOrThrow.StoreFrontId)
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == model.StoreFrontId)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToEdit == null)
				{
					throw new ApplicationException("StoreFrontToEdit cannot be found. It may be cross-client or inactive. StoreFrontId: " + model.StoreFrontId);
				}

				if (!storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to edit configuration for store front: " + storeFrontToEdit.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow;
			}

			if (model.StoreFrontConfigurationId == 0)
			{
				throw new ApplicationException("model.StoreFrontConfigurationId == 0. Make sure view sets StoreFrontConfigurationId to a valid configuration");
			}
			StoreFrontConfiguration config = storeFrontToEdit.StoreFrontConfigurations.SingleOrDefault(sfc => sfc.StoreFrontConfigurationId == model.StoreFrontConfigurationId);
			if (config == null)
			{
				throw new ApplicationException("store front configuration id [" + model.StoreFrontConfigurationId + "] not found in store front id [" + storeFrontToEdit.StoreFrontId + "] . Make sure view sets StoreFrontConfigurationId to a valid configuration");
			}

			if (ModelState.IsValid)
			{
				config.AccountAdmin_UserProfileId = model.AccountAdmin_UserProfileId;
				config.AccountThemeId = model.AccountThemeId;
				config.AccountLoginRegisterLinkText = model.AccountLoginRegisterLinkText;
				config.AccountLoginShowRegisterLink = model.AccountLoginShowRegisterLink;
				config.AdminThemeId = model.AdminThemeId;
				config.BodyBottomScriptTag = model.BodyBottomScriptTag;
				config.BodyTopScriptTag = model.BodyTopScriptTag;
				config.CartThemeId = model.CartThemeId;
				config.CheckoutThemeId = model.CheckoutThemeId;
				config.CheckoutLogInOrGuestWebFormId = model.CheckoutLogInOrGuestWebFormId;
				config.CheckoutDeliveryInfoDigitalOnlyWebFormId = model.CheckoutDeliveryInfoDigitalOnlyWebFormId;
				config.CheckoutDeliveryInfoShippingWebFormId = model.CheckoutDeliveryInfoShippingWebFormId;
				config.CheckoutDeliveryMethodWebFormId = model.CheckoutDeliveryMethodWebFormId;
				config.CheckoutPaymentInfoWebFormId = model.CheckoutPaymentInfoWebFormId;
				config.CheckoutConfirmOrderWebFormId = model.CheckoutConfirmOrderWebFormId;
				config.Orders_AutoAcceptPaid = model.Orders_AutoAcceptPaid;
				config.PaymentMethod_PayPal_Enabled = model.PaymentMethod_PayPal_Enabled;
				config.PaymentMethod_PayPal_UseLiveServer = model.PaymentMethod_PayPal_UseLiveServer;
				config.PaymentMethod_PayPal_Client_Id = model.PaymentMethod_PayPal_Client_Id;
				config.PaymentMethod_PayPal_Client_Secret = model.PaymentMethod_PayPal_Client_Secret;
				config.CatalogCategoryColLg = model.CatalogCategoryColLg;
				config.CatalogCategoryColMd = model.CatalogCategoryColMd;
				config.CatalogCategoryColSm = model.CatalogCategoryColSm;
				config.CatalogThemeId = model.CatalogThemeId;
				config.CatalogPageInitialLevels = model.CatalogPageInitialLevels;
				config.CatalogProductColLg = model.CatalogProductColLg;
				config.CatalogProductColMd = model.CatalogProductColMd;
				config.CatalogProductColSm = model.CatalogProductColSm;
				config.CatalogProductBundleColLg = model.CatalogProductBundleColLg;
				config.CatalogProductBundleColMd = model.CatalogProductBundleColMd;
				config.CatalogProductBundleColSm = model.CatalogProductBundleColSm;
				config.CatalogProductBundleItemColLg = model.CatalogProductBundleItemColLg;
				config.CatalogProductBundleItemColMd = model.CatalogProductBundleItemColMd;
				config.CatalogProductBundleItemColSm = model.CatalogProductBundleItemColSm;

				config.CatalogAdminThemeId = model.CatalogAdminThemeId;
				config.DefaultNewPageThemeId = model.DefaultNewPageThemeId;
				config.EnableGoogleAnalytics = model.EnableGoogleAnalytics;
				config.GoogleAnalyticsWebPropertyId = model.GoogleAnalyticsWebPropertyId;
				config.HtmlFooter = model.HtmlFooter;
				config.HomePageUseCatalog = model.HomePageUseCatalog;
				config.MetaApplicationName = model.MetaApplicationName;
				config.MetaApplicationTileColor = model.MetaApplicationTileColor;
				config.MetaDescription = model.MetaDescription;
				config.MetaKeywords = model.MetaKeywords;
				config.Name = model.Name;
				config.TimeZoneId = model.TimeZoneId;
				config.CatalogTitle = model.CatalogTitle;
				config.CatalogLayout = model.CatalogLayout;
				config.CatalogHeaderHtml = model.CatalogHeaderHtml;
				config.CatalogFooterHtml = model.CatalogFooterHtml;
				config.CatalogRootListTemplate = model.CatalogRootListTemplate;
				config.CatalogRootHeaderHtml = model.CatalogRootHeaderHtml;
				config.CatalogRootFooterHtml = model.CatalogRootFooterHtml;

				config.NavBarCatalogMaxLevels = model.NavBarCatalogMaxLevels;
				config.NavBarItemsMaxLevels = model.NavBarItemsMaxLevels;
				config.NavBarRegisterLinkText = model.NavBarRegisterLinkText;
				config.NavBarShowRegisterLink = model.NavBarShowRegisterLink;
				config.Order = model.Order;
				config.OrderAdminThemeId = model.OrderAdminThemeId;
				config.OrdersThemeId = model.OrdersThemeId;
				config.Register_WebFormId = model.Register_WebFormId;
				config.RegisterSuccess_PageId = model.RegisterSuccess_PageId;
				config.NotFoundError_PageId = model.NotFoundError_PageId;
				config.NotificationsThemeId = model.NotificationsThemeId;
				config.ProfileThemeId = model.ProfileThemeId;
				config.PublicUrl = model.PublicUrl;
				config.RegisteredNotify_UserProfileId = model.RegisteredNotify_UserProfileId;
				config.StoreError_PageId = model.StoreError_PageId;
				config.UseShoppingCart = model.UseShoppingCart;
				config.CartNavShowCartWhenEmpty = model.CartNavShowCartWhenEmpty;
				config.CartNavShowCartToAnonymous = model.CartNavShowCartToAnonymous;
				config.CartNavShowCartToRegistered = model.CartNavShowCartToRegistered;
				config.CartRequireLogin = model.CartRequireLogin;

				config.WelcomePerson_UserProfileId = model.WelcomePerson_UserProfileId;
				config.OrderAdmin_UserProfileId = model.OrderAdmin_UserProfileId;

				config.ConfigurationName = model.ConfigurationName;
				config.IsPending = model.IsPending;
				config.StartDateTimeUtc = model.StartDateTimeUtc;
				config.EndDateTimeUtc = model.EndDateTimeUtc;
				config = GStoreDb.StoreFrontConfigurations.Update(config);
				GStoreDb.SaveChanges();

				AddUserMessage("Store Front Edit Successful", "Your changes to Store Front Configuration '" + config.ConfigurationName.ToHtml() + "' [" + config.StoreFrontConfigurationId + "] for Store Front '" + config.Name.ToHtml() + "' [" + storeFrontToEdit.StoreFrontId + "] have been saved successfully.", UserMessageType.Success);

				if (model.ResetPagesToThemeId.HasValue && model.ResetPagesToThemeId != 0)
				{
					int pagesUpdated = config.StoreFront.ResetPagesToThemeId(model.ResetPagesToThemeId.Value, GStoreDb);

					if (pagesUpdated == 0)
					{
						AddUserMessage("Page Themes Matched", "All pages already have theme '" + config.Client.Themes.Single(t => t.ThemeId == model.ResetPagesToThemeId).Name.ToHtml() + "' [" + model.ResetPagesToThemeId.Value + "] for Store Front '" + config.Name.ToHtml() + "' [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Success);
					}
					else
					{
						AddUserMessage("Page Themes Updated", pagesUpdated + " Page(s) were changed to theme '" + config.Client.Themes.Single(t => t.ThemeId == model.ResetPagesToThemeId).Name.ToHtml() + "' [" + model.ResetPagesToThemeId.Value + "] for Store Front '" + config.Name.ToHtml() + "' [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Success);
					}
				}

				config.CreateStoreFrontFolders(Request.ApplicationPath, Server);

				return RedirectToAction("StoreFrontView", new { id = model.StoreFrontId, storeFrontConfigId = config.StoreFrontConfigurationId, Tab = model.ActiveTab });
			}

			int clientId = storeFrontToEdit.ClientId;
			int storeFrontId = storeFrontToEdit.StoreFrontId;

			return View("StoreFrontEdit", model);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Create)]
		public ActionResult StoreFrontNewConfig(int? id, string Tab)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Store Front Id is null");
			}
			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToEdit = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToEdit == null)
				{
					throw new ApplicationException("StoreFrontToEdit cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_StoreFrontConfig_Create))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to edit configuration for store front: " + storeFrontToEdit.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow;
			}

			int clientId = storeFrontToEdit.ClientId;
			int storeFrontId = storeFrontToEdit.StoreFrontId;

			StoreFrontConfiguration configToClone = storeFrontToEdit.CurrentConfigOrAny();
			StoreFrontConfiguration newStoreFrontConfig = null;
			if (configToClone != null)
			{
				newStoreFrontConfig = GStoreDb.CloneStoreFrontConfiguration(configToClone, CurrentUserProfileOrThrow);
			}
			else
			{
				newStoreFrontConfig = GStoreDb.StoreFrontConfigurations.Create();
				newStoreFrontConfig.SetDefaultsForNew(CurrentClientOrThrow);
				newStoreFrontConfig.StoreFront = storeFrontToEdit;
				newStoreFrontConfig.StoreFrontId = id.Value;
				newStoreFrontConfig.ApplyDefaultCartConfig();
				newStoreFrontConfig.ApplyDefaultCheckoutConfig();
				newStoreFrontConfig.ApplyDefaultOrdersConfig();
			}

			StoreFrontConfigAdminViewModel viewModel = new StoreFrontConfigAdminViewModel(newStoreFrontConfig, CurrentUserProfileOrThrow, Tab, true, false);

			return View("StoreFrontEdit", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Create)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult StoreFrontNewConfig(StoreFrontConfigAdminViewModel model)
		{
			if (model == null)
			{
				return HttpBadRequest("model is null");
			}

			model.UpdateClient(CurrentClientOrThrow);
			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToEdit = null;
			if (model.StoreFrontId != CurrentStoreFrontOrThrow.StoreFrontId)
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == model.StoreFrontId)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToEdit == null)
				{
					throw new ApplicationException("StoreFrontToEdit (create) cannot be found. It may be cross-client or inactive. StoreFrontId: " + model.StoreFrontId);
				}

				if (!storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to create a configuration for store front: " + storeFrontToEdit.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow;
			}

			ValidateStoreFrontName(model);
			ValidateStoreFrontConfigName(model);

			if (ModelState.IsValid)
			{
				StoreFrontConfiguration config = GStoreDb.StoreFrontConfigurations.Create();

				StoreFrontConfiguration configToCopyFrom = storeFrontToEdit.CurrentConfigOrAny();
				if (configToCopyFrom == null)
				{
					config.ApplyDefaultCartConfig();
				}
				else
				{
					config.CopyValuesFromCartConfigViewModel(storeFrontToEdit.CurrentConfigOrAny().CartConfigViewModel(false, false));
				}
				config.AccountAdmin_UserProfileId = model.AccountAdmin_UserProfileId;
				config.AccountThemeId = model.AccountThemeId;
				config.AccountLoginRegisterLinkText = model.AccountLoginRegisterLinkText;
				config.AccountLoginShowRegisterLink = model.AccountLoginShowRegisterLink;
				config.AdminThemeId = model.AdminThemeId;
				config.CartThemeId = model.CartThemeId;
				config.CheckoutThemeId = model.CheckoutThemeId;
				config.CheckoutLogInOrGuestWebFormId = model.CheckoutLogInOrGuestWebFormId;
				config.CheckoutDeliveryInfoDigitalOnlyWebFormId = model.CheckoutDeliveryInfoDigitalOnlyWebFormId;
				config.CheckoutDeliveryInfoShippingWebFormId = model.CheckoutDeliveryInfoShippingWebFormId;
				config.CheckoutDeliveryMethodWebFormId = model.CheckoutDeliveryMethodWebFormId;
				config.CheckoutPaymentInfoWebFormId = model.CheckoutPaymentInfoWebFormId;
				config.CheckoutConfirmOrderWebFormId = model.CheckoutConfirmOrderWebFormId;
				config.Orders_AutoAcceptPaid = model.Orders_AutoAcceptPaid;
				config.PaymentMethod_PayPal_Enabled = model.PaymentMethod_PayPal_Enabled;
				config.PaymentMethod_PayPal_UseLiveServer = model.PaymentMethod_PayPal_UseLiveServer;
				config.PaymentMethod_PayPal_Client_Id = model.PaymentMethod_PayPal_Client_Id;
				config.PaymentMethod_PayPal_Client_Secret = model.PaymentMethod_PayPal_Client_Secret;
				config.CatalogCategoryColLg = model.CatalogCategoryColLg;
				config.CatalogCategoryColMd = model.CatalogCategoryColMd;
				config.CatalogCategoryColSm = model.CatalogCategoryColSm;
				config.CatalogThemeId = model.CatalogThemeId;
				config.CatalogPageInitialLevels = model.CatalogPageInitialLevels;
				config.CatalogProductColLg = model.CatalogProductColLg;
				config.CatalogProductColMd = model.CatalogProductColMd;
				config.CatalogProductColSm = model.CatalogProductColSm;
				config.CatalogProductBundleColLg = model.CatalogProductBundleColLg;
				config.CatalogProductBundleColMd = model.CatalogProductBundleColMd;
				config.CatalogProductBundleColSm = model.CatalogProductBundleColSm;
				config.CatalogProductBundleItemColLg = model.CatalogProductBundleItemColLg;
				config.CatalogProductBundleItemColMd = model.CatalogProductBundleItemColMd;
				config.CatalogProductBundleItemColSm = model.CatalogProductBundleItemColSm;

				config.CatalogAdminThemeId = model.CatalogAdminThemeId;
				config.DefaultNewPageThemeId = model.DefaultNewPageThemeId;
				config.EnableGoogleAnalytics = model.EnableGoogleAnalytics;
				config.Folder = model.Folder;
				config.GoogleAnalyticsWebPropertyId = model.GoogleAnalyticsWebPropertyId;
				config.HtmlFooter = model.HtmlFooter;
				config.HomePageUseCatalog = model.HomePageUseCatalog;
				config.MetaApplicationName = model.MetaApplicationName;
				config.MetaApplicationTileColor = model.MetaApplicationTileColor;
				config.MetaDescription = model.MetaDescription;
				config.MetaKeywords = model.MetaKeywords;
				config.Name = model.Name;
				config.TimeZoneId = model.TimeZoneId;
				config.CatalogTitle = model.CatalogTitle;

				config.CatalogLayout = model.CatalogLayout;
				config.CatalogHeaderHtml = model.CatalogHeaderHtml;
				config.CatalogFooterHtml = model.CatalogFooterHtml;
				config.CatalogRootListTemplate = model.CatalogRootListTemplate;
				config.CatalogRootHeaderHtml = model.CatalogRootHeaderHtml;
				config.CatalogRootFooterHtml = model.CatalogRootFooterHtml;

				config.NavBarCatalogMaxLevels = model.NavBarCatalogMaxLevels;
				config.NavBarItemsMaxLevels = model.NavBarItemsMaxLevels;
				config.NavBarRegisterLinkText = model.NavBarRegisterLinkText;
				config.NavBarShowRegisterLink = model.NavBarShowRegisterLink;
				config.Order = model.Order;
				config.OrderAdminThemeId = model.OrderAdminThemeId;
				config.OrdersThemeId = model.OrdersThemeId;
				config.Register_WebFormId = model.Register_WebFormId;
				config.RegisterSuccess_PageId = model.RegisterSuccess_PageId;
				config.NotFoundError_PageId = model.NotFoundError_PageId;
				config.NotificationsThemeId = model.NotificationsThemeId;
				config.ProfileThemeId = model.ProfileThemeId;
				config.PublicUrl = model.PublicUrl;
				config.RegisteredNotify_UserProfileId = model.RegisteredNotify_UserProfileId;
				config.StoreError_PageId = model.StoreError_PageId;
				config.UseShoppingCart = model.UseShoppingCart;
				config.CartNavShowCartWhenEmpty = model.CartNavShowCartWhenEmpty;
				config.CartNavShowCartToAnonymous = model.CartNavShowCartToAnonymous;
				config.CartNavShowCartToRegistered = model.CartNavShowCartToRegistered;
				config.CartRequireLogin = model.CartRequireLogin;

				config.WelcomePerson_UserProfileId = model.WelcomePerson_UserProfileId;
				config.OrderAdmin_UserProfileId = model.OrderAdmin_UserProfileId;

				config.StoreFrontId = model.StoreFrontId;
				config.ClientId = model.Client.ClientId;
				config.ConfigurationName = model.ConfigurationName;
				config.IsPending = model.IsPending;
				config.StartDateTimeUtc = model.StartDateTimeUtc;
				config.EndDateTimeUtc = model.EndDateTimeUtc;
				config = GStoreDb.StoreFrontConfigurations.Add(config);
				GStoreDb.SaveChanges();

				AddUserMessage("New Store Front Configuration Successful", "New Configuration Created! Store Front Configuration '" + config.ConfigurationName.ToHtml() + "' [" + config.StoreFrontConfigurationId + "] for Store Front '" + config.Name.ToHtml() + "' [" + storeFrontToEdit.StoreFrontId + "].", UserMessageType.Success);

				if (storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_Manager))
				{
					return RedirectToAction("Manager");
				}
				else if (storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_StoreFrontConfig_View))
				{
					return RedirectToAction("StoreFrontView", new { id = config.StoreFrontId, storeFrontConfigId = config.StoreFrontConfigurationId, Tab = model.ActiveTab });
				}
				return RedirectToAction("Index", "StoreAdmin");
			}

			int clientId = storeFrontToEdit.ClientId;
			int storeFrontId = storeFrontToEdit.StoreFrontId;

			model.IsCreatePage = true;
			return View("StoreFrontEdit", model);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Delete)]
		public ActionResult StoreFrontDelete(int? id, int? storeFrontConfigId)
		{
			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToEdit = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToEdit == null)
				{
					throw new ApplicationException("StoreFrontToEdit cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to delete configurations for store front: " + storeFrontToEdit.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow;
			}
			StoreFrontConfiguration storeFrontConfig = null;
			if (storeFrontConfigId.HasValue)
			{
				storeFrontConfig = storeFrontToEdit.StoreFrontConfigurations.FirstOrDefault(c => c.StoreFrontConfigurationId == storeFrontConfigId.Value);
				if (storeFrontConfig == null)
				{
					AddUserMessage("Configuration not found", "Configuration id [" + storeFrontConfigId.Value + "] was not found, here is the current store front configuration instead.", UserMessageType.Warning);
					storeFrontConfig = storeFrontToEdit.CurrentConfigOrAny();
				}
			}
			else
			{
				//show current config if no config id passed in
				storeFrontConfig = storeFrontToEdit.CurrentConfigOrAny();
			}

			//make sure this is not the last active config, otherwise NO DELETE!

			if (storeFrontToEdit.StoreFrontConfigurations.AsQueryable().Count() <= 1)
			{
				AddUserMessage("Delete Denied", "You cannot delete the last configuration for store front '" + storeFrontConfig.Name.ToHtml() + "' [" + storeFrontConfig.StoreFrontId + "]. Edit this configuration, or create a new configuration, then delete this one. Configuration id: " + storeFrontConfig.StoreFrontConfigurationId, UserMessageType.Danger);
				return RedirectToAction("Manager");
			}

			//if no configs are active, go ahead and delete away

			if (storeFrontConfig.IsActiveDirect() && (storeFrontToEdit.StoreFrontConfigurations.AsQueryable().WhereIsActive().Count() == 1))
			{
				AddUserMessage("Delete Denied", "You cannot delete the last ACTIVE configuration for a store front. Edit this configuration, or create a new configuration, then delete this one or make this one INACTIVE then delete it. Configuration id: " + storeFrontConfig.StoreFrontConfigurationId, UserMessageType.Danger);
				return RedirectToAction("Manager");
			}

			StoreFrontConfigAdminViewModel viewModel = new StoreFrontConfigAdminViewModel(storeFrontConfig, CurrentUserProfileOrThrow, null, false, true);

			return View("StoreFrontDelete", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("StoreFrontDelete")]
		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Delete)]
		public ActionResult StoreFrontDeleteConfirmed(int? id, int? storeFrontConfigId)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Id (StoreFrontId) == null");
			}
			if (!storeFrontConfigId.HasValue)
			{
				return HttpBadRequest("storeFrontConfigId == null");
			}

			//verify the storeFront permissions in case we're operating on a different storefront
			StoreFront storeFrontToEdit = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (storeFrontToEdit == null)
				{
					throw new ApplicationException("StoreFrontToEdit cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to delete configurations for store front: " + storeFrontToEdit.CurrentConfigOrAny().Name.ToHtml() + " [" + storeFrontToEdit.StoreFrontId + "]", UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow;
			}

			StoreFrontConfiguration storeFrontConfig = null;
			storeFrontConfig = storeFrontToEdit.StoreFrontConfigurations.FirstOrDefault(c => c.StoreFrontConfigurationId == storeFrontConfigId.Value);
			if (storeFrontConfig == null)
			{
				AddUserMessage("Configuration not found", "Configuration id [" + storeFrontConfigId.Value + "] was not found. It may have been deleted by another user.", UserMessageType.Warning);
				return RedirectToAction("Manager");
			}

			//make sure this is not the last active config, otherwise NO DELETE!
			if (storeFrontToEdit.StoreFrontConfigurations.AsQueryable().Count() <= 1)
			{
				AddUserMessage("Delete Denied", "You cannot delete the last configuration for store front '" + storeFrontConfig.Name.ToHtml() + "' [" + storeFrontConfig.StoreFrontId + "]. Edit this configuration or create a new configuration, then delete this one. Configuration id: " + storeFrontConfig.StoreFrontConfigurationId, UserMessageType.Danger);
				return RedirectToAction("Manager");
			}

			//if no configs are active, go ahead and delete away
			if (storeFrontConfig.IsActiveDirect() && (storeFrontToEdit.StoreFrontConfigurations.AsQueryable().WhereIsActive().Count() == 1))
			{
				AddUserMessage("Delete Denied", "You cannot delete the last ACTIVE configuration for a store front. Edit this configuration or create a new configuration, then delete this one or make this one INACTIVE then delete it. Configuration id: " + storeFrontConfig.StoreFrontConfigurationId, UserMessageType.Danger);
				return RedirectToAction("Manager");
			}

			string configName = storeFrontConfig.ConfigurationName;
			int configId = storeFrontConfig.StoreFrontConfigurationId;

			bool result = GStoreDb.StoreFrontConfigurations.Delete(storeFrontConfig);
			if (result)
			{
				AddUserMessage("Configuration Deleted", "Store Front Configuration '" + configName.ToHtml() + "' [" + configId + "] was deleted successfully.", UserMessageType.Success);
				GStoreDb.SaveChanges();
			}
			else
			{
				AddUserMessage("Configuration Delete Error", "There was an error deleting Store Front Configuration '" + configName.ToHtml() + "' [" + configId + "].", UserMessageType.Danger);
			}

			return RedirectToAction("Manager");
		}

		protected SelectList ThemeList()
		{
			var query = CurrentClientOrThrow.Themes.OrderBy(t => t.Order).ThenBy(t => t.ThemeId);
			IEnumerable<SelectListItem> items = query.Select(t => new SelectListItem
			{
				Value = t.ThemeId.ToString(),
				Text = t.Name + " [" + t.ThemeId + "]"
			});
			return new SelectList(items, "Value", "Text");
		}

		protected SelectList UserProfileList(int clientId, int storeFrontId)
		{
			IQueryable<UserProfile> query = GStoreDb.UserProfiles.All();

			IOrderedQueryable<UserProfile> orderedQuery = query.Where(p => !p.ClientId.HasValue || p.ClientId.Value == clientId)
				.Where(p => !p.StoreFrontId.HasValue || p.StoreFrontId.Value == storeFrontId)
				.OrderBy(p => p.Order).ThenBy(p => p.UserProfileId).ThenBy(p => p.UserName);

			List<UserProfile> profiles = orderedQuery.ToList();

			IEnumerable<SelectListItem> items = profiles.Select(p => new SelectListItem
			{
				Value = p.UserProfileId.ToString(),
				Text = p.FullName + " <" + p.Email + ">"
				+ (p.StoreFrontId.HasValue ? " - Store '" + p.StoreFront.CurrentConfigOrAny().Name + "' [" + p.StoreFrontId + "]" : " (no store)")
				+ (p.ClientId.HasValue ? " - Client '" + p.Client.Name + "' [" + p.ClientId + "]" : " (no client)")
			});

			return new SelectList(items, "Value", "Text");
		}

		protected void ValidateStoreFrontName(StoreFrontConfiguration storeFrontConfig)
		{
			if (GStoreDb.StoreFrontConfigurations.Where(sf => sf.StoreFrontId != storeFrontConfig.StoreFrontId && sf.ClientId == storeFrontConfig.ClientId && sf.Name.ToLower() == storeFrontConfig.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Store Front name '" + storeFrontConfig.Name + "' is already in use. Please choose a new name");
				bool nameIsDirty = true;
				int index = 1;
				while (nameIsDirty)
				{
					index++;
					storeFrontConfig.Name = storeFrontConfig.Name + " " + index;
					nameIsDirty = GStoreDb.StoreFrontConfigurations.Where(sf => sf.ClientId == storeFrontConfig.ClientId && sf.Name.ToLower() == storeFrontConfig.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(storeFrontConfig.Name, storeFrontConfig.Name, null);
				}
			}
		}

		protected void ValidateStoreFrontName(StoreFrontConfigAdminViewModel viewModelForCreate)
		{
			if (viewModelForCreate == null)
			{
				throw new ArgumentNullException("viewModelForCreate");
			}
			if (viewModelForCreate.Client == null)
			{
				throw new ArgumentNullException("viewModelForCreate.Client");
			}
			int clientId = viewModelForCreate.Client.ClientId;

			if (GStoreDb.StoreFrontConfigurations.Where(sf => sf.StoreFrontId != viewModelForCreate.StoreFrontId && sf.ClientId == clientId && sf.Name.ToLower() == viewModelForCreate.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Store Front name '" + viewModelForCreate.Name + "' is already in use for another store front. Please choose a new name");
				bool nameIsDirty = true;
				int index = 1;
				while (nameIsDirty)
				{
					index++;
					viewModelForCreate.Name = viewModelForCreate.Name + " " + index;
					nameIsDirty = GStoreDb.StoreFrontConfigurations.Where(sf => sf.ClientId == viewModelForCreate.Client.ClientId && sf.Name.ToLower() == viewModelForCreate.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(viewModelForCreate.Name, viewModelForCreate.Name, null);
				}
			}
		}

		protected void ValidateStoreFrontConfigName(StoreFrontConfigAdminViewModel viewModelForCreate)
		{
			if (viewModelForCreate == null)
			{
				throw new ArgumentNullException("viewModelForCreate");
			}
			if (GStoreDb.StoreFrontConfigurations.Where(sf => sf.StoreFrontId == viewModelForCreate.StoreFrontId && sf.ClientId == viewModelForCreate.Client.ClientId && sf.ConfigurationName.ToLower() == viewModelForCreate.ConfigurationName.ToLower()).Any())
			{
				this.ModelState.AddModelError("ConfigurationName", "Store Front Configuration Name '" + viewModelForCreate.ConfigurationName + "' is already in use for another configuration. Please choose a new name");
				bool nameIsDirty = true;
				int index = 1;
				while (nameIsDirty)
				{
					index++;
					viewModelForCreate.ConfigurationName = viewModelForCreate.ConfigurationName + " " + index;
					nameIsDirty = GStoreDb.StoreFrontConfigurations.Where(sf => sf.StoreFrontId == viewModelForCreate.StoreFrontId && sf.ClientId == viewModelForCreate.Client.ClientId && sf.ConfigurationName.ToLower() == viewModelForCreate.ConfigurationName.ToLower()).Any();
				}
				if (ModelState.ContainsKey("ConfigurationName"))
				{
					ModelState["ConfigurationName"].Value = new ValueProviderResult(viewModelForCreate.ConfigurationName, viewModelForCreate.ConfigurationName, null);
				}
			}
		}

		protected SelectList WebFormList(int clientId, int storeFrontId)
		{
			SelectListItem itemNone = new SelectListItem();

			var query = CurrentClientOrThrow.WebForms.OrderBy(pg => pg.Order).ThenBy(pg => pg.WebFormId);
			IEnumerable<SelectListItem> items = query.Select(wf => new SelectListItem
			{
				Value = wf.WebFormId.ToString(),
				Text = wf.Name + " [" + wf.WebFormId + "]"
			});

			return new SelectList(items, "Value", "Text");
		}

		protected SelectList PageList(int clientId, int storeFrontId)
		{
			var query = CurrentStoreFrontOrThrow.Pages.OrderBy(pg => pg.Order).ThenBy(pg => pg.PageId);
			IEnumerable<SelectListItem> items = query.Select(pg => new SelectListItem
			{
				Value = pg.PageId.ToString(),
				Text = pg.Name + " [" + pg.PageId + "]"
			});
			return new SelectList(items, "Value", "Text");
		}
	}
}
