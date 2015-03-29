using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GStoreData.AppHtmlHelpers;
using GStoreData.ControllerBase;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData
{
	public static class CatalogExtensions
	{
		#region TreeNode methods

		public static List<TreeNode<ProductCategory>> CategoryTreeWhereActiveForNavBar(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<ProductCategory>>();
			}
			var query = storeFront.ProductCategories.AsQueryable()
				.WhereIsActive()
				.WhereRegisteredAnonymousCheck(isRegistered)
				.WhereShowInNavBarMenu(isRegistered)
				.OrderBy(cat => cat.Order)
				.ThenBy(cat => cat.Name)
				.AsTree(cat => cat.ProductCategoryId, cat => cat.ParentCategoryId);
			return query.ToList();
		}

		public static List<TreeNode<ProductCategory>> CategoryTreeWhereActiveForCatalogList(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<ProductCategory>>();
			}
			var query = storeFront.ProductCategories.AsQueryable()
				.WhereIsActive()
				.WhereRegisteredAnonymousCheck(isRegistered)
				.WhereShowInCatalogList(isRegistered)
				.OrderBy(cat => cat.Order)
				.ThenBy(cat => cat.Name)
				.AsTree(cat => cat.ProductCategoryId, cat => cat.ParentCategoryId);
			return query.ToList();
		}

		public static List<TreeNode<ProductCategory>> CategoryTreeWhereActiveForCatalogByName(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<ProductCategory>>();
			}
			var query = storeFront.ProductCategories.AsQueryable()
				.WhereIsActive()
				.WhereRegisteredAnonymousCheck(isRegistered)
				.WhereShowInCatalogByName(isRegistered)
				.OrderBy(cat => cat.Order)
				.ThenBy(cat => cat.Name)
				.AsTree(cat => cat.ProductCategoryId, cat => cat.ParentCategoryId);
			return query.ToList();
		}

		public static bool HasChildNodes(this TreeNode<ProductCategory> category)
		{
			return category.ChildNodes.Any();
		}

		public static bool HasChildMenuItems(this TreeNode<ProductCategory> category, int maxLevels)
		{
			return (maxLevels > category.Depth && category.Entity.AllowChildCategoriesInMenu && category.ChildNodes.Any());
		}

		#endregion

		#region Category Counts

		/// <summary>
		/// This is an expensive database and recursion operation, use with caution
		/// </summary>
		/// <param name="storeDb"></param>
		/// <param name="storeFront"></param>
		public static void RecalculateProductCategoryActiveCount(this IGstoreDb storeDb, StoreFront storeFront)
		{
			List<ProductCategory> categories = storeFront.ProductCategories.ToList();

			var treeQuery = storeFront.ProductCategories.AsTree(prod => prod.ProductCategoryId, prod => prod.ParentCategoryId);
			List<TreeNode<ProductCategory>> categoriesTree = treeQuery.ToList();

			foreach (ProductCategory category in categories)
			{
				//foreach category, calculate direct activecount
				int activeCountForAnonymous = category.Products.AsQueryable().WhereIsActive().Where(p => !p.ForRegisteredOnly).Count() + category.ProductBundles.AsQueryable().WhereIsActive().Where(p => !p.ForRegisteredOnly).Count();
				category.DirectActiveCountForAnonymous = activeCountForAnonymous;

				int activeCountForRegistered = category.Products.AsQueryable().WhereIsActive().Where(p => !p.ForAnonymousOnly).Count() + category.ProductBundles.AsQueryable().WhereIsActive().Where(p => !p.ForAnonymousOnly).Count();
				category.DirectActiveCountForRegistered = activeCountForRegistered;

			}

			foreach (ProductCategory category in categories)
			{
				TreeNode<ProductCategory> categoryNode = categoriesTree.FindEntity(category);
				int childActiveCountForAnonymous = categoryNode.ActiveCountWithChildrenForAnonymous();
				int childActiveCountForRegistered = categoryNode.ActiveCountWithChildrenForRegistered();
				categoryNode.Entity.ChildActiveCountForAnonymous = childActiveCountForAnonymous;
				categoryNode.Entity.ChildActiveCountForRegistered = childActiveCountForRegistered;
			}
			storeDb.SaveChangesEx(false, false, false, false);
		}

		/// <summary>
		/// This is an expensive database and recursion operation, use with caution
		/// </summary>
		/// <param name="storeDb"></param>
		/// <param name="storeFront"></param>
		public static void RecalculateProductCategoryActiveCount(this IGstoreDb storeDb, int storeFrontId)
		{
			StoreFront storeFront = storeDb.StoreFronts.FindById(storeFrontId);
			storeDb.RecalculateProductCategoryActiveCount(storeFront);
		}

		private static int ActiveCountWithChildrenForAnonymous(this TreeNode<ProductCategory> categoryNode)
		{
			int count = categoryNode.Entity.DirectActiveCountForAnonymous;
			foreach (TreeNode<ProductCategory> childNode in categoryNode.ChildNodes)
			{
				count += childNode.ActiveCountWithChildrenForAnonymous();
			}

			return count;
		}

		private static int ActiveCountWithChildrenForRegistered(this TreeNode<ProductCategory> categoryNode)
		{
			int count = categoryNode.Entity.DirectActiveCountForRegistered;
			foreach (TreeNode<ProductCategory> childNode in categoryNode.ChildNodes)
			{
				count += childNode.ActiveCountWithChildrenForRegistered();
			}

			return count;
		}

		#endregion

		#region Product Category

		/// <summary>
		/// Returns a path of this category up to the top
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public static string CategoryPath(this ProductCategory category, string delimiter = " -> ", int maxLevels = 10)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			return category.CategoryPathRecurse(delimiter, maxLevels);
		}

		private static string CategoryPathRecurse(this ProductCategory category, string delimiter = " -> ", int maxLevels = 10, int currentLevel = 1)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}

			string text = category.Name;

			if (category.ParentCategoryId.HasValue && (currentLevel < maxLevels))
			{
				text = category.ParentCategory.CategoryPathRecurse(delimiter, maxLevels, currentLevel + 1) + delimiter + text;
			}
			return text;
		}

		/// <summary>
		/// Main category image
		/// </summary>
		/// <param name="category"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this ProductCategory category, string applicationPath, RouteData routeData, bool forCart = false)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			if (string.IsNullOrWhiteSpace(category.ImageName))
			{
				return null;
			}
			return category.StoreFront.ProductCategoryCatalogFileUrl(applicationPath, routeData, category.ImageName);
		}

		public static string ImagePath(this ProductCategory category, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (category == null || string.IsNullOrEmpty(category.ImageName))
			{
				return null;
			}

			return category.StoreFront.ProductCategoryCatalogFilePath(applicationPath, routeData, server, category.ImageName);
		}

		public static string DefaultSummaryCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSummaryCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSummaryCaption + "')";
				}
				return category.DefaultSummaryCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Summary')";
			}
			return "Summary";
		}

		public static string DefaultTopDescriptionCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultTopDescriptionCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultTopDescriptionCaption + "')";
				}
				return category.DefaultTopDescriptionCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Description for " + productName + "')";
			}
			return "Description for " + productName;
		}

		public static string DefaultBottomDescriptionCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultBottomDescriptionCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultBottomDescriptionCaption + "')";
				}
				return category.DefaultBottomDescriptionCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Details for " + productName + "')";
			}
			return "Details for " + productName;
		}

		public static string DefaultSampleImageCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSampleImageCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSampleImageCaption + "')";
				}
				return category.DefaultSampleImageCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Sample Image for " + productName + "')";
			}
			return "Sample Image for " + productName;
		}

		public static string DefaultSampleDownloadCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSampleDownloadCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSampleDownloadCaption + "')";
				}
				return category.DefaultSampleDownloadCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Sample Download for " + productName + "')";
			}
			return "Sample Download for " + productName;
		}

		public static string DefaultSampleAudioCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSampleAudioCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSampleAudioCaption + "')";
				}
				return category.DefaultSampleAudioCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Sample Audio for " + productName + "')";
			}
			return "Sample Audio for " + productName;
		}

		public static MvcHtmlString NoProductsMessageHtmlOrSystemDefault(this ProductCategory category, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.NoProductsMessageHtml)))
			{
				if (wrapForAdmin)
				{
					return new MvcHtmlString("(Category Default: '" + category.NoProductsMessageHtml + "')");
				}
				return new MvcHtmlString(category.NoProductsMessageHtml);
			}
			if (category != null)
			{
				if (wrapForAdmin)
				{
					return new MvcHtmlString("(System Default: <h2>There are no current " + category.ProductTypePlural.ToHtml() + " in this Category</h2>");
				}
				return new MvcHtmlString("<h2>There are no current " + category.ProductTypePlural.ToHtml() + " in this Category</h2>");
			}

			if (wrapForAdmin)
			{
				return new MvcHtmlString("(System Default: <h2>There are no current products in this Category</h2>");
			}
			return new MvcHtmlString("<h2>There are no current products in this Category</h2>");
		}

		public static string MetaDescriptionOrSystemDefault(this ProductCategory category, StoreFrontConfiguration storeFrontConfig)
		{
			if (category != null && (!string.IsNullOrEmpty(category.MetaDescription)))
			{
				return category.MetaDescription;
			}
			if (storeFrontConfig != null && (!string.IsNullOrEmpty(storeFrontConfig.MetaDescription)))
			{
				return storeFrontConfig.MetaDescription;
			}
			return BaseController.DefaultMetaDescription;
		}

		public static string MetaKeywordsOrSystemDefault(this ProductCategory category, StoreFrontConfiguration storeFrontConfig)
		{
			if (category != null && (!string.IsNullOrEmpty(category.MetaKeywords)))
			{
				return category.MetaKeywords;
			}
			if (storeFrontConfig != null && (!string.IsNullOrEmpty(storeFrontConfig.MetaKeywords)))
			{
				return storeFrontConfig.MetaKeywords;
			}
			return BaseController.DefaultMetaKeywords;
		}

		public static void CloneFromParentForNew(this ProductCategory productCategory, ProductCategory parentProductCategory)
		{
			productCategory.ParentCategory = parentProductCategory;
			productCategory.ParentCategoryId = parentProductCategory.ProductCategoryId;

			productCategory.AllowChildCategoriesInMenu = parentProductCategory.AllowChildCategoriesInMenu;
			productCategory.BundleListTemplate = parentProductCategory.BundleListTemplate;
			productCategory.BundleTypePlural = parentProductCategory.BundleTypePlural;
			productCategory.BundleTypeSingle = parentProductCategory.BundleTypeSingle;
			productCategory.CategoryDetailTemplate = parentProductCategory.CategoryDetailTemplate;
			productCategory.ChildCategoryFooterHtml = parentProductCategory.ChildCategoryFooterHtml;
			productCategory.ChildCategoryHeaderHtml = parentProductCategory.ChildCategoryHeaderHtml;
			productCategory.DefaultBottomDescriptionCaption = parentProductCategory.DefaultBottomDescriptionCaption;
			productCategory.DefaultSampleAudioCaption = parentProductCategory.DefaultSampleAudioCaption;
			productCategory.DefaultSampleDownloadCaption = parentProductCategory.DefaultSampleDownloadCaption;
			productCategory.DefaultSampleImageCaption = parentProductCategory.DefaultSampleImageCaption;
			productCategory.DefaultSummaryCaption = parentProductCategory.DefaultSummaryCaption;
			productCategory.DefaultTopDescriptionCaption = parentProductCategory.DefaultTopDescriptionCaption;
			productCategory.DisplayForDirectLinks = parentProductCategory.DisplayForDirectLinks;
			productCategory.ForAnonymousOnly = parentProductCategory.ForAnonymousOnly;
			productCategory.ForRegisteredOnly = parentProductCategory.ForRegisteredOnly;
			productCategory.HideInMenuIfEmpty = parentProductCategory.HideInMenuIfEmpty;
			productCategory.ImageName = parentProductCategory.ImageName;
			productCategory.MetaDescription = parentProductCategory.MetaDescription;
			productCategory.MetaKeywords = parentProductCategory.MetaKeywords;
			productCategory.NoProductsMessageHtml = parentProductCategory.NoProductsMessageHtml;
			productCategory.ProductBundleDetailTemplate = parentProductCategory.ProductBundleDetailTemplate;
			productCategory.ProductBundleFooterHtml = parentProductCategory.ProductBundleFooterHtml;
			productCategory.ProductBundleHeaderHtml = parentProductCategory.ProductBundleHeaderHtml;
			productCategory.ProductDetailTemplate = parentProductCategory.ProductDetailTemplate;
			productCategory.ProductFooterHtml = parentProductCategory.ProductFooterHtml;
			productCategory.ProductHeaderHtml = parentProductCategory.ProductHeaderHtml;
			productCategory.ProductListTemplate = parentProductCategory.ProductListTemplate;
			productCategory.ProductTypePlural = parentProductCategory.ProductTypePlural;
			productCategory.ProductTypeSingle = parentProductCategory.ProductTypeSingle;
			productCategory.ShowInCatalogIfEmpty = parentProductCategory.ShowInCatalogIfEmpty;
			productCategory.ShowInMenu = parentProductCategory.ShowInMenu;
			productCategory.Theme = parentProductCategory.Theme;
			productCategory.ThemeId = parentProductCategory.ThemeId;
			productCategory.UseDividerAfterOnMenu = parentProductCategory.UseDividerAfterOnMenu;
			productCategory.UseDividerBeforeOnMenu = parentProductCategory.UseDividerBeforeOnMenu;
		}

		public static string RandomImageName(this ProductCategory category)
		{
			string folderPath = StoreFrontExtensions.MapPathToGStore("~/Content/Server/CatalogContent/Categories");
			return StoreFrontExtensions.FolderRandomImageFileName(folderPath);
		}

		#endregion

		#region Bundle

		/// <summary>
		/// Main category image
		/// </summary>
		/// <param name="category"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this ProductBundle bundle, string applicationPath, RouteData routeData, bool forCart = false)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			if (string.IsNullOrWhiteSpace(bundle.ImageName))
			{
				return null;
			}
			return bundle.StoreFront.ProductBundleCatalogFileUrl(applicationPath, routeData, bundle.ImageName);
		}

		public static string SummaryCaptionOrDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.SummaryCaption))
			{
				return bundle.SummaryCaption;
			}
			return bundle.Category.DefaultSummaryCaptionOrSystemDefault(bundle.Name);
		}

		public static string TopDescriptionCaptionOrDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.TopDescriptionCaption))
			{
				return bundle.TopDescriptionCaption;
			}
			return bundle.Category.DefaultTopDescriptionCaptionOrSystemDefault(bundle.Name);
		}

		public static string BottomDescriptionCaptionOrDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.BottomDescriptionCaption))
			{
				return bundle.BottomDescriptionCaption;
			}
			return bundle.Category.DefaultBottomDescriptionCaptionOrSystemDefault(bundle.Name);
		}

		public static string ProductTypeSingleOrSystemDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.ProductTypeSingle))
			{
				return bundle.ProductTypeSingle;
			}
			return bundle.Category.ProductTypeSingle.OrDefault("Item");
		}

		public static string ProductTypePluralOrSystemDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.ProductTypePlural))
			{
				return bundle.ProductTypePlural;
			}
			return bundle.Category.ProductTypePlural.OrDefault("Items");
		}

		public static string MetaDescriptionOrSystemDefault(this ProductBundle bundle, StoreFrontConfiguration storeFrontConfig)
		{
			if (bundle != null && (!string.IsNullOrEmpty(bundle.MetaDescription)))
			{
				return bundle.MetaDescription;
			}
			return bundle.Category.MetaDescriptionOrSystemDefault(storeFrontConfig);
		}

		public static string MetaKeywordsOrSystemDefault(this ProductBundle bundle, StoreFrontConfiguration storeFrontConfig)
		{
			if (bundle != null && (!string.IsNullOrEmpty(bundle.MetaKeywords)))
			{
				return bundle.MetaKeywords;
			}
			return bundle.Category.MetaKeywordsOrSystemDefault(storeFrontConfig);
		}

		public static ProductBundleDetailTemplateEnum ProductBundleDetailTemplateOrCategory(this ProductBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (bundle.ProductBundleDetailTemplate.HasValue)
			{
				return bundle.ProductBundleDetailTemplate.Value;
			}

			return bundle.Category.ProductBundleDetailTemplate;
		}

		public static decimal? UnitPrice(this ProductBundle bundle, int qty)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.ProductBundleItems == null || bundle.ProductBundleItems.Count == 0)
			{
				return null;
			}

			if (!bundle.ProductBundleItems.AsQueryable().WhereIsActive().All(bi => bi.BaseUnitPrice.HasValue))
			{
				//if any items don't have a unit price, unit price of this item is null (call for price)
				return null;
			}
			return bundle.ProductBundleItems.AsQueryable().WhereIsActive().Sum(bi => bi.UnitPriceExt(bi.Quantity));
		}

		public static decimal? ListPrice(this ProductBundle bundle, int qty)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.ProductBundleItems == null || bundle.ProductBundleItems.Count == 0)
			{
				return null;
			}

			if (!bundle.ProductBundleItems.AsQueryable().WhereIsActive().All(bi => bi.BaseListPrice.HasValue))
			{
				//if any items don't have a list price, list price of this item is null (call for price)
				return null;
			}
			return bundle.ProductBundleItems.AsQueryable().WhereIsActive().Sum(bi => bi.ListPriceExt(bi.Quantity));
		}

		public static string RandomImageName(this ProductBundle bundle)
		{
			string folderPath = StoreFrontExtensions.MapPathToGStore("~/Content/Server/CatalogContent/Bundles");
			return StoreFrontExtensions.FolderRandomImageFileName(folderPath);
		}


		#endregion

		#region Product

		/// <summary>
		/// Main product image
		/// </summary>
		/// <param name="product"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this Product product, string applicationPath, RouteData routeData, bool forCart = false)
		{
			if (product == null)
			{
				return null; ;
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.ImageName);
		}

		public static string ImagePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.ImageName);
		}

		/// <summary>
		/// Returns the URL to the product sample audio file
		/// </summary>
		/// <param name="product"></param>
		/// <param name="applicationPath"></param>
		/// <param name="routeData"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public static string SampleAudioUrl(this Product product, string applicationPath, RouteData routeData)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.SampleAudioFileName);
		}

		/// <summary>
		/// Returns the full file path to the product sample audio file
		/// </summary>
		/// <param name="product"></param>
		/// <param name="applicationPath"></param>
		/// <param name="routeData"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public static string SampleAudioPath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.SampleAudioFileName);
		}

		public static string DigitalDownloadFilePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductDigitalDownloadFilePath(applicationPath, routeData, server, product.DigitalDownloadFileName);
		}

		public static string SampleDownloadFileUrl(this Product product, string applicationPath, RouteData routeData)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.SampleDownloadFileName);
		}

		public static string SampleDownloadFilePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.SampleDownloadFileName);
		}

		public static string SampleImageFileUrl(this Product product, string applicationPath, RouteData routeData)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.SampleImageFileName);
		}

		public static string SampleImageFilePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.SampleImageFileName);
		}

		public static string SummaryCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SummaryCaption))
			{
				return product.SummaryCaption;
			}
			return product.Category.DefaultSummaryCaptionOrSystemDefault(product.Name);
		}

		public static string TopDescriptionCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.TopDescriptionCaption))
			{
				return product.TopDescriptionCaption;
			}
			return product.Category.DefaultTopDescriptionCaptionOrSystemDefault(product.Name);
		}

		public static string BottomDescriptionCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.BottomDescriptionCaption))
			{
				return product.BottomDescriptionCaption;
			}
			return product.Category.DefaultBottomDescriptionCaptionOrSystemDefault(product.Name);
		}

		public static string SampleImageCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SampleImageCaption))
			{
				return product.SampleImageCaption;
			}
			return product.Category.DefaultSampleImageCaptionOrSystemDefault(product.Name);
		}

		public static string SampleDownloadCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SampleDownloadCaption))
			{
				return product.SampleDownloadCaption;
			}
			return product.Category.DefaultSampleDownloadCaptionOrSystemDefault(product.Name);
		}

		public static string SampleAudioCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SampleAudioCaption))
			{
				return product.SampleAudioCaption;
			}
			return product.Category.DefaultSampleAudioCaptionOrSystemDefault(product.Name);
		}

		public static string MetaDescriptionOrSystemDefault(this Product product, StoreFrontConfiguration storeFrontConfig)
		{
			if (product != null && (!string.IsNullOrEmpty(product.MetaDescription)))
			{
				return product.MetaDescription;
			}
			return product.Category.MetaDescriptionOrSystemDefault(storeFrontConfig);
		}

		public static string MetaKeywordsOrSystemDefault(this Product product, StoreFrontConfiguration storeFrontConfig)
		{
			if (product != null && (!string.IsNullOrEmpty(product.MetaKeywords)))
			{
				return product.MetaKeywords;
			}
			return product.Category.MetaKeywordsOrSystemDefault(storeFrontConfig);
		}

		public static ProductDetailTemplateEnum ProductDetailTemplateOrCategory(this Product product)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (product.ProductDetailTemplate.HasValue)
			{
				return product.ProductDetailTemplate.Value;
			}

			return product.Category.ProductDetailTemplate;
		}

		public static string RandomImageName(this Product product)
		{
			string folderPath = StoreFrontExtensions.MapPathToGStore("~/Content/Server/CatalogContent/Products");
			return StoreFrontExtensions.FolderRandomImageFileName(folderPath);
		}


		#endregion 


		public static bool ValidateProductUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int? currentProductId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this product";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			Product conflict = db.Products.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.UrlName.ToLower() == urlName && (p.ProductId != currentProductId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Product '" + conflict.Name + "' [" + conflict.ProductId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Product URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateProductCategoryUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int? currentProductCategoryId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this category";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			ProductCategory conflict = db.ProductCategories.Where(pc => pc.ClientId == clientId && pc.StoreFrontId == storeFrontId && pc.UrlName.ToLower() == urlName && (pc.ProductCategoryId != currentProductCategoryId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Category '" + conflict.Name + "' [" + conflict.ProductCategoryId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Category URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);

			return false;

		}

		public static bool ValidateProductBundleUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int? currentProductBundleId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this product Bundle";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			ProductBundle conflict = db.ProductBundles.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.UrlName.ToLower() == urlName && (p.ProductBundleId != currentProductBundleId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Product Bundle '" + conflict.Name + "' [" + conflict.ProductBundleId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Product Bundle URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static Product CreateProduct(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			Product record = db.Products.Create();

			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.ImageName = viewModel.ImageName;
			record.UrlName = viewModel.UrlName;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.DigitalDownload = viewModel.DigitalDownload;
			record.MaxQuantityPerOrder = viewModel.MaxQuantityPerOrder;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;
			record.ProductCategoryId = viewModel.ProductCategoryId;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.AvailableForPurchase = viewModel.AvailableForPurchase;
			record.RequestAQuote_Show = viewModel.RequestAQuote_Show;
			record.RequestAQuote_Label = viewModel.RequestAQuote_Label;
			record.RequestAQuote_PageId = viewModel.RequestAQuote_PageId;
			record.BaseListPrice = viewModel.BaseListPrice;
			record.BaseUnitPrice = viewModel.BaseUnitPrice;
			record.ThemeId = viewModel.ThemeId;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;

			record.SummaryCaption = viewModel.SummaryCaption;
			record.SummaryHtml = viewModel.SummaryHtml;
			record.TopDescriptionCaption = viewModel.TopDescriptionCaption;
			record.TopDescriptionHtml = viewModel.TopDescriptionHtml;
			record.TopLinkHref = viewModel.TopLinkHref;
			record.TopLinkLabel = viewModel.TopLinkLabel;
			record.BottomDescriptionCaption = viewModel.BottomDescriptionCaption;
			record.BottomDescriptionHtml = viewModel.BottomDescriptionHtml;
			record.BottomLinkHref = viewModel.BottomLinkHref;
			record.BottomLinkLabel = viewModel.BottomLinkLabel;
			record.FooterHtml = viewModel.FooterHtml;

			record.DigitalDownloadFileName = viewModel.DigitalDownloadFileName;
			record.SampleAudioCaption = viewModel.SampleAudioCaption;
			record.SampleAudioFileName = viewModel.SampleAudioFileName;
			record.SampleDownloadCaption = viewModel.SampleDownloadCaption;
			record.SampleDownloadFileName = viewModel.SampleDownloadFileName;
			record.SampleImageCaption = viewModel.SampleImageCaption;
			record.SampleImageFileName = viewModel.SampleImageFileName;

			record.UpdateAuditFields(userProfile);

			db.Products.Add(record);
			db.SaveChanges();

			return record;

		}

		public static Product UpdateProduct(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			//find existing record, update it
			Product record = storeFront.Products.SingleOrDefault(p => p.ProductId == viewModel.ProductId);
			if (record == null)
			{
				throw new ApplicationException("Product not found in storefront Products. Product Id: " + viewModel.ProductId);
			}

			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.ImageName = viewModel.ImageName;
			record.UrlName = viewModel.UrlName;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.DigitalDownload = viewModel.DigitalDownload;
			record.MaxQuantityPerOrder = viewModel.MaxQuantityPerOrder;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;
			record.ProductCategoryId = viewModel.ProductCategoryId;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.UpdatedBy = userProfile;
			record.UpdateDateTimeUtc = DateTime.UtcNow;

			record.AvailableForPurchase = viewModel.AvailableForPurchase;
			record.RequestAQuote_Show = viewModel.RequestAQuote_Show;
			record.RequestAQuote_Label = viewModel.RequestAQuote_Label;
			record.RequestAQuote_PageId = viewModel.RequestAQuote_PageId;
			record.BaseListPrice = viewModel.BaseListPrice;
			record.BaseUnitPrice = viewModel.BaseUnitPrice;
			record.ThemeId = viewModel.ThemeId;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;

			record.SummaryCaption = viewModel.SummaryCaption;
			record.SummaryHtml = viewModel.SummaryHtml;
			record.TopDescriptionCaption = viewModel.TopDescriptionCaption;
			record.TopDescriptionHtml = viewModel.TopDescriptionHtml;
			record.TopLinkHref = viewModel.TopLinkHref;
			record.TopLinkLabel = viewModel.TopLinkLabel;
			record.BottomDescriptionCaption = viewModel.BottomDescriptionCaption;
			record.BottomDescriptionHtml = viewModel.BottomDescriptionHtml;
			record.BottomLinkHref = viewModel.BottomLinkHref;
			record.BottomLinkLabel = viewModel.BottomLinkLabel;

			record.FooterHtml = viewModel.FooterHtml;
			record.DigitalDownloadFileName = viewModel.DigitalDownloadFileName;
			record.SampleAudioCaption = viewModel.SampleAudioCaption;
			record.SampleAudioFileName = viewModel.SampleAudioFileName;
			record.SampleDownloadCaption = viewModel.SampleDownloadCaption;
			record.SampleDownloadFileName = viewModel.SampleDownloadFileName;
			record.SampleImageCaption = viewModel.SampleImageCaption;
			record.SampleImageFileName = viewModel.SampleImageFileName;

			db.Products.Update(record);
			db.SaveChanges();

			return record;

		}

		public static ProductCategory CreateProductCategory(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.CategoryEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			ProductCategory record = db.ProductCategories.Create();

			record.AllowChildCategoriesInMenu = viewModel.AllowChildCategoriesInMenu;
			record.ChildActiveCountForAnonymous = 0;
			record.ChildActiveCountForRegistered = 0;
			record.DirectActiveCountForAnonymous = 0;
			record.DirectActiveCountForRegistered = 0;

			record.ImageName = viewModel.ImageName;
			record.ParentCategoryId = viewModel.ParentCategoryId;
			record.ProductTypeSingle = viewModel.ProductTypeSingle;
			record.ProductTypePlural = viewModel.ProductTypePlural;
			record.BundleTypeSingle = viewModel.BundleTypeSingle;
			record.BundleTypePlural = viewModel.BundleTypePlural;

			record.HideInMenuIfEmpty = viewModel.HideInMenuIfEmpty;
			record.ShowInMenu = viewModel.ShowInMenu;
			record.DisplayForDirectLinks = viewModel.DisplayForDirectLinks;
			record.ShowInCatalogIfEmpty = viewModel.ShowInCatalogIfEmpty;
			record.UrlName = viewModel.UrlName;

			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.UseDividerAfterOnMenu = viewModel.UseDividerAfterOnMenu;
			record.UseDividerBeforeOnMenu = viewModel.UseDividerBeforeOnMenu;
			record.ThemeId = viewModel.ThemeId;
			record.CategoryDetailTemplate = viewModel.CategoryDetailTemplate;
			record.ProductListTemplate = viewModel.ProductListTemplate;
			record.BundleListTemplate = viewModel.BundleListTemplate;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;
			record.ChildCategoryHeaderHtml = viewModel.ChildCategoryHeaderHtml;
			record.ChildCategoryFooterHtml = viewModel.ChildCategoryFooterHtml;
			record.ProductHeaderHtml = viewModel.ProductHeaderHtml;
			record.ProductFooterHtml = viewModel.ProductFooterHtml;
			record.NoProductsMessageHtml = viewModel.NoProductsMessageHtml;

			record.DefaultSummaryCaption = viewModel.DefaultSummaryCaption;
			record.DefaultTopDescriptionCaption = viewModel.DefaultTopDescriptionCaption;
			record.DefaultBottomDescriptionCaption = viewModel.DefaultBottomDescriptionCaption;
			record.DefaultSampleImageCaption = viewModel.DefaultSampleImageCaption;
			record.DefaultSampleAudioCaption = viewModel.DefaultSampleAudioCaption;
			record.DefaultSampleDownloadCaption = viewModel.DefaultSampleDownloadCaption;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.UpdateAuditFields(userProfile);


			db.ProductCategories.Add(record);
			db.SaveChanges();

			return record;

		}

		public static ProductCategory UpdateProductCategory(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.CategoryEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			//find existing record, update it
			ProductCategory record = storeFront.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == viewModel.ProductCategoryId);
			if (record == null)
			{
				throw new ApplicationException("Category not found in storefront Categories. Category Id: " + viewModel.ProductCategoryId);
			}

			record.AllowChildCategoriesInMenu = viewModel.AllowChildCategoriesInMenu;
			record.ImageName = viewModel.ImageName;
			record.ParentCategoryId = viewModel.ParentCategoryId;
			record.ProductTypeSingle = viewModel.ProductTypeSingle;
			record.ProductTypePlural = viewModel.ProductTypePlural;
			record.BundleTypeSingle = viewModel.BundleTypeSingle;
			record.BundleTypePlural = viewModel.BundleTypePlural;

			record.HideInMenuIfEmpty = viewModel.HideInMenuIfEmpty;
			record.ShowInMenu = viewModel.ShowInMenu;
			record.ShowInCatalogIfEmpty = viewModel.ShowInCatalogIfEmpty;
			record.DisplayForDirectLinks = viewModel.DisplayForDirectLinks;
			record.UrlName = viewModel.UrlName;

			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.UseDividerAfterOnMenu = viewModel.UseDividerAfterOnMenu;
			record.UseDividerBeforeOnMenu = viewModel.UseDividerBeforeOnMenu;
			record.ThemeId = viewModel.ThemeId;
			record.CategoryDetailTemplate = viewModel.CategoryDetailTemplate;
			record.ProductListTemplate = viewModel.ProductListTemplate;
			record.BundleListTemplate = viewModel.BundleListTemplate;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;
			record.ChildCategoryHeaderHtml = viewModel.ChildCategoryHeaderHtml;
			record.ChildCategoryFooterHtml = viewModel.ChildCategoryFooterHtml;
			record.ProductHeaderHtml = viewModel.ProductHeaderHtml;
			record.ProductFooterHtml = viewModel.ProductFooterHtml;
			record.NoProductsMessageHtml = viewModel.NoProductsMessageHtml;

			record.DefaultSummaryCaption = viewModel.DefaultSummaryCaption;
			record.DefaultTopDescriptionCaption = viewModel.DefaultTopDescriptionCaption;
			record.DefaultBottomDescriptionCaption = viewModel.DefaultBottomDescriptionCaption;
			record.DefaultSampleImageCaption = viewModel.DefaultSampleImageCaption;
			record.DefaultSampleAudioCaption = viewModel.DefaultSampleAudioCaption;
			record.DefaultSampleDownloadCaption = viewModel.DefaultSampleDownloadCaption;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;

			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			record.UpdatedBy = userProfile;
			record.UpdateDateTimeUtc = DateTime.UtcNow;

			record = db.ProductCategories.Update(record);
			db.SaveChanges();

			return record;

		}

		public static ProductBundle CreateProductBundle(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductBundleEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			ProductBundle record = db.ProductBundles.Create();

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.UpdateAuditFields(userProfile);
			record = record.MapFromViewModel(viewModel);

			record = db.ProductBundles.Add(record);
			db.SaveChanges();

			return record;
		}

		public static ProductBundle UpdateProductBundle(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductBundleEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			ProductBundle record = storeFront.ProductBundles.SingleOrDefault(pb => pb.ProductBundleId == viewModel.ProductBundleId);
			if (record == null)
			{
				throw new ApplicationException("Product Bundle not found in storefront Product Bundles. Product Bundle Id: " + viewModel.ProductBundleId);
			}

			record.UpdateAuditFields(userProfile);
			record = record.MapFromViewModel(viewModel);

			record = db.ProductBundles.Update(record);
			db.SaveChanges();

			return record;
		}

		public static int UpdateProductBundleItems(this IGstoreDb db, ProductBundle productBundle, List<Areas.CatalogAdmin.ViewModels.ProductBundleItemEditAdminViewModel> bundleItems, StoreFront storeFront, UserProfile userProfile)
		{
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}
			if (bundleItems == null)
			{
				throw new ArgumentNullException("bundleItems");
			}

			if (bundleItems.Count == 0)
			{
				return 0;
			}

			int counter = 0;
			foreach (Areas.CatalogAdmin.ViewModels.ProductBundleItemEditAdminViewModel itemModel in bundleItems)
			{
				ProductBundleItem item = productBundle.ProductBundleItems.SingleOrDefault(bi => bi.ProductBundleItemId == itemModel.ProductBundleItemId);
				if (item == null)
				{
					throw new ApplicationException("Product Bundle Item Id: " + itemModel.ProductBundleItemId + " not found in Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "]");
				}

				item.Order = itemModel.Order;
				item.ProductVariantInfo = itemModel.ProductVariantInfo;
				item.Quantity = itemModel.Quantity;
				item.BaseUnitPrice = itemModel.BaseUnitPrice;
				item.BaseListPrice = itemModel.BaseListPrice;
				item.IsPending = itemModel.IsPending;
				item.StartDateTimeUtc = itemModel.StartDateTimeUtc;
				item.EndDateTimeUtc = itemModel.EndDateTimeUtc;
				item = db.ProductBundleItems.Update(item);
				counter++;
			}

			db.SaveChanges();

			return counter;
		}

		private static ProductBundle MapFromViewModel(this ProductBundle record, GStoreData.Areas.CatalogAdmin.ViewModels.ProductBundleEditAdminViewModel viewModel)
		{
			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.ImageName = viewModel.ImageName;
			record.UrlName = viewModel.UrlName;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.MaxQuantityPerOrder = viewModel.MaxQuantityPerOrder;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;
			record.ProductCategoryId = viewModel.ProductCategoryId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			record.AvailableForPurchase = viewModel.AvailableForPurchase;
			record.RequestAQuote_Show = viewModel.RequestAQuote_Show;
			record.RequestAQuote_Label = viewModel.RequestAQuote_Label;
			record.RequestAQuote_PageId = viewModel.RequestAQuote_PageId;
			record.ThemeId = viewModel.ThemeId;
			record.ProductBundleDetailTemplate = viewModel.ProductBundleDetailTemplate;
			record.SummaryCaption = viewModel.SummaryCaption;
			record.SummaryHtml = viewModel.SummaryHtml;
			record.TopDescriptionCaption = viewModel.TopDescriptionCaption;
			record.TopDescriptionHtml = viewModel.TopDescriptionHtml;
			record.TopLinkHref = viewModel.TopLinkHref;
			record.TopLinkLabel = viewModel.TopLinkLabel;
			record.BottomDescriptionCaption = viewModel.BottomDescriptionCaption;
			record.BottomDescriptionHtml = viewModel.BottomDescriptionHtml;
			record.BottomLinkHref = viewModel.BottomLinkHref;
			record.BottomLinkLabel = viewModel.BottomLinkLabel;
			record.FooterHtml = viewModel.FooterHtml;
			record.ProductTypeSingle = viewModel.ProductTypeSingle;
			record.ProductTypePlural = viewModel.ProductTypePlural;

			return record;
		}

		/// <summary>
		/// re-orders siblings and puts them in order by 10's, and saves to database
		/// </summary>
		/// <param name="navBarItems"></param>
		public static void ProductCategoriesRenumberSiblings(this IGstoreDb db, IEnumerable<ProductCategory> productCategories)
		{
			List<ProductCategory> sortedItems = productCategories.AsQueryable().ApplyDefaultSort().ToList();

			int order = 100;
			foreach (ProductCategory item in sortedItems)
			{
				item.Order = order;
				order += 10;
			}
			db.SaveChanges();
		}

		/// <summary>
		/// Creates a Product Bundle item with basic settings for a fast add admin command
		/// This overload uses a Product entity and a ProductBundleId int value
		/// </summary>
		/// <param name="db"></param>
		/// <param name="productBundleId"></param>
		/// <param name="product"></param>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public static ProductBundleItem CreateProductBundleItemFastAdd(this IGstoreDb db, int productBundleId, Product product, StoreFront storeFront, UserProfile userProfile, int quantity = 1)
		{
			if (productBundleId == 0)
			{
				throw new ArgumentNullException("productBundleId cannot be 0");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}

			ProductBundle productBundle = storeFront.ProductBundles.Where(pb => pb.ProductBundleId == productBundleId).SingleOrDefault();
			if (productBundle == null)
			{
				throw new ApplicationException("Product Bundle not found by id: " + productBundleId);
			}

			return db.CreateProductBundleItemFastAdd(productBundle, product, storeFront, userProfile, quantity);
		}

		/// <summary>
		/// Creates a Product Bundle item with basic settings for a fast add admin command
		/// This overload uses a ProductBundle entity and a ProductId int value
		/// </summary>
		/// <param name="db"></param>
		/// <param name="bundle"></param>
		/// <param name="productId"></param>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public static ProductBundleItem CreateProductBundleItemFastAdd(this IGstoreDb db, ProductBundle bundle, int productId, StoreFront storeFront, UserProfile userProfile, int quantity = 1)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (productId == 0)
			{
				throw new ApplicationException("ProductId cannot be zero");
			}

			Product product = storeFront.Products.Where(p => p.ProductId == productId).SingleOrDefault();
			if (product == null)
			{
				throw new ApplicationException("Product not found by id: " + productId);
			}

			return db.CreateProductBundleItemFastAdd(bundle, product, storeFront, userProfile, quantity);

		}

		/// <summary>
		/// Creates a Product Bundle item with basic settings for a fast add admin command
		/// This is the prefered overload
		/// </summary>
		/// <param name="db"></param>
		/// <param name="bundle"></param>
		/// <param name="product"></param>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public static ProductBundleItem CreateProductBundleItemFastAdd(this IGstoreDb db, ProductBundle bundle, Product product, StoreFront storeFront, UserProfile userProfile, int quantity = 1)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}

			ProductBundleItem bundleItem = db.ProductBundleItems.Create();
			bundleItem.SetDefaultsForNew(bundle);

			bundleItem.Quantity = quantity;
			bundleItem.ProductBundle = bundle;
			bundleItem.ProductBundleId = bundle.ProductBundleId;

			bundleItem.BaseListPrice = product.BaseListPrice;
			bundleItem.BaseUnitPrice = product.BaseUnitPrice;
			bundleItem.Product = product;
			bundleItem.ProductId = product.ProductId;
			bundleItem.ProductVariantInfo = "";

			bundleItem = db.ProductBundleItems.Add(bundleItem);
			db.SaveChanges();

			return bundleItem;

		}

		public static void SetDefaultsForNew(this ProductBundleItem item, ProductBundle bundle)
		{
			item.Client = bundle.Client;
			item.ClientId = bundle.ClientId;
			item.StoreFront = bundle.StoreFront;
			item.StoreFrontId = bundle.StoreFrontId;
			item.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			item.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			item.IsPending = false;
			item.Quantity = 1;
			item.ProductVariantInfo = "";
			item.ProductBundle = bundle;
			item.Order = bundle.ProductBundleItems.Count == 0 ? 100 : bundle.ProductBundleItems.Max(pbi => pbi.Order) + 10;
		}

		/// <summary>
		/// Returns a queryable filter to check ForanonymousOnly and ForRegisteredOnly against the current user
		/// </summary>
		/// <param name="query"></param>
		/// <param name="isRegistered"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereRegisteredAnonymousCheck(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			return query.Where(pc =>
					(isRegistered || !pc.ForRegisteredOnly)
					&&
					(!isRegistered || !pc.ForAnonymousOnly));
		}

		/// <summary>
		/// Returns a queryable filter for whether to show this category in the navbar menu
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereShowInNavBarMenu(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			if (isRegistered)
			{
				return query.Where(cat => cat.ShowInMenu && (!cat.HideInMenuIfEmpty || cat.ChildActiveCountForRegistered > 0));
			}
			else
			{
				return query.Where(cat => cat.ShowInMenu && (!cat.HideInMenuIfEmpty || cat.ChildActiveCountForAnonymous > 0));
			}
		}

		/// <summary>
		/// Returns a queryable filter whether this category should be navigable in the catalog
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereShowInCatalogList(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			if (isRegistered)
			{
				return query.Where(cat =>
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForRegistered != 0)
					);
			}
			else
			{
				return query.Where(cat =>
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForAnonymous != 0)
					);
			}
		}

		/// <summary>
		/// Returns a queryable filter whether this category should show when targetted by name in category details
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereShowInCatalogByName(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			if (isRegistered)
			{
				return query.Where(cat =>
					cat.DisplayForDirectLinks
					||
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForRegistered != 0)
					);
			}
			else
			{
				return query.Where(cat =>
					cat.DisplayForDirectLinks
					||
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForAnonymous != 0)
					);
			}
		}

		/// <summary>
		/// Returns a queryable filter to check ForanonymousOnly and ForRegisteredOnly against the current user
		/// </summary>
		/// <param name="query"></param>
		/// <param name="isRegistered"></param>
		/// <returns></returns>
		public static IQueryable<ProductBundle> WhereRegisteredAnonymousCheck(this IQueryable<ProductBundle> query, bool isRegistered)
		{
			return query.Where(pc =>
					(isRegistered || !pc.ForRegisteredOnly)
					&&
					(!isRegistered || !pc.ForAnonymousOnly));
		}

		public static string SyncCatalogFile(this Product product, Expression<Func<Product, String>> expression, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			return product.SyncFileHelper(expression, false, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncDigitalDownloadFile(this Product product, Expression<Func<Product, String>> expression, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			return product.SyncFileHelper(expression, true, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		private static string SyncFileHelper(this Product product, Expression<Func<Product, String>> expression, bool digitalDownload, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			StoreFront storeFront = product.StoreFront;
			StoreFrontConfiguration storeFrontConfig = storeFront.CurrentConfigOrAny();
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("product.StoreFront.CurrentConfigOrAny");
			}

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<Product, String>(expression, new ViewDataDictionary<Product>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName ?? metadata.PropertyName;
			PropertyInfo property = typeof(Product).GetProperty(expressionText);
			string currentFileName = (property.GetValue(product) ?? "").ToString();


			hasDbChanges = false;
			StringBuilder results = new StringBuilder();

			if (!string.IsNullOrEmpty(currentFileName))
			{
				if (eraseFileNameIfNotFound)
				{
					//file name is set, verify file and set to null if not found
					string filePath = null;
					if (digitalDownload)
					{
						filePath = storeFront.ProductDigitalDownloadFilePath(applicationPath, routeData, server, currentFileName);
					}
					else
					{
						filePath = storeFront.ProductCatalogFilePath(applicationPath, routeData, server, currentFileName);
					}
					if (string.IsNullOrEmpty(filePath))
					{
						results.AppendLine("- - - - - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - '" + currentFileName + "' not found, setting to null - - - - -");
						currentFileName = null;
						property.SetValue(product, null);
						hasDbChanges = true;
					}
					else
					{
						if (verbose)
						{
							results.AppendLine("OK - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - '" + currentFileName + "' File Confirmed");
						}
					}
				}
			}

			if (string.IsNullOrEmpty(currentFileName))
			{
				if (searchForFileIfBlank)
				{
					//if file name is not set (or file was not found and set to blank), see if there is a suitable file in the file system
					string newFileName = null;
					if (digitalDownload)
					{
						newFileName = storeFront.ChooseFileNameWildcard(storeFrontConfig.Client, "DigitalDownload/Products", defaultFileName, applicationPath, server);
					}
					else
					{
						newFileName = storeFront.ChooseFileNameWildcard(storeFrontConfig.Client, "CatalogContent/Products", defaultFileName, applicationPath, server);
					}

					if (string.IsNullOrEmpty(newFileName))
					{
						if (verbose)
						{
							results.AppendLine("OK - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - File is null and no file found starting with '" + defaultFileName + "'");
						}
					}
					else
					{
						results.AppendLine("- - - - - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - New File '" + newFileName + "' - - - - -");
						currentFileName = newFileName;
						property.SetValue(product, newFileName);
						hasDbChanges = true;
					}
				}
				else
				{
					if (verbose)
					{
						results.AppendLine("OK Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - Ignoring Blank file link because searchForFileIfBlank = false");
					}
				}
			}
			return results.ToString();
		}

		public static string SyncImageFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_Image";
			return product.SyncCatalogFile(model => model.ImageName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncDigitalDownloadFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_DigitalDownload";
			return product.SyncDigitalDownloadFile(model => model.DigitalDownloadFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncSampleAudioFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_SampleAudio";
			return product.SyncCatalogFile(model => model.SampleAudioFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncSampleDownloadFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_SampleDownload";
			return product.SyncCatalogFile(model => model.SampleDownloadFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncSampleImageFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_SampleImage";
			return product.SyncCatalogFile(model => model.SampleImageFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncImageFile(this ProductBundle productBundle, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}
			string defaultFileName = productBundle.UrlName + "_Image";
			return productBundle.SyncCatalogFile(model => model.ImageName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncCatalogFile(this ProductBundle productBundle, Expression<Func<ProductBundle, String>> expression, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			return productBundle.SyncFileHelper(expression, false, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		private static string SyncFileHelper(this ProductBundle productBundle, Expression<Func<ProductBundle, String>> expression, bool digitalDownload, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}
			StoreFront storeFront = productBundle.StoreFront;
			StoreFrontConfiguration storeFrontConfig = storeFront.CurrentConfigOrAny();
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("productBundle.StoreFront.CurrentConfigOrAny");
			}

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<ProductBundle, String>(expression, new ViewDataDictionary<ProductBundle>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName ?? metadata.PropertyName;
			PropertyInfo property = typeof(ProductBundle).GetProperty(expressionText);
			string currentFileName = (property.GetValue(productBundle) ?? "").ToString();


			hasDbChanges = false;
			StringBuilder results = new StringBuilder();

			if (!string.IsNullOrEmpty(currentFileName))
			{
				if (eraseFileNameIfNotFound)
				{
					//file name is set, verify file and set to null if not found
					string filePath = storeFront.ProductBundleCatalogFilePath(applicationPath, routeData, server, currentFileName);
					if (string.IsNullOrEmpty(filePath))
					{
						results.AppendLine("- - - - - Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - '" + currentFileName + "' not found, setting to null - - - - -");
						currentFileName = null;
						property.SetValue(productBundle, null);
						hasDbChanges = true;
					}
					else
					{
						if (verbose)
						{
							results.AppendLine("OK - Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - '" + currentFileName + "' File Confirmed");
						}
					}
				}
			}

			if (string.IsNullOrEmpty(currentFileName))
			{
				if (searchForFileIfBlank)
				{
					//if file name is not set (or file was not found and set to blank), see if there is a suitable file in the file system
					string newFileName = storeFront.ChooseFileNameWildcard(storeFrontConfig.Client, "CatalogContent/Bundles", defaultFileName, applicationPath, server);

					if (string.IsNullOrEmpty(newFileName))
					{
						if (verbose)
						{
							results.AppendLine("OK - Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - File is null and no file found starting with '" + defaultFileName + "'");
						}
					}
					else
					{
						results.AppendLine("- - - - - Product 'Bundle " + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - New File '" + newFileName + "' - - - - -");
						currentFileName = newFileName;
						property.SetValue(productBundle, newFileName);
						hasDbChanges = true;
					}
				}
				else
				{
					if (verbose)
					{
						results.AppendLine("OK Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - Ignoring Blank file link because searchForFileIfBlank = false");
					}
				}
			}
			return results.ToString();
		}

	}
}