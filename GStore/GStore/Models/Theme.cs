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
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public string FolderName { get; set; }

		[Required]
		public int Order { get; set; }

		public virtual ICollection<StoreFront> StoreFronts { get; set; }

	}
}