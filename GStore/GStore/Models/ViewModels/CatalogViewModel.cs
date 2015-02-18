using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;

namespace GStore.Models.ViewModels
{
	public class CatalogViewModel
	{
		public StoreFront StoreFront { get; set; }
		public StoreFrontConfiguration StoreFrontConfig { get; set; }
		public List<TreeNode<ProductCategory>> CategoryTree { get; set; }
		public int MaxLevels { get; set; }
		public ProductCategory CurrentCategoryOrNull { get; set; }
		public Product CurrentProductOrNull { get; set; }

		public CatalogViewModel(StoreFront storeFront, List<TreeNode<ProductCategory>> categoryTree, int maxLevels, ProductCategory currentCategoryOrNull, Product currentProductOrNull, TreeNode<ProductCategory> currentCategoryNodeOrNull = null, List<Product> productListOrNull = null)
		{
			this.StoreFront = storeFront;
			this.StoreFrontConfig = storeFront.CurrentConfig();
			this.CategoryTree = categoryTree;
			this.MaxLevels = maxLevels;
			this.CurrentCategoryOrNull = currentCategoryOrNull;
			this.CurrentProductOrNull = currentProductOrNull;
			this._currentCategoryNode = currentCategoryNodeOrNull;
			this._products = productListOrNull;
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

		public List<Product> CurrentProductsListOrNull
		{
			get
			{
				if (_products != null)
				{
					return _products;
				}
				if (CurrentCategoryOrNull == null)
				{
					return null;
				}
				_products = CurrentCategoryOrNull.Products.AsQueryable().WhereIsActive().ToList();
				return _products;
			}
		}
		protected List<Product> _products = null;
	}
}