using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("ProductCategoryAttributes")]
	public class ProductCategoryAttribute : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Product Category Attribute Id")]
		public int ProductCategoryAttributeId { get; set; }

		[ForeignKey("Product Category")]
		public virtual ProductCategory ProductCategory { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Product Category Id")]
		public int ProductCategoryId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		[MaxLength(250)]
		public string Name { get; set; }

		public int Order { get; set; }

		[Display(Name = "Icon Image Name")]
		public string IconImageName { get; set; }

		[Display(Name = "Data Type Id")]
		public GStore.Data.GStoreAttributeDataType DataType { get; set; }

		[Display(Name = "Data Type")]
		public string DataTypeText { get; set; }

		[ForeignKey("ValueListId")]
		[Display(Name = "Value List")]
		public virtual ValueList ValueList { get; set; }

		[Display(Name = "Value List Id")]
		public int? ValueListId { get; set; }



		[Display(Name = "Include in Search")]
		public bool IncludeInSearch { get; set; }

		[Display(Name = "Include in Top filters")]
		public bool IncludeInTopFilters { get; set; }

		[Display(Name = "Include in Side filters")]
		public bool IncludeInSideFilters { get; set; }

	}
}