using System.Collections.Generic;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class MenuViewModel
	{
		public StoreFront StoreFront { get; protected set; }
		public StoreFrontConfiguration StoreFrontConfig { get; protected set; }
		public List<TreeNode<ProductCategory>> CategoryTree { get; protected set; }
		public List<TreeNode<NavBarItem>> NavBarItemTree { get; protected set; }
		public UserProfile UserProfile { get; protected set; }
		public bool ShowCart { get; protected set; }
		public bool ShowChat { get; protected set;}
		public bool ShowBlog { get; protected set; }

		public bool ShowAboutGStore { get; protected set; }
		public Cart Cart { get; protected set; }
		public long? ChatUserCount { get; protected set; }

		/// <summary>
		/// this constructor will internally call functions to create the navbar tree and category tree for menus
		/// </summary>
		/// <param name="storeFrontConfig"></param>
		/// <param name="userProfile"></param>
		/// <param name="sessionId"></param>
		public MenuViewModel(StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, string sessionId, long? chatUserCount)
		{
			bool isRegistered = false;
			if (userProfile != null)
			{
				isRegistered = true;
			}

			this.StoreFront = (storeFrontConfig == null ? null : storeFrontConfig.StoreFront);
			this.StoreFrontConfig = storeFrontConfig;
			this.CategoryTree = this.StoreFront.CategoryTreeWhereActiveForNavBar(isRegistered);
			this.NavBarItemTree = this.StoreFront.NavBarTreeWhereActive(isRegistered);
			this.UserProfile = userProfile;

			this.ShowAboutGStore = true;
			if (storeFrontConfig != null && (!storeFrontConfig.ShowAboutGStoreMenu))
			{
				this.ShowAboutGStore = false;
			}


			ChatUserCount = chatUserCount;
			this.ShowBlog = false;
			if ((storeFrontConfig != null) && storeFrontConfig.ShowBlogInMenu)
			{
				this.ShowBlog = true;
			}

			this.ShowChat = false;
			if (Settings.AppEnableChat && (storeFrontConfig != null) && (storeFrontConfig.ChatEnabled))
			{
				this.ShowChat = true;
			}

			this.ShowCart = false;
			if ((storeFrontConfig != null) && (storeFrontConfig.UseShoppingCart))
			{
				this.Cart = this.StoreFront.GetCart(sessionId, userProfile);
				if (userProfile == null && storeFrontConfig.CartNavShowCartToAnonymous)
				{
					this.ShowCart = (storeFrontConfig.CartNavShowCartWhenEmpty || Cart.ItemCount > 0);
				}
				else if (userProfile != null && storeFrontConfig.CartNavShowCartToRegistered)
				{
					this.ShowCart = (storeFrontConfig.CartNavShowCartWhenEmpty || Cart.ItemCount > 0);
				}
			}

		}

		/// <summary>
		/// this constructor will use parameters passed in for navbar tree and category tree for menus
		/// </summary>
		/// <param name="storeFrontConfig"></param>
		/// <param name="categoryTree"></param>
		/// <param name="navBarItemTree"></param>
		/// <param name="userProfile"></param>
		/// <param name="sessionId"></param>
		public MenuViewModel(StoreFrontConfiguration storeFrontConfig, List<TreeNode<ProductCategory>> categoryTree, List<TreeNode<NavBarItem>> navBarItemTree, UserProfile userProfile, string sessionId)
		{
			this.StoreFront = (storeFrontConfig == null ? null : storeFrontConfig.StoreFront);
			this.CategoryTree = categoryTree;
			this.NavBarItemTree = navBarItemTree;
			this.UserProfile = userProfile;

			this.ShowCart = false;
			if ((storeFrontConfig != null) && (storeFrontConfig.UseShoppingCart))
			{
				this.Cart = this.StoreFront.GetCart(sessionId, userProfile);
				if (userProfile == null && storeFrontConfig.CartNavShowCartToAnonymous)
				{
					this.ShowCart = (storeFrontConfig.CartNavShowCartWhenEmpty || Cart.ItemCount > 0);
				}
				else if (userProfile != null && storeFrontConfig.CartNavShowCartToRegistered)
				{
					this.ShowCart = (storeFrontConfig.CartNavShowCartWhenEmpty || Cart.ItemCount > 0);
				}
			}
		}

		public AdminMenuViewModel AdminMenuViewModel
		{
			get
			{
				return new AdminMenuViewModel(this.StoreFront, this.UserProfile, "");
			}
		}
	}
}