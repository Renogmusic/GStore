using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace GStoreWeb.Controllers
{
	[System.Web.Mvc.Authorize()]
	public class AccountController : AreaBaseController.RootAreaBaseController
	{
		private AspNetIdentityUserManager _userManager;

		public AccountController()
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

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.AccountTheme.FolderName;
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
		public ActionResult Login(string returnUrl, bool? checkingOut)
		{
			if (CurrentStoreFrontOrNull == null)
			{
				System.Diagnostics.Debug.WriteLine("No active storefront");
			}

			LoginViewModel viewModel = new LoginViewModel() { CheckingOut = checkingOut };
			ViewBag.ReturnUrl = returnUrl;
			return View("Login", viewModel);
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
					AspNetIdentityUser user = SignInManager.UserManager.Users.Single(u => u.UserName.ToLower() == model.Email.ToLower());
					string userId = user.Id;
					UserProfile profile = GStoreDb.GetUserProfileByEmail(user.Email);
					if (!PostLoginAuthCheck(profile))
					{
						return RedirectToAction("Login", new { CheckingOut = model.CheckingOut });
					}

					profile.LastLogonDateTimeUtc = DateTime.UtcNow;
					GStoreDb.SaveChangesDirect();
					GStoreDb.LogSecurityEvent_LoginSuccess(this.HttpContext, this.RouteData, profile, this);

					StoreFront storeFront = CurrentStoreFrontOrNull;
					if (storeFront != null)
					{
						Cart cart = storeFront.GetCart(Session.SessionID, null);
						cart = storeFront.MigrateCartToProfile(GStoreDb, cart, profile, this);
					}
					if (profile.NotifyAllWhenLoggedOn)
					{
						string title = user.UserName;
						if (profile != null)
						{
							title = profile.FullName;

						}
						string message = "Logged on";


						Microsoft.AspNet.SignalR.IHubContext hubCtx = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GStoreWeb.Hubs.NotifyHub>();
						hubCtx.Clients.All.addNewMessageToPage(title, message);
					}

					if (model.CheckingOut ?? false)
					{
						return RedirectToAction("Index", "Checkout", new { ContinueAsLogin = true });
					}
					return RedirectToLocal(returnUrl);

				case SignInStatus.LockedOut:
					UserProfile profileLockout = GStoreDb.GetUserProfileByEmail(model.Email);
					GStoreDb.LogSecurityEvent_LoginLockedOut(this.HttpContext, RouteData, model.Email, profileLockout, this);
					string notificationBaseUrl = Url.Action("Details", "Notifications", new { id = "" });
					string forgotPasswordUrl = Request.Url.Host + (Request.Url.IsDefaultPort ? string.Empty : ":" + Request.Url.Port) + Url.Action("ForgotPassword", "Account");
					CurrentStoreFrontOrThrow.HandleLockedOutNotification(GStoreDb, Request, profileLockout, notificationBaseUrl, forgotPasswordUrl);
					ViewBag.CheckingOut = model.CheckingOut;
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					//allow pass-through even if storefront config is inactive because user may be an admin
					UserProfile profileVerify = GStoreDb.GetUserProfileByEmail(model.Email);
					GStoreDb.LogSecurityEvent_LoginNeedsVerification(this.HttpContext, RouteData, model.Email, profileVerify, this);
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe, CheckingOut = model.CheckingOut });
				case SignInStatus.Failure:
				default:
					UserProfile userProfileFailure = GStoreDb.GetUserProfileByEmail(model.Email, false);
					GStoreDb.LogSecurityEvent_LoginFailed(this.HttpContext, RouteData, model.Email, model.Password, userProfileFailure, this);

					if (userProfileFailure == null)
					{
						//unknown user, maybe ask to sign up?
						ModelState.AddModelError("", "User Name or Password is invalid. Please correct it and try again. ");
					}
					else
					{
						//looks like an existing user but wrong password
						ModelState.AddModelError("", "User Name or Password is invalid. Please check your password and try again. ");
					}
					return View(model);
			}
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register(bool? CheckingOut)
		{
			if (CurrentStoreFrontOrNull == null)
			{
				AddUserMessage("Store is Inactive.", "Sorry, this store is inactive. You cannot register until it is activated.", UserMessageType.Danger);
			}
			RegisterViewModel viewModel = new RegisterViewModel() { CheckingOut = CheckingOut };
			return View(viewModel);
		}

		[AllowAnonymous]
		public ActionResult Unauthorized(bool? checkingOut)
		{
			ViewBag.CheckingOut = checkingOut;
			return View("Unauthorized");
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			StoreFront storeFront = CurrentStoreFrontOrNull;
			StoreFrontConfiguration storeFrontConfig = CurrentStoreFrontConfigOrNull;
			if ((storeFront != null) && (storeFrontConfig != null) && (storeFrontConfig.RegisterWebForm != null) && storeFrontConfig.RegisterWebForm.IsActiveBubble())
			{
				FormProcessorExtensions.ValidateFields(this, storeFrontConfig.RegisterWebForm);
			}

			if (ModelState.IsValid)
			{
				var user = new AspNetIdentityUser(model.Email) { UserName = model.Email, Email = model.Email };
				user.TwoFactorEnabled = Settings.IdentityEnableTwoFactorAuth;
				IdentityResult result = null;
				try
				{
					result = await UserManager.CreateAsync(user, model.Password);
				}
				catch (System.Data.Entity.Validation.DbEntityValidationException exDbEx)
				{
					foreach (System.Data.Entity.Validation.DbEntityValidationResult valResult in exDbEx.EntityValidationErrors)
					{
						ICollection<System.Data.Entity.Validation.DbValidationError> valErrors = valResult.ValidationErrors;
						foreach (System.Data.Entity.Validation.DbValidationError error in valErrors)
						{
							ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
						}
					}
					return View(model);
				}
				catch (Exception ex)
				{
					string error = ex.ToString();
					throw;
				}
				if (result.Succeeded)
				{
					await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: false);

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771

					IGstoreDb ctx = GStoreDb;
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
					newProfile.Order = CurrentStoreFrontOrThrow.UserProfiles.Max(up => up.Order) + 10;
					newProfile.EntryDateTime = Session.EntryDateTime().Value;
					newProfile.EntryRawUrl = Session.EntryRawUrl();
					newProfile.EntryReferrer = Session.EntryReferrer();
					newProfile.EntryUrl = Session.EntryUrl();
					newProfile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					newProfile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					newProfile.StoreFrontId = CurrentStoreFrontOrThrow.StoreFrontId;
					newProfile.StoreFront = CurrentStoreFrontOrThrow;
					newProfile.ClientId = this.CurrentClientOrThrow.ClientId;
					newProfile.Client = this.CurrentClientOrThrow;
					newProfile = ctx.UserProfiles.Add(newProfile);
					ctx.SaveChanges();

					ctx.UserName = user.UserName;
					ctx.CachedUserProfile = null;


					string customFields = string.Empty;
					if (storeFrontConfig != null && storeFrontConfig.RegisterWebForm != null && storeFrontConfig.RegisterWebForm.IsActiveBubble())
					{
						FormProcessorExtensions.ProcessWebForm(this, storeFrontConfig.RegisterWebForm, null, true, null);
						customFields = FormProcessorExtensions.BodyTextCustomFieldsOnly(this, storeFrontConfig.RegisterWebForm);
					}

					bool confirmResult = SendEmailConfirmationCode(user.Id, newProfile);

					ctx.LogSecurityEvent_NewRegister(this.HttpContext, RouteData, newProfile, this);
					string notificationBaseUrl = Url.Action("Details", "Notifications", new { id = "" });
					CurrentStoreFrontOrThrow.HandleNewUserRegisteredNotifications(this.GStoreDb, Request, newProfile, notificationBaseUrl, true, true, customFields);

					if (storeFront != null)
					{
						Cart cart = storeFront.GetCart(Session.SessionID, null);
						cart = storeFront.MigrateCartToProfile(GStoreDb, cart, newProfile, this);
						storeFront.MigrateOrdersToNewProfile(GStoreDb, newProfile, this);
					}

					if (Settings.IdentityEnableNewUserRegisteredBroadcast && CurrentClientOrThrow.EnableNewUserRegisteredBroadcast)
					{
						string title = model.FullName;
						string message = "Newly registered!";
						Microsoft.AspNet.SignalR.IHubContext hubCtx = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GStoreWeb.Hubs.NotifyHub>();
						hubCtx.Clients.All.addNewMessageToPage(title, message);
					}

					if (model.CheckingOut ?? false)
					{
						return RedirectToAction("LoginOrGuest", "Checkout", new { ContinueAsLogin = true });
					}

					if (storeFrontConfig != null && storeFrontConfig.RegisterSuccess_PageId.HasValue)
					{
						return Redirect(storeFrontConfig.RegisterSuccessPage.UrlResolved(this.Url));
					}
					return RedirectToAction("RegisterSuccess");
				}
				AddErrors(result);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[Authorize]
		public ActionResult RegisterSuccess()
		{
			return View("RegisterSuccess", CurrentUserProfileOrThrow);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			if (User.IsRegistered())
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
					Microsoft.AspNet.SignalR.IHubContext hubCtx = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GStoreWeb.Hubs.NotifyHub>();
					hubCtx.Clients.All.addNewMessageToPage(title, message);
				}

				//do not migrate cart to anonymous at log off
				//StoreFront storeFront = CurrentStoreFrontOrNull;
				//if (storeFront != null)
				//{
				//	Cart cart = CurrentStoreFrontOrThrow.GetCart(Session.SessionID, profile);
				//	if (cart != null)
				//	{
				//		storeFront.MigrateCartToAnonymous(GStoreDb, cart);
				//	}
				//}
					
				GStoreDb.LogSecurityEvent_LogOff(HttpContext, RouteData, profile, this);
			}

			AuthenticationManager.SignOut();
			return RedirectToLocal("~/");
		}

		//
		// GET: /Account/VerifyCode
		[AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe, bool? checkingOut)
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
			return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe, CheckingOut = checkingOut });
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
					UserProfile profile = CurrentUserProfileOrThrow;
					if (!PostLoginAuthCheck(profile))
					{
						return RedirectToAction("Login", new { CheckingOut = model.CheckingOut });
					}

					profile.LastLogonDateTimeUtc = DateTime.UtcNow;
					GStoreDb.SaveChangesDirect();
					GStoreDb.LogSecurityEvent_VerificationCodeSuccess(HttpContext, RouteData, model.Code, model.Provider, model.ReturnUrl, profile, this);

					StoreFront storeFront = CurrentStoreFrontOrNull;
					if (storeFront != null)
					{
						Cart cart = storeFront.GetCart(Session.SessionID, null);
						cart = storeFront.MigrateCartToProfile(GStoreDb, cart, profile, this);
					}
					if (profile.NotifyAllWhenLoggedOn)
					{
						string title = User.Identity.Name;
						if (profile != null)
						{
							title = profile.FullName;

						}
						string message = "Logged on";


						Microsoft.AspNet.SignalR.IHubContext hubCtx = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<GStoreWeb.Hubs.NotifyHub>();
						hubCtx.Clients.All.addNewMessageToPage(title, message);
					}

					if (model.CheckingOut.HasValue && model.CheckingOut.Value)
					{
						return RedirectToAction("Index", "Cart");
					}
					return RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					GStoreDb.LogSecurityEvent_VerificationCodeFailedLockedOut(HttpContext, RouteData, model.Code, model.Provider, model.ReturnUrl, null, this);
					ViewBag.CheckingOut = model.CheckingOut;
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
		public async Task<ActionResult> ConfirmEmail(string userId, string code, bool? CheckingOut)
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
				ViewBag.CheckingOut = CheckingOut;
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
		public ActionResult ForgotPassword(bool? checkingOut)
		{
			ForgotPasswordViewModel viewModel = new ForgotPasswordViewModel() { CheckingOut = checkingOut };
			return View(viewModel);
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
				if (user == null)
				{
					//todo: user not found; email the person to get them to sign up, or do nothing?
					GStoreDb.LogSecurityEvent_ForgotPasswordEmailNotFound(HttpContext, RouteData, model.Email, this);
					return View("ForgotPasswordNoUser", model);
				}

				UserProfile profile = GStoreDb.GetUserProfileByEmail(model.Email, false);
				if (profile == null)
				{
					GStoreDb.LogSecurityEvent_ForgotPasswordProfileNotFound(HttpContext, RouteData, model.Email, this);
					return View("ForgotPasswordNoUser", model);
				}

				GStoreDb.LogSecurityEvent_ForgotPasswordSuccess(HttpContext, RouteData, model.Email, profile, this);

				// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

				StoreFront storeFront = CurrentStoreFrontOrNull;
				string subject = StoreFrontExtensions.ForgotPasswordSubject(storeFront, callbackUrl, Request.Url);
				string messageText = StoreFrontExtensions.ForgotPasswordMessageText(storeFront, callbackUrl, Request.Url);
				string messageHtml = StoreFrontExtensions.ForgotPasswordMessageHtml(storeFront, callbackUrl, Request.Url);
				bool result;
				try
				{
					result = this.SendEmail(profile.Email, profile.FullName, subject, messageText, messageHtml);
					if (result)
					{
						AddUserMessage("Email Sent", "A password reset email was sent to " + model.Email, UserMessageType.Info);
					}
					else
					{
						AddUserMessage("Error sending Email", "Email to " + model.Email + " failed.", UserMessageType.Danger);
					}
				}
				catch (Exception)
				{
					string message = "There was an error sending email to " + model.Email;
					AddUserMessage("Error sending Email", message, UserMessageType.Danger);
				}
				return RedirectToAction("ForgotPasswordConfirmation", "Account", new { CheckingOut = model.CheckingOut });

			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation(bool? checkingOut)
		{
			ViewBag.CheckingOut = checkingOut;
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code, bool? checkingOut)
		{
			if (code == null)
			{
				return HttpBadRequest("ResetPassword code = null");
			}
			ResetPasswordViewModel viewModel = new ResetPasswordViewModel() { CheckingOut = checkingOut };
			return View(viewModel);
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
				return RedirectToAction("ResetPasswordConfirmation", "Account", new { CheckingOut = model.CheckingOut});
			}
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
			{
				UserProfile profile = GStoreDb.GetUserProfileByEmail(model.Email, false);
				GStoreDb.LogSecurityEvent_PasswordResetSuccess(HttpContext, RouteData, model.Email, profile, this);

				return RedirectToAction("ResetPasswordConfirmation", "Account", new { CheckingOut = model.CheckingOut });
			}

			UserProfile profileFailed = GStoreDb.GetUserProfileByEmail(model.Email, false);
			GStoreDb.LogSecurityEvent_PasswordResetFailed(HttpContext, RouteData, model.Email, result.Errors, profileFailed, this);

			AddErrors(result);

			ResetPasswordViewModel viewModel = new ResetPasswordViewModel() { CheckingOut = model.CheckingOut };
			return View(viewModel);
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation(bool? checkingOut)
		{
			ViewBag.CheckingOut = checkingOut;
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
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe, bool? checkingOut)
		{
			var userId = await SignInManager.GetVerifiedUserIdAsync();
			if (userId == null)
			{
				return HttpBadRequest("SendCode SignInManager.GetVerifiedUserIdAsync failed");
			}
			ViewBag.CheckingOut = checkingOut;
			var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe, CheckingOut = checkingOut });
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
				SendCodeViewModel viewModel = new SendCodeViewModel() { CheckingOut = model.CheckingOut };
				return View(viewModel);
			}

			// Generate the token and send it
			if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return HttpBadRequest("SendCode SignInManager.SendTwoFactorCodeAsync failed. Provider: " + model.SelectedProvider);
			}

			GStoreDb.LogSecurityEvent_VerificationCodeSent(HttpContext, RouteData, model.ReturnUrl, model.SelectedProvider, null, this);

			return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe, CheckingOut = model.CheckingOut });
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
			var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: true);
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
			if (User.IsRegistered())
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
				var user = new AspNetIdentityUser(model.Email) { UserName = model.Email, Email = model.Email };
				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: false);
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

		/// <summary>
		/// Checks if user is allowed to log in. Mostly for inactive storefront bounce after login
		/// </summary>
		/// <param name="profile"></param>
		/// <returns></returns>
		protected bool PostLoginAuthCheck(UserProfile profile)
		{
			if (profile == null)
			{
				throw new ArgumentNullException("profile");
			}

			if (profile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				return true;
			}

			StoreFront currentStoreFront = CurrentStoreFrontOrNull;
			if (currentStoreFront != null)
			{
				return true;
			}

			//storefront is null. Inactive StoreFront record (nobody but sys admin can log in) or Config inactive (only storeadmins can log in)
			StoreFrontConfiguration testSFConfig = null;
			try
			{
				testSFConfig = GStoreDb.GetCurrentStoreFrontConfig(Request, false, true);
			}
			catch (Exception)
			{
			}

			if (testSFConfig == null)
			{
				//store front not found (no matching bindings or other reason)
				AuthenticationManager.SignOut();
				GStoreDb.LogSecurityEvent_LoginFailedNoStoreFront(this.HttpContext, this.RouteData, profile, this);
				AddUserMessage("Login Error", "This Store Front is not activated. Login is limited to System Admins only.", UserMessageType.Danger);
				return false;
			}

			int storeFrontId = testSFConfig.StoreFrontId;


			if (!testSFConfig.IsActiveBubble())
			{
				//store front inactive (no matching bindings or other reason)
				AuthenticationManager.SignOut();
				string inactiveStoreFrontName = testSFConfig.Name;
				GStoreDb.LogSecurityEvent_LoginFailedStoreFrontInactive(this.HttpContext, this.RouteData, inactiveStoreFrontName, storeFrontId, profile, this);
				AddUserMessage("Login Error", "This Store Front is inactive. Login is limited to System Admins only.", UserMessageType.Danger);
				return false;
			}

			if (testSFConfig == null)
			{
				//no config found; only users with store admin access can log in
				if (testSFConfig.StoreFront.Authorization_IsAuthorized(profile, GStoreAction.Admin_StoreAdminArea))
				{
					return true;
				}
				AuthenticationManager.SignOut();
				GStoreDb.LogSecurityEvent_LoginFailedNoStoreFrontConfig(this.HttpContext, this.RouteData, storeFrontId, profile, this);
				AddUserMessage("Login Error", "This Store Front Configuration is not activated. Login is limited to Site Administrators only.", UserMessageType.Danger);
				return false;
			}

			if (!testSFConfig.IsActiveBubble())
			{
				//all configs are inactive
				//no config found; only users with store admin access can log in
				if (testSFConfig.StoreFront.Authorization_IsAuthorized(profile, GStoreAction.Admin_StoreAdminArea))
				{
					return true;
				}
				AuthenticationManager.SignOut();

				string storeFrontName = testSFConfig.Name;
				string configName = testSFConfig.ConfigurationName;
				int configurationId = testSFConfig.StoreFrontConfigurationId;

				GStoreDb.LogSecurityEvent_LoginFailedStoreFrontConfigInactive(this.HttpContext, this.RouteData, storeFrontName, storeFrontId, configName, configurationId, profile, this);
				AddUserMessage("Login Error", "This Store Front Configuration is inactive. Login is limited to Site Administrators only.", UserMessageType.Danger);
				return false;
			}

			return true;

		}

		#region Helpers
		private ApplicationSignInManager _signInManager;

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set { _signInManager = value; }
		}

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private bool SendEmailConfirmationCode(string userId, UserProfile profile)
		{
			if (profile == null)
			{
				throw new ArgumentNullException("profile");
			}

			// Send an email with this link
			string code = UserManager.GenerateEmailConfirmationTokenAsync(userId).Result;
			var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);

			StoreFront storeFront = CurrentStoreFrontOrNull;
			string subject = StoreFrontExtensions.EmailConfirmationCodeSubject(storeFront, callbackUrl, Request.Url);
			string messageHtml = StoreFrontExtensions.EmailConfirmationCodeMessageHtml(storeFront, callbackUrl, Request.Url);
			string messageText = StoreFrontExtensions.EmailConfirmationCodeMessageText(storeFront, callbackUrl, Request.Url);

			bool result;
			try
			{
				result = this.SendEmail(profile.Email, profile.FullName, subject, messageText, messageHtml);
				if (result)
				{
					AddUserMessage("Email Sent", "Confirmation Email was sent to " + profile.Email, UserMessageType.Info);
				}
				else
				{
					AddUserMessage("Email Failed", "Sorry, we could not send a Confirmation Email to " + profile.Email, UserMessageType.Info);
				}
			}
			catch
			{
				result = false;
				AddUserMessage("Email Failed", "Sorry, there was an error sending the confirmation email to " + profile.Email, UserMessageType.Info);
			}

			return result;
		}


		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		/// <summary>
		/// redirects to a local url, adds "Login=true" to the url in case the returning page needs to handle referrer issues
		/// </summary>
		/// <param name="returnUrl"></param>
		/// <returns></returns>
		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (!string.IsNullOrEmpty(returnUrl))
			{
				if (Url.IsLocalUrl(returnUrl))
				{
					if (returnUrl.Contains('?'))
					{
						returnUrl += "&status=login";
					}
					else
					{
						returnUrl += "?status=login";
					}
					return Redirect(returnUrl);
				}
			}
			return Redirect("~/?status=login");
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
