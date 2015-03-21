using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("Notification")]
	public class Notification : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int NotificationId { get; set; }

		public int ToUserProfileId { get; set; }

		[ForeignKey("ToUserProfileId")]
		public virtual UserProfile ToUserProfile { get; set; }

		public int FromUserProfileId { get; set; }

		[ForeignKey("FromUserProfileId")]
		public virtual UserProfile FromUserProfile { get; set; }

		[Required]
		public string To { get; set; }

		[Required]
		public string From { get; set; }

		[Required]
		public string Subject  { get; set; }

		[Display(Name="Priority")]
		[Required]
		public string Importance { get; set;}

		[AllowHtml]
		[Required]
		[DataType(DataType.Html)]
		public string Message { get; set; }

		[Display(Name = "Read")]
		public bool Read { get; set; }

		[Required]
		public string UrlHost { get; set; }

		[Required]
		public string BaseUrl { get; set; }

		public int? OrderId { get; set; }

		public virtual Order Order { get; set; }

		/// <summary>
		/// Notifications table data for user
		/// </summary>
		[Display(Name="Links")]
		public virtual ICollection<NotificationLink> NotificationLinks { get; set; }

	}
}
