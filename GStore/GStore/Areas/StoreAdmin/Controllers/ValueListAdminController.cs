using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Areas.StoreAdmin.ViewModels;
using GStore.AppHtmlHelpers;
using GStore.Models;
using GStore.Models.ViewModels;


namespace GStore.Areas.StoreAdmin.Controllers
{
	public class ValueListAdminController : BaseClasses.StoreAdminBaseController
	{
		[AuthorizeGStoreAction(GStoreAction.ValueLists_Manager)]
		public ActionResult Manager(string SortBy, bool? SortAscending)
		{
			IOrderedQueryable<ValueList> valueLists = CurrentClientOrThrow.ValueLists.AsQueryable().ApplySort(this, SortBy, SortAscending);

			ValueListManagerAdminViewModel viewModel = new ValueListManagerAdminViewModel(CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow, valueLists);
			return View("Manager", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ValueLists_Create)]
		public ActionResult Create(string Tab)
		{
			Models.ValueList valueList = GStoreDb.ValueLists.Create();
			valueList.SetDefaultsForNew(CurrentClientOrThrow);
			ValueListEditAdminViewModel viewModel = new ValueListEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, valueList, Tab, isStoreAdminEdit: true, isCreatePage: true);
			return View("Create", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.ValueLists_Create)]
		public virtual ActionResult Create(ValueListEditAdminViewModel viewModel)
		{

			Client client = CurrentClientOrThrow;
			bool nameIsValid = GStoreDb.ValidateValueListName(this, viewModel.Name, CurrentClientOrThrow.ClientId, null);

			if (nameIsValid && ModelState.IsValid)
			{
				try
				{
					ValueList valueList = null;
					valueList = GStoreDb.CreateValueList(viewModel, CurrentClientOrThrow, CurrentUserProfileOrThrow);
					AddUserMessage("Value List Created!", "Value List '" + valueList.Name.ToHtml() + "' [" + valueList.ValueListId + "] was created successfully for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", AppHtmlHelpers.UserMessageType.Success);
					if (CurrentStoreFrontOrThrow.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.ValueLists_View))
					{
						return RedirectToAction("Details", new { id = valueList.ValueListId });
					}
					return RedirectToAction("Index", "StoreAdmin");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Value List '" + viewModel.Name.ToHtml() + "' for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Value List!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Value List Create Error", "There was an error with your entry for new Value List '" + viewModel.Name.ToHtml() + "' for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]. Please correct it below and save.", AppHtmlHelpers.UserMessageType.Danger);
			}
			viewModel.IsStoreAdminEdit = true;
			viewModel.IsCreatePage = true;
			viewModel.IsActiveDirect = !(viewModel.IsPending || (viewModel.StartDateTimeUtc > DateTime.UtcNow) || (viewModel.EndDateTimeUtc < DateTime.UtcNow));

