using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;
using GStore.Identity;

namespace GStore.Models.ViewModels
{
	public class PageViewModel
	{
		public PageViewModel(Page page, bool showEditPageLink, bool editMode, bool autoPost, bool forTemplateSyncOnly, int? pageTemplateIdForSync)
		{
			if (page == null && !forTemplateSyncOnly)
			{
				throw new ArgumentNullException("page", "page must be specified for page view model or forTemplateSyncOnly must be true");
			}
			if (forTemplateSyncOnly && !pageTemplateIdForSync.HasValue)
			{
				throw new ArgumentNullException("pageTemplateIdForSync", "PageTemplateIdForSync must be specified when forTemplateSyncOnly is true");
			}

			this.Page = page;
			this.EditMode = editMode;
			this.AutoPost = autoPost;
			this.ShowEditPageLink = showEditPageLink;
			this.ForTemplateSyncOnly = forTemplateSyncOnly;
			this.PageTemplateIdForSync = pageTemplateIdForSync;
		}

		public Page Page { get; set; }

		[Display(Name = "Edit Mode")]
		public bool EditMode { get; set; }

		[Display(Name = "User Can Edit")]
		public bool ShowEditPageLink { get; set; }

		[Display(Name = "Auto-Post")]
		public bool AutoPost { get; set; }

		[Display(Name = "For Sync Only")]
		public bool ForTemplateSyncOnly { get; set; }

		public int? PageTemplateIdForSync { get; set; }


	}
}