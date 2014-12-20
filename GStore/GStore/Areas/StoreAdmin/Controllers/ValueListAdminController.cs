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
		[AuthorizeGStoreAction(GStoreAction.WebForms_Manager)]
		public ActionResult Manager(string SortBy, bool? SortAscending)
		{
			IOrderedQueryable<WebForm> webForms = CurrentClientOrThrow.WebForms.AsQueryable().ApplySort(this, SortBy, SortAscending);

			//ValueListManagerAdminViewModel viewModel = new ValueListManagerAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForms);
			//return View("Manager", viewModel);
			return View("Manager");
		}

		[AuthorizeGStoreAction(GStoreAction.WebForms_Create)]
		public ActionResult Create(string Tab)
		{
			Models.WebForm webForm = GStoreDb.WebForms.Create();
			webForm.SetDefaultsForNew(CurrentClientOrThrow.ClientId);
			WebFormEditViewModel viewModel = new WebFormEditViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, isStoreAdminEdit: true, isCreatePage: true);
			return View("Create", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.WebForms_Create)]
		public virtual ActionResult Create(WebFormEditViewModel webFormEditViewModel)
		{

			Client client = CurrentClientOrThrow;
			bool nameIsValid = GStoreDb.ValidateWebFormName(this, webFormEditViewModel.Name, CurrentClientOrThrow.ClientId, null);

			if (nameIsValid && ModelState.IsValid)
			{
				try
				{
					WebForm webForm = null;
					webForm = GStoreDb.CreateWebForm(webFormEditViewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
					AddUserMessage("Web Form Created!", "Web Form '" + webForm.Name.ToHtml() + "' [" + webForm.WebFormId + "] was created successfully for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", AppHtmlHelpers.UserMessageType.Success);
					if (CurrentStoreFrontOrThrow.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.WebForms_Manager))
					{
						return RedirectToAction("Manager");
					}
					return RedirectToAction("Index", "StoreAdmin");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Web Form '" + webFormEditViewModel.Name.ToHtml() + "' for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Web Form!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Web Form Create Error", "There was an error with your entry for new Web Form '" + webFormEditViewModel.Name.ToHtml() + "' for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]. Please correct it below and save.", AppHtmlHelpers.UserMessageType.Danger);
			}
			webFormEditViewModel.IsStoreAdminEdit = true;
			webFormEditViewModel.IsCreatePage = true;
			webFormEditViewModel.IsActiveDirect = !(webFormEditViewModel.IsPending || (webFormEditViewModel.StartDateTimeUtc > DateTime.UtcNow) || (webFormEditViewModel.EndDateTimeUtc < DateTime.UtcNow));

			return View("Create", webFormEditViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.WebForms_Edit)]
		public ActionResult Edit(int? id, string Tab, string SortBy, bool? SortAscending)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("WebFormId = null");
			}

			Client client = CurrentClientOrThrow;
			Models.WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
			if (webForm == null)
			{
				AddUserMessage("Web Form not found", "Sorry, the Web Form you are trying to edit cannot be found. Web Form id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			WebFormEditViewModel viewModel = new WebFormEditViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
			return View("Edit", viewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.WebForms_View, GStoreAction.WebForms_Edit)]
		public ActionResult Details(int? id, string Tab, string SortBy, bool? SortAscending)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("WebFormId = null");
			}

			Client client = CurrentClientOrThrow;
			Models.WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
			if (webForm == null)
			{
				AddUserMessage("Web Form not found", "Sorry, the Web Form you are trying to view cannot be found. Web Form id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			WebFormEditViewModel viewModel = new WebFormEditViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
			return View("Details", viewModel);
		}


		[AuthorizeGStoreAction(true, GStoreAction.WebForms_View, GStoreAction.WebForms_Delete)]
		public ActionResult Delete(int? id, string Tab, string SortBy, bool? SortAscending)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("WebFormId = null");
			}

			Client client = CurrentClientOrThrow;
			Models.WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
			if (webForm == null)
			{
				AddUserMessage("Web Form not found", "Sorry, the Web Form you are trying to Delete cannot be found. Web Form id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			WebFormEditViewModel viewModel = new WebFormEditViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
			return View("Delete", viewModel);
		}

		[HttpPost]
		[ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.WebForms_Delete)]
		public ActionResult DeleteConfirmed(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Web Form Id = null");
			}

			Client client = CurrentClientOrThrow;
			Models.WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
			if (webForm == null)
			{
				AddUserMessage("Web Form not found", "Sorry, the Web Form you are trying to Delete cannot be found. It may have been deleted already. Web Form id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			string webFormName = webForm.Name;
			try
			{
				List<WebFormField> webFormFieldsToDelete = webForm.WebFormFields.ToList();
				foreach (WebFormField field in webFormFieldsToDelete)
				{
					GStoreDb.WebFormFields.Delete(field);
				}
				bool deleted = GStoreDb.WebForms.DeleteById(id.Value);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Web Form Deleted", "Web Form '" + webFormName.ToHtml() + "' [" + id + "] was deleted successfully. Web Form Fields deleted: " + webFormFieldsToDelete.Count + ".", AppHtmlHelpers.UserMessageType.Success);
					return RedirectToAction("Manager");
				}
				AddUserMessage("Web Form Delete Error", "There was an error deleting Web Form '" + webFormName.ToHtml() + "' [" + id + "]. It may have already been deleted.", AppHtmlHelpers.UserMessageType.Warning);
				return RedirectToAction("Manager");

			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Web Form '" + webFormName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Web Form Delete Error", errorMessage.ToHtml(), AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("Manager");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[GStore.Identity.AuthorizeGStoreAction(Identity.GStoreAction.WebForms_Edit)]
		public virtual PartialViewResult UpdateWebFormAjax(int? id, WebFormEditViewModel webFormEditViewModel, WebFormFieldEditViewModel[] WebFormFields, string FastAddField)
		{
			if (!id.HasValue)
			{
				return HttpBadRequestPartial("id is null");
			}

			if (webFormEditViewModel.WebFormId == 0)
			{
				return HttpBadRequestPartial("Web Form Id in view model is 0");
			}

			if (webFormEditViewModel.WebFormId != id.Value)
			{
				return HttpBadRequestPartial("Web Form Id mismatch. Parameter value: '" + id.Value + "' != view model value: " + webFormEditViewModel.WebFormId);
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			WebForm webFormToUpdate = storeFront.Client.WebForms.SingleOrDefault(wf => wf.WebFormId == webFormEditViewModel.WebFormId);

			if (webFormToUpdate == null)
			{
				throw new ApplicationException("Web Form not found in client web forms. WebFormId: " + webFormEditViewModel.WebFormId + " Client '" + storeFront.Client.Name + "' [" + storeFront.ClientId + "]");
			}

			bool nameIsValid = GStoreDb.ValidateWebFormName(this, webFormEditViewModel.Name, storeFront.ClientId, webFormEditViewModel.WebFormId);
			bool fastAddIsValid = false;
			if (!string.IsNullOrWhiteSpace(FastAddField))
			{
				fastAddIsValid = GStoreDb.ValidateWebFormFieldName(this, FastAddField, storeFront.ClientId, webFormEditViewModel.WebFormId, null);
			}

			if (nameIsValid && ModelState.IsValid)
			{

				WebForm webForm = null;
				try
				{
					webForm = GStoreDb.UpdateWebForm(webFormEditViewModel, storeFront, CurrentUserProfileOrThrow);

					if (WebFormFields != null && WebFormFields.Count() > 0)
					{
						foreach (WebFormFieldEditViewModel field in WebFormFields)
						{
							GStoreDb.UpdateWebFormField(field, storeFront, CurrentUserProfileOrThrow);
						}
					}

					if (fastAddIsValid)
					{
						WebFormField newField = GStoreDb.CreateWebFormFieldFastAdd(webFormEditViewModel, FastAddField, storeFront, CurrentUserProfileOrThrow);
						AddUserMessage("Field Created!", "Field '" + newField.Name.ToHtml() + "' [" + newField.WebFormFieldId + "] created successfully.", UserMessageType.Success);
						ModelState.Remove("FastAddField");
					}

					AddUserMessage("Web Form Changes Saved!", "Web Form '" + webForm.Name.ToHtml() + "' [" + webForm.WebFormId + "] saved successfully for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "]", AppHtmlHelpers.UserMessageType.Success);
					this.ModelState.Clear();
					webFormEditViewModel = new WebFormEditViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, isStoreAdminEdit: true, activeTab: webFormEditViewModel.ActiveTab, sortBy: webFormEditViewModel.SortBy, sortAscending: webFormEditViewModel.SortAscending);
					return PartialView("_WebFormEditPartial", webFormEditViewModel);
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while saving your changes to Web Form '" + webFormEditViewModel.Name + "' [" + webFormEditViewModel.WebFormId + "] for Client: '" + storeFront.Client.Name + "' [" + storeFront.ClientId + "] \nError: '" + ex.GetType().FullName + "'";

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): '" + ex.ToString() + "'";
					}
					AddUserMessage("Error Saving Web Form!", errorMessage.ToHtmlLines(), AppHtmlHelpers.UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Web Form Edit Error", "There was an error with your entry for Web Form " + webFormEditViewModel.Name.ToHtml() + " [" + webFormEditViewModel.WebFormId + "] for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "]. Please correct it.", AppHtmlHelpers.UserMessageType.Danger);
			}

			foreach (string key in this.ModelState.Keys.Where(k => k.StartsWith("WebFormFields[")).ToList())
			{
				this.ModelState.Remove(key);
			}


			webFormEditViewModel.FillFieldsFromViewModel(webFormToUpdate, WebFormFields);
			webFormEditViewModel.IsStoreAdminEdit = true;
			return PartialView("_WebFormEditPartial", webFormEditViewModel);
		}


	}
}
