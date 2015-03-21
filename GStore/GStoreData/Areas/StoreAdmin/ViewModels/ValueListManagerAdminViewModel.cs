using System.Linq;
using GStoreData.Models;

namespace GStoreData.Areas.StoreAdmin.ViewModels
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