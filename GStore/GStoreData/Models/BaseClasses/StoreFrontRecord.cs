using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models.BaseClasses
{
	public abstract class StoreFrontRecord: ClientRecord
	{

		[ForeignKey("StoreFrontId")]
		[Display(Name = "Store Front", Description = "Store Front for this Store Front Record")]
		public virtual StoreFront StoreFront { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[Display(Name = "Store Front Id", Description="Store Front Id number for this Store Front Record")]
		public int StoreFrontId { get; set; }
	}
}