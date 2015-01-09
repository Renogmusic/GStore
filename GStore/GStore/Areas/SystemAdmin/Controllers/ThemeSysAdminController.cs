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
	public class ThemeSysAdminController : BaseClasses.SystemAdminBaseController
	{

		public ActionResult Index(int? clientId, string SortBy, bool? SortAscending)
		{
			clientId = FilterClientIdRaw();

			IQueryable<Theme> query = null;
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					query = GStoreDb.Themes.All();
				}
				else if (clientId.Value == 0)
				{
					query = GStoreDb.Themes.Where(sb => sb.ClientId == null);
				}
				else
				{
					query = GStoreDb.Themes.Where(sb => sb.ClientId == clientId.Value);
				}
			}
			else
			{
				query = GStoreDb.Themes.All();
			}

			IOrderedQueryable<Theme> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			this.BreadCrumbsFunc = htmlHelper => this.ThemesBreadcrumb(htmlHelper, clientId, false);
			return View(queryOrdered.ToList());
		}

		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Theme id is null");
			}
			Theme theme = GStoreDb.Themes.FindById(id.Value);
			if (theme == null)
			{
				return HttpNotFound();
			}

			this.BreadCrumbsFunc = htmlHelper => this.ThemeBreadcrumb(htmlHelper, theme.ClientId, theme, false);
			return View(theme);
		}

		public ActionResult Create(int? clientId)
		{
			Client client = null;
			if (clientId.HasValue)
			{
				client = GStoreDb.Clients.FindById(clientId.Value);
			}

			Theme model = GStoreDb.Themes.Create();
			model.SetDefaultsForNew(client);
			this.BreadCrumbsFunc = htmlHelper => this.ThemeBreadcrumb(htmlHelper, clientId, model, false);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Theme theme)
		{
			//check if Theme name or folder is dupe for this client
			ValidateThemeName(theme);

			if (ModelState.IsValid)
			{
				theme = GStoreDb.Themes.Create(theme);
				theme.UpdateAuditFields(CurrentUserProfileOrThrow);
				theme = GStoreDb.Themes.Add(theme);
				AddUserMessage("Theme Added", "Theme '" + theme.Name.ToHtml() + "' [" + theme.ThemeId + "] created successfully!", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.SaveChanges();

				return RedirectToAction("Index");
			}
			int? clientId = null;
			if (theme.ClientId != default(int))
			{
				clientId = theme.ClientId;
			}

			this.BreadCrumbsFunc = htmlHelper => this.ThemeBreadcrumb(htmlHelper, theme.ClientId, theme, false);
			return View(theme);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Theme id is null");
			}
			Theme theme = GStoreDb.Themes.FindById(id.Value);
			if (theme == null)
			{
				return HttpNotFound();
			}

			this.BreadCrumbsFunc = htmlHelper => this.ThemeBreadcrumb(htmlHelper, theme.ClientId, theme, false);
			return View(theme);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Theme theme)
		{
			ValidateThemeName(theme);

			if (ModelState.IsValid)
			{
				theme.UpdateAuditFields(CurrentUserProfileOrThrow);
				theme = GStoreDb.Themes.Update(theme);
				AddUserMessage("Theme Updated", "Theme '" + theme.Name.ToHtml() + "' [" + theme.ThemeId + "] updated successfully!", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.SaveChanges();

				return RedirectToAction("Index");
			}

			this.BreadCrumbsFunc = htmlHelper => this.ThemeBreadcrumb(htmlHelper, theme.ClientId, theme, false);
			return View(theme);
		}

		public ActionResult Activate(int id)
		{
			this.ActivateTheme(id);
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
				return HttpBadRequest("Theme id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			Theme theme = db.Themes.FindById(id.Value);
			if (theme == null)
			{
				return HttpNotFound();
			}
			this.BreadCrumbsFunc = htmlHelper => this.ThemeBreadcrumb(htmlHelper, theme.ClientId, theme, false);
			return View(theme);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				Theme target = GStoreDb.Themes.FindById(id);
				if (target == null)
				{
					//Theme not found, already deleted? overpost?
					throw new ApplicationException("Error deleting Theme. Theme not found. It may have been deleted by another user. ThemeId: " + id);
				}
				string themeName = target.Name;

				bool deleted = GStoreDb.Themes.DeleteById(id);
				GStoreDb.SaveChanges();
				if (deleted)
				{
					AddUserMessage("Theme Deleted", "Theme '" + themeName.ToHtml() + "' [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Success);
				}
			}
			catch (Exception ex)
			{

				throw new ApplicationException("Error deleting Theme.  See inner exception for errors.  Related child tables may still have data to be deleted. ThemeId: " + id, ex);
			}
			return RedirectToAction("Index");
		}

	}
}
