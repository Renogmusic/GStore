using System.Linq;
using GStoreData.Models;

namespace GStoreData.Areas.StoreAdmin.ViewModels
{
	public class WebFormManagerAdminViewModel : StoreAdminViewModel
	{
		public WebFormManagerAdminViewModel(StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, IOrderedQueryable<WebForm> webForms)
			: base(storeFrontConfig, userProfile)
		{
			this.WebForms = webForms;
		}

		public IOrderedQueryable<WebForm> WebForms { get; set; }

	}
}