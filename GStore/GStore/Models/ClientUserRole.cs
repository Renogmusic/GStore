using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	//Note: these permissions can also be set at the client level; if set at the client, it applies to all store fronts
	[Table("ClientUserRoles")]
	public class ClientUserRole : BaseClasses.ClientLiveRecord
	{
		[Key]
		public int ClientUserRoleId { get; set; }

		[ForeignKey("UserProfileId")]
		public virtual UserProfile UserProfile { get; set; }
		public int UserProfileId { get; set; }

		[Display(Description="Administrator - Can do everything")]
		public bool Client_IsAdmin { get; set; }

		#region Same Permissions as StoreFront, but here they apply to all store fronts

		[Display(Description = "Store Front Administrator - all functions for store front")]
		public bool StoreFront_IsAdmin { get; set; }

		[Display(Description = "Report Administrator - all reporting functions")]
		public bool Reports_IsAdmin { get; set; }

		[Display(Description = "View Combined Store Front Sales Reports")]
		public bool Reports_CanViewSalesReports { get; set; }

		[Display(Description = "View Combined Store Front Usage Log Reports")]
		public bool Reports_CanViewUsageReports { get; set; }

		[Display(Description = "Catalog Administrator - all catalog functions)")]
		public bool Catalog_IsAdmin { get; set; }

		[Display(Description = "Catalog - Manage Catalog Images")]
		public bool Catalog_CanManageImages { get; set; }

		[Display(Description = "Users - User Administrator")]
		public bool Users_IsAdmin { get; set; }

		[Display(Description = "Users - View User Profile")]
		public bool Users_CanViewProfiles { get; set; }

		[Display(Description = "Users - Reset Passwords")]
		public bool Users_CanResetPasswords { get; set; }

		[Display(Description = "Users - Can create new user profiles")]
		public bool Users_CanCreateUserProfiles { get; set; }

		[Display(Description = "Users - Edit User Profile")]
		public bool Users_CanEditProfiles { get; set; }

		[Display(Description = "Users - View Carts")]
		public bool Users_CanViewCarts { get; set; }

		[Display(Description = "Users - View Orders")]
		public bool Users_CanViewOrders { get; set; }

		#endregion

	}
}

