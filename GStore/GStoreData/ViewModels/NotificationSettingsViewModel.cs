using System.ComponentModel.DataAnnotations;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class NotificationSettingsViewModel
	{
		/// <summary>
		/// Email address from identity
		/// </summary>
		[Editable(false)]
		public string Email { get; set; }

		/// <summary>
		/// True if user allows other users to send them messages through site
		/// </summary>
		[Display(Name = "Allow users to send me site messages")]
		public bool AllowUsersToSendSiteMessages { get; set; }

		/// <summary>
		/// When true, notifies all users (and anonymous) when user logs  on
		/// </summary>
		[Display(Name = "Notify all users when I log on/off")]
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
		/// When true, notifies user by email for all site messages
		/// </summary>
		[Display(Name = "Send me a Text Message when there is a site message for me")]
		public bool SendSiteMessagesToSms { get; set; }

		public UserProfile UserProfile { get; set; }
	}
}
