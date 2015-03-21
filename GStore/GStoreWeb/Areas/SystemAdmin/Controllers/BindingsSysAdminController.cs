using System;
using System.Linq;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.SystemAdmin;
using GStoreData.Models;

namespace GStoreWeb.Areas.SystemAdmin.Controllers
{
	public class BindingsSysAdminController : AreaBaseController.SystemAdminAreaBaseController
	{

		public ActionResult Index(int? clientId, int? storeFrontId, string SortBy, bool? SortAscending)
		{
			clientId = FilterClientIdRaw();

			StoreFront storeFront = null;
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

			if (storeFrontId.HasValue && storeFrontId.Value != 0)
			{
				query = query.Where(b => b.StoreFrontId == storeFrontId.Value);
				storeFront = GStoreDb.StoreFronts.FindById(storeFrontId.Value);
			}

			ViewData.Add("StoreFrontId", storeFrontId);
			IOrderedQueryable<StoreBinding> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			this.BreadCrumbsFunc = html => this.BindingsBreadcrumb(html, clientId, storeFront, false);
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
			this.BreadCrumbsFunc = html => this.BindingBreadcrumb(html, storeBinding.ClientId, storeBinding.StoreFront, storeBinding);

			return View(storeBinding);
		}

		public ActionResult Create(int? clientId, int? storeFrontId)
		{
			if (GStoreDb.Clients.IsEmpty())
			{
				AddUserMessage("No Clients in database.", "You must create a Client and Store Front to add a store binding.", UserMessageType.Warning);
				return RedirectToAction("Create", "ClientSysAdmin");
			}
			if (GStoreDb.StoreFronts.IsEmpty())
			{
				AddUserMessage("No Store Fronts in database.", "You must create a Store Front to add a store binding.", UserMessageType.Warning);
				return RedirectToAction("Create", "StoreFrontSysAdmin");
			}

			StoreBinding model = GStoreDb.StoreBindings.Create();
			model.SetDefaultsForNew(Request, clientId, storeFrontId);
			this.BreadCrumbsFunc = html => this.BindingBreadcrumb(html, model.ClientId, model.StoreFront, model);

			if (clientId == null)
			{
				Client firstClient = GStoreDb.Clients.Where(c => c.StoreFronts.Any()).ApplyDefaultSort().First();
				model.Client = firstClient;
				model.ClientId = firstClient.ClientId;
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(StoreBinding storeBinding, bool? clientIdChanged)
		{
			if (ModelState.IsValid && !(clientIdChanged ?? false))
			{
				storeBinding = GStoreDb.StoreBindings.Create(storeBinding);
				storeBinding.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeBinding = GStoreDb.StoreBindings.Add(storeBinding);
				GStoreDb.SaveChanges();
				AddUserMessage("Store Binding Added", "Store Binding [" + storeBinding.StoreBindingId + "] created successfully!", UserMessageType.Success);
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

			this.BreadCrumbsFunc = html => this.BindingBreadcrumb(html, storeBinding.ClientId, storeBinding.StoreFront, storeBinding);
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

			this.BreadCrumbsFunc = html => this.BindingBreadcrumb(html, storeBinding.ClientId, storeBinding.StoreFront, storeBinding);
			return View(storeBinding);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(StoreBinding storeBinding, bool? clientIdChanged)
		{
			if (ModelState.IsValid && !(clientIdChanged ?? false))
			{
				storeBinding.UpdateAuditFields(CurrentUserProfileOrThrow);
				storeBinding = GStoreDb.StoreBindings.Update(storeBinding);
				GStoreDb.SaveChanges();
				AddUserMessage("Store Binding Saved", "Store Binding [" + storeBinding.StoreBindingId + "] updated successfully.", UserMessageType.Success);
				return RedirectToAction("Index");
			}

			this.BreadCrumbsFunc = html => this.BindingBreadcrumb(html, storeBinding.ClientId, storeBinding.StoreFront, storeBinding);
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
			IGstoreDb db = GStoreDb;
			StoreBinding storeBinding = db.StoreBindings.FindById(id.Value);
			if (storeBinding == null)
			{
				return HttpNotFound();
			}
			this.BreadCrumbsFunc = html => this.BindingBreadcrumb(html, storeBinding.ClientId, storeBinding.StoreFront, storeBinding);
			return View(storeBinding);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				StoreBinding target = GStoreDb.StoreBindings.FindById(id);
				string storeFrontName = target.StoreFront.CurrentConfigOrAny().Name;

				if (target == null)
				{
					//storeBinding not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Store Binding. Store Binding not found. It may have been deleted by another user. StoreBindingId: " + id);
				}
				bool deleted = GStoreDb.StoreBindings.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Store Binding Deleted", "Store Binding for Store Front '" + storeFrontName.ToHtml() + "' Binding [" + id + "] was deleted successfully.", UserMessageType.Success);
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
