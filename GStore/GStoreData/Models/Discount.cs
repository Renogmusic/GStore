using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	[Table("Discount")]
	public class Discount : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Discount Id")]
		public int DiscountId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(50)]
		[Display(Name = "Code")]
		public string Code { get; set; }

		[Required]
		[Display(Name = "Order")]
		public int Order { get; set; }

		[Required]
		[Display(Name="Max Uses", Description="Maximum number of times this discount code can be used or 0 for no limit")]
		public int MaxUses { get; set; }

		[Required]
		[Display(Name = "Times Used")]
		public int UseCount { get; set; }

		[Required]
		[Display(Name = "Min Subtotal to Qualify")]
		public decimal MinSubtotal { get; set; }

		/// <summary>
		/// Number of percent off, whole numbers so 50 = 50% off, 0 for no % off (discount may have other benefits)
		/// </summary>
		[Required]
		[Range(0, 99)]
		[Display(Name = "Percent Off Total")]
		public decimal PercentOff { get; set; }

		/// <summary>
		/// Flat discount
		/// </summary>
		[Required]
		[Range(0, 10000)]
		[Display(Name = "Flat Discount")]
		public decimal FlatDiscount { get; set; }

		[Display(Name = "Free Shipping")]
		public bool FreeShipping { get; set; }

		[Display(Name = "Free Product")]
		public Product FreeProduct { get; set; }

	}
}