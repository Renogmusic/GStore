using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using GStore.Models;
using GStore.Models.Extensions;
using GStore.Models.ViewModels;

namespace GStore.Controllers
{
	[Authorize]
	public class ManageController : BaseClass.BaseController
	{
		public ManageController()
		{
		}

		public ManageController(AspNetIdentityUserManager userManager)
		{
			UserManager = userManager;
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
		// GET: /Manage/Index
		public async Task<ActionResult> Index(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
				: message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
				: message == ManageMessageId.Error ? "An error has occurred."
				: message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
				: message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
				: "";


			UserProfile profile = GStoreDb.GetCurrentUserProfile();
			Identity.AspNetIdentityUser aspUser = profile.AspNetIdentityUser();

			var model = new ManageViewModel
			{
				HasPassword = HasPassword(),
				PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId()),
				TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
				Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()),
				BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId()),
				EmailConfirmed = aspUser.EmailConfirmed,
				AspNetIdentityUser = aspUser,
				UserProfile = profile
			};
			return View(model);
		}

		//
		// GET: /Manage/RemoveLogin
		public ActionResult RemoveLogin()
		{
			var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
			ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
			return View(linkedAccounts);
		}

		//
		// POST: /Manage/RemoveLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
		{
			ManageMessageId? message;
			var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}
				message = ManageMessageId.RemoveLoginSuccess;
			}
			else
			{
				message = ManageMessageId.Error;
			}
			return RedirectToAction("ManageLogins", new { Message = message });
		}

		//
		// GET: /Manage/AddPhoneNumber
		public ActionResult AddPhoneNumber()
		{
			return View();
		}

		//
		// POST: /Manage/AddPhoneNumber
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
				var message = new IdentityMessage
				{
					Destination = model.Number,
					Body = "Your security code is: " + code + " \n"
					+ CurrentStoreFront.OutgoingMessageSignature()
				};
				await UserManager.SmsService.SendAsync(message);
			}
			return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
		}

		//
		// POST: /Manage/EnableTwoFactorAuthentication
		[HttpPost]
		public async Task<ActionResult> EnableTwoFactorAuthentication()
		{
			await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			return RedirectToAction("Index", "Manage");
		}

		//
		// POST: /Manage/DisableTwoFactorAuthentication
		[HttpPost]
		public async Task<ActionResult> DisableTwoFactorAuthentication()
		{
			await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			return RedirectToAction("Index", "Manage");
		}

		//
		// GET: /Manage/VerifyPhoneNumber
		public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
		{
			var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
			// Send an SMS through the SMS provider to verify the phone number
			return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
		}

		//
		// POST: /Manage/VerifyPhoneNumber
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
				return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
			}
			// If we got this far, something failed, redisplay form
			ModelState.AddModelError("", "Failed to verify phone");
			return View(model);
		}

		//
		// GET: /Manage/RemovePhoneNumber
		public async Task<ActionResult> RemovePhoneNumber()
		{
			var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
			if (!result.Succeeded)
			{
				return RedirectToAction("Index", new { Message = ManageMessageId.Error });
			}
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
			}
			return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
		}

		//
		// GET: /Manage/ChangePassword
		public ActionResult ChangePassword()
		{
			return View();
		}

		//
		// POST: /Manage/ChangePassword
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				if (user != null)
				{
					await SignInAsync(user, isPersistent: false);
				}


				UserProfile profile = GStoreDb.GetCurrentUserProfile();
				UserProfile accountAdmin = CurrentStoreFront.AccountAdmin;
				Notification notification = GStoreDb.Notifications.Create();

				notification.From = (accountAdmin == null ? "System Administrator" : accountAdmin.FullName);
				notification.FromUserProfileId = (accountAdmin == null ? 0 : accountAdmin.UserProfileId);
				notification.UserProfileId = profile.UserProfileId;
				notification.To = profile.FullName;
				notification.Importance = "Normal";
				notification.Subject = "FYI - Your password has been changed";
				notification.UrlHost = Request.Url.Host;
				if (!Request.Url.IsDefaultPort)
				{
					notification.UrlHost += ":" + Request.Url.Port;
				}

				notification.BaseUrl = Url.Action("Details", "Notifications", new { id = "" });
				notification.Message = "FYI - Your password was changed on " + Request.Url.Host
					+ " \n - This is just a courtesy message to let you know.";

				GStoreDb.Notifications.Add(notification);
				GStoreDb.SaveChanges();

				return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
			}
			AddErrors(result);
			return View(model);
		}

		//
		// GET: /Manage/SetPassword
		public ActionResult SetPassword()
		{
			return View();
		}

		//
		// POST: /Manage/SetPassword
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
					return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Manage/ManageLogins
		public async Task<ActionResult> ManageLogins(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
				: message == ManageMessageId.Error ? "An error has occurred."
				: "";
			var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			if (user == null)
			{
				throw new ApplicationException("Cannot find user name: " + User.Identity.Name);
			}
			var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
			var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
			ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
			return View(new ManageLoginsViewModel
			{
				CurrentLogins = userLogins,
				OtherLogins = otherLogins
			});
		}

		//
		// POST: /Manage/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
		}

		//
		// GET: /Manage/LinkLoginCallback
		public async Task<ActionResult> LinkLoginCallback()
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
			if (loginInfo == null)
			{
				return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
			}
			var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
			return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
		}

		[Authorize]
		public async Task<ActionResult> ConfirmEmail()
		{
			UserProfile profile = GStoreDb.GetCurrentUserProfile();
			string userId = profile.UserId;
			await this.SendEmailConfirmationCode(userId);
			return View(profile);
		}

		//
		// GET: /Manage/Settings
		public ActionResult Notifications()
		{
			UserProfile profile = GStoreDb.GetCurrentUserProfile();

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
		// POST: /Manage/Settings
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Notifications(NotificationSettingsViewModel model)
		{

			UserProfile profile = GStoreDb.GetCurrentUserProfile();

			profile.AllowUsersToSendSiteMessages = model.AllowUsersToSendSiteMessages;
			profile.NotifyAllWhenLoggedOn = model.NotifyAllWhenLoggedOn;
			profile.NotifyOfSiteUpdatesToEmail = model.NotifyOfSiteUpdatesToEmail;
			profile.SendSiteMessagesToEmail = model.SendSiteMessagesToEmail;
			profile.SubscribeToNewsletterEmail = model.SubscribeToNewsletterEmail;

			GStoreDb.SaveChanges();

			return RedirectToAction("Index");
		}

		public ActionResult ErrorTest()
		{
			throw new ApplicationException("Some error test!");
		}

		protected override void HandleUnknownAction(string actionName)
		{
			//string action = ControllerContext.RouteData.Values["action"].ToString();
			//string controller = ControllerContext.RouteData.Values["controller"].ToString();

			//string message = "Unknown action: " + actionName;
			//GStoreDbContext ctx = new GStoreDbContext(User);
			//ctx.LogSystemEvent(HttpContext, controller + action, SystemEventLevel.ApplicationException, message);
			//this.TempData.Add("Error", new ApplicationException(message));

			base.HandleUnknownAction(actionName);
		}
		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private async Task SendEmailConfirmationCode(string userId)
		{
			// Send an email with this link
			string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
			var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);
			string messageHtml = "Thank you for registering at " + Request.Url.Host + "!<br/><br/>"
				+ "<a href=\"" + callbackUrl + "\">Please click this link to confirm your email address</a>"
				+ "<br/><br/>" + Properties.Settings.Default.IdentitySendGridMailFromName + " - " + Properties.Settings.Default.IdentitySendGridMailFromEmail
				+ Server.HtmlEncode(CurrentStoreFront.OutgoingMessageSignature()).Replace("\n", " \n<br/>");


			await UserManager.SendEmailAsync(userId, "Please confirm your Email account for " + CurrentStoreFront.Name, messageHtml);
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

		public enum ManageMessageId
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