using System.ComponentModel.DataAnnotations;

namespace GStoreData.Models.BaseClasses
{
	/// <summary>
	/// Enum of GStore payment sources
	/// </summary>
	public enum GStorePaymentSourceEnum : int
	{
		
		/// <summary>
		/// Manual Entry
		/// </summary>
		[Display(Name = "Manual Entry", Order = 60)]
		ManualEntry = 0,

		/// <summary>
		/// PayPal
		/// </summary>
		[Display(Name = "PayPal", Order = 50)]
		PayPal = 50,



	}

}
