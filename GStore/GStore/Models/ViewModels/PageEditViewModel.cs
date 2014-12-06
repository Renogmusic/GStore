using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;
using GStore.Identity;
using System.Web.Mvc;

namespace GStore.Models.ViewModels
{
	public class PageEditViewModel
	{
		public PageEditViewModel()
		{
			//model binder will use this option
			SetDefaults(null);
		}


		public PageEditViewModel(Page page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			SetDefaults(page);
		}

		protected void SetDefaults(Page page)
		{
			if (page == null)
			{
				this.IsPending = false;
				this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				this.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				return;
			}
			this.BodyBottomScriptTag = page.BodyBottomScriptTag;
			this.BodyTopScriptTag = page.BodyTopScriptTag;
			this.MetaDescription = page.MetaDescription;
			this.MetaKeywords = page.MetaKeywords;
			this.Name = page.Name;
			this.Order = page.Order;
			this.PageId = page.PageId;
			this.PageTemplateId = page.PageTemplateId;
			this.PageTemplateName = page.PageTemplate.Name;
			this.PageTitle = page.PageTitle;
			this.ForRegisteredOnly = page.ForRegisteredOnly;
			this.ThemeId = page.ThemeId;
			this.Url = page.Url;
			this.CreatedBy = page.CreatedBy;
			this.CreateDateTimeUtc = page.CreateDateTimeUtc;
			this.UpdatedBy = page.UpdatedBy;
			this.UpdateDateTimeUtc = page.UpdateDateTimeUtc;
			this.IsPending = page.IsPending;
			this.StartDateTimeUtc = page.StartDateTimeUtc;
			this.EndDateTimeUtc = page.EndDateTimeUtc;
			this.OriginalPageUrl = page.Url;

			this.PageTemplateSelectList = page.Client.PageTemplates.AsQueryable().ToSelectList(page.PageTemplateId).ToList();
			this.ThemeSelectList = page.Client.Themes.AsQueryable().ToSelectList(page.ThemeId).ToList();
		}

		public List<SelectListItem> PageTemplateSelectList
		{ 
			get
			{
				return _pageTemplateSelectList;
			}
			set
			{
				_pageTemplateSelectList = value;
			}
		}
		protected List<SelectListItem> _pageTemplateSelectList = null;

		public List<SelectListItem> ThemeSelectList
		{
			get
			{
				return _themeSelectList;
			}
			set
			{
				_themeSelectList = value;
			}
		}
		protected List<SelectListItem> _themeSelectList = null;


		[Editable(false)]
		[Display(Name = "Original Page Url")]
		public string OriginalPageUrl { get; set; }

		[Key]
		[Editable(false)]
		[Display(Name = "Page Id")]
		public int PageId { get; set; }

		[Editable(false)]
		[Display(Name = "Page Template Name")]
		public string PageTemplateName { get; set; }

		[Required]
		[Display(Name="Page Name", Description="Page name (for your reference in Reports)")]
		public string Name { get; set; }

		[Display(Name = "Page Template", Description = "Template Used for this page.  Changing the template will required you to edit the page content")]
		public int PageTemplateId { get; set; }

		[Display(Name = "Index", Description = "Internal Index number used to sort this page in reports and lists on this site")]
		public int Order { get; set; }

		[Display(Name = "Url ( starts with / for home, example: /Contact )", ShortName="Url", Description="Url of this page. Be sure to match this up with navigation bars or links into the site.")]
		[Required]
		public string Url { get; set; }

		[Display(Name = "Registered Users Only", Description="Check this box below to allow Only Registered Users to view this page")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "Page Title in Browser and Favorites", Description="Page title in the browser, also used when page is added to bookmarks or home screen" )]
		public string PageTitle { get; set; }

		[Display(Name = "Meta Tag Keywords", Description ="Meta Tag Keywords for search engines to index this page.")]
		public string MetaKeywords { get; set; }

		[Display(Name = "Meta Tag Description", Description ="Meta Tag Description for search engines to describe this page.")]
		public string MetaDescription { get; set; }

		[Display(Name = "Theme", Description ="Theme for this page.  Note: after you save this change, click the Refresh button to see the new theme.")]
		public int ThemeId { get; set; }

		[AllowHtml]
		[DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
		[Display(Name = "Page Top Script Tags", Description ="Scripts tags on the top portion of the page. Be careful here because bad HTML here can cause problems with your page. Tags should include the <script> and </script> elements")]
		public string BodyTopScriptTag { get; set; }

		[AllowHtml]
		[DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
		[Display(Name = "Page Bottom Script Tags", Description = "Scripts tags on the bottom portion of the page. Be careful here because bad HTML here can cause problems with your page. Tags should include the <script> and </script> elements")]
		public string BodyBottomScriptTag { get; set; }

		[Display(Name = "Save as Draft")]
		public bool IsPending { get; set; }

		[Required]
		[Display(Name = "Start Date")]
		public DateTime StartDateTimeUtc { get; set; }

		[Required]
		[Display(Name = "End Date")]
		public DateTime EndDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Created On")]
		public DateTime? CreateDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Created By")]
		public UserProfile CreatedBy { get; set; }

		[Editable(false)]
		[Display(Name = "Updated On")]
		public DateTime? UpdateDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Updated By")]
		public UserProfile UpdatedBy { get; set; }
	}
}