using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("StoreBindings")]
	public class StoreBinding : BaseClasses.StoreFrontLiveRecord
	{
		public int StoreBindingId { get; set; }

		[Required]
		public string HostName { get; set; }
		public int? Port { get; set; }

		[Required]
		public string RootPath { get; set; }

		public int Order { get; set; }
	}
}