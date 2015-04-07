using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("ProductCategoryAltProductBundle")]
	public class ProductCategoryAltProductBundle : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Category Alt Product Bundle Id")]
		public int ProductCategoryAltProductBundleId { get; set; }

		[Display(Name = "Product Category Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int ProductCategoryId { get; set; }

		[ForeignKey("ProductCategoryId")]
		[Display(Name = "Product Category")]
		public virtual ProductCategory Category { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int ProductBundleId { get; set; }

		[ForeignKey("ProductBundleId")]
		[Display(Name = "ProductBundle")]
		public virtual ProductBundle ProductBundle { get; set; }

		[Display(Name = "Index")]
		public int Order { get; set; }
	}
}