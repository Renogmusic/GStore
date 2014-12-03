using GStore.Models;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Identity
{
	/// <summary>
	/// Extension class to handle authorization (permission) and role checks
	/// </summary>
	public static class AuthorizationExtensions
	{
		/// <summary>
		/// Checks a single StoreFront permission for the specified user.
		/// </summary>
		/// <param name="userProfile"></param>
		/// <param name="storeFront"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static bool Authorization_IsAuthorized(this UserProfile userProfile, StoreFront storeFront, GStore.Identity.GStoreAction action)
		{
			return storeFront.Authorization_IsAuthorized(userProfile, action);
		}

		/// <summary>
		/// Checks Multiple StoreFront permissions for the specified user.
		/// If allowAnyMatch is true, this is an OR test for each option.  If allowAnyMatch = false, this is an AND test for all options
		/// </summary>
		/// <param name="userProfile"></param>
		/// <param name="storeFront"></param>
		/// <param name="allowAnyMatch">If allowAnyMatch is true, this is an OR test for each option.  If allowAnyMatch = false, this is an AND test for all options</param>
		/// <param name="actions"></param>
		/// <returns></returns>
		public static bool Authorization_IsAuthorized(this UserProfile userProfile, StoreFront storeFront, bool allowAnyMatch, params GStore.Identity.GStoreAction[] actions)
		{
			return storeFront.Authorization_IsAuthorized(userProfile, allowAnyMatch, actions);
		}

		/// <summary>
		/// Checks a single StoreFront permission for the specified user.
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static bool Authorization_IsAuthorized(this StoreFront storeFront, UserProfile userProfile, GStore.Identity.GStoreAction action)
		{
			if (userProfile == null)
			{
				return false;
			}
			if (userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				return true;
			}
			if (storeFront == null)
			{
				return false;
			}

			return CheckSinglePermission(userProfile, storeFront, action);
		}

		/// <summary>
		/// Checks multiple StoreFront permissions for the specified user
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="allowAnyMatch">If allowAnyMatch is true, this is an OR test for each option.  If allowAnyMatch = false, this is an AND test for all options</param>
		/// <param name="actions"></param>
		/// <returns></returns>
		public static bool Authorization_IsAuthorized(this StoreFront storeFront, UserProfile userProfile, bool allowAnyMatch, params GStore.Identity.GStoreAction[] actions)
		{
			if (actions == null || actions.Count() == 0)
			{
				throw new ApplicationException("Authorization_IsAuthorized: no GStoreActions specified. You must call Authorization_IsAuthorized with one or more GStoreActions");
			}

			if (userProfile == null)
			{
				return false;
			}
			if (userProfile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				return true;
			}

			if (storeFront == null)
			{
				return false;
			}

			foreach (GStoreAction action in actions)
			{
				bool result = CheckSinglePermission(userProfile, storeFront, action);
				if (allowAnyMatch && result)
				{
					//OR test, if one is true, return true
					return true;
				}
				if (!allowAnyMatch && result == false)
				{
					//AND result, one is false, return false
					return false;
				}
			}
			if (allowAnyMatch)
			{
				//OR result, nothing matched
				return false;
			}
			else
			{
				//AND result, all matched
				return true;
			}
		}

		/// <summary>
		/// Checks an individual permission for a user and store front, used by other functions, no direct calls
		/// </summary>
		/// <param name="userProfile"></param>
		/// <param name="storeFront"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		private static bool CheckSinglePermission(UserProfile userProfile, StoreFront storeFront, GStoreAction action)
		{
			if (storeFront == null)
			{
				return false;
			}
			return userProfile.ClientUserRoles.AsQueryable()
				.WhereIsActiveAndIsInScope(storeFront)
				.Any(cur => cur.ClientRole.ClientRoleActions.AsQueryable().WhereIsActive().Where(cra => cra.GStoreActionId == action).Any());
		}

		/// <summary>
		/// Returns a list of ClientRoleAction for a user.  Note: This is for database mapping only, identity roles like systemadmin will only return roles explicitly linked
		/// </summary>
		/// <param name="db"></param>
		/// <param name="userProfile"></param>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static List<ClientRoleAction> Authorization_ListDefinedRoleActions(this Data.IGstoreDb db, UserProfile userProfile, StoreFront storeFront)
		{
			//returns true if there is an active ClientUserRoleAction 
			return db.ClientRoleActions
				.Where(cra => cra.ClientRole.ClientUserRoles.AsQueryable().WhereIsActiveAndIsInScope(storeFront).Any(cur => cur.UserProfileId == userProfile.UserProfileId))
				.WhereIsActive()
				.ToList();
		}

		public static IQueryable<ClientUserRole> WhereIsActiveAndIsInScope(this IQueryable<ClientUserRole> query, StoreFront storeFront)
		{
			return query.WhereIsActive()
				.Where(cur => cur.ClientId == storeFront.ClientId && (cur.ScopeStoreFront == null || cur.ScopeStoreFrontId == storeFront.StoreFrontId));
		}
	}
}