			return View("Create", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ValueLists_Edit)]
		public ActionResult Edit(int? id, string Tab, string SortBy, bool? SortAscending, int? valueListItemId)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ValueListId = null");
			}

			Client client = CurrentClientOrThrow;
			Models.ValueList valueList = client.ValueLists.Where(p => p.ValueListId == id.Value).SingleOrDefault();
			if (valueList == null)
			{
				AddUserMessage("Value List not found", "Sorry, the Value List you are trying to edit cannot be found. Value List id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			ValueListEditAdminViewModel viewModel = new ValueListEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, valueList, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);

			ViewData["ValueListItemId"] = valueListItemId;
			return View("Edit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.WebForms_Edit)]
		public virtual PartialViewResult UpdateValueListAjax(int? id, ValueListEditAdminViewModel valueListViewModel, ValueListItemEditAdminViewModel[] listItems, string fastAddValueListItem)
		{
			if (!id.HasValue)
			{
				return HttpBadRequestPartial("id is null");
			}

			if (valueListViewModel.ValueListId == 0)
			{
				return HttpBadRequestPartial("Value List Id in view model is 0");
			}

			if (valueListViewModel.ValueListId != id.Value)
			{
				return HttpBadRequestPartial("Value List Id mismatch. Parameter value: '" + id.Value + "' != view model value: " + valueListViewModel.ValueListId);
			}

			Client client = CurrentClientOrThrow;
			ValueList valueListToUpdate = client.ValueLists.SingleOrDefault(vl => vl.ValueListId == valueListViewModel.ValueListId);

			if (valueListToUpdate == null)
			{
				throw new ApplicationException("Value List not found in client Value Lists. Value List Id: " + valueListToUpdate.ValueListId + " Client '" + client.Name + "' [" + client.ClientId + "]");
			}

			bool nameIsValid = GStoreDb.ValidateValueListName(this, valueListToUpdate.Name, client.ClientId, valueListToUpdate.ValueListId);
			bool fastAddIsValid = false;
			if (!string.IsNullOrWhiteSpace(fastAddValueListItem))
			{
				fastAddIsValid = GStoreDb.ValidateValueListFastAddItemName(this, fastAddValueListItem, valueListToUpdate, null);
			}

			if (nameIsValid && ModelState.IsValid)
			{
				ValueList valueList = null;
				try
				{
					valueList = GStoreDb.UpdateValueList(valueListViewModel, client, CurrentUserProfileOrThrow);

					if (listItems != null && listItems.Count() > 0)
					{
						foreach (ValueListItemEditAdminViewModel listItem in listItems)
						{
							if (listItem.ValueListId != id.Value)
							{
								throw new ApplicationException("ValueListId mismatch for list item. ValueList id: " + id.Value + " listItem.ValueListId: " + listItem.ValueListId);
							}
							ValueListItem listItemUpdated = GStoreDb.UpdateValueListItem(listItem, client, CurrentUserProfileOrThrow);
						}
					}
					valueListViewModel = new ValueListEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, valueList, isStoreAdminEdit: true, activeTab: valueListViewModel.ActiveTab, sortBy: valueListViewModel.SortBy, sortAscending: valueListViewModel.SortAscending);

					if (fastAddIsValid)
					{
						ValueListItem newItem = GStoreDb.CreateValueListItemFastAdd(valueListViewModel, fastAddValueListItem, client, CurrentUserProfileOrThrow);
						AddUserMessage("Value List Item Created!", "Value List Item '" + newItem.Name.ToHtml() + "' [" + newItem.ValueListItemId + "] created successfully.", UserMessageType.Success);
						ModelState.Remove("fastAddValueListItem");
					}

					AddUserMessage("Value List Changes Saved!", "Value List '" + valueList.Name.ToHtml() + "' [" + valueList.ValueListId + "] saved successfully for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", AppHtmlHelpers.UserMessageType.Success);

					this.ModelState.Clear();
					return PartialView("_ValueListEditPartial", valueListViewModel);
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while saving your changes to Value List '" + valueListViewModel.Name + "' [" + valueListViewModel.ValueListId + "] for Client: '" + client.Name + "' [" + client.ClientId + "] \nError: '" + ex.GetType().FullName + "'";

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): '" + ex.ToString() + "'";
					}
					AddUserMessage("Error Saving Value List!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Value List Edit Error", "There was an error with your entry for Value List " + valueListViewModel.Name.ToHtml() + " [" + valueListViewModel.ValueListId + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]. Please correct it.", AppHtmlHelpers.UserMessageType.Danger);
			}

			foreach (string key in this.ModelState.Keys.Where(k => k.StartsWith("ValueListItems[")).ToList())
			{
				this.ModelState.Remove(key);
			}


			valueListViewModel.IsStoreAdminEdit = true;
			return PartialView("_ValueListEditPartial", valueListViewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ValueLists_View, GStoreAction.ValueLists_Edit)]
		public ActionResult Details(int? id, string Tab, string SortBy, bool? SortAscending)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ValueListId = null");
			}

			Client client = CurrentClientOrThrow;
			Models.ValueList valueList = client.ValueLists.Where(p => p.ValueListId == id.Value).SingleOrDefault();
			if (valueList == null)
			{
				AddUserMessage("Value List not found", "Sorry, the Value List you are trying to view cannot be found. Value List id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			ValueListEditAdminViewModel viewModel = new ValueListEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, valueList, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
			return View("Details", viewModel);
		}


		[AuthorizeGStoreAction(true, GStoreAction.ValueLists_View, GStoreAction.ValueLists_Delete)]
		public ActionResult Delete(int? id, string Tab, string SortBy, bool? SortAscending)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ValueListId = null");
			}

			Client client = CurrentClientOrThrow;
			Models.ValueList valueList = client.ValueLists.Where(p => p.ValueListId == id.Value).SingleOrDefault();
			if (valueList == null)
			{
				AddUserMessage("Value List not found", "Sorry, the Value List you are trying to Delete cannot be found. Value List id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			ValueListEditAdminViewModel viewModel = new ValueListEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, valueList, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
			return View("Delete", viewModel);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.ValueLists_Delete)]
		public ActionResult DeleteConfirmed(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Value List Id = null");
			}

			Client client = CurrentClientOrThrow;
			Models.ValueList valueList = client.ValueLists.Where(p => p.ValueListId == id.Value).SingleOrDefault();
			if (valueList == null)
			{
				AddUserMessage("Value List not found", "Sorry, the Value List you are trying to Delete cannot be found. It may have been deleted already. Value List id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			string valueListName = valueList.Name;
			try
			{
				List<ValueListItem> valueListItemsToDelete = valueList.ValueListItems.ToList();
				foreach (ValueListItem listItem in valueListItemsToDelete)
				{
					GStoreDb.ValueListItems.Delete(listItem);
				}
				bool deleted = GStoreDb.ValueLists.DeleteById(id.Value);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Value List Deleted", "Value List '" + valueListName.ToHtml() + "' [" + id + "] was deleted successfully. Value List Items deleted: " + valueListItemsToDelete.Count + ".", AppHtmlHelpers.UserMessageType.Success);
					return RedirectToAction("Manager");
				}
				AddUserMessage("Value List Delete Error", "There was an error deleting Value List '" + valueListName.ToHtml() + "' [" + id + "]. It may have already been deleted.", AppHtmlHelpers.UserMessageType.Warning);
				return RedirectToAction("Manager");

			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Value List '" + valueListName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Value List Delete Error", errorMessage.ToHtml(), AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("Manager");
			}
		}
	}
}
