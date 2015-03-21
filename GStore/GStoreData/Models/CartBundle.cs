using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	[Table("CartBundle")]
	public class CartBundle : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Cart Bundle Id")]
		public int CartBundleId { get; set; }

		[Required]
		[Display(Name = "Cart Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int CartId { get; set; }

		[ForeignKey("CartId")]
		public virtual Cart Cart { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int ProductBundleId { get; set; }

		[ForeignKey("ProductBundleId")]
		public virtual ProductBundle ProductBundle { get; set; }

		public virtual ICollection<CartItem> CartItems { get; set; }

		[Range(1, 999)]
		public int Quantity { get; set; }

	}
}