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

		[Required]
		public string Folder { get; set; }

		public int NavBarCatalogMaxLevels { get; set; }

		public int CatalogPageInitialLevels { get; set; }

		public int NavBarItemsMaxLevels { get; set; }

		[Required]
		public string AccountLayout { get; set; }

		[Required]
		public string ManageLayout { get; set; }

		[Required]
		public string NotificationsLayout { get; set; }

		[Required]
		public string CatalogLayout { get; set; }

		[Required]
		public int CatalogCategoryColSm { get; set; }

		[Required]
		public int CatalogCategoryColMd { get; set; }

		[Required]
		public int CatalogCategoryColLg { get; set; }


		[Required]
		public int CatalogProductColSm { get; set; }

		[Required]
		public int CatalogProductColMd { get; set; }

		[Required]
		public int CatalogProductColLg { get; set; }

		[Required]
		public string AdminLayout { get; set; }

		public int WelcomePerson_UserProfileId { get; set; }
		[ForeignKey("WelcomePerson_UserProfileId")]
		public virtual UserProfile WelcomePerson { get; set; }

		public int AccountAdmin_UserProfileId { get; set; }
		[ForeignKey("AccountAdmin_UserProfileId")]
		public virtual UserProfile AccountAdmin { get; set; }

		public int RegisteredNotify_UserProfileId { get; set; }
		[ForeignKey("RegisteredNotify_UserProfileId")]
		public virtual UserProfile RegisteredNotify { get; set; }

		[ForeignKey("ThemeId")]
		public virtual Theme Theme { get; set; }
		public int ThemeId { get; set; }


		public virtual ICollection<NavBarItem> NavBarItems { get; set; }
		public virtual ICollection<Page> Pages { get; set; }
		public virtual ICollection<Product> Products { get; set; }
		public virtual ICollection<ProductCategory> ProductCategories { get; set; }
		public virtual ICollection<StoreBinding> StoreBindings { get; set; }
		public virtual ICollection<StoreFrontUserRole> StoreFrontUserRoles { get; set; }
		public virtual ICollection<UserProfile> UserProfiles { get; set; }


		public virtual ICollection<Notification> Notifications { get; set; }

		public string OutgoingMessageSignature()
		{
			return "\n-Sent From " + Name + " \n " + PublicUrl;
		}

		public string StoreFrontVirtualDirectoryToMap()
		{
			return this.ClientVirtualDirectoryToMap() + "/StoreFronts/" + System.Web.HttpUtility.UrlEncode(this.Folder);
		}
	}
}