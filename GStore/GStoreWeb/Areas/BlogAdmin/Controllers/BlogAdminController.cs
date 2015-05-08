using System;
using System.Linq;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Identity;
using GStoreData.Models;

namespace GStoreWeb.Areas.BlogAdmin.Controllers
{
	public class BlogAdminController : AreaBaseController.BlogAdminAreaBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Admin_BlogAdminArea)]
        public ActionResult Index()
        {
			//need permission check for index/manager

			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null && !Session.BlogAdminVisitLogged())
			{
				profile.LastBlogAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
				Session.BlogAdminVisitLogged(true);
			}

			return View("Index", this.BlogAdminViewModel);
        }

		public ActionResult Details(int blogId)
		{
			//need permissions checks

			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.BlogId == blogId).Single();

			return View("Details", blog);

		}

		public ActionResult Create()
		{
			//need permissions checks

			Blog newBlog = GStoreDb.Blogs.Create();

			newBlog.SetDefaultsForNew(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);

			return View("Create", newBlog);
		}

		public ActionResult Edit(int blogId)
		{
			//need permissions checks

			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.BlogId == blogId).Single();

			return View("Details", blog);
		}

		public ActionResult Delete(int blogId)
		{
			//need permissions checks

			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.BlogId == blogId).Single();

			return View("Delete", blog);
		}

		public ActionResult EntryIndex(int blogId)
		{
			//need permissions checks

			this.BlogAdminViewModel.FilterBlogId = blogId;
			return View("EntryIndex", this.BlogAdminViewModel);
		}

		public ActionResult EntryDetails(int blogId, int blogEntryId)
		{
			//need permissions checks

			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.BlogId == blogId).Single();
			BlogEntry entry = blog.BlogEntries.Where(be => be.BlogEntryId == blogEntryId).Single();

			return View("EntryDetails", entry);

		}

		public ActionResult EntryCreate(int blogId)
		{

			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.BlogId == blogId).Single();
			BlogEntry newEntry = GStoreDb.BlogEntries.Create();
			newEntry.SetDefaultsForNew(blog, CurrentUserProfileOrThrow);

			return View("EntryCreate", newEntry);
		}

		public ActionResult EntryEdit(int blogId, int blogEntryId)
		{
			//need permissions checks

			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.BlogId == blogId).Single();
			BlogEntry entry = blog.BlogEntries.Where(be => be.BlogEntryId == blogEntryId).Single();
			this.BlogAdminViewModel.FilterBlogId = blogId;
			return View("EntryIndex", this.BlogAdminViewModel);
		}

		public ActionResult EntryDelete(int blogId, int blogEntryId)
		{
			//need permissions checks

			Blog blog = CurrentStoreFrontOrThrow.Blogs.Where(b => b.BlogId == blogId).Single();
			BlogEntry entry = blog.BlogEntries.Where(be => be.BlogEntryId == blogEntryId).Single();
			this.BlogAdminViewModel.FilterBlogId = blogId;
			return View("EntryDelete", entry);
		}

	}

}
