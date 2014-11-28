using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("StoreBindings")]
	public class StoreBinding : BaseClasses.StoreFrontLiveRecord
	{
		[Key]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Store Binding Id")]
		public int StoreBindingId { get; set; }

		[Required]
		[MaxLength(250)]
		[Display(Name = "Host Name")]
		public string HostName { get; set; }

		public int? Port { get; set; }

		[Required]
		[MaxLength(250)]
		[Display(Name = "Root Path")]
		public string RootPath { get; set; }

		[Display(Name = "Use Url Store Name")]
		public bool UseUrlStoreName { get; set; }

		[Display(Name = "Url Store Name")]
		public string UrlStoreName { get; set; }

		public int Order { get; set; }
	}
}