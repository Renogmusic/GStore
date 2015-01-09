using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("ProductReview")]
	public class ProductReview : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Review Id")]
		public int ProductReviewId { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Product Id")]
		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		[Display(Name = "Product")]
		public virtual Product Product { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		[Display(Name = "User Profile Id")]
		public int UserProfileId { get; set; }

		[Display(Name = "User Profile")]
		[ForeignKey("UserProfileId")]
		public virtual UserProfile UserProfile { get; set; }

		[Required]
		[Range(0D,5D)]
		public decimal StarRating { get; set; }

		[Required]
		[MaxLength(200)]
		public string Title { get; set; }

		[Required]
		[AllowHtml]
		[DataType(DataType.Html)]
		public string Body { get; set; }

	}
}