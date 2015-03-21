using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreData
{
	public static class EventLogExtensions
	{
		#region Defined Security Events

		public const string GStoreFolder_BadRequests = "~/App_Data/EventLog/BadRequests";
		public const string GStoreFolder_FileNotFoundLogs = "~/App_Data/EventLog/FileNotFoundLogs";
		public const string GStoreFolder_LogExceptions = "~/App_Data/EventLog/LogExceptions";
		public const string GStoreFolder_PageViewEvents = "~/App_Data/EventLog/PageViewEvents";
		public const string GStoreFolder_SecurityEvents = "~/App_Data/EventLog/SecurityEvents";
		public const string GStoreFolder_SystemEvents = "~/App_Data/EventLog/SystemEvents";
		public const string GStoreFolder_UserActionEvents = "~/App_Data/EventLog/UserActionEvents";
		public const string GStoreFolder_EmailSent = "~/App_Data/EventLog/EmailSent";
		public const string GStoreFolder_SmsSent = "~/App_Data/EventLog/SmsSent";

		/// <summary>
		/// Returns one of the GStoreFolder constant values for a virtual path based on a string name of the constant
		/// </summary>
		/// <param name="constantName"></param>
		/// <returns></returns>
		public static string GStoreLogConstantNameToPath(string constantName)
		{
			if (string.IsNullOrEmpty(constantName))
			{
				throw new ArgumentNullException("constantName");
			}

			switch (constantName.ToLower())
			{
				case "gstorefolder_badrequests":
					return GStoreFolder_BadRequests;
				case "gstorefolder_filenotfoundlogs":
					return GStoreFolder_FileNotFoundLogs;
				case "gstorefolder_logexceptions":
					return GStoreFolder_LogExceptions;
				case "gstorefolder_pageviewevents":
					return GStoreFolder_PageViewEvents;
				case "gstorefolder_securityevents":
					return GStoreFolder_SecurityEvents;
				case "gstorefolder_systemevents":
					return GStoreFolder_SystemEvents;
				case "gstorefolder_useractionevents":
					return GStoreFolder_UserActionEvents;
				case "gstorefolder_emailsent":
					return GStoreFolder_EmailSent;
				case "gstorefolder_smssent":
					return GStoreFolder_SmsSent;
				default:
					throw new ApplicationException("Folder constant not found: " + constantName);
			}
		}

		public static void LogSecurityEvent_LoginSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Login success for " + profile.UserName
				+ " \n\n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Success", SecurityEventLevel.LoginSuccess, true, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_LogOff(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "LogOff for " + mvcHttpContext.User.Identity.Name
				+ " \n\n-Email: " + mvcHttpContext.User.Identity.Name
				+ " \n-Name: " + mvcHttpContext.User.Identity.Name
				+ " \n-UserId: " + mvcHttpContext.User.Identity.Name
				+ " \n-UserProfileId: " + mvcHttpContext.User.Identity.Name;

			if (profile != null)
			{
				message = "LogOff for " + profile.UserName
					+ " \n\n-Email: " + profile.Email
					+ " \n-Name: " + profile.FullName
					+ " \n-UserId: " + profile.UserId
					+ " \n-UserProfileId: " + profile.UserProfileId;
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "LogOff", SecurityEventLevel.LogOff, true, false, mvcHttpContext.User.Identity.Name, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginFailed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string loginAttempted, string passwordAttempted, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Login failed for logon: " + loginAttempted
				+ " \n\n-" + (profile == null ? "Unknown user" : "Existing user")
				+ " \n-Password attempted: " + passwordAttempted;

			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId
				+ " \n-LastLogonDateTimeUtc: " + profile.LastLogonDateTimeUtc
				+ " \n-Failed Attempts: " + profile.AspNetIdentityUser().AccessFailedCount
				+ " \n-Locked Out: " + profile.AspNetIdentityUser().LockoutEndDateUtc.HasValue.ToString();
				if (profile.AspNetIdentityUser().LockoutEndDateUtc.HasValue)
				{
					message += " \n-Locked Out Until: " + profile.AspNetIdentityUser().LockoutEndDateUtc;
				}
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Failed", SecurityEventLevel.LoginFailure, false, true, loginAttempted, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginFailedNoStoreFront(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Post Login check failed, No Store Front Found. Logon: " + profile.UserName
				+ " \n\n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Failed No StoreFront", SecurityEventLevel.LoginFailureNoStoreFront, false, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginFailedStoreFrontInactive(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string storeFrontName, int storeFrontId, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Post Login check failed, Store Front '" + storeFrontName + "' [" + storeFrontId + "] is Inactive. Logon: " + profile.UserName
				+ " \n\n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Failed StoreFrontInactive", SecurityEventLevel.LoginFailureStoreFrontInactive, false, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginFailedNoStoreFrontConfig(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, int storeFrontId, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Post Login check failed, No Configuration was found for Store Front Id [" + storeFrontId + "]. Logon: " + profile.UserName
				+ " \n\n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Failed StoreFrontConfigInactive", SecurityEventLevel.LoginFailureNoStoreFrontConfig, false, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginFailedStoreFrontConfigInactive(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string storeFrontName, int storeFrontId, string configurationName, int configurationId, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Post Login check failed, Configuration '" + configurationName + "' [" + configurationId + "] is Inactive for Store Front '" + storeFrontName + "' [" + storeFrontId + "]. Logon: " + profile.UserName
				+ " \n\n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Failed StoreFrontConfigInactive", SecurityEventLevel.LoginFailureStoreFrontConfigInactive, false, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginLockedOut(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string loginAttempted, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Login attempted for locked out account: " + loginAttempted;

			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId
				+ " \n-LastLogonDateTimeUtc: " + profile.LastLogonDateTimeUtc
				+ " \n-Failed Attempts: " + profile.AspNetIdentityUser().AccessFailedCount
				+ " \n-Locked Out: " + profile.AspNetIdentityUser().LockoutEndDateUtc.HasValue.ToString();
				if (profile.AspNetIdentityUser().LockoutEndDateUtc.HasValue)
				{
					message += " \n-Locked Out Until: " + profile.AspNetIdentityUser().LockoutEndDateUtc;
				}
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Locked Out", SecurityEventLevel.Lockout, false, false, loginAttempted, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginNeedsVerification(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string login, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Login needs email or phone verification for account: " + login;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId
				+ " \n-LastLogonDateTimeUtc: " + profile.LastLogonDateTimeUtc
				+ " \n-Failed Attempts: " + profile.AspNetIdentityUser().AccessFailedCount
				+ " \n-Locked Out: " + profile.AspNetIdentityUser().LockoutEndDateUtc.HasValue.ToString();
				if (profile.AspNetIdentityUser().LockoutEndDateUtc.HasValue)
				{
					message += " \n-Locked Out Until: " + profile.AspNetIdentityUser().LockoutEndDateUtc;
				}
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Needs Verification", SecurityEventLevel.LoginNeedsVerification, true, false, login, profile, message, controller);
		}

		public static void LogSecurityEvent_NewRegister(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "New user signup: " + profile.UserName;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "New User Registration", SecurityEventLevel.NewRegister, true, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_EmailConfirmed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Email Confirmed for user: " + profile.UserName;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Email Confirmed", SecurityEventLevel.EmailConfirmed, true, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_EmailConfirmFailedUserNotFound(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string userNameAttempted, string codeAttempted, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Email Confirm Failed. Unknown user: " + userNameAttempted + " Code: " + codeAttempted;
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Email Confirmed", SecurityEventLevel.EmailConfirmFailedUnknownUser, false, true, userNameAttempted, null, message, controller);
		}

		public static void LogSecurityEvent_EmailConfirmFailed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string userId, string codeAttempted, IEnumerable<string> resultErrors, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Email Confirm Failed for User: " + userId + " Code : " + codeAttempted;
			foreach (string error in resultErrors)
			{
				message += " \n-Error: " + error;
			}

			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Email Confirmed", SecurityEventLevel.EmailConfirmFailed, false, false, userId, profile, message, controller);
		}

		public static void LogSecurityEvent_VerificationCodeSent(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string returnUrl, string provider, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Verification code sent: " + provider + " Url: " + returnUrl;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Verification Code Sent", SecurityEventLevel.VerificationCodeSent, true, false, "(unknown)", profile, message, controller);
		}

		public static void LogSecurityEvent_ForgotPasswordSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Forgot Password Success for User Email: " + email;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Forgot Password Success", SecurityEventLevel.ForgotPasswordSuccess, true, false, email, profile, message, controller);
		}

		public static void LogSecurityEvent_ForgotPasswordEmailNotFound(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Forgot Password Failed. No user with Email: " + email;
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Forgot Password Failed Unknown User", SecurityEventLevel.ForgotPasswordFailedUnknownUser, false, true, email, null, message, controller);
		}

		public static void LogSecurityEvent_ForgotPasswordProfileNotFound(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Forgot Password Failed. User Profile not found for Email: " + email;
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Forgot Password Failed Profile Not Found", SecurityEventLevel.ForgotPasswordFailedProfileNotFound, false, true, email, null, message, controller);
		}

		public static void LogSecurityEvent_PasswordResetSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Password reset successfully for User Email: " + email;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Password Reset Success", SecurityEventLevel.PasswordResetSuccess, true, false, email, profile, message, controller);
		}

		public static void LogSecurityEvent_PasswordResetFailedUnknownUser(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Password reset invalid. No user with Email: " + email;
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Password Reset Failed Unknown User", SecurityEventLevel.PasswordResetFailedUnknownUser, false, true, email, null, message, controller);
		}

		public static void LogSecurityEvent_PasswordResetFailed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, IEnumerable<string> resultErrors, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Password reset failed for User Email: " + email;
			foreach (string error in resultErrors)
			{
				message += " \n-Error: " + error;

			}
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Password Reset Failed", SecurityEventLevel.PasswordResetFailed, false, true, email, profile, message, controller);
		}


		public static void LogSecurityEvent_PhoneConfirmed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string phoneNumber, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Phone Confirmed for user: " + profile.UserName + " Phone: " + phoneNumber;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Phone Confirmed", SecurityEventLevel.PhoneConfirmed, true, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_VerificationCodeSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string code, string provider, string returnUrl, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Verification code confirmed: " + code + " Provider: " + provider + " ReturnUrl: " + returnUrl;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Verification Code Success", SecurityEventLevel.VerificationCodeSuccess, true, true, "(unknown)", profile, message, controller);
		}

		public static void LogSecurityEvent_VerificationCodeFailedLockedOut(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string code, string provider, string returnUrl, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Verification code failed. User is locked out. code: " + code + " Provider: " + provider + " ReturnUrl: " + returnUrl;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Verification Code Failed With Lockout", SecurityEventLevel.VerificationCodeFailedLockedOut, false, true, "(unknown)", profile, message, controller);
		}

		public static void LogSecurityEvent_VerificationCodeFailedInvalidCode(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string code, string provider, string returnUrl, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string message = "Verification code failed. Invalid code: " + code + " Provider: " + provider + " ReturnUrl: " + returnUrl;
			if (profile != null)
			{
				message += " \n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;
			}

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Verification Code Bad Code", SecurityEventLevel.VerificationCodeFailedBadCode, false, true, "(unknown)", profile, message, controller);
		}

		#endregion

		#region Main Logging methods

		public static SystemEvent LogSystemEvent(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, string source, SystemEventLevel level, string message, string exceptionMessage, string baseExceptionMessage, string baseExceptionToString, GStoreData.ControllerBase.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext();

			SystemEvent newEvent = newEvent = newctx.SystemEvents.Create();
			newEvent.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false, false), controller);

			newEvent.Level = (int)level;
			newEvent.LevelText = level.ToString();
			newEvent.ExceptionMessage = exceptionMessage;
			newEvent.BaseExceptionMessage = baseExceptionMessage;
			newEvent.BaseExceptionToString = baseExceptionToString;

			string simpleInfo = newEvent.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--System Event: " + newEvent.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			if (Settings.AppLogSystemEventsToDb)
			{
				try
				{
					newctx.SystemEvents.Add(newEvent);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(httpContext, routeData);
					newEvent.LogToFile(httpContext);
				}
			}
			if (Settings.AppLogSystemEventsToFile)
			{
				newEvent.LogToFile(httpContext);
			}

			return newEvent;
		}

		public static SecurityEvent LogSecurityEvent(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string source, SecurityEventLevel level, bool success, bool anonymous, string userName, UserProfile profile, string message, GStoreData.ControllerBase.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext(userName);

			SecurityEvent newEvent = newctx.SecurityEvents.Create();

			newEvent.SetBasicFields(mvcHttpContext, routeData, source, message, anonymous, profile, controller);

			newEvent.Level = (int)level;
			newEvent.LevelText = level.ToString();
			newEvent.Success = success;

			string simpleInfo = newEvent.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--Security Event: " + newEvent.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			if (Settings.AppLogSecurityEventsToDb)
			{
				try
				{
					newctx.SecurityEvents.Add(newEvent);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(mvcHttpContext, routeData);
					newEvent.LogToFile(mvcHttpContext);
				}
			}
			if (Settings.AppLogSecurityEventsToFile)
			{
				newEvent.LogToFile(mvcHttpContext);
			}

			return newEvent;

		}

		public static PageViewEvent LogPageViewEvent(this IGstoreDb ctx, System.Web.Mvc.ResultExecutingContext context)
		{
			IGstoreDb newctx = ctx.NewContext();

			PageViewEvent newEvent = newctx.PageViewEvents.Create();

			GStoreData.ControllerBase.BaseController controller = null;
			if (context.Controller is GStoreData.ControllerBase.BaseController)
			{
				controller = context.Controller as GStoreData.ControllerBase.BaseController;
			}

			string source = context.RouteData.ToSourceString();
			string message = "PageView"
				+ " \n-Url: " + context.HttpContext.Request.RawUrl;

			newEvent.SetBasicFields(context.HttpContext, context.RouteData, source, message, !context.HttpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false, false), controller);

			string simpleInfo = newEvent.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--Page View Event: " + newEvent.SimpleInfo());
			System.Diagnostics.Trace.Unindent();


			if (Settings.AppLogPageViewEventsToDb)
			{
				try
				{
					newctx.PageViewEvents.Add(newEvent);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(context.HttpContext, context.RouteData);
					newEvent.LogToFile(context.HttpContext);
				}
			}
			if (Settings.AppLogPageViewEventsToFile)
			{
				newEvent.LogToFile(context.HttpContext);
			}

			return newEvent;
		}

		public static UserActionEvent LogUserActionEvent(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, GStoreData.ControllerBase.BaseController controller, UserActionCategoryEnum category, UserActionActionEnum action, string label, bool success, int? cartId = null, string categoryUrlName = null, string discountCode = null, string emailAddress = null, int? notificationId = null, string orderNumber = null, int? orderItemId = null, int? pageId = null, string productUrlName = null, string productBundleUrlName = null, string smsPhone = null, string uploadFileName = null)
		{
			if (!Settings.AppEnableUserActionLog)
			{
				return null;
			}

			IGstoreDb newctx = ctx.NewContext();

			UserActionEvent newEvent = newctx.UserActionEvents.Create();

			string source = routeData.ToSourceString();

			string message = "User Action Event"
				+ " \n-Category: " + category.ToString()
				+ " \n-Action: " + action.ToString()
				+ " \n-Label: " + label.ToString()
				+ " \n-Success: " + success.ToString();

			newEvent.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, newctx.GetCurrentUserProfile(false, false), controller);
			newEvent.CartId = cartId;
			newEvent.Category = category;
			newEvent.CategoryUrlName = categoryUrlName;
			newEvent.DiscountCode = discountCode;
			newEvent.EmailAddress = emailAddress;
			newEvent.Label = label;
			newEvent.NotificationId = notificationId;
			newEvent.OrderNumber = orderNumber;
			newEvent.OrderItemId = orderItemId;
			newEvent.PageId = pageId;
			newEvent.ProductUrlName = productUrlName;
			newEvent.ProductBundleUrlName = productBundleUrlName;
			newEvent.SmsPhone = smsPhone;
			newEvent.Success = success;
			newEvent.UploadFileName = uploadFileName;
			newEvent.Action = action;
			newEvent.Label = label;

			string simpleInfo = newEvent.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--User Action Event: " + newEvent.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			if (Settings.AppLogUserActionEventsToDb)
			{
				try
				{
					newctx.UserActionEvents.Add(newEvent);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(httpContext, routeData);
					newEvent.LogToFile(httpContext);
				}
			}
			if (Settings.AppLogUserActionEventsToFile)
			{
				newEvent.LogToFile(httpContext);
			}

			return newEvent;
		}

		public static EmailSent LogEmailSent(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, GStoreData.ControllerBase.BaseController controller, string toName, string toAddress, string fromName, string fromAddress, string subject, string htmlBody, string htmlSignature, string textBody, string textSignature, bool success, string exceptionString)
		{
			IGstoreDb newctx = ctx.NewContext();

			EmailSent newLog = newctx.EmailsSent.Create();
			string message = "Email sent to '" + toName + "' <" + toAddress + "> from '" + fromName + "' <" + fromAddress + ">";
			string source = "App";
			if (routeData != null)
			{
				source = routeData.ToSourceString();
			}

			newLog.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false, false), controller);

			newLog.ToName = toName.OrDefault("(blank)");
			newLog.ToAddress = toAddress.OrDefault("(blank)");
			newLog.FromName = fromName.OrDefault("(blank)");
			newLog.FromAddress = fromAddress.OrDefault("(blank)");
			newLog.Subject = subject.OrDefault("(blank)");
			newLog.HtmlBody = htmlBody.OrDefault("(blank)");
			newLog.HtmlSignature = htmlSignature.OrDefault("(blank)");
			newLog.TextBody = textBody.OrDefault("(blank)");
			newLog.TextSignature = textSignature.OrDefault("(blank)");
			newLog.Success = success;
			newLog.ExceptionString = exceptionString;
			
			string simpleInfo = newLog.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--Email Sent Event: " + newLog.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			if (Settings.AppLogEmailSentToDb)
			{
				try
				{
					newctx.EmailsSent.Add(newLog);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(httpContext, routeData);
					newLog.LogToFile(httpContext);
				}
			}
			if (Settings.AppLogEmailSentToFile)
			{
				newLog.LogToFile(httpContext);
			}

			return newLog;

		}

		public static SmsSent LogSmsSent(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, GStoreData.ControllerBase.BaseController controller, string toPhone, string fromPhone, string textBody, string textSignature, bool success, string exceptionString)
		{
			IGstoreDb newctx = ctx.NewContext();

			SmsSent newLog = newctx.SmssSent.Create();
			string message = "Sms sent to '" + toPhone + "' from '" + fromPhone +"'";
			string source = "App";
			if (routeData != null)
			{
				source = routeData.ToSourceString();
			}

			newLog.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false, false), controller);

			newLog.ToPhone = toPhone.OrDefault("(blank)");
			newLog.FromPhone = fromPhone.OrDefault("(blank)");
			newLog.TextBody = textBody.OrDefault("(blank)");
			newLog.TextSignature = textSignature;
			newLog.Success = success;
			newLog.ExceptionString = exceptionString;

			string simpleInfo = newLog.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--Sms Sent Event: " + newLog.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			if (Settings.AppLogSmsSentToDb)
			{
				try
				{
					newctx.SmssSent.Add(newLog);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(httpContext, routeData);
					newLog.LogToFile(httpContext);
				}
			}
			if (Settings.AppLogSmsSentToFile)
			{
				newLog.LogToFile(httpContext);
			}

			return newLog;

		}

		public static FileNotFoundLog LogFileNotFound(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, GStoreData.ControllerBase.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext();

			FileNotFoundLog newLog = newctx.FileNotFoundLogs.Create();
			string message = "404 File Not Found: " + httpContext.Request.RawUrl;
			string source = "App";
			if (routeData != null)
			{
				source = routeData.ToSourceString();
			}

			newLog.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false, false), controller);

			string simpleInfo = newLog.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--File Not Found Event: " + newLog.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			if (Settings.AppLogFileNotFoundEventsToDb)
			{
				try
				{
					newctx.FileNotFoundLogs.Add(newLog);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(httpContext, routeData);
					newLog.LogToFile(httpContext);
				}
			}
			if (Settings.AppLogFileNotFoundEventsToFile)
			{
				newLog.LogToFile(httpContext);
			}

			return newLog;

		}


		public static BadRequest LogBadRequest(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, GStoreData.ControllerBase.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext();

			BadRequest badRequest = newctx.BadRequests.Create();

			string message = "400 error. Bad Request: " + httpContext.Request.RawUrl;
			string source = "App";
			if (routeData != null)
			{
				source = routeData.ToSourceString();
			}

			badRequest.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false, false), controller);

			string simpleInfo = badRequest.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--Bad Request Event: " + badRequest.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			if (Settings.AppLogBadRequestEventsToDb)
			{
				try
				{
					newctx.BadRequests.Add(badRequest);
					newctx.SaveChanges();
				}
				catch (Exception ex)
				{
					//can't save to database, attempt save to file
					ex.LogToFile(httpContext, routeData);
					badRequest.LogToFile(httpContext);
				}
			}
			if (Settings.AppLogBadRequestEventsToFile)
			{
				badRequest.LogToFile(httpContext);
			}

			return badRequest;
		}
		#endregion

		#region File logging methods (if db logging fails)

		public static void LogToFile(this Exception ex, HttpContextBase httpContext, RouteData routeData)
		{
			string message = "Exception Log to DB Failed."
				+ "  \n-Message: " + ex.Message
				+ "  \n-Source: " + ex.Source
				+ "  \n-TargetSiteName: " + ex.TargetSite.Name
				+ "  \n-StackTrace: " + ex.StackTrace;

			//create POCO version of a system event
			SystemEvent newEvent = new SystemEvent();
			newEvent.SetDefaults(null);
			newEvent.SetBasicFields(httpContext, null, routeData.ToSourceString(), message, true, null, null);
			newEvent.Level = (int)SystemEventLevel.CriticalError;
			newEvent.LevelText = SystemEventLevel.CriticalError.ToString();
			newEvent.ExceptionMessage = ex.Message;
			newEvent.BaseExceptionMessage = ex.GetBaseException().Message;
			newEvent.BaseExceptionToString = ex.GetBaseException().Message.ToString();

			string simpleInfo = newEvent.SimpleInfo();
			System.Diagnostics.Trace.Indent();
			System.Diagnostics.Trace.WriteLine("--Logging Exception: System Event: " + newEvent.SimpleInfo());
			System.Diagnostics.Trace.Unindent();

			string folder = httpContext.Server.MapPath(GStoreFolder_LogExceptions);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			newEvent.SaveEntityToFile(folder, fileName);
		}

		public static void LogToFile(this SystemEvent newEvent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_SystemEvents);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			newEvent.SaveEntityToFile(folder, fileName);
		}

		public static void LogToFile(this SecurityEvent newEvent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_SecurityEvents);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			newEvent.SaveEntityToFile(folder, fileName);
		}

		public static void LogToFile(this BadRequest newEvent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_BadRequests);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			newEvent.SaveEntityToFile(folder, fileName);
		}

		public static void LogToFile(this FileNotFoundLog newEvent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_FileNotFoundLogs);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			newEvent.SaveEntityToFile(folder, fileName);
		}

		public static void LogToFile(this PageViewEvent newEvent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_PageViewEvents);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			newEvent.SaveEntityToFile(folder, fileName);
		}

		public static void LogToFile(this UserActionEvent newEvent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_UserActionEvents);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			newEvent.SaveEntityToFile(folder, fileName);
		}

		public static void LogToFile(this EmailSent emailSent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_EmailSent);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			emailSent.SaveEntityToFile(folder, fileName);
			string header = "From: '" + emailSent.FromName + "' <" + emailSent.FromAddress + ">"
				+ "\r\nTo: '" + emailSent.ToName + "' <" + emailSent.ToAddress + ">"
				+ "\r\nSent: " + emailSent.CreateDateTimeUtc.ToGStoreSystemDefaultDateTimeString()
				+ "\r\nSubject: " + emailSent.Subject
				+ "\r\n\r\n";
			
			System.IO.File.WriteAllText(folder + "\\" + fileName + "_Text.txt", header + emailSent.TextBody);
			System.IO.File.WriteAllText(folder + "\\" + fileName + "_Html.html", header.ToHtmlLines() + emailSent.HtmlBody);
		}

		public static void LogToFile(this SmsSent smsSent, HttpContextBase httpContext)
		{
			string folder = httpContext.Server.MapPath(GStoreFolder_SmsSent);
			string fileName = DateTime.UtcNow.ToString("yyyy-MM-dd_hh_mm_ss_" + Guid.NewGuid().ToString()) + ".xml";
			smsSent.SaveEntityToFile(folder, fileName);
		}

		#endregion

		public static string ToSourceString(this RouteData routeData)
		{
			if (routeData == null)
			{
				return "(null)";
			}

			string source = routeData.Controller() + " -> " + routeData.Action();
			if (!string.IsNullOrEmpty(routeData.Area()))
			{
				source = routeData.Area() + " -> " + source;
			}
			return source;
		}

		/// <summary>
		/// Returns a string of common information for an event
		/// </summary>
		/// <returns></returns>
		public static string SimpleInfo(this Models.BaseClasses.EventLogBase record, int maxMessageLength = 80)
		{
			return "-Source: " + record.Source
				+ "\n -Message: " + (record.Message.Length > maxMessageLength ? record.Message.Substring(0, maxMessageLength) + "...more" : record.Message)
				+ "\n -RawUrl: " + record.RawUrl
				+ "\n -Area: " + record.Area
				+ "\n -Controller: " + record.Controller
				+ "\n -ActionName: " + record.ActionName
				+ "\n -ActionParameters: " + record.ActionParameters
				+ "\n -ClientId: " + (record.ClientId.HasValue ? record.ClientId.ToString() : "(null)")
				+ "\n -StoreFrontId: " + (record.StoreFrontId.HasValue ? record.StoreFrontId.ToString() : "(null)")
				+ "\n -UserName: " + record.UserName
				+ "\n -Method: " + record.HttpMethod
				+ "\n -UrlReferrer: " + record.UrlReferrer
				+ "\n -UserAgent: " + record.UserAgent;
		}

		public static void SetBasicFields(this Models.BaseClasses.EventLogBase record, HttpContextBase httpContext, RouteData routeData, string source, string message, bool anonymous, UserProfile profile, GStoreData.ControllerBase.BaseController controller)
		{
			string siteId = httpContext.ApplicationInstance.Server.MachineName
				+ ":" + System.Web.Hosting.HostingEnvironment.SiteName
				+ httpContext.Request.ApplicationPath;

			record.StartDateTimeUtc = DateTime.UtcNow;
			record.EndDateTimeUtc = DateTime.UtcNow;


			if (controller != null)
			{
				try
				{
					record.StoreFrontId = controller.CurrentStoreFrontIdOrNull;
				}
				catch (Exception)
				{
					record.StoreFrontId = null;
				}
				try
				{
					record.ClientId = controller.CurrentClientIdOrNull;
				}
				catch (Exception)
				{
					record.ClientId = null;
				}
			}

			if (routeData != null)
			{
				if (routeData.DataTokens.ContainsKey("area"))
				{
					record.Area = routeData.DataTokens["area"].ToString();
				}
				record.Controller = routeData.Values["controller"].ToString();
				record.ActionName = routeData.Values["action"].ToString();
				record.ActionParameters = string.Empty;

				bool isFirst = true;
				foreach (var item in routeData.Values)
				{
					if (!isFirst)
					{
						record.ActionParameters += ", ";
					}
					record.ActionParameters += item.Key + " = " + item.Value;
					isFirst = false;
				}
			}
			else
			{
				record.Controller = string.Empty;
				record.ActionName = string.Empty;
				record.ActionParameters = string.Empty;
			}

			record.ServerName = httpContext.Server.MachineName;
			record.ApplicationPath = httpContext.Request.ApplicationPath;
			record.HostName = httpContext.Request.Url.Host;
			record.HttpMethod = httpContext.Request.HttpMethod;
			record.IsSecureConnection = httpContext.Request.IsSecureConnection;
			record.UserHostAddress = httpContext.Request.UserHostAddress;
			record.UrlReferrer = (httpContext.Request.UrlReferrer == null ? "" : httpContext.Request.UrlReferrer.ToString());
			record.UserAgent = httpContext.Request.UserAgent;
			record.RawUrl = httpContext.Request.RawUrl;
			record.Url = httpContext.Request.Url.ToString();
			record.Querystring = httpContext.Request.QueryString.ToString();
			record.Source = source;
			record.Message = message;
			record.Anonymous = anonymous;
			record.SessionId = httpContext.Session.SessionID;

			if (profile == null)
			{
				record.UserId = null;
				record.UserName = null;
				record.UserProfileId = null;
				record.FullName = null;
			}
			else
			{
				record.UserId = profile.UserId;
				record.UserName = profile.UserName;
				record.UserProfileId = profile.UserProfileId;
				record.FullName = profile.FullName;
			}
		}

		public static void CreateEventLogFolders(HttpContext context)
		{
			if (Settings.AppLogBadRequestEventsToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_BadRequests)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_BadRequests));
				}
			}

			if (Settings.AppLogFileNotFoundEventsToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_FileNotFoundLogs)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_FileNotFoundLogs));
				}
			}

			if (Settings.AppLogLogExceptionsToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_LogExceptions)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_LogExceptions));
				}
			}

			if (Settings.AppLogPageViewEventsToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_PageViewEvents)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_PageViewEvents));
				}
			}

			if (Settings.AppLogSecurityEventsToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_SecurityEvents)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_SecurityEvents));
				}
			}

			if (Settings.AppLogSystemEventsToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_SystemEvents)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_SystemEvents));
				}
			}

			if (Settings.AppLogUserActionEventsToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_UserActionEvents)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_UserActionEvents));
				}
			}

			if (Settings.AppLogEmailSentToFile)
			{
				if (!System.IO.Directory.Exists(context.Server.MapPath(GStoreFolder_EmailSent)))
				{
					System.IO.Directory.CreateDirectory(context.Server.MapPath(GStoreFolder_EmailSent));
				}
			}


		}

	}
}