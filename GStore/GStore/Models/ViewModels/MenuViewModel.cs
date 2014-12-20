using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;
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

			bool isRegistered = false;
			if (userProfile != null)
			{
				isRegistered = true;
			}
			if (storeFront != null)
			{
				this.CategoryTree = storeFront.CategoryTreeWhereActive(isRegistered);
				this.NavBarItemTree = storeFront.NavBarTreeWhereActive(isRegistered);
			}
			else
			{
				this.CategoryTree = storeFront.CategoryTreeWhereActive(isRegistered);
				this.NavBarItemTree = storeFront.NavBarTreeWhereActive(isRegistered);
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