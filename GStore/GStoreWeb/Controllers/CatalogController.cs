using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreWeb.Controllers
{
	public class CatalogController : AreaBaseController.RootAreaBaseController
    {
        // GET: Catalog
		public ActionResult Index()
		{
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActiveForCatalogList(User.IsRegistered()), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, null, null, null, null);
			return View("Index", this.LayoutNameForCatalog, model);
		}

		public ActionResult ViewCategoryByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewCategory, "Bad Url", false);
				return Index();
			}

			ProductCategory category = CurrentStoreFrontOrThrow.ProductCategories.AsQueryable()
				.WhereRegisteredAnonymousCheck(User.IsRegistered())
				.Where(cat => cat.UrlName.ToLower() == urlName.ToLower())
				.WhereIsActive()
				.WhereShowInCatalogByName(User.IsRegistered())
				.SingleOrDefault();

			if (category == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewCategoryNotFound, urlName, false, categoryUrlName: urlName);
				return CategoryNotFound(urlName);
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewCategory, urlName, true, categoryUrlName: urlName);

			SetMetaTags(category);

			return ViewCategory(category);
		}

		public ActionResult ViewProductByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewProduct, "Bad Url", false);
				return Index();
			}

			Product product = CurrentStoreFrontOrThrow.Products.AsQueryable().Where(prod => prod.UrlName.ToLower() == urlName.ToLower()).WhereIsActive().SingleOrDefault();
			if (product == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewProductNotFound, urlName, false, productUrlName: urlName);
				return ProductNotFound(urlName);
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewProduct, urlName, true, productUrlName: urlName);

			SetMetaTags(product);

			return ViewProduct(product);
		}

		public ActionResult ViewBundleByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewBundle, "Bad Url", false);
				return Index();
			}

			ProductBundle bundle = CurrentStoreFrontOrThrow.ProductBundles.AsQueryable().Where(prod => prod.UrlName.ToLower() == urlName.ToLower()).WhereIsActive().SingleOrDefault();
			if (bundle == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewBundleNotFound, urlName, false, productBundleUrlName: urlName);
				return BundleNotFound(urlName);
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Catalog, UserActionActionEnum.Catalog_ViewBundle, urlName, true, productBundleUrlName: urlName);

			SetMetaTags(bundle);

			return ViewBundle(bundle);
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
				return CurrentStoreFrontConfigOrThrow.CatalogLayout.ToString() + "/_Layout";
			}
		}

		protected ActionResult ViewCategory(ProductCategory category)
		{
			if (category == null)
			{
				throw new ApplicationException("Category is null, be sure category is set before calling ViewCategory");
			}
			/// get current catalog item

			List<TreeNode<ProductCategory>> categoryTree = CurrentStoreFrontOrThrow.CategoryTreeWhereActiveForCatalogByName(User.IsRegistered());

			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, categoryTree, CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, category, null, null, null);

			if (category.Theme != null)
			{
				ViewData.Theme(category.Theme);
			}
			return View("ViewCategory", this.LayoutNameForCatalog, model);
		}

		protected ActionResult CategoryNotFound(string categoryUrlName)
		{
			if (string.IsNullOrWhiteSpace(categoryUrlName))
			{
				categoryUrlName = "The category you linked to";
			}
			AddUserMessage("Sorry!", categoryUrlName.ToHtml() + " was not found. Here is a list of our current products.", UserMessageType.Info);
			return Index();
		}

		protected ActionResult ViewProduct(Product product)
		{
			if (product == null)
			{
				throw new ApplicationException("Product is null, be sure product is set before calling ViewProduct");
			}
			/// get current catalog item
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActiveForCatalogByName(User.IsRegistered()), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, product.Category, product, null, null);

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

		protected ActionResult ProductNotFound(string productUrlName)
		{
			if (string.IsNullOrWhiteSpace(productUrlName))
			{
				productUrlName = "The product you linked to";
			}
			AddUserMessage("Sorry!", productUrlName.ToHtml() + " was not found. Here is a list of our current products.", UserMessageType.Info);

			return Index();
		}

		protected ActionResult ViewBundle(ProductBundle bundle)
		{
			if (bundle == null)
			{
				throw new ApplicationException("Product Bundle is null, be sure bundle is set before calling ViewBundle");
			}
			/// get current catalog item
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActiveForCatalogByName(User.IsRegistered()), CurrentStoreFrontConfigOrThrow.CatalogPageInitialLevels, bundle.Category, null, bundle, null);

			if (bundle.Theme != null)
			{
				ViewData.Theme(bundle.Theme);
			}
			else if (bundle.Category.Theme != null)
			{
				ViewData.Theme(bundle.Category.Theme);
			}
			return View("ViewBundle", this.LayoutNameForCatalog, model);
		}

		protected ActionResult BundleNotFound(string bundleUrlName)
		{
			if (string.IsNullOrWhiteSpace(bundleUrlName))
			{
				bundleUrlName = "The product bundle you linked to";
			}
			AddUserMessage("Sorry!", bundleUrlName.ToHtml() + " was not found. Here is a list of our current products.", UserMessageType.Info);

			return Index();
		}

		protected void SetMetaTags(ProductCategory category)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			_metaDescriptionOverride = category.MetaDescriptionOrSystemDefault(config);
			_metaKeywordsOverride = category.MetaKeywordsOrSystemDefault(config);
		}

		protected void SetMetaTags(Product product)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			_metaDescriptionOverride = product.MetaDescriptionOrSystemDefault(config);
			_metaKeywordsOverride = product.MetaKeywordsOrSystemDefault(config);
		}

		protected void SetMetaTags(ProductBundle bundle)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			_metaDescriptionOverride = bundle.MetaDescriptionOrSystemDefault(config);
			_metaKeywordsOverride = bundle.MetaKeywordsOrSystemDefault(config);
		}

	}
}
