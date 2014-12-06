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
	public class PageTemplateSysAdminController : BaseClasses.SystemAdminBaseController 
    {

        public ActionResult Index(int? clientId, string SortBy, bool? SortAscending)
        {
			clientId = FilterClientIdRaw();

			IQueryable<PageTemplate> query = null;
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					query = GStoreDb.PageTemplates.All();
				}
				else if (clientId.Value == 0)
				{
					query = GStoreDb.PageTemplates.Where(sb => sb.ClientId == null);
				}
				else
				{
					query = GStoreDb.PageTemplates.Where(sb => sb.ClientId == clientId.Value);
				}
			}
			else
			{
				query = GStoreDb.PageTemplates.All();
			}

			IOrderedQueryable<PageTemplate> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			return View(queryOrdered.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
				return HttpBadRequest("Page Template id is null");
            }
			PageTemplate pageTemplate = GStoreDb.PageTemplates.FindById(id.Value);
            if (pageTemplate == null)
            {
                return HttpNotFound();
            }
            return View(pageTemplate);
        }

        public ActionResult Create(int? clientId, string viewName)
		{

			ViewBag.ClientList = ClientList();
			PageTemplate model = GStoreDb.PageTemplates.Create();
			model.SetDefaultsForNew(clientId);
			if (!string.IsNullOrEmpty(viewName))
			{
				model.ViewName = viewName;
				model.Name = viewName;
				model.Description = viewName;
			}
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PageTemplate pageTemplate)
        {
			//check if Page Template name or folder is already taken
			ValidatePageTemplateName(pageTemplate);

            if (ModelState.IsValid)
            {
				Data.IGstoreDb db = GStoreDb;
				pageTemplate = db.PageTemplates.Create(pageTemplate);
				pageTemplate.UpdateAuditFields(CurrentUserProfileOrThrow);
				pageTemplate = GStoreDb.PageTemplates.Add(pageTemplate);
				GStoreDb.SaveChanges();
				AddUserMessage("Page Template Added", "Page Template '" + pageTemplate.Name.ToHtml() + "' [" + pageTemplate.PageTemplateId + "] created successfully!", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
            }
			ViewBag.ClientList = ClientList();
			return View(pageTemplate);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
				return HttpBadRequest("Page Template id is null");
			}
			PageTemplate pageTemplate = GStoreDb.PageTemplates.FindById(id.Value);
			if (pageTemplate == null)
            {
                return HttpNotFound();
            }
			ViewBag.ClientList = ClientList();
			return View(pageTemplate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Edit(PageTemplate pageTemplate)
		{
			ValidatePageTemplateName(pageTemplate);

            if (ModelState.IsValid)
			{
				PageTemplate originalValues = GStoreDb.PageTemplates.Single(c => c.PageTemplateId == pageTemplate.PageTemplateId);
				pageTemplate.UpdateAuditFields(CurrentUserProfileOrThrow);
				pageTemplate = GStoreDb.PageTemplates.Update(pageTemplate);
				AddUserMessage("Page Template changes saved", "Changes to Page Template '" + pageTemplate.Name.ToHtml() + "' [" + pageTemplate.PageTemplateId + "] saved successfully.", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.SaveChanges();

                return RedirectToAction("Index");
            }
			ViewBag.ClientList = ClientList();
			return View(pageTemplate);
        }

		public ActionResult Activate(int id)
		{
			this.ActivatePageTemplate(id);
			if (Request.UrlReferrer != null)
			{
				return Redirect(Request.UrlReferrer.ToString());

			}
			return RedirectToAction("Index");
		}

		public ActionResult ActivateSection(int id)
		{
			this.ActivatePageTemplateSection(id);
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
				return HttpBadRequest("Page Template id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			PageTemplate pageTemplate = db.PageTemplates.FindById(id.Value);
			if (pageTemplate == null)
            {
                return HttpNotFound();
            }
			return View(pageTemplate);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			try
			{
				PageTemplate target = GStoreDb.PageTemplates.FindById(id);
				if (target == null)
				{
					//Page Template not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Page Template. Page Template not found. It may be deleted by another user. Page Template Id: " + id);
				}
				List<PageTemplateSection> sectionsToDelete = target.Sections.ToList();
				foreach (PageTemplateSection section in sectionsToDelete)
				{
					GStoreDb.PageTemplateSections.Delete(section);
				}

				string pageTemplateName = target.Name;
				bool deleted = GStoreDb.PageTemplates.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Page Template Deleted", "Page Template '" + pageTemplateName.ToHtml() + "' [" + id + "] was deleted successfully. Sections deleted: " + sectionsToDelete.Count + ".", AppHtmlHelpers.UserMessageType.Success);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting Page Template. See inner exception for errors.  Related child tables may still have data to be deleted or Page Template may be deleted by another user. Page Template Id: " + id, ex);
			}
            return RedirectToAction("Index");
        }

		public ActionResult SectionIndex(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Page Template id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			PageTemplate template = db.PageTemplates.FindById(id.Value);
			if (template == null)
			{
				return HttpNotFound("Page Template not found. Page Template id: " + id);
			}

			return View(template);
		}

		public ActionResult SectionCreate(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Page Template id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			PageTemplate template = db.PageTemplates.FindById(id.Value);
			if (template == null)
			{
				return HttpNotFound("Page Template not found. Page Template id: " + id);
			}

			PageTemplateSection model = GStoreDb.PageTemplateSections.Create();
			model.SetDefaultsForNew(template);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SectionCreate(PageTemplateSection section)
		{
			ValidatePageTemplateSectionName(section);

			if (section.PageTemplateId == default(int))
			{
				return HttpBadRequest("Page Template id is 0");
			}

			Data.IGstoreDb db = GStoreDb;
			PageTemplate template = db.PageTemplates.FindById(section.PageTemplateId);
			if (template == null)
			{
				return HttpNotFound("Page Template not found. Page Template Id: " + section.PageTemplateId);
			}

			if (section.ClientId != template.ClientId)
			{
				return HttpBadRequest("View Model ClientId: " + section.ClientId + " does not match template client id: " + section.ClientId);
			}


			if (ModelState.IsValid)
			{
				section = GStoreDb.PageTemplateSections.Add(section);
				GStoreDb.SaveChanges();
				AddUserMessage("Page Template Section Created", "Page Template Section created successfully", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("SectionIndex", new { id = section.PageTemplateId });
			}

			return View(section);
		}

		public ActionResult SectionEdit(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Page Template Section id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			PageTemplateSection section = db.PageTemplateSections.FindById(id.Value);
			if (section == null)
			{
				return HttpNotFound("Page Template Section not found. Page Template Section Id: " + id);
			}

			return View(section);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SectionEdit(PageTemplateSection section)
		{
			ValidatePageTemplateSectionName(section);
			if (section.PageTemplateSectionId == 0)
			{
				return HttpBadRequest("section.PageTemplateSectionId is 0");
			}

			PageTemplateSection sectionToUpdate = GStoreDb.PageTemplateSections.FindById(section.PageTemplateSectionId);
			if (section == null)
			{
				return HttpNotFound("Page Template Section not found. Page Template Section Id: " + section.PageTemplateSectionId);
			}
			if (section.ClientId != sectionToUpdate.ClientId)
			{
				return HttpBadRequest("View Model ClientId: " + section.ClientId + " does not match template client id: " + section.ClientId);
			}

			if (ModelState.IsValid)
			{
				section.UpdateAuditFields(CurrentUserProfileOrThrow);
				section = GStoreDb.PageTemplateSections.Update(section);
				GStoreDb.SaveChanges();
				AddUserMessage("Page Template Section Updated", "Changes saved successfully to Page Template Section '" + section.Name.ToHtml() + "' [" + section.PageTemplateSectionId + "]", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("SectionIndex", new { id = section.PageTemplateId });
			}

			return View(section);
		}

		public ActionResult SectionDetails(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Page Template Section id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			PageTemplateSection section = db.PageTemplateSections.FindById(id.Value);
			if (section == null)
			{
				return HttpNotFound("Page Template Section not found by id: " + id);
			}

			return View(section);
		}

		public ActionResult SectionDelete(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Page Template Section id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			PageTemplateSection section = db.PageTemplateSections.FindById(id.Value);
			if (section == null)
			{
				return HttpNotFound("Page Template Section not found by id: " + id);
			}

			return View(section);
		}

		public ActionResult SectionSync(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("Page Template id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			PageTemplate template = db.PageTemplates.FindById(id.Value);
			if (template == null)
			{
				return HttpNotFound("Page Template not found by id: " + id);
			}
			Models.ViewModels.PageViewModel model = new Models.ViewModels.PageViewModel(null, false, false, false, true, id.Value);
			return View("~/Views/Page/" + template.ViewName + ".cshtml", model);
		}

		[HttpPost, ActionName("SectionDelete")]
		[ValidateAntiForgeryToken]
		public ActionResult SectionDeleteConfirmed(int id)
		{
			try
			{
				PageTemplateSection target = GStoreDb.PageTemplateSections.FindById(id);

				if (target == null)
				{
					//valueList not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Page Template Section! Section not found. It may have been deleted by another user. Page Template Section Id: " + id);
				}

				bool deleted = GStoreDb.PageTemplateSections.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Page Template Section Deleted", "Page Template Section [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
				}
				else
				{
					AddUserMessage("Deleting Page Template Section Failed!", "Deleting Page Template Section Failed. Page Template Section Id: " + id, AppHtmlHelpers.UserMessageType.Danger);
				}

				return RedirectToAction("SectionIndex", new { id = target.PageTemplateId });
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting Page Template Section.  See inner exception for errors.  Related child tables may still have data to be deleted. Page Template Section Id: " + id, ex);
			}
		}

    }
}
