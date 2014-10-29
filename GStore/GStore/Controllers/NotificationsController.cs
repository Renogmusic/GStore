using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GStore.Models;
using GStore.Models.Extensions;

namespace GStore.Controllers
{
	[Authorize]
	public class NotificationsController : BaseClass.BaseController
    {

		// GET: Notifications
		public ActionResult Index()
		{
			UserProfile userProfile = GStoreDb.GetCurrentUserProfile();
			List<Notification> notifications = GStoreDb.Notifications.Where(n => n.UserProfileId == userProfile.UserProfileId).OrderByDescending(n => n.CreateDateTimeUtc).ToList();
			return View(notifications);
		}

		[HttpPost]
		[Authorize(Roles="SystemAdmin")]
		public ActionResult Index(int? ViewForUserProfileId)
		{
			
			UserProfile userProfile;
			if (ViewForUserProfileId.HasValue)
			{
				userProfile = GStoreDb.UserProfiles.FindById(ViewForUserProfileId.Value);
				if (userProfile == null)
				{
					throw new ApplicationException("User Profile not found for UserProfileId: " + ViewForUserProfileId);
				}
			}
			else
			{
				userProfile = GStoreDb.GetCurrentUserProfile();
			}
			ViewData["SelectedUserProfileId"] = userProfile.UserProfileId;

			List<Models.Notification> notifications = GStoreDb.Notifications.Where(n => n.UserProfileId == userProfile.UserProfileId).OrderByDescending(n => n.CreateDateTimeUtc).ToList();
			return View(notifications);
		}

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			UserProfile userProfile = GStoreDb.GetCurrentUserProfile();

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				notification = GStoreDb.Notifications.Where(n => n.UserProfileId == userProfile.UserProfileId && n.NotificationId == id).SingleOrDefault();
			}

			if (notification == null)
			{
				return HttpNotFound();
			}

			if ((notification != null) && (!notification.Read) && (notification.UserProfileId == userProfile.UserProfileId))
			{
				//mark as read
				notification.Read = true;
				GStoreDb.SaveChanges();
			}

