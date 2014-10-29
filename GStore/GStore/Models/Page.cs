using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("Pages")]
	public class Page : BaseClasses.ClientLiveRecord
	{
		[Editable(false)]
		public int PageId { get; set; }

		public int StoreFrontId { get; set; }
		[ForeignKey("StoreFrontId")]
		public virtual StoreFront StoreFront { get; set; }

		public int PageTemplateId { get; set; }
		[ForeignKey("PageTemplateId")]
		public virtual PageTemplate PageTemplate { get; set; }

		[Required]
		public string Name { get; set; }

		public int Order { get; set; }

		//todo: add url to routes
		[Required]
		public string Url { get; set; }

		public bool Public { get; set; }

		public string MetaDescription { get; set; }

		public string PageTitle { get; set; }

		public string BodyTopScriptTag { get; set; }
		public string BodyBottomScriptTag { get; set; }

		public virtual ICollection<PageSection> Sections { get; set; }

		public bool SectionsAllSpecified()
		{
			return false;
		}
	}
}