using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using GStoreData.Models.BaseClasses;

namespace GStoreData.Models
{
	[Table("CartPaymentInfo")]
	public class CartPaymentInfo : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Cart Payment Info Id")]
		public int CartPaymentInfoId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Cart Id")]
		public int CartId { get; set; }

		[ForeignKey("CartId")]
		[Display(Name = "Cart Id")]
		public virtual Cart Cart { get; set; }

		[Display(Name = "Payment Source")]
		public GStorePaymentSourceEnum PaymentSource { get; set; }

		[Display(Name = "PayPal Payment Auth Id")]
		public string PayPalPaymentId { get; set; }

		/// <summary>
		/// Payment Id on bounce back from paypal
		/// </summary>
		public string PayPalReturnPaymentId { get; set; }

		/// <summary>
		/// Payment Id on bounce back from paypal
		/// </summary>
		public string PayPalReturnPayerId { get; set; }

		/// <summary>
		/// Token querystring value on bounce back from paypal
		/// </summary>
		public string PayPalReturnToken { get; set; }

		[Display(Name = "JSON Payment Data")]
		public string Json { get; set; }

		public int? WebFormResponseId { get; set; }

		[ForeignKey("WebFormResponseId")]
		public virtual WebFormResponse WebFormResponse { get; set; }

	}
}