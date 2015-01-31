using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GStore.Models.ViewModels
{
	public class PayPalInfo
	{
		public string Business { get; set; }
		public string Item_Name { get; set; }
		public string Item_Number { get; set; }
	}
}
