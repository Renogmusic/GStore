using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Models.Extensions;
using GStore.Identity;

namespace GStore.Models.ViewModels
{
	public class PageViewModel
	{
		public PageViewModel(Page page, bool showEditPageLink, bool editMode)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page", "page must be specified for page view model");
			}

			this.Page = page;
			this.EditMode = editMode;
			this.ShowEditPageLink = showEditPageLink;
		}

		public bool EditMode { get; set; }

		public Page Page { get; set; }

		public bool ShowEditPageLink { get; set; }
	}
}