using GStore.Models;
using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Models.ViewModels;

namespace GStore.Areas.CatalogAdmin.ViewModels
{
	public class CategoryListAdminViewModel : CatalogAdminViewModel
	{
		public CategoryListAdminViewModel(StoreFrontConfiguration currentStoreFrontConfig, UserProfile userProfile) : base(currentStoreFrontConfig, userProfile)
		{
			if (currentStoreFrontConfig == null)
			{
				throw new ArgumentNullException("currentStoreFrontConfig");
			}
		}

		public List<ProductCategory> ProductCategories
		{
			get
			{
				if (_products != null)
				{
					return _products;
				}
				if (this.StoreFrontConfig == null)
				{
					throw new ArgumentNullException("currentStoreFrontConfig");
				}

				_products = this.StoreFront.ProductCategories.AsQueryable().ApplyDefaultSort().ToList();
				return _products;
			}
		}
		protected List<ProductCategory> _products = null;

		public IEnumerable<TreeNode<ProductCategory>> ProductCategoryTree
		{
			get
			{
				if (_productCategoryTree != null)
				{
					return _productCategoryTree;
				}
				if (this.ProductCategories == null)
				{
					return null;
				}

				_productCategoryTree = this.ProductCategories.AsQueryable().AsTree(pc => pc.ProductCategoryId, pc => pc.ParentCategoryId);
				return _productCategoryTree;
			}
		}
		protected IEnumerable<TreeNode<ProductCategory>> _productCategoryTree = null;

	}
}