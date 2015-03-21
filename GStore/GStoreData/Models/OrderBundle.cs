using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	[Table("OrderBundle")]
	public class OrderBundle : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Order Bundle Id")]
		public int OrderBundleId { get; set; }

		public int CartBundleId { get; set; }

		[ForeignKey("CartBundleId")]
		public virtual CartBundle CartBundle { get; set; }

		[Required]
		[Display(Name = "Order Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int OrderId { get; set; }

		[ForeignKey("OrderId")]
		public virtual Order Order { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int ProductBundleId { get; set; }

		[ForeignKey("ProductBundleId")]
		public virtual ProductBundle ProductBundle { get; set; }

		[Range(1, 999)]
		public int Quantity { get; set; }

		public virtual ICollection<OrderItem> OrderItems { get; set; }
	}
}