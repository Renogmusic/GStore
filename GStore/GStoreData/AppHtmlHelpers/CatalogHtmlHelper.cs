using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GStoreData.Models;
using GStoreData.ViewModels;
using System.Web.Mvc.Html;

namespace GStoreData.AppHtmlHelpers
{
	//Catalog file structure
	//
	// --Root
	// Index - views top of catalog
	// ViewCategory - views details about a specific category
	// ViewBundle - views details about a specific bundle
	// ViewProduct - views Details about a specific product
	//
	// --Layout Folders
	// [Layout]/_Layout - Layout shell for this layout
	// [Layout]/_BundleDetails_Partial  - Bundle details for this layout
	// [Layout]/_CategoryDetails - Category details for this layout
	// [Layout]/_ProductDetails - Product details for this layout

	// [Layout]/_ProductList_Partial - Lists products in this category for this layout
	// [Layout]/_BundleList_Partial - Lists bundles in this category for this layout
	// [Layout]/_CategoryList_Partial - Lists products in this category for this layout


	/// <summary>
	/// 
	/// </summary>
	public static class CatalogHtmlHelper
	{
		/// <summary>
		/// Displays the top of the product category (root of catalog)
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogTopPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			string view = "_Top_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Displays list of categories for the root (top) category list page
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogTopCategoryListPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			string view = "_Top_CategoryList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Displays the category details display partial using Model.CurrentCategoryOrNull.CategoryDetailTemplate
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogCategoryDetailsPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentCategoryOrNull == null)
			{
				throw new ApplicationException("Category is null for RenderCatalogCategoryDetailsPartial. htmlHelper.ViewData.Model.CurrentCategoryOrNull = null");
			}

