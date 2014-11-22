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

		[Index("UniqueRecord", IsUnique = true, Order = 1)]
		public int PageTemplateId { get; set; }
		[ForeignKey("PageTemplateId")]
		public virtual PageTemplate PageTemplate { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[MaxLength(100)]
		public string Name { get; set; }

		public int Order { get; set; }

		[DataType(DataType.MultilineText)]
		[Required]
		public string Description { get; set; }

		public bool IsRequired { get; set; }

		public virtual ICollection<PageSection> PageSections { get; set; }
	}
}