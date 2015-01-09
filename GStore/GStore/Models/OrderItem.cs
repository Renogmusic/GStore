using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GStore.Data;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("OrderItem")]
	public class OrderItem : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Order Item Id")]
		public int OrderItemId { get; set; }

		[Required]
		[Display(Name = "Cart Item Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int CartItemId { get; set; }

		[Required]
		public int OrderId { get; set; }

		[Required]
		public int ProductId { get; set; }

		[ForeignKey("CartItemId")]
		public virtual CartItem CartItem { get; set; }

		[ForeignKey("OrderId")]
		public virtual Order Order { get; set; }

		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }


		[Range(1, 999)]
		public int Quantity { get; set; }

		[Required]
		[Range(0, 1000000)]
		public decimal UnitPrice { get; set; }

		[Required]
		[Range(0, 1000000)]
		public decimal ListPrice { get; set; }

		[Required]
		[Range(0, 1000000)]
		public decimal UnitPriceExt { get; set; }

		[Required]
		[Range(0, 1000000)]
		public decimal ListPriceExt { get; set; }

		[Required]
		[Range(0, 1000000)]
		public decimal LineDiscount { get; set; }

		[Required]
		[Range(0, 1000000)]
		public decimal LineTotal { get; set; }

		[Display(Name = "Refunded Amount", Description = "Amount refunded to the user.")]
		public decimal ItemRefundedAmount { get; set; }

		public string ProductVariantInfo { get; set; }

		public bool StatusItemEditedByUser { get; set; }

		public bool StatusItemEditedByMerchant { get; set; }

		public bool StatusItemCancelledByUser { get; set; }

		public bool StatusItemCancelledByMerchant { get; set; }

		[Display(Name = "Item Accepted", Description = "Item in order has been accepted by the merchant.")]
		public bool StatusItemAccepted { get; set; }

		[Display(Name = "Item Payment Received", Description = "Payment has been received for this item.")]
		public bool StatusItemPaymentReceived { get; set; }

		[Display(Name = "Item Shipped", Description = "This item has been shipped.")]
		public bool StatusItemShipped { get; set; }

		[Display(Name = "Item Delivered", Description = "This item has been delivered.")]
		public bool StatusItemDelivered { get; set; }

		[Display(Name = "Item Feedback Received", Description = "Received feedback on this item from the user.")]
		public bool StatusItemFeedbackReceived { get; set; }

		[Display(Name = "Item Returned", Description = "User returned this item for a refund.")]
		public bool StatusItemReturned { get; set; }

	}
}