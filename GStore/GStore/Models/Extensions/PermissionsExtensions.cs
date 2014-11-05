using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Models.Extensions
{
	/// <summary>
	/// Extension class to handle permission and role checks
	/// </summary>
	public static class PermissionsExtensions
	{
		/// <summary>
		/// Returns true if the user can create and admin store fronts for the client
		/// </summary>
		/// <param name="client"></param>
		/// <param name="userProfile"></param>
		/// <returns></returns>
		public static bool CanCreateAndAdminStoreFrontsForClient(this Client client, UserProfile userProfile)
		{
			return client.IsClientStoreFrontAdmin(userProfile);
		}

		/// <summary>
		/// Returns true if the current user has any admin permission for the store front
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <returns></returns>
		public static bool HasAnyAdminPermission(this StoreFront storeFront, UserProfile userProfile)
		{
			Identity.AspNetIdentityUser aspNetUser = userProfile.AspNetIdentityUser();
			if (userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				//ASP.Net identity systemadmin, all permissions are true
				return true;
			}

			if (userProfile.ClientUserRoles
					.Where(data => data.ClientId == storeFront.ClientId)
					.WhereIsActive()
					.Any(data => 
						data.Catalog_CanManageImages
						|| data.Catalog_IsAdmin 
						|| data.Client_IsAdmin
						|| data.Reports_CanViewSalesReports
						|| data.Reports_CanViewUsageReports
						|| data.Reports_IsAdmin
						|| data.StoreFront_IsAdmin
						|| data.Users_CanCreateUserProfiles
						|| data.Users_CanEditProfiles
						|| data.Users_CanResetPasswords
						|| data.Users_CanViewCarts
						|| data.Users_CanViewOrders
						|| data.Users_CanViewProfiles
						|| data.Users_IsAdmin)
				)
			{
				//user is catalog or storefront administrator for all stores by client permission
				return true;
			}

			if (userProfile.StoreFrontUserRoles
					.Where(data => data.StoreFrontId == storeFront.StoreFrontId)
					.WhereIsActive()
					.Any(data =>
						data.Catalog_CanManageImages
						|| data.Catalog_IsAdmin
						|| data.Reports_CanViewSalesReports
						|| data.Reports_CanViewUsageReports
						|| data.Reports_IsAdmin
						|| data.StoreFront_IsAdmin
						|| data.Users_CanCreateUserProfiles
						|| data.Users_CanEditProfiles
						|| data.Users_CanResetPasswords
						|| data.Users_CanViewCarts
						|| data.Users_CanViewOrders
						|| data.Users_CanViewProfiles
						|| data.Users_IsAdmin)
				)
			{
				//user is catalog or storefront administrator for this store by store front permission
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true if the user can administer the storefront catalog
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <returns></returns>
		public static bool CanCreateAndAdminCatalog(this StoreFront storeFront, UserProfile userProfile)
		{
			Identity.AspNetIdentityUser aspNetUser = userProfile.AspNetIdentityUser();
			if (userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				//ASP.Net identity systemadmin, all permissions are true
				return true;
			}

			if (userProfile.ClientUserRoles
					.Where(data => data.ClientId == storeFront.ClientId)
					.WhereIsActive()
					.Any(data => data.Catalog_IsAdmin || data.StoreFront_IsAdmin || data.Client_IsAdmin)
				)
			{
				//user is catalog or storefront administrator for all stores by client permission
				return true;
			}

			if (userProfile.StoreFrontUserRoles
					.Where(data => data.StoreFrontId == storeFront.StoreFrontId)
					.WhereIsActive()
					.Any(data => data.Catalog_IsAdmin || data.StoreFront_IsAdmin)
				)
			{
				//user is catalog or storefront administrator for this store by store front permission
				return true;
			}

			return false;
		}

		
		/// <summary>
		/// Returns true if the user is a storefront administrator
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <returns></returns>
		public static bool IsStoreFrontAdmin(this StoreFront storeFront, UserProfile userProfile)
		{
			Identity.AspNetIdentityUser aspNetUser = userProfile.AspNetIdentityUser();
			if (userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				//ASP.Net identity systemadmin, all permissions are true
				return true;
			}

			if (userProfile.ClientUserRoles
					.Where(data => data.ClientId == storeFront.ClientId)
					.WhereIsActive()
					.Any(data => data.StoreFront_IsAdmin || data.Client_IsAdmin)
				)
			{
				//user is storefront administrator for all stores by client permission
				return true;
			}

			if (userProfile.StoreFrontUserRoles
					.Where(data => data.StoreFrontId == storeFront.StoreFrontId)
					.WhereIsActive()
					.Any(data => data.StoreFront_IsAdmin)
				)
			{
				//user is storefront administrator for this store by store front permission
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns true
		/// </summary>
		/// <param name="client"></param>
		/// <param name="userProfile"></param>
		/// <returns></returns>
		public static bool IsClientStoreFrontAdmin(this Client client, UserProfile userProfile)
		{
			Identity.AspNetIdentityUser aspNetUser = userProfile.AspNetIdentityUser();
			if (userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				//ASP.Net identity systemadmin, all permissions are true
				return true;
			}

			if (userProfile.ClientUserRoles
					.Where(data => data.ClientId == client.ClientId)
					.WhereIsActive()
					.Any(data => data.StoreFront_IsAdmin || data.Client_IsAdmin)
				)
			{
				//user is client admin by client permission
				return true;
			}

			return false;
		}
	}
}