using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GStore.Models;
using GStore.Data;
using GStore.Models.ViewModels;
using GStore.AppHtmlHelpers;

namespace GStore.Controllers
{
	[Authorize]
	public class ProfileController : BaseClass.BaseController
	{
		public ProfileController()
		{
		}

		public ProfileController(AspNetIdentityUserManager userManager)
		{
			UserManager = userManager;
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.ProfileTheme.FolderName;
			}
		}

		private AspNetIdentityUserManager _userManager;
		public AspNetIdentityUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AspNetIdentityUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//
		// GET: /Profile/Index
		public async Task<ActionResult> Index(ProfileMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ProfileMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: message == ProfileMessageId.SetPasswordSuccess ? "Your password has been set."
				: message == ProfileMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
				: message == ProfileMessageId.Error ? "An error has occurred."
				: message == ProfileMessageId.AddPhoneSuccess ? "Your phone number was added."
				: message == ProfileMessageId.RemovePhoneSuccess ? "Your phone number was removed."
				: "";


			UserProfile profile = CurrentUserProfileOrThrow;
			Identity.AspNetIdentityUser aspUser = profile.AspNetIdentityUser();

			var model = new ProfileViewModel
			{
				HasPassword = HasPassword(),
				PhoneNumber = aspUser.PhoneNumber,
				TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
				Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()),
				BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId()),
				EmailConfirmed = aspUser.EmailConfirmed,
				PhoneNumberConfirmed = aspUser.PhoneNumberConfirmed,
				AspNetIdentityUser = aspUser,
				UserProfile = profile
			};
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult UpdateTimeZone(string timeZoneId)
		{
			UserProfile userProfile = CurrentUserProfileOrThrow;
			userProfile.TimeZoneId = timeZoneId;
			GStoreDb.UserProfiles.Update(userProfile);
			GStoreDb.SaveChanges();
			return RedirectToAction("Index");
		}

		public ActionResult SendTestEmail()
		{
			UserProfile profile = CurrentUserProfileOrThrow;
			if (!profile.AspNetIdentityUser().EmailConfirmed)
			{
				AddUserMessage("Test Email not sent", "You must confirm your email address before you can receive email.", AppHtmlHelpers.UserMessageType.Warning);
				return RedirectToAction("Index");
			}

			Client client = CurrentClientOrThrow;
			StoreFront storeFront = CurrentStoreFrontOrThrow;


			string subject = "Test Email from " + storeFront.CurrentConfig().Name + " - " + Request.BindingHostName();
			string textBody = "Test Email from " + storeFront.CurrentConfig().Name + " - " + Request.BindingHostName();
			string htmlBody = "Test Email from " + storeFront.CurrentConfig().Name + " - " + Request.BindingHostName();
 
			bool result = GStore.AppHtmlHelpers.AppHtmlHelper.SendEmail(client, profile.Email, profile.FullName, subject, textBody, htmlBody, Request.Url.Host);
			if (result)
			{
				AddUserMessage("Test Email Sent!", "Test Email was sent to '" + profile.Email.ToHtml() + "'.", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_SendTestEmail, profile.Email, true, emailAddress: profile.Email);
			}
			else
			{
				AddUserMessage("Test Email not sent", "Test Email was NOT sent. This store does not have Email send activated.", AppHtmlHelpers.UserMessageType.Warning);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_SendTestEmail, "Email Disabled", false, emailAddress: profile.Email);
			}

			return RedirectToAction("Index");
		}

		public ActionResult SendTestSms()
		{
			UserProfile profile = CurrentUserProfileOrThrow;

			string smsPhone = profile.AspNetIdentityUser().PhoneNumber;
			if (string.IsNullOrWhiteSpace(smsPhone))
			{
				AddUserMessage("Test Text Message not sent", "You must add a phone number before you can receive SMS text messages.", AppHtmlHelpers.UserMessageType.Warning);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_SendTestSms, "No Phone Number", false, smsPhone: smsPhone);
				return RedirectToAction("Index");
			}

			if (!profile.AspNetIdentityUser().PhoneNumberConfirmed)
			{
				AddUserMessage("Test Text Message not sent", "You must confirm your phone number before you can receive SMS text messages.", AppHtmlHelpers.UserMessageType.Warning);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_SendTestSms, "Phone Number Not Confirmed", false, smsPhone: smsPhone);
				return RedirectToAction("Index");
			}

			Client client = CurrentClientOrThrow;
			StoreFront storeFront = CurrentStoreFrontOrThrow;

			string textBody = "Test Text Message from " + storeFront.CurrentConfig().Name + " - " + Request.BindingHostName();

			bool result = GStore.AppHtmlHelpers.AppHtmlHelper.SendSms(client, smsPhone, textBody, Request.Url.Host);
			if (result)
			{
				AddUserMessage("Test Text Message Sent!", "Test SMS Text Message was sent to '" + profile.AspNetIdentityUser().PhoneNumber.ToHtml() + "'.", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_SendTestSms, smsPhone, true, smsPhone: smsPhone);
			}
			else
			{
				AddUserMessage("Test Text Message Not Sent!", "Test Text Message was NOT sent. This store does not have SMS Text send activated.", AppHtmlHelpers.UserMessageType.Warning);
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_SendTestSms, "Sms Disabled", false, smsPhone: smsPhone);
			}
			return RedirectToAction("Index");
		}


		//
		// GET: /Profile/RemoveLogin
		public ActionResult RemoveLogin()
		{
			var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
			ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
			return View(linkedAccounts);
		}

		//
		// POST: /Profile/RemoveLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
		{
			ProfileMessageId? message;
			var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}
				message = ProfileMessageId.RemoveLoginSuccess;
			}
			else
			{
				message = ProfileMessageId.Error;
			}
			return RedirectToAction("Logins", new { Message = message });
		}

		//
		// GET: /Profile/AddPhoneNumber
		public ActionResult AddPhoneNumber()
		{
			return View();
		}

		//
		// POST: /Profile/AddPhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			// Generate the token and send it
			var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
			if (UserManager.SmsService != null)
			{

				StoreFront storeFront = CurrentStoreFrontOrNull;
				string messageBody = Data.StoreFrontExtensions.AddPhoneNumberMessage(storeFront, code, Request.Url);

				var message = new IdentityMessage
				{
					Destination = model.Number,
					Body = messageBody
				};
				await UserManager.SmsService.SendAsync(message);
			}
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_AddPhoneNumber, model.Number, true, smsPhone: model.Number);

			return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
		}

		//
		// POST: /Profile/EnableTwoFactorAuthentication
		[HttpPost]
		public async Task<ActionResult> EnableTwoFactorAuthentication()
		{
			await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_EnableTwoFactorAuth, "", true);
			return RedirectToAction("Index", "Profile");
		}

		//
		// POST: /Profile/DisableTwoFactorAuthentication
		[HttpPost]
		public async Task<ActionResult> DisableTwoFactorAuthentication()
		{
			await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_DisableTwoFactorAuth, "", true);
			return RedirectToAction("Index", "Profile");
		}

		//
		// GET: /Profile/VerifyPhoneNumber
		public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
		{
			var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
			// Send an SMS through the SMS provider to verify the phone number
			if (phoneNumber == null)
			{
				return HttpBadRequest("VerifyPhoneNumber phoneNumber = null");
			}
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_VerifyPhoneNumber_GetCode, code, true);
			return View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
		}

		//
		// POST: /Profile/VerifyPhoneNumber
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_VerifyPhoneNumber_VerifyCode, model.Code, true);
				return RedirectToAction("Index", new { Message = ProfileMessageId.AddPhoneSuccess });
			}
			else
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_VerifyPhoneNumber_VerifyCode, model.Code, false);
			}
			// If we got this far, something failed, redisplay form
			ModelState.AddModelError("", "Failed to verify phone");
			return View(model);
		}

		//
		// GET: /Profile/RemovePhoneNumber
		public async Task<ActionResult> RemovePhoneNumber()
		{
			var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
			if (!result.Succeeded)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_RemovePhoneNumber, "", false);
				return RedirectToAction("Index", new { Message = ProfileMessageId.Error });
			}
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_RemovePhoneNumber, "", true);
			return RedirectToAction("Index", new { Message = ProfileMessageId.RemovePhoneSuccess });
		}

		//
		// GET: /Profile/ChangePassword
		public ActionResult ChangePassword()
		{
			return View();
		}

		//
		// POST: /Profile/ChangePassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}

				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_PasswordChanged, "", true);

				StoreFront storeFront = CurrentStoreFrontOrNull;
				if (storeFront == null)
				{
					AddUserMessage("Password updated.", "Your password has been changed successfully for all store fronts.", AppHtmlHelpers.UserMessageType.Info);
				}
				else
				{
					string baseUrl = Url.Action("Details", "Notifications", new { id = "" });
					GStoreDb.CreatePasswordChangedNotification(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, Request.Url, baseUrl);
				}
				return RedirectToAction("Index", new { Message = ProfileMessageId.ChangePasswordSuccess });
			}

			string error = null;
			try
			{
				error = string.Join(", ", result.Errors);
			}
			catch (Exception)
			{
				error = "Unknown Error";
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_PasswordChanged, error, false);
			AddErrors(result);
			return View(model);
		}

		//
		// GET: /Profile/SetPassword
		public ActionResult SetPassword()
		{
			return View();
		}

		//
		// POST: /Profile/SetPassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
				if (result.Succeeded)
				{
					var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
					if (user != null)
					{
						await SignInAsync(user, isPersistent: false);
					}
					return RedirectToAction("Index", new { Message = ProfileMessageId.SetPasswordSuccess });
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Profile/ProfileLogins
		public async Task<ActionResult> Logins(ProfileMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ProfileMessageId.RemoveLoginSuccess ? "The external login was removed."
				: message == ProfileMessageId.Error ? "An error has occurred."
				: "";
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user == null)
			{
				throw new ApplicationException("Cannot find user name: " + User.Identity.Name);
			}
			var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
			var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
			ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
			return View(new ProfileLoginsViewModel
			{
				CurrentLogins = userLogins,
				OtherLogins = otherLogins
			});
		}

		//
		// POST: /Profile/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Profile"), User.Identity.GetUserId());
		}

		//
		// GET: /Profile/LinkLoginCallback
		public async Task<ActionResult> LinkLoginCallback()
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
			if (loginInfo == null)
			{
				return RedirectToAction("Logins", new { Message = ProfileMessageId.Error });
			}
			var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
			return result.Succeeded ? RedirectToAction("Logins") : RedirectToAction("Logins", new { Message = ProfileMessageId.Error });
		}

		[Authorize]
		public async Task<ActionResult> ConfirmEmail()
		{
			UserProfile profile = CurrentUserProfileOrThrow;
			string userId = profile.UserId;
			await this.SendEmailConfirmationCode(userId);
			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_ConfirmEmailSent, profile.Email, true, emailAddress: profile.Email);

			return View(profile);
		}

		//
		// GET: /Profile/Settings
		public ActionResult Notifications()
		{
			UserProfile profile = CurrentUserProfileOrThrow;

			NotificationSettingsViewModel viewModel = new NotificationSettingsViewModel()
			{

				AllowUsersToSendSiteMessages = profile.AllowUsersToSendSiteMessages,
				Email = profile.Email,
				NotifyAllWhenLoggedOn = profile.NotifyAllWhenLoggedOn,
				NotifyOfSiteUpdatesToEmail = profile.NotifyOfSiteUpdatesToEmail,
				SendSiteMessagesToEmail = profile.SendSiteMessagesToEmail,
				SendSiteMessagesToSms = profile.SendSiteMessagesToSms,
				SubscribeToNewsletterEmail = profile.SubscribeToNewsletterEmail,
				UserProfile = profile
			};

			return View(viewModel);
		}

		//
		// POST: /Profile/Settings
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Notifications(NotificationSettingsViewModel model)
		{
			UserProfile profile = CurrentUserProfileOrThrow;

			profile.AllowUsersToSendSiteMessages = model.AllowUsersToSendSiteMessages;
			profile.NotifyAllWhenLoggedOn = model.NotifyAllWhenLoggedOn;
			profile.NotifyOfSiteUpdatesToEmail = model.NotifyOfSiteUpdatesToEmail;
			profile.SendSiteMessagesToEmail = model.SendSiteMessagesToEmail;
			profile.SendSiteMessagesToSms = model.SendSiteMessagesToSms;
			profile.SubscribeToNewsletterEmail = model.SubscribeToNewsletterEmail;

			GStoreDb.UserProfiles.Update(profile);
			GStoreDb.SaveChanges();

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Profile, UserActionActionEnum.Profile_UpdateNotifications, "", true);

			return RedirectToAction("Index");
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private async Task SendEmailConfirmationCode(string userId)
		{
			// Send an email with this link
			string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);

			var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);

			StoreFront storeFront = CurrentStoreFrontOrNull;
			string subject = Data.StoreFrontExtensions.EmailConfirmationCodeSubject(storeFront, callbackUrl, Request.Url);
			string messageHtml = Data.StoreFrontExtensions.EmailConfirmationCodeMessageHtml(storeFront, callbackUrl, Request.Url);

			await UserManager.SendEmailAsync(userId, subject, messageHtml);
		}


		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SignInAsync(Identity.AspNetIdentityUser user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private bool HasPassword()
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user != null)
			{
				return user.PasswordHash != null;
			}
			return false;
		}

		private bool HasPhoneNumber()
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user != null)
			{
				return user.PhoneNumber != null;
			}
			return false;
		}

		public enum ProfileMessageId
		{
			AddPhoneSuccess,
			ChangePasswordSuccess,
			SetTwoFactorSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			RemovePhoneSuccess,
			Error
		}

		#endregion
	}
}