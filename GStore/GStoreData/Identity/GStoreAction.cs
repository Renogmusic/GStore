
namespace GStoreData.Identity
{
	/// <summary>
	/// Specific actions that are protected by permissions
	/// </summary>
	public enum GStoreAction
	{

		Admin_StoreAdminArea = 10000,
		Admin_CatalogAdminArea = 20000,
		Admin_OrderAdminArea = 30000,

		#region StoreAdmin Actions

		ClientConfig_StoreAdminMenuItem = 11000,
		ClientConfig_Manager = 11110,
		ClientConfig_View = 11120,
		ClientConfig_Edit = 11130,
		ClientConfig_StoreFrontConfig_View = 11140,
		ClientConfig_StoreFrontConfig_Edit = 11150,
		ClientConfig_StoreFrontConfig_Create = 11160,
		ClientConfig_StoreFrontConfig_Delete = 11170,
		ClientConfig_StoreFrontConfig_Cart_View = 11180,
		ClientConfig_StoreFrontConfig_Cart_Edit = 11181,
		ClientConfig_StoreFrontConfig_Cart_Preview = 11182,

		Pages_StoreAdminMenuItem = 12000,
		Pages_Manager = 12110,
		Pages_View = 12120,
		Pages_Create = 12130,
		Pages_Edit = 12140,
		Pages_Delete = 12150,

		NavBarItems_StoreAdminMenuItem = 13000,
		NavBarItems_Manager = 13110,
		NavBarItems_View = 13120,
		NavBarItems_Create = 13130,
		NavBarItems_Edit = 13140,
		NavBarItems_Delete = 13150,

		WebForms_StoreAdminMenuItem = 14000,
		WebForms_Manager = 14110,
		WebForms_View = 14120,
		WebForms_Create = 14130,
		WebForms_Edit = 14150,
		WebForms_Delete = 14160,

		ValueLists_StoreAdminMenuItem = 15000,
		ValueLists_Manager = 15110,
		ValueLists_View = 15120,
		ValueLists_Create = 15130,
		ValueLists_Edit = 15140,
		ValueLists_Delete = 15150,

		#region Store Admin Actions Not Yet Implemented

		UserProfile_StoreAdminMenuItem = 16000,
		UserProfile_Manager = 16110,
		UserProfile_Account_CreateNew = 16120,
		UserProfile_Account_ResetPassword = 16130,
		UserProfile_Profile_View = 16140,
		UserProfile_Profile_Edit = 16150,
		UserProfile_Profile_Deactivate = 16160,
		UserProfile_Profile_Reactivate = 16170,
		UserProfile_Notifications_View = 16180,
		UserProfile_Orders_View = 16190,
		UserProfile_Roles_AssignUserRoles = 16200,

		ClientRole_StoreAdminMenuItem = 17000,
		ClientRole_Manager = 17110,
		ClientRole_Admin_Manager = 17120,
		ClientRole_Admin_Create = 17130,
		ClientRole_Admin_EditClaims = 17140,
		ClientRole_Admin_Delete = 17150,
		ClientRole_Admin_AssignUsers = 17160,
		ClientRole_User_Manager = 17170,
		ClientRole_User_Create = 17180,
		ClientRole_User_EditClaims = 17190,
		ClientRole_User_Delete = 17200,
		ClientRole_User_AssignUsers = 17210,

		Reports_StoreAdminMenuItem = 18000,
		Reports_Manager = 18110,
		Reports_MonthlySales_View = 18120,
		Reports_MonthlyPageViews_View = 18130,
		Reports_MonthlyVisitors_View = 18140,
		Reports_AbandonedCarts_View = 18150,

		#endregion

		#endregion

		#region Orders Admin Actions

		Orders_OrderAdminMenuItem = 31000,
		Orders_Manager = 31110,

		Carts_OrderAdminMenuItem = 32000,
		Carts_Manager = 32110,
		Carts_ViewAbandoned = 32120,
		Carts_ViewCurrent = 32130,
		Carts_TransferAbandonedCart = 32140,
		Carts_SendCheckOutLink = 32150,

		Checkout_SendReceiptCopy = 32500,

		Discounts_OrderAdminMenuItem = 33000,
		Discounts_Manager = 33110,
		Discounts_View = 33120,
		Discounts_Create = 33130,
		Discounts_Edit = 33140,
		Discounts_Delete = 33150,

		GiftCards_OrderAdminMenuItem = 34000,
		GiftCards_Manager = 34110,
		GiftCards_View = 34120,
		GiftCards_Create = 34130,
		GiftCards_Edit = 34140,
		GiftCards_Delete = 34150,
		GiftCards_Reload = 34160,

		#endregion

		#region Catalog Admin Actions

		Categories_CatalogAdminMenuItem = 21000,
		Categories_Manager = 21110,
		Categories_Create = 21120,
		Categories_View = 21130,
		Categories_Edit = 21140,
		Categories_Delete = 21150,
		Categories_SyncImages = 21160,

		Products_CatalogAdminMenuItem = 22000,
		Products_Manager = 22110,
		Products_Create = 22120,
		Products_View = 22130,
		Products_Edit = 22140,
		Products_Delete = 22150,
		Products_SyncFiles = 22160,

		Bundles_CatalogAdminMenuItem = 23000,
		Bundles_Manager = 23110,
		Bundles_Create = 23120,
		Bundles_View = 23130,
		Bundles_Edit = 23140,
		Bundles_Delete = 23150,
		Bundles_SyncFiles = 23160
	
		#endregion

	}
}