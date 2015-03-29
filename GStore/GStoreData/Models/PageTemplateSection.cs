using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("PageTemplateSection")]
	public class PageTemplateSection : BaseClasses.ClientRecord
	{
		[Key]
		[Display(Name = "Page Template Section Id")]
		public int PageTemplateSectionId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[Display(Name = "Page Template Id")]
		public int PageTemplateId { get; set; }

		[ForeignKey("PageTemplateId")]
		[Display(Name = "Page Template")]
		public virtual PageTemplate PageTemplate { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(100)]
		public string Name { get; set; }

		public int Order { get; set; }

		[DataType(DataType.MultilineText)]
		[Required]
		public string Description { get; set; }

		[Display(Name="Default Raw Html Value")]
		public string DefaultRawHtmlValue { get; set; }

		[Display(Name = "Is Variable", Description = "Flags this field as a variable")]
		public bool IsVariable { get; set; }

		[Display(Name = "Default String Value", Description="Default string value for variable fields")]
		public string DefaultStringValue { get; set; }

		[Display(Name = "Pre-Text Html")]
		public string PreTextHtml { get; set; }

		[Display(Name = "Post-Text Html")]
		public string PostTextHtml { get; set; }

		[Display(Name = "Text Css Class")]
		public string DefaultTextCssClass { get; set; }

		[Display(Name = "Edit in Page Top Editor")]
		public bool EditInTop { get; set; }

		[Display(Name = "Edit in Page Bottom Editor")]
		public bool EditInBottom { get; set; }

		[Display(Name="Page Sections")]
		public virtual ICollection<PageSection> PageSections { get; set; }
	}
}