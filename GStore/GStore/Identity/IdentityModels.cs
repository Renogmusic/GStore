using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Security.Principal;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;

namespace GStore.Identity
{
	/// <summary>
	/// Formerly called ApplicationUser by startup template
	/// </summary>
	public class AspNetIdentityUser : Microsoft.AspNet.Identity.EntityFramework.IdentityUser<string, AspNetIdentityUserLogin, AspNetIdentityUserRole, AspNetIdentityUserClaim>
	{
		public AspNetIdentityUser()
		{
			this.Id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Creates a new user id with user name as the user id
		/// </summary>
		/// <param name="userId"></param>
		public AspNetIdentityUser(string userId)
		{
			this.Id = userId;
			this.UserName = userId;
		}

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(Microsoft.AspNet.Identity.UserManager<AspNetIdentityUser, string> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}

		//from webapi for OWin
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(Microsoft.AspNet.Identity.UserManager<AspNetIdentityUser, string> manager, string authenticationType)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
			// Add custom user claims here
			return userIdentity;
		}
	}

	public class AspNetIdentityRole : Microsoft.AspNet.Identity.EntityFramework.IdentityRole<string, AspNetIdentityUserRole>
	{
		public AspNetIdentityRole()
		{
			base.Id = Guid.NewGuid().ToString();
		}
		public AspNetIdentityRole(string roleName)
		{
			this.Id = roleName;
			this.Name = roleName;
		}
	}

	public class AspNetIdentityUserLogin : Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>
	{
		public AspNetIdentityUserLogin()
		{
		}
		public AspNetIdentityUserLogin(string userId, string loginProvider, string providerKey)
		{
			base.UserId = userId;
			base.LoginProvider = loginProvider;
			base.ProviderKey = providerKey;
		}
	}

	public class AspNetIdentityUserRole : Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>
	{
		public AspNetIdentityUserRole()
		{
		}
		public AspNetIdentityUserRole(string userId, string roleId)
		{
			base.UserId = userId;
			base.RoleId = RoleId;
		}
	}

	public class AspNetIdentityUserClaim : Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>
	{
		public AspNetIdentityUserClaim()
		{
		}
		public AspNetIdentityUserClaim(string userId, string claimType, string claimValue)
		{
			base.ClaimType = claimType;
			base.ClaimValue = claimValue;
			base.UserId = userId;
		}
	}

	public class AspNetIdentityContext : Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext<AspNetIdentityUser, AspNetIdentityRole, string, AspNetIdentityUserLogin, AspNetIdentityUserRole, AspNetIdentityUserClaim>
	{
		public AspNetIdentityContext()
			: base(Properties.Settings.Default.AspNetIdentityNameOrConnectionString)
		{
		}

		public static AspNetIdentityContext Create()
		{
			return new AspNetIdentityContext();
		}

		protected AspNetIdentityRoleManager _cachedRoleManager = null;
		protected AspNetIdentityRoleManager RoleManager
		{
			get
			{
				if (_cachedRoleManager == null)
				{
					Microsoft.AspNet.Identity.IRoleStore<AspNetIdentityRole, string> roleStore = new Microsoft.AspNet.Identity.EntityFramework.RoleStore<AspNetIdentityRole, string, AspNetIdentityUserRole>(this);
					AspNetIdentityRoleManager roleManager = new AspNetIdentityRoleManager(roleStore);
					_cachedRoleManager = roleManager;
				}
				return _cachedRoleManager;
			}
		}

		protected AspNetIdentityUserManager _cachedUserManager = null;
		protected AspNetIdentityUserManager UserManager
		{
			get
			{
				if (_cachedUserManager == null)
				{
					Microsoft.AspNet.Identity.IUserStore<AspNetIdentityUser, string> userStore = new Microsoft.AspNet.Identity.EntityFramework.UserStore<AspNetIdentityUser, AspNetIdentityRole, string, AspNetIdentityUserLogin, AspNetIdentityUserRole, AspNetIdentityUserClaim>(this);
					AspNetIdentityUserManager userManager = new AspNetIdentityUserManager(userStore);
					_cachedUserManager = userManager;
				}
				return _cachedUserManager;
			}
		}

		/// <summary>
		/// Checks if a role exists (by ID), returns true if exists, false if not
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public bool AspNetRoleExists(string roleName)
		{
			try
			{
				return this.RoleManager.RoleExists(roleName);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error performing role check. Database error checking roles", ex);
			}
		}

		/// <summary>
		/// Checks if a role exists (by ID), returns true if exists, false if not
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public bool AspNetUserExists(string userName)
		{
			bool userExists = false;
			try
			{
				userExists = this.UserManager.Users.Where(user => user.UserName == userName).Any();
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error performing user check. Database error checking users", ex);
			}
			return userExists;
		}

