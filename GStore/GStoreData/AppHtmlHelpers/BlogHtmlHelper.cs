using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using GStoreData.Models;
using GStoreData.ViewModels;
using System.Web.Mvc.Html;
using System.Web;

namespace GStoreData.AppHtmlHelpers
{
	/// <summary>
	/// HTML Helper for Blog section
	/// </summary>
	public static class BlogHtmlHelper
	{
		public static MvcHtmlString LatestEntryInfoLink(this Blog blog, HtmlHelper htmlHelper, bool isRegistered)
		{
			if (blog == null)
			{
				throw new ArgumentNullException("blog");
			}

			if (blog.BlogEntries == null || blog.BlogEntries.Count == 0)
			{
				return new MvcHtmlString("(none)");
			}

			BlogEntry latest = blog.LatestEntryForUser(isRegistered);

			if (latest == null)
			{
				return new MvcHtmlString("(none)");
			}

			return latest.EntryInfoLink(htmlHelper);
		}

		public static MvcHtmlString EntryInfoLink(this BlogEntry entry, HtmlHelper htmlHelper)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}

			return htmlHelper.ActionLink(entry.Name + " - " + entry.PostDateTimeUtc.ToUserDateTimeString(htmlHelper), "Index", new { blogUrlName = entry.Blog.UrlName, blogEntryUrlName = entry.UrlName });
		}

	}
}
