using GStore.Areas.OrderAdmin.ViewModels;
using GStore.Data;

namespace GStore.Areas.OrderAdmin.Controllers.BaseClasses
{
	[Identity.AuthorizeGStoreAction(Identity.GStoreAction.Admin_OrderAdminArea)]
	public class OrderAdminBaseController : GStore.Controllers.BaseClass.BaseController
	{
		public OrderAdminBaseController(IGstoreDb dbContext): base(dbContext)
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		public OrderAdminBaseController()
		{
			this._logActionsAsPageViews = false;
			this._throwErrorIfUserProfileNotFound = true;
			this._useInactiveStoreFrontAsActive = false;
			this._useInactiveStoreFrontConfigAsActive = true;
		}

		protected override string LayoutName
		{
			get
			{
				return CurrentStoreFrontConfigOrAny.OrderAdminLayoutName;
			}
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrAny.OrderAdminTheme.FolderName;
			}
		}

		public OrderAdminViewModel OrderAdminViewModel
		{
			get
			{
				return new OrderAdminViewModel(CurrentStoreFrontConfigOrAny, CurrentUserProfileOrThrow);
			}
		}


	}
}