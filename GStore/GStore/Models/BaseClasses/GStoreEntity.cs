using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Data;
using System.ComponentModel.DataAnnotations;

namespace GStore.Models.BaseClasses
{
	public abstract class GStoreEntity
	{
		[Display(Name = "Is Pending")]
		public bool IsPending { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		[Display(Name = "Start Date")]
		public DateTime StartDateTimeUtc { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		[Display(Name = "End Date")]
		public DateTime EndDateTimeUtc { get; set; }

	}
}