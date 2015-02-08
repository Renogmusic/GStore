using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Models
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

		[Display(Name = "Show If Empty")]
		public bool ShowIfEmpty { get; set; }

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

		[Required]
		[Display(Name = "Category Details Template", Description = "Template for Category details.")]
		public CategoryListTemplateEnum CategoryDetailTemplate { get; set; }

		[Required]
		[Display(Name = "Product List Template", Description = "Template for product list in the category page.")]
		public ProductListTemplateEnum ProductListTemplate { get; set; }

		[Required]
		[Display(Name = "Product Details Template", Description = "Template for product details.")]
		public ProductDetailTemplateEnum ProductDetailTemplate { get; set; }

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
		[Display(Name = "No Products Message Html", Description = "Message shown when there are no products in this category.")]
		public string NoProductsMessageHtml { get; set; }

		public virtual ICollection<Product> Products { get; set; }

	}

	public enum CategoryListTemplateEnum : int
	{
		Default = 0,
	}

	public enum ProductListTemplateEnum : int
	{
		Default = 0,
	}

	public enum ProductDetailTemplateEnum: int
	{
		Default = 0,
	}
}