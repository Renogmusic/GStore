using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Areas.StoreAdmin.ViewModels;
using GStore.AppHtmlHelpers;
using GStore.Models;
using GStore.Models.ViewModels;


namespace GStore.Areas.StoreAdmin.Controllers
{
	public class PageAdminController : BaseClasses.StoreAdminBaseController
	{
		[AuthorizeGStoreAction(GStoreAction.Pages_Manager)]
		public ActionResult Manager(string SortBy, bool? SortAscending)
		{
			IOrderedQueryable<Page> pages = CurrentStoreFrontOrThrow.Pages.AsQueryable().ApplySort(this, SortBy, SortAscending);

			PageAdminManagerViewModel viewModel = new PageAdminManagerViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, pages);
			return View("Manager", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Pages_Create)]
		public ActionResult Create(string Tab)
		{
			Models.Page page = GStoreDb.Pages.Create();
			page.SetDefaultsForNew(CurrentStoreFrontOrThrow);
			Models.ViewModels.PageEditViewModel viewModel = new PageEditViewModel(page, isStoreAdminEdit: true, isCreatePage: true, activeTab: Tab);
			return View("Create", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Create)]
		public virtual ActionResult Create(GStore.Models.ViewModels.PageEditViewModel pageEditViewModel)
		{
			if (pageEditViewModel.PageTemplateId == 0)
			{
				ModelState.AddModelError("PageTemplateId", "Page Template must be selected");
			}

			if (pageEditViewModel.ThemeId == 0)
			{
				ModelState.AddModelError("ThemeId", "Page Theme must be selected");
			}

			if (string.IsNullOrWhiteSpace(pageEditViewModel.Url))
			{
				ModelState.AddModelError("Url", "Url is blank. Enter a valid Url for this page. Example: '/' or '/Contact'");
			}
			else
			{
				if (pageEditViewModel.Url.Trim('/').Trim() == ".")
				{
					ModelState.AddModelError("Url", "Url '" + pageEditViewModel.Url.ToHtml() + "' is invalid. Example: '/' or '/Contact'");
				}
				if (pageEditViewModel.Url.Contains('?'))
				{
					ModelState.AddModelError("Url", "Url '" + pageEditViewModel.Url.ToHtml() + "' is invalid. Question mark is not allowed in URL. Example: '/' or '/Contact'");
				}
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidatePageUrl(this, pageEditViewModel.Url, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (urlIsValid && ModelState.IsValid)
			{
				try
				{
					Page page = null;
					page = GStoreDb.CreatePage(pageEditViewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Page Created!", "Page '" + page.Name.ToHtml() + "' [" + page.PageId + "] was created successfully for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);

					return RedirectToAction("Manager");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating page '" + pageEditViewModel.Name + "' Url: '" + pageEditViewModel.Url + "' for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Page!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Page Create Error", "There was an error with your entry for new page '" + pageEditViewModel.Name.ToHtml() + "' for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it below and save.", AppHtmlHelpers.UserMessageType.Danger);
			}

			pageEditViewModel.IsStoreAdminEdit = true;
			pageEditViewModel.FillListsIfEmpty(storeFront.Client, storeFront);
			pageEditViewModel.IsCreatePage = true;
			return View("Create", pageEditViewModel);
		}



		[AuthorizeGStoreAction(GStoreAction.Pages_Edit)]
		public ActionResult Edit(int? id, string Tab)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("PageId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Models.Page page = storeFront.Pages.Where(p => p.PageId == id.Value).SingleOrDefault();
			if (page == null)
			{
				AddUserMessage("Page not found", "Sorry, the page you are trying to edit cannot be found. Page id: [" + id.Value + "] for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			Models.ViewModels.PageEditViewModel viewModel = new PageEditViewModel(page, isStoreAdminEdit: true, activeTab: Tab);
			return View("Edit", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.Pages_View, GStoreAction.Pages_Edit)]
		public ActionResult Details(int? id, string Tab)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("PageId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Models.Page page = storeFront.Pages.Where(p => p.PageId == id.Value).SingleOrDefault();
			if (page == null)
			{
				AddUserMessage("Page not found", "Sorry, the page you are trying to view cannot be found. Page id: [" + id.Value + "] for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			Models.ViewModels.PageEditViewModel viewModel = new PageEditViewModel(page, isStoreAdminEdit: true, isReadOnly: true, activeTab: Tab);
			return View("Details", viewModel);
		}


		[AuthorizeGStoreAction(true, GStoreAction.Pages_View, GStoreAction.Pages_Delete)]
		public ActionResult Delete(int? id, string Tab)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("PageId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Models.Page page = storeFront.Pages.Where(p => p.PageId == id.Value).SingleOrDefault();
			if (page == null)
			{
				AddUserMessage("Page not found", "Sorry, the page you are trying to Delete cannot be found. Page id: [" + id.Value + "] for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			Models.ViewModels.PageEditViewModel viewModel = new PageEditViewModel(page, isStoreAdminEdit: true, isReadOnly: true, isDeletePage: true, activeTab: Tab);
			return View("Delete", viewModel);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Pages_Delete)]
		public ActionResult DeleteConfirmed(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("PageId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Models.Page page = storeFront.Pages.Where(p => p.PageId == id.Value).SingleOrDefault();
			if (page == null)
			{
				AddUserMessage("Page not found", "Sorry, the page you are trying to Delete cannot be found. It may have been deleted already. Page id: [" + id.Value + "] for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			string pageName = page.Name;
			try
			{
				List<PageSection> sectionsToDelete = page.Sections.ToList();
				foreach (PageSection section in sectionsToDelete)
				{
					GStoreDb.PageSections.Delete(section);
				}
				bool deleted = GStoreDb.Pages.DeleteById(id.Value);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Page Deleted", "Page '" + pageName.ToHtml() + "' [" + id + "] was deleted successfully. Sections deleted: " + sectionsToDelete.Count + ".", AppHtmlHelpers.UserMessageType.Success);
					return RedirectToAction("Manager");
				}
				AddUserMessage("Page Delete Error", "There was an error deleting Page '" + pageName.ToHtml() + "' [" + id + "]. It may have already been deleted.", AppHtmlHelpers.UserMessageType.Warning);
				return RedirectToAction("Manager");

			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Page '" + pageName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Page Delete Error", errorMessage.ToHtml(), AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("Manager");
			}


		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Edit)]
		public virtual PartialViewResult UpdatePageAjax(int? PageId, GStore.Models.ViewModels.PageEditViewModel pageEditViewModel)
		{
			if (!PageId.HasValue)
			{
				return HttpBadRequestPartial("Page id parameter is null");
			}

			if (pageEditViewModel.PageId == 0)
			{
				return HttpBadRequestPartial("Page id in viewmodel is 0");
			}

			if (PageId.Value != pageEditViewModel.PageId)
			{
				return HttpBadRequestPartial("Page id mismatch. PageId Parameter " + PageId.Value + " != " + pageEditViewModel.PageId);
			}

			if (pageEditViewModel.PageTemplateId == 0)
			{
				ModelState.AddModelError("PageTemplateId", "Page Template must be selected");
			}

			if (pageEditViewModel.ThemeId == 0)
			{
				ModelState.AddModelError("ThemeId", "Page Theme must be selected");
			}

			if (string.IsNullOrWhiteSpace(pageEditViewModel.Url))
			{
				ModelState.AddModelError("Url", "Url is invalid. Example: '/' or '/Contact'");
			}
			else
			{
				if (pageEditViewModel.Url.Trim('/').Trim() == ".")
				{
					ModelState.AddModelError("Url", "Url '" + pageEditViewModel.Url.ToHtml() + "' is invalid. Example: '/' or '/Contact'");
				}
				if (pageEditViewModel.Url.Contains('?'))
				{
					ModelState.AddModelError("Url", "Url '" + pageEditViewModel.Url.ToHtml() + "' is invalid. Question mark is not allowed in URL. Example: '/' or '/Contact'");
				}
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidatePageUrl(this, pageEditViewModel.Url, storeFront.StoreFrontId, storeFront.ClientId, pageEditViewModel.PageId);

			if (urlIsValid && ModelState.IsValid)
			{

				Page page = null;
				try
				{
					page = GStoreDb.UpdatePage(pageEditViewModel, this, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Page Changes Saved!", "Page '" + page.Name.ToHtml() + "' [" + page.PageId + "] saved successfully for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);
					pageEditViewModel = new PageEditViewModel(page, isStoreAdminEdit: true, activeTab: pageEditViewModel.ActiveTab);
					return PartialView("_PageEditPartial", pageEditViewModel);
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while saving your changes to page '" + pageEditViewModel.Name + "' Url: '" + pageEditViewModel.Url + "' [" + pageEditViewModel.PageId + "] for Store Front: '" + storeFront.Name + "' [" + storeFront.StoreFrontId + "] \nError: '" + ex.GetType().FullName + "'";

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): '" + ex.ToString() + "'";
					}
					AddUserMessage("Error Saving Page!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Page Edit Error", "There was an error with your entry for page " + pageEditViewModel.Name.ToHtml() + " [" + pageEditViewModel.PageId + "] for Store Front '" + storeFront.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it.", AppHtmlHelpers.UserMessageType.Danger);
			}

			pageEditViewModel.IsStoreAdminEdit = true;
			pageEditViewModel.FillListsIfEmpty(CurrentClientOrThrow, CurrentStoreFrontOrThrow);
			return PartialView("_PageEditPartial", pageEditViewModel);
		}



	}
}
