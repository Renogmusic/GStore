using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GStore.Models.ViewModels
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