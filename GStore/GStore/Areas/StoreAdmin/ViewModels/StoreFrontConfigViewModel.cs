using GStore.Models;
using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class StoreFrontConfigViewModel : StoreAdminViewModel
	{
		public StoreFrontConfigViewModel() { }

		public StoreFrontConfigViewModel(StoreFront storeFront, UserProfile userProfile, string activeTab): base(storeFront, userProfile)
		{
			this.ActiveTab = activeTab;
			this.StoreFrontId = storeFront.StoreFrontId;
			this.AccountAdmin = storeFront.AccountAdmin;
			this.AccountAdmin_UserProfileId = storeFront.AccountAdmin_UserProfileId;
			this.AccountLayoutName = storeFront.AccountLayoutName;
			this.AccountTheme = storeFront.AccountTheme;
			this.AccountThemeId = storeFront.AccountThemeId;
			this.AccountLoginRegisterLinkText = storeFront.AccountLoginRegisterLinkText;
			this.AccountLoginShowRegisterLink = storeFront.AccountLoginShowRegisterLink;
			this.AdminLayoutName = storeFront.AdminLayoutName;
			this.AdminTheme = storeFront.AdminTheme;
			this.AdminThemeId = storeFront.AdminThemeId;
			this.CatalogCategoryColLg = storeFront.CatalogCategoryColLg;
			this.CatalogCategoryColMd = storeFront.CatalogCategoryColMd;
			this.CatalogCategoryColSm = storeFront.CatalogCategoryColSm;
			this.CatalogLayoutName = storeFront.CatalogLayoutName;
			this.CatalogTheme = storeFront.CatalogTheme;
			this.CatalogThemeId = storeFront.CatalogThemeId;
			this.CatalogPageInitialLevels = storeFront.CatalogPageInitialLevels;
			this.CatalogProductColLg = storeFront.CatalogProductColLg;
			this.CatalogProductColMd = storeFront.CatalogProductColMd;
			this.CatalogProductColSm = storeFront.CatalogProductColSm;
			this.DefaultNewPageLayoutName = storeFront.DefaultNewPageLayoutName;
			this.DefaultNewPageTheme = storeFront.DefaultNewPageTheme;
			this.DefaultNewPageThemeId = storeFront.DefaultNewPageThemeId;
			this.EnableGoogleAnalytics = storeFront.EnableGoogleAnalytics;
			this.GoogleAnalyticsWebPropertyId = storeFront.GoogleAnalyticsWebPropertyId;
			this.HtmlFooter = storeFront.HtmlFooter;
			this.MetaApplicationName = storeFront.MetaApplicationName;
			this.MetaApplicationTileColor = storeFront.MetaApplicationTileColor;
			this.MetaDescription = storeFront.MetaDescription;
			this.MetaKeywords = storeFront.MetaKeywords;
			this.Order = storeFront.Order;
			this.BodyTopScriptTag = storeFront.BodyTopScriptTag;
			this.BodyBottomScriptTag = storeFront.BodyBottomScriptTag;
			this.Name = storeFront.Name;
			this.NavBarCatalogMaxLevels = storeFront.NavBarCatalogMaxLevels;
			this.NavBarItemsMaxLevels = storeFront.NavBarItemsMaxLevels;
			this.NavBarRegisterLinkText = storeFront.NavBarRegisterLinkText;
			this.NavBarShowRegisterLink = storeFront.NavBarShowRegisterLink;
			this.NotFoundErrorPage = storeFront.NotFoundErrorPage;
			this.NotFoundError_PageId = storeFront.NotFoundError_PageId;
			this.NotificationsLayoutName = storeFront.NotificationsLayoutName;
			this.NotificationsTheme = storeFront.NotificationsTheme;
			this.NotificationsThemeId = storeFront.NotificationsThemeId;
			this.ProfileLayoutName = storeFront.ProfileLayoutName;
			this.ProfileTheme = storeFront.ProfileTheme;
			this.ProfileThemeId = storeFront.ProfileThemeId;
			this.PublicUrl = storeFront.PublicUrl;
			this.RegisteredNotify = storeFront.RegisteredNotify;
			this.RegisteredNotify_UserProfileId = storeFront.RegisteredNotify_UserProfileId;
			this.RegisterSuccessPage = storeFront.RegisterSuccessPage;
			this.RegisterSuccess_PageId = storeFront.RegisterSuccess_PageId;
			this.RegisterWebForm = storeFront.RegisterWebForm;
			this.Register_WebFormId = storeFront.Register_WebFormId;
			this.StoreErrorPage = storeFront.StoreErrorPage;
			this.StoreError_PageId = storeFront.StoreError_PageId;
			this.WelcomePerson = storeFront.WelcomePerson;
			this.WelcomePerson_UserProfileId = storeFront.WelcomePerson_UserProfileId;

			this.IsSystemAdmin = userProfile.AspNetIdentityUserIsInRoleSystemAdmin();
		}

		public string ActiveTab { get; set; }

		[Key]
		[Display(Name = "Store Front Id", Description = "Internal Store Front Id Number")]
		public int StoreFrontId { get; set; }

		[Display(Name = "Account Admin", Description = "Profile to use for sending Account Login notices such as Locked Out notification and Password Changed notification")]
		public UserProfile AccountAdmin { get; set; }

		[Display(Name = "Account Admin Id", Description = "Profile to use for sending Account Login notices such as Locked Out notification and Password Changed notification")]
		public int AccountAdmin_UserProfileId { get; set; }

		[Display(Name = "Account (register and login) Layout", Description = "Always 'Bootstrap'")]
		[MaxLength(10)]
		public string AccountLayoutName { get; set; }

		[Display(Name = "Account Register/Login Theme", Description = "Choose a Theme for the Account Register/Login section of the site")]
		public Theme AccountTheme { get; set; }

		[Display(Name = "Account (register and login) Theme Id", Description = "Choose a Theme for the Account Register/Login section of the site")]
		public int AccountThemeId { get; set; }

		[Display(Name = "Account Login Show Register Link", Description = "Check this box to show the register/signup link on the login page. Uncheck to NOT show it")]
		public bool AccountLoginShowRegisterLink { get; set; }

		[Display(Name = "Account Login Register Link Text", Description = "Text label for the register/signup link on the login page.")]
		[MaxLength(50)]
		public string AccountLoginRegisterLinkText { get; set; }

		[Display(Name = "Store Admin Layout Name", Description = "Always 'Bootstrap'")]
		[MaxLength(10)]
		public string AdminLayoutName { get; set; }

		[Display(Name = "Store Admin Theme", Description = "Choose a Theme for the Store Admin section of the site")]
		public Theme AdminTheme { get; set; }

		[Display(Name = "Store Admin Theme Id", Description = "Choose a Theme for the Store Admin section of the site")]
		public int AdminThemeId { get; set; }

		[Display(Name = "Catalog Category Column Span Large", Description = "Number of columns (12 max) to span each Category on the catalog page for Large displays (desktop)")]
		[Range(1, 12)]
		public int CatalogCategoryColLg { get; set; }

		[Display(Name = "Catalog Category Column Span Medium", Description = "Number of columns (12 max) to span each Category on the catalog page for Medium displays (tablet)")]
		[Range(1, 12)]
		public int CatalogCategoryColMd { get; set; }

		[Display(Name = "Catalog Category Column Span Small", Description = "Number of columns (12 max) to span each Category on the catalog page for Small displays (phone)")]
		[Range(1, 12)]
		public int CatalogCategoryColSm { get; set; }

		[Display(Name = "Catalog Layout Name", Description = "Always 'Bootstrap'")]
		[MaxLength(10)]
		public string CatalogLayoutName { get; set; }

		[Display(Name = "Catalog Theme", Description = "Choose a Theme for the Catalog section of the site with products and categories")]
		public Theme CatalogTheme { get; set; }

		[Display(Name = "Catalog Theme Id", Description = "Choose a Theme for the Catalog section of the site with products and categories")]
		public int CatalogThemeId { get; set; }

		[Display(Name = "Catalog Page Initial Levels", Description = "Enter the number of levels to automatically drill down on the catalog page, max 6")]
		[Range(1, 6)]
		public int CatalogPageInitialLevels { get; set; }

		[Display(Name = "Catalog Product Column Span Large", Description = "Number of columns (12 max) to span each Product on the catalog page for Large displays (desktop)")]
		[Range(1, 12)]
		public int CatalogProductColLg { get; set; }

		[Display(Name = "Catalog Product Column Span Medium", Description = "Number of columns (12 max) to span each Product on the catalog page for Medium displays (tablet)")]
		[Range(1, 12)]
		public int CatalogProductColMd { get; set; }

		[Display(Name = "Catalog Product Column Span Small", Description = "Number of columns (12 max) to span each Product on the catalog page for Small displays (phone)")]
		[Range(1, 12)]
		public int CatalogProductColSm { get; set; }

		[Display(Name = "Default New Page Layout Name", Description = "Always 'Bootstrap'")]
		[MaxLength(10)]
		public string DefaultNewPageLayoutName { get; set; }

		[Display(Name = "Default New Page Theme", Description = "Choose a Theme as the default for new pages")]
		public Theme DefaultNewPageTheme { get; set; }

		[Display(Name = "Default New Page Theme Id", Description = "Choose a Theme as the default for new pages")]
		public int DefaultNewPageThemeId { get; set; }

		[Display(Name = "Enable Google Analytics", Description = "Check this box to use Google Analytics tracking\nBe sure to enter the Google analytics WebProperty Id as well")]
		public bool EnableGoogleAnalytics { get; set; }

		[Display(Name = "Google Analytics Web Property Id", Description = "Google Analytics Web Property Id for tracking\nExample: UA-123456-1")]
		[MaxLength(50)]
		public string GoogleAnalyticsWebPropertyId { get; set; }

		[AllowHtml]
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

		[Display(Name = "Page Script on Top of Pages", Description = "Advanced: Script tags or HTML tags shown on the Top of pages for search engines.")]
		public string BodyTopScriptTag { get; set; }

		[Display(Name = "Page Script on Bottom of Pages", Description = "Advanced: Script tags or HTML tags shown on the bottom of pages for search engines.")]
		public string BodyBottomScriptTag { get; set; }

		[Display(Name = "Name", Description = "Name of Store Front - this is added to the Page title of all pages on the web site.")]
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		[Display(Name = "Nav Bar Catalog Max Levels", Description = "Enter the number of Category levels to expand on the NavBar menu")]
		public int NavBarCatalogMaxLevels { get; set; }

		[Display(Name = "Nav Bar Items Max Levels", Description = "Enter the number of Menu Item levels to expand on the NavBar menu")]
		public int NavBarItemsMaxLevels { get; set; }

		[Display(Name = "Nav Bar Show Register Link", Description = "Check this box to show the Register/Sign-Up link on the Site Menu (Nav Bar). Uncheck to NOT show it")]
		public bool NavBarShowRegisterLink { get; set; }

		[Display(Name = "Nav Bar Register Link Text", Description = "Enter a label for the Register/Sign-Up link on the Site Menu (Nav Bar)\nExample: Register")]
		[MaxLength(50)]
		public string NavBarRegisterLinkText { get; set; }

		[Required]
		[Display(Name = "Index", Description = "Use this to sort your store front list in a particular order. \nExample: 100")]
		public int Order { get; set; }


		[Display(Name = "Register Web Form", Description = "Register Form for new users signing up or use the system default registration form")]
		public WebForm RegisterWebForm { get; set; }

		[Display(Name = "Register Web Form Id", Description = "Register Form for new users signing up or use the system default registration form")]
		public int? Register_WebFormId { get; set; }

		[Display(Name = "Not Found Error Page", Description = "Choose a page to display for File Not Found (404) errors or use the system default not found page")]
		public Page NotFoundErrorPage { get; set; }

		[Display(Name = "Not Found Error Page Id", Description = "Choose a page to display for File Not Found (404) errors or use the system default not found page")]
		public int? NotFoundError_PageId { get; set; }

		[Display(Name = "Notifications Layout Name", Description = "Always 'Bootstrap'")]
		[MaxLength(10)]
		public string NotificationsLayoutName { get; set; }

		[Display(Name = "Notifications (messaging) Theme", Description = "Choose a Theme for the Notifications/Messaging section of the site")]
		public Theme NotificationsTheme { get; set; }

		[Display(Name = "Notifications Theme Id", Description = "Choose a Theme for the Notifications/Messaging section of the site")]
		public int NotificationsThemeId { get; set; }

		[Display(Name = "Profile Layout Name", Description = "Always 'Bootstrap'")]
		[MaxLength(10)]
		public string ProfileLayoutName { get; set; }

		[Display(Name = "Profile Theme", Description = "Choose a Theme for the User Profile section of the site")]
		public Theme ProfileTheme { get; set; }

		[Display(Name = "Profile Theme Id", Description = "Choose a Theme for the User Profile section of the site")]
		public int ProfileThemeId { get; set; }

		[Display(Name = "Public Url", Description = "Enter the full URL to your site\nExample: http://www.gstore.renog.info/stores/gstore")]
		[MaxLength(200)]
		public string PublicUrl { get; set; }

		[Display(Name = "Registered Notify", Description = "This profile will receive a notification when a new user signs up")]
		public UserProfile RegisteredNotify { get; set; }

		[Display(Name = "Registered Notify Id", Description = "This profile will receive a notification when a new user signs up")]
		public int RegisteredNotify_UserProfileId { get; set; }

		[Display(Name = "Register Success Page", Description = "Choose a page to display when a user signs up successfully, or or use the system default register success page")]
		public Page RegisterSuccessPage { get; set; }

		[Display(Name = "Register Success Page Id", Description = "Choose a page to display when a user signs up successfully, or or use the system default register success page")]
		public int? RegisterSuccess_PageId { get; set; }

		[Display(Name = "Store Error Page", Description = "Choose a page to display for web site errors or use the system default error page")]
		public Page StoreErrorPage { get; set; }

		[Display(Name = "Store Error Page Id", Description = "Choose a page to display for web site errors or use the system default error page")]
		public int? StoreError_PageId { get; set; }

		[Display(Name = "Welcome Person", Description = "This profile will be used to automatically send a welcome message to newly registered users for their first login")]
		public UserProfile WelcomePerson { get; set; }

		[Display(Name = "Welcome Person Id", Description = "This profile will be used to automatically send a welcome message to newly registered users for their first login")]
		public int WelcomePerson_UserProfileId { get; set; }
	}
}