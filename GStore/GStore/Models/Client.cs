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
		[Display(Name = "Client Id")]
		public int ClientId { get; set; }

		[Required]
		[Index(IsUnique=true)]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		[Index(IsUnique = true)]
		[MaxLength(100)]
		public string Folder { get; set; }

		public int Order { get; set; }

		[Display(Name = "Log Page Views")]
		public bool EnablePageViewLog { get; set; }

		[Display(Name = "Broadcast New Users")]
		public bool EnableNewUserRegisteredBroadcast { get; set; }

		[Display(Name = "Use SG EMail")]
		public bool UseSendGridEmail { get; set; }

		[Display(Name = "SG Account")]
		public string SendGridMailAccount { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "SG Password")]
		public string SendGridMailPassword { get; set; }

		[Display(Name = "SG From Email")]
		public string SendGridMailFromEmail { get; set; }

		[Display(Name = "SG From Name")]
		public string SendGridMailFromName { get; set; }

		[Display(Name = "Use Sms")]
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

		[Display(Name = "Is Pending")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date")]
		public DateTime StartDateTimeUtc { get; set; }

		[Display(Name = "End Date")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Store Fronts")]
		public virtual ICollection<StoreFront> StoreFronts { get; set; }

		[Display(Name = "Client Roles")]
		public virtual ICollection<ClientRole> ClientRoles { get; set; }

	}
}