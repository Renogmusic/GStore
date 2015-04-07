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
	public class BundleAdminController : AreaBaseController.CatalogAdminAreaBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Bundles_Manager)]
		public ActionResult Manager(int? productCategoryId, string SortBy, bool? SortAscending, bool returnToFrontEnd = false)
        {
			StoreFront storeFront = CurrentStoreFrontOrThrow;
			IQueryable<ProductBundle> query = null;

			if (!productCategoryId.HasValue)
			{
				ProductCategory firstCategory = storeFront.ProductCategories.AsQueryable().ApplyDefaultSort().FirstOrDefault();
				if (firstCategory != null)
				{
					productCategoryId = firstCategory.ProductCategoryId;
				}
			}

			if (productCategoryId.HasValue && productCategoryId.Value != 0)
			{
				query = storeFront.ProductBundles.Where(p => p.ProductCategoryId == productCategoryId.Value).AsQueryable();
			}
			else
			{
				query = storeFront.ProductBundles.AsQueryable();
			}

			IOrderedQueryable<ProductBundle> productBundles = query.ApplySort(this, SortBy, SortAscending);

			CatalogAdminViewModel viewModel = this.CatalogAdminViewModel;
			viewModel.UpdateSortedProductBundles(productBundles);
			viewModel.FilterProductCategoryId = productCategoryId;
			viewModel.SortBy = SortBy;
			viewModel.SortAscending = SortAscending;
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("Manager", this.CatalogAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.Bundles_Create)]
		public ActionResult Create(int? id, bool returnToFrontEnd = false, string Tab = "")
		{
			ProductBundle productBundle = GStoreDb.ProductBundles.Create();
			productBundle.SetDefaultsForNew(CurrentStoreFrontOrThrow);
			if (id.HasValue)
			{
				ProductCategory parentProductCategory = CurrentStoreFrontOrThrow.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == id.Value);
				if (parentProductCategory != null)
				{
					productBundle.Category = parentProductCategory;
					productBundle.ProductCategoryId = parentProductCategory.ProductCategoryId;
				}
			}

			ProductBundleEditAdminViewModel viewModel = new ProductBundleEditAdminViewModel(productBundle, CurrentUserProfileOrThrow, Tab, isCreatePage: true);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Bundles_Create)]
		public ActionResult Create(ProductBundleEditAdminViewModel viewModel, string createAndView)
		{

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidateProductBundleUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (urlIsValid && ModelState.IsValid)
			{
				try
				{
					ProcessFileUploads(viewModel, storeFront);
					ProductBundle productBundle = GStoreDb.CreateProductBundle(viewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Product Bundle Created!", "Product Bundle '" + productBundle.Name.ToHtml() + "' [" + productBundle.ProductBundleId + "] was created successfully for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);
					if (!string.IsNullOrWhiteSpace(createAndView))
					{
						return RedirectToAction("Details", new { id = productBundle.ProductBundleId, returnToFrontEnd = viewModel.ReturnToFrontEnd, tab = viewModel.ActiveTab });
					}
					if (viewModel.ReturnToFrontEnd)
					{
						return RedirectToAction("ViewBundleByName", "Catalog", new { area = "", urlName = viewModel.UrlName } );
					}
					if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
					{
						return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
					}
					return RedirectToAction("Index", "CatalogAdmin");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Product Bundle '" + viewModel.Name + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Product Bundle!", errorMessage.ToHtmlLines(), UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Create Product Bundle Error", "There was an error with your entry for new Product Bundle '" + viewModel.Name.ToHtml() + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it below and save.", UserMessageType.Danger);

			}

			if (Request.HasFiles())
			{
				AddUserMessage("Files not uploaded", "You must correct the form values below and re-upload your files", UserMessageType.Danger);
			}

			viewModel.FillListsIfEmpty(storeFront.Client, storeFront);

			viewModel.IsCreatePage = true;
			ViewData.Add("ReturnToFrontEnd", viewModel.ReturnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Bundles_Edit)]
		public ActionResult Edit(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductBundleId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductBundle productBundle = storeFront.ProductBundles.Where(p => p.ProductBundleId == id.Value).SingleOrDefault();
			if (productBundle == null)
			{
				AddUserMessage("Product Bundle not found", "Sorry, the Product Bundle you are trying to edit cannot be found. Product Bundle Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			ProductBundleEditAdminViewModel viewModel = new ProductBundleEditAdminViewModel(productBundle, CurrentUserProfileOrThrow, activeTab: Tab, isEditPage: true);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Bundles_Edit)]
		public ActionResult Edit(int? id, ProductBundleEditAdminViewModel viewModel, List<ProductBundleItemEditAdminViewModel> bundleItems, string saveAndView, string addProductId, int? removeItemId, int? removeAltCategoryId)
		{
			if (viewModel == null || viewModel.ProductBundleId == 0)
			{
				return HttpBadRequest("Product Bundle Id = 0");
			}
			if (viewModel.ProductBundleId != (id ?? 0))
			{
				return HttpBadRequest("Product Bundle Id mismatch Request id: " + (id.HasValue ? id.Value.ToString() : "(null)") + " viewModel.ProductBundleId: " + viewModel.ProductBundleId);
			}

			StoreFrontConfiguration config = CurrentStoreFrontConfigOrThrow;
			StoreFront storeFront = config.StoreFront;

			bool nameIsValid = GStoreDb.ValidateProductBundleUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, viewModel.ProductBundleId);

			ProductBundle productBundle = storeFront.ProductBundles.SingleOrDefault(p => p.ProductBundleId == viewModel.ProductBundleId);
			if (productBundle == null)
			{
				AddUserMessage("Product Bundle not found", "Sorry, the Product Bundle you are trying to edit cannot be found. Product Bundle Id: [" + viewModel.ProductBundleId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (viewModel.ReturnToFrontEnd)
				{
					return RedirectToAction("ViewBundleByName", "Catalog", new { area = "", urlName = viewModel.UrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			if (bundleItems != null && bundleItems.Count != 0)
			{
				if (!bundleItems.All(bi => productBundle.ProductBundleItems.Any(pb => pb.ProductBundleItemId == bi.ProductBundleItemId)))
				{
					return HttpBadRequest("bundle items do not all have the same ProductBundleId");
				}
			}

			if (!string.IsNullOrEmpty(addProductId))
			{
				int productIdToAdd = 0;
				if (!int.TryParse(addProductId, out productIdToAdd))
				{
					return HttpBadRequest("product id invalid");
				}

				if (productBundle.ProductBundleItems.Any(pbi => pbi.ProductId == productIdToAdd))
				{
					AddUserMessage("Product already exists in bundle", "Product id " + productIdToAdd + " already exists in this bundle.", UserMessageType.Info);
				}
				else
				{
					ProductBundleItem newItem = GStoreDb.CreateProductBundleItemFastAdd(productBundle, productIdToAdd, storeFront, CurrentUserProfileOrThrow, 1);
					AddUserMessage("Added Item!", "Product '" + newItem.Product.Name.ToHtml() + "' [" + newItem.ProductId + "] was added to the bundle '" + productBundle.Name.ToHtml() + "' [" + productBundle.ProductBundleId + "]", UserMessageType.Info);
				}
			}

			if (ModelState.IsValid && nameIsValid)
			{
				bool removedItem = false;
				ProcessFileUploads(viewModel, storeFront);
				productBundle = GStoreDb.UpdateProductBundle(viewModel, storeFront, CurrentUserProfileOrThrow);
				AddUserMessage("Product Bundle updated successfully!", "Product Bundle updated successfully. Product Bundle '" + productBundle.Name.ToHtml() + "' [" + productBundle.ProductBundleId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);


				if (bundleItems != null && bundleItems.Count != 0)
				{
					GStoreDb.UpdateProductBundleItems(productBundle, bundleItems, storeFront, CurrentUserProfileOrThrow);
				}
				if (removeItemId.HasValue && removeItemId.Value != 0)
				{
					removedItem = true;
					RemoveItem(productBundle, removeItemId.Value);
				}

				if (removeAltCategoryId.HasValue && removeAltCategoryId.Value != 0)
				{
					ProductCategoryAltProductBundle removeAltProductBundle = productBundle.CategoryAltBundles.SingleOrDefault(p => p.ProductCategoryId == removeAltCategoryId.Value);
					if (removeAltProductBundle == null)
					{
						AddUserMessage("Error removing cross-sell Category", "Category Id [" + removeAltCategoryId.Value + "] could not be found in bundle alt categories.", UserMessageType.Danger);
					}
					else
					{
						string oldName = removeAltProductBundle.Category.Name;
						GStoreDb.ProductCategoryAltProductBundles.Delete(removeAltProductBundle);
						GStoreDb.SaveChanges();
						AddUserMessage("Removed cross-sell Catergory", oldName.ToHtml() + " was removed.", UserMessageType.Info);
					}
				}

				if (removeAltCategoryId.HasValue)
				{
					return RedirectToAction("Edit", new { id = productBundle.ProductBundleId, returnToFrontEnd = viewModel.ReturnToFrontEnd, Tab = viewModel.ActiveTab });
				}
				if (!string.IsNullOrEmpty(addProductId) || removedItem)
				{
					return RedirectToAction("Edit", new { id = productBundle.ProductBundleId, returnToFrontEnd = viewModel.ReturnToFrontEnd, Tab = viewModel.ActiveTab });
				}
				if (!string.IsNullOrWhiteSpace(saveAndView))
				{
					return RedirectToAction("Details", new { id = productBundle.ProductBundleId, returnToFrontEnd = viewModel.ReturnToFrontEnd, Tab = viewModel.ActiveTab });
				}
				if (viewModel.ReturnToFrontEnd)
				{
					return RedirectToAction("ViewBundleByName", "Catalog", new { area = "", urlName = viewModel.UrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			if (Request.HasFiles())
			{
				AddUserMessage("Files not uploaded", "You must correct the form values below and re-upload your files", UserMessageType.Danger);
			}

			viewModel.UpdateProductBundle(productBundle);
			ViewData.Add("ReturnToFrontEnd", viewModel.ReturnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.Bundles_View, GStoreAction.Bundles_Edit)]
		public ActionResult Details(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductBundleId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductBundle productBundle = storeFront.ProductBundles.Where(p => p.ProductBundleId == id.Value).SingleOrDefault();
			if (productBundle == null)
			{
				AddUserMessage("Product Bundle not found", "Sorry, the Product Bundle you are trying to view cannot be found. Product Bundle Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
				}
				return RedirectToAction("Index", "CatalogAdmin");

			}

			ProductBundleEditAdminViewModel viewModel = new ProductBundleEditAdminViewModel(productBundle, CurrentUserProfileOrThrow, isDetailsPage: true, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("Details", viewModel);
		}

		protected void RemoveItem(ProductBundle bundle, int productId)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			ProductBundleItem bundleItem = bundle.ProductBundleItems.SingleOrDefault(pbi => pbi.ProductId == productId);
			if (bundleItem == null)
			{
				AddUserMessage("Bundle Item not found", "The item you are trying to remove (Product Id: " + productId + ") is no longer in bundle '" + bundle.Name.ToHtml() + "' [" + bundle.ProductBundleId + "]", UserMessageType.Info);
				return;
			}
			else
			{
				string itemToRemoveName = bundleItem.Product.Name;
				int productBundleItemId = bundleItem.ProductBundleItemId;
				bool result = GStoreDb.ProductBundleItems.DeleteById(productBundleItemId);
				if (result)
				{
					GStoreDb.SaveChanges();
					AddUserMessage("Bundle Item Removed", "Bundle item '" + itemToRemoveName.ToHtml() + "' [" + productBundleItemId + "] was removed from bundle '" + bundle.Name.ToHtml() + "' [" + bundle.ProductBundleId + "]", UserMessageType.Success);
				}
				else
				{
					AddUserMessage("Bundle Item Delete Error", "The item you are trying to remove '" + itemToRemoveName.ToHtml() + "' [" + productBundleItemId + "] could not be deleted from bundle '" + bundle.Name.ToHtml() + "' [" + bundle.ProductBundleId + "]", UserMessageType.Info);
				}
			}

		}


		[AuthorizeGStoreAction(GStoreAction.Bundles_Delete)]
		public ActionResult Delete(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductBundleId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductBundle productBundle = storeFront.ProductBundles.Where(p => p.ProductBundleId == id.Value).SingleOrDefault();
			if (productBundle == null)
			{
				AddUserMessage("Product Bundle not found", "Sorry, the Product Bundle you are trying to Delete cannot be found. Product Bundle id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			ProductBundleEditAdminViewModel viewModel = new ProductBundleEditAdminViewModel(productBundle, CurrentUserProfileOrThrow, isDeletePage: true, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("Delete", viewModel);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Bundles_Delete)]
		public ActionResult DeleteConfirmed(int? id, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductBundleId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			ProductBundle productBundle = storeFront.ProductBundles.Where(p => p.ProductBundleId == id.Value).SingleOrDefault();
			if (productBundle == null)
			{
				AddUserMessage("Product Bundle not found", "Sorry, the Product Bundle you are trying to Delete cannot be found. It may have been deleted already. Product Bundle Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			string productBundleName = productBundle.Name;
			string categoryUrlName = productBundle.Category.UrlName;
			try
			{
				bool deleted = GStoreDb.ProductBundles.DeleteById(id.Value);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Product Bundle Deleted", "Product Bundle '" + productBundleName.ToHtml() + "' [" + id + "] was deleted successfully.", UserMessageType.Success);
					if (returnToFrontEnd)
					{
						return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = categoryUrlName });
					}
					if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
					{
						return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
					}
					return RedirectToAction("Index", "CatalogAdmin");
				}
				AddUserMessage("Product Bundle Delete Error", "There was an error deleting Product Bundle '" + productBundleName.ToHtml() + "' [" + id + "]. It may have already been deleted.", UserMessageType.Warning);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = categoryUrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Product Bundle '" + productBundleName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Product Bundle Delete Error", errorMessage.ToHtml(), UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = categoryUrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Bundles_Manager))
				{
					return RedirectToAction("Manager", new { ProductCategoryId = productBundle.ProductCategoryId });
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}


		}


		/// <summary>
		/// Runs a sync over product Bundle files
		/// This operation is long, and it will check each category in the storefront categories
		/// For each category it will check 1 - if the CategoryImage exists in the file system,
		/// </summary>
		/// <returns></returns>
		[AuthorizeGStoreAction(GStoreAction.Categories_SyncImages)]
		public ActionResult SyncFiles(int? productCategoryId, bool eraseFileNameIfNotFound = true, bool searchForFileIfBlank = true, bool preview = true, bool verbose = true, bool returnToFrontEnd = false)
		{
			StringBuilder results = new StringBuilder();

			bool hasDbChanges = false;
			StoreFrontConfiguration storeFrontConfig = CurrentStoreFrontConfigOrAny;

			ProductCategory productCategory = null;
			if (productCategoryId.HasValue && productCategoryId.Value != 0)
			{
				productCategory = storeFrontConfig.StoreFront.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == productCategoryId.Value);
				if (productCategory == null)
				{
					return HttpBadRequest("Product category not found. Id: " + productCategoryId.Value);
				}
			}

			results.AppendLine("Starting Product Bundle File sync" + (productCategory == null ? " for all products" : " for Category '" + productCategory.Name + "' [" + productCategory.ProductCategoryId + "]"));

			List<ProductBundle> productBundles = null;
			if (productCategory == null)
			{
				productBundles = storeFrontConfig.StoreFront.ProductBundles.AsQueryable().ApplyDefaultSort().ToList();
			}
			else
			{
				productBundles = productCategory.ProductBundles.AsQueryable().ApplyDefaultSort().ToList();
			}

			string applicationPath = Request.ApplicationPath;
			int counter = 0;
			foreach (ProductBundle productBundle in productBundles)
			{
				bool hasDbChangesProductBundle = false;

				//ImageFile
				bool hasDbChangesImageFile;
				string resultImageFile = productBundle.SyncImageFile(eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, RouteData, Server, out hasDbChangesImageFile);
				hasDbChangesProductBundle = hasDbChangesImageFile;
				if (!string.IsNullOrEmpty(resultImageFile))
				{
					results.Append(resultImageFile);
				}

				hasDbChanges = hasDbChanges || hasDbChangesProductBundle;
				if (hasDbChangesProductBundle)
				{
					counter++;
					if (preview)
					{
						results.AppendLine("- - - - - Product Bundle update needed for '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] - - - - -");
					}
					else
					{
						GStoreDb.ProductBundles.Update(productBundle);
						results.AppendLine("- - - - - Product Bundle updated '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] - - - - -");
					}
				}
				else
				{
					if (verbose)
					{
						results.AppendLine("OK - No Product Bundle update needed for '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "]");
					}
				}
				if (verbose)
				{
					results.AppendLine();
				}
			}

			if (hasDbChanges)
			{
				if (preview)
				{
					results.AppendLine("- - - - - Database update needed for " + counter.ToString("N0") + " Product Bundle" + (counter == 1 ? "" : "s") + ". - - - - -");
				}
				else
				{
					GStoreDb.SaveChangesDirect();
					results.AppendLine("- - - - - Database updated successfully for " + counter.ToString("N0") + " Product Bundle" + (counter == 1 ? "" : "s") + ". - - - - -");
				}
			}
			else
			{
				results.AppendLine("No changes to save to database. Up-to-date.");
			}


			AddUserMessage("Category Image Sync Results", results.ToString().ToHtmlLines(), UserMessageType.Info);
			if (storeFrontConfig.StoreFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Categories_Manager))
			{
				return RedirectToAction("Manager", new { ProductCategoryId = productCategoryId ?? 0, returnToFrontEnd = returnToFrontEnd });
			}
			return RedirectToAction("Index", "CatalogAdmin", new { returnToFrontEnd = returnToFrontEnd });
		}

		protected void ProcessFileUploads(ProductBundleEditAdminViewModel viewModel, StoreFront storeFront)
		{
			string virtualFolder = storeFront.CatalogProductBundleContentVirtualDirectoryToMap(Request.ApplicationPath);
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
					throw new ApplicationException("Error saving bundle image file '" + imageFile.FileName + "' as '" + newFileName + "'", ex);
				}

				viewModel.ImageName = newFileName;
				AddUserMessage("Image Uploaded!", "Bundle Image '" + imageFile.FileName.ToHtml() + "' " + imageFile.ContentLength.ToByteString() + " was saved as '" + newFileName + "'", UserMessageType.Success);
			}
		}
	}
}
