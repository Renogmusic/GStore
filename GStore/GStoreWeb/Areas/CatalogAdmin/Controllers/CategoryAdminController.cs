using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.CatalogAdmin.ViewModels;
using GStoreData.Identity;
using GStoreData.Models;

namespace GStoreWeb.Areas.CatalogAdmin.Controllers
{
	public class CategoryAdminController : AreaBaseController.CatalogAdminAreaBaseController
	{
		[AuthorizeGStoreAction(GStoreAction.Categories_Manager)]
		public ActionResult Manager(bool returnToFrontEnd = false)
		{
			CatalogAdminViewModel model = new CatalogAdminViewModel(CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow);
			model.ReturnToFrontEnd = returnToFrontEnd;
			return View("Manager", model);
		}

		[AuthorizeGStoreAction(GStoreAction.Categories_Manager)]
		public ActionResult UpdateCounts(bool returnToFrontEnd = false)
		{
			GStoreDb.RecalculateProductCategoryActiveCount(CurrentStoreFrontOrThrow);
			AddUserMessage("Category Counts Recalculated", "Category counts have been recalculated.", UserMessageType.Success);
			return RedirectToAction("Manager", new { returnToFrontEnd = returnToFrontEnd });
		}

		[AuthorizeGStoreAction(GStoreAction.Categories_Create)]
		public ActionResult Create(int? id, bool returnToFrontEnd = false, string Tab = "")
		{
			ProductCategory productCategory = GStoreDb.ProductCategories.Create();
			productCategory.SetDefaultsForNew(CurrentStoreFrontOrThrow);
			if (id.HasValue)
			{
				ProductCategory parentProductCategory = CurrentStoreFrontOrThrow.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == id.Value);
				if (parentProductCategory != null)
				{
					productCategory.CloneFromParentForNew(parentProductCategory);
				}
			}

