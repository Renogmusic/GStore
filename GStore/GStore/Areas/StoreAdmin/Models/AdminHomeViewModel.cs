using GStore.Models;
using GStore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Areas.StoreAdmin.Models
{
	public class AdminHomeViewModel
	{
		public AdminHomeViewModel(GStore.Data.IGstoreDb db)
		{
			this.StoreFront = db.GetCurrentStoreFront(null, true);
			this.UserProfile = db.GetCurrentUserProfile(true);

			if (user.IsInRole("AccountAdmin"))
			{
				this.IsAccountAdmin = true;
			}
			if (user.IsInRole("NotificationAdmin"))
			{
				this.IsNotificationAdmin = true;
			}
			if (user.IsInRole("StoreAdmin"))
			{
				this.IsStoreAdmin  = true;
			}
			if (user.IsInRole("ClientAdmin"))
			{
				this.IsClientAdmin = true;
			}
			if (user.IsInRole("SystemAdmin"))
			{
				this.IsSystemAdmin = true;
			}

			this.StoreFront = storeFront;
			this.Client = storeFront.Client;
			this.AspNetIdentityUser = this.UserProfile.AspNetIdentityUser();
			this.AspNetIdentityRoles = this.AspNetIdentityUser.Roles;
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