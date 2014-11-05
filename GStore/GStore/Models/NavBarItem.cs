using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("NavBarItems")]
	public class NavBarItem : BaseClasses.StoreFrontLiveRecord
	{
		public int NavBarItemId { get; set; }

		[Required]
		public string Name { get; set; }

		public int Order { get; set; }

		public string htmlAttributes { get; set; }

		public bool OpenInNewWindow { get; set; }

		public bool IsAction { get; set; }
		public string Action { get; set; }
		public string Controller { get; set; }
		public string Area { get; set; }
		public int? ActionIdParam { get; set; }

		public bool IsLocalHRef { get; set; }
		public string LocalHRef { get; set; }

		public bool IsRemoteHRef { get; set; }
		public string RemoteHRef { get; set; }

		public bool UseDividerAfterOnMenu { get; set; }
		public bool UseDividerBeforeOnMenu { get; set; }

		public bool ForRegisteredOnly { get; set; }

		public int? ParentNavBarItemId { get; set; }

		[ForeignKey("ParentNavBarItemId")]
		public virtual NavBarItem ParentNavBarItem { get; set; }

	}
}