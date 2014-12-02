using GStore.Models;
using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

		public StoreFront StoreFront { get; set; }
		public Client Client { get; set; }
		public UserProfile UserProfile { get; set; }
		public bool IsSystemAdmin { get; set; }

	}
}