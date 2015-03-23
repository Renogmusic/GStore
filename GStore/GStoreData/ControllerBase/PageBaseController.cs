using System;
using System.Linq;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData.ControllerBase
{
	/// <summary>
	/// 
	/// Dynamic page controller base class for common functionality
	/// </summary>
	public abstract class PageBaseController : GStoreData.ControllerBase.BaseController
    {
		protected override string ThemeFolderName
		{
			get
			{
				Models.Page page = CurrentPageOrNull;
				if (page == null)
				{
					return CurrentStoreFrontConfigOrThrow.DefaultNewPageTheme.FolderName;
				}
				return page.Theme.FolderName;
			}
		}

		public virtual ActionResult Display()
		{
			this._logActionsAsPageViews = true;

			Page currentPage = CurrentPageOrThrow;
			CheckAccessAndRedirect();

			//string rawUrl = Request.RawUrl;
			string viewName = currentPage.PageTemplate.ViewName;
			bool showPageEditLink = currentPage.StoreFront.Authorization_IsAuthorized(CurrentUserProfileOrNull, GStoreAction.Pages_Edit);
			SetMetaTags(currentPage);
			
			return View(viewName, new PageViewModel(CurrentPageOrThrow, showPageEditLink, false, false, false, false, null, false, ""));
		}

		/// <summary>
		/// Returns an HTML page inside the store front layout
		/// </summary>
		/// <returns></returns>
		public ActionResult HtmlFile(string path)
		{
			this._logActionsAsPageViews = true;
			Page currentPage = CurrentPageOrThrow;
			CheckAccessAndRedirect();

			string fullFilePath = currentPage.StoreFront.ChooseFilePath(currentPage.StoreFront.Client, "/Pages/" + path, Request.ApplicationPath, Server);
			if (fullFilePath == null)
			{
				return HttpNotFound("HTML File not Found for '" + path + "'");
			}

			//string rawUrl = Request.RawUrl;
			bool showPageEditLink = currentPage.StoreFront.Authorization_IsAuthorized(CurrentUserProfileOrNull, GStoreAction.Pages_Edit);
			SetMetaTags(currentPage);

			return View("HtmlFile", model: fullFilePath);
		}

		[HttpGet]
		[AuthorizeGStoreAction(true, GStoreAction.Pages_View, GStoreAction.Pages_Edit)]
		public virtual ActionResult Details(bool? AutoPost, string Tab)
		{
			this._logActionsAsPageViews = false;
			bool autoPost = true;
			if (AutoPost.HasValue)
			{
				autoPost = AutoPost.Value;
			}

			string rawUrl = Request.RawUrl;
			string viewName = CurrentPageOrThrow.PageTemplate.ViewName;
			bool showEditLink = CurrentStoreFrontOrThrow.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Pages_Edit);
			return View(viewName, new PageViewModel(CurrentPageOrThrow, showEditLink, true, false, false, false, null, false, Tab));
		}

		[HttpGet]
		[AuthorizeGStoreAction(GStoreAction.Pages_Edit)]
		public virtual ActionResult Edit(bool? AutoPost, string Tab, bool resetContentToDefault = false)
		{
			this._logActionsAsPageViews = false;
			Page page = CurrentPageOrThrow;
			bool autoPost = true;
			if (AutoPost.HasValue)
			{
				autoPost = AutoPost.Value;
			}

			if (resetContentToDefault)
			{
				page = page.ResetContentToDefault(this.GStoreDb);
				AddUserMessage("Content Reset", "Page content reset to defaults for page '" + page.Name.ToHtml() + "' [" + page.PageId + "]", UserMessageType.Info);
				return RedirectToAction("Edit", new { resetContentToDefault = "" });
			}

			string rawUrl = Request.RawUrl;
			string viewName = CurrentPageOrThrow.PageTemplate.ViewName;
			return View(viewName, new PageViewModel(CurrentPageOrThrow, false, false, true, autoPost, false, null, false, Tab));
		}

		/// <summary>
		/// Stub to prevent GET requests to form poster
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[ActionName("SubmitForm")]
		public virtual ActionResult SubmitFormBlank()
		{
			this._logActionsAsPageViews = true;
			return Display();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual ActionResult SubmitForm()
		{
			this._logActionsAsPageViews = true;
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

			if (FormProcessorExtensions.ProcessWebForm(this, page.WebForm, page, false, null))
			{
				string messageTitle = (string.IsNullOrEmpty(page.WebFormThankYouTitle) ? "Thank You!" : page.WebFormThankYouTitle);
				string messageBody = (string.IsNullOrEmpty(page.WebFormThankYouMessage) ? "Thank you for your information!" : page.WebFormThankYouMessage);

				messageBody = messageBody.ReplaceVariables(CurrentClientOrNull, CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrNull, CurrentPageOrNull, string.Empty);
				messageTitle = messageTitle.ReplaceVariables(CurrentClientOrNull, CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrNull, CurrentPageOrNull, string.Empty);

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
		[AuthorizeGStoreAction(GStoreAction.Pages_Edit)]
		public virtual PartialViewResult UpdatePageAjax(int? PageId, PageEditViewModel pageEditViewModel)
		{
			this._logActionsAsPageViews = false;
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
					pageEditViewModel = new PageEditViewModel(page, activeTab: pageEditViewModel.ActiveTab);
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
		[AuthorizeGStoreAction(GStoreAction.Pages_Edit)]
		public virtual PartialViewResult UpdateSectionAjax(PageSectionEditViewModel viewModel)
		{
			this._logActionsAsPageViews = false;
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
				throw new ApplicationException("Page is null");
			}

			if (page.ForRegisteredOnly && !User.IsRegistered())
			{
				AddUserMessage("Log in required", "Please log in to view this page. URL: " + Request.RawUrl.ToHtml(), UserMessageType.Danger);
				RedirectToAction("Login", "Account", new { returnUrl = Request.RawUrl }).ExecuteResult(this.ControllerContext);
			}

			if (page.ForAnonymousOnly && User.IsRegistered())
			{
				AddUserMessage("Page", "The page you tried to view is for Anonymous users only. Log off to view this page. URL: " + Request.RawUrl.ToHtml(), UserMessageType.Info);
				RedirectToAction("Index", "Profile", null).ExecuteResult(this.ControllerContext);
			}

		}

		protected Models.Page _page = null;

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
							if (Settings.AppEnableAutomaticHomePageCreation)
							{

								Models.Page newHomePage = GStoreDb.AutoCreateHomePage(Request, exDPNF.StoreFront.CurrentConfigOrAny(), this);
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

		protected void SetMetaTags(Page page)
		{
			if (page == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(page.MetaApplicationName))
			{
				_metaApplicationNameOverride = page.MetaApplicationName;
			}
			if (!string.IsNullOrEmpty(page.MetaApplicationTileColor))
			{
				_metaApplicationTileColorOverride = page.MetaApplicationTileColor;
			}
			if (!string.IsNullOrEmpty(page.MetaDescription))
			{
				_metaDescriptionOverride = page.MetaDescription;
			}
			if (!string.IsNullOrEmpty(page.MetaKeywords))
			{
				_metaKeywordsOverride = page.MetaKeywords;
			}
			if (!string.IsNullOrEmpty(page.BodyTopScriptTag))
			{
				_bodyTopScriptTagOverride = page.BodyTopScriptTag;
			}
			if (!string.IsNullOrEmpty(page.BodyBottomScriptTag))
			{
				_bodyBottomScriptTagOverride = page.BodyBottomScriptTag;
			}
		}
    }
}

