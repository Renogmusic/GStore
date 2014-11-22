using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Models.Extensions
{
	public static class PageExtensions
	{
		public static List<PageTemplateSection> MissingRequiredSections(this Page page)
		{
			List<PageTemplateSection> missingRequiredSections = new List<PageTemplateSection>();
			var query = page.PageTemplate.Sections.AsQueryable().Where(pts => pts.IsRequired).OrderBy(pts => pts.Order);
			List<PageTemplateSection> requiredSections = query.ToList();
			foreach (PageTemplateSection item in requiredSections)
			{
				if (!page.Sections.AsQueryable().WhereIsActive().Any(s => s.PageTemplateSectionId == item.PageTemplateSectionId))
				{
					missingRequiredSections.Add(item);
				}
			}
			return missingRequiredSections;
		}

		public static List<PageTemplateSection> MissingOptionalSections(this Page page)
		{
			List<PageTemplateSection> missingOptionalSections = new List<PageTemplateSection>();
			var query = page.PageTemplate.Sections.AsQueryable().Where(pts => !pts.IsRequired).OrderBy(pts => pts.Order);
			List<PageTemplateSection> optionalSections = query.ToList();
			foreach (PageTemplateSection item in optionalSections)
			{
				if (!page.Sections.AsQueryable().WhereIsActive().Any(s => s.PageTemplateSectionId == item.PageTemplateSectionId))
				{
					missingOptionalSections.Add(item);
				}
			}
			return missingOptionalSections;
		}

	}
}