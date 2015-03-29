using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class PageSectionEditViewModel
	{
		public PageSectionEditViewModel()
		{
			//model binder will use this option
			SetDefaults(null);
		}


		public PageSectionEditViewModel(PageTemplateSection pageTemplateSection, Page page, PageSection pageSection, int index, bool autoSubmit)
		{
			if (pageTemplateSection == null)
			{
				throw new ArgumentNullException("pageTemplateSection");
			}
			if (page == null)
			{
				throw new ArgumentNullException("page", "page must be specified and exist before editing page sections");
			}
			if (index < 1)
			{
				throw new ArgumentOutOfRangeException("index", "Index cannot be 0, it starts from 1 and up");
			}

			SetDefaults(pageTemplateSection);
			this.PageTemplateSectionId = pageTemplateSection.PageTemplateSectionId;
			this.SectionName = pageTemplateSection.Name;
			this.DefaultRawHtmlValue = pageTemplateSection.DefaultRawHtmlValue;
			this.PageId = page.PageId;
			this.Index = index;
			this.AutoSubmit = autoSubmit;

			if (pageSection != null)
			{
				this.PageSectionId = pageSection.PageSectionId;
				this.UseDefaultFromTemplate = pageSection.UseDefaultFromTemplate;
				this.HasNothing = pageSection.HasNothing;
				this.HasPlainText = pageSection.HasPlainText;
				this.HasRawHtml = pageSection.HasRawHtml;
				this.Order = pageSection.Order;
				this.PageId = pageSection.PageId;
				this.PlainText = pageSection.PlainText;
				this.RawHtml = pageSection.RawHtml;
				this.StartDateTimeUtc = pageSection.StartDateTimeUtc;
				this.EndDateTimeUtc = pageSection.EndDateTimeUtc;
				this.IsPending = pageSection.IsPending;
				this.UpdateDateTimeUtc = pageSection.UpdateDateTimeUtc;
				this.UpdatedBy = pageSection.UpdatedBy;
			}
		}

		protected void SetDefaults(PageTemplateSection pageTemplateSection)
		{
			//set defaults
			this.PageSectionId = null;
			this.UseDefaultFromTemplate = true;
			if (pageTemplateSection != null)
			{
				this.RawHtml = pageTemplateSection.DefaultRawHtmlValue;
				this.DefaultRawHtmlValue = pageTemplateSection.DefaultRawHtmlValue;
			}
			this.HasPlainText = false;
			this.HasRawHtml = false;
			this.HasNothing = false;
			this.Order = 100;
			this.Index = 0;
			this.PlainText = string.Empty;
			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			this.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			this.IsPending = false;
		}


		[Editable(false)]
		public int PageTemplateSectionId { get; set; }

		[Editable(false)]
		public string SectionName { get; set; }

		[Editable(false)]
		public string DefaultRawHtmlValue { get; set; }
	
		[Editable(false)]
		public int PageId { get; set; }

		[Editable(false)]
		public int Index { get; set; }

		[Editable(false)]
		public bool AutoSubmit { get; set; }

		public bool IsAutoPosted { get; set; }

		[Key]
		[Editable(false)]
		public int? PageSectionId { get; set; }

		[Display(Name="Empty")]
		public bool HasNothing { get; set; }

		[Display(Name = "Use Default")]
		public bool UseDefaultFromTemplate { get; set; }

		[Display(Name = "Use Text")]
		public bool HasPlainText { get; set; }

		[Display(Name = "Use HTML")]
		public bool HasRawHtml { get; set; }

		public int Order { get; set; }

		[Display(Name="Text")]
		[DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
		public string PlainText { get; set; }

		[AllowHtml]
		[Display(Name="HTML")]
		[DataType(DataType.Html)]
		public string RawHtml { get; set; }

		public DateTime StartDateTimeUtc { get; set; }
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Inactive")]
		public bool IsPending { get; set; }

		public UserProfile UpdatedBy { get; protected set; }

		public DateTime UpdateDateTimeUtc { get; protected set; }
	}
}