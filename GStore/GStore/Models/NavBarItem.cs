using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("NavBarItems")]
	public class NavBarItem : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name="Menu Bar Item Id")]
		public int NavBarItemId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(100)]
		public string Name { get; set; }

		public int Order { get; set; }

		[Display(Name = "HTML attributes")]
		public string htmlAttributes { get; set; }

		[Display(Name = "Open In a New Window")]
		public bool OpenInNewWindow { get; set; }

		[Display(Name = "Is System Action")]
		public bool IsAction { get; set; }

		public string Action { get; set; }

		public string Controller { get; set; }

		public string Area { get; set; }

		[Display(Name = "Action Id Parameter")]
		public int? ActionIdParam { get; set; }

		[Display(Name = "Is Page")]
		public bool IsPage { get; set; }

		[ForeignKey("PageId")]
		public virtual Page Page { get; set; }

		[Display(Name = "Page Id")]
		public int? PageId { get; set; }

		[Display(Name = "Is Internal Link")]
		public bool IsLocalHRef { get; set; }

		[Display(Name = "Internal Link")]
		public string LocalHRef { get; set; }

		[Display(Name = "Is External Link")]
		public bool IsRemoteHRef { get; set; }

		[Display(Name = "External Link")]
		public string RemoteHRef { get; set; }

		[Display(Name = "Show a Divider Before Item")]
		public bool UseDividerAfterOnMenu { get; set; }

		[Display(Name = "Show a Divider After Item")]
		public bool UseDividerBeforeOnMenu { get; set; }

		[Display(Name = "Registered Users Only")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "Anonymous Users Only")]
		public bool ForAnonymousOnly { get; set; }

		[Display(Name = "Parent Menu Item Id")]
		public int? ParentNavBarItemId { get; set; }

		[ForeignKey("ParentNavBarItemId")]
		[Display(Name = "Parent Menu Item")]
		public virtual NavBarItem ParentNavBarItem { get; set; }

	}
}