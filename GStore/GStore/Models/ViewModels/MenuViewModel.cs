using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Models.Extensions;
using GStore.Identity;

namespace GStore.Models.ViewModels
{
	public class MenuViewModel
	{
		public StoreFront StoreFront { get; set; }
		public List<TreeNode<ProductCategory>> CategoryTree { get; set; }
		public List<TreeNode<NavBarItem>> NavBarItemTree { get; set; }
		public UserProfile UserProfile { get; set; }
		public bool ShowStoreAdminLink { get; set; }
		public bool ShowSystemAdminLink { get; set; }

		public MenuViewModel(StoreFront storeFront, UserProfile userProfile)
		{
			this.StoreFront = storeFront;
			this.UserProfile = userProfile;
			this.ShowSystemAdminLink = userProfile.AspNetIdentityUserIsInRoleSystemAdmin();

			if (storeFront != null)
			{
				this.CategoryTree = storeFront.CategoryTreeWhereActive();
				this.NavBarItemTree = storeFront.NavBarTreeWhereActive(userProfile == null);
			}
			else
			{
				this.CategoryTree = storeFront.CategoryTreeWhereActive();
				this.NavBarItemTree = storeFront.NavBarTreeWhereActive(userProfile == null);
			}
			this.ShowStoreAdminLink = storeFront.ShowStoreAdminLink(userProfile);
		}

		public MenuViewModel(StoreFront storeFront, List<TreeNode<ProductCategory>> categoryTree, List<TreeNode<NavBarItem>> navBarItemTree, UserProfile userProfile)
		{
			this.StoreFront = storeFront;
			this.CategoryTree = categoryTree;
			this.NavBarItemTree = navBarItemTree;
			this.UserProfile = userProfile;
			this.ShowStoreAdminLink = storeFront.ShowStoreAdminLink(userProfile);
			this.ShowSystemAdminLink = userProfile.AspNetIdentityUserIsInRoleSystemAdmin();
		}
	}
}