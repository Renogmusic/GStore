using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("StoreFronts")]
	public class StoreFront : BaseClasses.ClientLiveRecord
	{
		public int StoreFrontId { get; set; }

		[Required]
		public string Name { get; set; }

		[DataType(DataType.Url)]
		[Required]
		public string PublicUrl { get; set; }

		[Required]
		public string MetaApplicationName { get; set; }

		[Required]
		public string MetaApplicationTileColor { get; set; }

		public string MetaDescription { get; set; }

		public string MetaKeywords { get; set; }

		public int ThemeId { get; set; }
		public virtual Theme Theme { get; set; }

		[Required]
		public string Folder { get; set; }

		[Required]
		public string AccountLayout { get; set; }

		[Required]
		public string ManageLayout { get; set; }

		[Required]
		public string NotificationsLayout { get; set; }

		[Required]
		public string AdminLayout { get; set; }

		public int Admin_UserProfileId { get; set; }
		[ForeignKey("Admin_UserProfileId")]
		public virtual UserProfile Admin { get; set; }

		public int WelcomePerson_UserProfileId { get; set; }
		[ForeignKey("WelcomePerson_UserProfileId")]
		public virtual UserProfile WelcomePerson { get; set; }

		public int AccountAdmin_UserProfileId { get; set; }
		[ForeignKey("AccountAdmin_UserProfileId")]
		public virtual UserProfile AccountAdmin { get; set; }

		public int RegisteredNotify_UserProfileId { get; set; }
		[ForeignKey("RegisteredNotify_UserProfileId")]
		public virtual UserProfile RegisteredNotify { get; set; }

		public virtual ICollection<StoreBinding> StoreBindings { get; set; }
		public virtual ICollection<Page> Pages { get; set; }
		public virtual ICollection<UserProfile> UserProfiles { get; set; }

		public string OutgoingMessageSignature()
		{
			return "\n-Sent From " + Name + " \n " + PublicUrl;
		}

	}
}