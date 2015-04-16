using GStoreData;
using GStoreData.Models;
using GStoreData.AppHtmlHelpers;

namespace GStoreWeb.Areas.SystemAdmin.Controllers.AreaBaseController
{
	public class SystemAdminAreaBaseController : GStoreData.Areas.SystemAdmin.ControllerBase.SystemAdminBaseController
    {
		public override sealed GStoreData.ControllerBase.BaseController GStoreErrorControllerForArea
		{
			get
			{
				return new GStoreWeb.Areas.SystemAdmin.Controllers.GStoreController();
			}
		}

		protected override sealed string Area
		{
			get
			{
				return "SystemAdmin";
			}
		}

		protected override sealed void HandleUnknownAction(string actionName)
		{
			if (!User.IsRegistered())
			{
				this.ExecuteBounceToLoginNoAccess("You must log in to view this page", this.TempData);
				return;
			}
			//perform redirect if not authorized for system admin
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile == null)
			{
				this.ExecuteBounceToLoginNoAccess("You must log in with an active account to view this page", this.TempData);
				return;
			}
			if (!profile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				this.ExecuteBounceToLoginNoAccess("You must log in with an account that has permission to view this page", this.TempData);
				return;
			}

			base.HandleUnknownAction(actionName);
		}

	}
}
