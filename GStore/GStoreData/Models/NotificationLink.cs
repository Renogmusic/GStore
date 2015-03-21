using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("NotificationLink")]
	public class NotificationLink : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		public int NotificationLinkId { get; set; }

		[ForeignKey("NotificationId")]
		public virtual Notification Notification { get; set; }
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int NotificationId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int Order { get; set; }

		public bool IsExternal { get; set; }

		[Required]
		public string Url { get; set; }

		[Required]
		public string LinkText { get; set; }


	}
}