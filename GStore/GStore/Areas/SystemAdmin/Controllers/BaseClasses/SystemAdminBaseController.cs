using GStore.Areas.StoreAdmin.ViewModels;
using GStore.Controllers.BaseClass;
using GStore.Data;
using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;
using GStore.Identity;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace GStore.Areas.SystemAdmin.Controllers.BaseClasses
{
	[AuthorizeSystemAdmin]
	public abstract class SystemAdminBaseController : GStore.Controllers.BaseClass.BaseController
	{
		public SystemAdminBaseController(IGstoreDb dbContext): base(dbContext)
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = false;
			this._throwErrorIfStoreFrontNotFound = false;
		}

		public SystemAdminBaseController()
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = false;
			this._throwErrorIfStoreFrontNotFound = false;
		}

		protected Func<HtmlHelper, MvcHtmlString> BreadCrumbsFunc = null;

		public virtual MvcHtmlString Breadcrumbs(HtmlHelper htmlHelper)
		{
			if (BreadCrumbsFunc == null)
			{
				return TopBreadcrumb(htmlHelper, false);
			}
			return BreadCrumbsFunc(htmlHelper);
		}

		protected override string ThemeFolderName
		{
			get
			{
				return Settings.AppDefaultThemeFolderName;
			}
		}

		/// <summary>
		/// Returns the ClientId route value, includes -1 for all and 0 for null, and null if blank
		/// </summary>
		/// <returns></returns>
		public int? FilterClientIdRaw()
		{
			var clientId = RouteData.Values["ClientId"];
			if (clientId == null)
			{
				return null;
			}
			int value = 0;
			if (int.TryParse(clientId.ToString(), out value))
			{
				return value;
			}
			return null;
		}

		public int? FilterClientId()
		{
			int? value = FilterClientIdRaw();
			if (value.HasValue && value != 0 && value != -1)
			{
				return value;
			}
			return null;
		}

		public bool ClientIsFiltered()
		{
			int? filterValue = FilterClientIdRaw();
			if (!filterValue.HasValue)
			{
				//default filter for clientid = null
				return true;
			}
			if (filterValue.Value == -1)
			{
				return false;
			}
			return true;
		}

		public SelectList ClientFilterList()
		{
			int? clientId = FilterClientIdRaw();
			return ClientFilterListHelper(clientId, true, true, false);
		}

		public bool ShowAllClients()
		{
			if (FilterClientIdRaw().HasValue && (FilterClientIdRaw().Value == -1))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns a SelectList of client id's with an option for ALL (-1) and Null/None. Null/None (0) is the default
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		protected SelectList ClientFilterListWithAllAndNull(int? clientId, bool showAllOption = true, bool showNullOption = true, bool defaultNull = true, string labelForAll = "(ALL)", string labelForNull = "(NONE)")
		{
			return ClientFilterListHelper(clientId, showAllOption, showNullOption, defaultNull, labelForAll, labelForNull);
		}

		/// <summary>
		/// Returns a SelectList of store front id's with an option for ALL (-1) and Null/None. Null/None (0) is the default
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		protected SelectList StoreFrontFilterListWithAllAndNull(int? clientId, int? storeFrontId, bool showAllOption = true, bool showNullOption = false, bool defaultNull = false, string labelForAll = "(ALL)", string labelForNull = "(NONE)")
		{
			return StoreFrontFilterListHelper(clientId, storeFrontId, showAllOption, showNullOption, defaultNull, labelForAll, labelForNull);
		}

		protected SelectList ClientFilterListHelper(int? clientId, bool showAllOption = true, bool showNullOption = true, bool defaultNull = true, string labelForAll = "(ALL)", string labelForNull = "(NONE)")
		{
			int filterId = 0;
			string selectedValue = string.Empty;
			bool nullSelected = false;
			bool allSelected = false;

			if (clientId.HasValue)
			{
				filterId = clientId.Value;
				selectedValue = filterId.ToString();
				if (filterId == 0)
				{
					nullSelected = true;
				}
				else if (filterId == -1)
				{
					allSelected = true;
				}
			}
			else
			{
				if (defaultNull)
				{
					selectedValue = "0";
					nullSelected = true;
				}
				else
				{
					selectedValue = "-1";
					allSelected = true;
				}
			}
			List<SelectListItem> items = new List<SelectListItem>();

			if (showNullOption)
			{
				items.Add(new SelectListItem()
				{
					Value = "0",
					Text = (nullSelected ? "[SELECTED] " : string.Empty) + labelForNull,
					Selected = nullSelected
				});
			}

			if (showAllOption)
			{
				items.Add(new SelectListItem()
				{
					Value = "-1",
					Text = (allSelected ? "[SELECTED] " : string.Empty) + labelForAll,
					Selected = allSelected
				});
			}

			var query = GStoreDb.Clients.All().OrderBy(c => c.Order).ThenBy(c => c.ClientId);
			IQueryable<SelectListItem> clients = query.Select(c => new SelectListItem
			{
				Value = c.ClientId.ToString(),
				Text = (c.ClientId == filterId ? "[SELECTED] " : string.Empty) + c.Name + " [" + c.ClientId + "]"
					+ ((c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow) ? " [INACTIVE]" : string.Empty)
					+ (" Store Fronts: " + c.StoreFronts.Count())
			});
			items.AddRange(clients);

			return new SelectList(items, "Value", "Text", selectedValue);
		}

		protected SelectList StoreFrontFilterListHelper(int? clientId, int? storeFrontId, bool showAllOption = true, bool showNullOption = true, bool defaultNull = true, string labelForAll = "(ALL)", string labelForNull = "(NULL)")
		{
			int filterClientId = 0;
			int filterStoreFrontId = 0;

			string selectedValue = string.Empty;
			bool nullSelected = false;
			bool allSelected = false;

			if (clientId.HasValue)
			{
				filterClientId = clientId.Value;
			}

			if (storeFrontId.HasValue)
			{
				filterStoreFrontId = storeFrontId.Value;
				selectedValue = filterStoreFrontId.ToString();
				if (filterStoreFrontId == 0)
				{
					nullSelected = true;
				}
				else if (filterStoreFrontId == -1)
				{
					allSelected = true;
				}

			}
			else
			{
				if (defaultNull)
				{
					selectedValue = "0";
					nullSelected = true;
				}
				else
				{
					selectedValue = "-1";
					allSelected = true;
				}
			}

			if (filterClientId == 0)
			{
				showNullOption = true;
				showAllOption = false;
			}

			List<SelectListItem> items = new List<SelectListItem>();

			if (showNullOption)
			{
				items.Add(new SelectListItem()
				{
					Value = "0",
					Text = (nullSelected ? "[SELECTED] " : string.Empty) + labelForNull,
					Selected = nullSelected
				});
			}

			if (showAllOption)
			{
				items.Add(new SelectListItem()
				{
					Value = "-1",
					Text = (allSelected ? "[SELECTED] " : string.Empty) + labelForAll,
					Selected = allSelected
				});
			}

			List<StoreFront> storeFronts = GStoreDb.StoreFronts.All()
					.Where(sf => filterClientId == -1 || (filterClientId == 0 && filterClientId == null) || sf.ClientId == filterClientId).ApplyDefaultSort().ToList();


			List<SelectListItem> storeFrontList = storeFronts.Select(sf => new SelectListItem
			{
				Value = sf.StoreFrontId.ToString(),
				Text = (sf.StoreFrontId == filterStoreFrontId ? "[SELECTED] " : string.Empty) + (sf.CurrentConfigOrAny() == null ? "" : "'" + sf.CurrentConfigOrAny().Name + "'") + " [" + sf.StoreFrontId + "]"
					+ " - Client: " + sf.Client.Name + " [" + sf.ClientId + "]"
					+ ((sf.IsPending || sf.StartDateTimeUtc > DateTime.UtcNow || sf.EndDateTimeUtc < DateTime.UtcNow) ? " [INACTIVE]" : string.Empty),
				Selected = (sf.StoreFrontId == filterStoreFrontId)
			}).ToList();

			items.AddRange(storeFrontList);
			return new SelectList(items, "Value", "Text", selectedValue);
		}

		protected void ValidateClientName(Client client)
		{
			Client conflict = GStoreDb.Clients.Where(c => c.ClientId != client.ClientId && c.Name.ToLower() == client.Name.ToLower()).FirstOrDefault();
			if (conflict != null)
			{
				this.ModelState.AddModelError("Name", "Client name '" + client.Name + "' is already in use for client '" + conflict.Name.ToHtml() + "' [" + conflict.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				string oldName = client.Name;
				int index = 1;
				while (nameIsDirty)
				{
					index++;
					client.Name = oldName + " " + index;
					nameIsDirty = GStoreDb.Clients.Where(c => c.ClientId != client.ClientId && c.Name.ToLower() == client.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(client.Name, client.Name, null);
				}

			}
		}


		protected void ValidateClientFolder(Client client)
		{
			Client conflict = GStoreDb.Clients.Where(c => c.ClientId != client.ClientId && c.Folder.ToLower() == client.Folder.ToLower()).FirstOrDefault();
			if (conflict != null)
			{
				this.ModelState.AddModelError("Folder", "Client folder name '" + client.Folder + "' is already in use for client '" + conflict.Name.ToHtml() + "' [" + conflict.ClientId + "]. Please choose a new folder");
				string oldName = client.Folder;
				bool folderIsDirty = true;
				//try client name
				client.Folder = client.Name;
				folderIsDirty = GStoreDb.Clients.Where(c => c.Folder.ToLower() == client.Folder.ToLower()).Any();

				int index = 1;
				while (folderIsDirty)
				{
					index++;
					client.Folder = oldName + " " + index;
					folderIsDirty = GStoreDb.Clients.Where(c => c.Folder.ToLower() == client.Folder.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Folder"))
				{
					ModelState["Folder"].Value = new ValueProviderResult(client.Folder, client.Folder, null);
				}
			}
		}

		protected void ValidatePageTemplateName(PageTemplate pageTemplate)
		{
			PageTemplate conflict = GStoreDb.PageTemplates.Where(pt => pt.PageTemplateId != pageTemplate.PageTemplateId && pt.ClientId == pageTemplate.ClientId && pt.Name.ToLower() == pageTemplate.Name.ToLower()).FirstOrDefault();

			if (conflict != null)
			{
				this.ModelState.AddModelError("Name", "Page Template name '" + pageTemplate.Name + "' is already in use for page template id [" + conflict.PageTemplateId + "] for client '" + pageTemplate.Client.Name.ToHtml() + "' [" + pageTemplate.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				string oldName = pageTemplate.Name;
				int index = 1;
				while (nameIsDirty)
				{
					index++;
					pageTemplate.Name = oldName + " " + index;
					nameIsDirty = GStoreDb.PageTemplates.Where(pt => pt.ClientId == pageTemplate.ClientId && pt.Name.ToLower() == pageTemplate.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(pageTemplate.Name, pageTemplate.Name, null);
				}
			}
		}

		protected void ValidatePageTemplateSectionName(PageTemplateSection pageTemplateSection)
		{
			if (pageTemplateSection == null)
			{
				throw new ArgumentNullException("pageTemplateSection");
			}

			PageTemplateSection conflict = GStoreDb.PageTemplateSections.Where(
				pt => (pt.ClientId == pageTemplateSection.ClientId)
					&& (pt.Name.ToLower() == pageTemplateSection.Name.ToLower())
					&& (pt.PageTemplateId == pageTemplateSection.PageTemplateId)
					&& (pt.PageTemplateSectionId != pageTemplateSection.PageTemplateSectionId)
					).FirstOrDefault();
			if (conflict != null)
			{
				this.ModelState.AddModelError("Name", "Page Template Section Name '" + conflict.Name + "' is already in use for Page Template Section '" + conflict.Name.ToHtml() + "' [" + conflict.PageTemplateSectionId + "] for Page Template '" + conflict.PageTemplate.Name.ToHtml() + "' [" + conflict.PageTemplate.PageTemplateId + "] for client '" + conflict.Client.Name.ToHtml() + "' [" + conflict.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				string oldName = pageTemplateSection.Name;
				int index = 1;
				while (nameIsDirty)
				{
					index++;
					pageTemplateSection.Name = oldName + " " + index;
					nameIsDirty = conflict.PageTemplate.Sections.Any(pt => pt.Name.ToLower() == pageTemplateSection.Name.ToLower());
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(pageTemplateSection.Name, pageTemplateSection.Name, null);
				}
			}
		}

		protected void ValidateThemeName(Theme theme)
		{
			Theme conflict = GStoreDb.Themes.Where(t => t.ThemeId != theme.ThemeId && t.ClientId == theme.ClientId && t.Name.ToLower() == theme.Name.ToLower()).FirstOrDefault();
			if (conflict != null)
			{
				this.ModelState.AddModelError("Name", "Theme name '" + theme.Name + "' is already in use for Theme '" + conflict.Name.ToHtml() + "' [" + conflict.ThemeId + "] for client '" + theme.Client.Name.ToHtml() + "' [" + theme.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				string oldName = theme.Name;
				int index = 1;
				while (nameIsDirty)
				{
					index++;
					theme.Name = oldName + " " + index;
					nameIsDirty = GStoreDb.Themes.Where(t => t.ClientId == theme.ClientId && t.Name.ToLower() == theme.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(theme.Name, theme.Name, null);
				}
			}
		}

		protected MvcHtmlString TopBreadcrumb(HtmlHelper htmlHelper, bool ShowAsLink = false)
		{
			if (ShowAsLink)
			{
				return htmlHelper.ActionLink("System Admin", "Index", "SystemAdmin");
			}
			else
			{
				return new MvcHtmlString("System Admin");
			}
		}

		protected MvcHtmlString ClientsBreadcrumb(HtmlHelper htmlHelper, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				TopBreadcrumb(htmlHelper, true).ToHtmlString() 
				+ " -> " 
				+ (ShowAsLink ? htmlHelper.ActionLink("Clients", "Index", "ClientSysAdmin").ToHtmlString() : "Clients")
				);
		}

		protected MvcHtmlString ClientBreadcrumb(HtmlHelper htmlHelper, int? clientId, bool ShowAsLink = false, string clientIdZeroName = "(none)")
		{

			Client client = null;
			string name = "(unknown)";
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					name = "All";
				}
				else if (clientId.Value == 0)
				{
					name = clientIdZeroName;
				}
				else
				{
					client = GStoreDb.Clients.FindById(clientId.Value);
				}
			}
			else
			{
				name = "All";
			}
			bool showLink = false;
			RouteValueDictionary routeData = null;
			if (client != null)
			{
				showLink = ShowAsLink;
				routeData = new RouteValueDictionary(new { id = client.ClientId });
				name = "'" + client.Name + "' [" + client.ClientId + "]";
			}
			return new MvcHtmlString(
				ClientsBreadcrumb(htmlHelper, true).ToHtmlString() 
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "ClientSysAdmin", routeData, null).ToHtmlString() : name)
				);
		}
		

		protected MvcHtmlString StoreFrontsBreadcrumb(HtmlHelper htmlHelper, int? clientId, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				ClientBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Store Fronts", "Index", "StoreFrontSysAdmin", new { clientId = clientId }, null).ToHtmlString() : "Store Fronts")
				);
		}

		protected MvcHtmlString StoreFrontBreadcrumb(HtmlHelper htmlHelper, int? clientId, int? storeFrontId, bool ShowAsLink = false, string storeFrontIdZeroName = "(none)")
		{
			StoreFront storeFront = null;
			string name = "(unknown)";
			if (storeFrontId.HasValue)
			{
				if (storeFrontId.Value == -1)
				{
					name = "All";
				}
				else if (storeFrontId.Value == 0)
				{
					name = storeFrontIdZeroName;
				}
				else
				{
					storeFront = GStoreDb.StoreFronts.FindById(storeFrontId.Value);
					return StoreFrontBreadcrumb(htmlHelper, clientId, storeFront, ShowAsLink);
				}
			}
			else
			{
				name = "All";
			}

			if (storeFront != null)
			{
				if (storeFront.StoreFrontId == 0)
				{
					name = "New";
				}
				else
				{
					StoreFrontConfiguration config = storeFront.CurrentConfigOrAny();
					name = (config == null ? "id [" + storeFront.StoreFrontId + "]" : "'" + config.Name + "' [" + storeFront.StoreFrontId + "]");
				}
			}

			return new MvcHtmlString(
				StoreFrontsBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ name
				);
		}

		protected MvcHtmlString StoreFrontBreadcrumb(HtmlHelper htmlHelper, int? clientId, StoreFront storeFront, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (storeFront != null)
			{
				if (storeFront.StoreFrontId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = storeFront.StoreFrontId });
					StoreFrontConfiguration config = storeFront.CurrentConfigOrAny();
					name = (config == null ? "id [" + storeFront.StoreFrontId + "]" : "'" + config.Name + "' [" + storeFront.StoreFrontId + "]");
				}
			}

			return new MvcHtmlString(
				StoreFrontsBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "StoreFrontSysAdmin", routeData, null).ToHtmlString() : name)
				);
		}

		protected MvcHtmlString BindingsBreadcrumb(HtmlHelper htmlHelper, int? clientId, StoreFront storeFront, bool ShowAsLink = false)
		{
			int? storeFrontId = (storeFront == null ? (int?)null : storeFront.StoreFrontId);

			return new MvcHtmlString(
				StoreFrontBreadcrumb(htmlHelper, clientId, storeFront, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Bindings", "Index", "BindingsSysAdmin", new { clientId = clientId , storeFrontId = storeFrontId }, null).ToHtmlString() : "Bindings")
				);
		}

		protected MvcHtmlString BindingBreadcrumb(HtmlHelper htmlHelper, int? clientId, StoreFront storeFront, StoreBinding storeBinding, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (storeBinding != null)
			{
				if (storeBinding.StoreBindingId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = storeBinding.StoreBindingId });
					name = "Binding [" + storeBinding.StoreBindingId + "]";
				}
			}

			return new MvcHtmlString(
				BindingsBreadcrumb(htmlHelper, clientId, storeFront, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "BindingsSysAdmin", routeData, null).ToHtmlString() : name)
				);
		}

		protected MvcHtmlString PageTemplatesBreadcrumb(HtmlHelper htmlHelper, int? clientId, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				ClientBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Page Templates", "Index", "PageTemplateSysAdmin", new { clientId = clientId }, null).ToHtmlString() : "Page Templates")
				);
		}

		protected MvcHtmlString PageTemplateBreadcrumb(HtmlHelper htmlHelper, int? clientId, PageTemplate pageTemplate, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (pageTemplate != null)
			{
				if (pageTemplate.PageTemplateId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = pageTemplate.PageTemplateId });
					name = "'" + pageTemplate.Name + "' [" + pageTemplate.PageTemplateId + "]";
				}
			}
			return new MvcHtmlString(
				PageTemplatesBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "PageTemplateSysAdmin", routeData, null).ToHtmlString() : name)
				);
		}

		protected MvcHtmlString PageTemplateSectionsBreadcrumb(HtmlHelper htmlHelper, PageTemplate pageTemplate, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				PageTemplateBreadcrumb(htmlHelper, pageTemplate.ClientId, pageTemplate, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Sections", "SectionIndex", "PageTemplateSysAdmin", new { id = pageTemplate.PageTemplateId }, null).ToHtmlString() : "Sections")
				);
		}

		protected MvcHtmlString PageTemplateSectionBreadcrumb(HtmlHelper htmlHelper, PageTemplateSection pageTemplateSection, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (pageTemplateSection != null)
			{
				if (pageTemplateSection.PageTemplateSectionId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = pageTemplateSection.PageTemplateSectionId });
					name = "'" + pageTemplateSection.Name + "' [" + pageTemplateSection.PageTemplateSectionId + "]";
				}
			}
			return new MvcHtmlString(
				PageTemplateSectionsBreadcrumb(htmlHelper, pageTemplateSection.PageTemplate, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "SectionDetails", "PageTemplateSysAdmin", routeData, null).ToHtmlString() : name)
				);

		}

		protected MvcHtmlString ThemesBreadcrumb(HtmlHelper htmlHelper, int? clientId, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				ClientBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Themes", "Index", "ThemeSysAdmin", new { clientId = clientId }, null).ToHtmlString() : "Themes")
				);
		}

		protected MvcHtmlString ThemeBreadcrumb(HtmlHelper htmlHelper, int? clientId, Theme theme, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (theme != null)
			{
				if (theme.ThemeId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = theme.ThemeId });
					name = "'" + theme.Name + "' [" + theme.ThemeId + "]";
				}
			}
			return new MvcHtmlString(
				ThemesBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "ThemeSysAdmin", routeData, null).ToHtmlString() : name)
				);
		}

		protected MvcHtmlString UserProfilesBreadcrumb(HtmlHelper htmlHelper, int? clientId, int? storeFrontId, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				StoreFrontBreadcrumb(htmlHelper, clientId, storeFrontId, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("User Profiles", "Index", "UserProfileSysAdmin", new { clientId = clientId, storeFrontId = storeFrontId }, null).ToHtmlString() : "User Profiles")
				);
		}

		protected MvcHtmlString UserProfileBreadcrumb(HtmlHelper htmlHelper, int? clientId, int? storeFrontId, UserProfile userProfile, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (userProfile != null)
			{
				if (userProfile.UserProfileId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = userProfile.UserProfileId });
					name = "'" + userProfile.FullName + "' <" + userProfile.Email + "> [" + userProfile.UserProfileId + "]";
				}
			}

			return new MvcHtmlString(
				UserProfilesBreadcrumb(htmlHelper, clientId, storeFrontId, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "UserProfileSysAdmin", routeData, null).ToHtmlString() : name.ToHtml())
				);
		}

		protected MvcHtmlString ValueListsBreadcrumb(HtmlHelper htmlHelper, int? clientId, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				ClientBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Value Lists", "Index", "ValueListSysAdmin", new { clientId = clientId }, null).ToHtmlString() : "Value Lists")
				);
		}

		protected MvcHtmlString ValueListBreadcrumb(HtmlHelper htmlHelper, int? clientId, ValueList valueList, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (valueList != null)
			{
				if (valueList.ValueListId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = valueList.ValueListId });
					name = "'" + valueList.Name + "' [" + valueList.ValueListId + "]";
				}
			}
			return new MvcHtmlString(
				ValueListsBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "ValueListSysAdmin", routeData, null).ToHtmlString() : name)
				);
		}

		protected MvcHtmlString ValueListItemsBreadcrumb(HtmlHelper htmlHelper, ValueList valueList, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("List Items", "ListItemIndex", "ValueListSysAdmin", new { id = valueList.ValueListId }, null).ToHtmlString() : "List Items")
				);
		}

		protected MvcHtmlString ValueListItemBreadcrumb(HtmlHelper htmlHelper, ValueListItem valueListItem, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (valueListItem != null)
			{
				if (valueListItem.ValueListItemId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = valueListItem.ValueListItemId });
					name = "'" + valueListItem.Name + "' [" + valueListItem.ValueListItemId + "]";
				}
			}
			return new MvcHtmlString(
				ValueListItemsBreadcrumb(htmlHelper, valueListItem.ValueList, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "ListItemDetails", "ValueListSysAdmin", routeData, null).ToHtmlString() : name)
				);

		}

		protected MvcHtmlString WebFormsBreadcrumb(HtmlHelper htmlHelper, int? clientId, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				ClientBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Web Forms", "Index", "WebFormSysAdmin", new { clientId = clientId }, null).ToHtmlString() : "Web Forms")
				);
		}

		protected MvcHtmlString WebFormBreadcrumb(HtmlHelper htmlHelper, int? clientId, WebForm webForm, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (webForm != null)
			{
				if (webForm.WebFormId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = webForm.WebFormId });
					name = "'" + webForm.Name + "' [" + webForm.WebFormId + "]";
				}
			}
			return new MvcHtmlString(
				WebFormsBreadcrumb(htmlHelper, clientId, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "Details", "WebFormSysAdmin", routeData, null).ToHtmlString() : name)
				);
		}

		protected MvcHtmlString WebFormFieldsBreadcrumb(HtmlHelper htmlHelper, WebForm webForm, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				WebFormBreadcrumb(htmlHelper, webForm.ClientId, webForm, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Fields", "FieldIndex", "WebFormSysAdmin", new { id = webForm.WebFormId }, null).ToHtmlString() : "Fields")
				);
		}

		protected MvcHtmlString WebFormFieldBreadcrumb(HtmlHelper htmlHelper, WebFormField webFormField, bool ShowAsLink = false)
		{
			RouteValueDictionary routeData = null;
			string name = "(unknown)";
			bool showLink = false;
			if (webFormField != null)
			{
				if (webFormField.WebFormFieldId == 0)
				{
					name = "New";
				}
				else
				{
					showLink = ShowAsLink;
					routeData = new RouteValueDictionary(new { id = webFormField.WebFormFieldId });
					name = "'" + webFormField.Name + "' [" + webFormField.WebFormFieldId + "]";
				}
			}
			return new MvcHtmlString(
				WebFormFieldsBreadcrumb(htmlHelper, webFormField.WebForm, true).ToHtmlString()
				+ " -> "
				+ (showLink ? htmlHelper.ActionLink(name, "FieldDetails", "WebFormSysAdmin", routeData, null).ToHtmlString() : name)
				);

		}

		protected MvcHtmlString GStoreAboutBreadcrumb(HtmlHelper htmlHelper, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				TopBreadcrumb(htmlHelper, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("GStore", "About", "GStore").ToHtmlString() : "GStore")
				);
		}

		protected MvcHtmlString GStoreSystemInfoBreadcrumb(HtmlHelper htmlHelper, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				GStoreAboutBreadcrumb(htmlHelper, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("System Info", "SystemInfo", "GStore").ToHtmlString() : "System Info")
				);
		}

		protected MvcHtmlString GStoreSettingsBreadcrumb(HtmlHelper htmlHelper, bool ShowAsLink = false)
		{
			return new MvcHtmlString(
				GStoreAboutBreadcrumb(htmlHelper, true).ToHtmlString()
				+ " -> "
				+ (ShowAsLink ? htmlHelper.ActionLink("Settings", "Settings", "GStore").ToHtmlString() : "Settings")
				);
		}



	}
}