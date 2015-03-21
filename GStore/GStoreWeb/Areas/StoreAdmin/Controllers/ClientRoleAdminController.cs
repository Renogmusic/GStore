using System;
using System.Web.Mvc;
using GStoreData.Identity;

namespace GStoreWeb.Areas.StoreAdmin.Controllers
{
	public class ClientRoleAdminController : AreaBaseController.StoreAdminAreaBaseController
    {
		public ActionResult AA_Status_Incomplete()
		{
			throw new NotImplementedException();
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_Manager)]
		public ActionResult Manager()
        {
			return View("Manager", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.ClientRole_Admin_Manager)]
		public ActionResult Admin_Manager()
		{
			return View("Admin_Manager", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_Admin_Create)]
		public ActionResult Admin_Create()
		{
			return View("Admin_Create", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_Admin_EditClaims)]
		public ActionResult Admin_EditClaims()
		{
			return View("Admin_EditClaims", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_Admin_Delete)]
		public ActionResult Admin_Delete()
		{
			return View("Admin_Delete", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_Admin_AssignUsers)]
		public ActionResult Admin_AssignUsers()
		{
			return View("Admin_AssignUsers", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_User_Manager)]
		public ActionResult User_Manager()
		{
			return View("User_Manager", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_User_Create)]
		public ActionResult User_Create()
		{
			return View("User_Create", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_User_EditClaims)]
		public ActionResult User_EditClaims()
		{
			return View("User_EditClaims", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_User_Delete)]
		public ActionResult User_Delete()
		{
			return View("User_Delete", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ClientRole_User_AssignUsers)]
		public ActionResult User_AssignUsers()
		{
			return View("User_AssignUsers", this.StoreAdminViewModel);
		}
	}
}
