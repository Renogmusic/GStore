using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models.BaseClasses
{
	public abstract class StoreFrontRecord: ClientRecord
	{
		public int StoreFrontId { get; set;}

		[ForeignKey("StoreFrontId")]
		public virtual StoreFront StoreFront { get; set; }

	}
}