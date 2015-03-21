
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GStoreData.Models.BaseClasses;

namespace GStoreData.Models
{
	[Table("WebFormField")]
	public class WebFormField : BaseClasses.ClientRecord
	{
		[Key]
		[Display(Name = "Web Form Field Id")]
		public int WebFormFieldId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[Display(Name="Web Form Id")]
		public int WebFormId { get; set; }

		[ForeignKey("WebFormId")]
		[Display(Name = "Web Form")]
		public virtual WebForm WebForm { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(100)]
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Order")]
		public int Order { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Label Text")]
		public string LabelText { get; set; }

		[MaxLength(100)]
		[Display(Name = "Watermark Text")]
		public string Watermark { get; set; }

		[MaxLength(100)]
		[Display(Name = "Help Label Top Text")]
		public string HelpLabelTopText { get; set; }

		[MaxLength(100)]
		[Display(Name = "Help Label Bottom Text")]
		public string HelpLabelBottomText { get; set; }

		[Required]
		[MaxLength(200)]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Display(Name = "Is Required")]
		public bool IsRequired { get; set; }

		[Required]
		[Display(Name = "Data Type")]
		public GStoreValueDataType DataType { get; set; }

		[MaxLength(50)]
		[Display(Name = "Data Type String")]
		public string DataTypeString { get; set; }

		[Display(Name = "Value List Id")]
		public int? ValueListId { get; set; }

		[ForeignKey("ValueListId")]
		[Display(Name = "Value List")]
		public virtual ValueList ValueList { get; set; }

		[Display(Name = "Value List Null Text")]
		public string ValueListNullText { get; set; }

		[Display(Name = "Text Area Rows")]
		public int? TextAreaRows { get; set; }

		[Display(Name = "Text Area Columns")]
		public int? TextAreaColumns { get; set; }

		[Display(Name = "Web Form Field Responses")]
		public virtual ICollection<WebFormFieldResponse> WebFormFieldResponses { get; set; }
	}
}