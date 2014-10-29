using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("PageSections")]
	public class PageSection : BaseClasses.ClientLiveRecord
	{
		[Editable(false)]
		public int PageSectionId { get; set; }

		public int PageId { get; set; }
		[ForeignKey("PageId")]
		public virtual Page Page { get; set; }

		public int PageTemplateSectionId { get; set; }
		[ForeignKey("PageTemplateSectionId")]
		public virtual PageTemplateSection PageTemplateSection { get; set; }

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