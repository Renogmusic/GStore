using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	[Table("ProductCategories")]
	public class ProductCategory : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Category Id")]
		public int ProductCategoryId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
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

		[Display(Name = "Show a Divider before this item")]
		public bool UseDividerBeforeOnMenu { get; set; }

		[Display(Name = "Show a Divider after this item")]
		public bool UseDividerAfterOnMenu { get; set; }

		[Display(Name = "Direct Active Product Count")]
		public int DirectActiveCount { get; set; }

		[Display(Name = "Child Active Product Count")]
		public int ChildActiveCount { get; set; }

		public virtual ICollection<Product> Products { get; set; }

	}
}