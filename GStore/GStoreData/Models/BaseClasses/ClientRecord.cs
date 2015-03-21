﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models.BaseClasses
{
	public abstract class ClientRecord : AuditFieldsAllRequired
	{
		[ForeignKey("ClientId")]
		[Display(Name = "Client", Description = "Client for this Client Record")]
		public virtual Client Client { get; set; }

		[Index("UniqueRecord", IsUnique = true, Order = 1)]
		[Display(Name = "Client Id", Description = "Client Id for this Client Record")]
		public int ClientId { get; set; }

	}
}