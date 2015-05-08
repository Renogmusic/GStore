using System;
using System.Collections.Generic;
using System.Linq;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class CatalogViewModel
	{

		public StoreFront StoreFront { get; set; }
		public StoreFrontConfiguration StoreFrontConfig { get; set; }
		public List<TreeNode<ProductCategory>> CategoryTree { get; set; }
		public int MaxLevels { get; set; }

		public ProductCategory CurrentCategoryOrNull { get; set; }

		public Product CurrentProductOrNull { get; set; }

		public ProductBundle CurrentProductBundleOrNull { get; set; }

		public ProductBundleItem CurrentProductBundleItemOrNull { get; set; }

		public void KillCache()
		{
			_products = null;
			_bundles = null;
			_bundleItems = null;
		}

		public CatalogViewModel(StoreFront storeFront, List<TreeNode<ProductCategory>> categoryTree, int maxLevels, ProductCategory currentCategoryOrNull, Product currentProductOrNull, ProductBundle currentProductBundleOrNull, ProductBundleItem currentProductBundleItemOrNull, TreeNode<ProductCategory> currentCategoryNodeOrNull = null, List<Product> productListOrNull = null, List<ProductBundle> productBundleListOrNull = null)
		{
			this.StoreFront = storeFront;
			this.StoreFrontConfig = storeFront.CurrentConfig();
			this.CategoryTree = categoryTree;
			this.MaxLevels = maxLevels;
			this._currentCategoryNode = currentCategoryNodeOrNull;

			this.CurrentCategoryOrNull = currentCategoryOrNull;
			this.CurrentProductOrNull = currentProductOrNull;
			this.CurrentProductBundleOrNull = currentProductBundleOrNull;
			this.CurrentProductBundleItemOrNull = currentProductBundleItemOrNull;

			this._products = productListOrNull;
			this._bundles = productBundleListOrNull;
		}

		public TreeNode<ProductCategory> CurrentCategoryNodeOrNull
		{
			get
			{
				if (_currentCategoryNode != null)
				{
					return _currentCategoryNode;
				}
				if (CurrentCategoryOrNull == null && CurrentProductOrNull == null)
				{
					return null;
				}
				if (CurrentCategoryOrNull != null)
				{
					_currentCategoryNode = CategoryTree.FindEntity(CurrentCategoryOrNull);
				}
				else
				{
					_currentCategoryNode = CategoryTree.FindEntity(CurrentProductOrNull.Category);
				}
				return _currentCategoryNode; 
			}
		}
		protected TreeNode<ProductCategory> _currentCategoryNode = null;

		public List<Product> CurrentProductsListOrNull(UserProfile profile)
		{
			if (_products != null)
			{
				return _products;
			}
			if (CurrentCategoryOrNull == null)
			{
				return null;
			}

			if (CurrentCategoryOrNull.CategoryAltProducts.Count == 0)
			{
				//no cross-sell
				_products = CurrentCategoryOrNull.Products.AsQueryable().WhereRegisteredAnonymousCheck(profile != null).WhereIsActive().ApplyDefaultSort().ToList();
			}
			else
			{
				//merge cross-sell items
				List<Tuple<int, Product>> products = CurrentCategoryOrNull.Products.AsQueryable().WhereRegisteredAnonymousCheck(profile != null).WhereIsActive().ToList().Select(p => Tuple.Create(p.Order, p)).ToList();
				List<Tuple<int, Product>> crossSellProducts = CurrentCategoryOrNull.CategoryAltProducts.AsQueryable().WhereRegisteredAnonymousCheck(profile != null).WhereIsActive().Select(alt => Tuple.Create(alt.Order, alt.Product)).ToList();

				products.AddRange(crossSellProducts);
				_products = products.OrderBy(t => t.Item1).ThenBy(t => t.Item2.Order).Select(t => t.Item2).ToList();
			}
			return _products;
		}
		protected List<Product> _products = null;

		public List<ProductBundle> CurrentProductBundlesListOrNull(UserProfile profile)
		{
			if (_bundles != null)
			{
				return _bundles;
			}
			if (CurrentCategoryOrNull == null)
			{
				return null;
			}

			if (CurrentCategoryOrNull.CategoryAltProductBundles.Count == 0)
			{
				_bundles = CurrentCategoryOrNull.ProductBundles.AsQueryable().WhereRegisteredAnonymousCheck(profile != null).WhereIsActive().ApplyDefaultSort().ToList();
			}
			else
			{
				//merge cross-sell items
				List<Tuple<int, ProductBundle>> bundles = CurrentCategoryOrNull.ProductBundles.AsQueryable().WhereRegisteredAnonymousCheck(profile != null).WhereIsActive().ToList().Select(p => Tuple.Create(p.Order, p)).ToList();
				List<Tuple<int, ProductBundle>> crossSellBundles = CurrentCategoryOrNull.CategoryAltProductBundles.AsQueryable().WhereRegisteredAnonymousCheck(profile != null).WhereIsActive().Select(alt => Tuple.Create(alt.Order, alt.ProductBundle)).ToList();

				bundles.AddRange(crossSellBundles);
				_bundles = bundles.OrderBy(t => t.Item1).ThenBy(t => t.Item2.Order).Select(t => t.Item2).ToList();
			}

			return _bundles;
		}
		protected List<ProductBundle> _bundles = null;

		public List<ProductBundleItem> CurrentProductBundleItemsListOrNull()
		{
			if (_bundleItems != null)
			{
				return _bundleItems;
			}
			if (CurrentProductBundleOrNull == null)
			{
				return null;
			}
			_bundleItems = CurrentProductBundleOrNull.ProductBundleItems.AsQueryable().WhereIsActive().ApplyDefaultSort().ToList();
			return _bundleItems;
		}
		protected List<ProductBundleItem> _bundleItems = null;

	}
}