using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("ProductImage")]
	public class ProductImage : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Image Id")]
		public int ProductImageId { get; set; }

		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }

		[Required]
		public string ImageName { get; set; }

		[Required]
		public int Order { get; set; }

	}
}