			CategoryEditAdminViewModel viewModel = new CategoryEditAdminViewModel(productCategory, CurrentUserProfileOrThrow, activeTab: Tab, isCreatePage: true);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Categories_Create)]
		public ActionResult Create(CategoryEditAdminViewModel viewModel, string createAndView)
		{

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidateProductCategoryUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (urlIsValid && ModelState.IsValid)
			{
				try
				{
					ProcessFileUploads(viewModel, storeFront);
					ProductCategory productCategory = GStoreDb.CreateProductCategory(viewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Category Created!", "Category '" + productCategory.Name.ToHtml() + "' [" + productCategory.ProductCategoryId + "] was created successfully for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);

					if (!string.IsNullOrWhiteSpace(createAndView))
					{
						return RedirectToAction("Details", new { id = productCategory.ProductCategoryId, returnToFrontEnd = viewModel.ReturnToFrontEnd, Tab = viewModel.ActiveTab });
					}
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
					AddUserMessage("Error Creating Category Item!", errorMessage.ToHtmlLines(), UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				if (this.ModelState.ContainsKey("UrlName"))
				{
					ModelState["UrlName"].Value = new ValueProviderResult(viewModel.UrlName, viewModel.UrlName, null);
				}
				AddUserMessage("Create Category Error", "There was an error with your entry for new Category '" + viewModel.Name.ToHtml() + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it below and save.", UserMessageType.Danger);
			}

			if (Request.HasFiles())
			{
				AddUserMessage("Files not uploaded", "You must correct the form values below and re-upload your files", UserMessageType.Danger);
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
		public ActionResult Edit(CategoryEditAdminViewModel viewModel, string Tab, string saveAndView)
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
				ProcessFileUploads(viewModel, storeFront);
				productCategory = GStoreDb.UpdateProductCategory(viewModel, storeFront, CurrentUserProfileOrThrow);
				AddUserMessage("Category updated successfully!", "Category updated successfully. Category '" + productCategory.Name.ToHtml() + "' [" + productCategory.ProductCategoryId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);

				if (!string.IsNullOrWhiteSpace(saveAndView))
				{
					return RedirectToAction("Details", new { id = productCategory.ProductCategoryId, returnToFrontEnd = viewModel.ReturnToFrontEnd, Tab = viewModel.ActiveTab });
				}
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
			if (Request.HasFiles())
			{
				AddUserMessage("Files not uploaded", "You must correct the form values below and re-upload your files", UserMessageType.Danger);
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
					AddUserMessage("Category Deleted", "Category '" + productCategoryName.ToHtml() + "' [" + id + "] was deleted successfully.", UserMessageType.Success);
					if (returnToFrontEnd)
					{
						return RedirectToAction("Index", "Catalog", new { area = "" });
					}
					return RedirectToAction("Manager");
				}
				AddUserMessage("Category Delete Error", "There was an error deleting Category '" + productCategoryName.ToHtml() + "' [" + id + "]. It may have already been deleted.", UserMessageType.Warning);
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
				AddUserMessage("Category Delete Error", errorMessage.ToHtml(), UserMessageType.Danger);
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

		/// <summary>
		/// Runs a sync over category images.
		/// This operation is long, and it will check each category in the storefront categories
		/// For each category it will check 1 - if the CategoryImage exists in the file system,
		/// </summary>
		/// <returns></returns>
		[AuthorizeGStoreAction(GStoreAction.Categories_SyncImages)]
		public ActionResult SyncImages(bool eraseImageFileNameIfNotFound = true, bool searchForImageIfImageFileNameIsBlank = true, bool preview = true, bool verbose = true, bool returnToFrontEnd = false)
		{
			StringBuilder results = new StringBuilder();

			bool hasDbChanges = false;
			StoreFrontConfiguration storeFrontConfig = CurrentStoreFrontConfigOrAny;

			results.AppendLine("Starting Category Image sync");
			int counter = 0;
			foreach (ProductCategory category in storeFrontConfig.StoreFront.ProductCategories.AsQueryable().ApplyDefaultSort())
			{
				if (!string.IsNullOrEmpty(category.ImageName))
				{
					if (eraseImageFileNameIfNotFound)
					{
						//image name is set, verify image and set to null if not found
						string filePath = category.ImagePath(Request.ApplicationPath, RouteData, Server);
						if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
						{
							results.AppendLine("- - - - - Category '" + category.Name + "' [" + category.ProductCategoryId + "] Image '" + category.ImageName + "' not found, setting to null - - - - -");
							category.ImageName = null;
							if (!preview)
							{
								GStoreDb.ProductCategories.Update(category);
							}
							hasDbChanges = true;
							counter++;
						}
						else
						{
							if (verbose)
							{
								results.AppendLine("OK Category '" + category.Name + "' [" + category.ProductCategoryId + "] Image '" + category.ImageName + "' OK");
							}
						}
					}
				}

				if (string.IsNullOrEmpty(category.ImageName) && searchForImageIfImageFileNameIsBlank)
				{
					if (searchForImageIfImageFileNameIsBlank)
					{
						//if image name is not set, see if there is a suitable image with the url name of the category

						//choose file path on urlname, with filenames
						string imageFolder = storeFrontConfig.StoreFront.CatalogCategoryContentVirtualDirectoryToMap(Request.ApplicationPath);
						string newFileName = storeFrontConfig.StoreFront.ChooseFileNameWildcard(storeFrontConfig.Client, imageFolder, category.UrlName + "_Image", Request.ApplicationPath, Server);

						if (string.IsNullOrEmpty(newFileName))
						{
							if (verbose)
							{
								results.AppendLine("OK Category '" + category.Name + "' [" + category.ProductCategoryId + "] Image is null and no file found starting with '" + category.UrlName + "'");
							}
						}
						else
						{
							category.ImageName = newFileName;
							results.AppendLine("- - - - - New file to link to Category '" + category.Name + "' [" + category.ProductCategoryId + "] New Image '" + newFileName + "' - - - - -");
							if (!preview)
							{
								GStoreDb.ProductCategories.Update(category);
							}
							results.AppendLine();
							hasDbChanges = true;
							counter++;
						}
					}
					else
					{
						if (verbose)
						{
							results.AppendLine("OK Category '" + category.Name + "' [" + category.ProductCategoryId + "] Image is blank - not searching for files because searchForImageIfImageFileNameIsBlank = false");
						}
					}
				}
				if (verbose)
				{
					results.AppendLine();
				}
			}

			results.AppendLine();
			if (hasDbChanges)
			{
				if (preview)
				{
					results.AppendLine("- - - - - Database update needed for " + counter.ToString("N0") + " Categor" + (counter == 1 ? "y" : "ies") + ". - - - - -");
				}
				else
				{
					GStoreDb.SaveChangesDirect();
					results.AppendLine("- - - - - Database updated successfully for " + counter.ToString("N0") + " Categor" + (counter == 1 ? "y" : "ies") + ". - - - - -");
				}
			}
			else
			{
				results.AppendLine("No changes to save to database. Up-to-date.");
			}


			AddUserMessage("Category Image Sync Results", results.ToString().ToHtmlLines(), UserMessageType.Info);
			if (storeFrontConfig.StoreFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Categories_Manager))
			{
				return RedirectToAction("Manager", new { returnToFrontEnd = returnToFrontEnd });
			}
			return RedirectToAction("Index", "CatalogAdmin", new { returnToFrontEnd = returnToFrontEnd });
		}

		protected void ProcessFileUploads(CategoryEditAdminViewModel viewModel, StoreFront storeFront)
		{
			string virtualFolder = storeFront.CatalogCategoryContentVirtualDirectoryToMap(Request.ApplicationPath);
			string fileFolder = Server.MapPath(virtualFolder);
			if (!System.IO.Directory.Exists(fileFolder))
			{
				System.IO.Directory.CreateDirectory(fileFolder);
			}

			HttpPostedFileBase imageFile = Request.Files["ImageName_File"];
			if (imageFile != null && imageFile.ContentLength != 0)
			{
				string newFileName = imageFile.FileNameNoPath();
				if (!string.IsNullOrEmpty(Request.Form["ImageName_ChangeFileName"]))
				{
					newFileName = viewModel.UrlName + "_Image." + imageFile.FileName.FileExtension();
				}
				
				try
				{
					imageFile.SaveAs(fileFolder + "\\" + newFileName);
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Error saving category image file '" + imageFile.FileName + "' as '" + newFileName + "'", ex);
				}

				viewModel.ImageName = newFileName;
				AddUserMessage("Image Uploaded!", "Category Image '" + imageFile.FileName.ToHtml() + "' " + imageFile.ContentLength.ToByteString() + " was saved as '" + newFileName + "'", UserMessageType.Success);
			}
		}
	}
}
