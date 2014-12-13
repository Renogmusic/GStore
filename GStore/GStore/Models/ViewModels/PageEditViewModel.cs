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

		public PageEditViewModel(Page page, bool isStoreAdminEdit = false, bool isReadOnly = false, bool isDeletePage = false, bool isCreatePage = false, string activeTab = "")
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			this.IsStoreAdminEdit = isStoreAdminEdit;
			this.IsReadOnly = isReadOnly;
			this.IsDeletePage = isDeletePage;
			this.IsCreatePage = isCreatePage;
			this.ActiveTab = activeTab;

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
			this.MetaApplicationName = page.MetaApplicationName;
			this.MetaApplicationTileColor = page.MetaApplicationTileColor;
			this.Name = page.Name;
			this.Order = page.Order;
			this.PageId = page.PageId;
			this.PageTemplateId = page.PageTemplateId;
			this.PageTemplateName = (page.PageTemplate == null ? null : page.PageTemplate.Name);
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
			this.WebFormId = page.WebFormId;
			this.WebFormEmailToAddress = page.WebFormEmailToAddress;
			this.WebFormEmailToName = page.WebFormEmailToName;
			this.WebFormSuccessPageId = page.WebFormSuccessPageId;
			this.WebFormThankYouTitle = page.WebFormThankYouTitle;
			this.WebFormThankYouMessage = page.WebFormThankYouMessage;
			this.WebFormProcessorType = page.WebFormProcessorType;
			this.WebFormProcessorTypeName = page.WebFormProcessorTypeName;
			this.WebFormSaveToDatabase = page.WebFormSaveToDatabase;

			this.PageTemplateSelectList = page.Client.PageTemplates.AsQueryable().ToSelectList(page.PageTemplateId).ToList();
			this.ThemeSelectList = page.Client.Themes.AsQueryable().ToSelectList(page.ThemeId).ToList();
			this.WebFormSelectList = page.Client.WebForms.AsQueryable().ToSelectListWithNull(page.WebFormId).ToList();
			this.WebFormSuccessPageSelectList = page.StoreFront.Pages.AsQueryable().ToSelectListWithNull(page.WebFormSuccessPageId).ToList();

			this.IsActiveBubble = page.IsActiveBubble();
			this.IsActiveDirect = page.IsActiveDirect();
		}

		public void FillListsIfEmpty(Client client, StoreFront storeFront)
		{
			if (this.PageTemplateSelectList == null)
			{
				this.PageTemplateSelectList = client.PageTemplates.AsQueryable().ToSelectList(this.PageTemplateId).ToList();
			}
			if (this.ThemeSelectList == null)
			{
				this.ThemeSelectList = client.Themes.AsQueryable().ToSelectList(this.ThemeId).ToList();
			}
			if (this.WebFormSelectList == null)
			{
				this.WebFormSelectList = client.WebForms.AsQueryable().ToSelectListWithNull(this.WebFormId).ToList();
			}
			if (this.WebFormSuccessPageSelectList == null)
			{
				this.WebFormSuccessPageSelectList = storeFront.Pages.AsQueryable().ToSelectListWithNull(this.WebFormSuccessPageId).ToList();
			}
		}

		[Display(Name = "Is Store Admin Edit")]
		[Editable(false)]
		public bool IsStoreAdminEdit { get; set; }

		[Display(Name = "Is Read Only")]
		[Editable(false)]
		public bool IsReadOnly { get; set; }

		[Display(Name = "Is Delete Page")]
		[Editable(false)]
		public bool IsDeletePage { get; set; }

		[Display(Name = "Is Create Page")]
		[Editable(false)]
		public bool IsCreatePage { get; set; }

		[Display(Name = "Page Is Active (including store front)")]
		[Editable(false)]
		public bool IsActiveBubble { get; set; }

		[Display(Name = "Page Is Active")]
		[Editable(false)]
		public bool IsActiveDirect { get; set; }

		[Display(Name = "Active Tab")]
		[Editable(false)]
		public string ActiveTab { get; set; }


		[Editable(false)]
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

		[Editable(false)]
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
		public List<SelectListItem> WebFormSelectList
		{
			get
			{
				return _webFormSelectList;
			}
			set
			{
				_webFormSelectList = value;
			}
		}
		protected List<SelectListItem> _webFormSelectList = null;

		[Editable(false)]
		public List<SelectListItem> WebFormSuccessPageSelectList
		{
			get
			{
				return _webFormSuccessPageSelectList;
			}
			set
			{
				_webFormSuccessPageSelectList = value;
			}
		}
		protected List<SelectListItem> _webFormSuccessPageSelectList = null;


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
		[MaxLength(200)]
		public string Name { get; set; }

		[Display(Name = "Page Template", Description = "Template Used for this page.  Changing the template will required you to edit the page content")]
		public int PageTemplateId { get; set; }

		[Display(Name = "Web Form", Description = "Web Form used for this page.")]
		public int? WebFormId { get; set; }

		[Display(Name = "Web Form Processor", Description = "Web Form Processor used for the form on this page.")]
		public Data.WebFormProcessorType WebFormProcessorType { get; set; }

		[Display(Name = "Web Form Processor Type Name", Description = "Web Form Processor used for the form on this page.")]
		[MaxLength(100)]
		public string WebFormProcessorTypeName { get; set; }

		[Display(Name = "Web Form Save to Database", Description = "Check this box to automatically save form results to the database. \nIf unchecked, form will not be saved to the database..")]
		public bool WebFormSaveToDatabase { get; set; }

		[Display(Name = "Web Form Success Page", Description = "Page to send the user to after they submit the form. Leave blank to re-load this page.")]
		public int? WebFormSuccessPageId { get; set; }

		[Display(Name = "Web Form To Email Address", Description = "If using Email to send web form result, enter the email address here or leave it blank to send to the New User notification user.")]
		[MaxLength(200)]
		public string WebFormEmailToAddress { get; set; }

		[Display(Name = "Web Form To Email Name", Description = "If using Email to send web form result, enter the Name to send to here or leave it blank to send to the New User notification user with their name.")]
		[MaxLength(200)]
		public string WebFormEmailToName { get; set; }

		[Display(Name = "Web Form Thank You Message Title", Description = "Message Title for Form Submit Thank You Message. Variables allowed.")]
		[MaxLength(200)]
		public string WebFormThankYouTitle { get; set; }

		[AllowHtml]
		[Display(Name = "Web Form Thank You Message Body", Description = "Message Body for Form Submit Thank You Message. HTML and Variables allowed.")]
		[MaxLength(2000)]
		[DataType(DataType.Html)]
		public string WebFormThankYouMessage { get; set; }

		[Display(Name = "Index", Description = "Internal Index number used to sort this page in reports and lists on this site")]
		public int Order { get; set; }

		[Display(Name = "Url ( starts with / for home, example: /Contact )", ShortName="Url", Description="Url of this page. Be sure to match this up with navigation bars or links into the site.")]
		[Required]
		[MaxLength(250)]
		public string Url { get; set; }

		[Display(Name = "Registered Users Only", Description="Check this box below to allow Only Registered Users to view this page")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "Page Title in Browser and Favorites", Description="Page title in the browser, also used when page is added to bookmarks or home screen" )]
		[MaxLength(200)]
		public string PageTitle { get; set; }

		[Display(Name = "Meta Tag Keywords", Description = "Meta Tag Keywords for search engines to index this page.\nLeave this blank to use your Store Front default.")]
		[MaxLength(2000)]
		public string MetaKeywords { get; set; }

		[Display(Name = "Meta Tag Description", Description = "Meta Tag Description for search engines to describe this page.\nLeave this blank to use your Store Front default.")]
		[MaxLength(2000)]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Application Name", Description = "Meta Tag Application Name for Pinned Sites.\nLeave this blank to use your Store Front default.")]
		[MaxLength(200)]
		public string MetaApplicationName { get; set; }

		[Display(Name = "Meta Tag Application Tile Color", Description = "Meta Tag Application Tile Color for pinned sites.\nLeave this blank to use your Store Front default.")]
		[MaxLength(20)]
		public string MetaApplicationTileColor { get; set; }

		[AllowHtml]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Page Top Script Tags", Description = "Scripts tags on the top portion of the page. Be careful here because bad HTML here can cause problems with your page. Tags should include the <script> and </script> elements.\nLeave blank to use your Store Front default.")]
		[MaxLength(10000)]
		public string BodyTopScriptTag { get; set; }

		[AllowHtml]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Page Bottom Script Tags", Description = "Scripts tags on the bottom portion of the page. Be careful here because bad HTML here can cause problems with your page. Tags should include the <script> and </script> elements.\nLeave blank to use your Store Front default.")]
		[MaxLength(10000)]
		public string BodyBottomScriptTag { get; set; }

		[Display(Name = "Theme", Description = "Theme for this page.  Note: after you save this change, click the Refresh button to see the new theme.")]
		public int ThemeId { get; set; }

		[Display(Name = "Inactive", Description="Check this box to Inactivate a page immediately. \nIf checked, this page cannot be viewed from the site and can only be edited in the Store Admin section.")]
		public bool IsPending { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this page to be ACTIVE on.\nIf this date is in the future, your page will be inactive and can only be edited in the Store Admin section. \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this page to go INACTIVE on.\nIf this date is in the past, your page will be inactive and can only be edited in the Store Admin section. \nExample: 12/31/2199 11:59 PM")]
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