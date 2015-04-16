using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.SystemAdmin;
using GStoreData.Models;

namespace GStoreWeb.Areas.SystemAdmin.Controllers
{
	public class StoreFrontSysAdminController : AreaBaseController.SystemAdminAreaBaseController
	{

		// GET: SystemAdmin/StoreFrontSysAdmin
		public ActionResult Index(int? clientId, string SortBy, bool? SortAscending)
		{
			clientId = FilterClientIdRaw();

			IQueryable<StoreFront> query = null;
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					query = GStoreDb.StoreFronts.All();
				}
				else if (clientId.Value == 0)
				{
					query = GStoreDb.StoreFronts.Where(sb => sb.ClientId == null);
				}
				else
				{
					query = GStoreDb.StoreFronts.Where(sb => sb.ClientId == clientId.Value);
				}
			}
			else
			{
				query = GStoreDb.StoreFronts.All();
			}

			IOrderedQueryable<StoreFront> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontsBreadcrumb(htmlHelper, clientId, false);
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

			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, storeFront.ClientId, storeFront, false);
			return View(storeFront);
		}

		public ActionResult RecordSummary(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("store front id is null");
			}
			IGstoreDb db = GStoreDb;
			StoreFront storeFront = db.StoreFronts.FindById(id.Value);
			if (storeFront == null)
			{
				return HttpNotFound();
			}
			ViewData.Add("RecordSummary", ChildRecordSummary(storeFront, GStoreDb));
			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, storeFront.ClientId, storeFront, false);
			return View(storeFront);
		}

		// GET: SystemAdmin/StoreFrontSysAdmin/Create
		public ActionResult Create(int? clientId)
		{
			if (GStoreDb.Clients.IsEmpty())
			{
				AddUserMessage("No clients in database.", "There are no clients in the database. To Create a Store Front you need to create a Client first.", UserMessageType.Warning);
				return RedirectToAction("Create", "ClientSysAdmin");
			}

			Client client = null;
			if (clientId.HasValue)
			{
				client = GStoreDb.Clients.FindById(clientId.Value);
			}
			StoreFront model = GStoreDb.StoreFronts.Create();
			model.SetDefaultsForNew(client);
			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, clientId, model, false);
			return View(model);
		}

		// POST: SystemAdmin/StoreFrontSysAdmin/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(StoreFront storeFront, bool? createDefaultConfig, bool? populateProducts, bool? populateDiscounts, bool? populatePages, int? themeId)
		{
			if (!(createDefaultConfig ?? false) && (populatePages ?? false))
			{
				ModelState.AddModelError("createDefaultConfig", "You must select check the Create Default Configuration box when using Load Simple Sample Pages");
			}
			if ((storeFront.CurrentConfigOrAny() == null) && !(createDefaultConfig ?? false) && (populateProducts ?? false))
			{
				ModelState.AddModelError("createDefaultConfig", "You must select check the Create Default Configuration box when using Load Sample Products");
			}

			if (ModelState.IsValid)
			{
				IGstoreDb db = GStoreDb;
				storeFront = db.StoreFronts.Create(storeFront);
				storeFront.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeFront = db.StoreFronts.Add(storeFront);
				db.SaveChanges();
				AddUserMessage("Store Front Added", "Store Front [" + storeFront.StoreFrontId + "] for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "] was created successfully!", UserMessageType.Success);

				ActionResult configResult = null;
				if (createDefaultConfig.HasValue && createDefaultConfig.Value)
				{
					configResult = CreateConfig(storeFront.StoreFrontId, themeId);
				}

				if (populateDiscounts ?? false)
				{
					db.CreateSeedDiscounts(storeFront);
					AddUserMessage("Populated Discounts", "Sample Discounts are Loaded", UserMessageType.Success);
				}
				if (populateProducts ?? false)
				{
					if (storeFront.CurrentConfigOrAny() == null)
					{
						AddUserMessage("Could not Populate Products", "Could not populate products. Store Front does not have an active configuration", UserMessageType.Danger);
					}
					else
					{
						db.CreateSeedProducts(storeFront.CurrentConfigOrAny());
						AddUserMessage("Populated Products", "Sample Products, Bundles, and Categories are Loaded", UserMessageType.Success);
					}
				}
				if (populatePages ?? false)
				{
					if (storeFront.CurrentConfigOrAny() == null)
					{
						AddUserMessage("Could not Populate Pages", "Could not populate pages. Store Front does not have an active configuration", UserMessageType.Danger);
					}
					else
					{
						db.CreateSeedPages(storeFront.CurrentConfigOrAny());
						AddUserMessage("Populated Pages", "Simple Pages with Menu Links are Loaded", UserMessageType.Success);
					}
				}

				if (configResult != null)
				{
					return configResult;
				}
				return RedirectToAction("Index");
			}
			int? clientId = null;
			if (storeFront.ClientId != default(int))
			{
				clientId = storeFront.ClientId;
			}

			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, storeFront.ClientId, storeFront, false);
			return View(storeFront);
		}

		/// <summary>
		/// Creates a configuration where there is none for a storefront
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public ActionResult CreateConfig(int? id, int? themeId)
		{
			if (!id.HasValue || id.Value == 0)
			{
				return HttpBadRequest("storeFrontId is null or 0");
			}
			StoreFront storeFront = GStoreDb.StoreFronts.FindById(id.Value);
			if (storeFront == null)
			{
				return HttpBadRequest("storeFront not found by id: " + id.Value);
			}

			StoreFrontConfiguration configToCopyFrom = storeFront.CurrentConfigOrAny();
			StoreFrontConfiguration newConfig = GStoreDb.StoreFrontConfigurations.Create();

			if (configToCopyFrom != null)
			{
				newConfig = newConfig.UpdateValuesFromEntity(configToCopyFrom);
				newConfig.StoreFrontConfigurationId = 0;
				newConfig.StoreFront = storeFront;
				newConfig.Client = storeFront.Client;
			}
			else
			{
				UserProfile profile = GStoreDb.SeedAutoMapUserBestGuess();
				if (profile == null)
				{
					AddUserMessage("Config Create Error!", "No users found to link to new configuration for client '" + storeFront.Client.Name + "' [" + storeFront.ClientId + "]. Method: SeedAutoMapUserBestGuess"
						+ "<br/><a href=\"" + Url.Action("Create", "UserProfileSysAdmin") + "\">Click HERE to create a new user profile for this client.</a>", UserMessageType.Danger);
					return RedirectToAction("Create", "UserProfileSysAdmin", new { clientId = storeFront.ClientId, storeFrontId = storeFront.StoreFrontId });
				}

				Theme theme = null;
				if (themeId.HasValue && themeId != 0)
				{
					theme = storeFront.Client.Themes.SingleOrDefault(t => t.ThemeId == themeId.Value);
				}
				if (theme == null)
				{
					theme = storeFront.Client.Themes.FirstOrDefault(t => t.FolderName.ToLower() == Settings.AppDefaultThemeFolderName.ToLower());
				}
				if (theme == null)
				{
					theme = storeFront.Client.Themes.AsQueryable().ApplyDefaultSort().FirstOrDefault();
				}

				if (theme == null)
				{
					AddUserMessage("Config Create Error!", "No Themes found to link to new configuration for client '" + storeFront.Client.Name + "' [" + storeFront.ClientId + "]. Method: FirstTheme"
						+ "<br/> <a href=\"" + Url.Action("Create", "ThemeSysAdmin") + "\">Click HERE to create a new theme for this client.</a>", UserMessageType.Danger);
					return RedirectToAction("Create", "ThemeSysAdmin", new { clientId = storeFront.ClientId });
				}
				
				newConfig.StoreFront = storeFront;
				newConfig.StoreFrontId = storeFront.StoreFrontId;
				newConfig.SetDefaultsForNew(storeFront.Client);
				newConfig.AccountAdmin = profile;
				newConfig.WelcomePerson = profile;
				newConfig.OrderAdmin = profile;
				newConfig.RegisteredNotify = profile;
				newConfig.AccountTheme = theme;
				newConfig.AdminTheme = theme;
				newConfig.CartTheme = theme;
				newConfig.BlogTheme = theme;
				newConfig.ChatTheme = theme;
				newConfig.CheckoutTheme = theme;
				newConfig.CatalogTheme = theme;
				newConfig.CatalogAdminTheme = theme;
				newConfig.DefaultNewPageTheme = theme;
				newConfig.NotificationsTheme = theme;
				newConfig.OrdersTheme = theme;
				newConfig.OrderAdminTheme = theme;
				newConfig.ProfileTheme = theme;
				newConfig.ApplyDefaultCartConfig();
				newConfig.ApplyDefaultCheckoutConfig();
				newConfig.ApplyDefaultOrdersConfig();
			}

			newConfig = GStoreDb.StoreFrontConfigurations.Add(newConfig);
			GStoreDb.SaveChanges();

			string storeFrontRootFolder = newConfig.StoreFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath);

			string clientFrontFolderVirtualPath = storeFront.ClientVirtualDirectoryToMap(Request.ApplicationPath);
			string storeFrontFolderVirtualPath = newConfig.StoreFrontVirtualDirectoryToMapThisConfig(Request.ApplicationPath);

			if (!System.IO.Directory.Exists(Server.MapPath(clientFrontFolderVirtualPath)))
			{
				storeFront.Client.CreateClientFolders(Request.ApplicationPath, Server);
				AddUserMessage("Client Folders Created.", "Client Folders Created for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "] in '" + clientFrontFolderVirtualPath.ToHtml() + "'.", UserMessageType.Success);
			}

			if (!System.IO.Directory.Exists(Server.MapPath(storeFrontFolderVirtualPath)))
			{
				bool result = newConfig.CreateStoreFrontFolders(Request.ApplicationPath, Server);
				if (result)
				{
					AddUserMessage("Store Front Folders Created.", "Store Front Folders Created for New Configuration '" + newConfig.ConfigurationName.ToHtml() + "' [" + newConfig.StoreFrontConfigurationId + "] in '" + storeFrontFolderVirtualPath.ToHtml() + "'.", UserMessageType.Success);
				}
				else
				{
					AddUserMessage("File system Error!", "Store Front Folders could not be created for New Configuration '" + newConfig.ConfigurationName.ToHtml() + "' [" + newConfig.StoreFrontConfigurationId + "] in '" + storeFrontFolderVirtualPath.ToHtml() + "'.", UserMessageType.Success);
				}
			}

			AddUserMessage("Store Front Configuration Created.", "Store Front Configuration '" + newConfig.ConfigurationName.ToHtml() + "' [" + newConfig.StoreFrontConfigurationId + "] created successfully for Store Front '" + newConfig.Name.ToHtml() + "' [" + newConfig.StoreFrontId + "].", UserMessageType.Success);

			return RedirectToAction("Index");
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

			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, storeFront.ClientId, storeFront, false);
			return View(storeFront);
		}

		// POST: SystemAdmin/StoreFrontSysAdmin/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(StoreFront storeFront, bool? createDefaultConfig, int? themeId, bool? populateProducts, bool? populateDiscounts, bool? populatePages)
		{
			if ((storeFront.CurrentConfigOrAny() == null) && !(createDefaultConfig ?? false) && (populatePages ?? false))
			{
				ModelState.AddModelError("createDefaultConfig", "You must select check the Create Default Configuration box when using Load Simple Sample Pages");
			}
			if ((storeFront.CurrentConfigOrAny() == null) && !(createDefaultConfig ?? false) && (populateProducts ?? false))
			{
				ModelState.AddModelError("createDefaultConfig", "You must select check the Create Default Configuration box when using Load Sample Products");
			}
			if (ModelState.IsValid)
			{
				IGstoreDb db = GStoreDb;
				storeFront.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeFront = db.StoreFronts.Update(storeFront);
				db.SaveChanges();

				if (createDefaultConfig.HasValue && createDefaultConfig.Value)
				{
					ActionResult configResult = CreateConfig(storeFront.StoreFrontId, themeId);
				}

				if (storeFront.CurrentConfigOrAny() != null)
				{
					StoreFrontConfiguration config = storeFront.CurrentConfigOrAny();
					if (!config.StoreFrontFoldersAllExist(Request.ApplicationPath, Server))
					{
						config.CreateStoreFrontFolders(Request.ApplicationPath, Server);
						AddUserMessage("Store Front Folders Sync'd", "Store Front Folder sync'd or created for StoreFront '" + config.Name.ToHtml() + "' [" + storeFront.StoreFrontId + "] for client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "]", UserMessageType.Success);
					}
				}
				AddUserMessage("Store Front Updated", "Store Front [" + storeFront.StoreFrontId + "] for client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "] was updated successfully!", UserMessageType.Success);

				if (populateDiscounts ?? false)
				{
					db.CreateSeedDiscounts(storeFront);
					AddUserMessage("Populated Discounts", "Sample Discounts are Loaded", UserMessageType.Success);
				}
				if (populateProducts ?? false)
				{
					if (storeFront.CurrentConfigOrAny() == null)
					{
						AddUserMessage("Could not Populate Products", "Could not populate products. Store Front does not have an active configuration", UserMessageType.Danger);
					}
					else
					{
						db.CreateSeedProducts(storeFront.CurrentConfigOrAny());
						AddUserMessage("Populated Products", "Sample Products, Bundles, and Categories are Loaded", UserMessageType.Success);
					}
				}
				if (populatePages ?? false)
				{
					if (storeFront.CurrentConfigOrAny() == null)
					{
						AddUserMessage("Could not Populate Pages", "Could not populate Pages. Store front does not have an active configuration", UserMessageType.Danger);
					}
					else
					{
						db.CreateSeedPages(storeFront.CurrentConfigOrAny());
						AddUserMessage("Populated Pages", "Simple Pages with Menu Links are Loaded", UserMessageType.Success);
					}
				}

				return RedirectToAction("Index");
			}

			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, storeFront.ClientId, storeFront, false);
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

		public ActionResult ActivateConfig(int id)
		{
			this.ActivateStoreFrontConfigOnly(id);
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
			IGstoreDb db = GStoreDb;
			StoreFront storeFront = db.StoreFronts.FindById(id.Value);
			if (storeFront == null)
			{
				return HttpNotFound();
			}
			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, storeFront.ClientId, storeFront, false);
			return View(storeFront);
		}

		// POST: SystemAdmin/StoreFrontSysAdmin/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			StoreFront target = GStoreDb.StoreFronts.FindById(id);
			if (target == null)
			{
				//storefront not found, already deleted? overpost?
				throw new ApplicationException("Error deleting Store Front. Store Front not found. It may have been deleted by another user. StoreFrontId: " + id);
			}
			int clientId = target.ClientId;
			string clientName = target.Client.Name;

			try
			{
				List<StoreFrontConfiguration> configsToDelete = target.StoreFrontConfigurations.ToList();
				foreach (var config  in configsToDelete)
				{
					GStoreDb.StoreFrontConfigurations.Delete(config);
				}

				bool deleted = GStoreDb.StoreFronts.Delete(target);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Store Front Deleted", "Store Front [" + id + "] for client '" + clientName.ToHtml() + "' [" + clientId + "] was deleted successfully.", UserMessageType.Success);
				}
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				AddUserMessage("Store Front Delete Error!", "Error deleting Store Front.\nYou will need to delete with a Seek and Destroy.\nThis Store Front may have child records. Store Front id " + id + "<br/>Exception:" + ex.ToString(), UserMessageType.Danger);
				return RedirectToAction("Delete", new { id = id });
			}
		}

		public ActionResult SeekAndDestroy(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("store front id is null");
			}
			IGstoreDb db = GStoreDb;
			StoreFront storeFront = db.StoreFronts.FindById(id.Value);
			if (storeFront == null)
			{
				return HttpNotFound();
			}
			ViewData.Add("SeekAndDestroySummary", ChildRecordSummary(storeFront, GStoreDb));
			this.BreadCrumbsFunc = htmlHelper => this.StoreFrontBreadcrumb(htmlHelper, storeFront.ClientId, storeFront, false);
			return View(storeFront);
		}

		// POST: SystemAdmin/ClientSysAdmin/Delete/5
		[HttpPost, ActionName("SeekAndDestroy")]
		[ValidateAntiForgeryToken]
		public ActionResult SeekAndDestroyConfirmed(int id, bool? deleteEventLogs, bool? deleteFolders)
		{
			StoreFront target = GStoreDb.StoreFronts.FindById(id);
			if (target == null)
			{
				//client not found, already deleted? overpost?
				AddUserMessage("Store Front Delete Error!", "Store Front not found Store Front Id: " + id + "<br/>Store Front may have been deleted by another user.", UserMessageType.Danger);
				return RedirectToAction("Index");
			}
			StoreFrontConfiguration config = target.CurrentConfigOrAny();

			string name = config == null ? "id [" + target.StoreFrontId + "]" : config.Name;
			string folder = config == null ? null : config.Folder;
			string folderToMap = config == null ? null : config.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath);

			try
			{
				string report = SeekAndDestroyChildRecordsNoSave(target, deleteEventLogs ?? true, deleteFolders ?? true);
				AddUserMessage("Seek and Destroy report.", report.ToHtmlLines(), UserMessageType.Info);
				bool deleted = GStoreDb.StoreFronts.DeleteById(id);
				GStoreDb.SaveChangesEx(false, false, false, false);
				if (deleted)
				{
					AddUserMessage("Store Front Deleted", "Store Front '" + name.ToHtml() + "' [" + id + "] was deleted successfully.", UserMessageType.Success);
				}
			}
			catch (Exception ex)
			{
				AddUserMessage("Store Front Seek and Destroy Error!", "Error with Seek and Destroy for Store Front '" + name.ToHtml() + "' [" + id + "].<br/>This Store Front may have child records.<br/>Exception:" + ex.ToString(), UserMessageType.Danger);
			}
			return RedirectToAction("Index");
		}

		protected string ChildRecordSummary(StoreFront storeFront, IGstoreDb db)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (db == null)
			{
				throw new ArgumentNullException("db");
			}

			StringBuilder output = new StringBuilder();
			int storeFrontId = storeFront.StoreFrontId;
			string name = storeFront.CurrentConfigOrAny() == null ? "id " + storeFront.StoreFrontId : storeFront.CurrentConfigOrAny().Name;

			output.AppendLine("--File and Child Record Summary for Store Front '" + name.ToHtml() + " [" + storeFrontId + "]--");

			output.AppendLine("--Store Front Linked Records--");
			output.AppendLine("Store Fronts: " + db.StoreFronts.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Client User Roles: " + db.ClientUserRoles.Where(sf => sf.ScopeStoreFrontId == storeFrontId).Count());
			output.AppendLine("Carts: " + db.Carts.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Cart Items: " + db.CartItems.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Cart Bundles: " + db.CartBundles.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Discounts: " + db.Discounts.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Nav Bar Items: " + db.NavBarItems.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Notifications: " + db.Notifications.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Notification Links: " + db.NotificationLinks.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Pages: " + db.Pages.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Page Sections: " + db.PageSections.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Product Categories: " + db.ProductCategories.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Products: " + db.Products.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Product Bundles: " + db.ProductBundles.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Product Bundle Items: " + db.ProductBundleItems.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Store Bindings: " + db.StoreBindings.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Store Front Configurations: " + db.StoreFrontConfigurations.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("User Profiles: " + db.UserProfiles.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Web Form Responses: " + db.WebFormResponses.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Web Form Field Responses: " + db.WebFormFieldResponses.Where(sf => sf.StoreFrontId == storeFrontId).Count());

			output.AppendLine("--event logs--");
			output.AppendLine("Bad Requests: " + db.BadRequests.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("File Not Found Logs: " + db.FileNotFoundLogs.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Page View Events: " + db.PageViewEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Security Events: " + db.SecurityEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("System Events: " + db.SystemEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("User Action Events: " + db.UserActionEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());

			output.AppendLine("--File System--");
			string folderPath = Server.MapPath(storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath));
			output.AppendLine("Virtual Directory: '" + storeFront.StoreFrontVirtualDirectoryToMap(Request.ApplicationPath) + "'");
			output.AppendLine("Physicial Directory: '" + folderPath + "'");
			output.AppendLine("Folder Exists: " + System.IO.Directory.Exists(folderPath));
			if (System.IO.Directory.Exists(folderPath))
			{
				output.AppendLine("SubFolders: " + System.IO.Directory.EnumerateDirectories(folderPath, "*", System.IO.SearchOption.AllDirectories).Count());
				output.AppendLine("Files: " + System.IO.Directory.EnumerateFiles(folderPath, "*", System.IO.SearchOption.AllDirectories).Count());
			}

			return output.ToString();
		}

		/// <summary>
		/// deletes all child records and returns a string summary of the records deleted
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		protected string SeekAndDestroyChildRecordsNoSave(StoreFront storeFront, bool deleteEventLogs, bool deleteFolders)
		{
			StringBuilder output = new StringBuilder();
			IGstoreDb db = GStoreDb;
			int storeFrontId = storeFront.StoreFrontId;

			string virtualPath = null;
			if (storeFront.CurrentConfigOrAny() != null)
			{
				virtualPath = storeFront.StoreFrontVirtualDirectoryToMapAnyConfig(Request.ApplicationPath);
			}

			int deletedRecordCount = 0;

			int deletedStoreFrontRecords = 0;
			output.AppendLine("Deleting storefront records...");
			deletedStoreFrontRecords += db.StoreFronts.DeleteRange(db.StoreFronts.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.ClientUserRoles.DeleteRange(db.ClientUserRoles.Where(sf => sf.ScopeStoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.Carts.DeleteRange(db.Carts.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.CartItems.DeleteRange(db.CartItems.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.CartBundles.DeleteRange(db.CartBundles.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.Discounts.DeleteRange(db.Discounts.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.NavBarItems.DeleteRange(db.NavBarItems.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.Notifications.DeleteRange(db.Notifications.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.NotificationLinks.DeleteRange(db.NotificationLinks.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.Pages.DeleteRange(db.Pages.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.PageSections.DeleteRange(db.PageSections.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.ProductCategories.DeleteRange(db.ProductCategories.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.Products.DeleteRange(db.Products.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.ProductBundles.DeleteRange(db.ProductBundles.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.ProductBundleItems.DeleteRange(db.ProductBundleItems.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.StoreBindings.DeleteRange(db.StoreBindings.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.StoreFrontConfigurations.DeleteRange(db.StoreFrontConfigurations.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.UserProfiles.DeleteRange(db.UserProfiles.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.WebFormResponses.DeleteRange(db.WebFormResponses.Where(sf => sf.StoreFrontId == storeFrontId));
			deletedStoreFrontRecords += db.WebFormFieldResponses.DeleteRange(db.WebFormFieldResponses.Where(sf => sf.StoreFrontId == storeFrontId));
			output.AppendLine("Deleted " + deletedStoreFrontRecords.ToString("N0") + " storefront records!");
			deletedRecordCount += deletedStoreFrontRecords;

			if (deleteEventLogs)
			{
				output.AppendLine("Deleting event logs...");
				db.BadRequests.DeleteRange(db.BadRequests.Where(sf => sf.StoreFrontId == storeFrontId));
				db.FileNotFoundLogs.DeleteRange(db.FileNotFoundLogs.Where(sf => sf.StoreFrontId == storeFrontId));
				db.PageViewEvents.DeleteRange(db.PageViewEvents.Where(sf => sf.StoreFrontId == storeFrontId));
				db.SecurityEvents.DeleteRange(db.SecurityEvents.Where(sf => sf.StoreFrontId == storeFrontId));
				db.SystemEvents.DeleteRange(db.SystemEvents.Where(sf => sf.StoreFrontId == storeFrontId));
				db.UserActionEvents.DeleteRange(db.UserActionEvents.Where(sf => sf.StoreFrontId == storeFrontId));
				output.AppendLine("Deleted event logs!");
			}

			if (deleteFolders)
			{
				if (virtualPath == null)
				{
					output.AppendLine("Warning: No store front configuration, no folder name for StoreFront files. They might not exist or have been orphaned when the configuration was deleted.");
				}
				else
				{
					string folderPath = Server.MapPath(virtualPath);
					output.AppendLine("Deleting Files...");
					output.AppendLine("Virtual Directory: '" + virtualPath + "'");
					output.AppendLine("Physicial Directory: '" + folderPath + "'");
					output.AppendLine("Folder Exists: " + System.IO.Directory.Exists(folderPath));
					if (System.IO.Directory.Exists(folderPath))
					{
						try
						{
							System.IO.Directory.Delete(folderPath, true);
							AddUserMessage("Store Front Folders Deleted.", "Store Front folder was deleted successfully.", UserMessageType.Info);
							output.AppendLine("Deleted Files!");
						}
						catch (Exception)
						{
							AddUserMessage("Delete folders failed.", "Delete folders failed. You will have to delete the Store Front folder manually.", UserMessageType.Warning);
							output.AppendLine("Delete files failed!");
						}
					}
					else
					{
						output.AppendLine("Deleted Files!");
					}
				}
			}

			output.AppendLine("Total Records deleted: " + deletedRecordCount.ToString("N0"));
			return output.ToString();
		}

	}
}
