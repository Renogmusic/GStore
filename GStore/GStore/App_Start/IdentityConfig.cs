using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using System.Net;
using System.Configuration;
using Twilio;
using System.Diagnostics;
using Microsoft.AspNet.Identity;

namespace GStore
{
	public class EmailService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			return configSendGridasync(message);
		}

		private Task configSendGridasync(IdentityMessage message)
		{
			if (!Settings.AppEnableEmail)
			{
				return Task.FromResult(0);
			}

			string sendGridMailFromEmail = Settings.IdentitySendGridMailFromEmail;
			string sendGridMailFromName = Settings.IdentitySendGridMailFromName;

			string textSignature = "\n-Sent from GStore " + HttpContext.Current.Request.Url.Host;
			string sendGridMailAccount = Settings.IdentitySendGridMailAccount;
			string sendGridMailPassword = Settings.IdentitySendGridMailPassword;

			//string sendGridMailFromEmail = storeFront.Client.SendGridMailFromEmail;
			//string sendGridMailFromName = storeFront.Client.SendGridMailFromName;
			//string textSignature = storeFront.OutgoingMessageSignature();
			//string sendGridMailAccount = storeFront.Client.IdentitySendGridMailAccount;
			//string sendGridMailPassword = storeFront.Client.IdentitySendGridMailPassword;

			var myMessage = new SendGridMessage();
			myMessage.AddTo(message.Destination);
			myMessage.From = new System.Net.Mail.MailAddress(sendGridMailFromEmail, sendGridMailFromName);
			myMessage.Subject = message.Subject;
			myMessage.Text = message.Body + textSignature;
			myMessage.Html = message.Body + HttpUtility.HtmlEncode(textSignature).Replace("\n", "\n<br/>");

			var credentials = new NetworkCredential(
				sendGridMailAccount,
				sendGridMailPassword
					   );

			// Create a Web transport for sending email.
			var transportWeb = new Web(credentials);

			// Send the email.
			if (transportWeb != null)
			{
				return transportWeb.DeliverAsync(myMessage);
			}
			else
			{
				return Task.FromResult(0);
			}
		}
	}

	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			if (!Settings.AppEnableSMS)
			{
				return Task.FromResult(0);
			}

			string twilioSid = Settings.IdentityTwilioSid;
			string twilioToken = Settings.IdentityTwilioToken;
			string twilioFromPhone = Settings.IdentityTwilioFromPhone;
			string textSignature = "\n-Sent from GStore " + HttpContext.Current.Request.Url.Host;

			var Twilio = new TwilioRestClient(
				twilioSid,
				twilioToken
		   );
			
			var result = Twilio.SendMessage(twilioFromPhone,
			   message.Destination, message.Body);

			// Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
			Trace.TraceInformation(result.Status);

			// Twilio doesn't currently have an async API, so return success.
			return Task.FromResult(0);
		}
	}

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class AspNetIdentityUserManager : UserManager<Identity.AspNetIdentityUser, string>
    {
		public AspNetIdentityUserManager(IUserStore<Identity.AspNetIdentityUser, string> store)
            : base(store)
        {
        }

        public static AspNetIdentityUserManager Create(IdentityFactoryOptions<AspNetIdentityUserManager> options, IOwinContext context) 
        {
			var manager = new AspNetIdentityUserManager(new Microsoft.AspNet.Identity.EntityFramework.UserStore<Identity.AspNetIdentityUser, Identity.AspNetIdentityRole, string, Identity.AspNetIdentityUserLogin, Identity.AspNetIdentityUserRole, Identity.AspNetIdentityUserClaim>(context.Get<Identity.AspNetIdentityContext>()));
            // Configure validation logic for usernames
			manager.UserValidator = new UserValidator<Identity.AspNetIdentityUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
			manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<Identity.AspNetIdentityUser>
            {
                MessageFormat = "Your security code is {0} \n " + Settings.IdentityTwoFactorSignature
            });
			manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<Identity.AspNetIdentityUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0} \n " + Settings.IdentityTwoFactorSignature
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
					new DataProtectorTokenProvider<Identity.AspNetIdentityUser>(dataProtectionProvider.Create("ASP.NET Identity"))
					{
						TokenLifespan = TimeSpan.FromHours(24)
					};
            }

            return manager;
        }
    }

	public class AspNetIdentityRoleManager : RoleManager<Identity.AspNetIdentityRole, string>
	{
		public AspNetIdentityRoleManager(IRoleStore<Identity.AspNetIdentityRole, string> roleStore) : base(roleStore)
		{
		}

	}


    // Configure the application sign-in manager which is used in this application.
	public class ApplicationSignInManager : SignInManager<Identity.AspNetIdentityUser, string>
    {
        public ApplicationSignInManager(AspNetIdentityUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

		public override Task<ClaimsIdentity> CreateUserIdentityAsync(Identity.AspNetIdentityUser user)
        {
            return user.GenerateUserIdentityAsync(base.UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<AspNetIdentityUserManager>(), context.Authentication);
        }
    }
}
