using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.AppHtmlHelpers;

namespace GStoreWeb.Areas.BlogAdmin.Controllers.AreaBaseController
{
	public class BlogAdminAreaBaseController : GStoreData.Areas.BlogAdmin.ControllerBase.BlogAdminBaseController
    {
		public override sealed GStoreData.ControllerBase.BaseController GStoreErrorControllerForArea
		{
			get
			{
				return new GStoreWeb.Areas.BlogAdmin.Controllers.GStoreController();
			}
		}

		protected override sealed string Area
		{
			get
			{
				return "BlogAdmin";
			}
		}

		protected override sealed void HandleUnknownAction(string actionName)
		{
			if (!User.IsRegistered())
			{
				this.ExecuteBounceToLoginNoAccess("You must log in to view this page", this.TempData);
				return;
			}

			//perform redirect if not authorized for this area
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile == null)
			{
				this.ExecuteBounceToLoginNoAccess("You must log in with an active account to view this page", this.TempData);
				return;
			}

			StoreFront storeFront = CurrentStoreFrontOrNull;
			if (storeFront == null)
			{
				this.ExecuteBounceToLoginNoAccess("You must log in with an account on an active store front to view this page", this.TempData);
				return;
			}

			//check area permission
			if (!storeFront.Authorization_IsAuthorized(profile, GStoreAction.Admin_BlogAdminArea))
			{
				this.ExecuteBounceToLoginNoAccess("You must log in with an account that has permission to view this page", this.TempData);
				return;
			}

			base.HandleUnknownAction(actionName);
		}
		
	}
}
