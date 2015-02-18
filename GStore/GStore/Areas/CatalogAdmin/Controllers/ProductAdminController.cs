using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;
using GStore.Data;
using System.Linq;
using GStore.Areas.CatalogAdmin.ViewModels;
using System.Web;
using System.Text;
using System.Collections.Generic;

namespace GStore.Areas.CatalogAdmin.Controllers
{
	public class ProductAdminController : BaseClasses.CatalogAdminBaseController
    {
		[AuthorizeGStoreAction(GStoreAction.Products_Manager)]
		public ActionResult Manager(int? productCategoryId, string SortBy, bool? SortAscending, bool returnToFrontEnd = false)
        {
			IQueryable<Product> query = null;
			if (productCategoryId.HasValue)
			{
				query = CurrentStoreFrontOrThrow.Products.Where(p => p.ProductCategoryId == productCategoryId.Value).AsQueryable();
			}
			else
			{
				query = CurrentStoreFrontOrThrow.Products.AsQueryable();
			}
			IOrderedQueryable<Product> products = query.ApplySort(this, SortBy, SortAscending);
			CatalogAdminViewModel viewModel = this.CatalogAdminViewModel;
			viewModel.UpdateSortedProducts(products);
			viewModel.FilterProductCategoryId = productCategoryId;
			viewModel.SortBy = SortBy;
			viewModel.SortAscending = SortAscending;
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("Manager", this.CatalogAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.Products_Create)]
		public ActionResult Create(int? id, bool returnToFrontEnd = false)
		{
			Product product = GStoreDb.Products.Create();
			product.SetDefaultsForNew(CurrentStoreFrontOrThrow);
			if (id.HasValue)
			{
				ProductCategory parentProductCategory = CurrentStoreFrontOrThrow.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == id.Value);
				if (parentProductCategory != null)
				{
					product.Category = parentProductCategory;
					product.ProductCategoryId = parentProductCategory.ProductCategoryId;
				}
			}

