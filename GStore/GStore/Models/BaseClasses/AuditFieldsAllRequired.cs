using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace GStore.Models.BaseClasses
{
	/// <summary>
	/// Records with CreateDateTime, UpdateDateTime, REQUIRED: CreateUserId, UpdateUserId
	/// </summary>
	public abstract class AuditFieldsAllRequired
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
		[Display(Name = "Created By")]
		public int CreatedBy_UserProfileId { get; set; }
		[Required]
		[ForeignKey("CreatedBy_UserProfileId")]
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
		[Display(Name = "Updated By")]
		public int UpdatedBy_UserProfileId { get; set; }
		[Required]
		[ForeignKey("UpdatedBy_UserProfileId")]
		public virtual UserProfile UpdatedBy { get; set; }

		/// <summary>
		/// Sets defaults for the record (for new records)
		/// </summary>
		protected virtual void SetDefaults(UserProfile userProfile)
		{
			Update(userProfile);
		}

		/// <summary>
		/// Updated UpdateDateTimeUtc and updateuserid to current
		/// </summary>
		public void Update(UserProfile userProfile)
		{
			if (userProfile == null && CreatedBy == null && UpdatedBy == null)
			{
				throw new ApplicationException("User profile cannot be null for CreateUser/UpdateUser using AuditableFieldsAllRequired");
			}
			//new record fill create info
			if (CreateDateTimeUtc == null || CreateDateTimeUtc == DateTime.MinValue)
			{
				CreateDateTimeUtc = DateTime.UtcNow;
			}
			if (CreatedBy == null && userProfile != null)
			{
				CreatedBy = userProfile;
			}
			UpdateDateTimeUtc = DateTime.UtcNow;
			UpdatedBy = userProfile;

		}
	}
}