using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class CheckoutWebFormInfo
	{
		public CheckoutWebFormInfo(CheckoutViewModelBase checkoutViewModel, WebForm webForm, WebFormResponse webFormResponse)
		{
			this.CheckoutViewModel = checkoutViewModel;
			this.WebForm = webForm;
			this.WebFormResponse = webFormResponse;
		}

		public CheckoutViewModelBase CheckoutViewModel { get; set;}
		public WebForm WebForm { get; set; }
		public WebFormResponse WebFormResponse { get; set; }
	}
}
