using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Models.Extensions;
using GStore.Models.BaseClasses;

namespace GStore.Models.Extensions
{
	public static class GStoreDBExtensions
	{
		/// <summary>
		/// Gets the current store front, or null if anonymous; uses CachedStoreFront from context if available
		/// </summary>
		/// <param name="db"></param>
		/// <param name="request"></param>
		/// <param name="throwErrorIfNotFound"></param>
		/// <returns></returns>
		public static StoreFront GetCurrentStoreFront(this IGstoreDb db, HttpRequestBase request, bool throwErrorIfNotFound = true)
		{
			//if context has already set the current store front, return it
			if (db.CachedStoreFront != null)
			{
				return db.CachedStoreFront;
			}

			//verify database has been seeded and not blank
			if (db.StoreBindings.IsEmpty())
			{
				throw new ApplicationException("No Store bindings in database, be sure to run the Seed method");
			}

			if (request == null)
			{
				throw new ApplicationException("Request context is null, cannot get current store front");
			}
			string hostName = request.Url.Host.Trim().ToLower();
			string path = request.ApplicationPath.ToLower();
			int port = request.Url.Port;

			IQueryable<StoreBinding> query = db.StoreBindings.Where(
				sb => (sb.HostName.ToLower() == hostName)
					&& (sb.Port == port)
					&& (sb.RootPath.ToLower() == path)
			).WhereIsActive().OrderBy(sb => sb.Order).ThenByDescending(sb=> sb.UpdateDateTimeUtc) ;
			StoreBinding storeBinding = query.FirstOrDefault();
			
			if (storeBinding == null)
			{
				string errorMessage = "Cannot find active store binding for hostname: " + hostName + " Port:" + port + " Path:" + path;

				//why can't we find a binding?

				List<StoreBinding> inactiveBindings = db.StoreBindings.Where(
					sb => (sb.HostName.ToLower() == hostName)
						&& (sb.Port == port)
						&& (sb.RootPath.ToLower() == path)
				).OrderBy(sb=>sb.Order).ThenByDescending(sb=>sb.UpdateDateTimeUtc) .ToList();

				if (inactiveBindings.Count == 0)
				{
					//a-no url match (show no store sign-up page) (also hostname hackers)
					errorMessage += "\n No matching bindings found.";
					throw new Exceptions.NoMatchingBindingException(errorMessage, request.Url);
				}

				//b-url match, but client or storefront is pending or inactive (show not active page)
				errorMessage += "\n-Inactive Bindings found: " + inactiveBindings.Count
					+ "\n---First (most likely to be selected)---"
					+ "\n - StoreBindingId: " + inactiveBindings[0].StoreBindingId
					+ "\n - StoreBinding.IsActiveDirect: " + inactiveBindings[0].IsActiveDirect()
					+ "\n - StoreBinding.IsPending: " + inactiveBindings[0].IsPending.ToString()
					+ "\n - StoreBinding.StartDateTimeUtc(local): " + inactiveBindings[0].StartDateTimeUtc.ToLocalTime()
					+ "\n - StoreBinding.EndDateTimeUtc(local): " + inactiveBindings[0].EndDateTimeUtc.ToLocalTime()

					+ "\n - StoreFrontId: " + inactiveBindings[0].StoreFront.StoreFrontId
					+ "\n - StoreFrontId.IsActiveDirect: " + inactiveBindings[0].StoreFront.IsActiveDirect()
					+ "\n - StoreFrontId.IsPending: " + inactiveBindings[0].StoreFront.IsPending.ToString()
					+ "\n - StoreFrontId.StartDateTimeUtc(local): " + inactiveBindings[0].StoreFront.StartDateTimeUtc.ToLocalTime()
					+ "\n - StoreFrontId.EndDateTimeUtc(local): " + inactiveBindings[0].StoreFront.EndDateTimeUtc.ToLocalTime()

					+ "\n - ClientId: " + inactiveBindings[0].Client.ClientId
					+ "\n - Client.IsActiveDirect: " + inactiveBindings[0].Client.IsActiveDirect()
					+ "\n - Client.IsPending: " + inactiveBindings[0].Client.IsPending.ToString()
					+ "\n - Client.StartDateTimeUtc(local): " + inactiveBindings[0].Client.StartDateTimeUtc.ToLocalTime()
					+ "\n - Client.EndDateTimeUtc(local): " + inactiveBindings[0].Client.EndDateTimeUtc.ToLocalTime();


				throw new Exceptions.StoreFrontInactiveException(errorMessage, request.Url, inactiveBindings[0].StoreFront);
			}

			StoreFront storeFront = storeBinding.StoreFront;

			if (throwErrorIfNotFound && storeFront == null)
			{
				throw new ApplicationException("Store front not found for this host: " + request.Url.Host
					+ " Port: " + request.Url.Port
					+ " ApplicationPath: " + request.ApplicationPath
					+ " Url:" + request.Url);
			}

			db.CachedStoreFront = storeFront;
			return storeFront;
		}

