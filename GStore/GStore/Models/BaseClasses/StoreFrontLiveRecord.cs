using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace GStore.Models.BaseClasses
{
	/// <summary>
	/// Records with start date/end date and a link to client table and store front table (storefrontid, clientid and nav props)
	/// </summary>
	public abstract class StoreFrontLiveRecord : ClientLiveRecord
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