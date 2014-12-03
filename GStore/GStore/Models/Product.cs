using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("Products")]
	public class Product : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Id")]
		public int ProductId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Url Name")]
		public string UrlName { get; set; }

		[Display(Name = "Meta Tag Description")]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords")]
		public string MetaKeywords { get; set; }

		[Display(Name = "Product Category Id")]
		public int ProductCategoryId { get; set; }

		[ForeignKey("ProductCategoryId")]
		[Display(Name = "Product Category")]
		public virtual ProductCategory Category { get; set; }

		public int Order { get; set; }

		[Display(Name = "Image Name")]
		public string ImageName { get; set; }

		//inventoryonhand
		//showifoutofstock
		//silentoutofstock
		//discontinued

	}
}