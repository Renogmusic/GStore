using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("ClientRole")]
	public class ClientRole : BaseClasses.ClientRecord
	{
		[Key]
		public int ClientRoleId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(100)]
		public string Name { get; set; }

		public bool IsPublicUserRole { get; set; }

		[Required]
		public string Description { get; set; }

		public virtual ICollection<ClientRoleAction> ClientRoleActions { get; set; }
		public virtual ICollection<ClientUserRole> ClientUserRoles { get; set; }

	}
}