		public void CreateRoleIfNotExists(string roleName)
		{
			bool exists = false;
			try
			{
				exists = this.RoleManager.RoleExists(roleName);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error performing role check. Database error checking roles", ex);
			}

			if (!exists)
			{
				AspNetIdentityRole role = new AspNetIdentityRole(roleName);
				try
				{
					this.RoleManager.Create(role);
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Error creating role: " + roleName, ex);
				}
			}
		}

		public AspNetIdentityUser CreateUserIfNotExists(string userName, string email, string phone, string password)
		{
			if (!this.AspNetUserExists(userName))
			{
				AspNetIdentityUser user = new AspNetIdentityUser(userName);
				user.UserName = userName;
				user.Email = email;
				user.PhoneNumber = phone;
				user.PhoneNumberConfirmed = false;
				user.LockoutEnabled = true;
				IdentityResult resultUserAdd = null;
				try
				{
					resultUserAdd = this.UserManager.Create(user, password);
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Error creating new user UserName: " + userName, ex);
				}
				if (!resultUserAdd.Succeeded)
				{
					string userAddError = string.Empty;
					foreach (string item in resultUserAdd.Errors)
					{
						userAddError += "Error: " + item + "\n ";
					}
					throw new ApplicationException("Error adding user: " + userName + ".\n " + userAddError);
				}
			}
			return this.Users.Single(user => user.UserName == userName);
		}

		public AspNetIdentityUser CreateSystemAdminUserIfNotExists(string userName, string email, string phone, string password, string roleName = "SystemAdmin")
		{
			this.CreateRoleIfNotExists(roleName);

			if (!this.AspNetUserExists(userName))
			{
				AspNetIdentityUser adminUser = new AspNetIdentityUser(userName);
				adminUser.UserName = userName;
				adminUser.Email = email;
				adminUser.PhoneNumber = phone;
				adminUser.PhoneNumberConfirmed = true;
				adminUser.LockoutEnabled = false;
				IdentityResult resultUserAdd = null;
				try
				{
					resultUserAdd = this.UserManager.Create(adminUser, password);
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Error creating new user (system admin) UserName: " + userName, ex);
				}
				if (!resultUserAdd.Succeeded)
				{
					string userAddError = string.Empty;
					foreach (string item in resultUserAdd.Errors)
					{
						userAddError += "Error: " + item + "\n ";
					}
					throw new ApplicationException("Error adding admin user: " + userName + ".\n " + userAddError);
				}
				IdentityResult resultRoleAdd = this.UserManager.AddToRole(adminUser.Id, roleName);
				if (!resultRoleAdd.Succeeded)
				{
					{
						string roleAddError = string.Empty;
						foreach (string item in resultUserAdd.Errors)
						{
							roleAddError += "Error: " + item + "\n ";
						}
						throw new ApplicationException("Error adding admin user to role: " + roleName + ".\n " + roleAddError);
					}
				}
			}

			return this.Users.Single(user => user.UserName == userName);
		}

		public Identity.AspNetIdentityUser AspNetIdentityUser(GStore.Models.UserProfile userProfile)
		{
			Identity.AspNetIdentityContext ctx = new Identity.AspNetIdentityContext();
			Identity.AspNetIdentityUser user = ctx.Users.SingleOrDefault(usr => usr.Id == userProfile.UserId);
			if (user == null)
			{
				throw new ApplicationException("AspNetUser not found. Check for extra profiles in UserProfile table with duplicate email address."
					+ "\n\tUserId (from profile): " + userProfile.UserId
					+ "\n\tEmail (from profile): " + userProfile.Email
					+ "\n\tUserProfileId (from profile): " + userProfile.UserProfileId);
			}
			return user;
		}

		public List<AspNetIdentityRole> AspNetIdentityRoles(GStore.Models.UserProfile userProfile)
		{
			Identity.AspNetIdentityUser user = AspNetIdentityUser(userProfile);
			if (user == null)
			{
				return null;
			}

			var query = from userRole in user.Roles
						join role in this.Roles on userRole.RoleId equals role.Id
						select role;

			return query.ToList();
		}

		public ICollection<AspNetIdentityUserRole> AspNetIdentityUserRoles(GStore.Models.UserProfile userProfile)
		{
			Identity.AspNetIdentityUser user = AspNetIdentityUser(userProfile);
			if (user == null)
			{
				return null;
			}
			return user.Roles;
		}

		public ICollection<AspNetIdentityUserClaim> AspNetIdentityUserClaims(GStore.Models.UserProfile userProfile)
		{
			Identity.AspNetIdentityUser user = AspNetIdentityUser(userProfile);
			if (user == null)
			{
				return null;
			}
			return user.Claims;
		}

		public ICollection<AspNetIdentityUserLogin> AspNetIdentityUserLogins(Models.UserProfile userProfile)
		{
			Identity.AspNetIdentityUser user = AspNetIdentityUser(userProfile);
			if (user == null)
			{
				return null;
			}

			return user.Logins;
		}



	}
}