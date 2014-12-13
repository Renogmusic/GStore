using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("WebFormFieldResponses")]
	public class WebFormFieldResponse : BaseClasses.StoreFrontDataValueRecord
	{
		[Key]
		[Display(Name = "Web Form Field Response Id")]
		public int WebFormFieldResponseId { get; set; }

		[Required]
		[Display(Name = "Web Form Response Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int WebFormResponseId { get; set; }

		[ForeignKey("WebFormResponseId")]
		[Display(Name = "Web Form Response")]
		public virtual WebFormResponse WebFormResponse { get; set; }

		[Required]
		[Display(Name = "Web Form Field Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 5)]
		public int WebFormFieldId { get; set; }

		[ForeignKey("WebFormFieldId")]
		[Display(Name = "Web Form Field")]
		public virtual WebFormField WebFormField { get; set; }

	}
}