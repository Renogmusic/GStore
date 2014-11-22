//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Web;

//namespace GStore.Identity
//{
//	public partial class AspNetIdentityUserClaim : Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>
//	{
//		public AspNetIdentityUserClaim(Models.UserProfile createUserProfile, string userId, GStoreAction adminAction, bool isAdminAction, int? clientIdScope, int? storeFrontIdScope)
//		{
//			this.UserId = userId;
//			this.ClaimType = "User";
//			this.ClaimValue = System.Enum.GetName(typeof(GStoreAction), adminAction);
//			this.IsAdminClaim = isAdminAction;
//			this.ActionId = adminAction;
//			this.ActionName = System.Enum.GetName(typeof(GStoreAction), adminAction);
//			this.ClientRoleId = null;
//			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
//			this.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
//			this.IsPending = false;
//			this.SetDefaults(createUserProfile);
//		}

//		public AspNetIdentityUserClaim(Models.UserProfile createUserProfile, string userId, int clientRoleId, string clientRoleName, bool isAdminRole, int clientIdScope, int? storeFrontIdScope)
//		{
//			this.UserId = userId;
//			this.ClaimType = "ClientRole";
//			this.ClaimValue = clientRoleName;
//			this.IsAdminClaim = isAdminRole;
//			this.ActionId = null;
//			this.ActionName = "[" + clientRoleName + "]";
//			this.ClientRoleId = clientRoleId;
//			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
//			this.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
//			this.IsPending = false;
//			this.SetDefaults(createUserProfile);
//		}

//		[ForeignKey("UserId")]
//		public virtual AspNetIdentityUser User { get; set; }

//		public bool IsAdminClaim { get; set; }

//		public GStoreAction? ActionId { get; set; }

//		public int? ClientRoleId { get; set; }

//		[Required]
//		public string ActionName { get; set; }

//		/// <summary>
//		/// Client id this permission applies to, if null applies to all clients
//		/// </summary>
//		public int? ScopeClientId { get; set; }

//		/// <summary>
//		/// StoreFrontId this claim applies to, if null applies to all storefronts for the ScopeClientId
//		/// </summary>
//		public int? ScopeStoreFrontId { get; set; }

//		/// <summary>
//		/// If true, signals that this record is not active (like an Active flag)
//		/// </summary>
//		public bool IsPending { get; set; }
//		public DateTime StartDateTimeUtc { get; set; }
//		public DateTime EndDateTimeUtc { get; set; }

//		/// <summary>
//		/// Date and time in UTC when record was created
//		/// </summary>
//		[DataType(DataType.DateTime)]
//		[Editable(false)]
//		[Display(Name = "Created On")]
//		[UIHint("DateTimeUtcToLocal")]
//		public DateTime CreateDateTimeUtc { get; set; }

//		/// <summary>
//		/// UserID or "unknown" who created this record
//		/// </summary>
//		[Editable(false)]
//		[Display(Name = "Created By")]
//		public int? CreatedBy_UserProfileId { get; set; }

//		/// <summary>
//		/// Date and time in UTC when record was last updated
//		/// </summary>
//		[DataType(DataType.DateTime)]
//		[Editable(false)]
//		[Display(Name = "Updated On")]
//		[UIHint("DateTimeUtcToLocal")]
//		public DateTime UpdateDateTimeUtc { get; set; }

//		/// <summary>
//		/// UserID or "unknown" who last updated this record
//		/// </summary>
//		[Editable(false)]
//		[Display(Name = "Updated By")]
//		public int? UpdatedBy_UserProfileId { get; set; }

//		/// <summary>
//		/// Sets defaults for the record (for new records)
//		/// </summary>
//		protected virtual void SetDefaults(GStore.Models.UserProfile userProfile)
//		{
//			Update(userProfile);
//		}

//		/// <summary>
//		/// Updated UpdateDateTimeUtc and updateuserid to current
//		/// </summary>
//		public void Update(GStore.Models.UserProfile userProfile)
//		{
//			if (userProfile == null && this.CreatedBy_UserProfileId == null && this.UpdatedBy_UserProfileId == null)
//			{
//				throw new ApplicationException("User profile cannot be null for CreateUser/UpdateUser");
//			}
//			//new record fill create info
//			if (CreateDateTimeUtc == null || CreateDateTimeUtc == DateTime.MinValue)
//			{
//				CreateDateTimeUtc = DateTime.UtcNow;
//			}
//			if (this.CreatedBy_UserProfileId == null && userProfile != null)
//			{
//				CreatedBy_UserProfileId = userProfile.UserProfileId;
//			}
//			UpdateDateTimeUtc = DateTime.UtcNow;
//			UpdatedBy_UserProfileId = userProfile.UserProfileId;
//		}

//		public bool IsActiveDirect()
//		{
//			return IsActiveDirect(DateTime.UtcNow);
//		}
//		public bool IsActiveDirect(DateTime dateTime)
//		{
//			if (!IsPending && (StartDateTimeUtc < dateTime) && ( EndDateTimeUtc > dateTime))
//			{
//				return true;
//			}
//			return false;
//		}
//	}

//	/// <summary>
//	/// Identity context for ASP.Net Identity operations
//	/// </summary>
//	public partial class AspNetIdentityContext : Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext<AspNetIdentityUser, AspNetIdentityRole, string, AspNetIdentityUserLogin, AspNetIdentityUserRole, AspNetIdentityUserClaim>
//	{
//		protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
//		{
//			base.OnModelCreating(modelBuilder);
//		}
//	}
//}