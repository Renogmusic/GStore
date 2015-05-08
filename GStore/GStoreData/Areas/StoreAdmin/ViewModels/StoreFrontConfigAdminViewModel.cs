using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreData.Areas.StoreAdmin.ViewModels
{
	[NotMapped()]
	public class StoreFrontConfigAdminViewModel : StoreAdminViewModel, IValidatableObject
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
			this.AccountTheme = storeFrontConfig.AccountTheme;
			this.AccountThemeId = storeFrontConfig.AccountThemeId;
			this.AccountLoginRegisterLinkText = storeFrontConfig.AccountLoginRegisterLinkText;
			this.AccountLoginShowRegisterLink = storeFrontConfig.AccountLoginShowRegisterLink;
			this.AdminTheme = storeFrontConfig.AdminTheme;
			this.AdminThemeId = storeFrontConfig.AdminThemeId;
			this.CartTheme = storeFrontConfig.CartTheme;
			this.CartThemeId = storeFrontConfig.CartThemeId;
			this.CheckoutTheme = storeFrontConfig.CheckoutTheme;
			this.CheckoutThemeId = storeFrontConfig.CheckoutThemeId;
			this.CheckoutOrderMinimum = storeFrontConfig.CheckoutOrderMinimum;

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

			this.Orders_AutoAcceptPaid = storeFrontConfig.Orders_AutoAcceptPaid;
			this.PaymentMethod_PayPal_Enabled = storeFrontConfig.PaymentMethod_PayPal_Enabled;
			this.PaymentMethod_PayPal_UseLiveServer = storeFrontConfig.PaymentMethod_PayPal_UseLiveServer;
			this.PaymentMethod_PayPal_Client_Id = storeFrontConfig.PaymentMethod_PayPal_Client_Id;
			this.PaymentMethod_PayPal_Client_Secret = storeFrontConfig.PaymentMethod_PayPal_Client_Secret;

			this.CatalogCategoryColLg = storeFrontConfig.CatalogCategoryColLg;
			this.CatalogCategoryColMd = storeFrontConfig.CatalogCategoryColMd;
			this.CatalogCategoryColSm = storeFrontConfig.CatalogCategoryColSm;
			this.CatalogTheme = storeFrontConfig.CatalogTheme;
			this.CatalogThemeId = storeFrontConfig.CatalogThemeId;
			this.CatalogPageInitialLevels = storeFrontConfig.CatalogPageInitialLevels;
			this.CatalogProductColLg = storeFrontConfig.CatalogProductColLg;
			this.CatalogProductColMd = storeFrontConfig.CatalogProductColMd;
			this.CatalogProductColSm = storeFrontConfig.CatalogProductColSm;
			this.CatalogProductBundleColLg = storeFrontConfig.CatalogProductBundleColLg;
			this.CatalogProductBundleColMd = storeFrontConfig.CatalogProductBundleColMd;
			this.CatalogProductBundleColSm = storeFrontConfig.CatalogProductBundleColSm;
			this.CatalogProductBundleItemColLg = storeFrontConfig.CatalogProductBundleItemColLg;
			this.CatalogProductBundleItemColMd = storeFrontConfig.CatalogProductBundleItemColMd;
			this.CatalogProductBundleItemColSm = storeFrontConfig.CatalogProductBundleItemColSm;

			this.BlogTheme = storeFrontConfig.BlogTheme;
			this.BlogThemeId = storeFrontConfig.BlogThemeId;
			this.BlogAdminTheme = storeFrontConfig.BlogAdminTheme;
			this.BlogAdminThemeId = storeFrontConfig.BlogAdminThemeId;

			this.ChatTheme = storeFrontConfig.ChatTheme;
			this.ChatThemeId = storeFrontConfig.ChatThemeId;
			this.ChatEnabled = storeFrontConfig.ChatEnabled;
			this.ChatRequireLogin = storeFrontConfig.ChatRequireLogin;

			this.CatalogAdminThemeId = storeFrontConfig.CatalogAdminThemeId;
			this.CatalogAdminTheme = storeFrontConfig.CatalogAdminTheme;
			this.DefaultNewPageTheme = storeFrontConfig.DefaultNewPageTheme;
			this.DefaultNewPageThemeId = storeFrontConfig.DefaultNewPageThemeId;
			this.Folder = storeFrontConfig.Folder;
			this.EnableGoogleAnalytics = storeFrontConfig.EnableGoogleAnalytics;
			this.GoogleAnalyticsWebPropertyId = storeFrontConfig.GoogleAnalyticsWebPropertyId;
			this.HtmlFooter = storeFrontConfig.HtmlFooter;
			this.HomePageUseCatalog = storeFrontConfig.HomePageUseCatalog;
			this.HomePageUseBlog = storeFrontConfig.HomePageUseBlog;

			this.ShowBlogInMenu = storeFrontConfig.ShowBlogInMenu;
			this.ShowAboutGStoreMenu = storeFrontConfig.ShowAboutGStoreMenu;
			this.MetaApplicationName = storeFrontConfig.MetaApplicationName;
			this.MetaApplicationTileColor = storeFrontConfig.MetaApplicationTileColor;
			this.MetaDescription = storeFrontConfig.MetaDescription;
			this.MetaKeywords = storeFrontConfig.MetaKeywords;
			this.Order = storeFrontConfig.Order;
			this.OrderAdminThemeId = storeFrontConfig.OrderAdminThemeId;
			this.OrderAdminTheme = storeFrontConfig.OrderAdminTheme;
			this.OrdersThemeId = storeFrontConfig.OrdersThemeId;
			this.OrdersTheme = storeFrontConfig.OrdersTheme;
			this.BodyTopScriptTag = storeFrontConfig.BodyTopScriptTag;
			this.BodyBottomScriptTag = storeFrontConfig.BodyBottomScriptTag;
			this.Name = storeFrontConfig.Name;
			this.TimeZoneId = storeFrontConfig.TimeZoneId;
			this.CatalogTitle = storeFrontConfig.CatalogTitle;

			this.CatalogLayout = storeFrontConfig.CatalogLayout;
			this.CatalogHeaderHtml = storeFrontConfig.CatalogHeaderHtml;
			this.CatalogFooterHtml = storeFrontConfig.CatalogFooterHtml;
			this.CatalogRootListTemplate = storeFrontConfig.CatalogRootListTemplate;
			this.CatalogRootHeaderHtml = storeFrontConfig.CatalogRootHeaderHtml;
			this.CatalogRootFooterHtml = storeFrontConfig.CatalogRootFooterHtml;

			this.CatalogDefaultBottomDescriptionCaption = storeFrontConfig.CatalogDefaultBottomDescriptionCaption;
			this.CatalogDefaultNoProductsMessageHtml = storeFrontConfig.CatalogDefaultNoProductsMessageHtml;
			this.CatalogDefaultProductBundleTypePlural = storeFrontConfig.CatalogDefaultProductBundleTypePlural;
			this.CatalogDefaultProductBundleTypeSingle = storeFrontConfig.CatalogDefaultProductBundleTypeSingle;
			this.CatalogDefaultProductTypePlural = storeFrontConfig.CatalogDefaultProductTypePlural;
			this.CatalogDefaultProductTypeSingle = storeFrontConfig.CatalogDefaultProductTypeSingle;
			this.CatalogDefaultSampleAudioCaption = storeFrontConfig.CatalogDefaultSampleAudioCaption;
			this.CatalogDefaultSampleDownloadCaption = storeFrontConfig.CatalogDefaultSampleDownloadCaption;
			this.CatalogDefaultSampleImageCaption = storeFrontConfig.CatalogDefaultSampleImageCaption;
			this.CatalogDefaultSummaryCaption = storeFrontConfig.CatalogDefaultSummaryCaption;
			this.CatalogDefaultTopDescriptionCaption = storeFrontConfig.CatalogDefaultTopDescriptionCaption;

			this.NavBarCatalogMaxLevels = storeFrontConfig.NavBarCatalogMaxLevels;
			this.NavBarItemsMaxLevels = storeFrontConfig.NavBarItemsMaxLevels;
			this.NavBarRegisterLinkText = storeFrontConfig.NavBarRegisterLinkText;
			this.NavBarShowRegisterLink = storeFrontConfig.NavBarShowRegisterLink;
			this.NotFoundErrorPage = storeFrontConfig.NotFoundErrorPage;
			this.NotFoundError_PageId = storeFrontConfig.NotFoundError_PageId;
			this.NotificationsTheme = storeFrontConfig.NotificationsTheme;
			this.NotificationsThemeId = storeFrontConfig.NotificationsThemeId;
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
			this.OrderAdmin = storeFrontConfig.OrderAdmin;
			this.OrderAdmin_UserProfileId = storeFrontConfig.OrderAdmin_UserProfileId;


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

		[Display(Name = "Store Admin Theme", Description = "Choose a Theme for the Store Admin section of the site")]
		public Theme AdminTheme { get; protected set; }

		[Required]
		[Display(Name = "Store Admin Theme", Description = "Choose a Theme for the Store Admin section of the site")]
		public int AdminThemeId { get; set; }

		[Display(Name = "Reset All Pages to Theme", Description = "Set this box to a theme to reset the theme for ALL custom Pages.")]
		public int? ResetPagesToThemeId { get; set; }

		[Display(Name = "Shopping Cart Theme", Description = "Choose a Theme for the Shopping Cart section of the site")]
		public Theme CartTheme { get; protected set; }

		[Required]
		[Display(Name = "Shopping Cart Theme", Description = "Choose a Theme for the Shopping Cart section of the site")]
		public int CartThemeId { get; set; }

		[Required]
		[Display(Name = "Check Out Order Minimum Amount", Description = "Minimum order amount in dollars. Use 0 to allow free orders or no minimum. Default is 0.")]
		public decimal CheckoutOrderMinimum { get; set; }

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

		[Display(Name = "Catalog Admin Theme", Description = "Choose a Theme for the Catalog Admin section of the site")]
		public Theme CatalogAdminTheme { get; protected set; }

		[Required]
		[Display(Name = "Catalog Admin Theme", Description = "Choose a Theme for the Catalog Admin section of the site")]
		public int CatalogAdminThemeId { get; set; }

		[Display(Name = "Order Status Theme", Description = "Choose a Theme for the Order Status section of the site")]
		public Theme OrdersTheme { get; protected set; }

		[Required]
		[Display(Name = "Order Status Theme", Description = "Choose a Theme for the Order Status section of the site")]
		public int OrdersThemeId { get; set; }

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

		[Display(Name = "Store Catalog Theme", Description = "Choose a Theme for the Catalog section of the site with products and categories")]
		public Theme CatalogTheme { get; protected set; }

		[Required]
		[Display(Name = "Store Catalog Theme", Description = "Choose a Theme for the Catalog section of the site with products and categories")]
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
		[Display(Name = "Catalog Product Bundle Column Span Large", Description = "Number of columns (12 max) to span each Product Bundle on the catalog page for Large displays (desktop)")]
		[Range(1, 12)]
		public int CatalogProductBundleColLg { get; set; }

		[Required]
		[Display(Name = "Catalog Product Bundle Column Span Medium", Description = "Number of columns (12 max) to span each Product Bundle on the catalog page for Medium displays (tablet)")]
		[Range(1, 12)]
		public int CatalogProductBundleColMd { get; set; }

		[Required]
		[Display(Name = "Catalog Product Bundle Column Span Small", Description = "Number of columns (12 max) to span each Product Bundle on the catalog page for Small displays (phone)")]
		[Range(1, 12)]
		public int CatalogProductBundleColSm { get; set; }

		[Required]
		[Display(Name = "Catalog Product Bundle Item Column Span Large", Description = "Number of columns (12 max) to span each Product Bundle Item on the catalog page for Large displays (desktop)")]
		[Range(1, 12)]
		public int CatalogProductBundleItemColLg { get; set; }

		[Required]
		[Display(Name = "Catalog Product Bundle Item Column Span Medium", Description = "Number of columns (12 max) to span each Product Bundle Item on the catalog page for Medium displays (tablet)")]
		[Range(1, 12)]
		public int CatalogProductBundleItemColMd { get; set; }

		[Required]
		[Display(Name = "Catalog Product Bundle Item Column Span Small", Description = "Number of columns (12 max) to span each Product Bundle Item on the catalog page for Small displays (phone)")]
		[Range(1, 12)]
		public int CatalogProductBundleItemColSm { get; set; }

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

		[Display(Name = "Use Catalog as Home Page", Description = "Check this box to use your catalog page as your home page. Uncheck this box to use a custom page as your home page.\nDefault: Checked")]
		public bool HomePageUseCatalog { get; set; }

		[Display(Name = "Use Blog as Home Page", Description = "Check this box to use your Blog page as your home page. Uncheck this box to use a custom page as your home page.\nDefault: Checked")]
		public bool HomePageUseBlog { get; set; }

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
		[MaxLength(50)]
		[Display(Name = "Time Zone Id", Description = "Time zone for this store front.")]
		public string TimeZoneId { get; set; }

		[Display(Name = "Catalog Title", Description = "Catalog Title shown on all catalog pages.")]
		public string CatalogTitle { get; set; }

		[Required]
		[Display(Name = "Catalog Layout", Description = "Catalog Layout template setting.")]
		public CatalogLayoutEnum CatalogLayout { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Header Html", Description = "Catalog Header shown on all catalog pages.")]
		public string CatalogHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Footer Html", Description = "Catalog Footer shown on all catalog pages.")]
		public string CatalogFooterHtml { get; set; }

		[Required]
		[Display(Name = "Catalog Home List Template", Description = "Top (root) category display template to list root categories.")]
		public CategoryListTemplateEnum CatalogRootListTemplate { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Home Header Html", Description = "Catalog Header shown on the Home (root) catalog page.")]
		public string CatalogRootHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Home Footer Html", Description = "Catalog Footer shown on the Home (root) catalog page.")]
		public string CatalogRootFooterHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Bottom Description Caption", Description = "Default Top Description caption for products that do not have one defined.\nLeave this field blank to use the system default 'Details for [product name]'.\nExample: 'Description' or 'Details'")]
		public string CatalogDefaultBottomDescriptionCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog No Products Message Html", Description = "Message shown when there are no products in a category.\nLeave blank for the system default 'There are no products in this category.'")]
		public string CatalogDefaultNoProductsMessageHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Sample Audio Caption", Description = "Default Sample Audio caption for products that do not have one defined.\nLeave this field blank to use the system default 'Sample Audio for [product name]'.\nExample: 'Sample Sound' or 'Music'")]
		public string CatalogDefaultSampleAudioCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Sample Download File Caption", Description = "Default Sample Download File caption for products that do not have one defined.\nLeave this field blank to use the system default 'Sample Download for [product name]'.\nExample: 'Sample File' or 'Demo File'")]
		public string CatalogDefaultSampleDownloadCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Sample Image Caption", Description = "Default Sample Image caption for products that do not have one defined.\nLeave this field blank to use the system default 'Sample Image for [product name]'.\nExample: 'Sample Image' or 'Photo'")]
		public string CatalogDefaultSampleImageCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Summary Caption", Description = "Default Summary caption for products that do not have one defined.\nLeave this blank to use the system default 'Summary'\nExample: 'Summary' or 'Overview'")]
		public string CatalogDefaultSummaryCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Default Top Description Caption", Description = "Default Top Description caption for products that do not have one defined.\nLeave this field blank to use the system default 'Description for [product name]'.\nExample: 'Description' or 'Details'")]
		public string CatalogDefaultTopDescriptionCaption { get; set; }

		[Display(Name = "Catalog Default Product Type Single", Description = "Default Catalog name for Products (single)\nDefault: Item")]
		public string CatalogDefaultProductTypeSingle { get; set; }

		[Display(Name = "Catalog Default Product Type Plural", Description = "Default Catalog name for Products (plural)\nDefault: Items")]
		public string CatalogDefaultProductTypePlural { get; set; }

		[Display(Name = "Catalog Default Product Bundle Type Single", Description = "Default Catalog name for Product Bundles (single)\nDefault: Bundle")]
		public string CatalogDefaultProductBundleTypeSingle { get; set; }

		[Display(Name = "Catalog Default Product Bundle Type Plural", Description = "Default Catalog name for Product Bundles (plural)\nDefault: Bundles")]
		public string CatalogDefaultProductBundleTypePlural { get; set; }


		[Required]
		[Range(0, 6)]
		[Display(Name = "Site Menu Catalog Max Levels", Description = "Enter the number of Category levels to expand on the Site Top menu. 0 for none, up to 6.")]
		public int NavBarCatalogMaxLevels { get; set; }

		[Required]
		[Range(1, 6)]
		[Display(Name = "Site Menu Max Levels", Description = "Enter the maximum number of levels to expand on the Site Menu. Does not include Products.\nExample: 6")]
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

		[Display(Name = "Notifications (messaging) Theme", Description = "Choose a Theme for the Notifications/Messaging section of the site")]
		public Theme NotificationsTheme { get; protected set; }

		[Required]
		[Display(Name = "Notifications Theme", Description = "Choose a Theme for the Notifications/Messaging section of the site")]
		public int NotificationsThemeId { get; set; }

		[Display(Name = "Profile Theme", Description = "Choose a Theme for the User Profile section of the site")]
		public Theme ProfileTheme { get; protected set; }

		[Required]
		[Display(Name = "Profile Theme", Description = "Choose a Theme for the User Profile section of the site")]
		public int ProfileThemeId { get; set; }

		[Required]
		[Display(Name = "Public Url", Description = "Enter the full URL to your site\nExample: http://www.gstore.renog.info/stores/gstore")]
		[MaxLength(200)]
		public string PublicUrl { get; set; }

		[Display(Name = "Show Blog in Site Menu")]
		public bool ShowBlogInMenu { get; set; }

		[Display(Name = "Show About GStore Menu")]
		public bool ShowAboutGStoreMenu { get; set; }

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

		[Display(Name = "Show Shopping Cart even when Empty", Description = "Check this box to show the cart link in the top menu even when the cart is empty.\nDefault: checked")]
		public bool CartNavShowCartWhenEmpty { get; set; }

		[Display(Name = "Require Login to Add to Cart", Description = "Check this box to require the user to log in when adding to cart.\nDefault: Unchecked")]
		public bool CartRequireLogin { get; set; }


		[Display(Name = "Welcome Person", Description = "This profile will be used to automatically send a welcome message to newly registered users for their first login")]
		public UserProfile WelcomePerson { get; protected set; }

		[Required]
		[Display(Name = "Welcome Person", Description = "This profile will be used to automatically send a welcome message to newly registered users for their first login")]
		public int WelcomePerson_UserProfileId { get; set; }

		[ForeignKey("OrderAdmin_UserProfileId")]
		[Display(Name = "Order Admin", Description = "This profile will be used to handle new orders, and order notifications.")]
		public virtual UserProfile OrderAdmin { get; set; }

		[Display(Name = "Order Admin", Description = "This profile will be used to handle new orders, and order notifications.")]
		public int OrderAdmin_UserProfileId { get; set; }

		[Display(Name = "Auto-Accept orders if paid", Description = "Check this box to automatically accept all orders that are paid. Uncheck this box to manually accept and confirm paid orders.")]
		public bool Orders_AutoAcceptPaid { get; set; }

		[Display(Name = "Payment Method - Use PayPal", Description = "Check this box to use PayPal as a payment method.\nIf you do not have any payment methods selected, the user will order and you will have to contact them for payment info.")]
		public bool PaymentMethod_PayPal_Enabled { get; set; }

		[Display(Name = "Payment Method - PayPal - Use Live Transactions", Description = "Check this box to run Live transactions on Paypal, uncheck this box to use the sandbox/test server.")]
		public bool PaymentMethod_PayPal_UseLiveServer { get; set; }

		[Display(Name = "Payment Method - PayPal API - Client_Id", Description = "Enter your PayPal API key labeled 'client_id' \nExample: AbCDE1fGHIjkLMno_pqrStuVL1ZYaPFHJ23BD126512651211111y0fztLABCDEFGHIJKLMNabc1P-SN")]
		public string PaymentMethod_PayPal_Client_Id { get; set; }

		[Display(Name = "Payment Method - PayPal API - Client_Secret", Description = "Enter your PayPal API key labeled 'client_secret' \nExample: AbCDE1fGHIjkLMno_pqrStuVL1ZYaPFHJ23BD126512651211111y0fztLABCDEFGHIJKLMNabc1P-SN")]
		public string PaymentMethod_PayPal_Client_Secret { get; set; }

		[Display(Name = "Blog Theme", Description="Theme for the Blog List page and blogs that do not have a specific theme defined.")]
		public Theme BlogTheme { get; set; }

		[Required]
		[Display(Name = "Blog Theme", Description="Theme for the Blog List page and blogs that do not have a specific theme defined.")]
		public int BlogThemeId { get; set; }

		[Display(Name = "Blog Admin Theme", Description="Theme for the Blog Admin pages.")]
		public Theme BlogAdminTheme { get; set; }

		[Required]
		[Display(Name = "Blog Admin Theme", Description="Theme for the Blog Admin pages.")]
		public int BlogAdminThemeId { get; set; }

		[Display(Name = "Chat Theme", Description="Theme for the Chat pages.")]
		public Theme ChatTheme { get; set; }

		[Required]
		[Display(Name = "Chat Theme", Description="Theme for the Chat pages.")]
		public int ChatThemeId { get; set; }

		[Display(Name = "Enable Chat", Description = "Enable the Chat feature of the site for general chat on your web site.")]
		public bool ChatEnabled { get; set; }

		[Display(Name = "Chat - Login Required", Description = "Check this box to require users to log in to use the Chat feature of the site.")]
		public bool ChatRequireLogin { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to Inactivate this configuration immediately. \nIf checked, make sure you have another active configuration. or your site will be inactive and you can only access the store admin section.")]
		public bool IsPending { get; set; }

		[Required]
		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this configuration to go ACTIVE on. \nIf this date is in the future, your configuration will be inactive until the start date.\nExample: 1/1/2000 12:00 AM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Required]
		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this configuration to go INACTIVE on. \nIf this date is in the past, your configuration will be inactive immediately.\nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		#region IValidatableObject Members

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> result = new List<ValidationResult>();
			if (this.PaymentMethod_PayPal_Enabled && string.IsNullOrEmpty(this.PaymentMethod_PayPal_Client_Id))
			{
				string blankFieldName = "PaymentMethod_PayPal_Client_Id";
				string fieldDisplayBlank = this.GetDisplayName("PaymentMethod_PayPal_Client_Id");
				string fieldDisplayCheckbox = this.GetDisplayName("PaymentMethod_PayPal_Enabled");
				result.Add(new ValidationResult("'" + fieldDisplayBlank + "' must be entered when the '" + fieldDisplayCheckbox + "' box is checked.", new string[] { blankFieldName }));
			}

			if (this.PaymentMethod_PayPal_Enabled && string.IsNullOrEmpty(this.PaymentMethod_PayPal_Client_Secret))
			{
				string blankFieldName = "PaymentMethod_PayPal_Client_Secret";
				string fieldDisplayBlank = this.GetDisplayName("PaymentMethod_PayPal_Client_Secret");
				string fieldDisplayCheckbox = this.GetDisplayName("PaymentMethod_PayPal_Enabled");
				result.Add(new ValidationResult("'" + fieldDisplayBlank + "' must be entered when the '" + fieldDisplayCheckbox + "' box is checked.", new string[] { blankFieldName }));
			}

			return result;
		}

		#endregion


	}
}