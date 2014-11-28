using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GStore.Data.EntityFrameworkCodeFirstProvider;
using GStore.Models;
using GStore.Data;

namespace GStore.Areas.SystemAdmin.Controllers
{
	public class StoreFrontSysAdminController : BaseClasses.SystemAdminBaseController
	{

		// GET: SystemAdmin/StoreFrontSysAdmin
		public ActionResult Index(int? id, string SortBy, bool? SortAscending)
		{
			ViewBag.ClientFilterList = ClientFilterList(id);

			IQueryable<StoreFront> query = null;
			if (id.HasValue)
			{
				query = GStoreDb.StoreFronts.Where(sf => sf.ClientId == id.Value);
			}
			else
			{
				query = GStoreDb.StoreFronts.All();
			}

			IOrderedQueryable<StoreFront> queryOrdered = this.ApplySort(query, SortBy, SortAscending);
			return View(queryOrdered.ToList());
		}

		// GET: SystemAdmin/StoreFrontSysAdmin/Details/5
		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("StoreFront id is null");
			}
			StoreFront storeFront = GStoreDb.StoreFronts.FindById(id.Value);
			if (storeFront == null)
			{
				return HttpNotFound();
			}

			return View(storeFront);
		}

		// GET: SystemAdmin/StoreFrontSysAdmin/Create
		public ActionResult Create(int? clientId)
		{
			ViewBag.UserProfileList = UserProfileList(clientId, null);
			ViewBag.ThemeList = ThemeList();
			ViewBag.ClientList = ClientList();
			ViewBag.NotFoundPageList = NotFoundPageList();
			ViewBag.StoreErrorPageList = StoreErrorPageList();

			StoreFront model = GStoreDb.StoreFronts.Create();
			model.SetDefaultsForNew(clientId);
			return View(model);
		}

		// POST: SystemAdmin/StoreFrontSysAdmin/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(StoreFront storeFront)
		{
			//check if storefront name or folder is dupe for this client
			ValidateStoreFrontName(storeFront);
			ValidateStoreFrontFolder(storeFront);

			if (ModelState.IsValid)
			{
				storeFront = GStoreDb.StoreFronts.Create(storeFront);
				storeFront.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeFront = GStoreDb.StoreFronts.Add(storeFront);
				GStoreDb.SaveChanges();

				//create folders for new Store Front if they don't exist already
				try
				{
					CreateStoreFrontFolders(Server.MapPath(storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath)));
					AddUserMessage("StoreFront Folders Created", "StoreFront Folders were created in '" + storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath) + "'", AppHtmlHelpers.UserMessageType.Success);
				}
				catch (Exception ex)
				{
					AddUserMessage("Error Creating Store Front Folders!", "There was an error creating the store front folders in '" + storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath) + "'. You will need to create the folder manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
				}

				return RedirectToAction("Index");
			}
			int? clientId = null;
			if (storeFront.ClientId != default(int))
			{
				clientId = storeFront.ClientId;
			}

			ViewBag.UserProfileList = UserProfileList(clientId, null);
			ViewBag.ThemeList = ThemeList();
			ViewBag.ClientList = ClientList();
			ViewBag.NotFoundPageList = NotFoundPageList();
			ViewBag.StoreErrorPageList = StoreErrorPageList();

			return View(storeFront);
		}

		// GET: SystemAdmin/StoreFrontSysAdmin/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("StoreFront id is null");
			}
			StoreFront storeFront = GStoreDb.StoreFronts.FindById(id.Value);
			if (storeFront == null)
			{
				return HttpNotFound();
			}

			ViewBag.UserProfileList = UserProfileList(storeFront.ClientId, storeFront.StoreFrontId);
			ViewBag.ThemeList = ThemeList();
			ViewBag.ClientList = ClientList();
			ViewBag.NotFoundPageList = NotFoundPageList();
			ViewBag.StoreErrorPageList = StoreErrorPageList();

			return View(storeFront);
		}

		// POST: SystemAdmin/StoreFrontSysAdmin/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(StoreFront storeFront)
		{
			ValidateStoreFrontName(storeFront);
			ValidateStoreFrontFolder(storeFront);

			if (ModelState.IsValid)
			{
				StoreFront originalValues = GStoreDb.StoreFronts.Single(sf => sf.StoreFrontId == storeFront.StoreFrontId);
				string originalFolderName = originalValues.Folder;
				string originalFolderToMap = originalValues.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath);

				storeFront.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeFront = GStoreDb.StoreFronts.Update(storeFront);
				GStoreDb.SaveChanges();

				//detect if folder name has changed
				if (storeFront.Folder != originalFolderName)
				{
					//default behavior is to move the old folder to the new name
					string newStoreFrontFolder = Server.MapPath(storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath));
					string originalStoreFrontFolder = Server.MapPath(originalFolderToMap);
					if (System.IO.Directory.Exists(originalStoreFrontFolder))
					{
						try
						{
							System.IO.Directory.Move(originalStoreFrontFolder, newStoreFrontFolder);
							AddUserMessage("Folder Moved", "StoreFront folder name was changed, so the StoreFront folder was moved from '" + originalFolderToMap + "' to '" + storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath) + "'", AppHtmlHelpers.UserMessageType.Success);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Moving StoreFront Folder!", "There was an error moving the StoreFront folder from '" + originalFolderToMap + "' to '" + storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath) + "'. You will need to move the folders manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
						}
					}
					else
					{
						try
						{
							CreateStoreFrontFolders(newStoreFrontFolder);
							AddUserMessage("StoreFront Folders Created", "StoreFront Folders were created in '" + storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath) + "'", AppHtmlHelpers.UserMessageType.Info);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Creating StoreFront Folders!", "There was an error creating the Store front folders in '" + storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath) + "'. You will need to create the folders manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
						}
					}
				}

				return RedirectToAction("Index");
			}
			ViewBag.UserProfileList = UserProfileList(storeFront.ClientId, storeFront.StoreFrontId);
			ViewBag.ThemeList = ThemeList();
			ViewBag.ClientList = ClientList();
			ViewBag.NotFoundPageList = NotFoundPageList();
			ViewBag.StoreErrorPageList = StoreErrorPageList();

			return View(storeFront);
		}

		public ActionResult Activate(int id)
		{
			this.ActivateStoreFrontOnly(id);
			if (Request.UrlReferrer != null)
			{
				return Redirect(Request.UrlReferrer.ToString());

			}
			return RedirectToAction("Index");
		}

		// GET: SystemAdmin/StoreFrontSysAdmin/Delete/5
		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("StoreFront id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			StoreFront storeFront = db.StoreFronts.FindById(id.Value);
			if (storeFront == null)
			{
				return HttpNotFound();
			}
			return View(storeFront);
		}

		// POST: SystemAdmin/StoreFrontSysAdmin/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				StoreFront target = GStoreDb.StoreFronts.FindById(id);
				if (target == null)
				{
					//storefront not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Store Front. Store Front not found. It may have been deleted by another user. StoreFrontId: " + id);
				}
				string storeFrontName = target.Name;
				string storeFrontFolder = target.Folder;
				string storeFrontFolderToMap = target.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath);

				bool deleted = GStoreDb.StoreFronts.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Store Front Deleted", "Store Front '" + storeFrontName + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
					if (System.IO.Directory.Exists(Server.MapPath(storeFrontFolderToMap)))
					{
						AddUserMessage("Store Front folders not deleted", "Store Front folder '" + storeFrontFolderToMap + "' was not deleted from the file system. You will need to delete the folder manually.", AppHtmlHelpers.UserMessageType.Info);
					}
				}
			}
			catch (Exception ex)
			{

				throw new ApplicationException("Error deleting StoreFront.  See inner exception for errors.  Related child tables may still have data to be deleted. StoreFrontId: " + id, ex);
			}
			return RedirectToAction("Index");
		}

		protected SelectList ClientList()
		{
			var query = GStoreDb.Clients.All().OrderBy(c => c.Order).ThenBy(c => c.ClientId);
			IQueryable<SelectListItem> items = query.Select(c => new SelectListItem
			{
				Value = c.ClientId.ToString(),
				Text = c.Name + " [" + c.ClientId + "]"
			});
			return new SelectList(items, "Value", "Text");
		}

		protected SelectList ClientFilterList(int? id)
		{
			int filterId = 0;
			if (id.HasValue)
			{
				filterId = id.Value;
			}
			List<SelectListItem> items = new List<SelectListItem>();
			items.Add(new SelectListItem()
			{
				Value = string.Empty,
				Text = (!id.HasValue ? "[SELECTED] " : string.Empty) + "All",
				Selected = !id.HasValue
			});

			var query = GStoreDb.Clients.All().OrderBy(c=> c.Order).ThenBy(c => c.ClientId);
			IQueryable<SelectListItem> clients = query.Select(c => new SelectListItem
			{
				Value = c.ClientId.ToString(),
				Text = (c.ClientId == filterId ? "[SELECTED] ": string.Empty) + c.Name + " [" + c.ClientId + "]",
				Selected = (c.ClientId == filterId )
			});
			items.AddRange(clients);
			return new SelectList(items, "Value", "Text");
		}

		protected SelectList StoreErrorPageList()
		{
			SelectListItem itemNone = new SelectListItem();
			itemNone.Value = null;
			itemNone.Text = "(GStore System Default Error Page)";
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(itemNone);

			if (CurrentStoreFrontOrNull == null)
			{
				return new SelectList(list, "Value", "Text");
			}

			var query = CurrentStoreFrontOrNull.Pages.OrderBy(pg => pg.Order).ThenBy(pg => pg.PageId);
			IEnumerable<SelectListItem> items = query.Select(pg => new SelectListItem
			{
				Value = pg.PageId.ToString(),
				Text = pg.Name + " [" + pg.PageId + "]"
			});

			if (items.Count() > 0)
			{
				list.AddRange(items);
			}

			return new SelectList(list, "Value", "Text");
		}

		protected SelectList NotFoundPageList()
		{
			SelectListItem itemNone = new SelectListItem();
			itemNone.Value = null;
			itemNone.Text = "(GStore System Default Not Found Page)";
			List<SelectListItem> list = new List<SelectListItem>();
			list.Add(itemNone);
			
			if (CurrentStoreFrontOrNull == null)
			{
				return new SelectList(list, "Value", "Text");
			}

			var query = CurrentStoreFrontOrNull.Pages.OrderBy(pg => pg.Order).ThenBy(pg => pg.PageId);
			IEnumerable<SelectListItem> items = query.Select(pg => new SelectListItem
			{
				Value = pg.PageId.ToString(),
				Text = pg.Name + " [" + pg.PageId + "]"
			});

			if (items.Count() > 0)
			{
				list.AddRange(items);
			}

			return new SelectList(list, "Value", "Text");
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
				+ (p.StoreFrontId.HasValue ? " - Store '" + p.StoreFront.Name + "' [" + p.StoreFrontId + "]": " (no store)")
				+ (p.ClientId.HasValue ? " - Client '" + p.Client.Name + "' [" + p.ClientId + "]" : " (no client)")
			});

			return new SelectList(items, "Value", "Text");
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

		private static void CreateStoreFrontFolders(string basePath)
		{
			CreateFolderIfNotExists(basePath + "\\ErrorPages");
			CreateFolderIfNotExists(basePath + "\\Fonts");
			CreateFolderIfNotExists(basePath + "\\Images");
			CreateFolderIfNotExists(basePath + "\\Scripts");
			CreateFolderIfNotExists(basePath + "\\StoreFronts");
			CreateFolderIfNotExists(basePath + "\\Styles");
		}

		/// <summary>
		/// Creates a folder if it does not exist
		/// </summary>
		/// <param name="folder"></param>
		private static void CreateFolderIfNotExists(string folderPath)
		{
			if (!System.IO.Directory.Exists(folderPath))
			{
				System.IO.Directory.CreateDirectory(folderPath);
				System.Diagnostics.Trace.WriteLine("--File System: Created folder: " + folderPath);
			}
		}

	}
}