		/// <summary>
		/// Gets the current user profile for a logged on user, or null if anonymous; uses CachedUserProfile from context if available
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="throwErrorIfNotFound"></param>
		/// <returns></returns>
		public static UserProfile GetCurrentUserProfile(this IGstoreDb ctx, bool throwErrorIfNotFound = true)
		{
			if (ctx.CachedUserProfile != null)
			{
				return ctx.CachedUserProfile;
			}

			if (string.IsNullOrEmpty(ctx.UserName))
			{
				if (throwErrorIfNotFound)
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
					+ "\n - Active: " + inactiveProfiles[0].Active
					+ "\n - StartDateTimeUtc(local): " + inactiveProfiles[0].StartDateTimeUtc.ToLocalTime()
					+ "\n - EndDateTimeUtc(local): " + inactiveProfiles[0].EndDateTimeUtc.ToLocalTime();

				if (inactiveProfiles[0].StoreFront == null)
				{
					errorMessage += "\n - No storefront or client, may be system admin.";
				}
				else
				{
					errorMessage += "\n - StoreFront [" + inactiveProfiles[0].StoreFront.StoreFrontId + "]: " + inactiveProfiles[0].StoreFront.Name
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
				if (!results[0].Active)
				{
					throw new ApplicationException("Active User Profile not found for UserId: " + aspNetUserId + " \n User Profile is inactive. Set Active = true or add a new active profile");
				}

				throw new ApplicationException("Active User Profile not found for UserId: " + aspNetUserId
					+ " \n User Profile or client or store front may be inactive"
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					+ " \n Active flag: " + results[0].Active.ToString());

			}
			return userProfile;
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
				if (!results[0].Active)
				{
					throw new ApplicationException("Active User Profile not found for email: " + email + " \n User Profile is inactive. Set Active = true or add a new active profile");
				}

				throw new ApplicationException("Active User Profile not found for email: " + email
					+ " \n User Profile may be inactive"
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					+ " \n StartDateTime: " + results[0].StartDateTimeUtc.ToLocalTime()
					+ " \n Active flag: " + results[0].Active.ToString());

			}
			return userProfile;
		}

		public static Page GetCurrentPage(this StoreFront storeFront, HttpRequestBase request, bool throwErrorIfNotFound = true)
		{
			string url = "/" + request.Url.AbsolutePath.Trim('/');
			return storeFront.GetCurrentPage(url, throwErrorIfNotFound);
		}
		public static Page GetCurrentPage(this StoreFront storeFront, string url, bool throwErrorIfNotFound = true)
		{
			string urlLower = url.ToLower();
			var query = storeFront.Pages.Where(p => p.Url.ToLower() == urlLower).AsQueryable().WhereIsActive().OrderBy(p=> p.Order).ThenByDescending(p => p.UpdateDateTimeUtc);
			Page page = query.FirstOrDefault();
			if (throwErrorIfNotFound && page == null)
			{
				string errorMessage = "Active Page not found for url: " + url
					+ "\n-Store Front [" + storeFront.StoreFrontId + "]: " + storeFront.Name
					+ "\n-Client [" + storeFront.Client.ClientId + "]: " + storeFront.Client.Name;

				var inactivePagesQuery = storeFront.Pages.Where(p => p.Url.ToLower() == urlLower).AsQueryable().OrderBy(p=> p.Order).ThenByDescending(p => p.UpdateDateTimeUtc);
				List<Page> inactivePages = inactivePagesQuery.ToList();

				if (inactivePages.Count == 0)
				{
					throw new Exceptions.DynamicPageNotFoundException(errorMessage, url, storeFront);
				}
				errorMessage += "\n-Matching Pages found: " + inactivePages.Count
					+ "\n---First (most likely to be selected)---"
					+ "\n - PageId: " + inactivePages[0].PageId
					+ "\n - Page.Name: " + inactivePages[0].Name
					+ "\n - Page.IsActiveDirect: " + inactivePages[0].IsActiveDirect()
					+ "\n - Page.IsPending: " + inactivePages[0].IsPending.ToString()
					+ "\n - Page.StartDateTimeUtc(local): " + inactivePages[0].StartDateTimeUtc.ToLocalTime()
					+ "\n - Page.EndDateTimeUtc(local): " + inactivePages[0].EndDateTimeUtc.ToLocalTime()
					+ "\n - StoreFront [" + inactivePages[0].StoreFront.StoreFrontId + "]: " + inactivePages[0].StoreFront.Name
					+ "\n - Client [" + inactivePages[0].StoreFront.Client.ClientId + "]: " + inactivePages[0].StoreFront.Client.Name;

				throw new Exceptions.DynamicPageInactiveException(errorMessage, url, storeFront);

			}
			return page;
		}

