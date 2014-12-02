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

		public StoreFrontConfigViewModel(StoreFront storeFront, UserProfile userProfile): base(storeFront, userProfile)
		{
			this.StoreFrontId = storeFront.StoreFrontId;
			this.AccountAdmin_UserProfileId = storeFront.AccountAdmin_UserProfileId;
			this.AccountLayoutName = storeFront.AccountLayoutName;
			this.AccountLoginRegisterLinkText = storeFront.AccountLoginRegisterLinkText;
			this.AccountLoginShowRegisterLink = storeFront.AccountLoginShowRegisterLink;
			this.AdminLayoutName = storeFront.AdminLayoutName;
			this.CatalogCategoryColLg = storeFront.CatalogCategoryColLg;
			this.CatalogCategoryColMd = storeFront.CatalogCategoryColMd;
			this.CatalogCategoryColSm = storeFront.CatalogCategoryColSm;
			this.CatalogLayoutName = storeFront.CatalogLayoutName;
			this.CatalogPageInitialLevels = storeFront.CatalogPageInitialLevels;
			this.CatalogProductColLg = storeFront.CatalogProductColLg;
			this.CatalogProductColMd = storeFront.CatalogProductColMd;
			this.CatalogProductColSm = storeFront.CatalogProductColSm;
			this.DefaultNewPageLayoutName = storeFront.DefaultNewPageLayoutName;
			this.EnableGoogleAnalytics = storeFront.EnableGoogleAnalytics;
			this.GoogleAnalyticsWebPropertyId = storeFront.GoogleAnalyticsWebPropertyId;
			this.HtmlFooter = storeFront.HtmlFooter;
			this.MetaApplicationName = storeFront.MetaApplicationName;
			this.MetaApplicationTileColor = storeFront.MetaApplicationTileColor;
			this.MetaDescription = storeFront.MetaDescription;
			this.MetaKeywords = storeFront.MetaKeywords;
			this.Name = storeFront.Name;
			this.NavBarCatalogMaxLevels = storeFront.NavBarCatalogMaxLevels;
			this.NavBarItemsMaxLevels = storeFront.NavBarItemsMaxLevels;
			this.NavBarRegisterLinkText = storeFront.NavBarRegisterLinkText;
			this.NavBarShowRegisterLink = storeFront.NavBarShowRegisterLink;
			this.NotFoundError_PageId = storeFront.NotFoundError_PageId;
			this.NotificationsLayoutName = storeFront.NotificationsLayoutName;
			this.ProfileLayoutName = storeFront.ProfileLayoutName;
			this.PublicUrl = storeFront.PublicUrl;
			this.RegisteredNotify_UserProfileId = storeFront.RegisteredNotify_UserProfileId;
			this.StoreError_PageId = storeFront.StoreError_PageId;
			this.ThemeId = storeFront.ThemeId;
			this.WelcomePerson_UserProfileId = storeFront.WelcomePerson_UserProfileId;

			this.IsSystemAdmin = userProfile.AspNetIdentityUserIsInRoleSystemAdmin();
		}

		[Key]
		public int StoreFrontId { get; set; }
		public int AccountAdmin_UserProfileId { get; set; }
		public string AccountLayoutName { get; set; }
		public string AccountLoginRegisterLinkText { get; set; }
		public bool AccountLoginShowRegisterLink { get; set; }
		public string AdminLayoutName { get; set; }
		public int CatalogCategoryColLg { get; set; }
		public int CatalogCategoryColMd  { get; set; }
		public int CatalogCategoryColSm { get; set; }
		public string CatalogLayoutName { get; set; }
		public int CatalogPageInitialLevels { get; set; }
		public int CatalogProductColLg { get; set; }
		public int CatalogProductColMd { get; set; }
		public int CatalogProductColSm { get; set; }
		public string DefaultNewPageLayoutName { get; set; }
		public bool EnableGoogleAnalytics { get; set; }
		public string GoogleAnalyticsWebPropertyId { get; set; }

		[AllowHtml]
		public string HtmlFooter { get; set; }

		public string MetaApplicationName { get; set; }
		public string MetaApplicationTileColor { get; set; }
		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string Name { get; set; }
		public int NavBarCatalogMaxLevels { get; set; }
		public int NavBarItemsMaxLevels { get; set; }
		public string NavBarRegisterLinkText { get; set; }
		public bool NavBarShowRegisterLink { get; set; }
		public int? NotFoundError_PageId { get; set; }
		public string NotificationsLayoutName { get; set; }
		public string ProfileLayoutName { get; set; }
		public string PublicUrl { get; set; }
		public int RegisteredNotify_UserProfileId { get; set; }
		public int? StoreError_PageId { get; set; }
		public int ThemeId { get; set; }
		public int WelcomePerson_UserProfileId { get; set; }

	}
}