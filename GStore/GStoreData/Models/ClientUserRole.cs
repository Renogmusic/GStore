using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models
{
	[Table("ClientUserRole")]
	public class ClientUserRole : BaseClasses.ClientRecord
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

