using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GStoreData.AppHtmlHelpers;
using GStoreData.ControllerBase;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData
{
	public static class BlogExtensions
	{

		public static string MetaDescriptionOrSystemDefault(this Blog blog, StoreFrontConfiguration config)
		{
			if (!string.IsNullOrEmpty(blog.DefaultMetaDescription))
			{
				return blog.DefaultMetaDescription;
			}
			return config.MetaDescription;
		}

		public static string MetaKeywordsOrSystemDefault(this Blog blog, StoreFrontConfiguration config)
		{
			if (!string.IsNullOrEmpty(blog.DefaultMetaKeywords))
			{
				return blog.DefaultMetaKeywords;
			}
			return config.MetaKeywords;
		}

		public static Theme ThemeOrSystemDefault(this Blog blog, StoreFrontConfiguration config)
		{
			if (blog.Theme != null)
			{
				return blog.Theme;
			}
			return config.BlogTheme;
		}

		public static string MetaDescriptionOrSystemDefault(this BlogEntry entry, StoreFrontConfiguration config)
		{
			if (!string.IsNullOrEmpty(entry.MetaDescription))
			{
				return entry.MetaDescription;
			}
			return entry.Blog.MetaDescriptionOrSystemDefault(config);
		}

		public static string MetaKeywordsOrSystemDefault(this BlogEntry entry, StoreFrontConfiguration config)
		{
			if (!string.IsNullOrEmpty(entry.MetaKeywords))
			{
				return entry.MetaKeywords;
			}
			return entry.Blog.MetaKeywordsOrSystemDefault(config);
		}

		public static Theme ThemeOrSystemDefault(this BlogEntry blogEntry, StoreFrontConfiguration config)
		{
			if (blogEntry.Theme != null)
			{
				return blogEntry.Theme;
			}
			return blogEntry.Blog.ThemeOrSystemDefault(config);
		}

		public static bool ValidateBlogUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int? currentBlogId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this Blog";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}
			if (urlName.Trim().ToLower() == "all")
			{
				string errorMessage = "URL Name cannot be 'All'\n Please enter a unique URL name for this Blog";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			Blog conflict = db.Blogs.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.UrlName.ToLower() == urlName && (p.BlogId != currentBlogId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Blog '" + conflict.Name + "' [" + conflict.BlogId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Blog URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateBlogEntryUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int currentBlogId, int? currentBlogEntryId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this Blog Entry";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}
			if (urlName.Trim().ToLower() == "all")
			{
				string errorMessage = "URL Name cannot be 'All'\n Please enter a unique URL name for this Blog Entry";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}


			BlogEntry conflict = db.BlogEntries.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.UrlName.ToLower() == urlName && p.BlogId == currentBlogId && (p.BlogEntryId != currentBlogEntryId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Blog Entry '" + conflict.Name + "' [" + conflict.BlogEntryId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Blog Entry URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}



		public static IQueryable<Blog> WhereRegisteredAnonymousCheck(this IQueryable<Blog> query, bool isRegistered)
		{
			return query.Where(pc =>
					(isRegistered || !pc.ForRegisteredOnly || pc.ShowInListEvenIfNoPermission)
					&&
					(!isRegistered || !pc.ForAnonymousOnly));
		}

		public static IQueryable<BlogEntry> WhereRegisteredAnonymousCheck(this IQueryable<BlogEntry> query, bool isRegistered)
		{
			return query.Where(be =>
					(isRegistered || !be.ForRegisteredOnly || be.ShowInListEvenIfNoPermission)
					&&
					(!isRegistered || !be.ForAnonymousOnly)
					&&
					(isRegistered || !be.Blog.ForRegisteredOnly || be.Blog.ShowInListEvenIfNoPermission)
					&&
					(!isRegistered || !be.Blog.ForAnonymousOnly)
					)
					;
		}

		public static List<Blog> BlogsForUser(this StoreFront storeFront, bool isRegistered)
		{
			return storeFront.Blogs.AsQueryable().WhereIsActive().WhereRegisteredAnonymousCheck(isRegistered).ApplyDefaultSort().ToList();
		}

		public static List<BlogEntry> BlogEntriesForUser(this Blog blog, bool isRegistered)
		{
			return blog.BlogEntries.AsQueryable().WhereIsActive().WhereRegisteredAnonymousCheck(isRegistered).ApplyDefaultSort().ToList();
		}

		public static BlogEntry LatestEntryForUser(this Blog blog, bool isRegistered)
		{
			return blog.BlogEntries.AsQueryable().WhereIsActive().WhereRegisteredAnonymousCheck(isRegistered).ApplyDefaultSort().FirstOrDefault();
		}

	}
}