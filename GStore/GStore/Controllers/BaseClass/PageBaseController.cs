using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Models.Extensions;
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
			return View(viewName, new PageViewModel(CurrentPageOrThrow, showPageEditLink, false));
		}

		[HttpGet]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Edit)]
		public virtual ActionResult Edit()
		{
			string rawUrl = Request.RawUrl;
			string viewName = CurrentPageOrThrow.PageTemplate.ViewName;
			return View(viewName, new PageViewModel(CurrentPageOrThrow, false, true));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.Pages_Edit)]
		public virtual ActionResult Edit(PageSectionEditViewModel viewModel)
		{
			string rawUrl = Request.RawUrl;

			if (ModelState.IsValid)
			{
				if (viewModel.PageSectionId == null)
				{
					//create page section
					PageSection newSection = GStoreDb.CreatePageSection(viewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
				}
				else
				{
					//update existing section
					PageSection updatedSection = GStoreDb.UpdatePageSection(viewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
				}
			}

			return Edit();
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

