using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("ProductBundleItem")]
	public class ProductBundleItem : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Bundle Item Id", Description="Internal identifier for Product Bundle")]
		public int ProductBundleItemId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Bundle", Description = "Internal identifier for Product Bundle")]
		public int ProductBundleId { get; set; }

		[ForeignKey("ProductBundleId")]
		[Display(Name = "Bundle", Description = "Product Bundle")]
		public virtual ProductBundle ProductBundle { get; set; }

		[Display(Name = "Index", Description = "Index number to sort this item in the bundle. Lower numbers sort first.")]
		public int Order { get; set; }

		[Required]
		[Range(1, 1000)]
		[Display(Name = "Quantity", Description = "Number of items in each bundle.\nNote: the customer may select multiple quanties of the bundle up to Max Quantity.")]
		public int Quantity { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		[Display(Name = "Product Id", Description = "Internal unique identifier to link a product to this bundle.")]
		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		[Display(Name = "Product in Bundle", Description = "Product in this bundle")]
		public virtual Product Product { get; set; }

		[Display(Name = "Unit Price", Description = "Price each for this item in the bundle.\nNote: if quantity is 2 or more per bundle, the system will multiply this price by the quantity to determine unit price for the bundle.")]
		public decimal? BaseUnitPrice { get; set; }

		[Display(Name = "List Price", Description = "List Price each for this item in the bundle.\nNote: if quantity is 2 or more per bundle, the system will multiply this price by the quantity to determine list price for the bundle.")]
		public decimal? BaseListPrice { get; set; }

		public virtual ICollection<CartItem> CartItems { get; set; }

		[Display(Name = "Type", Description = "Product Variant Info for this product. Used to track unique colors, options, and other details about a product.")]
		public string ProductVariantInfo { get; set; }

		public decimal? UnitPrice(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			return this.BaseUnitPrice;
		}

		public decimal? ListPrice(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			return this.BaseListPrice;
		}

		public decimal? UnitPriceExt(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			if (this.BaseUnitPrice == null)
			{
				return null;
			}
			return quantity * UnitPrice(quantity);
		}

		public decimal? ListPriceExt(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			if (this.BaseListPrice == null)
			{
				return null;
			}
			return quantity * ListPrice(quantity);
		}

		public decimal LineDiscount(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			return 0M;
		}

	}

}