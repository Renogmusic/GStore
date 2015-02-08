using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("Product")]
	public class Product : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Id")]
		public int ProductId { get; set; }

		[Required]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
		[MaxLength(100)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Url Name")]
		public string UrlName { get; set; }

		[Display(Name = "For Registered Only")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Only")]
		public bool ForAnonymousOnly { get; set; }

		[Display(Name = "Meta Tag Description")]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords")]
		public string MetaKeywords { get; set; }

		[Display(Name = "Product Category Id")]
		public int ProductCategoryId { get; set; }

		[ForeignKey("ProductCategoryId")]
		[Display(Name = "Product Category")]
		public virtual ProductCategory Category { get; set; }

		public int Order { get; set; }

		[Display(Name = "Image Name")]
		public string ImageName { get; set; }

		[Display(Name = "Digital Download")]
		public bool DigitalDownload { get; set; }

		[Required]
		[Display(Name = "Max Quantity Per Order or 0 for no limit")]
		public int MaxQuantityPerOrder { get; set; }

		[Display(Name = "Product Reviews")]
		public ICollection<ProductReview> ProductReviews { get; set; }

		[Required]
		public decimal BaseUnitPrice { get; set; }

		[Required]
		public decimal BaseListPrice { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		public string SummaryHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		public string DescriptionHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		public string FooterHtml { get; set; }

		[DataType(DataType.Text)]
		public string DigitalDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleImageFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleImageCaption { get; set; }

		[DataType(DataType.Text)]
		public string SampleAudioFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleAudioCaption { get; set; }

		[DataType(DataType.Text)]
		public string SampleDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleDownloadCaption { get; set; }


		//inventoryonhand
		//showifoutofstock
		//silentoutofstock
		//discontinued

		public decimal UnitPrice(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			return this.BaseUnitPrice;
		}

		public decimal ListPrice(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			return this.BaseListPrice;
		}

		public decimal UnitPriceExt(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
			}
			return quantity * UnitPrice(quantity);
		}

		public decimal ListPriceExt(int quantity)
		{
			if (quantity < 1 || quantity > 10000)
			{
				throw new ArgumentOutOfRangeException("quantity", "quantity must be 1 to 10,000");
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