using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("ProductCategoryAltProduct")]
	public class ProductCategoryAltProduct : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Category Alt Product Id")]
		public int ProductCategoryAltProductId { get; set; }

		[Display(Name = "Product Category Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int ProductCategoryId { get; set; }

		[ForeignKey("ProductCategoryId")]
		[Display(Name = "Product Category")]
		public virtual ProductCategory Category { get; set; }

		[Index]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		[Display(Name = "Product")]
		public virtual Product Product { get; set; }

		[Display(Name = "Index")]
		public int Order { get; set; }
	}
}