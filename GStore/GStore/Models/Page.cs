using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GStore.Models.Extensions;

namespace GStore.Models
{
	[Table("Pages")]
	public class Page : BaseClasses.StoreFrontLiveRecord 
	{
		[Key]
		[Editable(false)]
		public int PageId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int PageTemplateId { get; set; }
		[ForeignKey("PageTemplateId")]
		public virtual PageTemplate PageTemplate { get; set; }

		[Required]
		public string Name { get; set; }

		public int Order { get; set; }

		//todo: add url to routes
		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		[MaxLength(250)]
		public string Url { get; set; }

		public bool Public { get; set; }

		public string MetaDescription { get; set; }

		public string PageTitle { get; set; }

		public string BodyTopScriptTag { get; set; }
		public string BodyBottomScriptTag { get; set; }

		public virtual ICollection<PageSection> Sections { get; set; }

	}
}