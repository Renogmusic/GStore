using GStore.Models;
using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GStore.Areas.OrderAdmin.ViewModels
{
	public class OrderAdminViewModel
	{
		public OrderAdminViewModel() { }

		public OrderAdminViewModel(StoreFrontConfiguration currentStoreFrontConfig, UserProfile userProfile)
		{
			if (currentStoreFrontConfig == null)
			{
				throw new ApplicationException("OrderAdminMenuViewModel: currentStoreFrontConfig is null, currentStoreFrontConfig must be specified.");
			}
			if (userProfile == null)
			{
				throw new ApplicationException("OrderAdminMenuViewModel: userProfile is null, UserProfile must be specified.");
			}
			this.StoreFrontConfig = currentStoreFrontConfig;
			this.StoreFront = currentStoreFrontConfig.StoreFront;
			this.UserProfile = userProfile;
			this.Client = currentStoreFrontConfig.Client;
			this.ShowSystemAdminLink = userProfile.AspNetIdentityUserIsInRoleSystemAdmin();
			this.ShowStoreAdminLink = this.StoreFront.ShowStoreAdminLink(userProfile);
			this.ShowCatalogAdminLink = this.StoreFront.ShowCatalogAdminLink(userProfile);
		}
		public void UpdateClient(Client client)
		{
			this.Client = client;
		}

		[Display(Name = "Store Front Configuration")]
		public StoreFrontConfiguration StoreFrontConfig { get; protected set; }

		[Display(Name = "Store Front")]
		public StoreFront StoreFront { get; protected set; }

		[Display(Name = "Client")]
		public Client Client { get; protected set; }

		[Display(Name = "User Profile")]
		public UserProfile UserProfile { get; protected set; }

		[Display(Name = "Is Active")]
		public bool IsActiveDirect { get; protected set; }

		[Display(Name = "Show System Admin Link")]
		public bool ShowSystemAdminLink { get; protected set; }

		[Display(Name = "Show Store Admin Link")]
		public bool ShowStoreAdminLink { get; protected set; }

		[Display(Name = "Show Catalog Admin Link")]
		public bool ShowCatalogAdminLink { get; protected set; }

	}
}