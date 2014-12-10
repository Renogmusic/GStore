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
using GStore.Identity;

namespace GStore.Areas.SystemAdmin.Controllers
{
	public class BindingsSysAdminController : BaseClasses.SystemAdminBaseController
	{

		public ActionResult Index(int? clientId, string SortBy, bool? SortAscending)
		{
			clientId = FilterClientIdRaw();

			IQueryable<StoreBinding> query = null;
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					query = GStoreDb.StoreBindings.All();
				}
				else if (clientId.Value == 0)
				{
					query = GStoreDb.StoreBindings.Where(sb => sb.ClientId == null);
				}
				else
				{
					query = GStoreDb.StoreBindings.Where(sb => sb.ClientId == clientId.Value);
				}
			}
			else
			{
				query = GStoreDb.StoreBindings.All();
			}

			IOrderedQueryable<StoreBinding> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			return View(queryOrdered.ToList());
		}

		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Store Binding Id is null");
			}
			StoreBinding storeBinding = GStoreDb.StoreBindings.FindById(id.Value);
			if (storeBinding == null)
			{
				return HttpNotFound();
			}
			return View(storeBinding);
		}

		public ActionResult Create(int? clientId, int? storeFrontId)
		{
			ViewBag.ClientList = ClientList();
			ViewBag.StoreFrontList = StoreFrontList(clientId);

			StoreBinding model = GStoreDb.StoreBindings.Create();
			model.SetDefaultsForNew(Request, clientId, storeFrontId);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(StoreBinding storeBinding)
		{
			if (ModelState.IsValid)
			{
				storeBinding = GStoreDb.StoreBindings.Create(storeBinding);
				storeBinding.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeBinding = GStoreDb.StoreBindings.Add(storeBinding);
				GStoreDb.SaveChanges();
				AddUserMessage("Store Binding Added", "Store Binding [" + storeBinding.StoreBindingId + "] created successfully!", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
			}
			int? clientId = null;
			if (storeBinding.ClientId != default(int))
			{
				clientId = storeBinding.ClientId;
			}

			int? storeFrontId = null;
			if (storeBinding.StoreFrontId != default(int))
			{
				storeFrontId = storeBinding.StoreFrontId;
			}

			ViewBag.ClientList = ClientList();
			ViewBag.StoreFrontList = StoreFrontList(storeBinding.ClientId);

			return View(storeBinding);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Store Binding Id is null");
			}
			StoreBinding storeBinding = GStoreDb.StoreBindings.FindById(id.Value);
			if (storeBinding == null)
			{
				return HttpNotFound();
			}
			ViewBag.ClientList = ClientList();
			ViewBag.StoreFrontList = StoreFrontList(storeBinding.ClientId);

			return View(storeBinding);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(StoreBinding storeBinding)
		{
			if (ModelState.IsValid)
			{
				storeBinding.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeBinding = GStoreDb.StoreBindings.Update(storeBinding);
				GStoreDb.SaveChanges();
				AddUserMessage("Store Binding Saved", "Store Binding [" + storeBinding.StoreBindingId + "] updated successfully.", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
			}
			ViewBag.ClientList = ClientList();
			ViewBag.StoreFrontList = StoreFrontList(storeBinding.ClientId);

			return View(storeBinding);
		}

		public ActionResult Activate(int id)
		{
			this.ActivateStoreBindingOnly(id);
			if (Request.UrlReferrer != null)
			{
				return Redirect(Request.UrlReferrer.ToString());

			}
			return RedirectToAction("Index");
		}

		public ActionResult Delete(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Store Binding Id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			StoreBinding storeBinding = db.StoreBindings.FindById(id.Value);
			if (storeBinding == null)
			{
				return HttpNotFound();
			}
			return View(storeBinding);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				StoreBinding target = GStoreDb.StoreBindings.FindById(id);
				string storeFrontName = target.StoreFront.Name;

				if (target == null)
				{
					//storeBinding not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Store Binding. Store Binding not found. It may have been deleted by another user. StoreBindingId: " + id);
				}
				bool deleted = GStoreDb.StoreBindings.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Store Binding Deleted", "Store Binding for Store Front '" + storeFrontName.ToHtml() + "' Binding [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
				}
			}
			catch (Exception ex)
			{

				throw new ApplicationException("Error deleting StoreBinding.  See inner exception for errors.  Related child tables may still have data to be deleted. StoreBindingId: " + id, ex);
			}
			return RedirectToAction("Index");
		}

	}
}
