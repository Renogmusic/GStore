using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GStore.Models
{
	/// <summary>
	/// User Profile information and settings
	/// </summary>
	[Table("UserProfiles")]
	public class UserProfile : BaseClasses.AuditFieldsUserProfileOptional
	{
		/// <summary>
		/// PK (counter) UserProfile.UserProfileId
		/// </summary>
		[Editable(false)]
		[Key]
		public int UserProfileId { get; set; }

		/// <summary>
		/// UserId (string) from Identity
		/// </summary>
		[Editable(false)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(255)]
		public string UserId { get; set; }

		/// <summary>
		/// User name from identity (same as email)
		/// </summary>
		[Display(Name = "User Name")]
		[Editable(false)]
		[Required]
		[Index(IsUnique=true)]
		[MaxLength(255)]
		public string UserName { get; set; }

		/// <summary>
		/// Email address from identity
		/// </summary>
		[Editable(false)]
		[Required]
		[Index(IsUnique = true)]
		[MaxLength(255)]
		public string Email { get; set; }

		public int? StoreFrontId { get; set; }
		[ForeignKey("StoreFrontId")]
		public virtual StoreFront StoreFront { get; set; }

		public int? ClientId { get; set; }
		[ForeignKey("ClientId")]
		public virtual Client Client { get; set; }


		public bool Active { get; set; }

		public DateTime StartDateTimeUtc { get; set; }

		public DateTime EndDateTimeUtc { get; set; }

		/// <summary>
		/// Name as displayed in messages and site logon/logoff
		/// </summary>
		[Display(Name = "Name")]
		[Required]
		public string FullName { get; set; }

		/// <summary>
		/// Notes entered at signup
		/// </summary>
		public string SignupNotes { get; set; }

		/// <summary>
		/// User has asked for more info at signup
		/// </summary>
		[Display(Name = "Send Me More Info by Email")]
		public bool SendMoreInfoToEmail { get; set; }

		#region Notification, messaging, subscription settings	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-

		/// <summary>
		/// True if user allows other users to send them messages through site
		/// </summary>
		[Display(Name = "Allow users to send me site messages")]
		public bool AllowUsersToSendSiteMessages { get; set; }

		/// <summary>
		/// When true, notifies all users (and anonymous) when user logs  on
		/// </summary>
		[Display(Name = "Display pop-up to all users when loging on/off")]
		public bool NotifyAllWhenLoggedOn { get; set; }

		/// <summary>
		/// When true, notifies user by email for site updates
		/// </summary>
		[Display(Name = "Email me when this site is updated")]
		public bool NotifyOfSiteUpdatesToEmail { get; set; }

		/// <summary>
		/// True if user is subscribed to the newsletter
		/// </summary>
		[Display(Name = "Subscribe to Newsletter Email")]
		public bool SubscribeToNewsletterEmail { get; set; }

		/// <summary>
		/// When true, notifies user by email for all site messages
		/// </summary>
		[Display(Name = "Email me when there is a site message for me")]
		public bool SendSiteMessagesToEmail { get; set; }

		/// <summary>
		/// When true, notifies user by SMS text message for all site messages
		/// </summary>
		[Display(Name = "Send a Text Message to my phone when there is a site message for me")]
		public bool SendSiteMessagesToSms { get; set; }

		#endregion

		#region Historical dates for auditing and notifications	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-	-

		/// <summary>
		/// Date and time in UTC of last successful logon
		/// </summary>
		[Editable(false)]
		public DateTime? LastLogonDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of more info sent to user
		/// </summary>
		[Editable(false)]
		public DateTime? SentMoreInfoToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last site update notification by email
		/// </summary>
		[Editable(false)]
		public DateTime? LastSiteUpdateSentToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last newsletter sent by email
		/// </summary>
		[Editable(false)]
		public DateTime? LastNewsletterSentToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last site message notification by email
		/// </summary>
		[Editable(false)]
		public DateTime? LastSiteMessageSentToEmailDateTimeUtc { get; set; }

		/// <summary>
		/// Date and time in UTC of last logon failure notification by email
		/// </summary>
		[Editable(false)]
		public DateTime? LastLockoutFailureNoticeDateTimeUtc { get; set; }

		#endregion


		/// <summary>
		/// Notifications table data for user
		/// </summary>
		public virtual ICollection<Notification> Notifications { get; set; }

		/// <summary>
		/// Notifications sent from the user user
		/// </summary>
		public virtual ICollection<Notification> NotificationsSent { get; set; }

		public virtual ICollection<ClientUserRole> ClientUserRoles { get; set; }

		public virtual ICollection<StoreFront> WelcomeStoreFronts { get; set; }
		public virtual ICollection<StoreFront> AccountAdminStoreFronts { get; set; }
		public virtual ICollection<StoreFront> RegisteredNotifyStoreFronts { get; set; }
	}
}
