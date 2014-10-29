using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("Products")]
	public class Product : BaseClasses.ClientLiveRecord
	{
		public int ProductId { get; set; }

		[Required]
		public string Name { get; set; }

		public string MetaDescription { get; set; }

		public string MetaKeywords { get; set; }

	}
}