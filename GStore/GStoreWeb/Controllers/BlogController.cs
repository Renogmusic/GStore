using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreWeb.Controllers
{
	public class BlogController : AreaBaseController.RootAreaBaseController
	{
		#region Public Blog Actions

		/// <summary>
		/// Flow: is both params are null, system will search for blogs. If no blogs or multiple blogs are found, a list will be returned.
		/// If only one blog is found, it will be displayed
		/// 
		/// </summary>
		/// <param name="blogUrlName"></param>
		/// <param name="blogEntryUrlName"></param>
		/// <returns></returns>
		public ActionResult Index(string blogUrlName, string blogEntryUrlName)
		{
			bool blogUrlNameIsAll = (!string.IsNullOrEmpty(blogUrlName) && blogUrlName.Trim().ToLower() == "all");
			bool blogEntryUrlNameIsAll = (!string.IsNullOrEmpty(blogEntryUrlName) && blogEntryUrlName.Trim().ToLower() == "all");

			if (string.IsNullOrWhiteSpace(blogUrlName) || blogUrlNameIsAll)
			{
				//no blog name, get list of blogs and auto-feed first
				List<Blog> blogs = CurrentStoreFrontOrThrow.BlogsForUser(User.IsRegistered());
				if (blogs.Count == 1 && !blogUrlNameIsAll)
				{
					List<BlogEntry> entries = blogs[0].BlogEntriesForUser(User.IsRegistered());
					if (entries.Count == 1)
					{
						return RedirectToAction("Index", RouteDataForIndex(blogs[0].UrlName, entries[0].UrlName));
					}
					return RedirectToAction("Index", RouteDataForIndex(blogs[0].UrlName, ""));
				}
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_List, "", true);
				return ListBlogs(blogs);
			}

			//blog was specified, look it up
			Blog blog = null;
			try
			{
				blog = GetBlog(blogUrlName);
			}
			catch(LoginRequiredException ex)
			{
				string message = "You must log in to view blog '" + ex.BlogName.ToHtml() + "'.";
				return BounceToLoginNoAccessResult(message);
			}
			catch (Exception)
			{
				throw;
			}
			if (blog == null)
			{
				//blog not found or no access, display user messages
				return RedirectToAction("Index", RouteDataForIndex("", ""));
			}

			if (string.IsNullOrWhiteSpace(blogEntryUrlName) || blogEntryUrlNameIsAll)
			{
				//blog is passed but no blog entry selected
				List<BlogEntry> entries = blog.BlogEntriesForUser(User.IsRegistered());
				if (entries.Count == 1 && !blogEntryUrlNameIsAll)
				{
					return RedirectToAction("Index", RouteDataForIndex(blog.UrlName, entries[0].UrlName));
				}
				return ViewBlog(blog, blogEntryUrlNameIsAll);
			}

			BlogEntry blogEntry = null;
			try
			{
				blogEntry = GetBlogEntry(blog, blogEntryUrlName);
			}
			catch(LoginRequiredException ex)
			{
				string message = "Sorry, you must log in to view blog entry '" + ex.BlogEntryName.ToHtml() + "' from the blog '" + ex.BlogName.ToHtml() + "'.";
				return BounceToLoginNoAccessResult(message);
			}
			catch (Exception)
			{
				throw;
			}
			if (blogEntry == null)
			{
				//blog entry not found or no access, show user messages
				return RedirectToAction("Index", RouteDataForIndex(blog.UrlName, ""));
			}

			return ViewBlogEntry(blogEntry);
		}

		/// <summary>
		/// Blogs param may be null, then it will auto-populate
		/// </summary>
		/// <param name="blogs"></param>
		/// <returns></returns>
		protected ActionResult ListBlogs(List<Blog> blogs)
		{
			if (blogs == null)
			{
				blogs = CurrentStoreFrontOrThrow.BlogsForUser(User.IsRegistered());
			}

			return View("Index", blogs);
		}

		protected ActionResult ViewBlog(Blog blog, bool blogEntryUrlNameIsAll)
		{
			if (blog == null)
			{
				throw new ArgumentNullException("blog");
			}

			if (!blogEntryUrlNameIsAll && blog.AutoDisplayLatestEntry)
			{
				BlogEntry firstEntry = blog.BlogEntriesForUser(User.IsRegistered()).FirstOrDefault();
				if (firstEntry != null)
				{
					return RedirectToAction("Index", RouteDataForIndex(blog.UrlName, firstEntry.UrlName));
				}
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_View, blog.UrlName, true, blogId: blog.BlogId);

			SetMetaTagsAndTheme(blog);

			return View("ViewBlog", blog);
		}

		protected ActionResult ViewBlogEntry(BlogEntry blogEntry)
		{
			if (blogEntry == null)
			{
				throw new ArgumentNullException("blogEntry");
			}

			GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_ViewEntry, blogEntry.UrlName, true, blogId: blogEntry.BlogId, blogEntryId: blogEntry.BlogEntryId);

			SetMetaTagsAndTheme(blogEntry);

			return View("ViewBlogEntry", blogEntry);
		}

		/// <summary>
		/// Gets a specific blog and logs events and sets user messages if not found
		/// does not log event if blog found
		/// </summary>
		/// <param name="blogUrlName"></param>
		/// <returns></returns>
		protected Blog GetBlog(string blogUrlName)
		{
			if (string.IsNullOrWhiteSpace(blogUrlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_View, "no blog name", false);
				AddUserMessage("Blog not found.", "Sorry, the blog you are looking for cannot be found. Please try another blog.", UserMessageType.Warning);
				return null;
			}

			//find the blog, check visibility, log if fail,
			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.UrlName.ToLower() == blogUrlName.Trim().ToLower()).SingleOrDefault();
			if (blog == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_View, "blog not found: " + blogUrlName, false);
				AddUserMessage("Blog not found.", "Sorry, the blog '" + blogUrlName.ToHtml() + "' cannot be found. Please try another blog.", UserMessageType.Warning);
				return null;
			}

			if (!blog.IsActiveDirect())
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_View, "blog is inactive: " + blogUrlName, false, blogId: blog.BlogId);
				AddUserMessage("Blog Is Not Active.", "Sorry, the blog '" + blogUrlName.ToHtml() + "' is not active. Please try another blog.", UserMessageType.Warning);
				return null;
			}

			if (blog.ForAnonymousOnly && !blog.ForRegisteredOnly && User.IsRegistered())
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_View, "blog anonymous only: " + blogUrlName, false, blogId: blog.BlogId);
				AddUserMessage("Blog Unavailable.", "Sorry, the blog '" + blogUrlName.ToHtml() + "' is for anonymous users only. Please try another blog.", UserMessageType.Warning);
				return null;
			}
			else if (blog.ForRegisteredOnly && !blog.ForAnonymousOnly && User.IsAnonymous())
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_View, "blog registered only: " + blogUrlName, false, blogId: blog.BlogId);
				throw new LoginRequiredException(blog.Name, null);
			}

			return blog;

		}

		protected BlogEntry GetBlogEntry(Blog blog, string blogEntryUrlName)
		{
			if (string.IsNullOrWhiteSpace(blogEntryUrlName))
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_ViewEntry, "no blog entry name", false, blogId: blog.BlogId);
				AddUserMessage("Blog Post not found.", "Sorry, the blog post you are looking for cannot be found. Please try another blog post.", UserMessageType.Warning);
				return null;
			}

			BlogEntry blogEntry = blog.BlogEntries.AsQueryable().WhereIsActive().WhereRegisteredAnonymousCheck(User.IsRegistered()).SingleOrDefault(be => be.UrlName.ToLower() == blogEntryUrlName.Trim().ToLower());
			if (blogEntry == null)
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_ViewEntry, "blog entry not found: " + blogEntryUrlName, false);
				AddUserMessage("Blog Post not found.", "Sorry, the blog post '" + blogEntryUrlName.ToHtml() + "' cannot be found. Please try another blog post.", UserMessageType.Warning);
				return null;
			}

			if (!blog.IsActiveDirect())
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_ViewEntry, "blog entry is inactive: " + blogEntryUrlName, false, blogId: blog.BlogId, blogEntryId: blogEntry.BlogEntryId);
				AddUserMessage("Blog Post Is Not Active.", "Sorry, the blog post '" + blogEntryUrlName.ToHtml() + "' is not active. Please try another blog post.", UserMessageType.Warning);
				return null;
			}

			if (blog.ForAnonymousOnly && !blog.ForRegisteredOnly && User.IsRegistered())
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_ViewEntry, "blog entry anonymous only: " + blogEntryUrlName, false, blogId: blog.BlogId, blogEntryId: blogEntry.BlogEntryId);
				AddUserMessage("Blog Post Unavailable.", "Sorry, the blog post '" + blogEntryUrlName.ToHtml() + "' is for anonymous users only. Please try another blog post.", UserMessageType.Warning);
				return null;
			}
			else if (blogEntry.ForRegisteredOnly && !blogEntry.ForAnonymousOnly && User.IsAnonymous())
			{
				GStoreDb.LogUserActionEvent(HttpContext, RouteData, this, UserActionCategoryEnum.Blog, UserActionActionEnum.Blog_ViewEntry, "blog entry registered only: " + blogEntryUrlName, false, blogId: blog.BlogId, blogEntryId: blogEntry.BlogEntryId);
				throw new LoginRequiredException(blog.Name, blogEntry.Name);
			}

			return blogEntry;
		}

		#endregion

		protected RouteValueDictionary RouteDataForIndex(string blogUrlName, string blogEntryUrlName)
		{
			RouteValueDictionary routeValues = new RouteValueDictionary();
			routeValues.Add("blogUrlName", blogUrlName);
			routeValues.Add("blogEntryUrlName", blogEntryUrlName);
			return routeValues;
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.BlogTheme.FolderName;
			}
		}

		protected void SetMetaTagsAndTheme(Blog blog)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			_metaDescriptionOverride = blog.MetaDescriptionOrSystemDefault(config);
			_metaKeywordsOverride = blog.MetaKeywordsOrSystemDefault(config);
			ViewData.Theme(blog.ThemeOrSystemDefault(config));
		}

		protected void SetMetaTagsAndTheme(BlogEntry entry)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			_metaDescriptionOverride = entry.MetaDescriptionOrSystemDefault(config);
			_metaKeywordsOverride = entry.MetaKeywordsOrSystemDefault(config);
			ViewData.Theme(entry.ThemeOrSystemDefault(config));
		}

		private class LoginRequiredException : Exception
		{
			public string BlogName { get; protected set; }
			public string BlogEntryName { get; protected set; }

			public LoginRequiredException(string blogName, string blogEntryName)
				: base(blogName)
			{
				this.BlogName = blogName;
				this.BlogEntryName = blogEntryName;
			}
		}

	}
}
