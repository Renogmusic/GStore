using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class ValueListManagerAdminViewModel : StoreAdminViewModel
	{
		public ValueListManagerAdminViewModel(StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, IOrderedQueryable<ValueList> valueLists)
			: base(storeFrontConfig, userProfile)
		{
			this.ValueLists = valueLists;
		}

		public IOrderedQueryable<ValueList> ValueLists { get; set; }

	}
}