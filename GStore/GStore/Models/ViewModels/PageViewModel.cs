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
		public PageViewModel(Page page, bool showEditPageLink, bool editMode, bool autoPost)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page", "page must be specified for page view model");
			}

			this.Page = page;
			this.EditMode = editMode;
			this.AutoPost = autoPost;
			this.ShowEditPageLink = showEditPageLink;
		}

		public Page Page { get; set; }

		[Display(Name = "Edit Mode")]
		public bool EditMode { get; set; }

		[Display(Name = "User Can Edit")]
		public bool ShowEditPageLink { get; set; }

		[Display(Name = "Auto-Post")]
		public bool AutoPost { get; set; }

	}
}