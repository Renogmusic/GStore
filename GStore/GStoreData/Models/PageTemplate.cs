﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("PageTemplate")]
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
		/// example: "Page Simple Welcome"  to point this template to "/Views/Page/Page Simple Welcome.cshtml"
		/// </summary>
		[Required]
		[Display(Name = "View Name (file name without .cshtml)", ShortName="View Name")]
		public string ViewName { get; set; }

		public virtual ICollection<PageTemplateSection> Sections { get; set; }

		public virtual ICollection<Page> Pages { get; set; }

	}
}