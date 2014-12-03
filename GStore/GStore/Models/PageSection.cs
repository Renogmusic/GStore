using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("PageSections")]
	public class PageSection : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Page Section Id")]
		public int PageSectionId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Page Id")]
		public int PageId { get; set; }

		[ForeignKey("PageId")]
		[Display(Name = "Page")]
		public virtual Page Page { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		[Display(Name = "Page Template Section Id")]
		public int PageTemplateSectionId { get; set; }

		[ForeignKey("PageTemplateSectionId")]
		[Display(Name = "Page Template Section")]
		public virtual PageTemplateSection PageTemplateSection { get; set; }

		public int Order { get; set; }

		[Display(Name = "Is Default")]
		public bool UseDefaultFromTemplate { get; set; }

		[Display(Name = "Is Blank")]
		public bool HasNothing { get; set; }

		[Display(Name = "Is Text")]
		public bool HasPlainText { get; set; }
		
		[System.Web.Mvc.AllowHtml]
		[UIHint("PlainText")]
		[Display(Name = "Plain text")]
		public string PlainText { get; set; }

		[Display(Name = "Is HTML")]
		public bool HasRawHtml { get; set; }
		
		[System.Web.Mvc.AllowHtml]
		[UIHint("RawHtml")]
		[Display(Name = "Raw Html")]
		public string RawHtml { get; set; }

	}
}