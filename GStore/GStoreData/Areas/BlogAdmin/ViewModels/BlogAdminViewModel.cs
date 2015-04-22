using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData.Areas.BlogAdmin.ViewModels
{
	public class BlogAdminViewModel
	{
		public BlogAdminViewModel() { }

		public BlogAdminViewModel(StoreFrontConfiguration currentStoreFrontConfig, UserProfile userProfile)
		{
			if (currentStoreFrontConfig == null)
			{
				throw new ApplicationException("BlogAdminMenuViewModel: currentStoreFrontConfig is null, currentStoreFrontConfig must be specified.");
			}
			if (userProfile == null)
			{
				throw new ApplicationException("BlogAdminMenuViewModel: userProfile is null, UserProfile must be specified.");
			}
			this.StoreFrontConfig = currentStoreFrontConfig;
			this.StoreFront = currentStoreFrontConfig.StoreFront;
			this.UserProfile = userProfile;
			this.Client = currentStoreFrontConfig.Client;
		}

		public void UpdateClient(Client client)
		{
			this.Client = client;
		}

		public AdminMenuViewModel AdminMenuViewModel
		{
			get
			{
				return new AdminMenuViewModel(this.StoreFront, this.UserProfile, "BlogAdmin");
			}
		}

		[Display(Name = "Store Front Configuration")]
		public StoreFrontConfiguration StoreFrontConfig { get; protected set; }

		[Display(Name = "Store Front")]
		public StoreFront StoreFront { get; protected set; }

		[Display(Name = "Client")]
		public Client Client { get; protected set; }

		[Display(Name = "User Profile")]
		public UserProfile UserProfile { get; protected set; }

		[Display(Name = "Is Active")]
		public bool IsActiveDirect { get; protected set; }

		[Display(Name = "Return to Front End")]
		public bool ReturnToFrontEnd { get; set; }

		public int? FilterProductCategoryId { get; set; }

		public string SortBy { get; set; }

		public bool? SortAscending { get; set; }

		public List<Product> Products
		{
			get
			{
				if (_products != null)
				{
					return _products;
				}
				if (this.StoreFront == null)
				{
					throw new ArgumentNullException("storeFront");
				}

				_products = this.StoreFront.Products.AsQueryable().ApplyDefaultSort().ToList();
				return _products;
			}
		}

		public List<ProductBundle> ProductBundles
		{
			get
			{
				if (_productBundles != null)
				{
					return _productBundles;
				}
				if (this.StoreFront == null)
				{
					throw new ArgumentNullException("storeFront");
				}

				_productBundles = this.StoreFront.ProductBundles.AsQueryable().ApplyDefaultSort().ToList();
				return _productBundles;
			}
		}

		public void UpdateSortedProducts(IOrderedQueryable<Product> sortedProducts)
		{
			_products = sortedProducts.ToList();
		}
		protected List<Product> _products = null;

		public IEnumerable<TreeNode<ProductCategory>> ProductCategoryTree
		{
			get
			{
				if (_productCategoryTree != null)
				{
					return _productCategoryTree;
				}
				if (this.StoreFront == null)
				{
					throw new ArgumentNullException("storeFront");
				}
				_productCategoryTree = this.ProductCategories.AsTree(pc => pc.ProductCategoryId, pc => pc.ParentCategoryId);
				return _productCategoryTree;
			}
		}
		protected IEnumerable<TreeNode<ProductCategory>> _productCategoryTree = null;

		public List<ProductCategory> ProductCategories
		{
			get
			{
				if (_productCategories != null)
				{
					return _productCategories;
				}
				if (this.StoreFront == null)
				{
					throw new ArgumentNullException("storeFront");
				}

				_productCategories = this.StoreFront.ProductCategories.AsQueryable().ApplyDefaultSort().ToList();
				return _productCategories;
			}
		}
		protected List<ProductCategory> _productCategories = null;

		public void UpdateSortedProductBundles(IOrderedQueryable<ProductBundle> sortedProductBundles)
		{
			_productBundles = sortedProductBundles.ToList();
		}
		protected List<ProductBundle> _productBundles = null;



	}
}