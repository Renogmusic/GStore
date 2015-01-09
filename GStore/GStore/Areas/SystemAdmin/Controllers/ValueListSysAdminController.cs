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

namespace GStore.Areas.SystemAdmin.Controllers
{
	public class ValueListSysAdminController : BaseClasses.SystemAdminBaseController
	{

		public ActionResult Index(int? clientId, string SortBy, bool? SortAscending)
		{
			clientId = FilterClientIdRaw();

			IQueryable<ValueList> query = null;
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					query = GStoreDb.ValueLists.All();
				}
				else if (clientId.Value == 0)
				{
					query = GStoreDb.ValueLists.Where(sb => sb.ClientId == null);
				}
				else
				{
					query = GStoreDb.ValueLists.Where(sb => sb.ClientId == clientId.Value);
				}
			}
			else
			{
				query = GStoreDb.ValueLists.All();
			}

			IOrderedQueryable<ValueList> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			this.BreadCrumbsFunc = htmlHelper => this.ValueListsBreadcrumb(htmlHelper, clientId);
			return View(queryOrdered.ToList());
		}

		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Value List Id is null");
			}
			ValueList valueList = GStoreDb.ValueLists.FindById(id.Value);
			if (valueList == null)
			{
				return HttpNotFound("Value List not found. Value List id: " + id);
			}
			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(valueList);
		}

		public ActionResult Create(int? clientId)
		{
			ValueList model = GStoreDb.ValueLists.Create();

			Client client = null;
			if (clientId.HasValue && clientId.Value > 0)
			{
				client = GStoreDb.Clients.FindById(clientId.Value);
			}
			model.SetDefaultsForNew(client);
			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, model.ClientId, model);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(ValueList valueList)
		{
			if (ModelState.IsValid)
			{
				valueList = GStoreDb.ValueLists.Create(valueList);
				valueList.UpdateAuditFields(CurrentUserProfileOrThrow);
				valueList = GStoreDb.ValueLists.Add(valueList);
				GStoreDb.SaveChanges();
				AddUserMessage("Value List Created", "Value List '" + valueList.Name.ToHtml() + "' [" + valueList.ValueListId + "] Created Successfully", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
			}
			int? clientId = null;
			if (valueList.ClientId != default(int))
			{
				clientId = valueList.ClientId;
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(valueList);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Value List Id is null");
			}
			ValueList valueList = GStoreDb.ValueLists.FindById(id.Value);
			if (valueList == null)
			{
				return HttpNotFound();
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(valueList);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(ValueList valueList)
		{
			if (ModelState.IsValid)
			{
				valueList.UpdateAuditFields(CurrentUserProfileOrThrow);
				valueList = GStoreDb.ValueLists.Update(valueList);
				GStoreDb.SaveChanges();
				AddUserMessage("Value List Updated", "Changes saved successfully to Value List '" + valueList.Name.ToHtml() + "' [" + valueList.ValueListId + "]", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(valueList);
		}

		public ActionResult Activate(int id)
		{
			this.ActivateValueList(id);
			if (Request.UrlReferrer != null)
			{
				return Redirect(Request.UrlReferrer.ToString());

			}
			return RedirectToAction("Index");
		}

		public ActionResult ActivateListItem(int id)
		{
			this.ActivateValueListItem(id);
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
				return HttpBadRequest("Value List Id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			ValueList valueList = db.ValueLists.FindById(id.Value);
			if (valueList == null)
			{
				return HttpNotFound();
			}
			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(valueList);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				ValueList target = GStoreDb.ValueLists.FindById(id);

				if (target == null)
				{
					//valueList not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Value List. Value List not found. It may have been deleted by another user. Value List Id: " + id);
				}

				List<ValueListItem> itemsToDelete = target.ValueListItems.ToList();
				foreach (ValueListItem listItem in itemsToDelete)
				{
					GStoreDb.ValueListItems.Delete(listItem);
				}

				bool deleted = GStoreDb.ValueLists.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Value List Deleted", "Value List [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
				}
				else
				{
					AddUserMessage("Deleting Value List Failed!", "Deleting Value List Failed. Value List Id: " + id, AppHtmlHelpers.UserMessageType.Danger);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting Value List.  See inner exception for errors.  Related child tables may still have data to be deleted. Value List Id: " + id, ex);
			}
			return RedirectToAction("Index");
		}

		public ActionResult ListItemIndex(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Value List id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			ValueList valueList = db.ValueLists.FindById(id.Value);
			if (valueList == null)
			{
				return HttpNotFound("Value List not found. Value List id: " + id);
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(valueList);
		}

		public ActionResult ListItemCreate(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Value List id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			ValueList valueList = db.ValueLists.FindById(id.Value);
			if (valueList == null)
			{
				return HttpNotFound("Value List not found. Value List id: " + id);
			}

			ValueListItem model = GStoreDb.ValueListItems.Create();
			model.SetDefaultsForNew(valueList);
			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ListItemCreate(ValueListItem valueListItem)
		{
			if (valueListItem.ValueListId == default(int))
			{
				return HttpBadRequest("Value List id is 0");
			}

			Data.IGstoreDb db = GStoreDb;
			ValueList valueList = db.ValueLists.FindById(valueListItem.ValueListId);
			if (valueList == null)
			{
				return HttpNotFound("Value List not found. Value List id: " + valueListItem.ValueListId);
			}

			if (valueList.ValueListItems.Any(vl => vl.Name.ToLower() == valueListItem.Name.ToLower()))
			{
				ModelState.AddModelError("Name", "An item with name '" + valueListItem.Name + "' already exists. Choose a new name or edit the original value.");
			}

			if (ModelState.IsValid)
			{
				valueListItem.ClientId = valueList.ClientId;
				valueListItem.ValueListId = valueList.ValueListId;
				valueListItem = GStoreDb.ValueListItems.Add(valueListItem);
				GStoreDb.SaveChanges();
				AddUserMessage("Value List Item Created", "Value List Item '" + valueListItem.Name.ToHtml() + "' [" + valueListItem.ValueListItemId + "] created successfully", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("ListItemIndex", new { id = valueListItem.ValueListId });
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueList.ClientId, valueList);
			return View(valueListItem);
		}

		public ActionResult ListItemEdit(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Value List Item id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			ValueListItem valueListItem = db.ValueListItems.FindById(id.Value);
			if (valueListItem == null)
			{
				return HttpNotFound("Value List Item not found. Value List Item id: " + id);
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueListItem.ClientId, valueListItem.ValueList);
			return View(valueListItem);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ListItemEdit(ValueListItem valueListItem)
		{
			if (ModelState.IsValid)
			{
				valueListItem.UpdateAuditFields(CurrentUserProfileOrThrow);
				valueListItem = GStoreDb.ValueListItems.Update(valueListItem);
				GStoreDb.SaveChanges();
				AddUserMessage("Value List Item Updated", "Changes saved successfully to Value List Item '" + valueListItem.Name.ToHtml() + "' [" + valueListItem.ValueListItemId + "]", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("ListItemIndex", new { id = valueListItem.ValueListId });
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueListItem.ClientId, valueListItem.ValueList);
			return View(valueListItem);
		}

		public ActionResult ListItemDetails(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Value List Item id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			ValueListItem valueListItem = db.ValueListItems.FindById(id.Value);
			if (valueListItem == null)
			{
				return HttpNotFound("Value List Item not found. Value List Item id: " + id);
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueListItem.ClientId, valueListItem.ValueList);
			return View(valueListItem);
		}

		public ActionResult ListItemDelete(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Value List Item id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			ValueListItem valueListItem = db.ValueListItems.FindById(id.Value);
			if (valueListItem == null)
			{
				return HttpNotFound("Value List Item not found. Value List Item id: " + id);
			}

			this.BreadCrumbsFunc = htmlHelper => this.ValueListBreadcrumb(htmlHelper, valueListItem.ClientId, valueListItem.ValueList);
			return View(valueListItem);
		}

		[HttpPost, ActionName("ListItemDelete")]
		[ValidateAntiForgeryToken]
		public ActionResult ListItemDeleteConfirmed(int id)
		{
			try
			{
				ValueListItem target = GStoreDb.ValueListItems.FindById(id);

				if (target == null)
				{
					//valueList not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Value List Item. Value List Item not found. It may have been deleted by another user. Value List Item Id: " + id);
				}

				bool deleted = GStoreDb.ValueListItems.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Value List Item Deleted", "Value List Item [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
				}
				else
				{
					AddUserMessage("Deleting Value List Item Failed!", "Deleting Value List Item Failed. Value List Id: " + id, AppHtmlHelpers.UserMessageType.Danger);
				}

				return RedirectToAction("ListItemIndex", new { id = target.ValueListId });
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting Value List Item.  See inner exception for errors.  Related child tables may still have data to be deleted. Value List Item Id: " + id, ex);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ListItemFastAdd(int valueListId, string stringValue)
		{
			if (string.IsNullOrEmpty(stringValue))
			{
				AddUserMessage("Item not added", "You must enter text to add a value list item", AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("ListItemIndex", new { id = valueListId });
			}

			ValueList valueList = GStoreDb.ValueLists.FindById(valueListId);
			if (valueList == null)
			{
				return HttpBadRequest("Value List not found by id: " + valueListId);
			}

			if (valueList.ValueListItems.Any(vl => vl.Name.ToLower() == stringValue.ToLower()))
			{
				AddUserMessage("Item not added", "Item with name '" + stringValue.ToHtml() + "' already exists in this list. Use a different item name or remove the old item first.", AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("ListItemIndex", new { id = valueListId });
			}

			ValueListItem listItem = GStoreDb.ValueListItems.Create();
			listItem.SetDefaultsForNew(valueList);
			listItem.Name = stringValue;
			listItem.Description = stringValue;
			listItem.IsPending = false;
			listItem.IsString = true;
			listItem.StringValue = stringValue;
			
			GStoreDb.ValueListItems.Add(listItem);
			GStoreDb.SaveChanges();

			AddUserMessage("Item added to List", "Item '" + stringValue.ToHtml() + "' [" + listItem.ValueListItemId + "] was successfully added to the list", AppHtmlHelpers.UserMessageType.Success);
			return RedirectToAction("ListItemIndex", new { id = valueListId });
		}

	}
}
