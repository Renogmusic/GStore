using GStore.Models;
using System.Collections.Generic;
using System.Linq;

namespace GStore.Data
{
	public static class PageExtensions
	{
		public static List<PageTemplateSection> MissingSections(this Page page)
		{
			List<PageTemplateSection> missingOptionalSections = new List<PageTemplateSection>();
			var query = page.PageTemplate.Sections.AsQueryable().OrderBy(pts => pts.Order);
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

		public static Page ResetContentToDefault(this Page page, IGstoreDb db)
		{
			List<PageSection> sections = page.Sections.ToList();
			foreach (PageSection section in sections)
			{
				db.PageSections.Delete(section);
			}

			db.Pages.Update(page);
			db.SaveChanges();

			return page;
		}

	}
}