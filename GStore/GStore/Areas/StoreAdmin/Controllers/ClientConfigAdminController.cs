using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
	public class ClientConfigAdminController : BaseClasses.StoreAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Manager)]
		public ActionResult Manager()
		{
			return View("Manager", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_Edit, GStoreAction.ClientConfig_View)]
		public ActionResult ClientView()
		{
			Models.ViewModels.ClientConfigViewModel viewModel = new Models.ViewModels.ClientConfigViewModel(CurrentClientOrThrow, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
			return View("ClientView", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Edit)]
		public ActionResult ClientEdit()
		{
			Models.ViewModels.ClientConfigViewModel viewModel = new Models.ViewModels.ClientConfigViewModel(CurrentClientOrThrow, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
			return View("ClientEdit", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Edit)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult ClientEdit(Models.ViewModels.ClientConfigViewModel model)
		{
			if (ModelState.IsValid)
			{
				GStore.Models.Client client = CurrentClientOrThrow;
				client.EnableNewUserRegisteredBroadcast = model.EnableNewUserRegisteredBroadcast;
				client.EnablePageViewLog = model.EnablePageViewLog;
				client.Name = model.Name;
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
				return RedirectToAction("ClientView");
			}

			Models.ViewModels.ClientConfigViewModel viewModel = new Models.ViewModels.ClientConfigViewModel(CurrentClientOrThrow, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
			return View("ClientEdit", this.StoreAdminViewModel);
		}


		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_StoreFrontConfig_Edit, GStoreAction.ClientConfig_StoreFrontConfig_View)]
		public ActionResult StoreFrontView(int? id)
		{
			//verify the storeFront permissions in case we're operating on a different storefront
			GStore.Models.StoreFront otherStoreFrontToView = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				otherStoreFrontToView = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (otherStoreFrontToView == null)
				{
					throw new ApplicationException("StoreFrontToView cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!otherStoreFrontToView.Authorization_IsAuthorized(CurrentUserProfileOrThrow, true, GStoreAction.ClientConfig_StoreFrontConfig_View, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to view configuration for store front: " + otherStoreFrontToView.Name + " [" + otherStoreFrontToView.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}

			return View("StoreFrontView", new Models.ViewModels.StoreFrontConfigViewModel(otherStoreFrontToView, CurrentUserProfileOrThrow));
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Edit)]
		public ActionResult StoreFrontEdit(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ID is null");
			}
			//verify the storeFront permissions in case we're operating on a different storefront
			GStore.Models.StoreFront otherStoreFrontToEdit = null;
			if (id.HasValue && CurrentStoreFrontOrThrow.StoreFrontId != id.Value)
			{
				otherStoreFrontToEdit = CurrentStoreFrontOrThrow.Client.StoreFronts.AsQueryable()
					.Where(sf => sf.StoreFrontId == id.Value)
					.WhereIsActive()
					.SingleOrDefault();
				if (otherStoreFrontToEdit == null)
				{
					throw new ApplicationException("StoreFrontToEdit cannot be found. It may be cross-client or inactive. StoreFrontId: " + id.Value);
				}

				if (!otherStoreFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, true, GStoreAction.ClientConfig_StoreFrontConfig_View, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to edit configuration for store front: " + otherStoreFrontToEdit.Name + " [" + otherStoreFrontToEdit.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Danger);
					return RedirectToAction("Manager");
				}

			}

			return View("StoreFrontEdit", new Models.ViewModels.StoreFrontConfigViewModel(otherStoreFrontToEdit, CurrentUserProfileOrThrow));
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Edit)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult StoreFrontEdit(Models.ViewModels.StoreFrontConfigViewModel model)
		{
			if (model == null)
			{
				return HttpBadRequest("model is null");
			}

			//verify the storeFront permissions in case we're operating on a different storefront
			GStore.Models.StoreFront storeFrontToEdit = null;
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

				if (!storeFrontToEdit.Authorization_IsAuthorized(CurrentUserProfileOrThrow, true, GStoreAction.ClientConfig_StoreFrontConfig_View, GStoreAction.ClientConfig_StoreFrontConfig_Edit))
				{
					AddUserMessage("Access denied.", "Sorry, you do not have permission to edit configuration for store front: " + storeFrontToEdit.Name + " [" + storeFrontToEdit.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToEdit = CurrentStoreFrontOrThrow;
			}

			if (ModelState.IsValid)
			{
				storeFrontToEdit.AccountAdmin_UserProfileId = model.AccountAdmin_UserProfileId;
				storeFrontToEdit.AccountLayoutName = model.AccountLayoutName;
				storeFrontToEdit.AdminLayoutName = model.AdminLayoutName;
				storeFrontToEdit.CatalogCategoryColLg = model.CatalogCategoryColLg;
				storeFrontToEdit.CatalogCategoryColMd = model.CatalogCategoryColMd;
				storeFrontToEdit.CatalogCategoryColSm = model.CatalogCategoryColSm;
				storeFrontToEdit.CatalogLayoutName = model.CatalogLayoutName;
				storeFrontToEdit.CatalogPageInitialLevels = model.CatalogPageInitialLevels;
				storeFrontToEdit.CatalogProductColLg = model.CatalogProductColLg;
				storeFrontToEdit.CatalogProductColMd = model.CatalogProductColMd;
				storeFrontToEdit.CatalogProductColSm = model.CatalogProductColSm;
				storeFrontToEdit.DefaultNewPageLayoutName = model.DefaultNewPageLayoutName;
				storeFrontToEdit.MetaApplicationName = model.MetaApplicationName;
				storeFrontToEdit.MetaApplicationTileColor = model.MetaApplicationTileColor;
				storeFrontToEdit.MetaDescription = model.MetaDescription;
				storeFrontToEdit.MetaKeywords = model.MetaKeywords;
				storeFrontToEdit.Name = model.Name;
				storeFrontToEdit.NavBarCatalogMaxLevels = model.NavBarCatalogMaxLevels;
				storeFrontToEdit.NavBarItemsMaxLevels = model.NavBarItemsMaxLevels;
				storeFrontToEdit.NotFoundError_PageId = model.NotFoundError_PageId;
				storeFrontToEdit.NotificationsLayoutName = model.NotificationsLayoutName;
				storeFrontToEdit.ProfileLayoutName = model.ProfileLayoutName;
				storeFrontToEdit.PublicUrl = model.PublicUrl;
				storeFrontToEdit.RegisteredNotify_UserProfileId = model.RegisteredNotify_UserProfileId;
				storeFrontToEdit.StoreError_PageId = model.StoreError_PageId;
				storeFrontToEdit.ThemeId = model.ThemeId;
				storeFrontToEdit.WelcomePerson_UserProfileId = model.WelcomePerson_UserProfileId;

				GStoreDb.StoreFronts.Update(storeFrontToEdit);
				GStoreDb.SaveChanges();

				return RedirectToAction("Edit", new { id = model.StoreFrontId });
			}

			return View("StoreFrontEdit", new Models.ViewModels.StoreFrontConfigViewModel(storeFrontToEdit, CurrentUserProfileOrThrow));
		}

	}
}