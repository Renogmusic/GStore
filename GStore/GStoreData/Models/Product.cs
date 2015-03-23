using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("Product")]
	public class Product : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Id")]
		public int ProductId { get; set; }

		[Required]
		[MaxLength(250)]
		[Display(Name = "Name", Description="Product Name")]
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

		[Display(Name = "Available For Purchase", Description="Check this box if the product is available for purchase (add to cart or buy now). Uncheck this box if product cannot be purchased online.")]
		public bool AvailableForPurchase { get; set; }

		[Display(Name = "Show Request a Quote button", Description = "Check this box to show the Request A Quote button to link to the request a quote page.")]
		public bool RequestAQuote_Show { get; set; }

		[Display(Name = "Request a Quote button label", Description = "Enter text for the Request A Quote button.\nExample: 'Get a Quote'")]
		public string RequestAQuote_Label { get; set; }

		[Display(Name = "Request A Quote page", Description="Page to use for the 'Request A Quote' button on this product.")]
		public int? RequestAQuote_PageId { get; set; }

		[ForeignKey("RequestAQuote_PageId")]
		[Display(Name = "Request A Quote page", Description = "Page to use for the 'Request A Quote' button on this product.")]
		public virtual Page RequestAQuote_Page { get; set; }

		[Display(Name = "Meta Tag Description", Description = "META Description tag for search engines. Description tag to describe the product to search engines.\nLeave this blank to use the category description Meta Tag, or Store Front Description Meta tag (when category description meta tag is blank)")]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords", Description = "META Keywords tags for search engines. Keywords separated by a space for search engines.\nLeave this blank to use the Category Keywords Meta Tag, or Store Front Keywords Meta Tag (when category keywords meta tag is blank)")]
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
		[Range(0, 1000)]
		[Display(Name = "Max Quantity Per Order or 0 for no limit")]
		public int MaxQuantityPerOrder { get; set; }

		[Required]
		[Display(Name="Unit Price")]
		public decimal? BaseUnitPrice { get; set; }

		[Required]
		[Display(Name = "List Price")]
		public decimal? BaseListPrice { get; set; }

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

		[DataType(DataType.Url)]
		[Display(Name = "Top Link URL", Description = "Link URL for the top link that appears before the top description field. Leave this blank if there is no link.")]
		public string TopLinkHref { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Top Link Label", Description = "Link Label for the top link that appears before the top description field. Leave this blank if there is no link.")]
		public string TopLinkLabel { get; set; }

		[DataType(DataType.Url)]
		[Display(Name = "Bottom Link URL", Description = "Link URL for the bottom link that appears at the end of the product display. Leave this blank if there is no link.")]
		public string BottomLinkHref { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Bottom Link Label", Description = "Link Label for the bottom link that appears after the bottom description field. Leave this blank if there is no link.")]
		public string BottomLinkLabel { get; set; }

		//inventoryonhand
		//showifoutofstock
		//silentoutofstock
		//discontinued

		public virtual ICollection<ProductBundleItem> ProductBundleItems { get; set; }

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