using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	[Table("ProductCategories")]
	public class ProductCategory : BaseClasses.StoreFrontLiveRecord
	{
		public int ProductCategoryId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
		public string UrlName { get; set; }

		public string ImageName { get; set; }

		public int Order { get; set; }

		public int? ParentCategoryId { get; set; }

		[ForeignKey("ParentCategoryId")]
		public virtual ProductCategory ParentCategory { get; set; }

		public bool AllowChildCategoriesInMenu { get; set; }
		public bool ShowInMenu { get; set; }

		public bool ShowIfEmpty { get; set; }

		public bool UseDividerBeforeOnMenu { get; set; }
		public bool UseDividerAfterOnMenu { get; set; }

		public int DirectActiveCount { get; set; }

		public int ChildActiveCount { get; set; }

		public virtual ICollection<Product> Products { get; set; }

	}
}