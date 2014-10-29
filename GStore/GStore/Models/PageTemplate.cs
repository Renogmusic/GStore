using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	[Table("PageTemplates")]
	public class PageTemplate : BaseClasses.AuditFieldsAllRequired
	{
		public int PageTemplateId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public int Order { get; set; }

		[DataType(DataType.MultilineText)]
		[Required]
		public string Description { get; set; }

		[Required]
		public string ViewName { get; set; }

		[Required]
		public string LayoutName { get; set; }


		public ICollection<PageTemplateSection> Sections { get; set; }

	}
}