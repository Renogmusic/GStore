using GStoreData.Identity;
using GStoreData.Models;

namespace GStoreWeb.Areas.StoreAdmin.Controllers.AreaBaseController
{
	public class StoreAdminAreaBaseController : GStoreData.Areas.StoreAdmin.ControllerBase.StoreAdminBaseController
    {
		public override sealed GStoreData.ControllerBase.BaseController GStoreErrorControllerForArea
		{
			get
			{
				return new GStoreWeb.Areas.StoreAdmin.Controllers.GStoreController();
			}
		}

		protected override sealed string Area
		{
			get
			{
				return "StoreAdmin";
			}
		}

		protected override sealed void HandleUnknownAction(string actionName)
		{
			if (!User.Identity.IsAuthenticated)
			{
				this.BounceToLogin("You must log in to view this page");
				return;
			}

			//perform redirect if not authorized for this area
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile == null)
			{
				this.BounceToLogin("You must log in with an active account to view this page");
				return;
			}

			StoreFront storeFront = CurrentStoreFrontOrNull;
			if (storeFront == null)
			{
				this.BounceToLogin("You must log in with an account on an active store front to view this page");
				return;
			}

			//check area permission
			if (!storeFront.Authorization_IsAuthorized(profile, GStoreAction.Admin_StoreAdminArea))
			{
				this.BounceToLogin("You must log in with an account that has permission to view this page");
				return;
			}

			base.HandleUnknownAction(actionName);
		}
		
	}

}
