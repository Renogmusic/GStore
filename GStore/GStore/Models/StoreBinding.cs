using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("StoreBindings")]
	public class StoreBinding : BaseClasses.StoreFrontLiveRecord
	{
		[Key]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int StoreBindingId { get; set; }

		[Required]
		[MaxLength(250)]
		public string HostName { get; set; }
		public int? Port { get; set; }

		[Required]
		[MaxLength(250)]
		public string RootPath { get; set; }

		public int Order { get; set; }
	}
}