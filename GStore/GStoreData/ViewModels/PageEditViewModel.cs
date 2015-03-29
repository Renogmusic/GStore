﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public struct PageVariableData
	{
		public PageVariableData(PageTemplateSection pageTemplateSection, PageSection pageSectionOrNull)
		{
			if (pageTemplateSection == null)
			{
				throw new ArgumentNullException("pageTemplateSection");
			}

			this.PageTemplateSection = pageTemplateSection;
			this.PageSection = pageSectionOrNull;
		}

		/// <summary>
		/// Page Template section for variable field data
		/// </summary>
		public PageTemplateSection PageTemplateSection;

		/// <summary>
		/// Page section for variable value data. May be null if no value exists
		/// </summary>
		public PageSection PageSection;
	}

	public class PageEditViewModel : IValidatableObject
	{
		public PageEditViewModel()
		{
			//model binder will use this option
			LoadValues(null);
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
			this.CreateMenuItemWithPage = isCreatePage;
			LoadValues(page);
		}

		protected void LoadValues(Page page)
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
			this.OriginalPageUrl = page.Url;
			this.OriginalPageTemplate = page.PageTemplate;
			this.OriginalTheme = page.Theme;
			this.OriginalWebForm = page.WebForm;
			this.Order = page.Order;
			this.PageId = page.PageId;
			this.PageTemplateId = page.PageTemplateId;
			this.PageTemplateName = (page.PageTemplate == null ? null : page.PageTemplate.Name);
			this.PageTitle = page.PageTitle;
			this.ForAnonymousOnly = page.ForAnonymousOnly;
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
			this.StoreFront = page.StoreFront;
			this.StoreFrontId = page.StoreFrontId;
			this.WebFormId = page.WebFormId;
			this.WebFormEmailToAddress = page.WebFormEmailToAddress;
			this.WebFormEmailToName = page.WebFormEmailToName;
			this.WebFormSuccessPageId = page.WebFormSuccessPageId;
			this.WebFormThankYouTitle = page.WebFormThankYouTitle;
			this.WebFormThankYouMessage = page.WebFormThankYouMessage;
			this.WebFormSaveToDatabase = page.WebFormSaveToDatabase;
			this.WebFormSaveToFile = page.WebFormSaveToFile;
			this.WebFormSendToEmail = page.WebFormSendToEmail;
			this.WebFormSaveToDatabase = page.WebFormSaveToDatabase;

			this.PageTemplateSelectList = page.Client.PageTemplates.AsQueryable().ToSelectList(page.PageTemplateId).ToList();
			this.ThemeSelectList = page.Client.Themes.AsQueryable().ToSelectList(page.ThemeId).ToList();
			this.WebFormSelectList = page.Client.WebForms.AsQueryable().ToSelectListWithNull(page.WebFormId).ToList();
			this.WebFormSuccessPageSelectList = page.StoreFront.Pages.AsQueryable().ToSelectListWithNull(page.WebFormSuccessPageId).ToList();

			if (page.PageTemplate != null)
			{
				List<PageTemplateSection> variableTemplateSections = page.PageTemplate.Sections.Where(pts => pts.IsVariable).AsQueryable().ApplyDefaultSort().ToList();
				this.Variables = variableTemplateSections.Select(pts => new PageVariableEditViewModel(page.PageId, pts, pts.PageSections.AsQueryable().WhereIsActive().FirstOrDefault(ps => ps.PageId == page.PageId))).ToList();
			}

			this.IsActiveBubble = page.IsActiveBubble();
			this.IsActiveDirect = page.IsActiveDirect();
		}

		public void FillListsIfEmpty(Client client, StoreFront storeFront)
		{
			if (this.StoreFront == null)
			{
				this.StoreFront = storeFront;
				this.StoreFrontId = storeFront.StoreFrontId;
			}
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
		public bool IsReadOnly { get; protected set; }

		[Display(Name = "Is Delete Page")]
		[Editable(false)]
		public bool IsDeletePage { get; protected set; }

		[Display(Name = "Is Create Page")]
		[Editable(false)]
		public bool IsCreatePage { get; set; }

		[Display(Name = "Page Is Active (including store front)")]
		[Editable(false)]
		public bool IsActiveBubble { get; protected set; }

		[Display(Name = "Page Is Active")]
		[Editable(false)]
		public bool IsActiveDirect { get; protected set; }

		[Display(Name = "Active Tab")]
		[Editable(false)]
		public string ActiveTab { get; set; }

		[Display(Name = "Add to Site Menu", Description="Check this box to add this page to the site menu.")]
		public bool CreateMenuItemWithPage { get; set; }

		[Editable(false)]
		[Display(Name = "Store Front")]
		public StoreFront StoreFront { get; protected set;}

		[Editable(false)]
		[Display(Name = "Store Front Id")]
		public int StoreFrontId { get; protected set; }

		[Display(Name = "Original Page Url")]
		public string OriginalPageUrl { get; protected set; }

		[Display(Name = "Page Template", Description = "Template Used for this page.  Changing the template will required you to edit the page content")]
		public PageTemplate OriginalPageTemplate { get; protected set; }

		[Display(Name = "Theme", Description = "Template Used for this page.  Changing the template will required you to edit the page content")]
		public Theme OriginalTheme { get; protected set; }

		[Display(Name = "Web Form", Description = "Template Used for this page.  Changing the template will required you to edit the page content")]
		public WebForm OriginalWebForm { get; protected set; }

		[Key]
		[Editable(false)]
		[Display(Name = "Page Id", Description="Internal number to identity this page")]
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

		[Display(Name = "Web Form Save to Database", Description = "Check this box to automatically save form results to the database. \nIf unchecked, form will not be saved to the database.")]
		public bool WebFormSaveToDatabase { get; set; }

		[Display(Name = "Web Form Save to File", Description = "Check this box to save form results to the Store Front Forms folder. \nIf unchecked, form will not be saved to file.")]
		public bool WebFormSaveToFile { get; set; }

		[Display(Name = "Web Form Send To Email", Description = "Check this box to send form results to email. \n If Email address is blank, form will be sent to the store front Registered Notify user.")]
		public bool WebFormSendToEmail { get; set; }

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

		[Display(Name = "Anonymous Users Only", Description = "Check this box below to allow Only Anonymous Users to view this page")]
		public bool ForAnonymousOnly { get; set; }

		[Display(Name = "Registered Users Only", Description = "Check this box below to allow Only Registered Users to view this page")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "Page Title in Browser and Favorites", Description = "Page title in the browser, also used when page is added to bookmarks or home screen")]
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
		[DataType(DataType.Html)]
		[Display(Name = "Page Top Script Tags", Description = "Scripts tags on the top portion of the page. Be careful here because bad HTML here can cause problems with your page. Tags should include the <script> and </script> elements.\nLeave blank to use your Store Front default.")]
		[MaxLength(10000)]
		public string BodyTopScriptTag { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
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

		[Editable(false)]
		public List<SelectListItem> PageTemplateSelectList
		{
			get
			{
				return _pageTemplateSelectList;
			}
			protected set
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
			protected set
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
			protected set
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
			protected set
			{
				_webFormSuccessPageSelectList = value;
			}
		}
		protected List<SelectListItem> _webFormSuccessPageSelectList = null;

		public List<PageVariableEditViewModel> Variables
		{
			get
			{
				return _variables;
			}
			set
			{
				_variables = value;
			}
		}
		protected List<PageVariableEditViewModel> _variables = null;

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> result = new List<ValidationResult>();

			if (this.ForAnonymousOnly && this.ForRegisteredOnly)
			{
				string registeredLabel = this.GetDisplayName("ForRegisteredOnly");
				string anonymousLabel = this.GetDisplayName("ForAnonymousOnly");
				string[] memberNames = { "ForRegisteredOnly", "ForAnonymousOnly" };
				ValidationResult item = new ValidationResult("You cannot select both " + registeredLabel + " and " + anonymousLabel + " you must choose one or none.", memberNames);
				result.Add(item);
			}

			if (this.WebFormId.HasValue && !(this.WebFormSaveToDatabase || this.WebFormSaveToFile || this.WebFormSendToEmail))
			{
				string[] memberNames = { "WebFormId" };
				string label1 = this.GetDisplayName("WebFormSaveToDatabase");
				string label2 = this.GetDisplayName("WebFormSaveToFile");
				string label3 = this.GetDisplayName("WebFormSendToEmail");
				ValidationResult item = new ValidationResult("You must select one of: " + label1 + ", " + label2 + ", or " + label3 + " when using a Web Form", memberNames);
				result.Add(item);
			}

			return result;
		}
	}
}