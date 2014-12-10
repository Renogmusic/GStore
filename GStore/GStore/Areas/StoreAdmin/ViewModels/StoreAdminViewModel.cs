using GStore.Models;
using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class StoreAdminViewModel
	{
		public StoreAdminViewModel() { }

		public StoreAdminViewModel(StoreFront currentStoreFront, UserProfile userProfile)
		{
			if (currentStoreFront == null)
			{
				throw new ApplicationException("StoreAdminMenuViewModel: currentStoreFront is null, StoreFront must be specified.");
			}
			if (userProfile == null)
			{
				throw new ApplicationException("StoreAdminMenuViewModel: userProfile is null, UserProfile must be specified.");
			}
			this.StoreFront = currentStoreFront;
			this.UserProfile = userProfile;
			this.Client = currentStoreFront.Client;
			this.IsSystemAdmin = this.UserProfile.AspNetIdentityUserIsInRoleSystemAdmin();
		}

		[Display(Name = "Store Front")]
		public StoreFront StoreFront { get; set; }

		[Display(Name = "Client")]
		public Client Client { get; set; }

		[Display(Name = "User Profile")]
		public UserProfile UserProfile { get; set; }

		[Display(Name = "Is System Admin")]
		public bool IsSystemAdmin { get; set; }

	}
}