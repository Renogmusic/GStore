using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Models.Extensions;
using GStore.Identity;
using System.Web.Mvc;

namespace GStore.Models.ViewModels
{
	public class PageSectionEditViewModel
	{
		public PageSectionEditViewModel()
		{
			//model binder will use this option
			SetDefaults();
		}


		public PageSectionEditViewModel(PageTemplateSection pageTemplateSection, Page page, PageSection pageSection, int index)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page", "page must be specified and exist before editing page sections");
			}

			SetDefaults();
			this.PageTemplateSectionId = pageTemplateSection.PageTemplateSectionId;
			this.SectionName = pageTemplateSection.Name;
			this.PageId = page.PageId;
			this.Index = index;

			if (pageSection != null)
			{
				this.PageSectionId = pageSection.PageSectionId;
				this.HasPlainText = pageSection.HasPlainText;
				this.HasRawHtml = pageSection.HasRawHtml;
				this.Order = pageSection.Order;
				this.PageId = pageSection.PageId;
				this.PlainText = pageSection.PlainText;
				this.RawHtml = pageSection.RawHtml;
				this.StartDateTimeUtc = pageSection.StartDateTimeUtc;
				this.EndDateTimeUtc = pageSection.EndDateTimeUtc;
				this.IsPending = pageSection.IsPending;
			}
		}

		protected void SetDefaults()
		{
			//set defaults
			this.PageSectionId = null;
			this.HasPlainText = false;
			this.HasRawHtml = false;
			this.Order = 100;
			this.Index = 0;
			this.PlainText = string.Empty;
			this.RawHtml = string.Empty;
			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			this.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			this.IsPending = false;
		}


		[Editable(false)]
		public int PageTemplateSectionId { get; set; }

		[Editable(false)]
		public string SectionName { get; set; }

		[Editable(false)]
		public int PageId { get; set; }

		[Editable(false)]
		public int Index { get; set; }

		[Key]
		[Editable(false)]
		public int? PageSectionId { get; set; }

		[Display(Name="Use Text")]
		public bool HasPlainText { get; set; }

		[Display(Name = "Use HTML")]
		public bool HasRawHtml { get; set; }

		public int Order { get; set; }

		[Display(Name="Text")]
		[DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
		public string PlainText { get; set; }

		[AllowHtml]
		[Display(Name="HTML")]
		[DataType(System.ComponentModel.DataAnnotations.DataType.MultilineText)]
		public string RawHtml { get; set; }
		public DateTime StartDateTimeUtc { get; set; }
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Save as Draft")]
		public bool IsPending { get; set; }

	}
}