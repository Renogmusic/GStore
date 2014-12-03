using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("ValueListItems")]
	public class ValueListItem : BaseClasses.ClientRecord
	{
		[Key]
		[Display(Name = "Value List Item Id")]
		public int ValueListItemId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[Display(Name = "Value List Id")]
		public int ValueListId { get; set; }

		[ForeignKey("ValueListId")]
		[Display(Name = "Value List")]
		public virtual ValueList ValueList { get; set; }
		
		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(200)]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public int Order { get; set; }

		[Display(Name = "Is Integer")]
		public bool IsInteger { get; set; }

		[Display(Name = "Integer Value")]
		public int? IntegerValue { get; set; }

		[Display(Name = "Is String")]
		public bool IsString { get; set; }

		[Display(Name = "String Value")]
		public string StringValue { get; set; }

	}
}