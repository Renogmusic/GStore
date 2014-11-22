using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("Products")]
	public class Product : BaseClasses.StoreFrontLiveRecord
	{
		public int ProductId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
		public string UrlName { get; set; }

		public string MetaDescription { get; set; }

		public string MetaKeywords { get; set; }

		public int ProductCategoryId { get; set; }

		[ForeignKey("ProductCategoryId")]
		public virtual ProductCategory Category { get; set; }

		public int Order { get; set; }

		public string ImageName { get; set; }

		//inventoryonhand
		//showifoutofstock
		//silentoutofstock
		//discontinued

	}
}