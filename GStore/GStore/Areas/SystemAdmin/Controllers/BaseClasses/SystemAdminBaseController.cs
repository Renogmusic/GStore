using GStore.Areas.StoreAdmin.ViewModels;
using GStore.Controllers.BaseClass;
using GStore.Data;
using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.SystemAdmin.Controllers.BaseClasses
{
	[Authorize(Roles="SystemAdmin")]
	public class SystemAdminBaseController : GStore.Controllers.BaseClass.BaseController
	{
		public SystemAdminBaseController(IGstoreDb dbContext): base(dbContext)
		{
			this._throwErrorIfUserProfileNotFound = false;
			this._throwErrorIfStoreFrontNotFound = false;
		}

		public SystemAdminBaseController()
		{
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

		protected SelectList ClientFilterListEx(int? clientId, bool showAllOption = true, bool showNullOption = false, bool defaultNull = false)
		{
			int filterId = 0;
			string selectedValue = string.Empty;
			if (clientId.HasValue)
			{
				filterId = clientId.Value;
				selectedValue = filterId.ToString();
			}
			else
			{
				if (defaultNull)
				{
					selectedValue = "0";
				}
				else
				{
					selectedValue = "-1";
				}
			}
			List<SelectListItem> items = new List<SelectListItem>();


			if (showNullOption)
			{
				items.Add(new SelectListItem()
				{
					Value = "0",
					Text = ((clientId.HasValue && clientId.Value == 0) || (!clientId.HasValue && defaultNull) ? "[SELECTED] " : string.Empty) + "(null)",
				});
			}

			if (showAllOption)
			{
				items.Add(new SelectListItem()
				{
					Value = "-1",
					Text = ((clientId.HasValue && clientId.Value == -1) || (!clientId.HasValue && !defaultNull) ? "[SELECTED] " : string.Empty) + "(All)",
				});
			}

			var query = GStoreDb.Clients.All().OrderBy(c => c.Order).ThenBy(c => c.ClientId);
			IQueryable<SelectListItem> clients = query.Select(c => new SelectListItem
			{
				Value = c.ClientId.ToString(),
				Text = (c.ClientId == filterId ? "[SELECTED] " : string.Empty) + c.Name + " [" + c.ClientId + "]",
			});
			items.AddRange(clients);

			return new SelectList(items, "Value", "Text", selectedValue);
		}

		protected SelectList ClientFilterList(int? clientId)
		{
			int filterId = 0;
			string selectedValue = string.Empty;
			if (clientId.HasValue)
			{
				filterId = clientId.Value;
				selectedValue = filterId.ToString();
			}

			List<SelectListItem> items = new List<SelectListItem>();


			items.Add(new SelectListItem()
			{
				Value = string.Empty,
				Text = ((!clientId.HasValue) ? "[SELECTED] " : string.Empty) + "(All)",
			});

			var query = GStoreDb.Clients.All().OrderBy(c => c.Order).ThenBy(c => c.ClientId);
			IQueryable<SelectListItem> clients = query.Select(c => new SelectListItem
			{
				Value = c.ClientId.ToString(),
				Text = (c.ClientId == filterId ? "[SELECTED] " : string.Empty) + c.Name + " [" + c.ClientId + "]",
			});
			items.AddRange(clients);

			return new SelectList(items, "Value", "Text", selectedValue);
		}


		protected SelectList StoreFrontFilterList(int? clientId, int? storeFrontId)
		{
			int filterClientId = 0;
			if (clientId.HasValue)
			{
				filterClientId = clientId.Value;
			}

			int filterStoreFrontId = 0;
			if (storeFrontId.HasValue)
			{
				filterStoreFrontId = storeFrontId.Value;
			}

			List<SelectListItem> items = new List<SelectListItem>();
			if (!clientId.HasValue)
			{
				items.Add(new SelectListItem()
				{
					Value = "",
					Text = "[SELECTED] (None)",
					Selected = true
				});
				return new SelectList(items, "Value", "Text");
			}

			if (filterClientId == 0)
			{

			}

			items.Add(new SelectListItem()
			{
				Value = "0",
				Text = (filterClientId == 0 ? "[SELECTED] " : string.Empty) + "-ALL-",
				Selected = !(filterClientId == 0)
			});

			var query = GStoreDb.StoreFronts.All()
					.Where(sf => filterClientId == 0 || sf.ClientId == filterClientId)
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
			return new SelectList(items, "Value", "Text");
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

			var query = GStoreDb.Clients.All().OrderBy(c => c.Order).ThenBy(c => c.ClientId);
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
				GStoreDb.StoreFronts.Where(sf => sf.ClientId == clientId.Value);
			}
			else
			{
				GStoreDb.StoreFronts.All();
			}
			query = query.ApplySort(this, null, null);

			IQueryable<SelectListItem> dbItems = query.Select(sf => new SelectListItem
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
				this.ModelState.AddModelError("Folder", "StoreFront Folder name '" + storeFront.Folder + "' is already in use. Please choose a new folder");
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
				this.ModelState.AddModelError("Name", "Store Front name '" + storeFront.Name + "' is already in use. Please choose a new name");
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

	}
}