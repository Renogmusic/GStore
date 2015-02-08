using GStore.Identity;
using GStore.Models;
using System;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;
using GStore.Data;
using System.Linq;
using GStore.Areas.CatalogAdmin.ViewModels;

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
		public ActionResult Create(ProductEditAdminViewModel viewModel)
		{

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			bool urlIsValid = GStoreDb.ValidateProductUrlName(this, viewModel.UrlName, storeFront.StoreFrontId, storeFront.ClientId, null);

			if (urlIsValid && ModelState.IsValid)
			{
				try
				{
					Product product = GStoreDb.CreateProduct(viewModel, storeFront, CurrentUserProfileOrThrow);
					AddUserMessage("Product Created!", "Product '" + product.Name.ToHtml() + "' [" + product.ProductId + "] was created successfully for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);
					if (viewModel.ReturnToFrontEnd)
					{
						return RedirectToAction("ViewProductByName", "Catalog", new { area = "", urlName = viewModel.UrlName } );
					}
					return RedirectToAction("Manager");
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
				return RedirectToAction("Manager");

			}

			ProductEditAdminViewModel viewModel = new ProductEditAdminViewModel(product, CurrentUserProfileOrThrow, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("CreateOrEdit", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.Products_Edit)]
		public ActionResult Edit(ProductEditAdminViewModel viewModel, string Tab)
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
				return RedirectToAction("Manager");
			}

			if (ModelState.IsValid && nameIsValid)
			{
				product = GStoreDb.UpdateProduct(viewModel, storeFront, CurrentUserProfileOrThrow);
				AddUserMessage("Product updated successfully!", "Product updated successfully. Product '" + product.Name.ToHtml() + "' [" + product.ProductId + "] for Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", UserMessageType.Success);
				if (viewModel.ReturnToFrontEnd)
				{
					return RedirectToAction("ViewProductByName", "Catalog", new { area = "", urlName = viewModel.UrlName });
				}
				return RedirectToAction("Manager");
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
				return RedirectToAction("Manager");

			}

			ProductEditAdminViewModel viewModel = new ProductEditAdminViewModel(product, CurrentUserProfileOrThrow, isDetailsPage: true, activeTab: Tab);
			viewModel.ReturnToFrontEnd = returnToFrontEnd;
			ViewData.Add("ReturnToFrontEnd", returnToFrontEnd);
			return View("Details", viewModel);
		}


		[AuthorizeGStoreAction(true, GStoreAction.Products_View, GStoreAction.Products_Delete)]
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
				return RedirectToAction("Manager");

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
				return RedirectToAction("Manager");
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
					return RedirectToAction("Manager");
				}
				AddUserMessage("Product Delete Error", "There was an error deleting Product '" + productName.ToHtml() + "' [" + id + "]. It may have already been deleted.", AppHtmlHelpers.UserMessageType.Warning);
				if (returnToFrontEnd)
				{
					return RedirectToAction("ViewCategoryByName", "Catalog", new { area = "", urlName = categoryUrlName });
				}
				return RedirectToAction("Manager");
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
				return RedirectToAction("Manager");
			}


		}

	}
}
