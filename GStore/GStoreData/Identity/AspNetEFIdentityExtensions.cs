using System;
using System.Collections.Generic;
using System.Linq;

namespace GStoreData.Identity
{
	public static class AspNetEFIdentityExtensions
	{
		public static Models.UserProfile GetUserProfile(this AspNetIdentityUser user, IGstoreDb GStoreDb, bool throwExceptionIfNotFound = false)
		{
			Models.UserProfile profile = GStoreDb.UserProfiles.SingleOrDefault(up => up.UserId == user.Id);
			if (throwExceptionIfNotFound && profile == null)
			{
				throw new ApplicationException("Cannot find user profile for user id: " + user.Id
					+ "\n\tEmail: " + user.Email
					+ "\n\tUserName: " + user.UserName);
			}
			return profile;
		}

		public static List<AspNetIdentityUser> AspNetIdentityUsers (this AspNetIdentityRole role)
		{
			return role.Users.Select(r => r.User).ToList();
		}

		public static List<AspNetIdentityRole> AspNetIdentityRoles (this AspNetIdentityUser user)
		{
			return user.Roles.Select(r => r.Role).ToList();
		}
	}
}