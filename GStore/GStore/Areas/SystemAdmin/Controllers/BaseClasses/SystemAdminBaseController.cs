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

namespace GStore.Areas.SystemAdmin.Controllers.BaseClasses
{
	[AuthorizeSystemAdmin]
	public class SystemAdminBaseController : GStore.Controllers.BaseClass.BaseController
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

		protected override string LayoutName
		{
			get
			{
				return Properties.Settings.Current.AppDefaultLayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				return Properties.Settings.Current.AppDefaultThemeFolderName;
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

			var query = GStoreDb.StoreFronts.All()
					.Where(sf => filterClientId == -1 || (filterClientId == 0 && filterClientId == null) || sf.ClientId == filterClientId)
					.OrderBy(sf => sf.Client.Order).ThenBy(sf => sf.ClientId).ThenBy(sf => sf.Order).ThenBy(sf => sf.StoreFrontId);


			IQueryable<SelectListItem> storeFronts = query.Select(sf => new SelectListItem
			{
				Value = sf.StoreFrontId.ToString(),
				Text = (sf.StoreFrontId == filterStoreFrontId ? "[SELECTED] " : string.Empty) + sf.Name + " [" + sf.StoreFrontId + "]"
					+ " - Client: " + sf.Client.Name + " [" + sf.ClientId + "]"
					+ ((sf.IsPending || sf.StartDateTimeUtc > DateTime.UtcNow || sf.EndDateTimeUtc < DateTime.UtcNow) ? " [INACTIVE]" : string.Empty),
				Selected = (sf.StoreFrontId == filterStoreFrontId)
			});

			items.AddRange(storeFronts);
			return new SelectList(items, "Value", "Text", selectedValue);
		}


		protected SelectList ClientList()
		{
			List<SelectListItem> items = new List<SelectListItem>();
			SelectListItem nullItem = new SelectListItem()
			{
				Value = "",
				Text = "(none)"
			};
			items.Add(nullItem);

			var query = GStoreDb.Clients.All().ApplySort(this, null, null);
			IQueryable<SelectListItem> dbItems = query.Select(c => new SelectListItem
			{
				Value = c.ClientId.ToString(),
				Text = c.Name + " [" + c.ClientId + "]"
			});

			items.AddRange(dbItems);

			return new SelectList(items, "Value", "Text");
		}

		protected SelectList StoreFrontList(int? clientId)
		{
			List<SelectListItem> items = new List<SelectListItem>();
			SelectListItem nullItem = new SelectListItem()
			{
				Value = "",
				Text = "(none)"
			};
			items.Add(nullItem);

			IQueryable<StoreFront> query = null;
			if (clientId.HasValue)
			{
				query = GStoreDb.StoreFronts.Where(sf => sf.ClientId == clientId.Value);
			}
			else
			{
				query = GStoreDb.StoreFronts.All();
			}
			IOrderedQueryable<StoreFront> orderedQuery = query.ApplySort(this, null, null);

			IQueryable<SelectListItem> dbItems = orderedQuery.Select(sf => new SelectListItem
			{
				Value = sf.StoreFrontId.ToString(),
				Text = sf.Name + " [" + sf.StoreFrontId + "] Client " + sf.Client.Name + " [" + sf.ClientId + "]"
			});

			items.AddRange(dbItems);

			return new SelectList(items, "Value", "Text");
		}

		protected SelectList UserProfileList(int? clientId, int? storeFrontId)
		{
			var query = GStoreDb.UserProfiles.All();

			if (clientId.HasValue)
			{
				query = query.Where(p => !p.ClientId.HasValue || p.ClientId.Value == clientId);
			}
			if (storeFrontId.HasValue)
			{
				query = query.Where(p => !p.StoreFrontId.HasValue || p.StoreFrontId.Value == storeFrontId);
			}
			query = query.OrderBy(p => p.Order).ThenBy(p => p.UserProfileId).ThenBy(p => p.UserName);

			IQueryable<SelectListItem> items = query.Select(p => new SelectListItem
			{
				Value = p.UserProfileId.ToString(),
				Text = p.FullName + " <" + p.Email + ">"
				+ (p.StoreFrontId.HasValue ? " - Store '" + p.StoreFront.Name + "' [" + p.StoreFrontId + "]" : " (no store)")
				+ (p.ClientId.HasValue ? " - Client '" + p.Client.Name + "' [" + p.ClientId + "]" : " (no client)")
			});

			return new SelectList(items, "Value", "Text");
		}

		protected SelectList StoreFrontSuccessPageList(int? clientId, int? storeFrontId)
		{
			List<SelectListItem> list = new List<SelectListItem>();
			if (clientId.HasValue && storeFrontId.HasValue)
			{
				var dbItems = StoreFrontPageListItems(clientId.Value, storeFrontId.Value);
				if (dbItems != null)
				{
					list.AddRange(dbItems);
				}
			}
			return new SelectList(list, "Value", "Text");
		}

		protected SelectList StoreFrontErrorPageList(int? clientId, int? storeFrontId)
		{
			SelectListItem itemNone = new SelectListItem();
			itemNone.Value = null;
			itemNone.Text = "(GStore System Default Error Page)";
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(itemNone);
			if (clientId.HasValue && storeFrontId.HasValue)
			{
				var dbItems = StoreFrontPageListItems(clientId.Value, storeFrontId.Value);
				if (dbItems != null)
				{
					list.AddRange(dbItems);
				}
			}
			return new SelectList(list, "Value", "Text");
		}

		protected SelectList RegisterWebFormList(int? clientId, int? storeFrontId)
		{

			SelectListItem itemNone = new SelectListItem();
			itemNone.Value = null;
			itemNone.Text = "(GStore System Default Register Form)";

			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(itemNone);
			if (clientId.HasValue && storeFrontId.HasValue)
			{
				var dbItems = RegisterWebFormListItems(clientId.Value, storeFrontId.Value);
				if (dbItems != null)
				{
					list.AddRange(dbItems);
				}
			}
			return new SelectList(list, "Value", "Text");
		}


		protected SelectList StoreFrontNotFoundPageList(int? clientId, int? storeFrontId)
		{

			SelectListItem itemNone = new SelectListItem();
			itemNone.Value = null;
			itemNone.Text = "(GStore System Default Not Found Page)";

			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(itemNone);
			if (clientId.HasValue && storeFrontId.HasValue)
			{
				var dbItems = StoreFrontPageListItems(clientId.Value, storeFrontId.Value);
				if (dbItems != null)
				{
					list.AddRange(dbItems);
				}
			}
			return new SelectList(list, "Value", "Text");
		}

		protected IEnumerable<SelectListItem> RegisterWebFormListItems(int clientId, int storeFrontId)
		{
			List<SelectListItem> list = new List<SelectListItem>();

			SelectListItem itemNone = new SelectListItem();
			if (clientId == 0 || storeFrontId == 0)
			{
				return null;
			}

			IQueryable<WebForm> query = GStoreDb.WebForms.Where(pg => pg.ClientId == clientId);

			IOrderedQueryable<WebForm> orderedQuery = query.ApplySort(null, null, null);
			IEnumerable<SelectListItem> items = orderedQuery.Select(wf => new SelectListItem
			{
				Value = wf.WebFormId.ToString(),
				Text = wf.Name + " [" + wf.WebFormId + "]"
					+ " - Client '" + wf.Client.Name + "' [" + wf.Client.ClientId + "]"
			});

			return items;
		}


		protected IEnumerable<SelectListItem> StoreFrontPageListItems(int clientId, int storeFrontId)
		{
			List<SelectListItem> list = new List<SelectListItem>();

			SelectListItem itemNone = new SelectListItem();
			if (clientId == 0 || storeFrontId == 0)
			{
				return null;
			}

			IQueryable<Page> query = GStoreDb.Pages.Where(pg => pg.ClientId == clientId)
				.Where(pg => pg.StoreFrontId == storeFrontId);

			IOrderedQueryable<Page> orderedQuery = query.ApplySort(null, null, null);
			IEnumerable<SelectListItem> items = orderedQuery.Select(pg => new SelectListItem
			{
				Value = pg.PageId.ToString(),
				Text = pg.Name + " [" + pg.PageId + "]"
					+ " - Store Front '" + pg.StoreFront.Name + "' [" + pg.StoreFront.StoreFrontId + "]"
					+ " - Client '" + pg.Client.Name + "' [" + pg.Client.ClientId + "]"
			});

			return items;
		}

		protected SelectList ThemeList()
		{
			var query = GStoreDb.Themes.All().OrderBy(t => t.Order).ThenBy(t => t.ThemeId);
			IQueryable<SelectListItem> items = query.Select(t => new SelectListItem
			{
				Value = t.ThemeId.ToString(),
				Text = t.Name + " [" + t.ThemeId + "]"
			});
			return new SelectList(items, "Value", "Text");
		}

		/// <summary>
		/// Returns a list of theme folders from the file system for selection
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		protected SelectList ThemeFolderList(int? clientId)
		{
			string virtualPath = "~/Content/Server/Themes/";
			if (!System.IO.Directory.Exists(Server.MapPath(virtualPath)))
			{
				throw new ApplicationException("Themes folder does not exist in file system at '" + virtualPath + "'");
			}
			System.IO.DirectoryInfo themesFolder = new System.IO.DirectoryInfo(Server.MapPath(virtualPath));
			IEnumerable<System.IO.DirectoryInfo> themeFolders = themesFolder.EnumerateDirectories("*", System.IO.SearchOption.TopDirectoryOnly);

			IEnumerable<SelectListItem> items = themeFolders.Select(t => new SelectListItem
			{
				Value = t.Name,
				Text = t.Name + " ["+ (System.IO.File.Exists(t.FullName + "\\bootstrap.min.css") ? "bootstrap.min.css OK" : "WARNING: no bootstrap.min.css") + "]"
			});
			return new SelectList(items, "Value", "Text");
		}

		protected void ValidateClientName(Client client)
		{
			if (GStoreDb.Clients.Where(c => c.ClientId != client.ClientId && c.Name.ToLower() == client.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Client name '" + client.Name + "' is already in use. Please choose a new name");
				bool nameIsDirty = true;
				while (nameIsDirty)
				{
					client.Name = client.Name + "_New";
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
			if (GStoreDb.Clients.Where(c => c.ClientId != client.ClientId && c.Folder.ToLower() == client.Folder.ToLower()).Any())
			{
				this.ModelState.AddModelError("Folder", "Client folder name '" + client.Folder + "' is already in use. Please choose a new folder");
				bool folderIsDirty = true;
				while (folderIsDirty)
				{
					client.Folder = client.Folder + "_New";
					folderIsDirty = GStoreDb.Clients.Where(c => c.Folder.ToLower() == client.Folder.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Folder"))
				{
					ModelState["Folder"].Value = new ValueProviderResult(client.Folder, client.Folder, null);
				}
			}
		}


		/// <summary>
		/// Sets ModelState to add errors
		/// </summary>
		/// <param name="storeFront"></param>
		protected void ValidateStoreFrontFolder(StoreFront storeFront)
		{
			if (GStoreDb.StoreFronts.Where(sf => sf.StoreFrontId != storeFront.StoreFrontId && sf.ClientId == storeFront.ClientId && sf.Folder.ToLower() == storeFront.Folder.ToLower()).Any())
			{
				this.ModelState.AddModelError("Folder", "StoreFront Folder name '" + storeFront.Folder + "' is already in use for client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "]. Please choose a new folder");
				bool folderIsDirty = true;
				while (folderIsDirty)
				{
					storeFront.Folder = storeFront.Folder + "_New";
					folderIsDirty = GStoreDb.StoreFronts.Where(sf => sf.StoreFrontId != storeFront.StoreFrontId && sf.ClientId == storeFront.ClientId && sf.Folder.ToLower() == storeFront.Folder.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Folder"))
				{
					ModelState["Folder"].Value = new ValueProviderResult(storeFront.Folder, storeFront.Folder, null);
				}
			}
		}

		protected void ValidateStoreFrontName(StoreFront storeFront)
		{
			if (GStoreDb.StoreFronts.Where(sf => sf.StoreFrontId != storeFront.StoreFrontId && sf.ClientId == storeFront.ClientId && sf.Name.ToLower() == storeFront.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Store Front name '" + storeFront.Name + "' is already in use for client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				while (nameIsDirty)
				{
					storeFront.Name = storeFront.Name + "_New";
					nameIsDirty = GStoreDb.StoreFronts.Where(sf => sf.ClientId == storeFront.ClientId && sf.Name.ToLower() == storeFront.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(storeFront.Name, storeFront.Name, null);
				}
			}
		}

		protected void ValidatePageTemplateName(PageTemplate pageTemplate)
		{
			if (GStoreDb.PageTemplates.Where(pt => pt.PageTemplateId != pageTemplate.PageTemplateId && pt.ClientId == pageTemplate.ClientId && pt.Name.ToLower() == pageTemplate.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Page Template name '" + pageTemplate.Name + "' is already in use for client '" + pageTemplate.Client.Name.ToHtml() + "' [" + pageTemplate.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				while (nameIsDirty)
				{
					pageTemplate.Name = pageTemplate.Name + "_New";
					nameIsDirty = GStoreDb.PageTemplates.Where(pt => pt.Name.ToLower() == pageTemplate.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(pageTemplate.Name, pageTemplate.Name, null);
				}
			}
		}

		protected void ValidatePageTemplateSectionName(PageTemplateSection pageTemplateSection)
		{
			if (GStoreDb.PageTemplateSections.Where(pt => pt.PageTemplateSectionId != pageTemplateSection.PageTemplateSectionId && pt.Name.ToLower() == pageTemplateSection.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Page Template Section Name '" + pageTemplateSection.Name + "' is already in use for client '" + pageTemplateSection.Client.Name.ToHtml() + "' [" + pageTemplateSection.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				while (nameIsDirty)
				{
					pageTemplateSection.Name = pageTemplateSection.Name + "_New";
					nameIsDirty = GStoreDb.PageTemplates.Where(pt => pt.Name.ToLower() == pageTemplateSection.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(pageTemplateSection.Name, pageTemplateSection.Name, null);
				}
			}
		}

		protected void ValidateThemeName(Theme theme)
		{
			if (GStoreDb.Themes.Where(t => t.ThemeId != theme.ThemeId && t.ClientId == theme.ClientId && t.Name.ToLower() == theme.Name.ToLower()).Any())
			{
				this.ModelState.AddModelError("Name", "Theme name '" + theme.Name + "' is already in use for client '" + theme.Client.Name.ToHtml() + "' [" + theme.ClientId + "]. Please choose a new name");
				bool nameIsDirty = true;
				while (nameIsDirty)
				{
					theme.Name = theme.Name + "_New";
					nameIsDirty = GStoreDb.Themes.Where(t => t.ClientId == theme.ClientId && t.Name.ToLower() == theme.Name.ToLower()).Any();
				}
				if (ModelState.ContainsKey("Name"))
				{
					ModelState["Name"].Value = new ValueProviderResult(theme.Name, theme.Name, null);
				}
			}
		}



	}
}