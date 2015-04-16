using System;
using System.Linq;
using System.Web.Mvc;
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

		public ActionResult Index(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return View("Index");
			}

			return ViewBlog(id);
		}

		#endregion

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.BlogTheme.FolderName;
			}
		}

		protected void SetMetaTags(ProductCategory category)
		{
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			_metaDescriptionOverride = category.MetaDescriptionOrSystemDefault(config);
			_metaKeywordsOverride = category.MetaKeywordsOrSystemDefault(config);
		}

		protected bool CheckBlogEnabled()
		{
			if (!Settings.AppEnableBlog)
			{
				AddUserMessage("Blog Unavailable", "Sorry, Blog is not available for this site.", UserMessageType.Danger);
				return false;
			}

			if (!CurrentStoreFrontConfigOrThrow.BlogEnabled)
			{
				AddUserMessage("Blog Unavailable", "Sorry, Blog is not available for this store.", UserMessageType.Danger);
				return false;
			}

			return true;
		}

	}
}
