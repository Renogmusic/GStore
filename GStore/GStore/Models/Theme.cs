using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("Theme")]
	public class Theme : BaseClasses.ClientRecord
	{
		[Display(Name="Theme Id")]
		public int ThemeId { get; set; }
		
		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[MaxLength(100)]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Folder Name")]
		[UIHint("ThemeFolder")]
		public string FolderName { get; set; }

		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Required]
		[Display(Name = "Order")]
		public int Order { get; set; }

		[Display(Name = "Account Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> AccountStoreFrontConfigurations { get; set; }

		[Display(Name = "Admin Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> AdminStoreFrontConfigurations { get; set; }

		[Display(Name = "Catalog Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> CatalogStoreFrontConfigurations { get; set; }

		[Display(Name = "Default New Page Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> DefaultNewPageStoreFrontConfigurations { get; set; }

		[Display(Name = "Notifications Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> NotificationsStoreFrontConfigurations { get; set; }

		[Display(Name = "Profile Store Front Configurations")]
		public virtual ICollection<StoreFrontConfiguration> ProfileStoreFrontConfigurations { get; set; }

	}
}