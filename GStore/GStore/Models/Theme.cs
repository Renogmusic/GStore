using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("Themes")]
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
		public string FolderName { get; set; }

		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Required]
		[Display(Name = "Order")]
		public int Order { get; set; }

		[Display(Name = "Account Store Fronts")]
		public virtual ICollection<StoreFront> AccountStoreFronts { get; set; }

		[Display(Name = "Admin Store Fronts")]
		public virtual ICollection<StoreFront> AdminStoreFronts { get; set; }

		[Display(Name = "Catalog Store Fronts")]
		public virtual ICollection<StoreFront> CatalogStoreFronts { get; set; }

		[Display(Name = "Default New Page Store Fronts")]
		public virtual ICollection<StoreFront> DefaultNewPageStoreFronts { get; set; }

		[Display(Name = "Notifications Store Fronts")]
		public virtual ICollection<StoreFront> NotificationsStoreFronts { get; set; }

		[Display(Name = "Profile Store Fronts")]
		public virtual ICollection<StoreFront> ProfileStoreFronts { get; set; }

	}
}