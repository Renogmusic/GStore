using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	[Table("CartItem")]
	public class CartItem : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Cart Item Id")]
		public int CartItemId { get; set; }

		[Required]
		[Display(Name = "Cart Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int CartId { get; set; }

		[ForeignKey("CartId")]
		public virtual Cart Cart { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 5)]
		public int? CartBundleId { get; set; }

		[ForeignKey("CartBundleId")]
		public virtual CartBundle CartBundle { get; set; }

		public int? ProductBundleItemId { get; set; }

		[ForeignKey("ProductBundleItemId")]
		public virtual ProductBundleItem ProductBundleItem { get; set; }

		[Range(1, 999)]
		public int Quantity { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		public decimal UnitPrice { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		public decimal ListPrice { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		public decimal UnitPriceExt { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		public decimal ListPriceExt { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		public decimal LineDiscount { get; set; }

		[Required]
		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		public decimal LineTotal { get; set; }

		public string ProductVariantInfo { get; set; }

	}
}