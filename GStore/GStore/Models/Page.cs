using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GStore.Data;

namespace GStore.Models
{
	[Table("Pages")]
	public class Page : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Page Id")]
		public int PageId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Page Template Id")]
		public int PageTemplateId { get; set; }

		[ForeignKey("PageTemplateId")]
		[Display(Name = "Page Template")]
		public virtual PageTemplate PageTemplate { get; set; }

		[Required]
		public string Name { get; set; }

		public int Order { get; set; }

		[ForeignKey("ThemeId") ]
		public virtual Theme Theme { get; set;}

		[Display(Name="Theme Id")]
		public int ThemeId { get; set; }

		//todo: add url to routes
		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		[MaxLength(250)]
		public string Url { get; set; }

		public bool Public { get; set; }

		[Display(Name = "Meta Tag Description")]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords")]
		public string MetaKeywords { get; set; }

		[Display(Name = "Page Title")]
		public string PageTitle { get; set; }

		[Display(Name = "Body Top Script Tag")]
		public string BodyTopScriptTag { get; set; }

		[Display(Name = "Body Bottom Script Tag")]
		public string BodyBottomScriptTag { get; set; }

		public virtual ICollection<PageSection> Sections { get; set; }

	}
}