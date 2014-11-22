using System;
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
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[MaxLength(100)]
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
		public string AccountLayoutName { get; set; }

		[Required]
		public string DefaultNewPageLayoutName { get; set; }

		[Required]
		public string ProfileLayoutName { get; set; }

		[Required]
		public string NotificationsLayoutName { get; set; }

		[Required]
		public string AdminLayoutName { get; set; }

		[Required]
		public string CatalogLayoutName { get; set; }

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

		public bool EnableGoogleAnalytics { get; set; }
		public string GoogleAnalyticsWebPropertyId { get; set; }

		[ForeignKey("ThemeId")]
		public virtual Theme Theme { get; set; }
		public int ThemeId { get; set; }


		public virtual ICollection<NavBarItem> NavBarItems { get; set; }
		public virtual ICollection<Page> Pages { get; set; }
		public virtual ICollection<Product> Products { get; set; }
		public virtual ICollection<ProductCategory> ProductCategories { get; set; }
		public virtual ICollection<StoreBinding> StoreBindings { get; set; }
		public virtual ICollection<UserProfile> UserProfiles { get; set; }

		public virtual ICollection<Notification> Notifications { get; set; }

		[ForeignKey("WelcomePerson_UserProfileId")]
		public virtual UserProfile WelcomePerson { get; set; }
		public int WelcomePerson_UserProfileId { get; set; }

		[ForeignKey("AccountAdmin_UserProfileId")]
		public virtual UserProfile AccountAdmin { get; set; }
		public int AccountAdmin_UserProfileId { get; set; }

		[ForeignKey("RegisteredNotify_UserProfileId")]
		public virtual UserProfile RegisteredNotify { get; set; }
		public int RegisteredNotify_UserProfileId { get; set; }

		///// <summary>
		///// File Not Found 404 Store Error Page or null if none (use system default 404 page)
		///// </summary>
		[ForeignKey("NotFoundError_PageId")]
		public virtual Page NotFoundErrorPage { get; set; }
		public int? NotFoundError_PageId { get; set; }

		///// <summary>
		///// Store Error Page (for any error other than not found 404) or null if none (use system default 404 page)
		///// </summary>
		[ForeignKey("StoreError_PageId")]
		public virtual Page StoreErrorPage { get; set; }
		public int? StoreError_PageId { get; set; }

	}
}