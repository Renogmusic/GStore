using GStore.Models;
using System.Web;

namespace GStore.Data
{
	public static class NotificationExtensions
	{
		public static string FullNotificationLinkUrl(this NotificationLink link, string urlHostForLocal)
		{
			if (link.IsExternal)
			{
				return link.NotificationLinkUrl();
			}

			return "http://" + urlHostForLocal + link.NotificationLinkUrl();
		}

		public static string FullNotificationLinkTag(this NotificationLink link, int counter, string urlHostForLocal)
		{
			string returnHtml = string.Empty;
			string url = link.NotificationLinkUrl();

			if (!link.IsExternal)
			{
				url = "http://" + urlHostForLocal + url;
			}

			returnHtml = "Link " + HttpUtility.HtmlEncode(counter) + ": "
				+ "<a target=\"_blank\""
				+ " href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ " title=\"Go to " + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ ">"
				+ HttpUtility.HtmlEncode(link.LinkText) + " (Url: " + HttpUtility.HtmlEncode(url) + ")"
				+ "</a>";

			return returnHtml;

		}

		public static string NotificationLinkUrl(this NotificationLink link)
		{
			string url = link.Url;

			//enforce url rules
			if (link.IsExternal)
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

		public static string NotificationLinkTag(this NotificationLink link, int counter)
		{
			string returnHtml = string.Empty;
			string url = link.NotificationLinkUrl();

			returnHtml = "Link " + HttpUtility.HtmlEncode(counter) + ": "
				+ "<a target=\"_blank\""
				+ " href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ " title=\"Go to " + HttpUtility.HtmlAttributeEncode(url) + "\""
				+ ">"
				+ HttpUtility.HtmlEncode(link.LinkText) + " (Url: " + HttpUtility.HtmlEncode(url) + ")"
				+ "</a>";

			return returnHtml;

		}
	}
}