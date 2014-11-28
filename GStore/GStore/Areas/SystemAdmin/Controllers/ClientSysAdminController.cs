﻿using System;
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
    public class ClientSysAdminController : BaseClasses.SystemAdminBaseController 
    {

		// GET: SystemAdmin/ClientSysAdmin
        public ActionResult Index(string SortBy, bool? SortAscending)
        {
			IQueryable<Client> query = GStoreDb.Clients.All();
			IOrderedQueryable<Client> queryOrdered = this.ApplySort(query, SortBy, SortAscending);
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
            return View(client);
        }

        // GET: SystemAdmin/ClientSysAdmin/Create
        public ActionResult Create()
		{
			Client model = GStoreDb.Clients.Create();
			model.SetDefaultsForNew();
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
				GStoreDb.SaveChanges();

				try
				{
					CreateClientFolders(Server.MapPath(client.ClientVirtualDirectoryToMap()));
					AddUserMessage("Client Folders Created", "Client Folders were created as: " + client.ClientVirtualDirectoryToMap(), AppHtmlHelpers.UserMessageType.Success);
				}
				catch (Exception ex)
				{
					AddUserMessage("Error Creating Client Folders!", "There was an error creating the client folder '" + client.ClientVirtualDirectoryToMap() + "'. You will need to create the folder manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
				}
                return RedirectToAction("Index");
            }
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
				string originalFolderToMap = originalValues.ClientVirtualDirectoryToMap();

				client.UpdateAuditFields(CurrentUserProfileOrThrow);
				client = GStoreDb.Clients.Update(client);
				GStoreDb.SaveChanges();

				//detect if folder name has changed
				if (client.Folder != originalFolderName)
				{
					//default behavior is to move the old folder to the new name
					string originalClientFolder = Server.MapPath(originalFolderToMap);
					string newClientFolder = Server.MapPath(client.ClientVirtualDirectoryToMap());
					if (System.IO.Directory.Exists(originalClientFolder))
					{
						try
						{
							System.IO.Directory.Move(originalClientFolder, newClientFolder);
							AddUserMessage("Folder Moved", "Client folder name was changed, so the client folder was moved from '" + originalFolderToMap + "' to '" + client.ClientVirtualDirectoryToMap() + "'", AppHtmlHelpers.UserMessageType.Success);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Moving Client Folder!", "There was an error moving the client folder from '" + originalFolderToMap + "' to '" + client.ClientVirtualDirectoryToMap() + "'. You will need to move the folder manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
						}
					}
					else
					{
						try
						{
							CreateClientFolders(newClientFolder);
							AddUserMessage("Folders Created", "Client folders were created: " + client.ClientVirtualDirectoryToMap(), AppHtmlHelpers.UserMessageType.Info);
						}
						catch (Exception ex)
						{
							AddUserMessage("Error Creating Client Folders!", "There was an error creating the client folder '" + client.ClientVirtualDirectoryToMap() + "'. You will need to create the folder manually. Error: " + ex.Message, AppHtmlHelpers.UserMessageType.Warning);
						}
					}
				}

                return RedirectToAction("Index");
            }
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
					throw new ApplicationException("Error deleting client. Client not found. It may be deleted by another user. ClientId: " + id);
				}
				string clientName = target.Name;
				string clientFolder = target.Folder;
				string clientFolderToMap = target.ClientVirtualDirectoryToMap();
				
				bool deleted = GStoreDb.Clients.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Client Deleted", "Client '" + clientName + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
					if (System.IO.Directory.Exists(Server.MapPath(clientFolderToMap)))
					{
						AddUserMessage("Client folders not deleted", "Client folder '" + clientFolderToMap + "' was not deleted from the file system. You will need to delete the folder manually.", AppHtmlHelpers.UserMessageType.Info);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting client. See inner exception for errors.  Related child tables may still have data to be deleted or client may be deleted by another user. ClientId: " + id, ex);
			}
            return RedirectToAction("Index");
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

		private static void CreateClientFolders(string basePath)
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
