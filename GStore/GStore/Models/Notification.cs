using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("Notifications")]
	public class Notification : BaseClasses.StoreFrontRecord
	{
		public int NotificationId { get; set; }
		public int UserProfileId { get; set; }
		public int FromUserProfileId { get; set; }

		[Required]
		public string To { get; set; }

		[Required]
		public string From { get; set; }

		[Required]
		public string Subject  { get; set; }

		[Display(Name="Priority")]
		[Required]
		public string Importance { get; set;}

		[DataType(DataType.MultilineText)]
		[AllowHtml]
		[Required]
		public string Message { get; set; }

		[Display(Name = "Read")]
		public bool Read { get; set; }

		[Required]
		public string UrlHost { get; set; }

		[Required]
		public string BaseUrl { get; set; }

		public virtual UserProfile UserProfile { get; set; }

		/// <summary>
		/// Notifications table data for user
		/// </summary>
		[Display(Name="Links")]
		public virtual ICollection<NotificationLink> NotificationLinks { get; set; }


	}

}
