using GStore.Models;
using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	[NotMapped()]
	public class StoreFrontConfigAdminViewModel : StoreAdminViewModel
	{
		public StoreFrontConfigAdminViewModel() { }

		public StoreFrontConfigAdminViewModel(StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, string activeTab, bool isCreatePage, bool isDeletePage)
			: base(storeFrontConfig, userProfile)
		{
			this.ActiveTab = activeTab;
			this.IsCreatePage = isCreatePage;
			this.IsDeletePage = isDeletePage;
			this.StoreFrontId = storeFrontConfig.StoreFrontId;
			this.StoreFrontConfigurationId = storeFrontConfig.StoreFrontConfigurationId;
			this.AccountAdmin = storeFrontConfig.AccountAdmin;
			this.AccountAdmin_UserProfileId = storeFrontConfig.AccountAdmin_UserProfileId;
			this.AccountLayoutName = storeFrontConfig.AccountLayoutName;
			this.AccountTheme = storeFrontConfig.AccountTheme;
			this.AccountThemeId = storeFrontConfig.AccountThemeId;
			this.AccountLoginRegisterLinkText = storeFrontConfig.AccountLoginRegisterLinkText;
			this.AccountLoginShowRegisterLink = storeFrontConfig.AccountLoginShowRegisterLink;
			this.AdminLayoutName = storeFrontConfig.AdminLayoutName;
			this.AdminTheme = storeFrontConfig.AdminTheme;
			this.AdminThemeId = storeFrontConfig.AdminThemeId;
			this.CartLayoutName = storeFrontConfig.CartLayoutName;
			this.CartTheme = storeFrontConfig.CartTheme;
			this.CartThemeId = storeFrontConfig.CartThemeId;
			this.CheckoutLayoutName = storeFrontConfig.CheckoutLayoutName;
			this.CheckoutTheme = storeFrontConfig.CheckoutTheme;
			this.CheckoutThemeId = storeFrontConfig.CheckoutThemeId;

			this.CheckoutLogInOrGuestWebFormId = storeFrontConfig.CheckoutLogInOrGuestWebFormId;
			this.CheckoutLogInOrGuestWebForm = storeFrontConfig.CheckoutLogInOrGuestWebForm;
			this.CheckoutDeliveryInfoDigitalOnlyWebFormId = storeFrontConfig.CheckoutDeliveryInfoDigitalOnlyWebFormId;
			this.CheckoutDeliveryInfoDigitalOnlyWebForm = storeFrontConfig.CheckoutDeliveryInfoDigitalOnlyWebForm;
			this.CheckoutDeliveryInfoShippingWebFormId = storeFrontConfig.CheckoutDeliveryInfoShippingWebFormId;
			this.CheckoutDeliveryInfoShippingWebForm = storeFrontConfig.CheckoutDeliveryInfoShippingWebForm;
			this.CheckoutDeliveryMethodWebFormId = storeFrontConfig.CheckoutDeliveryMethodWebFormId;
			this.CheckoutDeliveryMethodWebForm = storeFrontConfig.CheckoutDeliveryMethodWebForm;
			this.CheckoutPaymentInfoWebFormId = storeFrontConfig.CheckoutPaymentInfoWebFormId;
			this.CheckoutPaymentInfoWebForm = storeFrontConfig.CheckoutPaymentInfoWebForm;
			this.CheckoutConfirmOrderWebFormId = storeFrontConfig.CheckoutConfirmOrderWebFormId;
			this.CheckoutConfirmOrderWebForm = storeFrontConfig.CheckoutConfirmOrderWebForm;

			this.CatalogCategoryColLg = storeFrontConfig.CatalogCategoryColLg;
			this.CatalogCategoryColMd = storeFrontConfig.CatalogCategoryColMd;
			this.CatalogCategoryColSm = storeFrontConfig.CatalogCategoryColSm;
			this.CatalogLayoutName = storeFrontConfig.CatalogLayoutName;
			this.CatalogTheme = storeFrontConfig.CatalogTheme;
			this.CatalogThemeId = storeFrontConfig.CatalogThemeId;
			this.CatalogPageInitialLevels = storeFrontConfig.CatalogPageInitialLevels;
			this.CatalogProductColLg = storeFrontConfig.CatalogProductColLg;
			this.CatalogProductColMd = storeFrontConfig.CatalogProductColMd;
			this.CatalogProductColSm = storeFrontConfig.CatalogProductColSm;
			this.CatalogAdminLayoutName = storeFrontConfig.CatalogAdminLayoutName;
			this.CatalogAdminThemeId = storeFrontConfig.CatalogAdminThemeId;
			this.CatalogAdminTheme = storeFrontConfig.CatalogAdminTheme;
			this.DefaultNewPageLayoutName = storeFrontConfig.DefaultNewPageLayoutName;
			this.DefaultNewPageTheme = storeFrontConfig.DefaultNewPageTheme;
			this.DefaultNewPageThemeId = storeFrontConfig.DefaultNewPageThemeId;
			this.Folder = storeFrontConfig.Folder;
			this.EnableGoogleAnalytics = storeFrontConfig.EnableGoogleAnalytics;
			this.GoogleAnalyticsWebPropertyId = storeFrontConfig.GoogleAnalyticsWebPropertyId;
			this.HtmlFooter = storeFrontConfig.HtmlFooter;
			this.MetaApplicationName = storeFrontConfig.MetaApplicationName;
			this.MetaApplicationTileColor = storeFrontConfig.MetaApplicationTileColor;
			this.MetaDescription = storeFrontConfig.MetaDescription;
			this.MetaKeywords = storeFrontConfig.MetaKeywords;
			this.Order = storeFrontConfig.Order;
			this.OrderAdminLayoutName = storeFrontConfig.OrderAdminLayoutName;
			this.OrderAdminThemeId = storeFrontConfig.OrderAdminThemeId;
			this.OrderAdminTheme = storeFrontConfig.OrderAdminTheme;
			this.OrdersLayoutName = storeFrontConfig.OrdersLayoutName;
			this.OrdersThemeId = storeFrontConfig.OrdersThemeId;
			this.OrdersTheme = storeFrontConfig.OrdersTheme;
			this.BodyTopScriptTag = storeFrontConfig.BodyTopScriptTag;
			this.BodyBottomScriptTag = storeFrontConfig.BodyBottomScriptTag;
			this.Name = storeFrontConfig.Name;
			this.NavBarCatalogMaxLevels = storeFrontConfig.NavBarCatalogMaxLevels;
			this.NavBarItemsMaxLevels = storeFrontConfig.NavBarItemsMaxLevels;
			this.NavBarRegisterLinkText = storeFrontConfig.NavBarRegisterLinkText;
			this.NavBarShowRegisterLink = storeFrontConfig.NavBarShowRegisterLink;
			this.NotFoundErrorPage = storeFrontConfig.NotFoundErrorPage;
			this.NotFoundError_PageId = storeFrontConfig.NotFoundError_PageId;
			this.NotificationsLayoutName = storeFrontConfig.NotificationsLayoutName;
			this.NotificationsTheme = storeFrontConfig.NotificationsTheme;
			this.NotificationsThemeId = storeFrontConfig.NotificationsThemeId;
			this.ProfileLayoutName = storeFrontConfig.ProfileLayoutName;
			this.ProfileTheme = storeFrontConfig.ProfileTheme;
			this.ProfileThemeId = storeFrontConfig.ProfileThemeId;
			this.PublicUrl = storeFrontConfig.PublicUrl;
			this.RegisteredNotify = storeFrontConfig.RegisteredNotify;
			this.RegisteredNotify_UserProfileId = storeFrontConfig.RegisteredNotify_UserProfileId;
			this.RegisterSuccessPage = storeFrontConfig.RegisterSuccessPage;
			this.RegisterSuccess_PageId = storeFrontConfig.RegisterSuccess_PageId;
			this.RegisterWebForm = storeFrontConfig.RegisterWebForm;
			this.Register_WebFormId = storeFrontConfig.Register_WebFormId;
			this.StoreErrorPage = storeFrontConfig.StoreErrorPage;
			this.StoreError_PageId = storeFrontConfig.StoreError_PageId;
			this.UseShoppingCart = storeFrontConfig.UseShoppingCart;
			this.CartNavShowCartWhenEmpty = storeFrontConfig.CartNavShowCartWhenEmpty;
			this.CartNavShowCartToAnonymous = storeFrontConfig.CartNavShowCartToAnonymous;
			this.CartNavShowCartToRegistered = storeFrontConfig.CartNavShowCartToRegistered;
			this.CartRequireLogin = storeFrontConfig.CartRequireLogin;

			this.WelcomePerson = storeFrontConfig.WelcomePerson;
			this.WelcomePerson_UserProfileId = storeFrontConfig.WelcomePerson_UserProfileId;

			this.ConfigurationName = storeFrontConfig.ConfigurationName;
			this.IsPending = storeFrontConfig.IsPending;
			this.EndDateTimeUtc = storeFrontConfig.EndDateTimeUtc;
			this.StartDateTimeUtc = storeFrontConfig.StartDateTimeUtc;

			this.ConfigIsActiveDirect = storeFrontConfig.IsActiveDirect();
		}

		public string ActiveTab { get; set; }

		public bool IsCreatePage { get; set; }

		public bool IsDeletePage { get; set; }

		[Display(Name = "Status", Description = "Status of this Configuration")]
		public bool ConfigIsActiveDirect { get; set; }

		[Key]
		[Required]
		[Display(Name = "Store Front Configuration Id", Description = "Internal Store Front Configuration Id Number")]
		public int StoreFrontConfigurationId { get; set; }

		[Required]
		[Display(Name = "Store Front Id", Description = "Internal Store Front Id Number")]
		public int StoreFrontId { get; set; }

		[Display(Name = "Account Admin", Description = "Profile to use for sending Account Login notices such as Locked Out notification and Password Changed notification")]
		public UserProfile AccountAdmin { get; protected set; }

		[Required]
		[Display(Name = "Account Admin", Description = "Profile to use for sending Account Login notices such as Locked Out notification and Password Changed notification")]
		public int AccountAdmin_UserProfileId { get; set; }

		[Required]
		[Display(Name = "Account (register and login) Layout", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string AccountLayoutName { get; set; }

		[Display(Name = "Account Register/Login Theme", Description = "Choose a Theme for the Account Register/Login section of the site")]
		public Theme AccountTheme { get; protected set; }

		[Required]
		[Display(Name = "Account (register and login) Theme", Description = "Choose a Theme for the Account Register/Login section of the site")]
		public int AccountThemeId { get; set; }

		[Display(Name = "Account Login Show Register Link", Description = "Check this box to show the register/signup link on the login page. Uncheck to NOT show it")]
		public bool AccountLoginShowRegisterLink { get; set; }

		[Required]
		[Display(Name = "Account Login Register Link Text", Description = "Text label for the register/signup link on the login page.")]
		[MaxLength(50)]
		public string AccountLoginRegisterLinkText { get; set; }

		[Required]
		[Display(Name = "Store Admin Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string AdminLayoutName { get; set; }

		[Display(Name = "Store Admin Theme", Description = "Choose a Theme for the Store Admin section of the site")]
		public Theme AdminTheme { get; protected set; }

		[Required]
		[Display(Name = "Store Admin Theme", Description = "Choose a Theme for the Store Admin section of the site")]
		public int AdminThemeId { get; set; }

		[Required]
		[Display(Name = "Shopping Cart Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string CartLayoutName { get; set; }

		[Display(Name = "Shopping Cart Theme", Description = "Choose a Theme for the Shopping Cart section of the site")]
		public Theme CartTheme { get; protected set; }

		[Required]
		[Display(Name = "Shopping Cart Theme", Description = "Choose a Theme for the Shopping Cart section of the site")]
		public int CartThemeId { get; set; }

		[Required]
		[Display(Name = "Checkout Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string CheckoutLayoutName { get; set; }

		[Display(Name = "Checkout Theme", Description = "Choose a Theme for the Checkout section of the site")]
		public Theme CheckoutTheme { get; protected set; }

		[Required]
		[Display(Name = "Checkout Theme", Description = "Choose a Theme for the Checkout section of the site")]
		public int CheckoutThemeId { get; set; }

		[Display(Name = "Checkout Log In or Guest Web Form", Description = "Custom fields for the checkout Log in or Guest page")]
		public int? CheckoutLogInOrGuestWebFormId { get; set; }

		[Display(Name = "Checkout Log In or Guest Web Form", Description = "Custom fields for the checkout Log in or Guest page")]
		public WebForm CheckoutLogInOrGuestWebForm { get; protected set; }

		[Display(Name = "Checkout Delivery Info Digital Only Web Form", Description = "Custom fields for the checkout Delivery Info Digital Only page")]
		public int? CheckoutDeliveryInfoDigitalOnlyWebFormId { get; set; }

		[Display(Name = "Checkout Delivery Info Digital Only Web Form", Description = "Custom fields for the checkout Delivery Info Digital Only page")]
		public WebForm CheckoutDeliveryInfoDigitalOnlyWebForm { get; protected set; }

		[Display(Name = "Checkout Delivery Info Shipping Web Form", Description = "Custom fields for the checkout Delivery Info Shipping page")]
		public int? CheckoutDeliveryInfoShippingWebFormId { get; set; }

		[Display(Name = "Checkout Delivery Info Shipping Web Form", Description = "Custom fields for the checkout Delivery Info Shipping page")]
		public WebForm CheckoutDeliveryInfoShippingWebForm { get; protected set; }

		[Display(Name = "Checkout Delivery Method Web Form", Description = "Custom fields for the checkout Delivery Method page")]
		public int? CheckoutDeliveryMethodWebFormId { get; set; }

		[Display(Name = "Checkout Delivery Method Web Form", Description = "Custom fields for the checkout Delivery Method page")]
		public WebForm CheckoutDeliveryMethodWebForm { get; protected set; }

		[Display(Name = "Checkout Payment Info Web Form", Description = "Custom fields for the checkout Payment Info page")]
		public int? CheckoutPaymentInfoWebFormId { get; set; }

		[Display(Name = "Checkout Payment Info Web Form", Description = "Custom fields for the checkout Payment Info page")]
		public WebForm CheckoutPaymentInfoWebForm { get; protected set; }

		[Display(Name = "Checkout Confirm Order Web Form", Description = "Custom fields for the checkout Confirm Order page")]
		public int? CheckoutConfirmOrderWebFormId { get; set; }

		[Display(Name = "Checkout Confirm Order Web Form", Description = "Custom fields for the checkout Confirm Order page")]
		public WebForm CheckoutConfirmOrderWebForm { get; protected set; }

		[Required]
		[Display(Name = "Catalog Admin Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string CatalogAdminLayoutName { get; set; }

		[Display(Name = "Catalog Admin Theme", Description = "Choose a Theme for the Catalog Admin section of the site")]
		public Theme CatalogAdminTheme { get; protected set; }

		[Required]
		[Display(Name = "Catalog Admin Theme", Description = "Choose a Theme for the Catalog Admin section of the site")]
		public int CatalogAdminThemeId { get; set; }

		[Required]
		[Display(Name = "Order Status Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string OrdersLayoutName { get; set; }

		[Display(Name = "Order Status Theme", Description = "Choose a Theme for the Order Status section of the site")]
		public Theme OrdersTheme { get; protected set; }

		[Required]
		[Display(Name = "Order Status Theme", Description = "Choose a Theme for the Order Status section of the site")]
		public int OrdersThemeId { get; set; }

		[Required]
		[Display(Name = "Order Admin Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string OrderAdminLayoutName { get; set; }

		[Display(Name = "Order Admin Theme", Description = "Choose a Theme for the Order Admin section of the site")]
		public Theme OrderAdminTheme { get; protected set; }

		[Required]
		[Display(Name = "Order Admin Theme", Description = "Choose a Theme for the Order Admin section of the site")]
		public int OrderAdminThemeId { get; set; }

		[Required]
		[Display(Name = "Catalog Category Column Span Large", Description = "Number of columns (12 max) to span each Category on the catalog page for Large displays (desktop)")]
		[Range(1, 12)]
		public int CatalogCategoryColLg { get; set; }

		[Required]
		[Display(Name = "Catalog Category Column Span Medium", Description = "Number of columns (12 max) to span each Category on the catalog page for Medium displays (tablet)")]
		[Range(1, 12)]
		public int CatalogCategoryColMd { get; set; }

		[Required]
		[Display(Name = "Catalog Category Column Span Small", Description = "Number of columns (12 max) to span each Category on the catalog page for Small displays (phone)")]
		[Range(1, 12)]
		public int CatalogCategoryColSm { get; set; }

		[Required]
		[Display(Name = "Catalog Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string CatalogLayoutName { get; set; }

		[Display(Name = "Catalog Theme", Description = "Choose a Theme for the Catalog section of the site with products and categories")]
		public Theme CatalogTheme { get; protected set; }

		[Required]
		[Display(Name = "Catalog Theme", Description = "Choose a Theme for the Catalog section of the site with products and categories")]
		public int CatalogThemeId { get; set; }

		[Required]
		[Display(Name = "Catalog Page Initial Levels", Description = "Enter the number of levels to automatically drill down on the catalog page, max 6")]
		[Range(1, 6)]
		public int CatalogPageInitialLevels { get; set; }

		[Required]
		[Display(Name = "Catalog Product Column Span Large", Description = "Number of columns (12 max) to span each Product on the catalog page for Large displays (desktop)")]
		[Range(1, 12)]
		public int CatalogProductColLg { get; set; }

		[Required]
		[Display(Name = "Catalog Product Column Span Medium", Description = "Number of columns (12 max) to span each Product on the catalog page for Medium displays (tablet)")]
		[Range(1, 12)]
		public int CatalogProductColMd { get; set; }

		[Required]
		[Display(Name = "Catalog Product Column Span Small", Description = "Number of columns (12 max) to span each Product on the catalog page for Small displays (phone)")]
		[Range(1, 12)]
		public int CatalogProductColSm { get; set; }

		[Required]
		[Display(Name = "Default New Page Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string DefaultNewPageLayoutName { get; set; }

		[Display(Name = "Default New Page Theme", Description = "Choose a Theme as the default for new pages")]
		public Theme DefaultNewPageTheme { get; protected set; }

		[Required]
		[Display(Name = "Default New Page Theme", Description = "Choose a Theme as the default for new pages")]
		public int DefaultNewPageThemeId { get; set; }

		[Display(Name = "Enable Google Analytics", Description = "Check this box to use Google Analytics tracking\nBe sure to enter the Google analytics WebProperty Id as well")]
		public bool EnableGoogleAnalytics { get; set; }

		[Required]
		[Display(Name = "Folder", Description = "Info: Web Server Folder for this store front")]
		public string Folder { get; set; }

		[Required]
		[Display(Name = "Configuration Name", Description = "Internal Name of this store front configuration")]
		public string ConfigurationName { get; set; }

		[Display(Name = "Google Analytics Web Property Id", Description = "Google Analytics Web Property Id for tracking\nExample: UA-123456-1")]
		[MaxLength(50)]
		public string GoogleAnalyticsWebPropertyId { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Html Footer", Description = "Footer HTML shown in the middle footer section of all pages on the web site.\nBe sure HTML tags are balanced for proper display")]
		[MaxLength(250)]
		public string HtmlFooter { get; set; }

		[Display(Name = "Meta Application Name", Description = "Application Name for Pinned Site in Windows 7 and Windows 8 Tiles\nExample: Your Store Name\nLeave blank to use system default.")]
		[MaxLength(250)]
		public string MetaApplicationName { get; set; }

		[Display(Name = "Meta Application Tile Color", Description = "Application Tile Color for Pinned Site\nExample: #880088\nLeave blank to use system default.")]
		[MaxLength(25)]
		public string MetaApplicationTileColor { get; set; }

		[Display(Name = "Meta Description", Description = "Meta Description tag for search engines\nExample: the best site for online store builders.\nLeave blank to use system default.")]
		[MaxLength(1000)]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Keywords", Description = "Meta Keywords tag for search engines each keyword separated by a space\nExample: food carrots crafts arts\nLeave blank to use system default.")]
		[MaxLength(1000)]
		public string MetaKeywords { get; set; }

		[AllowHtml]
		[Display(Name = "Page Script on Top of Pages", Description = "Advanced: Script tags or HTML tags shown on the Top of pages for search engines.\nLeave blank if you are not sure what to do here.")]
		public string BodyTopScriptTag { get; set; }

		[AllowHtml]
		[Display(Name = "Page Script on Bottom of Pages", Description = "Advanced: Script tags or HTML tags shown on the bottom of pages for search engines.\nLeave blank if you are unsure what to do here.")]
		public string BodyBottomScriptTag { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Name", Description = "Name of Store Front - this is added to the Page title of all pages on the web site.")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Site Menu Catalog Max Levels", Description = "Enter the number of Category levels to expand on the Site Top menu. 0 for none, up to 6.")]
		[Range(0, 6)]
		public int NavBarCatalogMaxLevels { get; set; }

		[Required]
		[Display(Name = "Site Menu Max Levels", Description = "Enter the maximum number of levels to expand on the Site Menu. Does not include Products.\nExample: 6")]
		[Range(1, 6)]
		public int NavBarItemsMaxLevels { get; set; }

		[Display(Name = "Nav Bar Show Register Link", Description = "Check this box to show the Register/Sign-Up link on the Site Menu (Nav Bar). Uncheck to NOT show it")]
		public bool NavBarShowRegisterLink { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "Nav Bar Register Link Text", Description = "Enter a label for the Register/Sign-Up link on the Site Menu (Nav Bar)\nExample: Register")]
		public string NavBarRegisterLinkText { get; set; }

		[Required]
		[Display(Name = "Index", Description = "Use this to give priority to this configuration. Lower number = higher priority. \nExample: 100")]
		public int Order { get; set; }

		[Display(Name = "Register Web Form", Description = "Register Form for new users signing up or use the system default registration form")]
		public WebForm RegisterWebForm { get; protected set; }

		[Display(Name = "Register Web Form", Description = "Register Form for new users signing up or use the system default registration form")]
		public int? Register_WebFormId { get; set; }

		[Display(Name = "Not Found Error Page", Description = "Choose a page to display for File Not Found (404) errors or use the system default not found page")]
		public Page NotFoundErrorPage { get; protected set; }

		[Display(Name = "Not Found Error Page", Description = "Choose a page to display for File Not Found (404) errors or use the system default not found page")]
		public int? NotFoundError_PageId { get; set; }

		[Required]
		[Display(Name = "Notifications Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string NotificationsLayoutName { get; set; }

		[Display(Name = "Notifications (messaging) Theme", Description = "Choose a Theme for the Notifications/Messaging section of the site")]
		public Theme NotificationsTheme { get; protected set; }

		[Required]
		[Display(Name = "Notifications Theme", Description = "Choose a Theme for the Notifications/Messaging section of the site")]
		public int NotificationsThemeId { get; set; }

		[Required]
		[Display(Name = "Profile Layout Name", Description = "Always 'Default'")]
		[MaxLength(10)]
		public string ProfileLayoutName { get; set; }

		[Display(Name = "Profile Theme", Description = "Choose a Theme for the User Profile section of the site")]
		public Theme ProfileTheme { get; protected set; }

		[Required]
		[Display(Name = "Profile Theme", Description = "Choose a Theme for the User Profile section of the site")]
		public int ProfileThemeId { get; set; }

		[Required]
		[Display(Name = "Public Url", Description = "Enter the full URL to your site\nExample: http://www.gstore.renog.info/stores/gstore")]
		[MaxLength(200)]
		public string PublicUrl { get; set; }

		[Display(Name = "Registered Notify", Description = "This profile will receive a notification when a new user signs up")]
		public UserProfile RegisteredNotify { get; protected set; }

		[Required]
		[Display(Name = "Registered Notify", Description = "This profile will receive a notification when a new user signs up")]
		public int RegisteredNotify_UserProfileId { get; set; }

		[Display(Name = "Register Success Page", Description = "Choose a page to display when a user signs up successfully, or or use the system default register success page")]
		public Page RegisterSuccessPage { get; protected set; }

		[Display(Name = "Register Success Page", Description = "Choose a page to display when a user signs up successfully, or or use the system default register success page")]
		public int? RegisterSuccess_PageId { get; set; }

		[Display(Name = "Store Error Page", Description = "Choose a page to display for web site errors or use the system default error page")]
		public Page StoreErrorPage { get; protected set; }

		[Display(Name = "Store Error Page", Description = "Choose a page to display for web site errors or use the system default error page")]
		public int? StoreError_PageId { get; set; }

		[Display(Name = "Use Shopping Cart", Description = "Check this box to use the built-in shopping cart. If unchecked, items can be ordered one at a time.\nDefault: checked.")]
		public bool UseShoppingCart { get; set; }

		[Display(Name = "Show Shopping Cart to Anonymous users", Description = "Check this box to show the cart link in the top menu for anonymous users.\nDefault: checked")]
		public bool CartNavShowCartToAnonymous { get; set; }

		[Display(Name = "Show Shopping Cart to Registered users", Description = "Check this box to show the cart link in the top menu for Registered users.\nDefault: checked")]
		public bool CartNavShowCartToRegistered { get; set; }

		[Display(Name = "Show Shopping Cart event when Empty", Description = "Check this box to show the cart link in the top menu even when the cart is empty.\nDefault: checked")]
		public bool CartNavShowCartWhenEmpty { get; set; }

		[Display(Name = "Require Login to Add to Cart", Description = "Check this box to require the user to log in when adding to cart.\nDefault: Unchecked")]
		public bool CartRequireLogin { get; set; }

		[Display(Name = "Welcome Person", Description = "This profile will be used to automatically send a welcome message to newly registered users for their first login")]
		public UserProfile WelcomePerson { get; protected set; }

		[Required]
		[Display(Name = "Welcome Person", Description = "This profile will be used to automatically send a welcome message to newly registered users for their first login")]
		public int WelcomePerson_UserProfileId { get; set; }

		[Required]
		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this configuration to go ACTIVE on. \nIf this date is in the future, your configuration will be inactive until the start date.\nExample: 1/1/2000 12:00 AM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Required]
		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this configuration to go INACTIVE on. \nIf this date is in the past, your configuration will be inactive immediately.\nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to Inactivate this configuration immediately. \nIf checked, make sure you have another active configuration. or your site will be inactive and you can only access the store admin section.")]
		public bool IsPending { get; set; }


	}
}