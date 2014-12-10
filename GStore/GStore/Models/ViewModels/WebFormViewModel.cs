using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;
using GStore.Identity;

namespace GStore.Models.ViewModels
{
	public class WebFormViewModel
	{
		public WebFormViewModel(WebForm webForm, bool showEditWebFormLink, bool editMode, bool autoPost)
		{
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm", "Web Form must be specified for Web Form view model");
			}

			this.WebForm = webForm;
			this.EditMode = editMode;
			this.AutoPost = autoPost;
			this.ShowEditWebFormLink = showEditWebFormLink;
		}

		public WebForm WebForm { get; set; }

		[Display(Name = "Edit Mode")]
		public bool EditMode { get; set; }

		[Display(Name = "User Can Edit")]
		public bool ShowEditWebFormLink { get; set; }

		[Display(Name = "Auto-Post")]
		public bool AutoPost { get; set; }

	}
}