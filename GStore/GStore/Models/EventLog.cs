using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GStore.Models
{
	[Table("SystemEvents")]
	public class SystemEvent : BaseClasses.EventLogBase
	{
		public int SystemEventID { get; set; }
		public int Level { get; set; }

		[Required]
		public string LevelText { get; set; }
	}

	[Table("SecurityEvents")]
	public class SecurityEvent : BaseClasses.EventLogBase
	{
		public int SecurityEventID { get; set; }
		public int Level { get; set; }

		[Required]
		public string LevelText { get; set; }
		public bool Success { get; set; }

	}

	[Table("PageViewEvents")]
	public class PageViewEvent : BaseClasses.EventLogBase
	{
		public int PageViewEventID { get; set; }
	}

	[Table("UserActionEvents")]
	public class UserActionEvent : BaseClasses.EventLogBase
	{
		public int UserActionEventID { get; set; }
		
		[Required]
		public string Category { get; set; }
		
		[Required]
		public string Action { get; set; }
		
		[Required]
		public string Label { get; set; }

	}

	[Table("FileNotFoundLog")]
	public class FileNotFoundLog : BaseClasses.EventLogBase
	{
		public int FileNotFoundLogId { get; set; }
	}

	[Table("BadRequests")]
	public class BadRequest : BaseClasses.EventLogBase
	{
		public int BadRequestId { get; set; }
	}
}

