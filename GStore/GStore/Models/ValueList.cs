using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("ValueLists")]
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
		public string Description { get; set; }

		[Required]
		public int Order { get; set; }

		[Display(Name = "Editable")]
		public bool AllowEdit { get; set; }

		[Display(Name = "Removable")]
		public bool AllowDelete { get; set; }

		[Display(Name="Multiple Selections")]
		public bool IsMultiSelect { get; set; }

		[Display(Name = "Value List Items")]
		public virtual ICollection<ValueListItem> ValueListItems { get; set; }

		[Display(Name = "Web Form Fields")]
		public virtual ICollection<WebFormField> WebFormFields { get; set; }


	}
}