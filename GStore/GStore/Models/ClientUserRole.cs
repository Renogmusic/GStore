﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models
{
	[Table("ClientUserRoles")]
	public class ClientUserRole : BaseClasses.ClientLiveRecord
	{
		[Key]
		public int ClientUserRoleId { get; set; }

		[ForeignKey("UserProfileId")]
		public virtual UserProfile UserProfile { get; set; }
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int UserProfileId { get; set; }

		[ForeignKey("ClientRoleId")]
		public virtual ClientRole ClientRole { get; set; }
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int ClientRoleId { get; set; }

		[ForeignKey("ScopeStoreFrontId")]
		public virtual StoreFront ScopeStoreFront { get; set;}
		[Index("UniqueRecord", IsUnique = true, Order = 5)]
		public int? ScopeStoreFrontId { get; set; }


	}
}
