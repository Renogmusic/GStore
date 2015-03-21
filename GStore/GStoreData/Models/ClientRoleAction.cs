﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("ClientRoleAction")]
	public class ClientRoleAction : BaseClasses.ClientRecord
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
