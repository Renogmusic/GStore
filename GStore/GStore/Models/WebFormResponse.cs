using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("WebFormResponse")]
	public class WebFormResponse : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Display(Name="Web Form Response Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int WebFormResponseId { get; set; }

		[Display(Name = "Web Form Id")]
		public int WebFormId { get; set; }

		[Display(Name = "Web Form")]
		public virtual WebForm WebForm { get; set; }

		[Display(Name = "Page Id")]
		public int? PageId { get; set; }

		[Display(Name = "Page")]
		public Page Page { get; set; }

		public string BodyText { get; set; }

		public string Subject { get; set; }

		[Display(Name = "Web Form Fields")]
		public virtual ICollection<WebFormField> WebFormFields { get; set; }

		[Display(Name = "Web Form Field Responses")]
		public virtual ICollection<WebFormFieldResponse> WebFormFieldResponses { get; set; }

	}
}