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
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActiveForCatalogList(User.Identity.IsAuthenticated), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, null, null);
			return View("Index", this.LayoutNameForCatalog, model);
		}

		public ActionResult ViewCategoryByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewCategory, "Bad Url", false);
				return Index();
			}

			Models.ProductCategory category = CurrentStoreFrontOrThrow.ProductCategories.AsQueryable()
				.WhereRegisteredAnonymousCheck(User.Identity.IsAuthenticated)
				.Where(cat => cat.UrlName.ToLower() == urlName.ToLower())
				.WhereIsActive()
				.WhereShowInCatalogByName()
				.SingleOrDefault();

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

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.CatalogTheme.FolderName;
			}
		}

		protected string LayoutNameForCatalog
		{
			get
			{
				return "_Catalog_Layout_" + CurrentStoreFrontConfigOrThrow.CatalogLayout.ToString();
			}
		}

		protected ActionResult ViewCategory(Models.ProductCategory category)
		{
			if (category == null)
			{
				throw new ApplicationException("Category is null, be sure category is set before calling ViewCategory");
			}
			/// get current catalog item

			List<TreeNode<ProductCategory>> categoryTree = CurrentStoreFrontOrThrow.CategoryTreeWhereActiveForCatalogByName(User.Identity.IsAuthenticated);

			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, categoryTree, CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, category, null);

			if (category.Theme != null)
			{
				ViewData.Theme(category.Theme);
			}
			return View("ViewCategory", this.LayoutNameForCatalog, model);
		}

		protected ActionResult ViewProduct(Models.Product product)
		{
			if (product == null)
			{
				throw new ApplicationException("Product is null, be sure product is set before calling ViewProduct");
			}
			/// get current catalog item
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActiveForCatalogByName(User.Identity.IsAuthenticated), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, product.Category, product);

			if (product.Theme != null)
			{
				ViewData.Theme(product.Theme);
			}
			else if (product.Category.Theme != null)
			{
				ViewData.Theme(product.Category.Theme);
			}
			return View("ViewProduct", this.LayoutNameForCatalog, model);
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