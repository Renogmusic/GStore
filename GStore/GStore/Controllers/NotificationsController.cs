using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GStore.Models;
using GStore.Data;

namespace GStore.Controllers
{
	[Authorize]
	public class NotificationsController : BaseClass.BaseController
    {
		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontOrThrow.NotificationsLayoutName;
			}
		}

		// GET: Notifications
		public ActionResult Index()
		{
			UserProfile userProfile = CurrentUserProfileOrThrow;
			List<Notification> notifications = GStoreDb.Notifications.Where(n => n.ToUserProfileId == userProfile.UserProfileId).OrderByDescending(n => n.CreateDateTimeUtc).ToList();
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
				userProfile = CurrentUserProfileOrThrow;
			}
			ViewData["SelectedUserProfileId"] = userProfile.UserProfileId;

			List<Models.Notification> notifications = GStoreDb.Notifications.Where(n => n.ToUserProfileId == userProfile.UserProfileId).OrderByDescending(n => n.CreateDateTimeUtc).ToList();
			return View(notifications);
		}

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
				return HttpBadRequest("Notification id is null");
			}

			UserProfile userProfile = CurrentUserProfileOrThrow;

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				notification = GStoreDb.Notifications.Where(n => n.ToUserProfileId == userProfile.UserProfileId && n.NotificationId == id).SingleOrDefault();
			}

			if (notification == null)
			{
				return HttpNotFound();
			}

			if ((notification != null) && (!notification.Read) && (notification.ToUserProfileId == userProfile.UserProfileId))
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
				return HttpBadRequest("Notification id is null");
			}

			UserProfile userProfile = CurrentUserProfileOrThrow;

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				notification = GStoreDb.Notifications.Where(n => (n.ToUserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
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
			ViewBag.Importance = ImportanceItems();
			ViewBag.ToUserProfileId = AllowedToProfiles();
			return View();
		}

		protected SelectList ImportanceItems()
		{
			string[] importanceItems = { "Low", "Normal", "High" };
			return new SelectList(importanceItems, "Normal");
		}

		protected SelectList AllowedToProfiles()
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
				int currentUserProfileId = CurrentUserProfileOrThrow.UserProfileId;
				profiles = new SelectList(GStoreDb.UserProfiles.Where(prof => prof.UserProfileId != currentUserProfileId && prof.AllowUsersToSendSiteMessages == true).OrderBy(prof => prof.Email), "UserProfileId", "FullName");
			}
			return profiles;
		}

        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Create(Models.ViewModels.NotificationCreateViewModel data)
        {
			UserProfile target = GStoreDb.UserProfiles.SingleOrDefault(prof => prof.UserProfileId == data.ToUserProfileId);
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
			if (!ModelState.IsValid)
			{
				ViewBag.Importance = ImportanceItems();
				ViewBag.ToUserProfileId = AllowedToProfiles();
				return View(data);
			}

			Notification notification = GStoreDb.Notifications.Create();
			UserProfile sender = CurrentUserProfileOrThrow;
			notification.FromUserProfileId = sender.UserProfileId;
			notification.From = sender.FullName;
			notification.To = target.FullName;
			notification.Subject = data.Subject;
			notification.ToUserProfileId = data.ToUserProfileId;
			notification.Importance = data.Importance;
			notification.Message = data.Message;
			notification.UrlHost = Request.Url.Host;
			notification.Client = CurrentClientOrThrow;
			notification.StoreFront = CurrentStoreFrontOrThrow;
			if (!Request.Url.IsDefaultPort)
			{
				notification.UrlHost += ":" + Request.Url.Port;
			}

			notification.BaseUrl = Url.Action("Details", "Notifications", new { id = "" });

			List<NotificationLink> linkCollection = new List<NotificationLink>();
			if (!string.IsNullOrWhiteSpace(data.Link1Url))
			{
				if (string.IsNullOrWhiteSpace(data.Link1Text))
				{
					data.Link1Text = data.Link1Url;
				}
				NotificationLink newLink1 = GStoreDb.NotificationLinks.Create();
				newLink1.Notification = notification;
				newLink1.Order = 1;
				newLink1.LinkText = data.Link1Text;
				newLink1.Url = data.Link1Url;
				newLink1.Client = CurrentClientOrThrow;
				newLink1.StoreFront = CurrentStoreFrontOrThrow;
				if (data.Link1Url.StartsWith("/") || data.Link1Url.StartsWith("~/"))
				{
					newLink1.IsExternal = false;
				}
				else
				{
					newLink1.IsExternal = true;
				}
				linkCollection.Add(newLink1);
			}
			if (!string.IsNullOrWhiteSpace(data.Link2Url))
			{
				if (string.IsNullOrWhiteSpace(data.Link2Text))
				{
					data.Link2Text = data.Link2Url;
				}
				NotificationLink newLink2 = GStoreDb.NotificationLinks.Create();
				newLink2.Notification = notification;
				newLink2.Order = 2;
				newLink2.LinkText = data.Link2Text;
				newLink2.Url = data.Link2Url;
				newLink2.Client = CurrentClientOrThrow;
				newLink2.StoreFront = CurrentStoreFrontOrThrow;
				if (data.Link2Url.StartsWith("/") || data.Link2Url.StartsWith("~/"))
				{
					newLink2.IsExternal = false;
				}
				else
				{
					newLink2.IsExternal = true;
				}
				linkCollection.Add(newLink2);
			}
			if (!string.IsNullOrWhiteSpace(data.Link3Url))
			{
				if (string.IsNullOrWhiteSpace(data.Link3Text))
				{
					data.Link3Text = data.Link3Url;
				}
				NotificationLink newLink3 = GStoreDb.NotificationLinks.Create();
				newLink3.Notification = notification;
				newLink3.Order = 3;
				newLink3.LinkText = data.Link3Text;
				newLink3.Url = data.Link3Url;
				newLink3.Client = CurrentClientOrThrow;
				newLink3.StoreFront = CurrentStoreFrontOrThrow;
				if (data.Link3Url.StartsWith("/") || data.Link3Url.StartsWith("~/"))
				{
					newLink3.IsExternal = false;
				}
				else
				{
					newLink3.IsExternal = true;
				}
				linkCollection.Add(newLink3);
			}
			if (!string.IsNullOrWhiteSpace(data.Link4Url))
			{
				if (string.IsNullOrWhiteSpace(data.Link4Text))
				{
					data.Link4Text = data.Link4Url;
				}
				NotificationLink newLink4 = GStoreDb.NotificationLinks.Create();
				newLink4.Notification = notification;
				newLink4.Order = 4;
				newLink4.LinkText = data.Link4Text;
				newLink4.Url = data.Link4Url;
				newLink4.Client = CurrentClientOrThrow;
				newLink4.StoreFront = CurrentStoreFrontOrThrow;
				if (data.Link4Url.StartsWith("/") || data.Link4Url.StartsWith("~/"))
				{
					newLink4.IsExternal = false;
				}
				else
				{
					newLink4.IsExternal = true;
				}
				linkCollection.Add(newLink4);
			}


			if (linkCollection.Count != 0)
			{
				notification.NotificationLinks = linkCollection;
			}

			GStoreDb.Notifications.Add(notification);
			GStoreDb.SaveChanges();
			return RedirectToAction("Index");
        }

        // GET: Notifications/Edit/5
		[Authorize(Roles="SystemAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
				return HttpBadRequest("Notification id is null");
			}

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				UserProfile userProfile = CurrentUserProfileOrThrow;
				notification = GStoreDb.Notifications.Where(n => (n.ToUserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
			}

			if (notification == null)
            {
                return HttpNotFound();
            }

            ViewBag.ToUserProfileId = AllowedToProfiles();
            return View(notification);
        }

		public ActionResult Reply(int? id)
		{
			if (id == null)
			{
				return HttpBadRequest("Notification id is null");
			}

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				UserProfile userProfile = CurrentUserProfileOrThrow;
				notification = GStoreDb.Notifications.Where(n => (n.ToUserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
			}

			if (notification == null)
			{
				return HttpNotFound();
			}

			Models.ViewModels.NotificationCreateViewModel viewModel = new Models.ViewModels.NotificationCreateViewModel();
			viewModel.StartReply(notification);
			ViewBag.ToUserProfileId = AllowedToProfiles();
			ViewBag.Importance = ImportanceItems();
			return View("Create", viewModel);
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
				notification = GStoreDb.Notifications.Update(notification);
				foreach (NotificationLink link in notification.NotificationLinks)
				{
					GStoreDb.NotificationLinks.Update(link);
				}
				GStoreDb.SaveChanges();
                return RedirectToAction("Index");
            }

			ViewBag.ToUserProfileId = AllowedToProfiles();
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
			if (id == null)
			{
				return HttpBadRequest("Notification id is null");
			}

			Notification notification = null;
			if (User.IsInRole("SystemAdmin"))
			{
				notification = GStoreDb.Notifications.SingleOrDefault(n => n.NotificationId == id);
			}
			else
			{
				UserProfile userProfile = CurrentUserProfileOrThrow;
				notification = GStoreDb.Notifications.Where(n => (n.ToUserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
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
				UserProfile userProfile = CurrentUserProfileOrThrow;
				notification = GStoreDb.Notifications.Where(n => (n.ToUserProfileId == userProfile.UserProfileId) && (n.NotificationId == id)).SingleOrDefault();
			}

			GStoreDb.Notifications.Delete(notification);
			GStoreDb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
