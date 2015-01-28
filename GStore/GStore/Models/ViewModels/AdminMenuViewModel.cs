﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;
using GStore.Identity;

namespace GStore.Models.ViewModels
{
	public class AdminMenuViewModel
	{
		public UserProfile UserProfile { get; protected set; }
		public bool ShowOrderAdminLink { get; protected set; }
		public bool ShowCatalogAdminLink { get; protected set; }
		public bool ShowStoreAdminLink { get; protected set; }
		public bool ShowSystemAdminLink { get; protected set; }

		public AdminMenuViewModel(StoreFront storeFront, UserProfile userProfile, string currentArea)
		{
			this.UserProfile = userProfile;
			if (currentArea.ToLower() != "catalogadmin")
			{
				this.ShowCatalogAdminLink = storeFront.ShowCatalogAdminLink(userProfile);
			}
			if (currentArea.ToLower() != "orderadmin")
			{
				this.ShowOrderAdminLink = storeFront.ShowOrderAdminLink(userProfile);
			}
			if (currentArea.ToLower() != "storeadmin")
			{
				this.ShowStoreAdminLink = storeFront.ShowStoreAdminLink(userProfile);
			}
			if (currentArea.ToLower() != "systemadmin")
			{
				this.ShowSystemAdminLink = userProfile.AspNetIdentityUserIsInRoleSystemAdmin();
			}
		}
	}
}