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
		public bool EnableNewUserRegisteredBroadcast { get; set; }
		public bool EnablePageViewLog { get; set; }
		public string Name { get; set; }
		public string SendGridMailAccount { get; set; }
		public string SendGridMailFromEmail { get; set; }
		public string SendGridMailFromName { get; set; }
		public string SendGridMailPassword { get; set; }
		public string TwilioFromPhone { get; set; }
		public string TwilioSid { get; set; }
		public string TwilioSmsFromEmail { get; set; }
		public string TwilioSmsFromName { get; set; }
		public string TwilioToken { get; set; }
		public bool UseSendGridEmail { get; set; }
		public bool UseTwilioSms { get; set; }

	}
}