using System;
using System.Collections.Generic;
using System.Linq;
using GStoreData.Models;

namespace GStoreData
{
	public static class UserProfileExtensions
	{
		/// <summary>
		/// Gets the current user profile for a logged on user, or null if anonymous; uses CachedUserProfile from context if available
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="throwErrorIfNotFound"></param>
		/// <returns></returns>
		public static UserProfile GetCurrentUserProfile(this IGstoreDb ctx, bool throwErrorIfNotFound, bool throwErrorIfContextNameIsBlank)
		{
			if (ctx.CachedUserProfile != null)
			{
				return ctx.CachedUserProfile;
			}

			if (string.IsNullOrEmpty(ctx.UserName))
			{
				if (throwErrorIfContextNameIsBlank)
				{
					throw new ApplicationException("User name in GStore context is blank, cannot look up user profile. User may be anonymous");
				}
				return null;
			}

			UserProfile userProfile = ctx.UserProfiles.Where(up => up.UserName.ToLower() == ctx.UserName.ToLower()).WhereIsActive().SingleOrDefault();
			if (userProfile == null && throwErrorIfNotFound)
			{
				string errorMessage = "User Profile not found for UserName: " + ctx.UserName;

				var inactiveQuery = ctx.UserProfiles.Where(up => up.UserName.ToLower() == ctx.UserName.ToLower());
				List<UserProfile> inactiveProfiles = inactiveQuery.ToList();
				if (inactiveProfiles.Count() == 0)
				{
					errorMessage += "\n - No matching profile found active or not";
					throw new ApplicationException(errorMessage);
				}
				if (inactiveProfiles.Count > 1)
				{
					errorMessage += "\n - Inactive or Active Profiles Found: " + inactiveProfiles.Count
						+ "\n Error: profiles are out of unique constraint. Only one profile may exist per email address.";
					throw new ApplicationException(errorMessage);
				}

				errorMessage += "\n - Inactive Profile Found: " + inactiveProfiles.Count
					+ "\n - Note: Only one profile can exist per email address"
					+ "\n - UserProfileId: " + inactiveProfiles[0].UserProfileId.ToString()
					+ "\n - UserId: " + inactiveProfiles[0].UserId.ToString()
					+ "\n - UserName: " + inactiveProfiles[0].UserName
					+ "\n - Email: " + inactiveProfiles[0].Email
					+ "\n - IsPending: " + inactiveProfiles[0].IsPending
					+ "\n - StartDateTimeUtc(local): " + inactiveProfiles[0].StartDateTimeUtc.ToLocalTime()
					+ "\n - EndDateTimeUtc(local): " + inactiveProfiles[0].EndDateTimeUtc.ToLocalTime();

				if (inactiveProfiles[0].StoreFront == null)
				{
					errorMessage += "\n - No storefront or client, may be system admin.";
				}
				else
				{
					errorMessage += "\n - StoreFront [" + inactiveProfiles[0].StoreFront.StoreFrontId + "]: " + inactiveProfiles[0].StoreFront.CurrentConfigOrAny().Name
					+ "\n - Client [" + inactiveProfiles[0].StoreFront.Client.ClientId + "]: " + inactiveProfiles[0].StoreFront.Client.Name;
				}

				throw new ApplicationException(errorMessage);

			}

			ctx.CachedUserProfile = userProfile;
			return userProfile;
		}

		public static UserProfile GetUserProfileByAspNetUserId(this IGstoreDb ctx, string aspNetUserId, bool throwErrorIfNotFound = true)
		{
			if (string.IsNullOrEmpty(aspNetUserId))
			{
				if (throwErrorIfNotFound)
				{
					throw new ApplicationException("Active User Profile not found, aspNetUserId is blank");
				}
				return null;
			}

			UserProfile userProfile = ctx.UserProfiles.All().WhereIsActive().SingleOrDefault(up => up.UserId.ToLower() == aspNetUserId.ToLower());
			if (userProfile == null && throwErrorIfNotFound)
			{
				List<UserProfile> results = ctx.UserProfiles.Where(up => up.UserId.ToLower() == aspNetUserId.ToLower()).ToList();
				if (results.Count == 0)
				{
					throw new ApplicationException("Active User Profile not found for UserId: " + aspNetUserId + " \n User Profile is missing from UserProfiles table. Add profile for this user.");
				}
				if (results.Count > 1)
				{
					throw new ApplicationException("Active User Profile not found for UserId: " + aspNetUserId + " \n There appear to be multiple profiles; delete extra active profiles from UserProfiles table or make them inactive.");
				}
				if (results[0].IsPending)
				{
					throw new ApplicationException("Active User Profile not found for UserId: " + aspNetUserId + " \n User Profile is inactive. Set Active = true or add a new active profile");
				}

				throw new ApplicationException("Active User Profile not found for UserId: " + aspNetUserId
					+ " \n User Profile or client or store front may be inactive"
					+ " \n Is Pending: " + results[0].IsPending.ToString()
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					);

			}
			return userProfile;
		}

		public static UserProfile GetUserProfileByIdentityUser(this IGstoreDb ctx, System.Security.Principal.IPrincipal user, bool throwErrorIfNotFound = true)
		{
			return ctx.GetUserProfileByEmail(user.Identity.Name, throwErrorIfNotFound);
		}

