using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models.BaseClasses
{
	public abstract class StoreFrontRecordUserProfileOptional : ClientRecordUserProfileOptional
	{

		[ForeignKey("StoreFrontId")]
		[Display(Name = "Store Front")]
		public virtual StoreFront StoreFront { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[Display(Name = "Store Front Id")]
		public int StoreFrontId { get; set; }
	}
}