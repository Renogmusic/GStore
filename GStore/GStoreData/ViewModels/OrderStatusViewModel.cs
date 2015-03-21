using System.ComponentModel.DataAnnotations;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class OrdersViewModel
	{
		public OrdersViewModel() { }

		public OrdersViewModel(UserProfile userProfile)
		{

		}

		[Editable(false)]
		public UserProfile UserProfile {get; protected set;}

	}
}