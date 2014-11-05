using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Models.Extensions;

namespace GStore.Models.ViewModels
{
	public class MenuViewModel
	{
		public StoreFront StoreFront { get; set; }
		public List<TreeNode<ProductCategory>> CategoryTree { get; set; }
		public List<TreeNode<NavBarItem>> NavBarItemTree { get; set; }

		public MenuViewModel(StoreFront storeFront, List<TreeNode<ProductCategory>> categoryTree, List<TreeNode<NavBarItem>> navBarItemTree)
		{
			this.StoreFront = storeFront;
			this.CategoryTree = categoryTree;
			this.NavBarItemTree = navBarItemTree;
		}
	}
}