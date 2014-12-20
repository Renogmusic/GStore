using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Identity
{
	/// <summary>
	/// Specific actions that are protected by permissions
	/// </summary>
	public enum GStoreAction
	{

		Admin_StoreAdminArea = 1000,

		ClientConfig_StoreAdminMenuItem = 2000,
		ClientConfig_Manager = 2100,
		ClientConfig_View = 2200,
		ClientConfig_Edit = 2300,
		ClientConfig_StoreFrontConfig_View = 2400,
		ClientConfig_StoreFrontConfig_Edit = 2500,

		Pages_StoreAdminMenuItem = 3000,
		Pages_Manager = 3010,
		Pages_View = 3020,
		Pages_Create = 3030,
		Pages_Edit = 3040,
		Pages_Delete = 3050,

		NavBarItems_StoreAdminMenuItem = 3200,
		NavBarItems_Manager = 3210,
		NavBarItems_View = 3220,
		NavBarItems_Create = 3230,
		NavBarItems_Edit = 3240,
		NavBarItems_Delete = 3250,

		WebForms_StoreAdminMenuItem = 3500,
		WebForms_Manager = 3510,
		WebForms_View = 3520,
		WebForms_Create = 3530,
		WebForms_Edit = 3540,
		WebForms_Delete = 3550,

//items below are not implemented yet //
		Catalog_StoreAdminMenuItem = 4000,
		Catalog_Manager = 4100,
		Catalog_Categories_Manager = 4200,
		Catalog_Categories_Images_Manager = 4300,
		Catalog_Products_Manager = 4400,
		Catalog_Products_Images_Manager = 4500,

		UserProfile_StoreAdminMenuItem = 5000,
		UserProfile_Manager = 5100,
		UserProfile_Account_CreateNew = 5200,
		UserProfile_Account_ResetPassword = 5300,
		UserProfile_Profile_View = 5400,
		UserProfile_Profile_Edit = 5450,
		UserProfile_Profile_Deactivate = 5500,
		UserProfile_Profile_Reactivate = 5550,
		UserProfile_Notifications_View = 5600,
		UserProfile_Orders_View = 5700,
		UserProfile_Roles_AssignUserRoles = 5800,

		ClientRole_StoreAdminMenuItem = 6000,
		ClientRole_Manager = 6100,
		ClientRole_Admin_Manager = 6200,
		ClientRole_Admin_Create = 6210,
		ClientRole_Admin_EditClaims = 6220,
		ClientRole_Admin_Delete = 6230,
		ClientRole_Admin_AssignUsers = 6240,
		ClientRole_User_Manager = 6300,
		ClientRole_User_Create = 6310,
		ClientRole_User_EditClaims = 6320,
		ClientRole_User_Delete = 6330,
		ClientRole_User_AssignUsers = 6340,

		ValueList_StoreAdminMenu = 7000,
		ValueList_Manager = 7100,
		ValueList_Add = 7210,
		ValueList_View = 7220,
		ValueList_Edit = 7230,
		ValueList_Delete = 7240,
		ValueList_AddValue = 7310,
		ValueList_ViewValue = 7320,
		ValueList_EditValue = 7330,
		ValueList_DeleteValue = 7340,

		Orders_StoreAdminMenuItem = 8000,
		Orders_Manager = 8100,
		Orders_Carts_ViewAbandoned = 8200,
		Orders_Carts_ViewCurrent = 8300,
		Orders_Carts_TransferAbandonedCart = 8400,
		Orders_Carts_SendCheckOutLink = 8500,

		Reports_StoreAdminMenuItem = 9000,
		Reports_Manager = 9100,
		Reports_MonthlySales_View = 9200,
		Reports_MonthlyPageViews_View = 9300,
		Reports_MonthlyVisitors_View = 9400,
		Reports_AbandonedCarts_View = 9500

	}
}