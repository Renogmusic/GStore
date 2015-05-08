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

		public static BlogEntry LatestEntryForAdmin(this Blog blog)
		{
			return blog.BlogEntries.AsQueryable().ApplyDefaultSort().FirstOrDefault();
		}

		public static void SetDefaultsForNew(this Blog blog, StoreFront storeFront, UserProfile userProfile)
		{
			blog.SetDefaults(userProfile);

			if (storeFront == null)
			{
				blog.Name = "New Blog";
				blog.UrlName = blog.Name.FixUrlName();
				blog.Order = 1000;
			}
			else
			{
				blog.Client = storeFront.Client;
				blog.ClientId = storeFront.ClientId;
				blog.StoreFront = storeFront;
				blog.StoreFrontId = blog.StoreFrontId;

				blog.Name = "New Blog";
				blog.UrlName = blog.Name.FixUrlName();

				if (!storeFront.Blogs.Any())
				{
					blog.Order = 1000;
				}
				else 
				{
					blog.Order = storeFront.Blogs.Max(b => b.Order) + 10;
					if (storeFront.Blogs.Any(b => b.Name.ToLower() == blog.Name.ToLower() || b.UrlName.ToLower() == blog.UrlName.ToLower()))
					{
						bool conflict = true;
						int index = 0;
						do
						{
							index++;
							blog.Name = "New Blog " + index;
							blog.UrlName = blog.Name.FixUrlName();
							conflict = storeFront.Blogs.Any(b => b.Name.ToLower() == blog.Name.ToLower() || b.UrlName.ToLower() == blog.UrlName.ToLower());
						} while (conflict);
					}
				}
			}

			blog.AutoDisplayLatestEntry = true;
			blog.Description = blog.Name;
			blog.ShowInListEvenIfNoPermission = true;
			blog.IsPending = false;
			blog.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			blog.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);

		}

		public static void SetDefaultsForNew(this BlogEntry blogEntry, Blog blog, UserProfile userProfile)
		{
			if (blog == null)
			{
				throw new ArgumentNullException("blog");
			}
			StoreFront storeFront = blog.StoreFront;

			blogEntry.SetDefaults(userProfile);

			blogEntry.Client = storeFront.Client;
			blogEntry.ClientId = storeFront.ClientId;
			blogEntry.StoreFront = storeFront;
			blogEntry.StoreFrontId = storeFront.StoreFrontId;
			blogEntry.Blog = blog;
			blogEntry.BlogId = blog.BlogId;

			blogEntry.Name = "New Blog Post";
			blogEntry.UrlName = blogEntry.Name.FixUrlName();

			if (!blog.BlogEntries.Any())
			{
				blogEntry.Order = 1000;
			}
			else
			{
				blogEntry.Order = blog.BlogEntries.Max(be => be.Order) + 10;
				if (blog.BlogEntries.Any(be => be.Name.ToLower() == blogEntry.Name.ToLower() || be.UrlName.ToLower() == blogEntry.UrlName.ToLower()))
				{
					bool conflict = true;
					int index = 0;
					do
					{
						index++;
						blogEntry.Name = "New Blog Entry " + index;
						blogEntry.UrlName = blogEntry.Name.FixUrlName();
						conflict = blog.BlogEntries.Any(be => be.Name.ToLower() == blogEntry.Name.ToLower() || be.UrlName.ToLower() == blogEntry.UrlName.ToLower());
					} while (conflict);
				}
			}

			blogEntry.PostDateTimeUtc = DateTime.UtcNow;
			blogEntry.ShowInListEvenIfNoPermission = true;
			blogEntry.IsPending = false;
			blogEntry.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			blogEntry.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

		}

	}
}