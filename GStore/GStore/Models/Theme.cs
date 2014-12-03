using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models
{
	[Table("Themes")]
	public class Theme : BaseClasses.AuditFieldsAllRequired
	{
		public int ThemeId { get; set; }
		
		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 1)]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		[Index(IsUnique = true)]
		[MaxLength(100)]
		public string FolderName { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public int Order { get; set; }

	}
}