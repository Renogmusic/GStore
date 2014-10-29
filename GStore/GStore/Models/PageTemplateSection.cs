using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	[Table("PageTemplateSections")]
	public class PageTemplateSection : BaseClasses.AuditFieldsAllRequired
	{
		public int PageTemplateSectionId { get; set; }

		public int PageTemplateId { get; set; }
		[ForeignKey("PageTemplateId")]
		public virtual PageTemplate PageTemplate { get; set; }

		[Required]
		public string Name { get; set; }

		public int Order { get; set; }

		[DataType(DataType.MultilineText)]
		[Required]
		public string Description { get; set; }

		public virtual ICollection<PageSection> PageSections { get; set; }
	}
}