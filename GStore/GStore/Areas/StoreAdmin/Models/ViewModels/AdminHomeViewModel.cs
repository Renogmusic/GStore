using GStore.Models;
using GStore.Identity;
using GStore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Areas.StoreAdmin.Models.ViewModels
{
	public class AdminHomeViewModel
	{
		public AdminHomeViewModel(GStore.Data.IGstoreDb db, StoreFront currentStoreFront, UserProfile userProfile)
		{
			this.StoreFront = currentStoreFront;
			this.UserProfile = userProfile;
			this.Client = currentStoreFront.Client;
			this.AspNetIdentityUser = this.UserProfile.AspNetIdentityUser();

			if (AspNetIdentityUser.IsInRole("AccountAdmin"))
			{
				this.IsAccountAdmin = true;
			}
			if (AspNetIdentityUser.IsInRole("NotificationAdmin"))
			{
				this.IsNotificationAdmin = true;
			}
			if (AspNetIdentityUser.IsInRole("StoreAdmin"))
			{
				this.IsStoreAdmin  = true;
			}
			if (AspNetIdentityUser.IsInRole("ClientAdmin"))
			{
				this.IsClientAdmin = true;
			}
			if (AspNetIdentityUser.IsInRole("SystemAdmin"))
			{
				this.IsSystemAdmin = true;
			}

			this.AspNetIdentityRoles = AspNetIdentityUser.AspNetIdentityRoles();

		}

		public bool IsAccountAdmin { get; set; }
		public bool IsNotificationAdmin { get; set; }
		public bool IsStoreAdmin { get; set; }
		public bool IsClientAdmin { get; set; }
		public bool IsSystemAdmin { get; set; }

		public StoreFront StoreFront { get; set; }
		public Client Client { get; set; }
		public UserProfile UserProfile { get; set; }
		public Identity.AspNetIdentityUser AspNetIdentityUser { get; set; }
		public ICollection<Identity.AspNetIdentityRole> AspNetIdentityRoles { get; set; }
		public ICollection<Identity.AspNetIdentityUserClaim> AspNetIdentityClaims { get; set; }
		public ICollection<Identity.AspNetIdentityUserLogin> AspNetIdentityLogins { get; set; }

	}
}