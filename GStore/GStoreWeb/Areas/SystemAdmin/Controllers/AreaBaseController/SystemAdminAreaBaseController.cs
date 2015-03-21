using GStoreData;
using GStoreData.Models;

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
			if (!User.Identity.IsAuthenticated)
			{
				this.BounceToLogin("You must log in to view this page");
				return;
			}
			//perform redirect if not authorized for system admin
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile == null)
			{
				this.BounceToLogin("You must log in with an active account to view this page");
				return;
			}
			if (!profile.AspNetIdentityUserIsInRoleSystemAdmin())
			{
				this.BounceToLogin("You must log in with an account that has permission to view this page");
				return;
			}

			base.HandleUnknownAction(actionName);
		}

	}
}
