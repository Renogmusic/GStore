using GStore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
	public class UserProfileAdminController : BaseClasses.StoreAdminBaseController
	{
		public ActionResult AA_Status_Incomplete()
		{
			throw new NotImplementedException();
		}
		[AuthorizeGStoreAction(GStoreAction.UserProfile_Manager)]
		public ActionResult Manager()
		{
			return View("Manager", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Account_CreateNew)]
		public ActionResult Account_CreateNew()
		{
			return View("Account_CreateNew", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Account_ResetPassword)]
		public ActionResult Account_ResetPassword()
		{
			return View("Account_ResetPassword", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.UserProfile_Profile_View, GStoreAction.UserProfile_Profile_Edit)]
		public ActionResult Profile_View()
		{
			return View("Profile_View", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Profile_Edit)]
		public ActionResult Profile_Edit()
		{
			return View("Profile_Edit", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Profile_Deactivate)]
		public ActionResult Profile_Deactivate()
		{
			return View("Profile_Deactivate", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Profile_Reactivate)]
		public ActionResult Profile_Reactivate()
		{
			return View("Profile_Reactivate", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Notifications_View)]
		public ActionResult Notifications_View()
		{
			return View("Notifications_View", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Orders_View)]
		public ActionResult Orders_View()
		{
			return View("Orders_View", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.UserProfile_Roles_AssignUserRoles)]
		public ActionResult Roles_AssignUserRoles()
		{
			return View("Roles_AssignUserRoles", this.StoreAdminViewModel);
		}

	}
}