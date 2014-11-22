using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GStore.Models
{
	[Table("Clients")]
	public class Client : BaseClasses.AuditFieldsAllRequired
	{
		[Key]
		[Editable(false)]
		[Index("UniqueRecord", IsUnique = true, Order = 1)]
		public int ClientId { get; set; }

		[Required]
		[Index(IsUnique=true)]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		[Index(IsUnique = true)]
		[MaxLength(100)]
		public string Folder { get; set; }

		[Display(Name="Pending")]
		public bool IsPending { get; set; }

		[Display(Name = "Page View Log")]
		public bool EnablePageViewLog { get; set; }

		[Display(Name = "New User Boadcast")]
		public bool EnableNewUserRegisteredBroadcast { get; set; }

		[Display(Name = "Start Date UTC")]
		public DateTime StartDateTimeUtc { get; set; }

		[Display(Name = "End Date UTC")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Send Grid EMail")]
		public bool UseSendGridEmail { get; set; }

		[Display(Name = "Send Grid Account")]
		public string SendGridMailAccount { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Send Grid Password")]
		public string SendGridMailPassword { get; set; }

		[Display(Name = "Send Grid From")]
		public string SendGridMailFromEmail { get; set; }

		[Display(Name = "Send Grid Name")]
		public string SendGridMailFromName { get; set; }

		[Display(Name = "Use Twilio Sms")]
		public bool UseTwilioSms { get; set; }

		[Display(Name = "Twilio Sid")]
		public string TwilioSid { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Twilio Token")]
		public string TwilioToken { get; set; }

		[Display(Name = "Twilio Phone")]
		public string TwilioFromPhone { get; set; }

		[Display(Name = "Twilio From Name")]
		public string TwilioSmsFromName { get; set; }

		[Display(Name = "Twilio From Email")]
		public string TwilioSmsFromEmail { get; set; }

		public virtual ICollection<StoreFront> StoreFronts { get; set; }

		public virtual ICollection<ClientRole> ClientRoles { get; set; }

	}
}