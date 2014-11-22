using GStore.Models;
using GStore.Identity;
using GStore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GStore.Areas.StoreAdmin.Models.ViewModels
{
	public class StoreFrontConfigViewModel : StoreAdminViewModel
	{
		public StoreFrontConfigViewModel(StoreFront storeFront, UserProfile userProfile): base(storeFront, userProfile)
		{
			this.StoreFrontId = storeFront.StoreFrontId;
			this.AccountAdmin_UserProfileId = storeFront.AccountAdmin_UserProfileId;
			this.AccountLayoutName = storeFront.AccountLayoutName;
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
			this.MetaApplicationName = storeFront.MetaApplicationName;
			this.MetaApplicationTileColor = storeFront.MetaApplicationTileColor;
			this.MetaDescription = storeFront.MetaDescription;
			this.MetaKeywords = storeFront.MetaKeywords;
			this.Name = storeFront.Name;
			this.NavBarCatalogMaxLevels = storeFront.NavBarCatalogMaxLevels;
			this.NavBarItemsMaxLevels = storeFront.NavBarItemsMaxLevels;
			this.NotFoundError_PageId = storeFront.NotFoundError_PageId;
			this.NotificationsLayoutName = storeFront.NotificationsLayoutName;
			this.ProfileLayoutName = storeFront.ProfileLayoutName;
			this.PublicUrl = storeFront.PublicUrl;
			this.RegisteredNotify_UserProfileId = storeFront.RegisteredNotify_UserProfileId;
			this.StoreError_PageId = storeFront.StoreError_PageId;
			this.ThemeId = storeFront.ThemeId;
			this.WelcomePerson_UserProfileId = storeFront.WelcomePerson_UserProfileId;

		}

		[Key]
		public int StoreFrontId { get; set; }
		public int AccountAdmin_UserProfileId { get; set; }
		public string AccountLayoutName { get; set; }
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
		public string MetaApplicationName { get; set; }
		public string MetaApplicationTileColor { get; set; }
		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string Name { get; set; }
		public int NavBarCatalogMaxLevels { get; set; }
		public int NavBarItemsMaxLevels { get; set; }
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