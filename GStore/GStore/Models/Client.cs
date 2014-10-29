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
		[Editable(false)]
		public int ClientId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Folder { get; set; }

		public bool IsPending { get; set; }

		public bool EnablePageViewLog { get; set; }

		public bool EnableNewUserRegisteredBroadcast { get; set;}

		public DateTime StartDateTimeUtc { get; set; }
		
		public DateTime EndDateTimeUtc { get; set; }

		public bool UseSendGridEmail { get; set; }

		public string SendGridMailAccount { get; set; }

		[DataType(DataType.Password)]
		public string SendGridMailPassword { get; set; }

		public string SendGridMailFromEmail { get; set; }
		
		public string SendGridMailFromName { get; set; }

		public bool UseTwilioSms { get; set; }
		
		public string TwilioSid { get; set; }

		[DataType(DataType.Password)]
		public string TwilioToken { get; set; }

		public string TwilioFromPhone { get; set; }
		
		public string TwilioSmsFromName { get; set; }
		
		public string TwilioSmsFromEmail { get; set; }

		public virtual ICollection<StoreFront> StoreFronts { get; set; }

		public bool IsActiveDirect()
		{
			return IsActiveDirect(DateTime.UtcNow);
		}

		public bool IsActiveDirect(DateTime dateTimeUtc)
		{
			if ((!this.IsPending) && (this.StartDateTimeUtc < dateTimeUtc) && (this.EndDateTimeUtc > dateTimeUtc))
			{
				return true;
			}
			return false;
		}
	}
}