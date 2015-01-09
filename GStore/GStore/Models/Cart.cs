using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GStore.Data;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("Cart")]
	public class Cart : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Cart Id", Description="Internal Id number to identity unique carts.")]
		public int CartId { get; set; }

		[Required]
		[Display(Name = "Session Id", Description="Web Server Session Id to uniquely identity a user visit.")]
		[MaxLength(100)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public string SessionId { get; set; }

		[Display(Name = "User Profile Id", Description="User Profile id if user is logged in.")]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		public int? UserProfileId { get; set; }

		[Display(Name = "Order Id", Description = "Order Id number if this cart converted to an order.")]
		[Index("UniqueRecord", IsUnique = true, Order = 5)]
		public int? OrderId { get; set; }

		[Display(Name = "Order", Description = "Order if this cart converted to an order.")]
		[ForeignKey("OrderId")]
		public virtual Order Order { get; set; }

		[Display(Name = "User Profile", Description="User Profile if user is logged in.")]
		[ForeignKey("UserProfileId")]
		public virtual UserProfile UserProfile { get; set; }

		[Display(Name = "Email Address", Description = "Email Address entered at checkout or user's email if logged in.")]
		public string Email { get; set; }

		#region Tracking Fields
		[Required]
		[Display(Name = "User Agent", Description="User browser information")]
		public string UserAgent { get; set; }

		[Required]
		[Display(Name = "IP Address", Description="Internet network address for the user.")]
		[MaxLength(15)]
		public string IPAddress { get; set; }

		[Display(Name = "Session Start Url", Description="The first URL that the user went to on the site.\nAlso known as a Landing Url")]
		public string EntryUrl { get; set; }

		[Display(Name = "Session Start Raw Url", Description="Raw Url of first URL that the user went to on the site.\nAlso known as a Raw Landing Url")]
		public string EntryRawUrl { get; set; }

		[Display(Name = "Session Referrer", Description="Site or URL that the user was on when they clicked a link to visit this site.\nTells you what site referred the user here.")]
		public string EntryReferrer { get; set; }

		[Display(Name = "Session Start", Description="Date and Time that the user first landed on the site for this session.\nThis tells you how long they stayed before they added items to their cart.")]
		public DateTime EntryDateTimeUtc { get; set; }

		#endregion

		[Display(Name = "Discount Codes Attempted", Description="List of discount codes the user attempted to use, whether they were valid or not.")]
		public string DiscountCodesAttempted { get; set; }

		[Display(Name = "Discount Codes Failed", Description="List of failed discount codes the user tried to enter and was declined.")]
		public string DiscountCodesFailed { get; set; }

		[Display(Name = "Discount Code", Description="Valid discount code user applied to this order.\nCan also come from links where DiscountCode=[code] is set in the link querystring.")]
		public string DiscountCode { get; set; }

		[Display(Name = "Discount Id", Description="Discount Id to apply to this order.")]
		public int? DiscountId { get; set; }

		[Display(Name = "Discount", Description="Discount to apply to this order.")]
		[ForeignKey("DiscountId")]
		public virtual Discount Discount { get; set; }

		[Range(0, 1000000)]
		[Display(Name = "Item Count", Description="Total number of items in the cart.\nThis counts individual items.")]
		public int ItemCount { get; set; }

		[Range(0, 1000000)]
		[Display(Name = "Sub-Total", Description="Order Sub-total before tax, shipping, handling, and order discount.")]
		public decimal Subtotal { get; set; }

		[Range(0, 1000000)]
		[Display(Name = "Tax", Description="Tax for this order.")]
		public decimal Tax { get; set; }

		[Range(0, 1000000)]
		[Display(Name = "Shipping", Description="Shipping cost for this order.")]
		public decimal Shipping { get; set; }

		[Range(0, 1000000)]
		[Display(Name = "Handling", Description="Handling cost for this order.")]
		public decimal Handling { get; set; }

		[Range(0, 1000000)]
		[Display(Name = "Order Discount", Description="Order discount for this order.")]
		public decimal OrderDiscount { get; set; }

		[Range(0, 1000000)]
		[Display(Name = "Cart Total", Description="Order grand total. This is the amount that will be charged.")]
		public decimal Total { get; set; }

		public bool AllItemsAreDigitalDownload { get; set; }
		public int DigitalDownloadItemCount { get; set; }
		public int ShippingItemCount { get; set; }

		public int? DeliveryInfoDigitalId { get; set; }

		[ForeignKey("DeliveryInfoDigitalId")]
		public virtual DeliveryInfoDigital DeliveryInfoDigital { get; set; }

		public int? DeliveryInfoShippingId { get; set; }

		[ForeignKey("DeliveryInfoShippingId")]
		public virtual DeliveryInfoShipping DeliveryInfoShipping { get; set; }

		public int? PaymentId { get; set; }

		[ForeignKey("PaymentId")]
		public virtual Payment Payment { get; set; }



		#region Status fields

		[Display(Name = "Emailed Contents", Description="User has emailed the cart contents to a friend.")]
		public bool StatusEmailedContents { get; set; }

		[Display(Name = "Started Checkout", Description="User clicked the checkout link and started the checkout process.")]
		public bool StatusStartedCheckout { get; set; }

		[Display(Name = "Selected LogIn or Guest", Description = "User elected to log in or check out as a guest.")]
		public bool StatusSelectedLogInOrGuest { get; set; }

		[Display(Name = "Completed Delivery/Shipping Info", Description = "User entered shipping/delivery information.")]
		public bool StatusCompletedDeliveryInfo { get; set; }

		[Display(Name = "Selected Delivery Method", Description="User selected a delivery method.")]
		public bool StatusSelectedDeliveryMethod { get; set; }

		[Display(Name = "Completed Payment Info", Description="User entered valid payment information.")]
		public bool StatusEnteredPaymentInfo { get; set; }

		[Display(Name = "Placed Order", Description="")]
		public bool StatusPlacedOrder { get; set; }

		[Display(Name = "Printed Confirmation", Description="User printed the order confirmation page.")]
		public bool StatusPrintedConfirmation { get; set; }

		[Display(Name = "Emailed Confirmation", Description="User emailed the order confirmation page to a friend.")]
		public bool StatusEmailedConfirmation { get; set; }

		#endregion

		[Display(Name = "Cart Items", Description="Products/Items in the cart.")]
		public virtual ICollection<CartItem> CartItems { get; set; }

	}
}