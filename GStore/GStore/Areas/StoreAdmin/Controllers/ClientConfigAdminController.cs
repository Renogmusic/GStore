using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Areas.StoreAdmin.ViewModels;

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
			ClientConfigViewModel viewModel = new ClientConfigViewModel(CurrentClientOrThrow, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
			return View("ClientView", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Edit)]
		public ActionResult ClientEdit()
		{
			ClientConfigViewModel viewModel = new ClientConfigViewModel(CurrentClientOrThrow, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
			return View("ClientEdit", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_Edit)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult ClientEdit(ClientConfigViewModel model)
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

			return View("ClientEdit", model);
		}


		[AuthorizeGStoreAction(true, GStoreAction.ClientConfig_StoreFrontConfig_Edit, GStoreAction.ClientConfig_StoreFrontConfig_View)]
		public ActionResult StoreFrontView(int? id)
		{
			//verify the storeFront permissions in case we're operating on a different storefront
			GStore.Models.StoreFront storeFrontToView = null;
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
					AddUserMessage("Access denied.", "Sorry, you do not have permission to view configuration for store front: " + storeFrontToView.Name + " [" + storeFrontToView.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Danger);
					return RedirectToAction("Manager");
				}
			}
			else
			{
				storeFrontToView = CurrentStoreFrontOrThrow;
			}

			return View("StoreFrontView", new StoreFrontConfigViewModel(storeFrontToView, CurrentUserProfileOrThrow));
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Edit)]
		public ActionResult StoreFrontEdit(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Store Front Id is null");
			}
			//verify the storeFront permissions in case we're operating on a different storefront
			GStore.Models.StoreFront storeFrontToEdit = null;
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

			int clientId = storeFrontToEdit.ClientId;
			int storeFrontId = storeFrontToEdit.StoreFrontId;
			ViewBag.UserProfileList = UserProfileList(clientId, storeFrontId);
			ViewBag.ThemeList = ThemeList();
			ViewBag.NotFoundPageList = NotFoundPageList(clientId, storeFrontId);
			ViewBag.StoreErrorPageList = StoreErrorPageList(clientId, storeFrontId);

			return View("StoreFrontEdit", new StoreFrontConfigViewModel(storeFrontToEdit, CurrentUserProfileOrThrow));
		}

		[AuthorizeGStoreAction(GStoreAction.ClientConfig_StoreFrontConfig_Edit)]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult StoreFrontEdit(StoreFrontConfigViewModel model)
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
				storeFrontToEdit.AccountThemeId = model.AccountThemeId;
				storeFrontToEdit.AccountLoginRegisterLinkText = model.AccountLoginRegisterLinkText;
				storeFrontToEdit.AccountLoginShowRegisterLink = model.AccountLoginShowRegisterLink;
				storeFrontToEdit.AdminLayoutName = model.AdminLayoutName;
				storeFrontToEdit.AdminThemeId = model.AdminThemeId;
				storeFrontToEdit.CatalogCategoryColLg = model.CatalogCategoryColLg;
				storeFrontToEdit.CatalogCategoryColMd = model.CatalogCategoryColMd;
				storeFrontToEdit.CatalogCategoryColSm = model.CatalogCategoryColSm;
				storeFrontToEdit.CatalogLayoutName = model.CatalogLayoutName;
				storeFrontToEdit.CatalogThemeId = model.CatalogThemeId;
				storeFrontToEdit.CatalogPageInitialLevels = model.CatalogPageInitialLevels;
				storeFrontToEdit.CatalogProductColLg = model.CatalogProductColLg;
				storeFrontToEdit.CatalogProductColMd = model.CatalogProductColMd;
				storeFrontToEdit.CatalogProductColSm = model.CatalogProductColSm;
				storeFrontToEdit.DefaultNewPageLayoutName = model.DefaultNewPageLayoutName;
				storeFrontToEdit.DefaultNewPageThemeId = model.DefaultNewPageThemeId;
				storeFrontToEdit.EnableGoogleAnalytics = model.EnableGoogleAnalytics;
				storeFrontToEdit.GoogleAnalyticsWebPropertyId = model.GoogleAnalyticsWebPropertyId;
				storeFrontToEdit.HtmlFooter = model.HtmlFooter;
				storeFrontToEdit.MetaApplicationName = model.MetaApplicationName;
				storeFrontToEdit.MetaApplicationTileColor = model.MetaApplicationTileColor;
				storeFrontToEdit.MetaDescription = model.MetaDescription;
				storeFrontToEdit.MetaKeywords = model.MetaKeywords;
				storeFrontToEdit.Name = model.Name;
				storeFrontToEdit.NavBarCatalogMaxLevels = model.NavBarCatalogMaxLevels;
				storeFrontToEdit.NavBarItemsMaxLevels = model.NavBarItemsMaxLevels;
				storeFrontToEdit.NavBarRegisterLinkText = model.NavBarRegisterLinkText;
				storeFrontToEdit.NavBarShowRegisterLink = model.NavBarShowRegisterLink;
				storeFrontToEdit.NotFoundError_PageId = model.NotFoundError_PageId;
				storeFrontToEdit.NotificationsLayoutName = model.NotificationsLayoutName;
				storeFrontToEdit.NotificationsThemeId = model.NotificationsThemeId;
				storeFrontToEdit.ProfileLayoutName = model.ProfileLayoutName;
				storeFrontToEdit.ProfileThemeId = model.ProfileThemeId;
				storeFrontToEdit.PublicUrl = model.PublicUrl;
				storeFrontToEdit.RegisteredNotify_UserProfileId = model.RegisteredNotify_UserProfileId;
				storeFrontToEdit.StoreError_PageId = model.StoreError_PageId;
				storeFrontToEdit.WelcomePerson_UserProfileId = model.WelcomePerson_UserProfileId;

				GStoreDb.StoreFronts.Update(storeFrontToEdit);
				GStoreDb.SaveChanges();

				AddUserMessage("Store Front Edit Successful", "Your changes to Store Front '" + Server.HtmlEncode(storeFrontToEdit.Name) + "' [" + storeFrontToEdit.StoreFrontId + "] have been saved successfully.", AppHtmlHelpers.UserMessageType.Success);

				return RedirectToAction("StoreFrontView", new { id = model.StoreFrontId });
			}

			int clientId = storeFrontToEdit.ClientId;
			int storeFrontId = storeFrontToEdit.StoreFrontId;
			ViewBag.UserProfileList = UserProfileList(clientId, storeFrontId);
			ViewBag.ThemeList = ThemeList();
			ViewBag.NotFoundPageList = NotFoundPageList(clientId, storeFrontId);
			ViewBag.StoreErrorPageList = StoreErrorPageList(clientId, storeFrontId);

			return View("StoreFrontEdit", model);
		}

		protected SelectList ThemeList()
		{
			var query = GStoreDb.Themes.All().OrderBy(t => t.Order).ThenBy(t => t.ThemeId);
			IQueryable<SelectListItem> items = query.Select(t => new SelectListItem
			{
				Value = t.ThemeId.ToString(),
				Text = t.Name + " [" + t.ThemeId + "]"
			});
			return new SelectList(items, "Value", "Text");
		}

		protected SelectList UserProfileList(int clientId, int storeFrontId)
		{
			var query = GStoreDb.UserProfiles.All();

			query = query.Where(p => !p.ClientId.HasValue || p.ClientId.Value == clientId)
				.Where(p => !p.StoreFrontId.HasValue || p.StoreFrontId.Value == storeFrontId)
				.OrderBy(p => p.Order).ThenBy(p => p.UserProfileId).ThenBy(p => p.UserName);

			IQueryable<SelectListItem> items = query.Select(p => new SelectListItem
			{
				Value = p.UserProfileId.ToString(),
				Text = p.FullName + " <" + p.Email + ">"
				+ (p.StoreFrontId.HasValue ? " - Store '" + p.StoreFront.Name + "' [" + p.StoreFrontId + "]" : " (no store)")
				+ (p.ClientId.HasValue ? " - Client '" + p.Client.Name + "' [" + p.ClientId + "]" : " (no client)")
			});

			return new SelectList(items, "Value", "Text");
		}

		protected void ValidateStoreFrontName(GStore.Models.StoreFront storeFront)
		{
			if (GStoreDb.StoreFronts.Where(sf => sf.StoreFrontId != storeFront.StoreFrontId && sf.ClientId == storeFront.ClientId && sf.Name.ToLower() == storeFront.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Store Front name '" + storeFront.Name + "' is already in use. Please choose a new name");
				bool nameIsDirty = true;
				while (nameIsDirty)
				{
					storeFront.Name = storeFront.Name + "_New";
					nameIsDirty = GStoreDb.StoreFronts.Where(sf => sf.ClientId == storeFront.ClientId && sf.Name.ToLower() == storeFront.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(storeFront.Name, storeFront.Name, null);
				}
			}
		}

		protected SelectList StoreErrorPageList(int clientId, int storeFrontId)
		{
			SelectListItem itemNone = new SelectListItem();
			itemNone.Value = null;
			itemNone.Text = "(GStore System Default Error Page)";
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(itemNone);

			if (CurrentStoreFrontOrNull == null)
			{
				return new SelectList(list, "Value", "Text");
			}

			var query = CurrentStoreFrontOrNull.Pages.OrderBy(pg => pg.Order).ThenBy(pg => pg.PageId);
			IEnumerable<SelectListItem> items = query.Select(pg => new SelectListItem
			{
				Value = pg.PageId.ToString(),
				Text = pg.Name + " [" + pg.PageId + "]"
			});

			if (items.Count() > 0)
			{
				list.AddRange(items);
			}

			return new SelectList(list, "Value", "Text");
		}

		protected SelectList NotFoundPageList(int clientId, int storeFrontId)
		{
			SelectListItem itemNone = new SelectListItem();
			itemNone.Value = null;
			itemNone.Text = "(GStore System Default Not Found Page)";
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(itemNone);

			var query = CurrentStoreFrontOrThrow.Pages.OrderBy(pg => pg.Order).ThenBy(pg => pg.PageId);
			IEnumerable<SelectListItem> items = query.Select(pg => new SelectListItem
			{
				Value = pg.PageId.ToString(),
				Text = pg.Name + " [" + pg.PageId + "]"
			});

			if (items.Count() > 0)
			{
				list.AddRange(items);
			}

			return new SelectList(list, "Value", "Text");
		}



	}
}