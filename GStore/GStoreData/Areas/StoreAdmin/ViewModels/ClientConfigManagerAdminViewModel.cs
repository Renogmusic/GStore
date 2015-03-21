using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GStoreData.Models;

namespace GStoreData.Areas.StoreAdmin.ViewModels
{
	public class ClientConfigManagerAdminViewModel : StoreAdminViewModel
	{
		public ClientConfigManagerAdminViewModel() { }

		public ClientConfigManagerAdminViewModel(Client currentClient, StoreFrontConfiguration currentStoreFrontConfig, UserProfile userProfile)
			: base(currentStoreFrontConfig, userProfile)
		{
			if (currentClient == null)
			{
				throw new ArgumentNullException("currentClient");
			}
			if (currentStoreFrontConfig == null)
			{
				throw new ArgumentNullException("currentStoreFrontConfig");
			}

			this.CurrentClient = currentClient;
			this.CurrentStoreFrontConfig = currentStoreFrontConfig;
			this.CurrentStoreFront = currentStoreFrontConfig.StoreFront;
		}

		public Client CurrentClient { get; protected set; }
		public StoreFront CurrentStoreFront { get; protected set; }
		public StoreFrontConfiguration CurrentStoreFrontConfig { get; protected set; }

		public List<SelectListItem> StoreFrontConfigList(StoreFront storeFront)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException ("storeFront");
			}

			StoreFrontConfiguration currentConfig = storeFront.CurrentConfig();
			int currentconfigId = 0;
			if (currentConfig != null)
			{
				currentconfigId = currentConfig.StoreFrontConfigurationId;
			}

			List<SelectListItem> list = new List<SelectListItem>();
			List<StoreFrontConfiguration> orderedConfigs = storeFront.StoreFrontConfigurations.AsQueryable().ApplyDefaultSort().ToList();
			List<SelectListItem> listItemQuery = orderedConfigs.Select(c => new SelectListItem()
				{
					Value = c.StoreFrontConfigurationId.ToString(),
					Text = (c.StoreFrontConfigurationId == currentconfigId ? " [Current Active] " : "") 
						+ c.ConfigurationName + " [" + c.StoreFrontConfigurationId + "]" 
						+ (c.IsPending ? " [INACTIVE]" : " [" + c.StartDateTimeUtc.ToLocalTime().ToShortDateString() + " to " + c.EndDateTimeUtc.ToLocalTime().ToShortDateString() + "]"),
					Selected = (c.StoreFrontConfigurationId == currentconfigId)
				}).ToList();

			list.AddRange(listItemQuery.ToList());

			return list;
		}


	}
}