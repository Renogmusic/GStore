using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("ProductAttribute")]
	public class ProductAttribute : BaseClasses.StoreFrontDataValueRecord
	{
		[Key]
		[Display(Name = "Product Attribute Id")]
		public int ProductAttributeId { get; set; }

		[Display(Name = "Product Id")]
		public int ProductId { get; set; }

		[ForeignKey("ProductId")]
		[Display(Name = "Product")]
		public virtual Product Product { get; set; }

		[Display(Name = "Product Category Attribute Id")]
		public int ProductCategoryAttributeId { get; set; }

		[ForeignKey("ProductCategoryAttributeId")]
		[Display(Name = "Product Category Attribute")]
		public virtual ProductCategoryAttribute ProductCategoryAttribute { get; set; }


	}
}