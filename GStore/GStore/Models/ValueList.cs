using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("ValueList")]
	public class ValueList : BaseClasses.ClientRecord
	{
		[Key]
		[Display(Name = "Value List Id")]
		public int ValueListId { get; set; }
		
		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		[MaxLength(200)]
		public string Description { get; set; }

		[Required]
		public int Order { get; set; }

		[Display(Name = "Value List Items")]
		public virtual ICollection<ValueListItem> ValueListItems { get; set; }

		[Display(Name = "Web Form Fields")]
		public virtual ICollection<WebFormField> WebFormFields { get; set; }


	}
}