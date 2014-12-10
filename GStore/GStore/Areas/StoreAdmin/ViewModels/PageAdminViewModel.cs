using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class PageAdminManagerViewModel : StoreAdminViewModel
	{
		public PageAdminManagerViewModel(StoreFront storeFront, UserProfile userProfile, IOrderedQueryable<Page> pages)
			: base(storeFront, userProfile)
		{
			this.Pages = pages;
		}

		public IOrderedQueryable<Page> Pages { get; set; }

	}
}