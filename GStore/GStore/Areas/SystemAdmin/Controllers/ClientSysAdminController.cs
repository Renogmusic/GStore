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
    public class ClientSysAdminController : BaseClasses.SystemAdminBaseController 
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
        public ActionResult Create(Client client)
        {
			//check if client name or folder is already taken
			ValidateClientName(client);
			ValidateClientFolder(client);

            if (ModelState.IsValid)
            {
				Data.IGstoreDb db = GStoreDb;
				client = db.Clients.Create(client);
				client.UpdateAuditFields(CurrentUserProfileOrThrow);
				client = GStoreDb.Clients.Add(client);
				AddUserMessage("Client Added", "Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "] created successfully!", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.SaveChanges();

				string clientFolderVirtualPath = client.ClientVirtualDirectoryToMap(Request.ApplicationPath);
				try
				{
					SysAdminActivationExtensions.CreateClientFolders(Server.MapPath(clientFolderVirtualPath));
					AddUserMessage("Client Folders Created", "Client Folders were created in '" + clientFolderVirtualPath.ToHtml(), AppHtmlHelpers.UserMessageType.Success);
				}
				catch (Exception ex)
				{
					AddUserMessage("Error Creating Client Folders!", "There was an error creating client folders in '" + clientFolderVirtualPath.ToHtml() + "'. You will need to create the folder manually. Error: " + ex.Message.ToHtml(), AppHtmlHelpers.UserMessageType.Warning);
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
        public ActionResult Edit(Client client)
		{
			ValidateClientName(client);
			ValidateClientFolder(client);

            if (ModelState.IsValid)
			{
				Client originalValues = GStoreDb.Clients.Single(c => c.ClientId == client.ClientId);
				string originalFolderName = originalValues.Folder;
				string originalFolderToMap = originalValues.ClientVirtualDirectoryToMap(Request.ApplicationPath);

				client.UpdateAuditFields(CurrentUserProfileOrThrow);
				client = GStoreDb.Clients.Update(client);
				AddUserMessage("Client Updated", "Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "] updated!", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.SaveChanges();

				string originalClientFolder = Server.MapPath(originalFolderToMap);
				string newClientFolder = Server.MapPath(client.ClientVirtualDirectoryToMap(Request.ApplicationPath));

				if (client.Folder == originalFolderName && !System.IO.Directory.Exists(newClientFolder))
				{
					SysAdminActivationExtensions.CreateClientFolders(newClientFolder);
					AddUserMessage("Folders Created", "Client folders were not found, so they were created in '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'", AppHtmlHelpers.UserMessageType.Info);
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
							AddUserMessage("Folder Moved", "Client folder name was changed, so the client folder was moved from '" + originalFolderToMap.ToHtml() + "' to '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'", AppHtmlHelpers.UserMessageType.Success);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Moving Client Folder!", "There was an error moving the client folder from '" + originalFolderToMap.ToHtml() + "' to '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'. You will need to move the folder manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
						}
					}
					else
					{
						try
						{
							SysAdminActivationExtensions.CreateClientFolders(newClientFolder);
							AddUserMessage("Folders Created", "Client folders were created: " + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml(), AppHtmlHelpers.UserMessageType.Info);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Creating Client Folders!", "There was an error creating the client folders in '" + client.ClientVirtualDirectoryToMap(Request.ApplicationPath).ToHtml() + "'. You will need to create the folders manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
						}
					}
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
			Data.IGstoreDb db = GStoreDb; 
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
					AddUserMessage("Client Deleted", "Client '" + clientName.ToHtml() + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
					if (System.IO.Directory.Exists(Server.MapPath(clientFolderToMap)))
					{
						AddUserMessage("Client folders not deleted", "Client folder '" + clientFolderToMap.ToHtml() + "' was not deleted from the file system. You will need to delete the folder manually.", AppHtmlHelpers.UserMessageType.Info);
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
			Data.IGstoreDb db = GStoreDb;
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
					AddUserMessage("Client Deleted", "Client '" + clientName.ToHtml() + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
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
			output.AppendLine("ClientRoles: " + db.ClientRoles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("ClientRoleActions: " + db.ClientRoleActions.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("PageTemplates: " + db.PageTemplates.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("PageTemplateSections: " + db.PageTemplateSections.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Themes: " + db.Themes.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("WebForms: " + db.WebForms.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("ValueLists: " + db.ValueLists.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("ValueListItems: " + db.ValueListItems.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("WebForms: " + db.WebForms.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("WebFormFields: " + db.WebFormFields.Where(c => c.ClientId == clientId).Count());

			output.AppendLine("--Store Front Linked Records--");
			output.AppendLine("StoreFronts: " + db.StoreFronts.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("ClientUserRoles: " + db.ClientUserRoles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Carts: " + db.Carts.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("CartItems: " + db.CartItems.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Discounts: " + db.Discounts.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("NavBarItems: " + db.NavBarItems.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Notifications: " + db.Notifications.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("NotificationLink: " + db.NotificationLinks.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Pages: " + db.Pages.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("PageSections: " + db.PageSections.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("ProductCategories: " + db.ProductCategories.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("Products: " + db.Products.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("ProductReviews: " + db.ProductReviews.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("StoreBindings: " + db.StoreBindings.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("StoreFrontConfigurations: " + db.StoreFrontConfigurations.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("UserProfiles: " + db.UserProfiles.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("WebFormResponses: " + db.WebFormResponses.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("WebFormFieldResponses: " + db.WebFormFieldResponses.Where(c => c.ClientId == clientId).Count());

			output.AppendLine("--event logs--");
			output.AppendLine("BadRequests: " + db.BadRequests.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("FileNotFoundLogs: " + db.FileNotFoundLogs.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("PageViewEvent: " + db.PageViewEvents.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("SecurityEvents: " + db.SecurityEvents.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("SystemEvents: " + db.SystemEvents.Where(c => c.ClientId == clientId).Count());
			output.AppendLine("UserActionEvents: " + db.UserActionEvents.Where(c => c.ClientId == clientId).Count());

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

			output.AppendLine("Deleting main records...");
			db.ClientRoles.DeleteRange(db.ClientRoles.Where(c => c.ClientId == clientId));
			db.ClientRoleActions.DeleteRange(db.ClientRoleActions.Where(c => c.ClientId == clientId));
			db.PageTemplates.DeleteRange(db.PageTemplates.Where(c => c.ClientId == clientId));
			db.PageTemplateSections.DeleteRange(db.PageTemplateSections.Where(c => c.ClientId == clientId));
			db.Themes.DeleteRange(db.Themes.Where(c => c.ClientId == clientId));
			db.WebForms.DeleteRange(db.WebForms.Where(c => c.ClientId == clientId));
			db.ValueLists.DeleteRange(db.ValueLists.Where(c => c.ClientId == clientId));
			db.ValueListItems.DeleteRange(db.ValueListItems.Where(c => c.ClientId == clientId));
			db.WebForms.DeleteRange(db.WebForms.Where(c => c.ClientId == clientId));
			db.WebFormFields.DeleteRange(db.WebFormFields.Where(c => c.ClientId == clientId));
			output.AppendLine("Deleted main records!");


			output.AppendLine("Deleting storefront records...");
			db.StoreFronts.DeleteRange(db.StoreFronts.Where(c => c.ClientId == clientId));
			db.ClientUserRoles.DeleteRange(db.ClientUserRoles.Where(c => c.ClientId == clientId));
			db.Carts.DeleteRange(db.Carts.Where(c => c.ClientId == clientId));
			db.CartItems.DeleteRange(db.CartItems.Where(c => c.ClientId == clientId));
			db.Discounts.DeleteRange(db.Discounts.Where(c => c.ClientId == clientId));
			db.NavBarItems.DeleteRange(db.NavBarItems.Where(c => c.ClientId == clientId));
			db.Notifications.DeleteRange(db.Notifications.Where(c => c.ClientId == clientId));
			db.NotificationLinks.DeleteRange(db.NotificationLinks.Where(c => c.ClientId == clientId));
			db.Pages.DeleteRange(db.Pages.Where(c => c.ClientId == clientId));
			db.PageSections.DeleteRange(db.PageSections.Where(c => c.ClientId == clientId));
			db.ProductCategories.DeleteRange(db.ProductCategories.Where(c => c.ClientId == clientId));
			db.Products.DeleteRange(db.Products.Where(c => c.ClientId == clientId));
			db.ProductReviews.DeleteRange(db.ProductReviews.Where(c => c.ClientId == clientId));
			db.StoreBindings.DeleteRange(db.StoreBindings.Where(c => c.ClientId == clientId));
			db.StoreFrontConfigurations.DeleteRange(db.StoreFrontConfigurations.Where(c => c.ClientId == clientId));
			db.UserProfiles.DeleteRange(db.UserProfiles.Where(c => c.ClientId == clientId));
			db.WebFormResponses.DeleteRange(db.WebFormResponses.Where(c => c.ClientId == clientId));
			db.WebFormFieldResponses.DeleteRange(db.WebFormFieldResponses.Where(c => c.ClientId == clientId));
			output.AppendLine("Deleted storefront records!");

			if (deleteEventLogs)
			{
				output.AppendLine("Deleting event logs...");
				db.BadRequests.DeleteRange(db.BadRequests.Where(c => c.ClientId == clientId));
				db.FileNotFoundLogs.DeleteRange(db.FileNotFoundLogs.Where(c => c.ClientId == clientId));
				db.PageViewEvents.DeleteRange(db.PageViewEvents.Where(c => c.ClientId == clientId));
				db.SecurityEvents.DeleteRange(db.SecurityEvents.Where(c => c.ClientId == clientId));
				db.SystemEvents.DeleteRange(db.SystemEvents.Where(c => c.ClientId == clientId));
				db.UserActionEvents.DeleteRange(db.UserActionEvents.Where(c => c.ClientId == clientId));
				output.AppendLine("Deleted event logs!");
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

			return output.ToString();
		}
    }
}
