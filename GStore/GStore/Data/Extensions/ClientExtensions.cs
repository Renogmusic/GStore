using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;
using System.Web;

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

		public static string ClientVirtualDirectoryToMap(this Client client, string applicationPath)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			applicationPath = applicationPath.Trim('/');
			if (!string.IsNullOrEmpty(applicationPath))
			{
				applicationPath += "/";
			}
			return "/" + applicationPath + "Content/Clients/" + client.Folder.ToFileName();
		}

		public static void SetDefaultsForNew(this Client client, IGstoreDb db)
		{
			client.Name = "New Client";
			client.Order = 1000;
			if (!db.Clients.IsEmpty())
			{
				client.Order = db.Clients.All().Max(c => c.ClientId) + 10;
				if (db.Clients.All().Any(c => c.Name.ToLower() == client.Name.ToLower()))
				{
					bool nameIsDirty = true;
					int counter = 1;
					do
					{
						counter++;
						client.Name = "New Client " + counter;
						nameIsDirty = db.Clients.All().Any(c => c.Name.ToLower() == client.Name.ToLower());
					} while (nameIsDirty);
				}
			}

			client.Folder = client.Name;
			client.IsPending = false;
			client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			client.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			client.EnableNewUserRegisteredBroadcast = true;
			client.EnablePageViewLog = true;
			client.UseSendGridEmail = false;
			client.UseTwilioSms = false;
		}

		public static void SetDefaultsForNew(this PageTemplate pageTemplate, Client client)
		{
			pageTemplate.Order = 100;

			pageTemplate.Name = "New Page Template";
			if (client != null)
			{
				pageTemplate.Client = client;
				pageTemplate.ClientId = client.ClientId;
				pageTemplate.Order = client.PageTemplates.Count == 0 ? 100 : client.PageTemplates.Max(vl => vl.Order) + 10;
				if (client.PageTemplates.Any(pt => pt.Name.ToLower() == pageTemplate.Name.ToLower()))
				{
					bool nameIsDirty = true;
					int index = 1;
					do
					{
						index++;
						pageTemplate.Name = "New Page Template " + index;
						nameIsDirty = client.PageTemplates.Any(pt => pt.Name.ToLower() == pageTemplate.Name.ToLower());
					} while (nameIsDirty);
				}
			}
			pageTemplate.Description = pageTemplate.Name;
			pageTemplate.LayoutName = "Default";
			pageTemplate.ViewName = string.Empty;
			pageTemplate.IsPending = false;
			pageTemplate.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplate.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
		}

		public static void SetDefaultsForNew(this PageTemplateSection pageTemplateSection, PageTemplate pageTemplate)
		{
			pageTemplateSection.Name = "New Page Template Section";
			pageTemplateSection.Order = 100;
			if (pageTemplate != null)
			{
				pageTemplateSection.ClientId = pageTemplate.ClientId;
				pageTemplateSection.Client = pageTemplate.Client;
				pageTemplateSection.PageTemplateId = pageTemplate.PageTemplateId;
				pageTemplateSection.PageTemplate = pageTemplate;
				pageTemplateSection.Order = pageTemplate.Sections.Count == 0 ? 100 : pageTemplate.Sections.Max(vl => vl.Order) + 10;
				if (pageTemplate.Sections.Any(pts => pts.Name.ToLower() == pageTemplateSection.Name.ToLower()))
				{
					bool nameIsDirty = true;
					int index = 1;
					do
					{
						index++;
						pageTemplateSection.Name = "New Page Template Section " + index;
						nameIsDirty = pageTemplate.Sections.Any(pts => pts.Name.ToLower() == pageTemplateSection.Name.ToLower());
					} while (nameIsDirty);
				}
			}

			pageTemplateSection.Description = pageTemplateSection.Name;
			pageTemplateSection.IsPending = false;
			pageTemplateSection.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplateSection.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
		}

		public static void SetDefaultsForNew(this ValueList valueList, Client client)
		{
			valueList.Name = "New Value List";
			if (client != null)
			{
				valueList.ClientId = client.ClientId;
				valueList.Client = client;
				valueList.Order = valueList.Client.ValueLists.Count == 0 ? 100 : valueList.Client.ValueLists.Max(vl => vl.Order) + 10;
				if (client.ValueLists.Any(vl => vl.Name.ToLower() == valueList.Name.ToLower()))
				{
					bool nameIsDirty = true;
					int index = 1;
					do
					{
						index ++;
						valueList.Name = "New Value List " + index;
						nameIsDirty = client.ValueLists.Any(vl => vl.Name.ToLower() == valueList.Name.ToLower());
					} while (nameIsDirty);
				}
			}

			valueList.Description = valueList.Name;
			valueList.IsPending = false;
			valueList.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			valueList.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this ValueListItem valueListItem, ValueList valueList)
		{
			valueListItem.Name = "New Value";
			valueListItem.Order = 100;
			if (valueList != null)
			{
				valueListItem.ValueList = valueList;
				valueListItem.ValueListId = valueList.ValueListId;
				valueListItem.ClientId = valueList.ClientId;
				valueListItem.Client = valueList.Client;
				valueListItem.Order = (valueList.ValueListItems.Count == 0) ? 100 : valueList.ValueListItems.Max(vl => vl.Order) + 10;
				if (valueList.ValueListItems.Any(vl => vl.Name.ToLower() == valueListItem.Name.ToLower()))
				{
					int index = 1;
					do
					{
						index++;
						valueListItem.Name = "New Value " + index;

					} while (valueList.ValueListItems.Any(vl => vl.Name.ToLower() == valueListItem.Name.ToLower()));
				}
			}

			valueListItem.Description = valueListItem.Name;
			valueListItem.IsPending = false;
			valueListItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			valueListItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this WebForm webForm, Client client)
		{

			webForm.Order = 100;
			if (client != null)
			{
				webForm.Client = client;
				webForm.ClientId = client.ClientId;
				webForm.Order = client.WebForms.Count == 0 ? 100 : client.WebForms.Max(wf => wf.Order) + 10;
				webForm.Name = "New Web Form";
				if (client.WebForms.Any(wf => wf.Name.ToLower() == webForm.Name.ToLower()))
				{
					bool nameIsDirty = true;
					int index = 1;
					do
					{
						index++;
						webForm.Name = "New Web Form " + index;
						webForm.Description = "New Web Form " + index;
						nameIsDirty = client.WebForms.Any(wf => wf.Name.ToLower() == webForm.Name.ToLower());
					} while (nameIsDirty);

				}
			}
			else
			{
				webForm.Name = "New Web Form";
			}

			webForm.DisplayTemplateName = "WebForm";
			webForm.LabelMdColSpan = 2;
			webForm.FieldMdColSpan = 10;
			webForm.SubmitButtonClass = "btn btn-default";
			webForm.SubmitButtonText = "Submit";
			webForm.DisplayTemplateName = "WebForm";
			webForm.IsPending = false;
			webForm.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			webForm.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			webForm.Description = webForm.Name;
		}

		public static void SetDefaultsForNew(this WebFormField webFormField, WebForm webForm)
		{
			if (webForm != null)
			{
				webFormField.WebForm = webForm;
				webFormField.WebFormId = webForm.WebFormId;
				webFormField.ClientId = webForm.ClientId;
				webForm.Order = webForm.WebFormFields.Count == 0 ? 100 : webForm.WebFormFields.Max(wf => wf.Order) + 10;
				webFormField.Name = "New Field";
				bool nameIsDirty = webForm.WebFormFields.Any(wf => wf.Name.ToLower() == webFormField.Name.ToLower());
				int counter = 1;
				do
				{
					counter++;
					webFormField.Name = "New Field " + counter;
					nameIsDirty = webForm.WebFormFields.Any(wf => wf.Name.ToLower() == webFormField.Name.ToLower());
				} while (nameIsDirty);

				webFormField.LabelText = webFormField.Name;
				webFormField.Description = webFormField.Name;
			}
			webFormField.DataType = GStoreValueDataType.SingleLineText;
			webFormField.DataTypeString = webFormField.DataType.ToDisplayName();
			webFormField.IsPending = false;
			webFormField.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			webFormField.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this Theme theme, Client client)
		{
			if (client != null)
			{
				theme.Client = client;
				theme.ClientId = client.ClientId;
				theme.Order = client.Themes.Count == 0 ? 100 : client.Themes.Max(wf => wf.Order) + 10;
				theme.Name = "New Theme";
				bool nameIsDirty = client.Themes.Any(t => t.Name.ToLower() == theme.Name.ToLower());
				if (nameIsDirty)
				{
					int index = 1;
					do
					{
						index++;
						theme.Name = "New Theme " + index;
						nameIsDirty = client.Themes.Any(t => t.Name.ToLower() == theme.Name.ToLower());
					} while (nameIsDirty);
				}
			}
			else
			{
				theme.Name = "New Theme";
				theme.Order = 100;
			}
			theme.Description = theme.Name;
			theme.IsPending = false;
			theme.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			theme.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this Page page, StoreFrontConfiguration storeFrontConfig)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			StoreFront storeFront = storeFrontConfig.StoreFront;

			string pageName = "New Page";
			string url = "/NewUrl";
			int order = 1000;

			if (storeFront.Pages != null && storeFront.Pages.Count != 0)
			{
				order = storeFront.Pages.Max(pg => pg.Order) + 10;

				if (storeFront.Pages.Any(pg => pg.Name.ToLower() == pageName.ToLower()))
				{
					int index = 1;
					do
					{
						index++;
						pageName = "New Page " + index;
					} while (storeFront.Pages.Any(pg => pg.Name.ToLower() == pageName.ToLower()));
				}

				if (storeFront.Pages.Any(pg => pg.Url.ToLower() == url.ToLower()))
				{
					int index = 1;
					do
					{
						index++;
						url = "/NewUrl" + index;
					} while (storeFront.Pages.Any(pg => pg.Url.ToLower() == url.ToLower()));
				}
			}

			page.ClientId = storeFront.ClientId;
			page.Client = storeFront.Client;
			page.StoreFrontId = storeFront.StoreFrontId;
			page.StoreFront = storeFront;
			page.ThemeId = storeFrontConfig.DefaultNewPageThemeId;
			page.PageTitle = pageName;
			page.Name = pageName;
			page.Url = url;
			page.Order = order;
			page.IsPending = false;
			page.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			page.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this NavBarItem navBarItem, StoreFront storeFront)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			navBarItem.ClientId = storeFront.ClientId;
			navBarItem.Client = storeFront.Client;
			navBarItem.StoreFrontId = storeFront.StoreFrontId;
			navBarItem.StoreFront = storeFront;
			if (storeFront.NavBarItems == null || storeFront.NavBarItems.Count == 0)
			{
				navBarItem.Name = "New Menu Item";
				navBarItem.Order = 100;
			}
			else
			{
				navBarItem.Order = (storeFront.NavBarItems.Max(nb => nb.Order) + 10);
				navBarItem.Name = "New Menu Item " + navBarItem.Order;
			}
			navBarItem.ForRegisteredOnly = false;
			navBarItem.IsPage = true;
			navBarItem.OpenInNewWindow = false;
			navBarItem.UseDividerAfterOnMenu = true;
			navBarItem.IsPending = false;
			navBarItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			navBarItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);

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

		/// <summary>
		/// Returns a Select List for MVC. Return type is IEnumerable SelectListItem
		/// returns active records, and the currently selected value even if inactive
		/// </summary>
		/// <param name="pageTemplates"></param>
		/// <param name="selectedPageTemplateId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> ToSelectList(this IQueryable<ValueList> valueLists, int? selectedValueListId)
		{
			IQueryable<ValueList> query = valueLists.WhereIsActiveOrSelected(selectedValueListId);
			IOrderedQueryable<ValueList> orderedQuery = query.ApplyDefaultSort();

			int valueListId = selectedValueListId ?? 0;

			IEnumerable<SelectListItem> items = orderedQuery.Select(vl => new SelectListItem
			{
				Value = vl.ValueListId.ToString(),
				Text = (vl.ValueListId == valueListId ? "[SELECTED] " : string.Empty) + vl.Name + " [" + vl.ValueListId + "]" + (vl.IsActiveDirect() ? string.Empty : " [INACTIVE]"),
				Selected = vl.ValueListId == valueListId
			});

			return items;
		}

		public static IEnumerable<SelectListItem> ToSelectListWithNull(this IQueryable<ValueList> valueLists, int? selectedValueListId, string nullString = "(no value list)")
		{
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem() { Value = "", Text = nullString, Selected = (!selectedValueListId.HasValue) });
			items.AddRange(valueLists.ToSelectList(selectedValueListId));

			return items;
		}

		/// <summary>
		/// Returns a Select List for MVC. Return type is IEnumerable SelectListItem
		/// returns active records only
		/// </summary>
		/// <param name="pageTemplates"></param>
		/// <param name="selectedPageTemplateId"></param>
		/// <returns></returns>
		public static List<SelectListItem> ToSelectList(this ValueList valueList, int? selectedValueListItemId)
		{
			if (!valueList.IsActiveBubble())
			{
				return new List<SelectListItem>();
			}

			IOrderedQueryable<ValueListItem> orderedListItems = valueList.ValueListItems.AsQueryable().WhereIsActive().ApplyDefaultSort();

			int valueListItemId = selectedValueListItemId ?? 0;

			List<SelectListItem> items = orderedListItems.Select(vl => new SelectListItem
			{
				Value = vl.ValueListItemId.ToString(),
				Text = (vl.ValueListItemId == valueListItemId ? "[SELECTED] " : string.Empty) + vl.Name,
				Selected = vl.ValueListItemId == valueListItemId
			}).ToList();

			return items;
		}

		public static List<SelectListItem> ToSelectListWithNull(this ValueList valueList, int? selectedValueListItemId, string nullString = "(select a value)")
		{
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem() { Value = "", Text = nullString, Selected = (!selectedValueListItemId.HasValue) });
			items.AddRange(valueList.ToSelectList(selectedValueListItemId));

			return items;
		}


		/// <summary>
		/// Returns a Select List for MVC. Return type is IEnumerable SelectListItem
		/// returns active records, and the currently selected value even if inactive
		/// </summary>
		/// <param name="pageTemplates"></param>
		/// <param name="selectedPageTemplateId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> ToSelectList(this IQueryable<WebForm> webForms, int? selectedWebFormId)
		{
			IQueryable<WebForm> query = webForms.WhereIsActiveOrSelected(selectedWebFormId);
			IOrderedQueryable<WebForm> orderedQuery = query.ApplyDefaultSort();

			int webFormId = selectedWebFormId ?? 0;

			IEnumerable<SelectListItem> items = orderedQuery.Select(t => new SelectListItem
			{
				Value = t.WebFormId.ToString(),
				Text = (t.WebFormId == webFormId ? "[SELECTED] " : string.Empty) + t.Name + " [" + t.WebFormId + "]" + (t.IsActiveDirect() ? string.Empty : " [INACTIVE]"),
				Selected = t.WebFormId == webFormId
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
		public static IEnumerable<SelectListItem> ToSelectListWithNull(this IQueryable<WebForm> webForms, int? selectedWebFormId, string nullString = "(no web form)")
		{
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem() { Value = "", Text = nullString, Selected = (!selectedWebFormId.HasValue) });
			items.AddRange(webForms.ToSelectList(selectedWebFormId));

			return items;
		}


		/// <summary>
		/// Returns a Select List for MVC. Return type is IEnumerable SelectListItem
		/// returns active records, and the currently selected value even if inactive
		/// </summary>
		/// <param name="pageTemplates"></param>
		/// <param name="selectedPageTemplateId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> ToSelectList(this IQueryable<Page> pages, int? selectedPageId)
		{
			IQueryable<Page> query = pages.WhereIsActiveOrSelected(selectedPageId);
			IOrderedQueryable<Page> orderedQuery = query.ApplyDefaultSort();

			int pageId = selectedPageId ?? 0;

			IEnumerable<SelectListItem> items = orderedQuery.Select(t => new SelectListItem
			{
				Value = t.PageId.ToString(),
				Text = (t.PageId == pageId ? "[SELECTED] " : string.Empty) + t.Name + " [" + t.PageId + "]" + (t.IsActiveDirect() ? string.Empty : " [INACTIVE]"),
				Selected = t.PageId == pageId
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
		public static IEnumerable<SelectListItem> ToSelectListWithNull(this IQueryable<Page> pages, int? selectedPageId, string nullString = "(reload this page)")
		{
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem() { Value = "", Text = nullString, Selected = (!selectedPageId.HasValue) });
			items.AddRange(pages.ToSelectList(selectedPageId));

			return items;
		}

		/// <summary>
		/// Returns a Select List for MVC. Return type is IEnumerable SelectListItem
		/// returns active records, and the currently selected value even if inactive
		/// </summary>
		/// <param name="pageTemplates"></param>
		/// <param name="selectedPageTemplateId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> ToSelectList(this IQueryable<NavBarItem> navBarItems, int? selectedNavBarItemId)
		{
			IQueryable<NavBarItem> query = navBarItems.WhereIsActiveOrSelected(selectedNavBarItemId);
			IOrderedQueryable<NavBarItem> orderedQuery = query.ApplyDefaultSort();

			int navBarItemId = selectedNavBarItemId ?? 0;

			IEnumerable<SelectListItem> items = orderedQuery.Select(t => new SelectListItem
			{
				Value = t.NavBarItemId.ToString(),
				Text = (t.NavBarItemId == navBarItemId ? "[SELECTED] " : string.Empty) + t.Name + " [" + t.NavBarItemId + "]" + (t.IsActiveDirect() ? string.Empty : " [INACTIVE]"),
				Selected = t.NavBarItemId == navBarItemId
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
		public static IEnumerable<SelectListItem> ToSelectListWithNull(this IQueryable<NavBarItem> navBarItems, int? selectedNavBarItemId, string nullString = "(no parent - top level item)")
		{
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem() { Value = "", Text = nullString, Selected = (!selectedNavBarItemId.HasValue) });
			items.AddRange(navBarItems.ToSelectList(selectedNavBarItemId));

			return items;
		}

	}
}