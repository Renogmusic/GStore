using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.AppHtmlHelpers;

namespace GStoreWeb.Areas.CatalogAdmin.Controllers.AreaBaseController
{
	public class CatalogAdminAreaBaseController : GStoreData.Areas.CatalogAdmin.ControllerBase.CatalogAdminBaseController
    {
		public override sealed GStoreData.ControllerBase.BaseController GStoreErrorControllerForArea
		{
			get
			{
				return new GStoreWeb.Areas.CatalogAdmin.Controllers.GStoreController();
			}
		}

		protected override sealed string Area
		{
			get
			{
				return "CatalogAdmin";
			}
		}

		protected override sealed void HandleUnknownAction(string actionName)
		{
			if (!User.IsRegistered())
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
			if (!storeFront.Authorization_IsAuthorized(profile, GStoreAction.Admin_CatalogAdminArea))
			{
				this.BounceToLogin("You must log in with an account that has permission to view this page");
				return;
			}

			base.HandleUnknownAction(actionName);
		}
		
	}
}
