using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Data;
using GStore.Models.ViewModels;
using GStore.Models;
using GStore.Identity;

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

		public virtual ActionResult Display()
		{
			string rawUrl = Request.RawUrl;
			string viewName = CurrentPageOrThrow.PageTemplate.ViewName;
			bool showPageEditLink = CurrentStoreFrontOrThrow.Authorization_IsAuthorized(CurrentUserProfileOrNull, GStoreAction.Pages_Edit);
			return View(viewName, new PageViewModel(CurrentPageOrThrow, showPageEditLink, false, false));
		}

		[HttpGet]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Edit)]
		public virtual ActionResult Edit(bool? AutoPost)
		{
			bool autoPost = true;
			if (AutoPost.HasValue)
			{
				autoPost = AutoPost.Value;
			}

			string rawUrl = Request.RawUrl;
			string viewName = CurrentPageOrThrow.PageTemplate.ViewName;
			return View(viewName, new PageViewModel(CurrentPageOrThrow, false, true, autoPost));
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
					pageSection = GStoreDb.UpdatePageSection(viewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
					if (!quietOnSave)
					{
						AddUserMessage("Changes Saved!", "Page Section saved successfully", AppHtmlHelpers.UserMessageType.Success);
					}
				}
				else
				{
					pageSection = GStoreDb.CreatePageSection(viewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
					if (!quietOnSave)
					{
						AddUserMessage("Changes Saved!", "Page Section created successfully", AppHtmlHelpers.UserMessageType.Success);
					}
				}

				viewModel = new PageSectionEditViewModel(pageSection.PageTemplateSection, pageSection.Page, pageSection, viewModel.Index, viewModel.AutoSubmit);
				return PartialView("_SectionEditPartial", viewModel);
			}
			else
			{
				AddUserMessage("Error", "There was an error with your entry. Please correct it.", AppHtmlHelpers.UserMessageType.Danger);
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
						throw dbEx;
					}
					catch (Exceptions.StoreFrontInactiveException exSFI)
					{
						//storefront is inactive
						System.Diagnostics.Trace.WriteLine("--StoreFrontInactive in CurrentPage: " + exSFI.Message);
						throw exSFI;
					}
					catch (Exceptions.NoMatchingBindingException exNMB)
					{
						//no store found
						System.Diagnostics.Trace.WriteLine("--StoreFrontInactive in CurrentPage: " + exNMB.Message);
						throw exNMB;
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

