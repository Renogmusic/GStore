using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Data;
using GStore.Models.ViewModels;
using GStore.AppHtmlHelpers;
using GStore.Models;

namespace GStore.Controllers
{
    public class CatalogController : BaseClass.BaseController
    {
        // GET: Catalog
		public ActionResult Index()
		{
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(User.Identity.IsAuthenticated), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, null, null);
			return View("Index", "_Layout_Default", model);
		}

		public ActionResult ViewCategoryByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewCategory, "Bad Url", false);
				return Index();
			}

			Models.ProductCategory category = CurrentStoreFrontOrThrow.ProductCategories.AsQueryable().Where(cat => cat.UrlName.ToLower() == urlName.ToLower()).WhereIsActive().SingleOrDefault();
			if (category == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewCategoryNotFound, urlName, false, categoryUrlName: urlName);
				return CategoryNotFound(urlName);
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewCategory, urlName, true, categoryUrlName: urlName);
			return ViewCategory(category);
		}

		public ActionResult ViewProductByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewProduct, "Bad Url", false);
				return Index();
			}

			Models.Product product = CurrentStoreFrontOrThrow.Products.AsQueryable().Where(prod => prod.UrlName.ToLower() == urlName.ToLower()).WhereIsActive().SingleOrDefault();
			if (product == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewProductNotFound, urlName, false, productUrlName: urlName);
				return ProductNotFound(urlName);
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewProduct, urlName, false, productUrlName: urlName);
			return ViewProduct(product);
		}

		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.CatalogLayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.CatalogTheme.FolderName;
			}
		}

		protected ActionResult ViewCategory(Models.ProductCategory category)
		{
			if (category == null)
			{
				throw new ApplicationException("Category is null, be sure category is set before calling ViewCategory");
			}
			/// get current catalog item

			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(User.Identity.IsAuthenticated), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, category, null);

			//get products

			return View("ViewCategory", model);
		}

		protected ActionResult ViewProduct(Models.Product product)
		{
			if (product == null)
			{
				throw new ApplicationException("Product is null, be sure product is set before calling ViewProduct");
			}
			/// get current catalog item
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(User.Identity.IsAuthenticated), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, product.Category, product);
			return View("ViewProduct", model);
		}

		protected ActionResult ProductNotFound(string productName)
		{
			if (string.IsNullOrWhiteSpace(productName))
			{
				productName = "The product you linked to";
			}
			AddUserMessage("Sorry!", productName.ToHtml() + " was not found. Here is a list of our current products.", GStore.AppHtmlHelpers.UserMessageType.Info);

			return Index();
		}

		protected ActionResult CategoryNotFound(string categoryName)
		{
			if (string.IsNullOrWhiteSpace(categoryName))
			{
				categoryName = "The category you linked to";
			}
			AddUserMessage("Sorry!", categoryName.ToHtml() + " was not found. Here is a list of our current products.", GStore.AppHtmlHelpers.UserMessageType.Info);
			return Index();
		}

    }
}