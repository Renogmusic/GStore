using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.StoreAdmin.ViewModels;
using GStoreData.Identity;
using GStoreData.Models;


namespace GStoreWeb.Areas.StoreAdmin.Controllers
{
	public class NavBarItemAdminController : AreaBaseController.StoreAdminAreaBaseController
	{
		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Manager)]
		public ActionResult Manager(string SortBy, bool? SortAscending)
		{
			IOrderedQueryable<NavBarItem> navBarItems = CurrentStoreFrontOrThrow.NavBarItems.AsQueryable().ApplySort(this, SortBy, SortAscending);

			NavBarItemManagerAdminViewModel viewModel = new NavBarItemManagerAdminViewModel(CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow, navBarItems);
			return View("Manager", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Manager)]
		public ActionResult AdvancedManager(string SortBy, bool? SortAscending)
		{
			IOrderedQueryable<NavBarItem> navBarItems = CurrentStoreFrontOrThrow.NavBarItems.AsQueryable().ApplySort(this, SortBy, SortAscending);

			NavBarItemManagerAdminViewModel viewModel = new NavBarItemManagerAdminViewModel(CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow, navBarItems);
			return View("AdvancedManager", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Manager)]
		public ActionResult MoveItem(int? source, int? target)
		{
			if (!source.HasValue || source.Value == 0)
			{
				return HttpBadRequest("source cannot be null or 0");
			}
			if (!target.HasValue)
			{
				return HttpBadRequest("target cannot be null. Use 0 for Root");
			}

			if (source.Value == target.Value)
			{
				//nothing changed
				return RedirectToAction("Manager");
			}

			NavBarItem sourceItem = CurrentStoreFrontOrThrow.NavBarItems.SingleOrDefault(nb => nb.NavBarItemId == source.Value);
			if (sourceItem == null)
			{
				return HttpBadRequest("Source NavBarItem not found in store front NavBarItems. id: " + source.Value);
			}

			if (sourceItem.ParentNavBarItemId == target)
			{
				//target is already the direct parent of the source, no changes
				return RedirectToAction("Manager");
			}

			int? newParentNavBarItemId = (target == 0 ? null : target);
			NavBarItem targetItem = null;
			if (target.Value != 0)
			{
				targetItem = CurrentStoreFrontOrThrow.NavBarItems.SingleOrDefault(nb => nb.NavBarItemId == target.Value);
				if (targetItem == null)
				{
					return HttpBadRequest("Target NavBarItem not found in store front NavBarItems. id: " + target.Value);
				}

				//make sure target is not a child of the source
				NavBarItem parentCheck = targetItem;
				do
				{
					if (parentCheck.NavBarItemId == sourceItem.NavBarItemId)
					{
						AddUserMessage("Menu Item Move Error", "You cannot move a menu item onto its sub-items. Move the menu item to the root if you want to change the order. Source Menu Item: '" + sourceItem.Name.ToHtml() + "' [" + sourceItem.NavBarItemId + "] to Target Menu Item: '" + targetItem.Name.ToHtml() + "' [" + targetItem.NavBarItemId + "]", UserMessageType.Danger);
						return RedirectToAction("Manager");
					}
					parentCheck = parentCheck.ParentNavBarItem;
				} while (parentCheck != null);

			}

			if (sourceItem.ParentNavBarItemId != newParentNavBarItemId)
			{
				sourceItem.ParentNavBarItemId = newParentNavBarItemId;
				GStoreDb.NavBarItems.Update(sourceItem);
				GStoreDb.SaveChanges();
			}

			return RedirectToAction("Manager");
		}

		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Manager)]
		public ActionResult MoveUp(int? id)
		{
			return MoveUpOrDown(id, true);
		}

		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Manager)]
		public ActionResult MoveDown(int? id)
		{
			return MoveUpOrDown(id, false);
		}

		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Manager)]
		protected ActionResult MoveUpOrDown(int? id, bool moveUp)
		{
			if (!id.HasValue || id.Value == 0)
			{
				return HttpBadRequest("Id cannot be null or 0");
			}

			NavBarItem item = CurrentStoreFrontOrThrow.NavBarItems.SingleOrDefault(nb => nb.NavBarItemId == id.Value);
			if (item == null)
			{
				return HttpBadRequest("NavBarItem not found in store front NavBarItems. id: " + id.Value);
			}

			//find nearest sibling up or down and swap order with them
			List<NavBarItem> siblings = CurrentStoreFrontOrThrow.NavBarItems.AsQueryable().Where(nb => nb.ParentNavBarItemId == item.ParentNavBarItemId).ApplyDefaultSort().ToList();
			if (siblings.Count == 0)
			{
				//no siblings, nothing to do
				AddUserMessage("Item move", "No need to move Menu Item, it has no other items in its group. Menu Item: " + item.Name.ToHtml() + " [" + item.NavBarItemId + "]", UserMessageType.Info);
				return RedirectToAction("Manager");
			}

			int itemIndex = siblings.IndexOf(item);
			if (itemIndex == 0 && moveUp)
			{
				AddUserMessage("Item move", "No need to move Menu Item up, it is already first in its group. Menu Item: " + item.Name.ToHtml() + " [" + item.NavBarItemId + "]", UserMessageType.Info);
				return RedirectToAction("Manager");
			}
			if (!moveUp && (itemIndex == (siblings.Count - 1)))
			{
				AddUserMessage("Item move", "No need to move Menu Item down, it is already last in its group. Menu Item: " + item.Name.ToHtml() + " [" + item.NavBarItemId + "]", UserMessageType.Info);
				return RedirectToAction("Manager");
			}


			int targetIndex = (moveUp ? itemIndex - 1 : itemIndex + 1);
			NavBarItem target = siblings[targetIndex];

			//scenario 1 - target item has the same order number, so swap is not helpful, re-order all siblings, then do the swap
			if (target.Order == item.Order)
			{
				GStoreDb.NavBarItemsRenumberSiblings(siblings);
			}

			//scenario 2 - order numbers are unique, just swap with the sibling
			int targetOrder = target.Order;
			int itemOrder = item.Order;

			target.Order = itemOrder;
			item.Order = targetOrder;
			GStoreDb.NavBarItems.Update(target);
			GStoreDb.NavBarItems.Update(item);
			GStoreDb.SaveChanges();
			return RedirectToAction("Manager");
		}

		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Create)]
		public ActionResult Create(int? id)
		{
			NavBarItem navBarItem = GStoreDb.NavBarItems.Create();
			navBarItem.SetDefaultsForNew(CurrentStoreFrontOrThrow);
			if (id.HasValue)
			{
				NavBarItem parentNavBarItem = CurrentStoreFrontOrThrow.NavBarItems.SingleOrDefault(nb => nb.NavBarItemId == id.Value);
				if (parentNavBarItem != null)
				{
					navBarItem.ParentNavBarItem = parentNavBarItem;
					navBarItem.ParentNavBarItemId = parentNavBarItem.NavBarItemId;
				}
			}

			NavBarItemEditAdminViewModel viewModel = new NavBarItemEditAdminViewModel(navBarItem, CurrentUserProfileOrThrow, null, isCreatePage: true);

			return View("Create", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Create)]
		public ActionResult Create(NavBarItemEditAdminViewModel viewModel)
		{

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool nameIsValid = GStoreDb.ValidateNavBarItemName(this, viewModel.Name, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (nameIsValid && ModelState.IsValid)
			{
				try
				{
					NavBarItem navBarItem = GStoreDb.CreateNavBarItem(viewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Menu Item Created!", "Menu Item '" + navBarItem.Name.ToHtml() + "' [" + navBarItem.NavBarItemId + "] was created successfully for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);

					return RedirectToAction("Manager");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Menu Item '" + viewModel.Name + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Menu Item!", errorMessage.ToHtmlLines(), UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Create Menu Item Error", "There was an error with your entry for new menu item '" + viewModel.Name.ToHtml() + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it below and save.", UserMessageType.Danger);
			}

			viewModel.FillListsIfEmpty(storeFront.Client, storeFront);
			viewModel.UpdateParentNavBarItem(CurrentStoreFrontOrThrow.NavBarItems.SingleOrDefault(nb => nb.NavBarItemId == viewModel.ParentNavBarItemId)); 

			viewModel.IsSimpleCreatePage = true;

			return View("Create", viewModel);
		}


		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Create)]
		public ActionResult AdvancedCreate(string Tab)
		{
			NavBarItem navBarItem = GStoreDb.NavBarItems.Create();
			navBarItem.SetDefaultsForNew(CurrentStoreFrontOrThrow);
			NavBarItemEditAdminViewModel viewModel = new NavBarItemEditAdminViewModel(navBarItem, CurrentUserProfileOrThrow, Tab, isCreatePage: true);

			return View("AdvancedCreate", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Create)]
		public virtual ActionResult AdvancedCreate(NavBarItemEditAdminViewModel navBarItemEditViewModel)
		{
			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool nameIsValid = GStoreDb.ValidateNavBarItemName(this, navBarItemEditViewModel.Name, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (nameIsValid && ModelState.IsValid)
			{
				try
				{
					NavBarItem navBarItem = GStoreDb.CreateNavBarItem(navBarItemEditViewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Menu Item Created!", "Menu Item '" + navBarItem.Name.ToHtml() + "' [" + navBarItem.NavBarItemId + "] was created successfully for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);
					if (CurrentStoreFrontOrThrow.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.NavBarItems_View))
					{
						return RedirectToAction("AdvancedDetails", new { id = navBarItem.NavBarItemId, ReturnToManager = navBarItemEditViewModel.ReturnToManager });
					}
					return RedirectToAction("AdvancedManager");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Menu Item '" + navBarItemEditViewModel.Name + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Menu Item!", errorMessage.ToHtmlLines(), UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Create Menu Item Error", "There was an error with your entry for new menu item '" + navBarItemEditViewModel.Name.ToHtml() + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it below and save.", UserMessageType.Danger);
			}

			navBarItemEditViewModel.FillListsIfEmpty(storeFront.Client, storeFront);
			navBarItemEditViewModel.IsCreatePage = true;

			return View("AdvancedCreate", navBarItemEditViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Edit)]
		public ActionResult AdvancedEdit(int? id, string Tab, bool returnToManager = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("NavBarItemId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			NavBarItem navBarItem = storeFront.NavBarItems.Where(p => p.NavBarItemId == id.Value).SingleOrDefault();
			if (navBarItem == null)
			{
				AddUserMessage("Menu Item not found", "Sorry, the Menu Item you are trying to edit cannot be found. Menu Item Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				return RedirectToAction("AdvancedManager");

			}

			NavBarItemEditAdminViewModel viewModel = new NavBarItemEditAdminViewModel(navBarItem, CurrentUserProfileOrThrow, activeTab: Tab, returnToManager: returnToManager);

			return View("AdvancedEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Edit)]
		public ActionResult AdvancedEdit(NavBarItemEditAdminViewModel viewModel, string Tab)
		{
			StoreFront storeFront = CurrentStoreFrontOrThrow;

			bool nameIsValid = GStoreDb.ValidateNavBarItemName(this, viewModel.Name, storeFront.StoreFrontId, storeFront.ClientId, viewModel.NavBarItemId);

			NavBarItem navBarItem = storeFront.NavBarItems.SingleOrDefault(nb => nb.NavBarItemId == viewModel.NavBarItemId);
			if (navBarItem == null)
			{
				AddUserMessage("Menu Item not found", "Sorry, the Menu Item you are trying to edit cannot be found. Menu Item Id: [" + viewModel.NavBarItemId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (viewModel.ReturnToManager)
				{
					return RedirectToAction("Manager");
				}
				else
				{
					return RedirectToAction("AdvancedManager");
				}
			}

			if (ModelState.IsValid && nameIsValid)
			{
				navBarItem = GStoreDb.UpdateNavBarItem(viewModel, storeFront, CurrentUserProfileOrThrow);
				AddUserMessage("Menu Item updated successfully!", "Menu Item updated successfully. Menu Item '" + navBarItem.Name.ToHtml() + "' [" + navBarItem.NavBarItemId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);
				if (viewModel.ReturnToManager)
				{
					return RedirectToAction("Manager");
				}
				else
				{
					return RedirectToAction("AdvancedManager");
				}
			}

			return View("AdvancedEdit", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.NavBarItems_View, GStoreAction.NavBarItems_Edit)]
		public ActionResult AdvancedDetails(int? id, string Tab, bool returnToManager = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("NavBarItemId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			NavBarItem navBarItem = storeFront.NavBarItems.Where(p => p.NavBarItemId == id.Value).SingleOrDefault();
			if (navBarItem == null)
			{
				AddUserMessage("Menu Item not found", "Sorry, the Menu Item you are trying to view cannot be found. Menu Item Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				return RedirectToAction("AdvancedManager");

			}

			NavBarItemEditAdminViewModel viewModel = new NavBarItemEditAdminViewModel(navBarItem, CurrentUserProfileOrThrow, isDetailsPage: true, activeTab: Tab, returnToManager: returnToManager);
			return View("AdvancedDetails", viewModel);
		}


		[AuthorizeGStoreAction(true, GStoreAction.NavBarItems_View, GStoreAction.NavBarItems_Delete)]
		public ActionResult AdvancedDelete(int? id, string Tab, bool returnToManager = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("NavBarItemId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			NavBarItem navBarItem = storeFront.NavBarItems.Where(p => p.NavBarItemId == id.Value).SingleOrDefault();
			if (navBarItem == null)
			{
				AddUserMessage("Menu Item not found", "Sorry, the Menu Item you are trying to Delete cannot be found. Menu Item id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				return RedirectToAction("AdvancedManager");

			}

			NavBarItemEditAdminViewModel viewModel = new NavBarItemEditAdminViewModel (navBarItem, CurrentUserProfileOrThrow, isDeletePage: true, activeTab: Tab, returnToManager: returnToManager);
			return View("AdvancedDelete", viewModel);
		}

		[HttpPost]
		[ActionName("AdvancedDelete")]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.NavBarItems_Delete)]
		public ActionResult AdvancedDeleteConfirmed(int? id, bool returnToManager = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("NavBarItemId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			NavBarItem navBarItem = storeFront.NavBarItems.Where(p => p.NavBarItemId == id.Value).SingleOrDefault();
			if (navBarItem == null)
			{
				AddUserMessage("Menu Item not found", "Sorry, the Menu Item you are trying to Delete cannot be found. It may have been deleted already. Menu Item Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToManager)
				{
					return RedirectToAction("Manager");
				}
				else
				{
					return RedirectToAction("AdvancedManager");
				}
			}

			string navBarItemName = navBarItem.Name;
			try
			{
				bool deleted = GStoreDb.NavBarItems.DeleteById(id.Value);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Menu Item Deleted", "Menu Item '" + navBarItemName.ToHtml() + "' [" + id + "] was deleted successfully.", UserMessageType.Success);
					if (returnToManager)
					{
						return RedirectToAction("Manager");
					}
					else
					{
						return RedirectToAction("AdvancedManager");
					}
				}
				AddUserMessage("Menu Item Delete Error", "There was an error deleting Menu Item '" + navBarItemName.ToHtml() + "' [" + id + "]. It may have already been deleted.", UserMessageType.Warning);
				if (returnToManager)
				{
					return RedirectToAction("Manager");
				}
				else
				{
					return RedirectToAction("AdvancedManager");
				}
			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Menu Item '" + navBarItemName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Menu Item Delete Error", errorMessage.ToHtml(), UserMessageType.Danger);
				if (returnToManager)
				{
					return RedirectToAction("Manager");
				}
				else
				{
					return RedirectToAction("AdvancedManager");
				}
			}
		}
	}
}
