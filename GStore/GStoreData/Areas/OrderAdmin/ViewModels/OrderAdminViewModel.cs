using System;
using System.ComponentModel.DataAnnotations;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData.Areas.OrderAdmin.ViewModels
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
		}
		public void UpdateClient(Client client)
		{
			this.Client = client;
		}

		public AdminMenuViewModel AdminMenuViewModel
		{
			get
			{
				return new AdminMenuViewModel(this.StoreFront, this.UserProfile, "OrderAdmin");
			}
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

	}
}