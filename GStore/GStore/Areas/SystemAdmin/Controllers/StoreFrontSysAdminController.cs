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
using GStore.AppHtmlHelpers;
using System.Text;

namespace GStore.Areas.SystemAdmin.Controllers
{
	public class StoreFrontSysAdminController : BaseClasses.SystemAdminBaseController
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
		public ActionResult Create(StoreFront storeFront, bool? createDefaultConfig)
		{
			if (ModelState.IsValid)
			{
				storeFront = GStoreDb.StoreFronts.Create(storeFront);
				storeFront.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeFront = GStoreDb.StoreFronts.Add(storeFront);
				GStoreDb.SaveChanges();
				AddUserMessage("Store Front Added", "Store Front [" + storeFront.StoreFrontId + "] for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "] was created successfully!", AppHtmlHelpers.UserMessageType.Success);

				if (createDefaultConfig.HasValue && createDefaultConfig.Value)
				{
					return CreateConfig(storeFront.StoreFrontId);
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
		public ActionResult CreateConfig(int? id)
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
				Theme theme = storeFront.Client.Themes.AsQueryable().ApplyDefaultSort().FirstOrDefault();
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
				newConfig.RegisteredNotify = profile;
				newConfig.AccountTheme = theme;
				newConfig.AdminTheme = theme;
				newConfig.CartTheme = theme;
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
				AddUserMessage("Client Folders Created.", "Client Folders Created for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "] in '" + clientFrontFolderVirtualPath.ToHtml() + "'.", AppHtmlHelpers.UserMessageType.Success);
			}

			if (!System.IO.Directory.Exists(Server.MapPath(storeFrontFolderVirtualPath)))
			{
				newConfig.CreateStoreFrontFolders(Request.ApplicationPath, Server);
				AddUserMessage("Store Front Folders Created.", "Store Front Folders Created for New Configuration '" + newConfig.ConfigurationName.ToHtml() + "' [" + newConfig.StoreFrontConfigurationId + "] in '" + storeFrontFolderVirtualPath.ToHtml() + "'.", AppHtmlHelpers.UserMessageType.Success);
			}

			AddUserMessage("Store Front Configuration Created.", "Store Front Configuration '" + newConfig.ConfigurationName.ToHtml() + "' [" + newConfig.StoreFrontConfigurationId + "] created successfully for Store Front '" + newConfig.Name.ToHtml() + "' [" + newConfig.StoreFrontId + "].", AppHtmlHelpers.UserMessageType.Success);

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
		public ActionResult Edit(StoreFront storeFront)
		{
			if (ModelState.IsValid)
			{
				storeFront.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeFront = GStoreDb.StoreFronts.Update(storeFront);
				GStoreDb.SaveChanges();
				storeFront.CurrentConfigOrAny().CreateStoreFrontFolders(Request.ApplicationPath, Server);
				AddUserMessage("Store Front Updated", "Store Front [" + storeFront.StoreFrontId + "] for client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "] was updated successfully!", AppHtmlHelpers.UserMessageType.Success);
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
			Data.IGstoreDb db = GStoreDb;
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
					AddUserMessage("Store Front Deleted", "Store Front [" + id + "] for client '" + clientName.ToHtml() + "' [" + clientId + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
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
				return HttpBadRequest("client id is null");
			}
			Data.IGstoreDb db = GStoreDb;
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
					AddUserMessage("Store Front Deleted", "Store Front '" + name.ToHtml() + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
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
			output.AppendLine("StoreFronts: " + db.StoreFronts.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("ClientUserRoles: " + db.ClientUserRoles.Where(sf => sf.ScopeStoreFrontId == storeFrontId).Count());
			output.AppendLine("Carts: " + db.Carts.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("CartItems: " + db.CartItems.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Discounts: " + db.Discounts.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("NavBarItems: " + db.NavBarItems.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Notifications: " + db.Notifications.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("NotificationLink: " + db.NotificationLinks.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Pages: " + db.Pages.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("PageSections: " + db.PageSections.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("ProductCategories: " + db.ProductCategories.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("Products: " + db.Products.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("ProductReviews: " + db.ProductReviews.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("StoreBindings: " + db.StoreBindings.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("StoreFrontConfigurations: " + db.StoreFrontConfigurations.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("UserProfiles: " + db.UserProfiles.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("WebFormResponses: " + db.WebFormResponses.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("WebFormFieldResponses: " + db.WebFormFieldResponses.Where(sf => sf.StoreFrontId == storeFrontId).Count());

			output.AppendLine("--event logs--");
			output.AppendLine("BadRequests: " + db.BadRequests.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("FileNotFoundLogs: " + db.FileNotFoundLogs.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("PageViewEvent: " + db.PageViewEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("SecurityEvents: " + db.SecurityEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("SystemEvents: " + db.SystemEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());
			output.AppendLine("UserActionEvents: " + db.UserActionEvents.Where(sf => sf.StoreFrontId == storeFrontId).Count());

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

			output.AppendLine("Deleting storefront records...");
			db.StoreFronts.DeleteRange(db.StoreFronts.Where(sf => sf.StoreFrontId == storeFrontId));
			db.ClientUserRoles.DeleteRange(db.ClientUserRoles.Where(sf => sf.ScopeStoreFrontId == storeFrontId));
			db.Carts.DeleteRange(db.Carts.Where(sf => sf.StoreFrontId == storeFrontId));
			db.CartItems.DeleteRange(db.CartItems.Where(sf => sf.StoreFrontId == storeFrontId));
			db.Discounts.DeleteRange(db.Discounts.Where(sf => sf.StoreFrontId == storeFrontId));
			db.NavBarItems.DeleteRange(db.NavBarItems.Where(sf => sf.StoreFrontId == storeFrontId));
			db.Notifications.DeleteRange(db.Notifications.Where(sf => sf.StoreFrontId == storeFrontId));
			db.NotificationLinks.DeleteRange(db.NotificationLinks.Where(sf => sf.StoreFrontId == storeFrontId));
			db.Pages.DeleteRange(db.Pages.Where(sf => sf.StoreFrontId == storeFrontId));
			db.PageSections.DeleteRange(db.PageSections.Where(sf => sf.StoreFrontId == storeFrontId));
			db.ProductCategories.DeleteRange(db.ProductCategories.Where(sf => sf.StoreFrontId == storeFrontId));
			db.Products.DeleteRange(db.Products.Where(sf => sf.StoreFrontId == storeFrontId));
			db.ProductReviews.DeleteRange(db.ProductReviews.Where(sf => sf.StoreFrontId == storeFrontId));
			db.StoreBindings.DeleteRange(db.StoreBindings.Where(sf => sf.StoreFrontId == storeFrontId));
			db.StoreFrontConfigurations.DeleteRange(db.StoreFrontConfigurations.Where(sf => sf.StoreFrontId == storeFrontId));
			db.UserProfiles.DeleteRange(db.UserProfiles.Where(sf => sf.StoreFrontId == storeFrontId));
			db.WebFormResponses.DeleteRange(db.WebFormResponses.Where(sf => sf.StoreFrontId == storeFrontId));
			db.WebFormFieldResponses.DeleteRange(db.WebFormFieldResponses.Where(sf => sf.StoreFrontId == storeFrontId));
			output.AppendLine("Deleted storefront records!");

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

			return output.ToString();
		}

	}
}
