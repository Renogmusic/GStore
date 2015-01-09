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
	public class UserProfileSysAdminController : BaseClasses.SystemAdminBaseController
	{

		public ActionResult Index(int? clientId, int? storeFrontId, string SortBy, bool? SortAscending)
		{
			SelectList clientFilterList = ClientFilterListWithAllAndNull(clientId, labelForNull: "(NULL)");
			SelectList storeFrontFilterList = StoreFrontFilterListWithAllAndNull(clientId, storeFrontId, labelForNull: "(NULL)");

			ViewBag.StoreFrontFilterList = StoreFrontFilterListWithAllAndNull(clientId, storeFrontId);
			clientId = FilterClientIdRaw();

			if (clientFilterList.SelectedValue != null)
			{
				clientId = int.Parse(clientFilterList.SelectedValue.ToString());
			}
			if (clientId == 0)
			{
				//null client set storefront to null also
				storeFrontId = 0;
			}
			else if (storeFrontFilterList.SelectedValue != null)
			{
				storeFrontId = int.Parse(storeFrontFilterList.SelectedValue.ToString());
			}

			IQueryable<UserProfile> query = null;
			if (clientId.HasValue)
			{
				if (clientId.Value == -1)
				{
					query = GStoreDb.UserProfiles.All();
				}
				else if (clientId.Value == 0)
				{
					query = GStoreDb.UserProfiles.Where(sb => sb.ClientId == null);
				}
				else
				{
					query = GStoreDb.UserProfiles.Where(sb => sb.ClientId == clientId.Value);
				}
			}
			else
			{
				//no client filter (all)
				query = GStoreDb.UserProfiles.All();
			}

			if (storeFrontId.HasValue && storeFrontId.Value != -1)
			{
				if (storeFrontId.Value == 0)
				{
					query = query.Where(sb => sb.StoreFrontId == null);
				}
				else
				{
					query = query.Where(sb => sb.StoreFrontId == storeFrontId.Value);
				}

			}

			IOrderedQueryable<UserProfile> queryOrdered = query.ApplySort(this, SortBy, SortAscending);
			this.BreadCrumbsFunc = htmlHelper => this.UserProfilesBreadcrumb(htmlHelper, clientId, storeFrontId, false);
			return View(queryOrdered.ToList());
		}

		public ActionResult Details(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("User Profile id is null");
			}
			UserProfile profile = GStoreDb.UserProfiles.FindById(id.Value);
			if (profile == null)
			{
				return HttpNotFound();
			}

			this.BreadCrumbsFunc = htmlHelper => this.UserProfileBreadcrumb(htmlHelper, profile.ClientId, profile.StoreFrontId, profile, false);
			return View(profile);
		}

		public ActionResult Create(int? clientId, int? storeFrontId)
		{
			ViewData.Add("CreateLoginAndIdentity", true);
			ViewData.Add("Password", string.Empty);
			ViewData.Add("SendWelcomeMessage", true);
			ViewData.Add("SendRegisteredNotify", true);

			UserProfile model = GStoreDb.UserProfiles.Create();
			model.SetDefaultsForNew(clientId, storeFrontId);
			this.BreadCrumbsFunc = htmlHelper => this.UserProfileBreadcrumb(htmlHelper, clientId, storeFrontId, model, false);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(UserProfile profile, string Password, bool? CreateLoginAndIdentity, bool? SendUserWelcome, bool? SendRegisteredNotify, bool? clientIdChanged)
		{
			ViewData.Add("CreateLoginAndIdentity", CreateLoginAndIdentity);
			ViewData.Add("Password", Password);
			ViewData.Add("SendWelcomeMessage", SendUserWelcome);
			ViewData.Add("SendRegisteredNotify", SendRegisteredNotify);

			Identity.AspNetIdentityUser identityUser = null;

			if (ModelState.IsValid && !(clientIdChanged ?? false))
			{
				Identity.AspNetIdentityContext identityCtx = new Identity.AspNetIdentityContext();
				
				if (CreateLoginAndIdentity.HasValue && CreateLoginAndIdentity.Value)
				{
					if (identityCtx.Users.Any(u => u.UserName == profile.UserName))
					{
						ModelState.AddModelError("UserName", "User Name is already taken, choose a new user name or edit the original user.");
					}
					if (identityCtx.Users.Any(u => u.Id == profile.UserId))
					{
						ModelState.AddModelError("UserId", "User Id is already taken, choose a new UserId or edit the original user.");
					}
					if (identityCtx.Users.Any(u => u.Email == profile.Email))
					{
						ModelState.AddModelError("Email", "Email is already taken, choose a new UserId or edit the original user.");
					}

					if (string.IsNullOrEmpty(Password))
					{
						ModelState.AddModelError("Password", "Password is null, enter a password for this user or uncheck Create Login And Identity.");
					}
					else if (Password.Length < 6)
					{
						ModelState.AddModelError("Password", "Password is too short. You must have at least 6 characters.");
					}

					if (ModelState.IsValid)
					{
						identityUser = identityCtx.CreateUserIfNotExists(profile.UserName, profile.Email, null, Password);
					}
				}

				
			}

			if (ModelState.IsValid && !(clientIdChanged ?? false))
			{
				UserProfile newProfile = GStoreDb.UserProfiles.Create(profile);
				newProfile.UpdateAuditFields(CurrentUserProfileOrThrow);
				if (identityUser != null)
				{
					newProfile.UserId = identityUser.Id;
				}
				else
				{
					newProfile.UserId = profile.Email;
				}
				newProfile = GStoreDb.UserProfiles.Add(newProfile);
				AddUserMessage("User Profile Added", "User Profile '" + profile.FullName.ToHtml() + "' &lt;" + profile.Email.ToHtml() + " &gt; [" + profile.UserProfileId + "] created successfully!", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.SaveChanges();

				GStoreDb.LogSecurityEvent_NewRegister(this.HttpContext, RouteData, newProfile, this);
				if ((SendUserWelcome ?? true) || (SendRegisteredNotify ?? true))
				{
					string notificationBaseUrl = Url.Action("Details", "Notifications", new { id = "" });
					CurrentStoreFrontOrThrow.HandleNewUserRegisteredNotifications(this.GStoreDb, Request, newProfile, notificationBaseUrl, SendUserWelcome ?? true, SendRegisteredNotify ?? true, string.Empty);
				}

				return RedirectToAction("Index");
			}

			int? clientId = null;
			if (profile.ClientId != default(int))
			{
				clientId = profile.ClientId;
			}
			int? storeFrontId = null;
			if (profile.StoreFrontId != default(int))
			{
				storeFrontId = profile.StoreFrontId;
			}

			this.BreadCrumbsFunc = htmlHelper => this.UserProfileBreadcrumb(htmlHelper, profile.ClientId, profile.StoreFrontId, profile, false);
			return View(profile);
		}

		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("User Profile id is null");
			}
			UserProfile profile = GStoreDb.UserProfiles.FindById(id.Value);
			if (profile == null)
			{
				return HttpNotFound();
			}

			this.BreadCrumbsFunc = htmlHelper => this.UserProfileBreadcrumb(htmlHelper, profile.ClientId, profile.StoreFrontId, profile, false);
			return View(profile);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(UserProfile profile, bool? clientIdChanged)
		{
			if (ModelState.IsValid && !(clientIdChanged ?? false))
			{
				profile.UpdateAuditFields(CurrentUserProfileOrThrow);
				profile = GStoreDb.UserProfiles.Update(profile);
				AddUserMessage("User Profile Updated", "User Profile '" + profile.FullName.ToHtml() + "' &lt;" + profile.Email.ToHtml() + " &gt; [" + profile.UserProfileId + "] updated successfully!", AppHtmlHelpers.UserMessageType.Success);
				GStoreDb.SaveChanges();

				return RedirectToAction("Index");
			}

			this.BreadCrumbsFunc = htmlHelper => this.UserProfileBreadcrumb(htmlHelper, profile.ClientId, profile.StoreFrontId, profile, false);
			return View(profile);
		}

		public ActionResult Activate(int id)
		{
			this.ActivateUserProfile(id);
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
				return HttpBadRequest("User Profile id is null");
			}
			Data.IGstoreDb db = GStoreDb;
			UserProfile profile = db.UserProfiles.FindById(id.Value);
			if (profile == null)
			{
				return HttpNotFound();
			}
			this.BreadCrumbsFunc = htmlHelper => this.UserProfileBreadcrumb(htmlHelper, profile.ClientId, profile.StoreFrontId, profile, false);
			return View(profile);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				UserProfile target = GStoreDb.UserProfiles.FindById(id);
				if (target == null)
				{
					//user profile not found, already deleted? overpost?
					throw new ApplicationException("Error deleting User Profile. User Profile not found. It may have been deleted by another user. User Profile Id: " + id);
				}

				bool deleted = GStoreDb.UserProfiles.DeleteById(id);
				GStoreDb.SaveChanges();
				AddUserMessage("User Profile deleted", "User Profile Id [" + id + "] was deleted successfully.", AppHtmlHelpers.UserMessageType.Info);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Error deleting User Profile.  See inner exception for errors.  Related child tables may still have data to be deleted. User Profile Id: " + id, ex);
			}
			return RedirectToAction("Index");
		}

	}
}
