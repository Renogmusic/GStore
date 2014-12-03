using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using GStore.Models;
using GStore.Data;
using GStore.Models.ViewModels;

namespace GStore.Controllers
{
	[System.Web.Mvc.Authorize()]
	public class AccountController : BaseClass.BaseController
	{
		private AspNetIdentityUserManager _userManager;

		public AccountController()
		{
			_throwErrorIfStoreFrontNotFound = false;
			_throwErrorIfUserProfileNotFound = false;
			_throwErrorIfAnonymous = false;
			_useInactiveStoreFrontAsActive = false;
		}

		public AccountController(GStore.Data.IGstoreDb dbContext) : base(dbContext)
		{
			_throwErrorIfStoreFrontNotFound = false;
			_throwErrorIfUserProfileNotFound = false;
			_throwErrorIfAnonymous = false;
			_useInactiveStoreFrontAsActive = false;
		}

		public AccountController(AspNetIdentityUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
			_throwErrorIfStoreFrontNotFound = false;
			_throwErrorIfUserProfileNotFound = false;
			_throwErrorIfAnonymous = false;
			_useInactiveStoreFrontAsActive = false;
		}


		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontOrThrow.AccountLayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontOrThrow.AccountTheme.FolderName;
			}
		}

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
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			if (CurrentStoreFrontOrThrow == null)
			{
				//client is inactive or not found
				System.Diagnostics.Debug.WriteLine("No active storefront");
			}

			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		private ApplicationSignInManager _signInManager;

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set { _signInManager = value; }
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);

			switch (result)
			{
				case SignInStatus.Success:
					Identity.AspNetIdentityUser user = SignInManager.UserManager.Users.Single(u => u.UserName.ToLower() == model.Email.ToLower());
					string userId = user.Id;

					UserProfile profile = GStoreDb.GetUserProfileByEmail(user.Email);
					profile.LastLogonDateTimeUtc = DateTime.UtcNow;
					GStoreDb.SaveChangesDirect();
					GStoreDb.LogSecurityEvent_LoginSuccess(this.HttpContext, this.RouteData, profile, this);

					if (profile.NotifyAllWhenLoggedOn)
					{
						string title = user.UserName;
						if (profile != null)
						{
							title = profile.FullName;

						}
						string message = "Logged on";


						Microsoft.AspNet.SignalR.IHubContext hubCtx = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GStore.Hubs.NotifyHub>();
						hubCtx.Clients.All.addNewMessageToPage(title, message);
					}


					return RedirectToLocal(returnUrl);

				case SignInStatus.LockedOut:
					UserProfile profileLockout = GStoreDb.GetUserProfileByEmail(model.Email);
					GStoreDb.LogSecurityEvent_LoginLockedOut(this.HttpContext, RouteData, model.Email, profileLockout, this);
					string notificationBaseUrl = Url.Action("Details", "Notifications", new { id = "" });
					string forgotPasswordUrl = Request.Url.Host + (Request.Url.IsDefaultPort ? string.Empty : ":" + Request.Url.Port) + Url.Action("ForgotPassword", "Account");
					CurrentStoreFrontOrThrow.HandleLockedOutNotification(GStoreDb, Request, profileLockout, notificationBaseUrl, forgotPasswordUrl);
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					UserProfile profileVerify = GStoreDb.GetUserProfileByEmail(model.Email);
					GStoreDb.LogSecurityEvent_LoginNeedsVerification(this.HttpContext, RouteData, model.Email, profileVerify, this);
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					UserProfile userProfileFailure = GStoreDb.GetUserProfileByEmail(model.Email, false);
					GStoreDb.LogSecurityEvent_LoginFailed(this.HttpContext, RouteData, model.Email, model.Password, userProfileFailure, this);
					//todo: handle login attempt with unknown email - maybe ask user to sign up, or let us know if spam?
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new Identity.AspNetIdentityUser(model.Email) { UserName = model.Email, Email = model.Email };
				user.TwoFactorEnabled = Properties.Settings.Current.IdentityEnableTwoFactorAuth;
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771

					await SendEmailConfirmationCode(user.Id);

					Data.IGstoreDb ctx = GStoreDb.NewContext(user.UserName);
					UserProfile newProfile = ctx.UserProfiles.Create();
					newProfile.UserId = user.Id;
					newProfile.UserName = user.UserName;
					newProfile.Email = user.Email;
					newProfile.FullName = model.FullName;
					newProfile.NotifyOfSiteUpdatesToEmail = model.NotifyOfSiteUpdates;
					newProfile.SendMoreInfoToEmail = model.SendMeMoreInfo;
					newProfile.SignupNotes = model.SignupNotes;
					newProfile.NotifyAllWhenLoggedOn = true;
					newProfile.IsPending = false;
					newProfile.Order = 1000;
					newProfile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					newProfile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					newProfile.StoreFrontId = CurrentStoreFrontOrThrow.StoreFrontId;
					newProfile.ClientId = this.CurrentClientOrThrow.ClientId;
					ctx.UserProfiles.Add(newProfile);
					ctx.SaveChanges();

					ctx.LogSecurityEvent_NewRegister(this.HttpContext, RouteData, newProfile, this);
					string notificationBaseUrl = Url.Action("Details", "Notifications", new { id = "" });
					CurrentStoreFrontOrThrow.HandleNewUserRegisteredNotifications(this.GStoreDb, Request, newProfile, notificationBaseUrl, true, true);

					if (Properties.Settings.Current.IdentityEnableNewUserRegisteredBroadcast && CurrentClientOrThrow.EnableNewUserRegisteredBroadcast)
					{
						string title = model.FullName;
						string message = "Newly registered!";
						Microsoft.AspNet.SignalR.IHubContext hubCtx = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GStore.Hubs.NotifyHub>();
						hubCtx.Clients.All.addNewMessageToPage(title, message);
					}

					return View("RegisterSuccess", newProfile);
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			if (User.Identity.IsAuthenticated)
			{
				UserProfile profile = GStoreDb.GetCurrentUserProfile(false, true);
				if (profile == null || profile.NotifyAllWhenLoggedOn)
				{
					string title = User.Identity.Name;
					if (profile != null)
					{
						title = profile.FullName;
					}
					string message = "Logged off";
					Microsoft.AspNet.SignalR.IHubContext hubCtx = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GStore.Hubs.NotifyHub>();
					hubCtx.Clients.All.addNewMessageToPage(title, message);
				}
				GStoreDb.LogSecurityEvent_LogOff(HttpContext, RouteData, profile, this);
			}

			AuthenticationManager.SignOut();
			return RedirectToLocal("~/");
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			// Require that the user has already logged in via username/password or external login
			if (!await SignInManager.HasBeenVerifiedAsync())
			{
				return HttpBadRequest("VerifyCode SignInManager.HasBeenVerifiedAsync call failed");
			}
			var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
			if (user != null)
			{
				var code = await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
			}
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/VerifyCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// The following code protects for brute force attacks against the two factor codes. 
			// If a user enters incorrect codes for a specified amount of time then the user account 
			// will be locked out for a specified amount of time. 
			// You can configure the account lockout settings in IdentityConfig
			var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					GStoreDb.LogSecurityEvent_VerificationCodeSuccess(HttpContext, RouteData, model.Code, model.Provider, model.ReturnUrl, null, this);
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					GStoreDb.LogSecurityEvent_VerificationCodeFailedLockedOut(HttpContext, RouteData, model.Code, model.Provider, model.ReturnUrl, null, this);
					return View("Lockout");
				case SignInStatus.Failure:
				default:
					GStoreDb.LogSecurityEvent_VerificationCodeFailedInvalidCode(HttpContext, RouteData, model.Code, model.Provider, model.ReturnUrl, null, this);
					ModelState.AddModelError("", "Invalid code.");
					return View(model);
			}
		}

		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return HttpBadRequest("ConfirmEmail UserId and Code are blank");
			}
			IdentityResult result;
			try
			{
				result = await UserManager.ConfirmEmailAsync(userId, code);
			}
			catch (InvalidOperationException ioe)
			{
				// ConfirmEmailAsync throws when the userId is not found.
				GStoreDb.LogSecurityEvent_EmailConfirmFailedUserNotFound(HttpContext, RouteData, userId, code, this);
				ViewBag.errorMessage = ioe.Message;
				return HttpBadRequest("ConfirmEmail InvalidOperationException");
			}

			if (result.Succeeded)
			{
				UserProfile profile = GStoreDb.GetUserProfileByAspNetUserId(userId);
				GStoreDb.LogSecurityEvent_EmailConfirmed(HttpContext, RouteData, profile, this);
				return View();
			}

			// If we got this far, something failed.
			UserProfile profileFailed = GStoreDb.GetUserProfileByAspNetUserId(userId, false);
			GStoreDb.LogSecurityEvent_EmailConfirmFailed(HttpContext, RouteData, userId, code, result.Errors, profileFailed, this);

			AddErrors(result);
			ViewBag.errorMessage = "ConfirmEmail failed";
			return HttpBadRequest("ConfirmEmail failed");
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByNameAsync(model.Email);
				//if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				if (user == null)
				{
					//todo: user not found; email the person to get them to sign up, or do nothing?
					GStoreDb.LogSecurityEvent_ForgotPasswordEmailNotFound(HttpContext, RouteData, model.Email, this);
					return View("ForgotPasswordConfirmation");
				}

				UserProfile profile = GStoreDb.GetUserProfileByEmail(model.Email);
				GStoreDb.LogSecurityEvent_ForgotPasswordSuccess(HttpContext, RouteData, model.Email, profile, this);

				// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

				StoreFront storeFront = CurrentStoreFrontOrNull;
				string subject = Data.StoreFrontExtensions.ForgotPasswordSubject(storeFront, callbackUrl, Request.Url);
				string messageHtml = Data.StoreFrontExtensions.ForgotPasswordMessageHtml(storeFront, callbackUrl, Request.Url);

				await UserManager.SendEmailAsync(user.Id, subject, messageHtml);
				return RedirectToAction("ForgotPasswordConfirmation", "Account");

			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			if (code == null)
			{
				return HttpBadRequest("ResetPassword code = null");
			}
			return View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await UserManager.FindByNameAsync(model.Email);
			if (user == null)
			{
				GStoreDb.LogSecurityEvent_PasswordResetFailedUnknownUser(HttpContext, RouteData, model.Email, this);
				
				// Don't reveal that the user does not exist
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				UserProfile profile = GStoreDb.GetUserProfileByEmail(model.Email, false);
				GStoreDb.LogSecurityEvent_PasswordResetSuccess(HttpContext, RouteData, model.Email, profile, this);

				return RedirectToAction("ResetPasswordConfirmation", "Account");
			}

			UserProfile profileFailed = GStoreDb.GetUserProfileByEmail(model.Email, false);
			GStoreDb.LogSecurityEvent_PasswordResetFailed(HttpContext, RouteData, model.Email, result.Errors, profileFailed, this);

			AddErrors(result);
			return View();
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/SendCode
		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return HttpBadRequest("SendCode SignInManager.GetVerifiedUserIdAsync failed");
			}
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		//
		// POST: /Account/SendCode
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return HttpBadRequest("SendCode SignInManager.SendTwoFactorCodeAsync failed. Provider: " + model.SelectedProvider);
			}

			GStoreDb.LogSecurityEvent_VerificationCodeSent(HttpContext, RouteData, model.ReturnUrl, model.SelectedProvider, null, this);
			
			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// If the user does not have an account, then prompt the user to create an account
					ViewBag.ReturnUrl = returnUrl;
					ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Profile");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new Identity.AspNetIdentityUser(model.Email) { UserName = model.Email, Email = model.Email };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SendEmailConfirmationCode(string userId)
		{
			//no logging needed here, it's logged in the register method

			// Send an email with this link
			string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
			var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);

			StoreFront storeFront = CurrentStoreFrontOrNull;
			string subject = Data.StoreFrontExtensions.EmailConfirmationCodeSubject(storeFront, callbackUrl, Request.Url);
			string messageHtml = Data.StoreFrontExtensions.EmailConfirmationCodeMessageHtml(storeFront, callbackUrl, Request.Url);

			await UserManager.SendEmailAsync(userId, "Confirm your account for " + CurrentStoreFrontOrThrow.Name, messageHtml);
		}


		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return Redirect("~/");
		}

		public class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}