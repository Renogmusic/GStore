using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("ProductCategoryImage")]
	public class ProductCategoryImage : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Category Image Id")]
		public int ProductCategoryImageId { get; set; }

		public int ProductCategoryId { get; set; }

		[ForeignKey("ProductCategoryId")]
		public virtual ProductCategory ProductCategory { get; set; }

		[Required]
		public string ImageName { get; set; }

		[Required]
		public int Order { get; set; }

	}
}