using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using GStoreData.Models;

namespace GStoreData.AppHtmlHelpers
{
	public class PageVariableEditViewModel
	{
		public PageVariableEditViewModel()
		{
			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			this.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
		}

		public PageVariableEditViewModel(int pageId, PageTemplateSection pageTemplateSection, PageSection pageSectionOrNull)
		{
			this.PageId = pageId;

			if (pageTemplateSection == null)
			{
				throw new ArgumentNullException("pageTemplateSection");
			}
			this.PageTemplateSection = pageTemplateSection;
			if (pageTemplateSection != null)
			{
				this.PageTemplateSectionId = pageTemplateSection.PageTemplateSectionId;
			}
			this.PageSection = pageSectionOrNull;
			if (pageSectionOrNull != null)
			{
				this.PageSectionId = pageSectionOrNull.PageSectionId;
				this.StringValue = pageSectionOrNull.StringValue;
			}
			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			this.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
		}

		[Required]
		public int PageId { get; set; }

		public PageTemplateSection PageTemplateSection { get; protected set; }

		public PageSection PageSection { get; protected set; }

		[Required]
		public int? PageTemplateSectionId { get; set; }
		public int? PageSectionId { get; set; }

		public string StringValue { get; set; }

		/// <summary>
		/// Gets the current effective string value of this item.
		/// If 
		/// </summary>
		/// <returns></returns>
		public string CurrentValue(bool returnDefaultIfBlank)
		{
			if (this.PageTemplateSection == null)
			{
				throw new ArgumentNullException("this.pageTemplateSection");
			}

			if (!string.IsNullOrEmpty(this.StringValue))
			{
				return this.StringValue;
			}

			if (this.PageSection == null)
			{
				if (returnDefaultIfBlank)
				{
					return this.PageTemplateSection.DefaultStringValue;
				}
				return null;
			}
			if (string.IsNullOrEmpty(this.PageSection.StringValue))
			{
				if (returnDefaultIfBlank)
				{
					return this.PageTemplateSection.DefaultStringValue;
				}
				return null;
			}
			return this.PageSection.StringValue;
		}

		public bool CurrentValueIsDefault()
		{
			if (this.PageTemplateSection == null)
			{
				throw new ArgumentNullException("this.PageTemplateSection");
			}
			if (this.CurrentValue(true) == this.PageTemplateSection.DefaultStringValue)
			{
				return true;
			}
			return false;
		}


		public bool IsPending { get; set; }

		public DateTime EndDateTimeUtc { get; set; }

		public DateTime StartDateTimeUtc { get; set; }
	}
}
