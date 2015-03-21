using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	[Table("GiftCard")]
	public class GiftCard : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Gift Card Id")]
		public int GiftCardId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(50)]
		[Display(Name = "Code")]
		public string Code { get; set; } 

		[Required]
		[Display(Name = "Order")]
		public int Order { get; set; }

		[Required]
		[Display(Name = "Balance")]
		public decimal Balance { get; set; }

		[Required]
		[Display(Name = "Times Used")]
		public int UseCount { get; set; }

		[Required]
		[Display(Name = "Last Used Date Time Utc")]
		public DateTime? LastUsedDateTimeUtc { get; set; }

	}
}