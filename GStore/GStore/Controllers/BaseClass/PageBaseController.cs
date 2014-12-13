﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Data;
using GStore.Models.ViewModels;
using GStore.Models;
using GStore.Identity;
using GStore.AppHtmlHelpers;

namespace GStore.Controllers.BaseClass
{
	/// <summary>
	/// 
	/// Dynamic page controller base class for common functionality
	/// </summary>
    public abstract class PageBaseController : BaseController
    {
		Models.Page _page = null;
		protected override string LayoutName
		{
			get
			{
				Models.Page page = CurrentPageOrNull;
				if (page == null)
				{
					throw new ApplicationException("StoreFront error; unknown layout");
				}
				return page.PageTemplate.LayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				Models.Page page = CurrentPageOrNull;
				if (page == null)
				{
					throw new ApplicationException("StoreFront error; unknown theme");
				}
				return page.Theme.FolderName;
			}
		}

		public virtual ActionResult Display()
		{
			Page currentPage = CurrentPageOrThrow;
			CheckAccessAndRedirect();
			string rawUrl = Request.RawUrl;
			string viewName = CurrentPageOrThrow.PageTemplate.ViewName;
			bool showPageEditLink = CurrentStoreFrontOrThrow.Authorization_IsAuthorized(CurrentUserProfileOrNull, GStoreAction.Pages_Edit);
			return View(viewName, new PageViewModel(CurrentPageOrThrow, showPageEditLink, false, false, false, null, false, ""));
		}

