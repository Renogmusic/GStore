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
		[Display(Name = "Name")]
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

		[Display(Name = "Index")]
		public int Order { get; set; }

		[Display(Name = "Image Name")]
		//[StoreFrontPath()]
		public string ImageName { get; set; }

		[Display(Name = "Digital Download")]
		public bool DigitalDownload { get; set; }

		[Required]
		[Display(Name = "Max Quantity Per Order or 0 for no limit")]
		public int MaxQuantityPerOrder { get; set; }

		[Display(Name = "Product Reviews")]
		public ICollection<ProductReview> ProductReviews { get; set; }

		[Required]
		[Display(Name="Unit Price")]
		public decimal BaseUnitPrice { get; set; }

		[Required]
		[Display(Name = "List Price")]
		public decimal BaseListPrice { get; set; }

		[Display(Name = "Theme Id", Description = "Theme for Product Details page, or blank to use the category or store catalog theme")]
		public int? ThemeId { get; set; }

		[Display(Name = "Theme", Description = "Theme for Product Details page, or blank to use the category or store catalog theme")]
		public virtual Theme Theme { get; set; }

		[Display(Name = "Product Details Template", Description = "Template for product details. Is set, this overrides the Details Template set on the category.")]
		public virtual ProductDetailTemplateEnum? ProductDetailTemplate { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Summary Caption", Description="Product Summary caption, or leave blank to use the Category default.\nExample: 'Summary' or 'Overview'")]
		public string SummaryCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Summary", Description = "Product Summary or leave blank for none.\nExample: high-level description of product features.")]
		public string SummaryHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Top Description Caption", Description = "Product Top Description caption, or leave blank to use the Category default.\nExample: 'Description' or 'Details'")]
		public string TopDescriptionCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Top Description")]
		public string TopDescriptionHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Bottom Description Caption", Description = "Product Bottom Description caption, or leave blank to use the Category default.\nExample: 'Description' or 'Details'")]
		public string BottomDescriptionCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Bottom Description")]
		public string BottomDescriptionHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Footer Html")]
		public string FooterHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Digital Download File")]
		public string DigitalDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Image File")]
		public string SampleImageFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Image Caption", Description = "Product Sample Image caption, or leave blank to use the Category default.\nExample: 'Sample Photo' or 'Sample'")]
		public string SampleImageCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Audio File")]
		public string SampleAudioFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Audio Caption", Description = "Product Sample Audio File caption, or leave blank to use the Category default.\nExample: 'Sample of song' or 'music'")]
		public string SampleAudioCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Download File")]
		public string SampleDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Download File Caption", Description = "Product Download File caption, or leave blank to use the Category default.\nExample: 'Demo output' or 'Example file'")]
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