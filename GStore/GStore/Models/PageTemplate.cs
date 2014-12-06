using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	[Table("PageTemplates")]
	public class PageTemplate : BaseClasses.ClientRecord
	{
		[Key]
		[Display(Name="Page Template Id")]
		public int PageTemplateId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		public int Order { get; set; }

		[DataType(DataType.MultilineText)]
		[Required]
		public string Description { get; set; }

		/// <summary>
		/// Name of the file name View for MVC to display in the /Views/Page folder  without the .cshtml extension
		/// example: "Welcome"  to point this template to "/Views/Page/Welcome.cshtml"
		/// </summary>
		[Required]
		[Display(Name = "View Name (file name without .cshtml)", ShortName="View Name")]
		public string ViewName { get; set; }

		/// <summary>
		/// Always "Bootstrap" until we have more layout master pages
		/// </summary>
		[Required]
		[Display(Name="Layout Name (always 'Bootstrap')", ShortName="'Bootstrap'")]
		public string LayoutName { get; set; }

		public virtual ICollection<PageTemplateSection> Sections { get; set; }

	}
}