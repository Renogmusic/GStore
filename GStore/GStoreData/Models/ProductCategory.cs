using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("ProductCategory")]
	public class ProductCategory : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Category Id")]
		public int ProductCategoryId { get; set; }

		[Required]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
		[MaxLength(100)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Url Name")]
		public string UrlName { get; set; }

		[Display(Name = "Image Name")]
		public string ImageName { get; set; }

		public int Order { get; set; }

		[Display(Name = "Parent Category Id")]
		public int? ParentCategoryId { get; set; }

		[ForeignKey("ParentCategoryId")]
		[Display(Name = "Product Category")]
		public virtual ProductCategory ParentCategory { get; set; }

		[Display(Name = "Allow child Categories in Menu")]
		public bool AllowChildCategoriesInMenu { get; set; }

		[Display(Name = "Show in Menu")]
		public bool ShowInMenu { get; set; }

		[Display(Name = "Hide in Menu If Empty")]
		public bool HideInMenuIfEmpty { get; set; }

		[Display(Name = "Always Display Details for Direct Links")]
		public bool DisplayForDirectLinks { get; set; }

		[Display(Name = "Show in Catalog If Empty")]
		public bool ShowInCatalogIfEmpty { get; set; }

		[Display(Name = "For Registered Users Only")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Users Only")]
		public bool ForAnonymousOnly { get; set; }

		[Display(Name = "Show a Divider before this item")]
		public bool UseDividerBeforeOnMenu { get; set; }

		[Display(Name = "Show a Divider after this item")]
		public bool UseDividerAfterOnMenu { get; set; }

		[Display(Name = "Direct Active Product Count")]
		public int DirectActiveCount { get; set; }

		[Display(Name = "Child Active Product Count")]
		public int ChildActiveCount { get; set; }

		[Display(Name = "Theme Id", Description = "Theme for Category Details page and products that do not have a theme defined. Leave blank to use the store catalog theme")]
		public int? ThemeId { get; set; }

		[Display(Name = "Theme", Description = "Theme for Category Details page and products that do not have a theme defined. Leave blank to use the store catalog theme")]
		public virtual Theme Theme { get; set; }

		/// <summary>
		/// Category list template for sub-categories of this category
		/// </summary>
		[Required]
		[Display(Name = "Category Details Template", Description = "Template for Category details.")]
		public CategoryListTemplateEnum CategoryDetailTemplate { get; set; }

		/// <summary>
		/// Product list template for products in this category
		/// </summary>
		[Required]
		[Display(Name = "Product List Template", Description = "Template for product list in the category page.")]
		public ProductListTemplateEnum ProductListTemplate { get; set; }

		/// <summary>
		/// Bundle list template for bundles in this category
		/// </summary>
		[Required]
		[Display(Name = "Product List Template", Description = "Template for product list in the category page.")]
		public ProductBundleListTemplateEnum BundleListTemplate { get; set; }


		[Required]
		[Display(Name = "Product Details Template", Description = "Template for product details. Template can also be set per product, this sets the default.")]
		public ProductDetailTemplateEnum ProductDetailTemplate { get; set; }

		[Required]
		[Display(Name = "Product Bundle Details Template", Description = "Template for product bundle details. Template can also be set per bundle, this sets the default.")]
		public ProductBundleDetailTemplateEnum ProductBundleDetailTemplate { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Child Category Header Html", Description = "Header HTML shown before child categories.")]
		public string ChildCategoryHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Child Category Footer Html", Description = "Footer HTML shown after child categories.")]
		public string ChildCategoryFooterHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Product Header Html", Description = "Header HTML shown before products in this category.")]
		public string ProductHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Product Footer Html", Description = "Footer HTML shown after products in this category.")]
		public string ProductFooterHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Product Bundle Header Html", Description = "Header HTML shown before product bundles in this category.")]
		public string ProductBundleHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Product Bundle Footer Html", Description = "Footer HTML shown after product bundles in this category.")]
		public string ProductBundleFooterHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "No Products Message Html", Description = "Message shown when there are no products in this category.")]
		public string NoProductsMessageHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Summary Caption", Description = "Default Summary caption for products that do not have one defined.\nLeave this blank to use the system default 'Summary'\nExample: 'Summary' or 'Overview'")]
		public string DefaultSummaryCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Top Description Caption", Description = "Default Top Description caption for products that do not have one defined.\nLeave this field blank to use the system default 'Description for [product name]'.\nExample: 'Description' or 'Details'")]
		public string DefaultTopDescriptionCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Bottom Description Caption", Description = "Default Top Description caption for products that do not have one defined.\nLeave this field blank to use the system default 'Details for [product name]'.\nExample: 'Description' or 'Details'")]
		public string DefaultBottomDescriptionCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Sample Image Caption", Description = "Default Sample Image caption for products that do not have one defined.\nLeave this field blank to use the system default 'Sample Image for [product name]'.\nExample: 'Sample Image' or 'Photo'")]
		public string DefaultSampleImageCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Sample Audio Caption", Description = "Default Sample Audio caption for products that do not have one defined.\nLeave this field blank to use the system default 'Sample Audio for [product name]'.\nExample: 'Sample Sound' or 'Music'")]
		public string DefaultSampleAudioCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Sample Download File Caption", Description = "Default Sample Download File caption for products that do not have one defined.\nLeave this field blank to use the system default 'Sample Download for [product name]'.\nExample: 'Sample File' or 'Demo File'")]
		public string DefaultSampleDownloadCaption { get; set; }

		[Display(Name = "Meta Tag Description", Description = "META Description tag for search engines. Description tag to describe the Category to search engines.\nLeave this blank to use the Store Front Description Meta Tag. \nThis is also used for products that do not have a Description Meta Tag.")]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords", Description = "META Keywords tags for search engines. Keywords separated by a space for search engines.\nLeave this blank to use the Store Front Keywords Meta Tag. \nThis is also used for products that do not have a Keywords Meta Tag defined.")]
		public string MetaKeywords { get; set; }

		public virtual ICollection<Product> Products { get; set; }

		public virtual ICollection<ProductBundle> ProductBundles { get; set; }

	}


	/// <summary>
	/// Category list template
	/// </summary>
	public enum CategoryListTemplateEnum : int
	{
		Default = 0,
		Music = 100
	}

	/// <summary>
	/// Product list template for products under a category
	/// </summary>
	public enum ProductListTemplateEnum : int
	{
		Default = 0,
		Music = 100
	}

	/// <summary>
	/// Bundle list template for bundles under a category
	/// </summary>
	public enum ProductBundleListTemplateEnum : int
	{
		Default = 0,
		Music = 100
	}



	/// <summary>
	/// Product detail template for products in a category
	/// </summary>
	public enum ProductDetailTemplateEnum: int
	{
		Default = 0,
		Music = 100
	}

	/// <summary>
	/// Product bundle detail template for bundles in a category
	/// </summary>
	public enum ProductBundleDetailTemplateEnum : int
	{
		Default = 0,
		Music = 100
	}


}