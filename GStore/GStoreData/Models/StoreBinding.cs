using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("StoreBinding")]
	public class StoreBinding : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Store Binding Id", Description="Internal Store Binding Id number")]
		public int StoreBindingId { get; set; }

		[Required]
		[MaxLength(250)]
		[Display(Name = "Host Name", Description = "Host Name or * for all. Default: *")]
		public string HostName { get; set; }

		[Display(Name = "Port", Description = "Http Port Number or 0 for All. \nDefault: 80")]
		public int? Port { get; set; }

		[Required]
		[MaxLength(250)]
		[Display(Name = "Root Path", Description="Root Path of application (Request.ApplicationPath) or use * for Catch-All. \nDefault: /")]
		public string RootPath { get; set; }

		[Display(Name = "Use Url Store Name", ShortName="Use Url Store", Description="Use Url Store Name for this binding.\nUsed when Setting AppEnableStoresVirtualFolders is true.\nDefault: unchecked")]
		public bool UseUrlStoreName { get; set; }

		[Display(Name = "Url Store Name", ShortName = "Url Store", Description = "Store name when using Url Store Name is set.\nUsed when Setting AppEnableStoresVirtualFolders is true.\nuse * for Catch-All\nDefault: blank")]
		public string UrlStoreName { get; set; }

		[Display(Name = "Index", ShortName = "Index", Description = "Order index for this binding. Lower numbers are higher priority.\nDefault: 100")]
		public int Order { get; set; }
	}
}