		[HttpGet]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Edit)]
		public virtual ActionResult Edit(bool? AutoPost, string Tab)
		{
			bool autoPost = true;
			if (AutoPost.HasValue)
			{
				autoPost = AutoPost.Value;
			}

			string rawUrl = Request.RawUrl;
			string viewName = CurrentPageOrThrow.PageTemplate.ViewName;
			return View(viewName, new PageViewModel(CurrentPageOrThrow, false, true, autoPost, false, null, false, Tab));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult SubmitForm()
		{
			CheckAccessAndRedirect();
			Page page = CurrentPageOrThrow;
			if (page.WebForm == null)
			{
				return HttpBadRequest("There is no form for page '" + page.Name + "' [" + page.PageId + "]");
			}
			if (!page.WebForm.IsActiveBubble())
			{
				return HttpBadRequest("Form is inactive for page '" + page.Name + "' [" + page.PageId + "]");
			}

			if (FormProcessorExtensions.ProcessWebForm(this.GStoreDb, this.ModelState, page.WebForm, page, CurrentUserProfileOrNull, Request, page.WebFormProcessorType))
			{
				string messageTitle = (string.IsNullOrEmpty(page.WebFormThankYouTitle) ? "Thank You!" : page.WebFormThankYouTitle);
				string messageBody = (string.IsNullOrEmpty(page.WebFormThankYouMessage) ? "Thank you for your information!" : page.WebFormThankYouMessage);

				messageBody = messageBody.ReplaceVariables(string.Empty, CurrentClientOrNull, CurrentStoreFrontOrNull, CurrentUserProfileOrNull, CurrentPageOrNull);
				messageTitle = messageTitle.ReplaceVariables(string.Empty, CurrentClientOrNull, CurrentStoreFrontOrNull, CurrentUserProfileOrNull, CurrentPageOrNull);

				ModelState.Clear();

				AddUserMessage(messageTitle.ToHtml(), messageBody, UserMessageType.Success);
				if (page.WebFormSuccessPageId == null)
				{
					return Display();
				}
				Page redirectPageTarget = CurrentStoreFrontOrThrow.Pages.SingleOrDefault(p => p.PageId == page.WebFormSuccessPageId.Value);
				if (redirectPageTarget == null)
				{
					throw new ApplicationException("Success Page not found. page.WebFormSuccessPageId: " + page.WebFormSuccessPageId);
				}
				if (redirectPageTarget.IsActiveBubble())
				{
					return Redirect(redirectPageTarget.UrlResolved(Url));
				}
				return Display();
			}
			//re-display form if process error
			AddUserMessage("Form Error", "There was a problem with your form values. Please correct the errors below.", UserMessageType.Danger);
			return Display();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Edit)]
		public virtual PartialViewResult UpdatePageAjax(int? PageId, PageEditViewModel pageEditViewModel)
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
				string pageTemplateKey = (ModelState.ContainsKey("PageEditViewModel_PageTemplateId") ? "PageEditViewModel_PageTemplateId" : "PageTemplateId");
				ModelState.AddModelError(pageTemplateKey, "Page Template must be selected");
			}

			if (pageEditViewModel.ThemeId == 0)
			{
				string themeKey = (ModelState.ContainsKey("PageEditViewModel_ThemeId") ? "PageEditViewModel_ThemeId" : "ThemeId");
				ModelState.AddModelError(themeKey, "Page Theme must be selected");
			}

			if (string.IsNullOrWhiteSpace(pageEditViewModel.Url))
			{
				string urlKey = (ModelState.ContainsKey("PageEditViewModel_Url") ? "PageEditViewModel_Url" : "Url");
				ModelState.AddModelError(urlKey, "Url cannot be blank. Example: '/' or '/Contact'");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidatePageUrl(this, pageEditViewModel.Url, storeFront.StoreFrontId, storeFront.ClientId, pageEditViewModel.PageId);

			if (urlIsValid && ModelState.IsValid)
			{
				try
				{
					Page page = null;
					page = GStoreDb.UpdatePage(pageEditViewModel, this, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
					AddUserMessage("Page Changes Saved!", "Page '" + page.Name.ToHtml() + "' [" + page.PageId + "] saved successfully", AppHtmlHelpers.UserMessageType.Success);
					pageEditViewModel = new PageEditViewModel(page);
					return PartialView("_PageEditPartial", pageEditViewModel);
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while saving your changes to page '" + pageEditViewModel.Name + "' Url: '" + pageEditViewModel.Url + "' [" + pageEditViewModel.PageId + "] \nError: '" + ex.GetType().FullName + "'";

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += "\nException.ToString(): '" + ex.ToString() + "'";
					}
					AddUserMessage("Error Saving Page!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Page Edit Error", "There was an error with your entry for page " + pageEditViewModel.Name.ToHtml() + " [" + pageEditViewModel.PageId + "]. Please correct it.", AppHtmlHelpers.UserMessageType.Danger);
			}

			pageEditViewModel.FillListsIfEmpty(CurrentClientOrThrow, CurrentStoreFrontOrThrow);
			return PartialView("_PageEditPartial", pageEditViewModel);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Edit)]
		public virtual PartialViewResult UpdateSectionAjax(PageSectionEditViewModel viewModel)
		{
			bool quietOnSave = false;
			if (viewModel.IsAutoPosted)
			{
				quietOnSave = true;
			}
			if (viewModel.Index == 0)
			{
				ModelState.AddModelError("Index", "Index cannot be 0");
			}
			if (ModelState.IsValid)
			{
				PageSection pageSection = null;
				if (viewModel.PageSectionId.HasValue)
				{
					try
					{
						pageSection = GStoreDb.UpdatePageSection(viewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
						if (!quietOnSave)
						{
							AddUserMessage("Section Changes Saved!", "Page Section saved successfully", AppHtmlHelpers.UserMessageType.Success);
						}
						viewModel = new PageSectionEditViewModel(pageSection.PageTemplateSection, pageSection.Page, pageSection, viewModel.Index, viewModel.AutoSubmit);
						return PartialView("_SectionEditPartial", viewModel);
					}
					catch (Exception ex)
					{
						string errorMessage = "An error occurred while saving your changes to page section '" + viewModel.PageSectionId.ToString() + " \nError: '" + ex.GetType().FullName + "'";

						if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
						{
							errorMessage += " \nException.ToString(): '" + ex.ToString() + "'";
						}
						AddUserMessage("Error Updating Page Section [" + viewModel.PageSectionId + "]!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
						ModelState.AddModelError("Ajax", errorMessage);
					}
				}
				else
				{
					try
					{
						pageSection = GStoreDb.CreatePageSection(viewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
						if (!quietOnSave)
						{
							AddUserMessage("Section Changes Saved!", "Page Section '" + pageSection.PageTemplateSection.Name.ToHtml() + "' [" + pageSection.PageSectionId + "] created successfully", AppHtmlHelpers.UserMessageType.Success);
						}
						viewModel = new PageSectionEditViewModel(pageSection.PageTemplateSection, pageSection.Page, pageSection, viewModel.Index, viewModel.AutoSubmit);
						return PartialView("_SectionEditPartial", viewModel);
					}
					catch (Exception ex)
					{
						string errorMessage = "An error occurred while creating page section '" + viewModel.PageSectionId.ToString()
							+ "' for Page Template Section [" + viewModel.PageTemplateSectionId + "] \nError: '" + ex.GetType().FullName + "'";

						if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
						{
							errorMessage += " \nException.ToString(): '" + ex.ToString() + "'";
						}
						AddUserMessage("Error Creating Page Section [" + viewModel.PageSectionId + "]!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
						ModelState.AddModelError("Ajax", errorMessage);
					}
				}
			}
			else
			{
				AddUserMessage("Section Update Error", "There was an error with your entry. Please correct it.", AppHtmlHelpers.UserMessageType.Danger);
			}
			
			return PartialView("_SectionEditPartial", viewModel);
		}

		public Models.Page CurrentPageOrNull
		{
			get
			{
				try
				{
					if (_currentStoreFrontError)
					{
						return null;
					}
					return CurrentPageOrThrow;
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public void CheckAccessAndRedirect()
		{
			Page page = this.CurrentPageOrNull;
			if (page == null)
			{
				//null page
			}

			if (page.ForRegisteredOnly && !User.Identity.IsAuthenticated)
			{
				AddUserMessage("Log in required", "Please log in to view this page", UserMessageType.Danger);
				RedirectToAction("Login", "Account", null).ExecuteResult(this.ControllerContext);
			}
		}

		public Models.Page CurrentPageOrThrow
		{
			get
			{
				if (_page == null)
				{
					try
					{
						_page = CurrentStoreFrontOrThrow.GetCurrentPage(Request);
					}
					catch(Exceptions.DynamicPageInactiveException exDPI)
					{
						if (exDPI.IsHomePage)
						{
							System.Diagnostics.Debug.Print("--Dynamic Page Inactive Exception in dynamic url: " + Request.RawUrl);
							System.Diagnostics.Debug.Print("--" + exDPI.ToString());
						}
						string message = exDPI.Message;
						HttpNotFound((exDPI.IsHomePage ? "Home" : string.Empty) + "Page is Inactive for url: " + exDPI.Url);
					}
					catch(Exceptions.DynamicPageNotFoundException exDPNF)
					{
						if (exDPNF.IsHomePage)
						{
							System.Diagnostics.Debug.Print("--Dynamic Page Not Found Exception in dynamic url: " + Request.RawUrl);
							System.Diagnostics.Debug.Print("--" + exDPNF.ToString());
							if (Properties.Settings.Current.AppEnableAutomaticHomePageCreation)
							{

								Models.Page newHomePage = GStoreDb.AutoCreateHomePage(Request, exDPNF.StoreFront, this);
								_page = CurrentStoreFrontOrThrow.GetCurrentPage(Request);
								return _page;
							}
						}
						HttpNotFound((exDPNF.IsHomePage ? "Home" : string.Empty) + "Page Not Found for url: " + exDPNF.Url);
					}
					catch (Exceptions.DatabaseErrorException dbEx)
					{
						System.Diagnostics.Trace.WriteLine("--Database Error in CurrentPage: " + dbEx.Message);
						System.Diagnostics.Trace.Indent();
						System.Diagnostics.Trace.WriteLine("-- inner exception: " + dbEx.InnerException.ToString());
						System.Diagnostics.Trace.Unindent();
						throw;
					}
					catch (Exceptions.StoreFrontInactiveException exSFI)
					{
						//storefront is inactive
						System.Diagnostics.Trace.WriteLine("--StoreFrontInactive in CurrentPage: " + exSFI.Message);
						throw;
					}
					catch (Exceptions.NoMatchingBindingException exNMB)
					{
						//no store found
						System.Diagnostics.Trace.WriteLine("--StoreFrontInactive in CurrentPage: " + exNMB.Message);
						throw;
					}
					catch (Exception ex)
					{
						throw new ApplicationException("Error getting active dynamic page", ex);
					}
				}
				return _page;
			}
		}
    }
}

