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
	public class WebFormAdminController : AreaBaseController.StoreAdminAreaBaseController
	{
		[AuthorizeGStoreAction(GStoreAction.WebForms_Manager)]
		public ActionResult Manager(string SortBy, bool? SortAscending)
		{
			IOrderedQueryable<WebForm> webForms = CurrentClientOrThrow.WebForms.AsQueryable().ApplySort(this, SortBy, SortAscending);

			WebFormManagerAdminViewModel viewModel = new WebFormManagerAdminViewModel(CurrentStoreFrontConfigOrThrow, CurrentUserProfileOrThrow, webForms);
			return View("Manager", viewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.WebForms_Create)]
		public ActionResult Create(string Tab)
		{
			WebForm webForm = GStoreDb.WebForms.Create();
			webForm.SetDefaultsForNew(CurrentClientOrThrow);
			WebFormEditAdminViewModel viewModel = new WebFormEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, isStoreAdminEdit: true, isCreatePage:true);
			return View("Create", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.WebForms_Create)]
		public virtual ActionResult Create(WebFormEditAdminViewModel WebFormEditAdminViewModel)
		{

			Client client = CurrentClientOrThrow;
			bool nameIsValid = GStoreDb.ValidateWebFormName(this, WebFormEditAdminViewModel.Name, CurrentClientOrThrow.ClientId, null);

			if (nameIsValid && ModelState.IsValid)
			{
				try
				{
					WebForm webForm = null;
					webForm = GStoreDb.CreateWebForm(WebFormEditAdminViewModel, CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow);
					AddUserMessage("Web Form Created!", "Web Form '" + webForm.Name.ToHtml() + "' [" + webForm.WebFormId + "] was created successfully for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Success);
					if (CurrentStoreFrontOrThrow.Authorization_IsAuthorized(CurrentUserProfileOrThrow, GStoreAction.WebForms_View))
					{
						return RedirectToAction("Details", new { id = webForm.WebFormId });
					}
					return RedirectToAction("Index", "StoreAdmin");
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while Creating Web Form '" + WebFormEditAdminViewModel.Name.ToHtml() + "' for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "] \nError: " + ex.GetType().FullName;

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): " + ex.ToString();
					}
					AddUserMessage("Error Creating Web Form!", errorMessage.ToHtmlLines(), UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Web Form Create Error", "There was an error with your entry for new Web Form '" + WebFormEditAdminViewModel.Name.ToHtml() + "' for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]. Please correct it below and save.", UserMessageType.Danger);
			}
			WebFormEditAdminViewModel.IsStoreAdminEdit = true;
			WebFormEditAdminViewModel.IsCreatePage = true;
			WebFormEditAdminViewModel.IsActiveDirect = !(WebFormEditAdminViewModel.IsPending || (WebFormEditAdminViewModel.StartDateTimeUtc > DateTime.UtcNow) || (WebFormEditAdminViewModel.EndDateTimeUtc < DateTime.UtcNow));
			
			return View("Create", WebFormEditAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.WebForms_Edit)]
		public ActionResult Edit(int? id, string Tab, string SortBy, bool? SortAscending, int? webFormFieldId)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("WebFormId = null");
			}

			Client client = CurrentClientOrThrow;
			WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
			if (webForm == null)
			{
				AddUserMessage("Web Form not found", "Sorry, the Web Form you are trying to edit cannot be found. Web Form id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			//attempt sort so it can show if sort is unknown
			webForm.WebFormFields.AsQueryable().ApplySort(this, SortBy, SortAscending);

			WebFormEditAdminViewModel viewModel = new WebFormEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
			if (webFormFieldId.HasValue)
			{
				ViewData.Add("WebFormFieldId", webFormFieldId.Value);
			}
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
			WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
			if (webForm == null)
			{
				AddUserMessage("Web Form not found", "Sorry, the Web Form you are trying to view cannot be found. Web Form id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			WebFormEditAdminViewModel viewModel = new WebFormEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
			return View("Details", viewModel);
		}


		[AuthorizeGStoreAction(true, GStoreAction.WebForms_View, GStoreAction.WebForms_Delete)]
		public ActionResult Delete(int? id, string Tab, string SortBy, bool? SortAscending)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("WebFormId = null");
			}

			Client client= CurrentClientOrThrow;
			WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
			if (webForm == null)
			{
				AddUserMessage("Web Form not found", "Sorry, the Web Form you are trying to Delete cannot be found. Web Form id: [" + id.Value + "] for Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", UserMessageType.Danger);
				return RedirectToAction("Manager");

			}

			WebFormEditAdminViewModel viewModel = new WebFormEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, Tab, true, false, false, sortBy: SortBy, sortAscending: SortAscending);
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
			WebForm webForm = client.WebForms.Where(p => p.WebFormId == id.Value).SingleOrDefault();
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
					AddUserMessage("Web Form Deleted", "Web Form '" + webFormName.ToHtml() + "' [" + id + "] was deleted successfully. Web Form Fields deleted: " + webFormFieldsToDelete.Count + ".", UserMessageType.Success);
					return RedirectToAction("Manager");
				}
				AddUserMessage("Web Form Delete Error", "There was an error deleting Web Form '" + webFormName.ToHtml() + "' [" + id + "]. It may have already been deleted.", UserMessageType.Warning);
				return RedirectToAction("Manager");

			}
			catch (Exception ex)
			{
				string errorMessage = "There was an error deleting Web Form '" + webFormName + "' [" + id + "]. <br/>Error: '" + ex.GetType().FullName + "'";
				if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
				{
					errorMessage += " \nException.ToString(): " + ex.ToString();
				}
				AddUserMessage("Web Form Delete Error", errorMessage.ToHtml(), UserMessageType.Danger);
				return RedirectToAction("Manager");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[AuthorizeGStoreAction(GStoreAction.WebForms_Edit)]
		public virtual PartialViewResult UpdateWebFormAjax(int? id, WebFormEditAdminViewModel WebFormEditAdminViewModel, WebFormFieldEditAdminViewModel[] WebFormFields, string FastAddField)
		{
			if (!id.HasValue)
			{
				return HttpBadRequestPartial("id is null");
			}

			if (WebFormEditAdminViewModel.WebFormId == 0)
			{
				return HttpBadRequestPartial("Web Form Id in view model is 0");
			}

			if (WebFormEditAdminViewModel.WebFormId != id.Value)
			{
				return HttpBadRequestPartial("Web Form Id mismatch. Parameter value: '" + id.Value + "' != view model value: " + WebFormEditAdminViewModel.WebFormId);
			}

			StoreFront storeFront = CurrentStoreFrontOrThrow;
			WebForm webFormToUpdate = storeFront.Client.WebForms.SingleOrDefault(wf => wf.WebFormId == WebFormEditAdminViewModel.WebFormId);

			if (webFormToUpdate == null)
			{
				throw new ApplicationException("Web Form not found in client web forms. WebFormId: " + WebFormEditAdminViewModel.WebFormId + " Client '" + storeFront.Client.Name + "' [" + storeFront.ClientId + "]");
			}

			bool nameIsValid = GStoreDb.ValidateWebFormName(this, WebFormEditAdminViewModel.Name, storeFront.ClientId, WebFormEditAdminViewModel.WebFormId);
			bool fastAddIsValid = false;
			if (!string.IsNullOrWhiteSpace(FastAddField))
			{
				fastAddIsValid = GStoreDb.ValidateWebFormFieldName(this, FastAddField, storeFront.ClientId, WebFormEditAdminViewModel.WebFormId, null);
			}

			if (nameIsValid && ModelState.IsValid)
			{

				WebForm webForm = null;
				try
				{
					webForm = GStoreDb.UpdateWebForm(WebFormEditAdminViewModel, storeFront, CurrentUserProfileOrThrow);
					WebFormEditAdminViewModel.UpdateWebForm(webForm);

					if (WebFormFields != null && WebFormFields.Count() > 0)
					{
						foreach (WebFormFieldEditAdminViewModel field in WebFormFields)
						{
							GStoreDb.UpdateWebFormField(field, storeFront, CurrentUserProfileOrThrow);
						}
					}

					if (fastAddIsValid)
					{

						WebFormField newField = GStoreDb.CreateWebFormFieldFastAdd(WebFormEditAdminViewModel, FastAddField, storeFront, CurrentUserProfileOrThrow);
						AddUserMessage("Field Created!", "Field '" + newField.Name.ToHtml() + "' [" + newField.WebFormFieldId + "] created successfully.", UserMessageType.Success);
						ModelState.Remove("FastAddField");
					}

					AddUserMessage("Web Form Changes Saved!", "Web Form '" + webForm.Name.ToHtml() + "' [" + webForm.WebFormId + "] saved successfully for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "]", UserMessageType.Success);
					this.ModelState.Clear();
					WebFormEditAdminViewModel = new WebFormEditAdminViewModel(CurrentStoreFrontOrThrow, CurrentUserProfileOrThrow, webForm, isStoreAdminEdit: true, activeTab: WebFormEditAdminViewModel.ActiveTab, sortBy: WebFormEditAdminViewModel.SortBy, sortAscending: WebFormEditAdminViewModel.SortAscending);
					return PartialView("_WebFormEditPartial", WebFormEditAdminViewModel);
				}
				catch (Exception ex)
				{
					string errorMessage = "An error occurred while saving your changes to Web Form '" + WebFormEditAdminViewModel.Name + "' [" + WebFormEditAdminViewModel.WebFormId + "] for Client: '" + storeFront.Client.Name + "' [" + storeFront.ClientId + "] \nError: '" + ex.GetType().FullName + "'";

					if (CurrentUserProfileOrThrow.AspNetIdentityUserIsInRoleSystemAdmin())
					{
						errorMessage += " \nException.ToString(): '" + ex.ToString() + "'";
					}
					AddUserMessage("Error Saving Web Form!", errorMessage.ToHtmlLines(), UserMessageType.Danger);
					ModelState.AddModelError("Ajax", errorMessage);
				}
			}
			else
			{
				AddUserMessage("Web Form Edit Error", "There was an error with your entry for Web Form " + WebFormEditAdminViewModel.Name.ToHtml() + " [" + WebFormEditAdminViewModel.WebFormId + "] for Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.ClientId + "]. Please correct it.", UserMessageType.Danger);
			}

			foreach (string key in this.ModelState.Keys.Where(k => k.StartsWith("WebFormFields[")).ToList())
			{
				string temp = key.Remove(0, ("WebFormFields[").Length);
				temp = temp.Remove(temp.IndexOf(']'));
				int index = int.Parse(temp);

				System.Web.Mvc.ModelState value = null;
				this.ModelState.TryGetValue(key, out value);
				if (value.Errors.Count == 0)
				{
					this.ModelState.Remove(key);
				}
				else
				{
					this.ModelState.AddModelError("", "There was an error with field #" + (index + 1) + ". Please correct it and save again.");
				}
			}


			WebFormEditAdminViewModel.FillFieldsFromViewModel(webFormToUpdate, WebFormFields);
			WebFormEditAdminViewModel.IsStoreAdminEdit = true;
			return PartialView("_WebFormEditPartial", WebFormEditAdminViewModel);
		}

	}
}
