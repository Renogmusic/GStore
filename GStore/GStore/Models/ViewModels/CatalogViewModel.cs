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
		public List<TreeNode<ProductCategory>> CategoryTree { get; set; }
		public int MaxLevels { get; set; }
		public ProductCategory CurrentCategoryOrNull { get; set; }
		public Product CurrentProductOrNull { get; set; }

		public CatalogViewModel(StoreFront storeFront, List<TreeNode<ProductCategory>> categoryTree, int maxLevels, ProductCategory currentCategoryOrNull, Product currentProductOrNull)
		{
			this.StoreFront = storeFront;
			this.CategoryTree = categoryTree;
			this.MaxLevels = maxLevels;
			this.CurrentCategoryOrNull = currentCategoryOrNull;
			this.CurrentProductOrNull = currentProductOrNull;
		}

		public TreeNode<ProductCategory> CurrentCategoryNodeOrNull
		{
			get
			{
				if (CurrentCategoryOrNull == null && CurrentProductOrNull == null)
				{
					return null;
				}
				if (CurrentCategoryOrNull != null)
				{
					return CategoryTree.FindEntity(CurrentCategoryOrNull);
				}
				return CategoryTree.FindEntity(CurrentProductOrNull.Category);
			}
		}

		public List<Product> CurrentProductsListOrNull
		{
			get
			{
				if (CurrentCategoryOrNull == null)
				{
					return null;
				}

				return CurrentCategoryOrNull.Products.AsQueryable().WhereIsActive().ToList();
			}
		}
	}
}