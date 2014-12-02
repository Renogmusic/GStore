using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Web;

namespace GStore.Models.BaseClasses
{
	/// <summary>
	/// Records with CreateDateTime, UpdateDateTime, OPTIONAL: CreateUserId, UpdateUserId
	/// </summary>
	public abstract class AuditFieldsUserProfileOptional : GStoreEntity
	{
		/// <summary>
		/// Date and time in UTC when record was created
		/// </summary>
		[DataType(DataType.DateTime)]
		[Editable(false)]
		[Display(Name = "Created On")]
		[UIHint("DateTimeUtcToLocal")]
		public DateTime CreateDateTimeUtc { get; set; }

		/// <summary>
		/// UserID or "unknown" who created this record
		/// </summary>
		[Editable(false)]
		[Display(Name = "Created By Id")]
		public int? CreatedBy_UserProfileId { get; set; }

		[Display(Name = "Created By")]
		[ForeignKey("CreatedBy_UserProfileId")]
		[System.Xml.Serialization.XmlIgnore]
		public virtual UserProfile CreatedBy { get; set; }

		/// <summary>
		/// Date and time in UTC when record was last updated
		/// </summary>
		[DataType(DataType.DateTime)]
		[Editable(false)]
		[Display(Name = "Updated On")]
		[UIHint("DateTimeUtcToLocal")]
		public DateTime UpdateDateTimeUtc { get; set; }

		/// <summary>
		/// UserID or "unknown" who last updated this record
		/// </summary>
		[Editable(false)]
		[Display(Name = "Updated By Id")]
		public int? UpdatedBy_UserProfileId  { get; set; }
		[Display(Name = "Updated By")]
		[ForeignKey("UpdatedBy_UserProfileId")]
		[System.Xml.Serialization.XmlIgnore]
		public virtual UserProfile UpdatedBy { get; set; }

	}
}