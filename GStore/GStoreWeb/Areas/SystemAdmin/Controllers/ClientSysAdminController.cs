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
	public class ClientSysAdminController : AreaBaseController.SystemAdminAreaBaseController
    {

		// GET: SystemAdmin/ClientSysAdmin
        public ActionResult Index(string SortBy, bool? SortAscending)
        {
			IQueryable<Client> query = null;
			int? filterClientIdRaw = FilterClientIdRaw();

			if (!filterClientIdRaw.HasValue)
			{
				query = GStoreDb.Clients.Where(c => c.ClientId == null);
			}
			else if (filterClientIdRaw.Value == 0)
			{
				query = GStoreDb.Clients.Where(c => c.ClientId == null);
			}
			else if (filterClientIdRaw.Value == -1)
			{
				query = GStoreDb.Clients.All();
			}
			else
			{
				query = GStoreDb.Clients.Where(c => c.ClientId == filterClientIdRaw.Value);
			}

			IOrderedQueryable<Client> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			this.BreadCrumbsFunc = html => this.ClientsBreadcrumb(html);

			return View(queryOrdered.ToList());
        }

        // GET: SystemAdmin/ClientSysAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
				return HttpBadRequest("Client id is null");
            }
			Client client = GStoreDb.Clients.FindById(id.Value);
			
            if (client == null)
            {
                return HttpNotFound();
            }
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, id, false);
			return View(client);
        }

		public ActionResult RecordSummary(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("client id is null");
			}
			IGstoreDb db = GStoreDb;
			Client client = db.Clients.FindById(id.Value);
			if (client == null)
			{
				return HttpNotFound();
			}
			ViewData.Add("RecordSummary", ChildRecordSummary(client, GStoreDb));
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, id, false);
			return View(client);
		}

		// GET: SystemAdmin/ClientSysAdmin/Create
        public ActionResult Create()
		{
			Client model = GStoreDb.Clients.Create();
			model.SetDefaultsForNew(GStoreDb);
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, model.ClientId, false, "New"); 
			return View(model);
        }

        // POST: SystemAdmin/ClientSysAdmin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Client client, bool? populateThemes, bool? populatePageTemplates, bool? populateSampleWebForms)
        {
			//check if client name or folder is already taken
			ValidateClientName(client);
			ValidateClientFolder(client);

            if (ModelState.IsValid)
            {
				IGstoreDb db = GStoreDb;
				client = db.Clients.Create(client);
				client.UpdateAuditFields(CurrentUserProfileOrThrow);
				client = db.Clients.Add(client);
				AddUserMessage("Client Added", "Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "] created successfully!", UserMessageType.Success);
				db.SaveChanges();

				string clientFolderVirtualPath = client.ClientVirtualDirectoryToMap(Request.ApplicationPath);
				try
				{
					client.CreateClientFolders(Request.ApplicationPath, Server);
					AddUserMessage("Client Folders Created", "Client Folders were created in '" + clientFolderVirtualPath.ToHtml(), UserMessageType.Success);
				}
				catch (Exception ex)
				{
					AddUserMessage("Error Creating Client Folders!", "There was an error creating client folders in '" + clientFolderVirtualPath.ToHtml() + "'. You will need to create the folder manually. Error: " + ex.Message.ToHtml(), UserMessageType.Warning);
				}

				if (populateThemes.HasValue && populateThemes.Value)
				{
					List<Theme> newThemes = db.CreateSeedThemes(client);
					AddUserMessage("Populated Themes", "New Themes added: " + newThemes.Count, UserMessageType.Success);
				}
				if (populatePageTemplates.HasValue && populatePageTemplates.Value)
				{
					List<PageTemplate> newPageTemplates = db.CreateSeedPageTemplates(client);
					AddUserMessage("Populated Page Templates", "New Page Templates added: " + newPageTemplates.Count, UserMessageType.Success);
				}
				if (populateSampleWebForms.HasValue && populateSampleWebForms.Value)
				{
					List<WebForm> newWebForms = db.CreateSeedWebForms(client);
					AddUserMessage("Populated Sample Web Forms", "New Forms added: " + newWebForms.Count, UserMessageType.Success);
				}

                return RedirectToAction("Index");
            }
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, 0, false, "New"); 
			return View(client);
        }

        // GET: SystemAdmin/ClientSysAdmin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
				return HttpBadRequest("Client id is null");
			}
			Client client = GStoreDb.Clients.FindById(id.Value);
            if (client == null)
            {
                return HttpNotFound();
            }
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, id, false); 
			return View(client);
        }

        // POST: SystemAdmin/ClientSysAdmin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Edit(Client client, bool? populateThemes, bool? populatePageTemplates, bool? populateSampleWebForms)
		{
			ValidateClientName(client);
			ValidateClientFolder(client);

            if (ModelState.IsValid)
			{
				IGstoreDb db = GStoreDb;
				Client originalValues = db.Clients.Single(c => c.ClientId == client.ClientId);
				string originalFolderName = originalValues.Folder;
				string originalFolderToMap = originalValues.ClientVirtualDirectoryToMap(Request.ApplicationPath);

				client.UpdateAuditFields(CurrentUserProfileOrThrow);
				client = db.Clients.Update(client);
				AddUserMessage("Client Updated", "Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "] updated!", UserMessageType.Success);
				db.SaveChanges();

				if (populateThemes.HasValue && populateThemes.Value)
				{
					List<Theme> newThemes = db.CreateSeedThemes(client);
					AddUserMessage("Populated Themes", "New Themes added: " + newThemes.Count, UserMessageType.Success);
				}
				if (populatePageTemplates.HasValue && populatePageTemplates.Value)
				{
					List<PageTemplate> newPageTemplates = db.CreateSeedPageTemplates(client);
					AddUserMessage("Populated Page Templates", "New Page Templates added: " + newPageTemplates.Count, UserMessageType.Success);
				}
				if (populateSampleWebForms.HasValue && populateSampleWebForms.Value)
				{
					List<WebForm> newWebForms = db.CreateSeedWebForms(client);
					AddUserMessage("Populated Sample Web Forms", "New Forms added: " + newWebForms.Count, UserMessageType.Success);
				}
				string originalClientFolder = Server.MapPath(originalFolderToMap);
				string newClientFolder = Server.MapPath(client.ClientVirtualDirectoryToMap(Request.ApplicationPath));

				if (client.Folder == originalFolderName && !System.IO.Directory.Exists(newClientFolder))
				{
					client.CreateClientFolders(Request.ApplicationPath, Server);
					AddUserMessage("Folders Created", "Client folders were not found, so they were created in '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'", UserMessageType.Info);
				}
				else if (client.Folder != originalFolderName)
				{
					//detect if folder name has changed
				
					//default behavior is to move the old folder to the new name
					if (System.IO.Directory.Exists(originalClientFolder))
					{
						try
						{
							System.IO.Directory.Move(originalClientFolder, newClientFolder);
							AddUserMessage("Folder Moved", "Client folder name was changed, so the client folder was moved from '" + originalFolderToMap.ToHtml() + "' to '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'", UserMessageType.Success);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Moving Client Folder!", "There was an error moving the client folder from '" + originalFolderToMap.ToHtml() + "' to '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'. You will need to move the folder manually. Error: " + ex.Message, UserMessageType.Warning);
						}
					}
					else
					{
						try
						{
							client.CreateClientFolders(Request.ApplicationPath, Server);
							AddUserMessage("Folders Created", "Client folders were created: " + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml(), UserMessageType.Info);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Creating Client Folders!", "There was an error creating the client folders in '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'. You will need to create the folders manually. Error: " + ex.Message, UserMessageType.Warning);
						}
					}
				}
				else
				{
					client.CreateClientFolders(Request.ApplicationPath, Server);
				}

                return RedirectToAction("Index");
            }
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, client.ClientId, false); 
			return View(client);
        }

		public ActionResult Activate(int id)
		{
			this.ActivateClientOnly(id);
			if (Request.UrlReferrer != null)
			{
				return Redirect(Request.UrlReferrer.ToString());

			}
			return RedirectToAction("Index");
		}

        // GET: SystemAdmin/ClientSysAdmin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
				return HttpBadRequest("Client id is null");
			}
			IGstoreDb db = GStoreDb; 
			Client client = db.Clients.FindById(id.Value);
            if (client == null)
            {
                return HttpNotFound();
            }
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, id, false); 
			return View(client);
        }

        // POST: SystemAdmin/ClientSysAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			try
			{
				Client target = GStoreDb.Clients.FindById(id);
				if (target == null)
				{
					//client not found, already deleted? overpost?
					AddUserMessage("Client Delete Error!", "Client not found ClientId: " + id + "<br/>Client may have been deleted by another user.", UserMessageType.Danger);
					return RedirectToAction("Index");
				}
				string clientName = target.Name;
				string clientFolder = target.Folder;
				string clientFolderToMap = target.ClientVirtualDirectoryToMap(Request.ApplicationPath);
				
				bool deleted = GStoreDb.Clients.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Client Deleted", "Client '" + clientName.ToHtml() + "' [" + id + "] was deleted successfully.", UserMessageType.Success);
					if (System.IO.Directory.Exists(Server.MapPath(clientFolderToMap)))
					{
						AddUserMessage("Client folders not deleted", "Client folder '" + clientFolderToMap.ToHtml() + "' was not deleted from the file system. You will need to delete the folder manually.", UserMessageType.Info);
					}
				}
			}
			catch (Exception ex)
			{
				AddUserMessage("Client Delete Error!", "Error deleting client.\nYou will need to delete with a Seek and Destroy.\nThis client may have child records. ClientId: " + id + "<br/>Exception:" + ex.ToString(), UserMessageType.Danger);
				return RedirectToAction("Delete", new { id = id });
			}
            return RedirectToAction("Index");
        }

		public ActionResult SeekAndDestroy(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("client id is null");
			}
			IGstoreDb db = GStoreDb;
			Client client = db.Clients.FindById(id.Value);
			if (client == null)
			{
				return HttpNotFound();
			}
			ViewData.Add("SeekAndDestroySummary", ChildRecordSummary(client, GStoreDb));
			this.BreadCrumbsFunc = html => this.ClientBreadcrumb(html, id, false);
			return View(client);
		}

		// POST: SystemAdmin/ClientSysAdmin/Delete/5
		[HttpPost, ActionName("SeekAndDestroy")]
		[ValidateAntiForgeryToken]
		public ActionResult SeekAndDestroyConfirmed(int id, bool? deleteEventLogs, bool? deleteFolders)
		{
			Client target = GStoreDb.Clients.FindById(id);
			if (target == null)
			{
				//client not found, already deleted? overpost?
				AddUserMessage("Client Delete Error!", "Client not found ClientId: " + id + "<br/>Client may have been deleted by another user.", UserMessageType.Danger);
				return RedirectToAction("Index");
			}
			string clientName = target.Name;
			string clientFolder = target.Folder;
			string clientFolderToMap = target.ClientVirtualDirectoryToMap(Request.ApplicationPath);

			try
			{
				string report = SeekAndDestroyChildRecordsNoSave(target, deleteEventLogs ?? true, deleteFolders ?? true);
				AddUserMessage("Seek and Destroy report.", report.ToHtmlLines(), UserMessageType.Info);
				bool deleted = GStoreDb.Clients.DeleteById(id);
				GStoreDb.SaveChangesEx(false, false, false, false);
				if (deleted)
				{
					AddUserMessage("Client Deleted", "Client '" + clientName.ToHtml() + "' [" + id + "] was deleted successfully.", UserMessageType.Success);
				}
			}
			catch (Exception ex)
			{
				AddUserMessage("Client Seek and Destroy Error!", "Error with Seek and Destroy for Client '" + target.Name.ToHtml() + "' [" + target.ClientId + "].<br/>This client may have child records.<br/>Exception:" + ex.ToString(), UserMessageType.Danger);
			}
			return RedirectToAction("Index");
		}

		protected string ChildRecordSummary(Client client, IGstoreDb db)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (db == null)
			{
				throw new ArgumentNullException("db");
			}

			StringBuilder output = new StringBuilder();
			int clientId = client.ClientId;

			output.AppendLine("--File and Child Record Summary for Client '" + client.Name.ToHtml() + " [" + client.ClientId + "]--");
			output.AppendLine("--Client Linked Records--");
			output.AppendLine("Client Roles: " + db.ClientRoles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Client Role Actions: " + db.ClientRoleActions.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Page Templates: " + db.PageTemplates.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Page Template Sections: " + db.PageTemplateSections.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Themes: " + db.Themes.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Web Forms: " + db.WebForms.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Value Lists: " + db.ValueLists.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Value List Items: " + db.ValueListItems.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Web Forms: " + db.WebForms.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Web Form Fields: " + db.WebFormFields.Where(c => c.ClientId == clientId).Count());

			output.AppendLine("--Store Front Linked Records--");
			output.AppendLine("Store Fronts: " + db.StoreFronts.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Client User Roles: " + db.ClientUserRoles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Carts: " + db.Carts.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Cart Items: " + db.CartItems.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Cart Bundles: " + db.CartBundles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Discounts: " + db.Discounts.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Nav Bar Items: " + db.NavBarItems.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Notifications: " + db.Notifications.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Notification Links: " + db.NotificationLinks.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Pages: " + db.Pages.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Page Sections: " + db.PageSections.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Product Categories: " + db.ProductCategories.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Products: " + db.Products.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Product Bundles: " + db.ProductBundles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Product Bundle Items: " + db.ProductBundleItems.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Store Bindings: " + db.StoreBindings.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Store Front Configurations: " + db.StoreFrontConfigurations.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("User Profiles: " + db.UserProfiles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Web Form Responses: " + db.WebFormResponses.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Web Form FieldResponses: " + db.WebFormFieldResponses.Where(c => c.ClientId == clientId).Count());

			output.AppendLine("--event logs--");
			output.AppendLine("Bad Requests: " + db.BadRequests.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("File Not Found Logs: " + db.FileNotFoundLogs.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Page View Events: " + db.PageViewEvents.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Security Events: " + db.SecurityEvents.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("System Events: " + db.SystemEvents.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("User Action Events: " + db.UserActionEvents.Where(c => c.ClientId == clientId).Count());

			output.AppendLine("--File System--");
			string clientFolderPath = Server.MapPath(client.ClientVirtualDirectoryToMap(Request.ApplicationPath));
			output.AppendLine("Virtual Directory: '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath) + "'");
			output.AppendLine("Physicial Directory: '" + clientFolderPath + "'");
			output.AppendLine("Folder Exists: " + System.IO.Directory.Exists(clientFolderPath));
			if (System.IO.Directory.Exists(clientFolderPath))
			{
				output.AppendLine("SubFolders: " + System.IO.Directory.EnumerateDirectories(clientFolderPath, "*", System.IO.SearchOption.AllDirectories).Count());
				output.AppendLine("Files: " + System.IO.Directory.EnumerateFiles(clientFolderPath, "*", System.IO.SearchOption.AllDirectories).Count());
			}
			
			return output.ToString();
		}

		/// <summary>
		/// deletes all child records and returns a string summary of the records deleted
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		protected string SeekAndDestroyChildRecordsNoSave(Client client, bool deleteEventLogs, bool deleteFolders)
		{
			StringBuilder output = new StringBuilder();
			IGstoreDb db = GStoreDb;
			int clientId = client.ClientId;
			int deletedRecordCount = 0;

			int deletedMainRecords = 0;
			output.AppendLine("Deleting main records...");
			deletedMainRecords += db.ClientRoles.DeleteRange(db.ClientRoles.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.ClientRoleActions.DeleteRange(db.ClientRoleActions.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.PageTemplates.DeleteRange(db.PageTemplates.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.PageTemplateSections.DeleteRange(db.PageTemplateSections.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.Themes.DeleteRange(db.Themes.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.WebForms.DeleteRange(db.WebForms.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.ValueLists.DeleteRange(db.ValueLists.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.ValueListItems.DeleteRange(db.ValueListItems.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.WebForms.DeleteRange(db.WebForms.Where(c => c.ClientId == clientId));
			deletedMainRecords += db.WebFormFields.DeleteRange(db.WebFormFields.Where(c => c.ClientId == clientId));
			output.AppendLine("Deleted " + deletedMainRecords.ToString("N0") + " main records!");
			deletedRecordCount += deletedMainRecords;

			int deletedStoreFrontRecords = 0;
			output.AppendLine("Deleting storefront records...");
			deletedStoreFrontRecords += db.StoreFronts.DeleteRange(db.StoreFronts.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.ClientUserRoles.DeleteRange(db.ClientUserRoles.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.Carts.DeleteRange(db.Carts.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.CartItems.DeleteRange(db.CartItems.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.CartBundles.DeleteRange(db.CartBundles.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.Discounts.DeleteRange(db.Discounts.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.NavBarItems.DeleteRange(db.NavBarItems.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.Notifications.DeleteRange(db.Notifications.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.NotificationLinks.DeleteRange(db.NotificationLinks.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.Pages.DeleteRange(db.Pages.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.PageSections.DeleteRange(db.PageSections.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.ProductCategories.DeleteRange(db.ProductCategories.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.Products.DeleteRange(db.Products.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.ProductBundles.DeleteRange(db.ProductBundles.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.ProductBundleItems.DeleteRange(db.ProductBundleItems.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.StoreBindings.DeleteRange(db.StoreBindings.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.StoreFrontConfigurations.DeleteRange(db.StoreFrontConfigurations.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.UserProfiles.DeleteRange(db.UserProfiles.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.WebFormResponses.DeleteRange(db.WebFormResponses.Where(c => c.ClientId == clientId));
			deletedStoreFrontRecords += db.WebFormFieldResponses.DeleteRange(db.WebFormFieldResponses.Where(c => c.ClientId == clientId));
			output.AppendLine("Deleted " + deletedStoreFrontRecords.ToString("N0") + " storefront records!");
			deletedRecordCount += deletedStoreFrontRecords;

			if (deleteEventLogs)
			{
				int deletedEventLogs = 0;
				output.AppendLine("Deleting event logs...");
				deletedEventLogs += db.BadRequests.DeleteRange(db.BadRequests.Where(c => c.ClientId == clientId));
				deletedEventLogs += db.FileNotFoundLogs.DeleteRange(db.FileNotFoundLogs.Where(c => c.ClientId == clientId));
				deletedEventLogs += db.PageViewEvents.DeleteRange(db.PageViewEvents.Where(c => c.ClientId == clientId));
				deletedEventLogs += db.SecurityEvents.DeleteRange(db.SecurityEvents.Where(c => c.ClientId == clientId));
				deletedEventLogs += db.SystemEvents.DeleteRange(db.SystemEvents.Where(c => c.ClientId == clientId));
				deletedEventLogs += db.UserActionEvents.DeleteRange(db.UserActionEvents.Where(c => c.ClientId == clientId));
				output.AppendLine("Deleted " + deletedEventLogs.ToString("N0") + " event logs!");
				deletedRecordCount += deletedEventLogs;
			}

			if (deleteFolders)
			{
				output.AppendLine("Deleting Files...");
				string clientFolderPath = Server.MapPath(client.ClientVirtualDirectoryToMap(Request.ApplicationPath));
				output.AppendLine("Virtual Directory: '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath) + "'");
				output.AppendLine("Physicial Directory: '" + clientFolderPath + "'");
				output.AppendLine("Folder Exists: " + System.IO.Directory.Exists(clientFolderPath));
				if (System.IO.Directory.Exists(clientFolderPath))
				{
					try
					{
						System.IO.Directory.Delete(clientFolderPath, true);
						AddUserMessage("Client Folders Deleted.", "Client folder was deleted successfully.", UserMessageType.Info);
						output.AppendLine("Deleted Files!");
					}
					catch (Exception)
					{
						AddUserMessage("Delete folders failed.", "Delete folders failed. You will have to delete the client folder manually.", UserMessageType.Warning);
						output.AppendLine("Delete files failed!");
					}
				}
				else
				{
					output.AppendLine("Deleted Files!");
				}
			}

			output.AppendLine("Total Records deleted: " + deletedRecordCount.ToString("N0"));

			return output.ToString();
		}

	}
}
