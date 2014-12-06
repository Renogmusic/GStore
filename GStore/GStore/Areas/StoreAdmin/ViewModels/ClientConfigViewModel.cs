using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class ClientConfigViewModel : StoreAdminViewModel
	{
		public ClientConfigViewModel() { }

		public ClientConfigViewModel(Client client, StoreFront storeFront, UserProfile userProfile): base(storeFront, userProfile)
		{
			this.ClientId = client.ClientId;
			this.EnableNewUserRegisteredBroadcast = client.EnableNewUserRegisteredBroadcast;
			this.EnablePageViewLog = client.EnablePageViewLog;
			this.Name = client.Name;
			this.SendGridMailAccount = client.SendGridMailAccount;
			this.SendGridMailFromEmail = client.SendGridMailFromEmail;
			this.SendGridMailFromName = client.SendGridMailFromName;
			this.SendGridMailPassword = client.SendGridMailPassword;
			this.TwilioFromPhone = client.TwilioFromPhone;
			this.TwilioSid = client.TwilioSid;
			this.TwilioSmsFromEmail = client.TwilioSmsFromEmail;
			this.TwilioSmsFromName = client.TwilioSmsFromName;
			this.TwilioToken = client.TwilioToken;
			this.UseSendGridEmail = client.UseSendGridEmail;
			this.UseTwilioSms = client.UseTwilioSms;
		}

		[Key]
		public int ClientId { get; set; }

		[Display(Name = "Enable New User Registered Broadcast", Description = "Check this box to have the site display a message when a new user registers")]
		public bool EnableNewUserRegisteredBroadcast { get; set; }

		[Display(Name = "Enable Page View Log", Description = "Check this box to turn on the Page View Log (records each visit to your site)")]
		public bool EnablePageViewLog { get; set; }

		[Required]
		[Display(Name = "Client Name", Description = "Internal Name used on your reports and your administration pages")]
		public string Name { get; set; }

		[Display(Name = "Use SendGrid Email", Description = "Check this box to use a SendGrid Email account to send email. If not selected, email will be disabled.")]
		public bool UseSendGridEmail { get; set; }

		[Display(Name = "SendGrid Mail Account", Description = "SendGrid Account name for sending email from your web sites.")]
		public string SendGridMailAccount { get; set; }

		[Display(Name = "SendGrid From Email", Description = "FROM Email address for outgoing SendGrid email.")]
		public string SendGridMailFromEmail { get; set; }

		[Display(Name = "SendGrid From Name", Description = "FROM Name for outgoing SendGrid email.")]
		public string SendGridMailFromName { get; set; }

		[Display(Name = "SendGrid Password", Description = "Password for SendGrid to transmit outgoing email.")]
		public string SendGridMailPassword { get; set; }

		[Display(Name = "Use Twilio SMS Text", Description = "Check this box to use a Twilio account to send SMS Text messages.")]
		public bool UseTwilioSms { get; set; }

		[Display(Name = "Twilio Phone Number", Description = "Twilio SMS Text Phone number. \nExample: +18885551234")]
		public string TwilioFromPhone { get; set; }

		[Display(Name = "Twilio API Sid", Description = "Twilio API Key (from their web site) for sending Text Messages. \nExample: AC1234ccc1111dddd1111g121212c1111c")]
		public string TwilioSid { get; set; }
		
		[Display(Name = "Twilio SMS From Email", Description = "Email used in signature used when sending Twilio SMS Messages")]
		public string TwilioSmsFromEmail { get; set; }

		[Display(Name = "Twilio SMS From Name", Description = "Name used in signature used when sending Twilio SMS Messages")]
		public string TwilioSmsFromName { get; set; }

		[Display(Name = "Twilio API Token", Description = "Twilio TOKEN (from their web site) for sending Text Messages. \nExample: 44c111c11fffffeedb111d111ebb1111")]
		public string TwilioToken { get; set; }
		

	}
}