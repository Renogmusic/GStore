using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class WebFormAdminManagerViewModel : StoreAdminViewModel
	{
		public WebFormAdminManagerViewModel(StoreFront storeFront, UserProfile userProfile, IOrderedQueryable<WebForm> webForms)
			: base(storeFront, userProfile)
		{
			this.WebForms = webForms;
		}

		public IOrderedQueryable<WebForm> WebForms { get; set; }

	}
}