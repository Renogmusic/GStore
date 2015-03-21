using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("WebForm")]
	public class WebForm : BaseClasses.ClientRecord
	{
		[Key]
		[Display(Name="Web Form Id")]
		public int WebFormId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[MaxLength(100)]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Order")]
		public int Order { get; set; }

		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[MaxLength(100)]
		[Display(Name = "Title")]
		public string Title { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Form Header Html")]
		public string FormHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Form Footer Before Submit Button Html")]
		public string FormFooterBeforeSubmitHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Form Footer After Submit Button Html")]
		public string FormFooterAfterSubmitHtml { get; set; }

		[Required]
		[MaxLength(20)]
		[Display(Name = "Submit Button Text")]
		public string SubmitButtonText { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "Submit Button Class")]
		public string SubmitButtonClass { get; set; }

		[Required]
		[MaxLength(25)]
		[Display(Name = "Web Form Display Template Name", Description="Always 'WebForm'")]
		public string DisplayTemplateName { get; set; }

		[Required]
		[Display(Name = "Label column span on Medium Display and up")]
		public int LabelMdColSpan { get; set; }

		[Required]
		[Display(Name = "Field column span on Medium Display and up")]
		public int FieldMdColSpan { get; set; }

		[Display(Name = "Web Form Fields")]
		public virtual ICollection<WebFormField> WebFormFields { get; set; }

		[Display(Name = "Web Form Responses")]
		public virtual ICollection<WebFormResponse> WebFormResponses { get; set; }

		[Display(Name = "Pages")]
		public virtual ICollection<Page> Pages { get; set; }

	}
}