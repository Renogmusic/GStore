using System.Collections.Generic;
using System.Linq;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData.Areas.StoreAdmin.ViewModels
{
	public class PageManagerAdminViewModel : StoreAdminViewModel
	{
		public PageManagerAdminViewModel(StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, IOrderedQueryable<Page> pages)
			: base(storeFrontConfig, userProfile)
		{
			this.Pages = pages;
		}

		public IOrderedQueryable<Page> Pages { get; protected set; }

		public IEnumerable<PageEditViewModel> PageEditViewModels
		{
			get
			{
				if (_pageEditViewModels == null)
				{
					if (this.Pages == null)
					{
						return null;
					}
					_pageEditViewModels = this.Pages.AsEnumerable().Select(pg => new PageEditViewModel(pg, isStoreAdminEdit: true));
				}
				return _pageEditViewModels;
			}
			protected set
			{
				_pageEditViewModels = value;
			}
		}
		IEnumerable<PageEditViewModel> _pageEditViewModels = null;
	}
}