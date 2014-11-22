using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	[Table("ClientRoleActions")]
	public class ClientRoleAction : BaseClasses.ClientLiveRecord
	{
		[Key]
		public int ClientRoleActionId { get; set; }

		[ForeignKey("ClientRoleId")]
		public virtual ClientRole ClientRole { get; set; }
		[Index("UniqueRecord", IsUnique = true, Order = 2)]
		public int ClientRoleId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public Identity.GStoreAction GStoreActionId { get; set; }

		[Required]
		public string GStoreActionName { get; set; }
	}
}
