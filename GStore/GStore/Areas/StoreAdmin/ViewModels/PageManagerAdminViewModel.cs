using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class PageManagerAdminViewModel : StoreAdminViewModel
	{
		public PageManagerAdminViewModel(StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, IOrderedQueryable<Page> pages)
			: base(storeFrontConfig, userProfile)
		{
			this.Pages = pages;
		}

		public IOrderedQueryable<Page> Pages { get; protected set; }

		public IEnumerable<Models.ViewModels.PageEditViewModel> PageEditViewModels
		{
			get
			{
				if (_pageEditViewModels == null)
				{
					if (this.Pages == null)
					{
						return null;
					}
					_pageEditViewModels = this.Pages.AsEnumerable().Select(pg => new Models.ViewModels.PageEditViewModel(pg, isStoreAdminEdit: true));
				}
				return _pageEditViewModels;
			}
			protected set
			{
				_pageEditViewModels = value;
			}
		}
		IEnumerable<Models.ViewModels.PageEditViewModel> _pageEditViewModels = null;
	}
}