            return View(notification);
        }

		// GET: Notifications/MarkAsUnread/5
		public ActionResult MarkAsUnread(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			UserProfile userProfile = GStoreDb.GetCurrentUserProfile();

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				notification = GStoreDb.Notifications.Where(n => (n.UserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
			}

			if (notification == null)
			{
				return HttpNotFound();
			}

			if ((notification != null) && (notification.Read))
			{
				//mark as unread
				notification.Read = false;
				GStoreDb.SaveChanges();
			}

			return RedirectToAction("Index");

		}


        // GET: Notifications/Create
        public ActionResult Create()
        {

			SelectList profiles = null;
			if (User.IsInRole("SystemAdmin"))
			{
				var query = from profile in GStoreDb.UserProfiles.All()
							orderby profile.Email
							select new SelectListItem
							{
								Value = profile.UserProfileId.ToString()
								,
								Text = profile.FullName + " <" + profile.Email + "> " + (profile.AllowUsersToSendSiteMessages ? "(public)" : "(private)")
							};

				//todo: consider adding security admin and welcome people on top;
				profiles = new SelectList(query.ToList(), "Value", "Text");
			}
			else
			{
				int currentUserProfileId = GStoreDb.GetCurrentUserProfile().UserProfileId;
				profiles = new SelectList(GStoreDb.UserProfiles.Where(prof => prof.UserProfileId != currentUserProfileId && prof.AllowUsersToSendSiteMessages == true).OrderBy(prof => prof.Email), "UserProfileId", "FullName");
			}

			string[] importanceItems = {"Low", "Normal", "High" };

			SelectList importance = new SelectList(importanceItems, "Normal");
			ViewBag.Importance = importance;

			ViewBag.UserProfileId = profiles;
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "UserProfileId,Subject,Importance,Message")] Models.Notification notification, [Bind(Include="Order,Url,LinkText")] Models.NotificationLink[] links)
        {

			UserProfile target = GStoreDb.UserProfiles.SingleOrDefault(prof => prof.UserProfileId == notification.UserProfileId);
			if (target == null)
			{
				ModelState.AddModelError("", "Target recipient is not found. Please email the system administrator if you think this is an error.");
			}
			if (!User.IsInRole("SystemAdmin"))
			{
				if (!target.AllowUsersToSendSiteMessages)
				{
					ModelState.AddModelError("", "You are not authorized to send a message to the selected user. Please email the system administrator if you think this is an error.");
				}
			}

			if (ModelState.IsValid)
            {
				UserProfile sender = GStoreDb.GetCurrentUserProfile();
				notification.FromUserProfileId = sender.UserProfileId;
				notification.From = sender.FullName;
				notification.To = target.FullName;
				notification.UrlHost = Request.Url.Host;
				if (!Request.Url.IsDefaultPort)
				{
					notification.UrlHost += ":" + Request.Url.Port;
				}

				notification.BaseUrl = Url.Action("Details", "Notifications", new { id = "" });

				int linkCount = 0;
				List<NotificationLink> linkCollection = new List<NotificationLink>();
				foreach (NotificationLink link in links.Where(l => !(string.IsNullOrWhiteSpace(l.Url) && !string.IsNullOrWhiteSpace(l.LinkText))))
				{
					linkCount++;
					link.Notification = notification;
					if (link.Url.StartsWith("/") || link.Url.StartsWith("~/"))
					{
						link.IsExternal = false;
					}
					else
					{
						link.IsExternal = true;
					}
					linkCollection.Add(link);
				}
				if (linkCount != 0)
				{
					notification.NotificationLinks = linkCollection;
				}
				GStoreDb.Notifications.Add(notification);
				GStoreDb.SaveChanges();
                return RedirectToAction("Index");
            }

			SelectList profiles = null;
			if (User.IsInRole("SystemAdmin"))
			{
				var query = from profile in GStoreDb.UserProfiles.All()
							orderby profile.Email
							select new SelectListItem
							{
								Value = profile.UserProfileId.ToString()
								,
								Text = profile.FullName + " <" + profile.Email + "> " + (profile.AllowUsersToSendSiteMessages ? "(public)" : "(private)")
							};

				//todo: consider adding security admin and welcome people on top;
				profiles = new SelectList(query.ToList(), "Value", "Text");
			}
			else
			{
				int currentUserProfileId = GStoreDb.GetCurrentUserProfile().UserProfileId;
				profiles = new SelectList(GStoreDb.UserProfiles.Where(prof => prof.UserProfileId != currentUserProfileId && prof.AllowUsersToSendSiteMessages == true).OrderBy(prof => prof.Email), "UserProfileId", "FullName");
			}

			ViewBag.UserProfileId = profiles;
            return View(notification);
        }

        // GET: Notifications/Edit/5
		[Authorize(Roles="SystemAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				UserProfile userProfile = GStoreDb.GetCurrentUserProfile();
				notification = GStoreDb.Notifications.Where(n => (n.UserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
			}

			if (notification == null)
            {
                return HttpNotFound();
            }

			SelectList profiles = null;
			if (User.IsInRole("SystemAdmin"))
			{
				var query = from profile in GStoreDb.UserProfiles.All()
							orderby profile.Email
							select new SelectListItem
							{
								Value = profile.UserProfileId.ToString()
								,
								Text = profile.FullName + " <" + profile.Email + "> " + (profile.AllowUsersToSendSiteMessages ? "(public)" : "(private)")
							};

				//todo: consider adding security admin and welcome people on top;
				profiles = new SelectList(query.ToList(), "Value", "Text");
			}
			else
			{
				int currentUserProfileId = GStoreDb.GetCurrentUserProfile().UserProfileId;
				profiles = new SelectList(GStoreDb.UserProfiles.Where(prof => prof.UserProfileId != currentUserProfileId && prof.AllowUsersToSendSiteMessages == true).OrderBy(prof => prof.Email), "UserProfileId", "FullName");
			}

            ViewBag.UserProfileId = profiles;
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "SystemAdmin")]
		public ActionResult Edit([Bind(Include = "NotificationId,UserProfileId,To,From,Subject,Importance,Message,Read,CreateDateTimeUtc,NotificationLinks")] Models.Notification notification)
		{
            if (ModelState.IsValid)
            {
				GStoreDb.Notifications.Update(notification);
				foreach (NotificationLink link in notification.NotificationLinks)
				{
					GStoreDb.NotificationLinks.Update(link);
				}
				GStoreDb.SaveChanges();
                return RedirectToAction("Index");
            }

			SelectList profiles = null;
			if (User.IsInRole("SystemAdmin"))
			{
				var query = from profile in GStoreDb.UserProfiles.All()
							orderby profile.Email
							select new SelectListItem
							{
								Value = profile.UserProfileId.ToString()
								,
								Text = profile.FullName + " <" + profile.Email + "> " + (profile.AllowUsersToSendSiteMessages ? "(public)" : "(private)")
							};

				//todo: consider adding security admin and welcome people on top;
				profiles = new SelectList(query.ToList(), "Value", "Text");
			}
			else
			{
				int currentUserProfileId = GStoreDb.GetCurrentUserProfile().UserProfileId;
				profiles = new SelectList(GStoreDb.UserProfiles.Where(prof => prof.UserProfileId != currentUserProfileId && prof.AllowUsersToSendSiteMessages == true).OrderBy(prof => prof.Email), "UserProfileId", "FullName");
			}

			ViewBag.UserProfileId = profiles;
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				UserProfile userProfile = GStoreDb.GetCurrentUserProfile();
				notification = GStoreDb.Notifications.Where(n => (n.UserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
			}

            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				UserProfile userProfile = GStoreDb.GetCurrentUserProfile();
				notification = GStoreDb.Notifications.Where(n => (n.UserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
			}

			GStoreDb.Notifications.Delete(notification);
			GStoreDb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
