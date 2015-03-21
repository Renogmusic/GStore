using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStoreData.Models.BaseClasses
{
	/// <summary>
	/// Records with CreateDateTime, UpdateDateTime, REQUIRED: CreateUserId, UpdateUserId
	/// </summary>
	public abstract class AuditFieldsAllRequired: GStoreEntity
	{
		/// <summary>
		/// Date and time in UTC when record was created
		/// </summary>
		[DataType(DataType.DateTime)]
		[Editable(false)]
		[UIHint("DateTimeUtcToLocal")]
		[Required]
		[Display(Name = "Created On", Description = "Date and Time in UTC time this record was created")]
		public DateTime CreateDateTimeUtc { get; set; }

		/// <summary>
		/// UserID or "unknown" who created this record
		/// </summary>
		[Editable(false)]
		[Required]
		[Display(Name = "Created By Id", Description = "User Profile Id of the user that created this record.")]
		public int CreatedBy_UserProfileId { get; set; }

		[ForeignKey("CreatedBy_UserProfileId")]
		[Display(Name = "Created By", Description = "User Profile of the user that created this record.")]
		public virtual UserProfile CreatedBy { get; set; }

		/// <summary>
		/// Date and time in UTC when record was last updated
		/// </summary>
		[DataType(DataType.DateTime)]
		[Editable(false)]
		[UIHint("DateTimeUtcToLocal")]
		[Required]
		[Display(Name = "Updated On", Description = "Date and Time in UTC time this record was last updated.")]
		public DateTime UpdateDateTimeUtc { get; set; }

		/// <summary>
		/// UserID or "unknown" who last updated this record
		/// </summary>
		[Editable(false)]
		[Required]
		[Display(Name = "Updated By Id", Description = "User Profile Id of the user that last updated this record.")]
		public int UpdatedBy_UserProfileId { get; set; }

		[ForeignKey("UpdatedBy_UserProfileId")]
		[Display(Name = "Updated By", Description = "User Profile of the user that last updated this record.")]
		public virtual UserProfile UpdatedBy { get; set; }

	}
}