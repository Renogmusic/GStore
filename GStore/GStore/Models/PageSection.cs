using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("PageSections")]
	public class PageSection : BaseClasses.StoreFrontLiveRecord
	{
		[Editable(false)]
		public int PageSectionId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int PageId { get; set; }
		[ForeignKey("PageId")]
		public virtual Page Page { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int PageTemplateSectionId { get; set; }
		[ForeignKey("PageTemplateSectionId")]
		public virtual PageTemplateSection PageTemplateSection { get; set; }

		public int Order { get; set; }

		public bool HasNothing { get; set; }

		public bool HasPlainText { get; set; }
		
		[System.Web.Mvc.AllowHtml]
		[UIHint("PlainText")]
		public string PlainText { get; set; }

		public bool HasRawHtml { get; set; }
		
		[System.Web.Mvc.AllowHtml]
		[UIHint("RawHtml")]
		public string RawHtml { get; set; }

	}
}