			ProductEditAdminViewModel viewModel = new ProductEditAdminViewModel(product, CurrentUserProfileOrThrow, null, isCreatePage: true);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Products_Create)]
		public ActionResult Create(ProductEditAdminViewModel viewModel, string createAndView)
		{

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidateProductUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (urlIsValid && ModelState.IsValid)
			{
				try
				{
					ProcessFileUploads(viewModel, storeFront);
					Product product = GStoreDb.CreateProduct(viewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Product Created!", "Product '" + product.Name.ToHtml() + "' [" + product.ProductId + "] was created successfully for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);
					if (!string.IsNullOrWhiteSpace(createAndView))
					{
						return RedirectToAction("Details", new { id = product.ProductId, returnToFrontEnd = viewModel.ReturnToFrontEnd });
					}
					if (viewModel.ReturnToFrontEnd)
					{
						return RedirectToAction("ViewProductByName", "Catalog", new { area = "", urlName = viewModel.UrlName } );
					}
					if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
					{
						return RedirectToAction("Manager");
					}
					return RedirectToAction("Index", "CatalogAdmin");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Product '" + viewModel.Name + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Product!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Create Product Error", "There was an error with your entry for new Product '" + viewModel.Name.ToHtml() + "' for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]. Please correct it below and save.", AppHtmlHelpers.UserMessageType.Danger);
			}

			viewModel.FillListsIfEmpty(storeFront.Client, storeFront);

			viewModel.IsSimpleCreatePage = true;
			ViewData.Add("ReturnToFrontEnd", viewModel.ReturnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.Products_Edit)]
		public ActionResult Edit(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Product product = storeFront.Products.Where(p => p.ProductId == id.Value).SingleOrDefault();
			if (product == null)
			{
				AddUserMessage("Product not found", "Sorry, the Product you are trying to edit cannot be found. Product Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			ProductEditAdminViewModel viewModel = new ProductEditAdminViewModel(product, CurrentUserProfileOrThrow, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Products_Edit)]
		public ActionResult Edit(ProductEditAdminViewModel viewModel, string Tab, string saveAndView)
		{
			StoreFront storeFront = CurrentStoreFrontOrThrow;

			bool nameIsValid = GStoreDb.ValidateProductUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, viewModel.ProductId);

			Product product = storeFront.Products.SingleOrDefault(p => p.ProductId == viewModel.ProductId);
			if (product == null)
			{
				AddUserMessage("Product not found", "Sorry, the Product you are trying to edit cannot be found. Product Id: [" + viewModel.ProductId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (viewModel.ReturnToFrontEnd)
				{
					return RedirectToAction("ViewProductByName", "Catalog", new { area = "", urlName = viewModel.UrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			if (ModelState.IsValid && nameIsValid)
			{
				ProcessFileUploads(viewModel, storeFront);
				product = GStoreDb.UpdateProduct(viewModel, storeFront, CurrentUserProfileOrThrow);
				AddUserMessage("Product updated successfully!", "Product updated successfully. Product '" + product.Name.ToHtml() + "' [" + product.ProductId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);
				if (!string.IsNullOrWhiteSpace(saveAndView))
				{
					return RedirectToAction("Details", new { id = product.ProductId, returnToFrontEnd = viewModel.ReturnToFrontEnd });
				}
				if (viewModel.ReturnToFrontEnd)
				{
					return RedirectToAction("ViewProductByName", "Catalog", new { area = "", urlName = viewModel.UrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}
			viewModel.UpdateProduct(product);
			ViewData.Add("ReturnToFrontEnd", viewModel.ReturnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.Products_View, GStoreAction.Products_Edit)]
		public ActionResult Details(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Product product = storeFront.Products.Where(p => p.ProductId == id.Value).SingleOrDefault();
			if (product == null)
			{
				AddUserMessage("Product not found", "Sorry, the Product you are trying to view cannot be found. Product Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");

			}

			ProductEditAdminViewModel viewModel = new ProductEditAdminViewModel(product, CurrentUserProfileOrThrow, isDetailsPage: true, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("Details", viewModel);
		}


		[AuthorizeGStoreAction(GStoreAction.Products_Delete)]
		public ActionResult Delete(int? id, string Tab, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Product product = storeFront.Products.Where(p => p.ProductId == id.Value).SingleOrDefault();
			if (product == null)
			{
				AddUserMessage("Product not found", "Sorry, the Product you are trying to Delete cannot be found. Product id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			ProductEditAdminViewModel viewModel = new ProductEditAdminViewModel(product, CurrentUserProfileOrThrow, isDeletePage: true, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("Delete", viewModel);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Products_Delete)]
		public ActionResult DeleteConfirmed(int? id, bool returnToFrontEnd = false)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Product product = storeFront.Products.Where(p => p.ProductId == id.Value).SingleOrDefault();
			if (product == null)
			{
				AddUserMessage("Product not found", "Sorry, the Product you are trying to Delete cannot be found. It may have been deleted already. Product Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("Index", "Catalog", new { area = "" });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			string productName = product.Name;
			string categoryUrlName = product.Category.UrlName;
			try
			{
				bool deleted = GStoreDb.Products.DeleteById(id.Value);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Product Deleted", "Product '" + productName.ToHtml() + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
					if (returnToFrontEnd)
					{
						return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = categoryUrlName });
					}
					if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
					{
						return RedirectToAction("Manager");
					}
					return RedirectToAction("Index", "CatalogAdmin");
				}
				AddUserMessage("Product Delete Error", "There was an error deleting Product '" + productName.ToHtml() + "' [" + id + "]. It may have already been deleted.", AppHtmlHelpers.UserMessageType.Warning);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = categoryUrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Product '" + productName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Product Delete Error", errorMessage.ToHtml(), AppHtmlHelpers.UserMessageType.Danger);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = categoryUrlName });
				}
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}


		}

		[AuthorizeGStoreAction(true, GStoreAction.Products_View, GStoreAction.Products_Edit)]
		public ActionResult PreviewDigitalDownload(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ProductId = null");
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			Product product = storeFront.Products.Where(p => p.ProductId == id.Value).SingleOrDefault();
			if (product == null)
			{
				AddUserMessage("Product not found", "Sorry, the Product you are trying to view cannot be found. Product Id: [" + id.Value + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Danger);
				if (storeFront.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.Products_Manager))
				{
					return RedirectToAction("Manager");
				}
				return RedirectToAction("Index", "CatalogAdmin");
			}

			if (string.IsNullOrEmpty(product.DigitalDownloadFileName))
			{
				string errorMessage = "There is no Digital Download File linked to this product";
				return View("DigitalDownload_Error", new DigitalDownloadFileError(product, errorMessage));
			}

			string filePath = product.DigitalDownloadFilePath(Request.ApplicationPath, RouteData, Server);
			if (string.IsNullOrEmpty(filePath))
			{
				string errorMessage = "Digital Download File '" + product.DigitalDownloadFileName + "' could not be found in store front, client, or server digital download files.\n"
					+ "Be sure file exists, and it is located in the file system 'DigitalDownload/Products/[file name]";
				return View("DigitalDownload_Error", new DigitalDownloadFileError(product, errorMessage));
			}

			string mimeType = MimeMapping.GetMimeMapping(filePath);
			return new FilePathResult(filePath, mimeType) { FileDownloadName = product.DigitalDownloadFileName };

		}


		/// <summary>
		/// Runs a sync over product files
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
			if (productCategoryId.HasValue)
			{
				productCategory = storeFrontConfig.StoreFront.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == productCategoryId.Value);
				if (productCategory == null)
				{
					return HttpBadRequest("Product category not found. Id: " + productCategoryId.Value);
				}
			}

			results.AppendLine("Starting Product File sync" + (productCategory == null ? " for all products" : " for Category '" + productCategory.Name + "' [" + productCategory.ProductCategoryId + "]"));

			List<Product> products = null;
			if (productCategory == null)
			{
				products = storeFrontConfig.StoreFront.Products.AsQueryable().ApplyDefaultSort().ToList();
			}
			else
			{
				products = productCategory.Products.AsQueryable().ApplyDefaultSort().ToList();
			}

			string applicationPath = Request.ApplicationPath;
			int counter = 0;
			foreach (Product product in products)
			{
				bool hasDbChangesProduct = false;

				//ImageFile
				bool hasDbChangesImageFile;
				string resultImageFile = product.SyncImageFile(eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, RouteData, Server, out hasDbChangesImageFile);
				hasDbChangesProduct = hasDbChangesImageFile;
				if (!string.IsNullOrEmpty(resultImageFile))
				{
					results.Append(resultImageFile);
				}


				//DigitalDownloadFile
				bool hasDbChangesDigitalDownloadFile;
				string resultDigitalDownloadFile = product.SyncDigitalDownloadFile(eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, RouteData, Server, out hasDbChangesDigitalDownloadFile);
				hasDbChangesProduct = hasDbChangesProduct || hasDbChangesDigitalDownloadFile;
				if (!string.IsNullOrEmpty(resultDigitalDownloadFile))
				{
					results.Append(resultDigitalDownloadFile);
				}

				//SampleAudioFile
				bool hasDbChangesSampleAudioFile;
				string resultSampleAudioFile = product.SyncSampleAudioFile(eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, RouteData, Server, out hasDbChangesSampleAudioFile);
				hasDbChangesProduct = hasDbChangesProduct || hasDbChangesSampleAudioFile;
				if (!string.IsNullOrEmpty(resultSampleAudioFile))
				{
					results.Append(resultSampleAudioFile);
				}

				//SampleDownloadFile
				bool hasDbChangesSampleDownloadFile;
				string resultSampleDownloadFile = product.SyncSampleDownloadFile(eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, RouteData, Server, out hasDbChangesSampleDownloadFile);
				hasDbChangesProduct = hasDbChangesProduct || hasDbChangesSampleDownloadFile;
				if (!string.IsNullOrEmpty(resultSampleDownloadFile))
				{
					results.Append(resultSampleDownloadFile);
				}

				//SampleImageFile
				bool hasDbChangesSampleImageFile;
				string resultSampleImageFile = product.SyncSampleImageFile(eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, RouteData, Server, out hasDbChangesSampleImageFile);
				hasDbChangesProduct = hasDbChangesProduct || hasDbChangesSampleImageFile;
				if (!string.IsNullOrEmpty(resultSampleImageFile))
				{
					results.Append(resultSampleImageFile);
				}

				hasDbChanges = hasDbChanges || hasDbChangesProduct;
				if (hasDbChangesProduct)
				{
					counter++;
					if (preview)
					{
						results.AppendLine("- - - - - Product update needed for '" + product.Name + "' [" + product.ProductId + "] - - - - -");
					}
					else
					{
						GStoreDb.Products.Update(product);
						results.AppendLine("- - - - - Product updated '" + product.Name + "' [" + product.ProductId + "] - - - - -");
					}
				}
				else
				{
					if (verbose)
					{
						results.AppendLine("OK - No Product update needed for '" + product.Name + "' [" + product.ProductId + "]");
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
					results.AppendLine("- - - - - Database update needed for " + counter.ToString("N0") + " Product" + (counter == 1 ? "" : "s") + ". - - - - -");
				}
				else
				{
					GStoreDb.SaveChangesDirect();
					results.AppendLine("- - - - - Database updated successfully for " + counter.ToString("N0") + " Product" + (counter == 1 ? "" : "s") + ". - - - - -");
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

		protected void ProcessFileUploads(ProductEditAdminViewModel viewModel, StoreFront storeFront)
		{
			//todo: temporary file process

			string virtualFolder = storeFront.CatalogProductContentVirtualDirectoryToMap(Request.ApplicationPath);
			string fileFolder = Server.MapPath(virtualFolder);
			if (!System.IO.Directory.Exists(fileFolder))
			{
				System.IO.Directory.CreateDirectory(fileFolder);
			}

			HttpPostedFileBase imageFile = Request.Files["ImageName_File"];
			if (imageFile != null && imageFile.ContentLength != 0)
			{
				string newFileName = viewModel.UrlName + "_Image." + imageFile.FileName.FileExtension();
				imageFile.SaveAs(fileFolder + "\\" + newFileName);
				viewModel.ImageName = newFileName;
				AddUserMessage("Image Uploaded!", "Image '" + imageFile.FileName.ToHtml() + "' " + imageFile.ContentLength.ToByteString() + " was saved as '" + newFileName + "'", UserMessageType.Success);
			}

			HttpPostedFileBase digitalDownloadFile = Request.Files["DigitalDownloadFileName_File"];
			if (digitalDownloadFile != null && digitalDownloadFile.ContentLength != 0)
			{
				string digitalDownloadVirtualFolder = storeFront.ProductDigitalDownloadVirtualDirectoryToMap(Request.ApplicationPath);
				string digitalDownloadFileFolder = Server.MapPath(digitalDownloadVirtualFolder);
				if (!System.IO.Directory.Exists(digitalDownloadFileFolder))
				{
					System.IO.Directory.CreateDirectory(digitalDownloadFileFolder);
				}
				string newFileName = viewModel.UrlName + "_DigitalDownload." + digitalDownloadFile.FileName.FileExtension();
				digitalDownloadFile.SaveAs(digitalDownloadFileFolder + "\\" + newFileName);
				viewModel.DigitalDownloadFileName = newFileName;
				AddUserMessage("Digital Download Uploaded!", "Digital Download File '" + digitalDownloadFile.FileName.ToHtml() + "' " + digitalDownloadFile.ContentLength.ToByteString() + " was saved as '" + newFileName + "'", UserMessageType.Success);
			}

			HttpPostedFileBase sampleAudioFile = Request.Files["SampleAudioFileName_File"];
			if (sampleAudioFile != null && sampleAudioFile.ContentLength != 0)
			{
				string newFileName = viewModel.UrlName + "_SampleAudio." + sampleAudioFile.FileName.FileExtension();
				sampleAudioFile.SaveAs(fileFolder + "\\" + newFileName);
				viewModel.SampleAudioFileName = newFileName;
				AddUserMessage("Sample Audio Uploaded!", "Sample Audio File '" + sampleAudioFile.FileName.ToHtml() + "' " + sampleAudioFile.ContentLength.ToByteString() + " was saved as '" + newFileName + "'", UserMessageType.Success);
			}

			HttpPostedFileBase sampleDownloadFile = Request.Files["SampleDownloadFileName_File"];
			if (sampleDownloadFile != null && sampleDownloadFile.ContentLength != 0)
			{
				string newFileName = viewModel.UrlName + "_SampleDownload." + sampleDownloadFile.FileName.FileExtension();
				sampleDownloadFile.SaveAs(fileFolder + "\\" + newFileName);
				viewModel.SampleDownloadFileName = newFileName;
				AddUserMessage("Sample Download File Uploaded!", "Sample Download File '" + sampleDownloadFile.FileName.ToHtml() + "' " + sampleDownloadFile.ContentLength.ToByteString() + " was saved as '" + newFileName + "'", UserMessageType.Success);
			}

			HttpPostedFileBase sampleImageFile = Request.Files["SampleImageFileName_File"];
			if (sampleImageFile != null && sampleImageFile.ContentLength != 0)
			{
				string newFileName = viewModel.UrlName + "_SampleImage." + sampleImageFile.FileName.FileExtension();
				sampleImageFile.SaveAs(fileFolder + "\\" + newFileName);
				viewModel.SampleImageFileName = newFileName;
				AddUserMessage("Sample Image File Uploaded!", "Sample Image File '" + sampleImageFile.FileName.ToHtml() + "' " + sampleImageFile.ContentLength.ToByteString() + " was saved as '" + newFileName + "'", UserMessageType.Success);
			}

		}

	}

	
}