			string view = "_CategoryDetails_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Displays the bundle details display partial using the Model.CurrentProductBundleOrNull.ProductBundleDetailTemplateOrCategory()
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogBundleDetailsPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentProductBundleOrNull == null)
			{
				throw new ApplicationException("Bundle is null for RenderCatalogBundleDetailsPartial. htmlHelper.ViewData.Model.CurrentProductBundleOrNull = null");
			}

			string view = "_BundleDetails_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Displays the Product details display partial using the htmlHelper.ViewData.Model.CurrentProductOrNull.ProductDetailTemplateOrCategory()
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogProductDetailsPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentProductOrNull == null)
			{
				throw new ApplicationException("Product is null for RenderCatalogProductDetailsPartial. htmlHelper.ViewData.Model.CurrentProductOrNull = null");
			}

			string view = "_ProductDetails_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		public static void RenderCatalogProductDetailsPartialForBundleItem(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentProductOrNull == null)
			{
				throw new ApplicationException("Product is null for RenderCatalogProductDetailsPartialForBundleItem. htmlHelper.ViewData.Model.CurrentProductOrNull = null");
			}

			bool? oldValueIsBundlePage = htmlHelper.ViewData["isBundlePage"] as bool?;

			ViewDataDictionary<CatalogViewModel> newViewData = new ViewDataDictionary<CatalogViewModel>(htmlHelper.ViewData);
			newViewData["isBundlePage"] = true;
			string view = "_ProductDetails_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, newViewData);
			
			htmlHelper.ViewData["isBundlePage"] = oldValueIsBundlePage;
		}


		/// <summary>
		/// Renders sub-category partial view for child categories
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogCategoryListPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentCategoryOrNull == null)
			{
				throw new ApplicationException("Category is null for RenderCatalogCategoryListPartial. htmlHelper.ViewData.Model.CurrentCategoryOrNull = null");
			}

			string view = "_CategoryList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Renders a product partial to display the product in a list of products for the current category
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="bundle"></param>
		public static void RenderCatalogProductForListPartial(this HtmlHelper<CatalogViewModel> htmlHelper, Product product)
		{
			CatalogViewModel viewModel = htmlHelper.ViewData.Model;

			Product oldValueCurrentProductOrNull = htmlHelper.ViewData.Model.CurrentProductOrNull;

			ViewDataDictionary<CatalogViewModel> newViewData = new ViewDataDictionary<CatalogViewModel>(htmlHelper.ViewData);
			newViewData.Model.CurrentProductOrNull = product;

			string view = "_ProductForList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, newViewData);

			htmlHelper.ViewData.Model.CurrentProductOrNull = oldValueCurrentProductOrNull;
		}


		/// <summary>
		/// Renders bundle partial view for a bundle in the current category
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogBundleListPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentCategoryOrNull == null)
			{
				throw new ApplicationException("Category is null for RenderCatalogBundleListPartial. htmlHelper.ViewData.Model.CurrentCategoryOrNull = null");
			}

			string view = "_BundleList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Renders a bundle partial to display the bundle in a list of bundles for the current category
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="bundle"></param>
		public static void RenderCatalogBundleForListPartial(this HtmlHelper<CatalogViewModel> htmlHelper, ProductBundle bundle)
		{
			CatalogViewModel viewModel = htmlHelper.ViewData.Model;

			ProductBundle oldValueProductBundle = htmlHelper.ViewData.Model.CurrentProductBundleOrNull;

			ViewDataDictionary<CatalogViewModel> newViewData = new ViewDataDictionary<CatalogViewModel>(htmlHelper.ViewData);
			newViewData.Model.CurrentProductBundleOrNull = bundle;

			string view = "_BundleForList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, newViewData);

			htmlHelper.ViewData.Model.CurrentProductBundleOrNull = oldValueProductBundle;
		}

		/// <summary>
		/// Renders product list partial view for products in the current category
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogProductListPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentCategoryOrNull == null)
			{
				throw new ApplicationException("Category is null for RenderCatalogProductListPartial. htmlHelper.ViewData.Model.CurrentCategoryOrNull = null");
			}

			string view = "_ProductList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Renders bundle item list partial view for the current bundle
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogBundleItemListPartial(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			if (htmlHelper.ViewData.Model.CurrentProductBundleOrNull == null)
			{
				throw new ApplicationException("Bundle is null for RenderCatalogBundleItemListPartial. htmlHelper.ViewData.Model.CurrentProductBundleOrNull = null");
			}

			string view = "_BundleItemList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		/// <summary>
		/// Renders bundle item list partial view for the current bundle
		/// </summary>
		/// <param name="htmlHelper"></param>
		public static void RenderCatalogBundleItemForListPartial(this HtmlHelper<CatalogViewModel> htmlHelper, ProductBundleItem item)
		{
			CatalogViewModel viewModel = htmlHelper.ViewData.Model;

			ProductBundleItem oldValueProductBundleItem = htmlHelper.ViewData.Model.CurrentProductBundleItemOrNull;
			Product oldValueProduct = htmlHelper.ViewData.Model.CurrentProductOrNull;

			ViewDataDictionary<CatalogViewModel> newViewData = new ViewDataDictionary<CatalogViewModel>(htmlHelper.ViewData);

			newViewData.Model.CurrentProductBundleItemOrNull = item;
			newViewData.Model.CurrentProductOrNull = item.Product;

			string view = "_BundleItemForList_Partial";
			htmlHelper.RenderCatalogPartialHelper(view, newViewData);

			htmlHelper.ViewData.Model.CurrentProductBundleItemOrNull = oldValueProductBundleItem;
			htmlHelper.ViewData.Model.CurrentProductOrNull = oldValueProduct;
		}

		/// <summary>
		/// Displays a catalog partial view using the store front config CatalogLayout as a folder name to prefix the view.
		/// For simple display use methods RenderCatalogViewCategoryPartial, RenderCatalogViewBundlePartial, and RenderCatalogViewProductPartial
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="view"></param>
		/// <param name="model"></param>
		public static void RenderCatalogPartial(this HtmlHelper<CatalogViewModel> htmlHelper, string view)
		{
			htmlHelper.RenderCatalogPartialHelper(view, null);
		}

		private static void RenderCatalogPartialHelper(this HtmlHelper<CatalogViewModel> htmlHelper, string view, ViewDataDictionary<CatalogViewModel> newViewDataOrNull)
		{
			if (htmlHelper.ViewData.Model == null)
			{
				throw new ArgumentNullException("htmlHelper.ViewData.Model");
			}

			StoreFrontConfiguration config = htmlHelper.CurrentStoreFrontConfig(false);
			if (config == null)
			{
				throw new ArgumentNullException("htmlHelper.CurrentStoreFrontConfig");
			}

			string partialViewPath = config.CatalogLayout.ToString() + "/" + view;
			if (config == null)
			{
				throw new ApplicationException("Current store front config not find to display catalog view '" + view + "'");
			}

			try
			{
				//catalog display
				if (newViewDataOrNull == null)
				{
					htmlHelper.RenderPartial(partialViewPath, htmlHelper.ViewData.Model);
				}
				else
				{
					htmlHelper.RenderPartial(partialViewPath, newViewDataOrNull);
				}
			}
			catch (Exception ex)
			{
				string errorMsg = "Error rendering catalog partial view '" + partialViewPath + "' for catalog layout '" + config.CatalogLayout.ToString() + "'" + "\n\n" + ex.Message;

				if (ex.InnerException != null)
				{
					errorMsg += "\nInner Exception: " + ex.InnerException.GetType().FullName + " " + ex.InnerException.Message;
					if (ex.InnerException.InnerException != null)
					{
						errorMsg += "\n2nd Inner Exception: " + ex.InnerException.InnerException.GetType().FullName + " " + ex.InnerException.InnerException.Message;
						if (ex.InnerException.InnerException.InnerException != null)
						{
							errorMsg += "\n3rd Inner Exception: " + ex.InnerException.InnerException.InnerException.GetType().FullName + " " + ex.InnerException.InnerException.InnerException.Message;
						}
					}
				}
				throw new ApplicationException(errorMsg, ex);
			}
		}

		/// <summary>
		/// Returns a List of product bundles sorted and available for cross-sell
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static List<ProductBundle> CatalogCrossSellBundles(this HtmlHelper<CatalogViewModel> htmlHelper)
		{
			CatalogViewModel viewModel = htmlHelper.ViewData.Model;
			Product product = viewModel.CurrentProductOrNull;
			StoreFrontConfiguration config = viewModel.StoreFrontConfig;

			if (product == null)
			{
				return null;
			}

			IQueryable<ProductBundle> relatedBundles = null;
			if (viewModel.CurrentProductBundleOrNull != null)
			{
				//Related bundles not including the current one
				relatedBundles = product.ProductBundleItems
					.AsQueryable()
					.WhereIsActive()
					.Select(pbi => pbi.ProductBundle)
					.Where(pbi => pbi.ProductBundleId != viewModel.CurrentProductBundleOrNull.ProductBundleId)
					.AsQueryable();
			}
			else
			{
				//all bundles where this product is available
				relatedBundles = product.ProductBundleItems
					.AsQueryable()
					.WhereIsActive()
					.Select(pbi => pbi.ProductBundle)
					.AsQueryable();
			}

			return relatedBundles
				.CanAddToCart(config.StoreFront)
				.ApplyDefaultSort()
				.ToList();
		}


	}
}