		public static UserProfile GetUserProfileByEmail(this IGstoreDb ctx, string email, bool throwErrorIfNotFound = true)
		{
			if (string.IsNullOrEmpty(email))
			{
				if (throwErrorIfNotFound)
				{
					throw new ApplicationException("Active User Profile not found, email address is blank");
				}
				return null;
			}

			UserProfile userProfile = ctx.UserProfiles.All().WhereIsActive().SingleOrDefault(up => up.Email.ToLower() == email.ToLower());
			if (userProfile == null && throwErrorIfNotFound)
			{
				List<UserProfile> results = ctx.UserProfiles.Where(up => up.Email.ToLower() == email.ToLower()).ToList();
				if (results.Count == 0)
				{
					throw new ApplicationException("Active User Profile not found for email: " + email + " \n User Profile is missing from UserProfiles table. Add profile for this user.");
				}
				if (results.Count > 1)
				{
					throw new ApplicationException("Active User Profile not found for email: " + email + " \n There appear to be multiple profiles; delete extra active profiles from UserProfiles table or make them inactive.");
				}
				if (results[0].IsPending)
				{
					throw new ApplicationException("Active User Profile not found for email: " + email + " \n User Profile is inactive. Set Active = true or add a new active profile");
				}

				throw new ApplicationException("Active User Profile not found for email: " + email
					+ " \n User Profile may be inactive"
					+ " \n IsPending: " + results[0].IsPending.ToString()
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					);

			}
			return userProfile;
		}



		/// <summary>
		/// Checks if the user is in the ASP.Net Identity User Role "System Admin"
		/// </summary>
		/// <param name="profile"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public static bool AspNetIdentityUserIsInRoleSystemAdmin(this UserProfile userProfile)
		{
			if (userProfile == null)
			{
				return false;
			}
			return userProfile.AspNetIdentityUser().IsInRole("SystemAdmin");
		}

		/// <summary>
		/// Checks if the user is in a specific ASP.Net Identity User Role; i.e. "SystemAdmin"
		/// </summary>
		/// <param name="profile"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public static bool AspNetIdentityUserIsInRole(this UserProfile userProfile, string roleName)
		{
			if (userProfile == null)
			{
				return false;
			}
			return userProfile.AspNetIdentityUser().IsInRole(roleName);
		}

		public static Identity.AspNetIdentityUser AspNetIdentityUser(this UserProfile userProfile)
		{
			if (userProfile == null)
			{
				return null;
			}
			Identity.AspNetIdentityContext identityCtx = new Identity.AspNetIdentityContext();
			return identityCtx.AspNetIdentityUser(userProfile);
		}

		public static List<Identity.AspNetIdentityRole> AspNetIdentityRoles(this UserProfile userProfile)
		{
			if (userProfile == null)
			{
				return new List<Identity.AspNetIdentityRole>();
			}
			Identity.AspNetIdentityContext identityCtx = new Identity.AspNetIdentityContext();
			return identityCtx.AspNetIdentityRoles(userProfile);
		}

		public static ICollection<Identity.AspNetIdentityUserRole> AspNetIdentityUserRoles(this UserProfile userProfile)
		{
			Identity.AspNetIdentityContext identityCtx = new Identity.AspNetIdentityContext();
			return identityCtx.AspNetIdentityUserRoles(userProfile);
		}

		public static ICollection<Identity.AspNetIdentityUserClaim> AspNetIdentityUserClaims(this UserProfile userProfile)
		{
			Identity.AspNetIdentityContext identityCtx = new Identity.AspNetIdentityContext();
			return identityCtx.AspNetIdentityUserClaims(userProfile);
		}

		public static ICollection<Identity.AspNetIdentityUserLogin> AspNetIdentityUserLogins(this UserProfile userProfile)
		{
			Identity.AspNetIdentityContext identityCtx = new Identity.AspNetIdentityContext();
			return identityCtx.AspNetIdentityUserLogins(userProfile);
		}

		public static bool IsActiveDirect(this UserProfile record)
		{
			return record.IsActiveDirect(DateTime.UtcNow);
		}
		public static bool IsActiveDirect(this UserProfile record, DateTime dateTime)
		{
			if (!record.IsPending && (record.StartDateTimeUtc < dateTime) && (record.EndDateTimeUtc > dateTime))
			{
				return true;
			}
			return false;
		}

		public static void SetDefaultsForNew(this UserProfile profile, Client client, StoreFront storeFront)
		{
			if (client != null)
			{
				profile.Client = client;
				profile.ClientId = client.ClientId;
				profile.TimeZoneId = client.TimeZoneId;
			}

			if (storeFront != null)
			{
				profile.StoreFront = storeFront;
				profile.StoreFrontId = storeFront.StoreFrontId;
				StoreFrontConfiguration storeFrontConfig = storeFront.CurrentConfigOrAny();
				if (storeFrontConfig != null)
				{
					profile.TimeZoneId = storeFrontConfig.TimeZoneId;
				}
			}

			profile.EntryDateTime = DateTime.UtcNow;
			profile.IsPending = true;
			profile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			profile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);

		}



	}
}