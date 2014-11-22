using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models.BaseClasses
{
	public abstract class ClientRecord : AuditFieldsAllRequired
	{
		[ForeignKey("ClientId")]
		public virtual Client Client { get; set; }
		[Index("UniqueRecord", IsUnique = true, Order = 1)]
		public int ClientId { get; set; }

	}
}