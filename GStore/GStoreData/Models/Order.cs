using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	[Table("Order")]
	public class Order : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Order Id", Description="Internal Id number to identify unique Orders.")]
		public int OrderId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Order Number", Description = "Order Number for user reference and lookups.")]
		[MaxLength(50)]
		public string OrderNumber { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email Address", Description = "Email Address for this order.")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Full Name", Description = "Full Name for this order.")]
		public string FullName { get; set; }

		[Display(Name = "Original Cart Id", Description = "Cart Id this order came from.")]
		public int? OriginalCartId { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Session Id", Description="Web Server Session Id to uniquely identity a user visit.")]
		public string SessionId { get; set; }

		[Display(Name = "User Profile Id", Description="User Profile id if user was logged in.")]
		public int? UserProfileId { get; set; }

		[ForeignKey("UserProfileId")]
		[Display(Name = "User Profile", Description="User Profile if user was logged in.")]
		public virtual UserProfile UserProfile { get; set; }

		public int? DeliveryInfoDigitalId { get; set; }

		public int? DeliveryInfoShippingId { get; set; }

		[ForeignKey("DeliveryInfoDigitalId")]
		public virtual DeliveryInfoDigital DeliveryInfoDigital { get; set; }

		[ForeignKey("DeliveryInfoShippingId")]
		public virtual DeliveryInfoShipping DeliveryInfoShipping { get; set; }

		public bool AllItemsAreDigitalDownload { get; set; }

		public int DigitalDownloadItemCount { get; set; }

		public int ShippingItemCount { get; set; }

		#region Tracking Fields
		[Required]
		[Display(Name = "User Agent", Description="User browser information")]
		public string UserAgent { get; set; }

		[Required]
		[MaxLength(15)]
		[Display(Name = "IP Address", Description="Internet network address for the user.")]
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
		[Display(Name = "Item Count", Description="Total number of items in the Order.\nThis counts individual items.")]
		public int ItemCount { get; set; }

		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		[Display(Name = "Sub-Total", Description="Order Sub-total before tax, shipping, handling, and order discount.")]
		public decimal Subtotal { get; set; }

		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		[Display(Name = "Tax", Description="Tax for this order.")]
		public decimal Tax { get; set; }

		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		[Display(Name = "Shipping", Description="Shipping cost for this order.")]
		public decimal Shipping { get; set; }

		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		[Display(Name = "Handling", Description="Handling cost for this order.")]
		public decimal Handling { get; set; }

		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		[Display(Name = "Order Discount", Description="Order discount for this order.")]
		public decimal OrderDiscount { get; set; }

		[DataType(DataType.Currency)]
		[Range(0, 1000000)]
		[Display(Name = "Order Total", Description="Order grand total. This is the amount that will be charged.")]
		public decimal Total { get; set; }

		[DataType(DataType.Currency)]
		[Display(Name = "Refunded Amount", Description = "Amount refunded to the user.")]
		public decimal RefundedAmount { get; set; }


		#region Status fields

		public bool StatusOrderEditedByUser { get; set; }

		public bool StatusOrderEditedByMerchant { get; set; }

		public bool StatusOrderCancelledByUser { get; set; }

		public bool StatusOrderCancelledByMerchant { get; set; }

		[Display(Name = "Order Accepted", Description = "Order has been accepted by the merchant.")]
		public bool StatusOrderAccepted { get; set; }

		[Display(Name = "Order Payment Processed", Description = "Payment has been processed.")]
		public bool StatusOrderPaymentProcessed { get; set; }

		[Display(Name = "Order Shipped", Description = "Order has been shipped.")]
		public bool StatusOrderShipped { get; set; }

		[Display(Name = "Order Delivered", Description="Order has been delivered.")]
		public bool StatusOrderDelivered { get; set; }

		[Display(Name = "Order Downloaded", Description = "Order has been downloaded.")]
		public bool StatusOrderDownloaded { get; set; }

		[Display(Name = "Feedback Received", Description = "Received feedback from the user.")]
		public bool StatusOrderFeedbackReceived { get; set; }

		[Display(Name = "Order Items Returned", Description = "User returned one or more items for a refund.")]
		public bool StatusOrderItemsReturned { get; set; }

		#endregion

		public virtual ICollection<Cart> Carts { get; set; }

		public virtual ICollection<Notification> Notifications { get; set; }

		[Display(Name = "Payments", Description = "Payments processed for this order.")]
		public virtual ICollection<Payment> Payments { get; set; }

		[Display(Name = "Order Bundles", Description = "Bundles of Products/Items in the Order.")]
		public virtual ICollection<OrderBundle> OrderBundles { get; set; }

		[Display(Name = "Order Items", Description = "Products/Items in the Order.")]
		public virtual ICollection<OrderItem> OrderItems { get; set; }

		public int? LoginOrGuestWebFormResponseId { get; set; }

		[ForeignKey("LoginOrGuestWebFormResponseId")]
		public virtual WebFormResponse LoginOrGuestWebFormResponse { get; set; }

		public int? PaymentInfoWebFormResponseId { get; set; }

		[ForeignKey("PaymentInfoWebFormResponseId")]
		public virtual WebFormResponse PaymentInfoWebFormResponse { get; set; }

		public int? ConfirmOrderWebFormResponseId { get; set; }

		[ForeignKey("ConfirmOrderWebFormResponseId")]
		public virtual WebFormResponse ConfirmOrderWebFormResponse { get; set; }
	}
}