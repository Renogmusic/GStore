using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace GStore.Models
{
	[Table("NotificationLinks")]
	public class NotificationLink : BaseClasses.StoreFrontRecord
	{
		public int NotificationLinkId { get; set; }
		public int NotificationId { get; set; }
		public int Order { get; set; }

		public bool IsExternal { get; set; }

		[Required]
		public string Url { get; set; }

		[Required]
		public string LinkText { get; set; }

		public virtual Notification Notification { get; set; }

		public string FullNotificationLinkUrl(string urlHostForLocal)
		{
			if (this.IsExternal)
			{
				return NotificationLinkUrl();
			}

			return "http://" + urlHostForLocal + NotificationLinkUrl();
		}

		public string FullNotificationLinkTag(int counter, string urlHostForLocal)
		{
			string returnHtml = string.Empty;
			string url = this.NotificationLinkUrl();

			if (!this.IsExternal)
			{
				url = "http://" + urlHostForLocal + url;
			}

			returnHtml = "Link " + HttpUtility.HtmlEncode(counter) + ": "
				+ "<a target=\"_blank\""
				+ " href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ " title=\"Go to " + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ ">"
				+ HttpUtility.HtmlEncode(this.LinkText) + " (Url: " + HttpUtility.HtmlEncode(url) + ")"
				+ "</a>";

			return returnHtml;

		}

		public string NotificationLinkUrl()
		{
			string url = this.Url;

			//enforce url rules
			if (this.IsExternal)
			{
				if (url.StartsWith("http://") || url.StartsWith("mailto:"))
				{
					//link start is good
				}
				else
				{
					url = "http://" + url;
				}
			}
			else
			{
				string appRoot = HttpContext.Current.Request.ApplicationPath;
				if (url.StartsWith("/") || url.StartsWith("~/"))
				{
					//starts with slash add root virtual path
					url = appRoot + url.TrimStart('~').TrimStart('/');
				}
				else
				{
					url = appRoot + url;
				}
			}

			return url;

		}

		public string NotificationLinkTag(int counter)
		{
			string returnHtml = string.Empty;
			string url = this.NotificationLinkUrl();

			returnHtml = "Link " + HttpUtility.HtmlEncode(counter) + ": "
				+ "<a target=\"_blank\""
				+ " href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ " title=\"Go to " + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ ">"
				+ HttpUtility.HtmlEncode(this.LinkText) + " (Url: " + HttpUtility.HtmlEncode(url) + ")"
				+ "</a>";

			return returnHtml;

		}
	}


}