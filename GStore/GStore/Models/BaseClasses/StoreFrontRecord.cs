using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models.BaseClasses
{
	public abstract class StoreFrontRecord: ClientRecord
	{

		[ForeignKey("StoreFrontId")]
		[Display(Name = "Store Front")]
		public virtual StoreFront StoreFront { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		[Display(Name = "Store Front Id")]
		public int StoreFrontId { get; set; }
	}
}