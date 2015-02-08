using GStore.Identity;
using GStore.Models;
using GStore.Data;
using System;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;
using System.Collections.Generic;
using System.Linq;
using GStore.Areas.CatalogAdmin.ViewModels;

namespace GStore.Areas.CatalogAdmin.Controllers
{
	public class CategoryAdminController : BaseClasses.CatalogAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Categories_Manager)]
        public ActionResult Manager(bool returnToFrontEnd = false)
        {
			CatalogAdminViewModel model = new CatalogAdminViewModel(CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow);
			model.ReturnToFrontEnd = returnToFrontEnd;
			return View("Manager", model);
        }

		[AuthorizeGStoreAction(GStoreAction.Categories_Create)]
		public ActionResult Create(int? id, bool returnToFrontEnd = false)
		{
			ProductCategory productCategory = GStoreDb.ProductCategories.Create();
			productCategory.SetDefaultsForNew(CurrentStoreFrontOrThrow);
			if (id.HasValue)
			{
				ProductCategory parentProductCategory = CurrentStoreFrontOrThrow.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == id.Value);
				if (parentProductCategory != null)
				{
					productCategory.ParentCategory = parentProductCategory;
					productCategory.ParentCategoryId = parentProductCategory.ProductCategoryId;
				}
			}

			CategoryEditAdminViewModel viewModel = new CategoryEditAdminViewModel(productCategory, CurrentUserProfileOrThrow, null, isCreatePage: true);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Categories_Create)]
		public ActionResult Create(CategoryEditAdminViewModel viewModel)
		{

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidateProductCategoryUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (urlIsValid && ModelState.IsValid)
			{
				try
				{
					ProductCategory productCategory = GStoreDb.CreateProductCategory(viewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Category Created!", "Category '" + productCategory.Name.ToHtml() + "' [" + productCategory.ProductCategoryId + "] was created successfully for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);

					if (viewModel.ReturnToFrontEnd)
					{
						return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = productCategory.UrlName });
					}
					return RedirectToAction("Manager");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Category '" + viewModel.Name + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Category Item!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				if (this.ModelState.ContainsKey("UrlName"))
				{
					ModelState["UrlName"].Value = new ValueProviderResult(viewModel.UrlName, viewModel.UrlName, null);
				}
				AddUserMessage("Create Category Error", "There was an error with your entry for new Category '" + viewModel.Name.ToHtml() + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it below and save.", AppHtmlHelpers.UserMessageType.Danger);
			}

			viewModel.FillListsIfEmpty(storeFront.Client, storeFront);

			viewModel.IsSimpleCreatePage = true;

			return View("CreateOrEdit", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Categories_Edit)]
		public ActionResult Edit(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductCategoryId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductCategory productCategory = storeFront.ProductCategories.Where(pc => pc.ProductCategoryId == id.Value).SingleOrDefault();
			if (productCategory == null)
			{
				AddUserMessage("Category not found", "Sorry, the Category you are trying to edit cannot be found. Category Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = productCategory.UrlName });
				}
				return RedirectToAction("Manager");
			}

			CategoryEditAdminViewModel viewModel = new CategoryEditAdminViewModel(productCategory, CurrentUserProfileOrThrow, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;

			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Categories_Edit)]
		public ActionResult Edit(CategoryEditAdminViewModel viewModel, string Tab)
		{
			StoreFront storeFront = CurrentStoreFrontOrThrow;

			bool nameIsValid = GStoreDb.ValidateProductCategoryUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, viewModel.ProductCategoryId);

			ProductCategory productCategory = storeFront.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == viewModel.ProductCategoryId);
			if (productCategory == null)
			{
				AddUserMessage("Category not found", "Sorry, the Category you are trying to edit cannot be found. Category Id: [" + viewModel.ProductCategoryId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (viewModel.ReturnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = productCategory.UrlName });
				}
				return RedirectToAction("Manager");
			}

			if (ModelState.IsValid && nameIsValid)
			{
				productCategory = GStoreDb.UpdateProductCategory(viewModel, storeFront, CurrentUserProfileOrThrow);
				AddUserMessage("Category updated successfully!", "Category updated successfully. Category '" + productCategory.Name.ToHtml() + "' [" + productCategory.ProductCategoryId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);
				if (viewModel.ReturnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = productCategory.UrlName });
				}
				return RedirectToAction("Manager");
			}

			if (this.ModelState.ContainsKey("UrlName"))
			{
				ModelState["UrlName"].Value = new ValueProviderResult(viewModel.UrlName, viewModel.UrlName, null);
			}

			viewModel.UpdateProductCategoryAndParent(productCategory);
			return View("CreateOrEdit", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.Categories_View, GStoreAction.Categories_Edit)]
		public ActionResult Details(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductCategoryId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductCategory productCategory = storeFront.ProductCategories.Where(pc => pc.ProductCategoryId == id.Value).SingleOrDefault();
			if (productCategory == null)
			{
				AddUserMessage("Category not found", "Sorry, the Category you are trying to view cannot be found. Category Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = productCategory.UrlName });
				}
				return RedirectToAction("Manager");

			}

			CategoryEditAdminViewModel viewModel = new CategoryEditAdminViewModel(productCategory, CurrentUserProfileOrThrow, isDetailsPage: true, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			return View("Details", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.Categories_View, GStoreAction.Categories_Delete)]
		public ActionResult Delete(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductCategoryId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductCategory productCategory = storeFront.ProductCategories.Where(pc => pc.ProductCategoryId == id.Value).SingleOrDefault();
			if (productCategory == null)
			{
				AddUserMessage("Category not found", "Sorry, the Category you are trying to Delete cannot be found. Category id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = productCategory.UrlName });
				}
				return RedirectToAction("Manager");
			}

			CategoryEditAdminViewModel viewModel = new CategoryEditAdminViewModel(productCategory, CurrentUserProfileOrThrow, isDeletePage: true, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			return View("Delete", viewModel);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Categories_Delete)]
		public ActionResult DeleteConfirmed(int? id, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductCategoryId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductCategory productCategory = storeFront.ProductCategories.Where(pc => pc.ProductCategoryId == id.Value).SingleOrDefault();
			if (productCategory == null)
			{
				AddUserMessage("Category not found", "Sorry, the Category you are trying to Delete cannot be found. It may have been deleted already. Category Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				return RedirectToAction("Manager");
			}

			string productCategoryName = productCategory.Name;
			try
			{
				bool deleted = GStoreDb.ProductCategories.DeleteById(id.Value);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Category Deleted", "Category '" + productCategoryName.ToHtml() + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
					if (returnToFrontEnd)
					{
						return RedirectToAction("Index", "Catalog", new { area = "" });
					}
					return RedirectToAction("Manager");
				}
				AddUserMessage("Category Delete Error", "There was an error deleting Category '" + productCategoryName.ToHtml() + "' [" + id + "]. It may have already been deleted.", AppHtmlHelpers.UserMessageType.Warning);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				return RedirectToAction("Manager");
			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Category '" + productCategoryName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Category Delete Error", errorMessage.ToHtml(), AppHtmlHelpers.UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				return RedirectToAction("Manager");
			}


		}

		[AuthorizeGStoreAction(GStoreAction.Categories_Manager)]
		public ActionResult MoveUp(int? id)
		{
			return MoveUpOrDown(id, true);
		}

		[AuthorizeGStoreAction(GStoreAction.Categories_Manager)]
		public ActionResult MoveDown(int? id)
		{
			return MoveUpOrDown(id, false);
		}

		[AuthorizeGStoreAction(GStoreAction.Categories_Manager)]
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

			ProductCategory sourceItem = CurrentStoreFrontOrThrow.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == source.Value);
			if (sourceItem == null)
			{
				return HttpBadRequest("Source ProductCategory not found in store front ProductCategories. id: " + source.Value);
			}

			if (sourceItem.ParentCategoryId == target)
			{
				//target is already the direct parent of the source, no changes
				return RedirectToAction("Manager");
			}

			int? newParentProductCategoryId = (target == 0 ? null : target);
			ProductCategory targetItem = null;
			if (target.Value != 0)
			{
				targetItem = CurrentStoreFrontOrThrow.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == target.Value);
				if (targetItem == null)
				{
					return HttpBadRequest("Target ProductCategory not found in store front ProductCategories. id: " + target.Value);
				}

				//make sure target is not a child of the source
				ProductCategory parentCheck = targetItem;
				do
				{
					if (parentCheck.ProductCategoryId == sourceItem.ProductCategoryId)
					{
						AddUserMessage("Category Move Error", "You cannot move a Category onto its sub-items. Move the Category to the root if you want to change the order. Source Category : '" + sourceItem.Name.ToHtml() + "' [" + sourceItem.ProductCategoryId + "] to Target Category: '" + targetItem.Name.ToHtml() + "' [" + targetItem.ProductCategoryId + "]", UserMessageType.Danger);
						return RedirectToAction("Manager");
					}
					parentCheck = parentCheck.ParentCategory;
				} while (parentCheck != null);

			}

			if (sourceItem.ParentCategoryId != newParentProductCategoryId)
			{
				sourceItem.ParentCategoryId = newParentProductCategoryId;
				GStoreDb.ProductCategories.Update(sourceItem);
				GStoreDb.SaveChanges();
			}

			return RedirectToAction("Manager");
		}

		[AuthorizeGStoreAction(GStoreAction.Categories_Manager)]
		protected ActionResult MoveUpOrDown(int? id, bool moveUp)
		{
			if (!id.HasValue || id.Value == 0)
			{
				return HttpBadRequest("Id cannot be null or 0");
			}

			ProductCategory category = CurrentStoreFrontOrThrow.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == id.Value);
			if (category == null)
			{
				return HttpBadRequest("ProductCategory not found in store front ProductCategories. id: " + id.Value);
			}

			//find nearest sibling up or down and swap order with them
			List<ProductCategory> siblings = CurrentStoreFrontOrThrow.ProductCategories.AsQueryable().Where(pc => pc.ParentCategoryId == category.ParentCategoryId).ApplyDefaultSort().ToList();
			if (siblings.Count == 0)
			{
				//no siblings, nothing to do
				AddUserMessage("Item move", "No need to move Category, it has no other items in its group. Category: " + category.Name.ToHtml() + " [" + category.ProductCategoryId + "]", UserMessageType.Info);
				return RedirectToAction("Manager");
			}

			int itemIndex = siblings.IndexOf(category);
			if (itemIndex == 0 && moveUp)
			{
				AddUserMessage("Item move", "No need to move Category up, it is already first in its group. Category: " + category.Name.ToHtml() + " [" + category.ProductCategoryId + "]", UserMessageType.Info);
				return RedirectToAction("Manager");
			}
			if (!moveUp && (itemIndex == (siblings.Count - 1)))
			{
				AddUserMessage("Item move", "No need to move Category down, it is already last in its group. Category: " + category.Name.ToHtml() + " [" + category.ProductCategoryId + "]", UserMessageType.Info);
				return RedirectToAction("Manager");
			}


			int targetIndex = (moveUp ? itemIndex - 1 : itemIndex + 1);
			ProductCategory target = siblings[targetIndex];

			//scenario 1 - target item has the same order number, so swap is not helpful, re-order all siblings, then do the swap
			if (target.Order == category.Order)
			{
				GStoreDb.ProductCategoriesRenumberSiblings(siblings);
			}

			//scenario 2 - order numbers are unique, just swap with the sibling
			int targetOrder = target.Order;
			int itemOrder = category.Order;

			target.Order = itemOrder;
			category.Order = targetOrder;
			GStoreDb.ProductCategories.Update(target);
			GStoreDb.ProductCategories.Update(category);
			GStoreDb.SaveChanges();
			return RedirectToAction("Manager");
		}
	}
}
