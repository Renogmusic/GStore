using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Data;
using System.Web.Routing;

namespace GStore.Models.Extensions
{
	public static class EventLogExtensions
	{
		public static void LogSystemEvent(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, string source, SystemEventLevel level, string message, Controllers.BaseClass.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext();

			SystemEvent newEvent = null;
			try
			{
				newEvent = newctx.SystemEvents.Create();
			}
			catch (Exception exLog)
			{
				string logMessage = "Error logging system event. Unable to save to database."
					+ " \nUrl: " + httpContext.Request.RawUrl
					+ " \nSource: " + source
					+ " \nSystemEventLevel: " + level.ToString()
					+ " Message: " + message;

				throw new ApplicationException(logMessage, exLog);
			}

			newEvent.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false), controller);

			newEvent.Level = (int)level;
			newEvent.LevelText = level.ToString();
			newctx.SystemEvents.Add(newEvent);
			newctx.SaveChanges();

		}

		public static void LogSecurityEvent(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string source, SecurityEventLevel level, bool success, bool anonymous, string userName, UserProfile profile, string message, Controllers.BaseClass.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext(userName);

			SecurityEvent newEvent = newctx.SecurityEvents.Create();

			newEvent.SetBasicFields(mvcHttpContext, routeData, source, message, anonymous, profile, controller);

			newEvent.Level = (int)level;
			newEvent.LevelText = level.ToString();
			newEvent.Success = success;
			newctx.SecurityEvents.Add(newEvent);

			newctx.SaveChanges();

		}

		public static void LogPageViewEvent(this IGstoreDb ctx, System.Web.Mvc.ResultExecutingContext context)
		{
			IGstoreDb newctx = ctx.NewContext();

			PageViewEvent newEvent = newctx.PageViewEvents.Create();

			Controllers.BaseClass.BaseController controller = null;
			if (context.Controller is Controllers.BaseClass.BaseController)
			{
				controller = context.Controller as Controllers.BaseClass.BaseController;
			}

			string source = context.RouteData.Values["controller"].ToString()
				+ " -> " + context.RouteData.Values["action"].ToString();

			string message = "PageView"
				+ " \n-Url: " + context.HttpContext.Request.RawUrl;

			newEvent.SetBasicFields(context.HttpContext, context.RouteData, source, message, !context.HttpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false), controller);
			newctx.PageViewEvents.Add(newEvent);

			newctx.SaveChanges();

		}

		public static void LogUserActionEvent(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, string source, string category, string action, string label, Controllers.BaseClass.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext();

			UserActionEvent newEvent = newctx.UserActionEvents.Create();

			string message = "User Action Event"
				+ " \n-Category: " + category
				+ " \n-Action: " + action
				+ " \n-Label: " + label;

			newEvent.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, newctx.GetCurrentUserProfile(false), controller);
			newEvent.Category = category;
			newEvent.Action = action;
			newEvent.Label = label;
			newctx.UserActionEvents.Add(newEvent);

			newctx.SaveChanges();

		}

		public static void LogFileNotFound(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext();

			FileNotFoundLog newLog = newctx.FileNotFoundLogs.Create();
			string message = "404 File Not Found: " + httpContext.Request.RawUrl;
			string source = "App";
			if (routeData != null)
			{
				source = routeData.Values["controller"].ToString()
				+ " -> " + routeData.Values["action"].ToString();
			}

			newLog.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false), controller);
			newctx.FileNotFoundLogs.Add(newLog);

			newctx.SaveChanges();
		}


		public static void LogBadRequest(this IGstoreDb ctx, HttpContextBase httpContext, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			IGstoreDb newctx = ctx.NewContext();

			BadRequest badRequest = newctx.BadRequests.Create();

			string message = "400 error. Bad Request: " + httpContext.Request.RawUrl;
			string source = "App";
			if (routeData != null)
			{
				source = routeData.Values["controller"].ToString()
				+ " -> " + routeData.Values["action"].ToString();
			}

			badRequest.SetBasicFields(httpContext, routeData, source, message, !httpContext.User.Identity.IsAuthenticated, ctx.GetCurrentUserProfile(false), controller);
			newctx.BadRequests.Add(badRequest);

			newctx.SaveChanges();
		}

		public static void LogSecurityEvent_LoginSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, Controllers.BaseClass.BaseController controller)
		{
			string message = "Login success for " + profile.UserName
				+ " \n\n-Email: " + profile.Email
				+ " \n-Name: " + profile.FullName
				+ " \n-UserId: " + profile.UserId
				+ " \n-UserProfileId: " + profile.UserProfileId;

			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Login Success", SecurityEventLevel.LoginSuccess, true, false, profile.UserName, profile, message, controller);
		}

		public static void LogSecurityEvent_LoginFailed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string loginAttempted, string passwordAttempted, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_LoginLockedOut(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string loginAttempted, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_LoginNeedsVerification(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string login, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_NewRegister(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_EmailConfirmed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_EmailConfirmFailedUserNotFound(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string userNameAttempted, string codeAttempted, Controllers.BaseClass.BaseController controller)
		{
			string message = "Email Confirm Failed. Unknown user: " + userNameAttempted + " Code: " + codeAttempted;
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Email Confirmed", SecurityEventLevel.EmailConfirmFailedUnknownUser, false, true, userNameAttempted, null, message, controller);
		}

		public static void LogSecurityEvent_EmailConfirmFailed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string userId, string codeAttempted, IEnumerable<string> resultErrors, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_VerificationCodeSent(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string returnUrl, string provider, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_ForgotPasswordSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_ForgotPasswordEmailNotFound(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, Controllers.BaseClass.BaseController controller)
		{
			string message = "Forgot Password Failed. No user with Email: " + email;
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Forgot Password Failed Unknown User", SecurityEventLevel.ForgotPasswordFailedUnknownUser, false, true, email, null, message, controller);
		}

		public static void LogSecurityEvent_PasswordResetSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_PasswordResetFailedUnknownUser(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, Controllers.BaseClass.BaseController controller)
		{
			string message = "Password reset invalid. No user with Email: " + email;
			ctx.LogSecurityEvent(mvcHttpContext, routeData, "Password Reset Failed Unknown User", SecurityEventLevel.PasswordResetFailedUnknownUser, false, true, email, null, message, controller);
		}

		public static void LogSecurityEvent_PasswordResetFailed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string email, IEnumerable<string> resultErrors, UserProfile profile, Controllers.BaseClass.BaseController controller)
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


		public static void LogSecurityEvent_PhoneConfirmed(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string phoneNumber, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_VerificationCodeSuccess(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string code, string provider, string returnUrl, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_VerificationCodeFailedLockedOut(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string code, string provider, string returnUrl, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

		public static void LogSecurityEvent_VerificationCodeFailedInvalidCode(this IGstoreDb ctx, HttpContextBase mvcHttpContext, RouteData routeData, string code, string provider, string returnUrl, UserProfile profile, Controllers.BaseClass.BaseController controller)
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

	}
}