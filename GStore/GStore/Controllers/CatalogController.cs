using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Data;
using GStore.Models.ViewModels;

namespace GStore.Controllers
{
    public class CatalogController : BaseClass.BaseController
    {
        // GET: Catalog
		public ActionResult Index()
		{
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(), CurrentStoreFrontOrThrow.CatalogPageInitialLevels, null, null);
			return View("Index", model);
		}

		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontOrThrow.CatalogLayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontOrThrow.CatalogTheme.FolderName;
			}
		}


		public ActionResult ViewCategoryByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				return Index();
			}

			Models.ProductCategory category = CurrentStoreFrontOrThrow.ProductCategories.AsQueryable().Where(cat => cat.UrlName.ToLower() == urlName.ToLower()).WhereIsActive().SingleOrDefault();
			if (category == null)
			{
				return CategoryNotFound(urlName);
			}

			return ViewCategory(category);
		}

		public ActionResult ViewCategoryById(int? id)
		{

			if (!id.HasValue)
			{
				return Index();
			}

			Models.ProductCategory category = CurrentStoreFrontOrThrow.ProductCategories.AsQueryable().Where(cat => cat.ProductCategoryId == id.Value).WhereIsActive().SingleOrDefault();
			if (category == null)
			{
				return CategoryNotFound("Category Id: " + id);
			}

			return ViewCategory(category);
		}

		public ActionResult ViewProductByName(string urlName)
		{
			if (string.IsNullOrWhiteSpace(urlName))
			{
				return Index();
			}

			Models.Product product = CurrentStoreFrontOrThrow.Products.AsQueryable().Where(prod => prod.UrlName.ToLower() == urlName.ToLower()).WhereIsActive().SingleOrDefault();
			if (product == null)
			{
				return ProductNotFound(urlName);
			}

			return ViewProduct(product);
		}

		public ActionResult ViewProductById(int? id)
		{
			if (!id.HasValue)
			{
				return Index();
			}

			Models.Product product = CurrentStoreFrontOrThrow.Products.AsQueryable().Where(prod => prod.ProductId == id.Value).WhereIsActive().SingleOrDefault();
			if (product == null)
			{
				return ProductNotFound("Product Id: " + id);
			}

			return ViewProduct(product);

		}

		protected ActionResult ViewCategory(Models.ProductCategory category)
		{
			if (category == null)
			{
				throw new ApplicationException("Category is null, be sure category is set before calling ViewCategory");
			}
			/// get current catalog item

			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(), CurrentStoreFrontOrThrow.CatalogPageInitialLevels, category, null);

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
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(), CurrentStoreFrontOrThrow.CatalogPageInitialLevels, product.Category, product);
			return View("ViewProduct", model);
		}

		protected ActionResult ProductNotFound(string productName)
		{
			if (string.IsNullOrWhiteSpace(productName))
			{
				productName = "The product you linked to";
			}
			AddUserMessage("Sorry!", productName + " was not found. Here is a list of our current products.", GStore.AppHtmlHelpers.UserMessageType.Info);
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(), CurrentStoreFrontOrThrow.CatalogPageInitialLevels, null, null);
			return View("Index", model);
		}

		protected ActionResult CategoryNotFound(string categoryName)
		{
			if (string.IsNullOrWhiteSpace(categoryName))
			{
				categoryName = "The category you linked to";
			}
			AddUserMessage("Sorry!", categoryName + " was not found. Here is a list of our current products.", GStore.AppHtmlHelpers.UserMessageType.Info);
			CatalogViewModel model = new CatalogViewModel(CurrentStoreFrontOrThrow, CurrentStoreFrontOrThrow.CategoryTreeWhereActive(), CurrentStoreFrontOrThrow.CatalogPageInitialLevels, null, null);
			return View("Index", model);
		}

    }
}