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
	public class WebFormSysAdminController : BaseClasses.SystemAdminBaseController
	{

		public ActionResult Index(int? clientId, string SortBy, bool? SortAscending)
		{
			clientId = FilterClientIdRaw();

			IQueryable<WebForm> query = null;
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					query = GStoreDb.WebForms.All();
				}
				else if (clientId.Value == 0)
				{
					query = GStoreDb.WebForms.Where(sb => sb.ClientId == null);
				}
				else
				{
					query = GStoreDb.WebForms.Where(sb => sb.ClientId == clientId.Value);
				}
			}
			else
			{
				query = GStoreDb.WebForms.All();
			}

			IOrderedQueryable<WebForm> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			return View(queryOrdered.ToList());
		}

		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Web Form Id is null");
			}
			WebForm webForm = GStoreDb.WebForms.FindById(id.Value);
			if (webForm == null)
			{
				return HttpNotFound("Web Form not found. Web Form id: " + id);
			}
			return View(webForm);
		}

		public ActionResult Create(int? clientId)
		{
			ViewBag.ClientList = ClientList();

			WebForm model = GStoreDb.WebForms.Create();
			model.SetDefaultsForNew(clientId);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(WebForm webForm)
		{
			if (ModelState.IsValid)
			{
				webForm = GStoreDb.WebForms.Create(webForm);
				webForm.UpdateAuditFields(CurrentUserProfileOrThrow);
				webForm = GStoreDb.WebForms.Add(webForm);
				GStoreDb.SaveChanges();
				AddUserMessage("Web Form Created", "Web Form '" + webForm.Name.ToHtml() + "' [" + webForm.WebFormId + "] Created Successfully", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
			}
			int? clientId = null;
			if (webForm.ClientId != default(int))
			{
				clientId = webForm.ClientId;
			}

			ViewBag.ClientList = ClientList();

			return View(webForm);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Web Form Id is null");
			}
			WebForm webForm = GStoreDb.WebForms.FindById(id.Value);
			if (webForm == null)
			{
				return HttpNotFound();
			}
			ViewBag.ClientList = ClientList();

			return View(webForm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(WebForm webForm)
		{
			if (ModelState.IsValid)
			{
				webForm.UpdateAuditFields(CurrentUserProfileOrThrow);
				webForm = GStoreDb.WebForms.Update(webForm);
				GStoreDb.SaveChanges();
				AddUserMessage("Web Form Updated", "Changes saved successfully to Web Form '" + webForm.Name.ToHtml() + "' [" + webForm.WebFormId + "]", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
			}
			ViewBag.ClientList = ClientList();

			return View(webForm);
		}

		public ActionResult Activate(int id)
		{
			this.ActivateWebForm(id);
			if (Request.UrlReferrer != null)
			{
				return Redirect(Request.UrlReferrer.ToString());

			}
			return RedirectToAction("Index");
		}

		public ActionResult ActivateField(int id)
		{
			this.ActivateWebFormField(id);
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
				return HttpBadRequest("Web Form Id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			WebForm webForm = db.WebForms.FindById(id.Value);
			if (webForm == null)
			{
				return HttpNotFound();
			}
			return View(webForm);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				WebForm target = GStoreDb.WebForms.FindById(id);

				if (target == null)
				{
					//webForm not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Web Form. Web Form not found. It may have been deleted by another user. Web Form Id: " + id);
				}

				List<WebFormField> fieldsToDelete = target.WebFormFields.ToList();
				foreach (WebFormField webFormField in fieldsToDelete)
				{
					GStoreDb.WebFormFields.Delete(webFormField);
				}

				bool deleted = GStoreDb.WebForms.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Web Form Deleted", "Web Form [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
				}
				else
				{
					AddUserMessage("Deleting Web Form Failed!", "Deleting Web Form Failed. Web Form Id: " + id, AppHtmlHelpers.UserMessageType.Danger);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting Web Form.  See inner exception for errors.  Related child tables may still have data to be deleted. Web Form Id: " + id, ex);
			}
			return RedirectToAction("Index");
		}

		public ActionResult FieldIndex(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Web Form id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			WebForm webForm = db.WebForms.FindById(id.Value);
			if (webForm == null)
			{
				return HttpNotFound("Web Form not found. Web Form id: " + id);
			}

			return View(webForm);
		}

		public ActionResult FieldCreate(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Web Form id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			WebForm webForm = db.WebForms.FindById(id.Value);
			if (webForm == null)
			{
				return HttpNotFound("Web Form not found. Web Form id: " + id);
			}

			WebFormField model = GStoreDb.WebFormFields.Create();
			model.SetDefaultsForNew(webForm);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult FieldCreate(WebFormField webFormField)
		{
			if (webFormField.WebFormId == default(int))
			{
				return HttpBadRequest("Web Form id is 0");
			}

			Data.IGstoreDb db = GStoreDb;
			WebForm webForm = db.WebForms.FindById(webFormField.WebFormId);
			if (webForm == null)
			{
				return HttpNotFound("Web Form not found. Web Form id: " + webFormField.WebFormId);
			}

			if (ModelState.IsValid)
			{
				webFormField.ClientId = webForm.ClientId;
				webFormField.WebFormId = webForm.WebFormId;
				webFormField.DataTypeString = webFormField.DataType.ToDisplayName();
				GStoreDb.WebFormFields.Add(webFormField);
				GStoreDb.SaveChanges();
				AddUserMessage("Web Form Field Created", "Web Form Field '" + webFormField.Name.ToHtml() + "' [" + webFormField.WebFormFieldId + "] created successfully", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("FieldIndex", new { id = webFormField.WebFormId });
			}

			return View(webFormField);
		}

		public ActionResult FieldEdit(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Web Form Field id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			WebFormField webFormField = db.WebFormFields.FindById(id.Value);
			if (webFormField == null)
			{
				return HttpNotFound("Web Form Field not found. Web Form Field id: " + id);
			}
			ViewBag.ClientList = ClientList();

			return View(webFormField);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult FieldEdit(WebFormField webFormField)
		{
			if (ModelState.IsValid)
			{
				webFormField.DataTypeString = webFormField.DataType.ToDisplayName();
				webFormField.UpdateAuditFields(CurrentUserProfileOrThrow);
				webFormField = GStoreDb.WebFormFields.Update(webFormField);
				GStoreDb.SaveChanges();
				AddUserMessage("Web Form Field Updated", "Changes saved successfully to Web Form Field '" + webFormField.Name.ToHtml() + "' [" + webFormField.WebFormFieldId + "]", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("FieldIndex", new { id = webFormField.WebFormId });
			}
			webFormField.WebForm = GStoreDb.WebForms.Single(wf => wf.WebFormId == webFormField.WebFormId);
			ViewBag.ClientList = ClientList();

			return View(webFormField);
		}

		public ActionResult FieldDetails(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Web Form Field id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			WebFormField webFormField = db.WebFormFields.FindById(id.Value);
			if (webFormField == null)
			{
				return HttpNotFound("Web Form Field not found. Web Form Field id: " + id);
			}

			return View(webFormField);
		}

		public ActionResult FieldDelete(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Web Form Field id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			WebFormField webFormField = db.WebFormFields.FindById(id.Value);
			if (webFormField == null)
			{
				return HttpNotFound("Web Form Field not found. Web Form Field id: " + id);
			}

			return View(webFormField);
		}

		[HttpPost, ActionName("FieldDelete")]
		[ValidateAntiForgeryToken]
		public ActionResult FieldDeleteConfirmed(int id)
		{
			try
			{
				WebFormField target = GStoreDb.WebFormFields.FindById(id);

				if (target == null)
				{
					//webForm not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Web Form Field. Web Form Field not found. It may have been deleted by another user. Web Form Field Id: " + id);
				}

				bool deleted = GStoreDb.WebFormFields.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Web Form Field Deleted", "Web Form Field [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
				}
				else
				{
					AddUserMessage("Deleting Web Form Field Failed!", "Deleting Web Form Field Failed. Web Form Id: " + id, AppHtmlHelpers.UserMessageType.Danger);
				}

				return RedirectToAction("FieldIndex", new { id = target.WebFormId });
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting Web Form Field.  See inner exception for errors.  Related child tables may still have data to be deleted. Web Form Field Id: " + id, ex);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult FieldFastAdd(int webFormId, string stringValue)
		{
			if (string.IsNullOrEmpty(stringValue))
			{
				AddUserMessage("Field not added", "You must enter text to add a Web Form Field", AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("FieldIndex", new { id = webFormId });
			}

			WebForm webForm = GStoreDb.WebForms.FindById(webFormId);
			if (webForm == null)
			{
				return HttpBadRequest("Web Form not found by id: " + webFormId);
			}

			if (webForm.WebFormFields.Any(vl => vl.Name.ToLower() == stringValue.ToLower()))
			{
				AddUserMessage("Field not added", "Field with name '" + stringValue.ToHtml() + "' already exists in this Web Form. Use a different Field Name or remove the old Field first.", AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("FieldIndex", new { id = webFormId });
			}

			WebFormField webFormField = GStoreDb.WebFormFields.Create();
			webFormField.SetDefaultsForNew(webForm);
			webFormField.Name = stringValue;
			webFormField.Description = stringValue;
			webFormField.LabelText = stringValue;
			webFormField.HelpLabelTopText = string.Empty;
			webFormField.HelpLabelBottomText = string.Empty;
			webFormField.IsPending = false;
			if (webForm.WebFormFields.Count == 0)
			{
				webFormField.Order = 1000;
			}
			else
			{
				webFormField.Order = webForm.WebFormFields.Max(vl => vl.Order) + 10;
			}

			GStoreDb.WebFormFields.Add(webFormField);
			GStoreDb.SaveChanges();

			AddUserMessage("Field added to Web Form", "Field '" + stringValue.ToHtml() + "' [" + webFormField.WebFormFieldId + "] was successfully added to the Web Form", AppHtmlHelpers.UserMessageType.Success);
			return RedirectToAction("FieldIndex", new { id = webFormId });
		}

	}
}