		public static IQueryable<UserProfile> WhereIsActive(this IQueryable<UserProfile> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<UserProfile> WhereIsActive(this IQueryable<UserProfile> query, DateTime dateTimeUtc)
		{
			return query.Where(up =>
				up.Active
				&& (up.StartDateTimeUtc < dateTimeUtc)
				&& (up.EndDateTimeUtc > dateTimeUtc)
				);
		}


		public static IQueryable<Client> WhereIsActive(this IQueryable<Client> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<Client> WhereIsActive(this IQueryable<Client> query, DateTime dateTimeUtc)
		{
			return query.Where(c =>
				!c.IsPending
				&& (c.StartDateTimeUtc < dateTimeUtc)
				&& (c.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<StoreFront> WhereIsActive(this IQueryable<StoreFront> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<StoreFront> WhereIsActive(this IQueryable<StoreFront> query, DateTime dateTimeUtc)
		{
			return query.Where(sf =>
				!sf.IsPending
				&& (sf.StartDateTimeUtc < dateTimeUtc)
				&& (sf.EndDateTimeUtc > dateTimeUtc)
				&& (!sf.Client.IsPending)
				&& (sf.Client.StartDateTimeUtc < dateTimeUtc)
				&& (sf.Client.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<StoreBinding> WhereIsActive(this IQueryable<StoreBinding> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<StoreBinding> WhereIsActive(this IQueryable<StoreBinding> query, DateTime dateTimeUtc)
		{
			return query.Where(sb =>
				!sb.IsPending
				&& (sb.StartDateTimeUtc < dateTimeUtc)
				&& (sb.EndDateTimeUtc > dateTimeUtc)
				&& (!sb.Client.IsPending)
				&& (sb.Client.StartDateTimeUtc < dateTimeUtc)
				&& (sb.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!sb.StoreFront.IsPending)
				&& (sb.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (sb.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<Page> WhereIsActive(this IQueryable<Page> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<Page> WhereIsActive(this IQueryable<Page> query, DateTime dateTimeUtc)
		{
			return query.Where(p =>
				!p.IsPending
				&& (p.StartDateTimeUtc < dateTimeUtc) 
				&& (p.EndDateTimeUtc > dateTimeUtc)
				&& (!p.Client.IsPending)
				&& (p.Client.StartDateTimeUtc < dateTimeUtc) 
				&& (p.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!p.StoreFront.IsPending)
				&& (p.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (p.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}


		public static void HandleLockedOutNotification(this Controllers.BaseClass.BaseController controller, UserProfile profile)
		{

			//todo: notify system admin of logon failures and lockouts

			//notify user through site message if their account is locked out, and it has been at least an hour since they were last told they are locked out
			if (profile.LastLockoutFailureNoticeDateTimeUtc != null
				&& (DateTime.UtcNow - profile.LastLockoutFailureNoticeDateTimeUtc.Value).Hours < 1)
			{
				//user has been notified in the past hour, do not re-notify
				return;
			}

			IGstoreDb db = controller.GStoreDb.NewContext();
			UserProfile accountAdmin = controller.CurrentStoreFront.AccountAdmin;

			Notification notification = db.Notifications.Create();
			notification.UserProfileId = profile.UserProfileId;
			notification.From = (accountAdmin == null ? "System Administrator" : accountAdmin.FullName);
			notification.FromUserProfileId = (accountAdmin == null ? 0 : accountAdmin.UserProfileId);
			notification.To = profile.FullName;
			notification.Importance = "Low";
			notification.Subject = "Login failure for " + controller.Request.Url.Host;
			notification.UrlHost = controller.Request.Url.Host;
			if (!controller.Request.Url.IsDefaultPort)
			{
				notification.UrlHost += ":" + controller.Request.Url.Port;
			}

			notification.BaseUrl = controller.Url.Action("Details", "Notifications", new { id = "" });
			notification.Message = "Somebody tried to log on as you with the wrong password at " + controller.Request.Url.Host
				+ " \n\nFor security reasons, your account has been locked out for 5 minutes."
				+ " \n\nIf this was you, please disregard this message and try again in 5 minutes."
				+ " \n\nIf you forgot your password, you can reset it at http://" + controller.Request.Url.Host + controller.Url.Action("ForgotPassword", "Account")
				+ " \n\nIf this was not you, the below information may be helpful."
				+ " \n\nIP Address: " + controller.Request.UserHostAddress
				+ " \nHost Name: " + controller.Request.UserHostName;

			db.Notifications.Add(notification);
			db.SaveChanges();

			UserProfile profileUpdate = db.UserProfiles.FindById(profile.UserProfileId);
			profileUpdate.LastLockoutFailureNoticeDateTimeUtc = DateTime.UtcNow;
			db.SaveChangesDirect();

		}

		public static void HandleNewUserRegisteredNotifications(this Controllers.BaseClass.BaseController controller, UserProfile newProfile, string notificationBaseUrl)
		{
			UserProfile welcomeUserProfile = controller.CurrentStoreFront.WelcomePerson;
			HttpRequestBase request = controller.Request;
			IGstoreDb db = controller.GStoreDb.NewContext(newProfile.UserName);

			Notification notification = db.Notifications.Create();
			notification.From = (welcomeUserProfile == null ? "System Administrator" : welcomeUserProfile.FullName);
			notification.FromUserProfileId = (welcomeUserProfile == null ? 0 : welcomeUserProfile.UserProfileId);
			notification.UserProfileId = newProfile.UserProfileId;
			notification.To = newProfile.FullName;
			notification.Importance = "Normal";
			notification.Subject = "Welcome!";
			notification.UrlHost = request.Url.Host;
			if (!request.Url.IsDefaultPort)
			{
				notification.UrlHost += ":" + request.Url.Port;
			}

			notification.BaseUrl = notificationBaseUrl;
			notification.Message = "Welcome to the web site!\nEnjoy your stay, and email us if you have any questions, suggestions, feedback, or anything!"
				+ "\n\n" + Properties.Settings.Default.IdentitySendGridMailFromName + " - " + Properties.Settings.Default.IdentitySendGridMailFromEmail;


			NotificationLink link1 = db.NotificationLinks.Create();
			link1.Order = 1;
			link1.IsExternal = false;
			link1.LinkText = "My Songs";
			link1.Url = "/Songs";
			link1.Notification = notification;
			db.NotificationLinks.Add(link1);

			NotificationLink link2 = db.NotificationLinks.Create();
			link2.Order = 2;
			link2.IsExternal = false;
			link2.LinkText = "Kids Apps";
			link2.Url = "/KidApps";
			link2.Notification = notification;
			db.NotificationLinks.Add(link2);

			NotificationLink link3 = db.NotificationLinks.Create();
			link3.Order = 3;
			link3.IsExternal = false;
			link3.LinkText = "My Experimental Apps";
			link3.Url = "/PlayApps";
			link3.Notification = notification;
			db.NotificationLinks.Add(link3);

			NotificationLink link4 = db.NotificationLinks.Create();
			link4.Order = 4;
			link4.IsExternal = true;
			link4.LinkText = "Click Here to Email Me";
			link4.Url = "mailto:renogmusic@yahoo.com";
			link4.Notification = notification;
			db.NotificationLinks.Add(link4);

			db.Notifications.Add(notification);
			db.SaveChanges();

			UserProfile registeredNotify = controller.CurrentStoreFront.RegisteredNotify;
			if (registeredNotify != null)
			{
				string messageBody = "New User Registered on " + db.GetCurrentStoreFront(request, true).Name + "!"
					+ "\n-Name: " + newProfile.FullName
					+ "\n-Email: " + newProfile.Email
					+ "\n-Date/Time: " + DateTime.Now.ToString()
					+ "\n-Send Me More Info: " + newProfile.SendMoreInfoToEmail.ToString()
					+ "\n-Notify Of Site Updates: " + newProfile.NotifyOfSiteUpdatesToEmail.ToString()
					+ "\nSignup Notes: "
					+ "\n-Store Front: " + controller.CurrentStoreFront.Name
					+ "\n-Company: " + controller.CurrentClient.Name
					+ "\n" + newProfile.SignupNotes
					+ "\n" + newProfile.SignupNotes
					+ "\n\n-UserProfileId: " + newProfile.UserProfileId
					+ "\n-IP Address: " + request.UserHostAddress;

				Notification newUserNotify = db.Notifications.Create();
				newUserNotify.From = (welcomeUserProfile == null ? "System Administrator" : welcomeUserProfile.FullName);
				newUserNotify.FromUserProfileId = welcomeUserProfile.UserProfileId;
				newUserNotify.UserProfileId = registeredNotify.UserProfileId;
				newUserNotify.To = registeredNotify.FullName;
				newUserNotify.Importance = "Normal";
				newUserNotify.Subject = "New User Registered on " + controller.CurrentStoreFront.Name + " - " + newProfile.FullName + " <" + newProfile.Email + ">";
				newUserNotify.UrlHost = request.Url.Host;
				if (!request.Url.IsDefaultPort)
				{
					newUserNotify.UrlHost += ":" + request.Url.Port;
				}

				newUserNotify.BaseUrl = notificationBaseUrl;
				newUserNotify.Message = messageBody;
				db.Notifications.Add(newUserNotify);
				db.SaveChanges();
			}
		}
	}
}