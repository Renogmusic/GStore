using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GStore.Data
{
	public static class ClientExtensions
	{
		public static bool IsActiveDirect(this Client client)
		{
			return client.IsActiveDirect(DateTime.UtcNow);
		}

		public static bool IsActiveDirect(this Client client, DateTime dateTimeUtc)
		{
			if (client == null)
			{
				return false;
			}
			if ((!client.IsPending) && (client.StartDateTimeUtc < dateTimeUtc) && (client.EndDateTimeUtc > dateTimeUtc))
			{
				return true;
			}
			return false;
		}

		public static string ClientVirtualDirectoryToMap(this Client client)
		{
			return "/Content/Clients/" + System.Web.HttpUtility.UrlEncode(client.Folder);
		}

		public static void SetDefaultsForNew(this Client client)
		{
			client.Name = "New Client " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			client.Folder = client.Name;
			client.IsPending = false;
			client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			client.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			client.EnableNewUserRegisteredBroadcast = true;
			client.EnablePageViewLog = true;
			client.UseSendGridEmail = false;
			client.UseTwilioSms = false;
		}

		public static void SetDefaultsForNew(this PageTemplate pageTemplate, int? clientId)
		{
			if (clientId.HasValue)
			{
				pageTemplate.ClientId = clientId.Value;
			}
			pageTemplate.Name = "New Page Template";
			pageTemplate.Description = string.Empty;
			pageTemplate.LayoutName = "Bootstrap";
			pageTemplate.Order = 1000;
			pageTemplate.ViewName = string.Empty;
			pageTemplate.IsPending = false;
			pageTemplate.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplate.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
		}

		public static void SetDefaultsForNew(this PageTemplateSection pageTemplateSection, PageTemplate pageTemplate)
		{
			pageTemplateSection.Name = "New Page Template Section";
			pageTemplateSection.Order = 1000;
			pageTemplateSection.PageTemplateId = (pageTemplate == null ? 0 : pageTemplate.PageTemplateId);
			pageTemplateSection.PageTemplate = pageTemplate;
			pageTemplateSection.IsPending = false;
			pageTemplateSection.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplateSection.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			pageTemplateSection.ClientId = (pageTemplate == null ? 0 : pageTemplate.ClientId);
		}

		public static void SetDefaultsForNew(this ValueList valueList, int? clientId)
		{
			valueList.AllowDelete = true;
			valueList.AllowEdit = true;
			valueList.IsMultiSelect = true;
			if (clientId.HasValue)
			{
				valueList.ClientId = clientId.Value;
			}
			valueList.IsPending = false;
			valueList.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			valueList.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this ValueListItem valueListItem, ValueList valueList)
		{
			if (valueList != null)
			{
				valueListItem.ValueList = valueList;
				valueListItem.ValueListId = valueList.ValueListId;
				valueListItem.ClientId = valueList.ClientId;
			}

			valueListItem.IsPending = false;
			valueListItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			valueListItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this Theme theme, int? clientId)
		{
			if (clientId.HasValue)
			{
				theme.ClientId = clientId.Value;
			}
			theme.Name = "New Theme " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			theme.Order = 1000;
			theme.IsPending = false;
			theme.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			theme.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}


		/// <summary>
		/// Returns a Select List for MVC. Return type is IEnumerable SelectListItem
		/// returns active records, and the currently selected value even if inactive
		/// </summary>
		/// <param name="pageTemplates"></param>
		/// <param name="selectedPageTemplateId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> ToSelectList(this IQueryable<PageTemplate> pageTemplates, int? selectedPageTemplateId)
		{
			IQueryable<PageTemplate> query = pageTemplates.WhereIsActiveOrSelected(selectedPageTemplateId);
			IOrderedQueryable<PageTemplate> orderedQuery = query.ApplySort(null, null, null);

			int templateId = selectedPageTemplateId ?? 0;

			IEnumerable<SelectListItem> items = orderedQuery.Select(pg => new SelectListItem
			{
				Value = pg.PageTemplateId.ToString(),
				Text = (pg.PageTemplateId == templateId ? "[SELECTED] " : string.Empty) + pg.Name + " [" + pg.PageTemplateId + "]" + (pg.IsActiveDirect() ? string.Empty : " [INACTIVE]"),
				Selected = pg.PageTemplateId == templateId
			});

			return items;
		}

		/// <summary>
		/// Returns a Select List for MVC. Return type is IEnumerable SelectListItem
		/// returns active records, and the currently selected value even if inactive
		/// </summary>
		/// <param name="pageTemplates"></param>
		/// <param name="selectedPageTemplateId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> ToSelectList(this IQueryable<Theme> themes, int? selectedThemeId)
		{
			IQueryable<Theme> query = themes.WhereIsActiveOrSelected(selectedThemeId);
			IOrderedQueryable<Theme> orderedQuery = query.ApplySort(null, null, null);

			int themeId = selectedThemeId ?? 0;

			IEnumerable<SelectListItem> items = orderedQuery.Select(t => new SelectListItem
			{
				Value = t.ThemeId.ToString(),
				Text = (t.ThemeId == themeId ? "[SELECTED] " : string.Empty) + t.Name + " [" + t.ThemeId + "]" + (t.IsActiveDirect() ? string.Empty : " [INACTIVE]"),
				Selected = t.ThemeId == themeId
			});

			return items;
		}


	}
}