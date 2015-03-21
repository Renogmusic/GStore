﻿using System.Collections.Generic;
using System.Linq;
using GStoreData.Models;

namespace GStoreData.Areas.StoreAdmin.ViewModels
{
	public class NavBarItemManagerAdminViewModel : StoreAdminViewModel
	{
		public NavBarItemManagerAdminViewModel(StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, IOrderedQueryable<NavBarItem> navBarItems)
			: base(storeFrontConfig, userProfile)
		{
			this.NavBarItems = navBarItems;
			if (navBarItems != null)
			{
				this.NavBarItemEditViewModels = navBarItems.Select(nb => new NavBarItemEditAdminViewModel(nb, userProfile)).ToList();
			}
		}

		public IOrderedQueryable<NavBarItem> NavBarItems { get; protected set; }

		public List<NavBarItemEditAdminViewModel> NavBarItemEditViewModels { get; protected set; }

		public IEnumerable<TreeNode<NavBarItem>> NavBarItemTree()
		{
			if (_navBarItemTree != null)
			{
				return _navBarItemTree;
			}
			if (this.NavBarItems == null)
			{
				return null;
			}
			
			_navBarItemTree = this.NavBarItems.AsQueryable().AsTree(nb => nb.NavBarItemId, nb => nb.ParentNavBarItemId);
			return _navBarItemTree;
		}
		protected IEnumerable<TreeNode<NavBarItem>> _navBarItemTree = null;
	}
}