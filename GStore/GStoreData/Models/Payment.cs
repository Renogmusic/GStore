using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using GStoreData.Models.BaseClasses;

namespace GStoreData.Models
{
	[Table("Payment")]
	public class Payment : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Payment Id")]
		public int PaymentId { get; set; }

		[Display(Name = "Order Id")]
		public int? OrderId { get; set; }

		[ForeignKey("OrderId")]
		[Display(Name = "Order Id")]
		public virtual Order Order { get; set; }

		[Required]
		[Display(Name = "Cart Id")]
		public int? CartId { get; set; }

		[ForeignKey("CartId")]
		[Display(Name = "Cart Id")]
		public virtual Cart Cart { get; set; }

		/// <summary>
		/// Amount of payment (total)
		/// </summary>
		[Display(Name = "Amount Paid", Description="Amount that was paid for this order.")]
		public decimal AmountPaid { get; set; }

		[Display(Name = "Payment Source")]
		public GStorePaymentSourceEnum PaymentSource { get; set; }

		/// <summary>
		/// Payment was processed successfully
		/// </summary>
		[Display(Name = "Payment Is Processed", Description="Payment was processed successfully")]
		public bool IsProcessed { get; set; }

		[Display(Name = "Payment Is Processed")]
		public DateTime? ProcessDateTimeUtc { get; set; }

		[Display(Name = "Transaction Id")]
		public string TransactionId { get; set; }

		[Display(Name = "JSON Payment Data")]
		public string Json { get; set; }

		/// <summary>
		/// Payment failed. See FailureException for details
		/// </summary>
		[Display(Name = "Payment Failed", Description="Payment failed. See FailureException for details.")]
		public bool PaymentFailed { get; set; }

		[Display(Name = "Failure Exception")]
		public string FailureException { get; set; }

		[Display(Name = "PayPal Payment Id", Description = "PayPal Payment Resource from PayPal payment (response.id)")]
		public string PayPalPaymentResource { get; set; }

		[Display(Name = "PayPal Payment State", Description = "PayPal Payment State from PayPal payment (response.state")]
		public string PayPalState { get; set; }

		[Display(Name = "PayPal Payment Intent", Description = "PayPal Payment Intent from PayPal payment (response.intent")]
		public string PayPalIntent { get; set; }

		[Display(Name = "PayPal Payment Create Time", Description = "Time payment was started from PayPal payment (response.create_time")]
		public string PayPalCreateTime { get; set; }

		[Display(Name = "PayPal Payment Update Time", Description = "Time payment was completed from PayPal payment (response.update_time")]
		public string PayPalUpdateTime { get; set; }

		[Display(Name = "PayPal Payment Method", Description = "Payment method from PayPal payment (response.payer.payment_method")]
		public string PayPalPaymentMethod { get; set; }

		[Display(Name = "PayPal Payment Is a Direct Credit Card payment", Description = "Checked if this is a PayPal direct credit card payment from PayPal payment (response.payer.funding_instruments.Any()")]
		public bool PayPalIsDirectCreditCardPayment { get; set; }

		[Display(Name = "PayPal Direct Credit Card Number", Description = "Credit card number for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.number")]
		public string PayPalDirectCreditCardNumber { get; set; }

		[Display(Name = "PayPal Direct Credit Card Type", Description = "Credit card type for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.type")]
		public string PayPalDirectCreditCardType { get; set; }

		[Display(Name = "PayPal Direct Credit Card Expire Month", Description = "Expiration month for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.expire_month")]
		public string PayPalDirectCreditCardExpireMonth { get; set; }

		[Display(Name = "PayPal Direct Credit Card Expire Year", Description = "Expiration year for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.expire_year")]
		public string PayPalDirectCreditCardExpireYear { get; set; }

		[Display(Name = "PayPal Direct Credit Card First Name", Description = "First Name for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.first_name")]
		public string PayPalDirectCreditCardFirstName { get; set; }

		[Display(Name = "PayPal Direct Credit Card First Name", Description = "Last Name for direct credit card payment from PayPal payment (response.payer.funding_instruments[0].credit_card.last_name")]
		public string PayPalDirectCreditCardLastName { get; set; }

		[Display(Name = "PayPal Payer Email", Description = "Email address entered from PayPal payment (response.payer.payer_info.email")]
		public string PayPalPayerEmail { get; set; }

		[Display(Name = "PayPal Payer First Name", Description = "first name entered from PayPal payment (response.payer.payer_info.first_name")]
		public string PayPalPayerFirstName { get; set; }

		[Display(Name = "PayPal Payer Last Name", Description = "last name entered from PayPal payment (response.payer.payer_info.last_name")]
		public string PayPalPayerLastName { get; set; }

		[Display(Name = "PayPal Payer Id", Description = "Payer Id from PayPal payment (response.payer.payer_info.payer_id")]
		public string PayPalPayerId { get; set; }

		[Display(Name = "PayPal Payment Resource API Link", Description = "Payment API Link from PayPal payment (response.links.[rel=self].href")]
		public string PayPalPaymentResourceLink { get; set; }

		[Display(Name = "PayPal Transaction Total", Description = "Transaction Total from PayPal payment (response.transactions.[0].amount.total")]
		public string PayPalTransactionTotal { get; set; }

		[Display(Name = "PayPal Transaction Currency", Description = "Transaction currency from PayPal payment (response.transactions.[0].amount.currency")]
		public string PayPalTransactionCurrency { get; set; }

		[Display(Name = "PayPal Transaction Description", Description = "Transaction description from PayPal payment (response.transactions.[0].description")]
		public string PayPalTransactionDescription { get; set; }

		[Display(Name = "PayPal Sale Transaction Id", Description = "PayPal Transaction Id from PayPal payment (response.transactions.[0].related_resources.[0].sale.id")]
		public string PayPalSaleId { get; set; }

		[Display(Name = "PayPal Sale Create Time", Description = "Time sale was started from PayPal payment (response.transactions.[0].related_resources.[0].sale.create_time")]
		public string PayPalSaleCreateTime { get; set; }

		[Display(Name = "PayPal Sale Update Time", Description = "Time sale was completed from PayPal payment (response.transactions.[0].related_resources.[0].sale.update_time")]
		public string PayPalSaleUpdateTime { get; set; }

		[Display(Name = "PayPal Sale Total", Description = "PayPal Sale Total from PayPal payment (response.transactions.[0].related_resources.[0].sale.amount.total")]
		public string PayPalSaleAmountTotal { get; set; }

		[Display(Name = "PayPal Sale Currency", Description = "PayPal Sale Currency from PayPal payment (response.transactions.[0].related_resources.[0].sale.amount.currency")]
		public string PayPalSaleAmountCurrency { get; set; }

		[Display(Name = "PayPal Sale Payment Mode", Description = "PayPal Sale Payment Mode from PayPal payment (response.transactions.[0].related_resources.[0].sale.payment_mode")]
		public string PayPalSalePaymentMode { get; set; }

		[Display(Name = "PayPal Sale Status", Description = "PayPal Sale status from PayPal payment (response.transactions.[0].related_resources.[0].sale.state")]
		public string PayPalSaleState { get; set; }

		[Display(Name = "PayPal Sale Protection Eligibility", Description = "PayPal Sale protection eligibility from PayPal payment (response.transactions.[0].related_resources.[0].sale.protection_eligibility")]
		public string PayPalSaleProtectionEligibility { get; set; }

		[Display(Name = "PayPal Sale Protection Eligibility Type", Description = "PayPal Sale protection eligibility type from PayPal payment (response.transactions.[0].related_resources.[0].sale.protection_eligibility_type")]
		public string PayPalSaleProtectionEligibilityType { get; set; }

		[Display(Name = "PayPal Sale Transaction Fee", Description = "PayPal Sale transaction fee from PayPal payment (response.transactions.[0].related_resources.[0].sale.transaction_fee.value")]
		public string PayPalSaleTransactionFeeValue { get; set; }

		[Display(Name = "PayPal Sale Transaction Fee Currency", Description = "PayPal Sale transaction fee currency from PayPal payment (response.transactions.[0].related_resources.[0].sale.transaction_fee.currency")]
		public string PayPalSaleTransactionFeeCurrency { get; set; }

		[Display(Name = "PayPal Sale API Link to Sale", Description = "PayPal API Link to sale from PayPal payment (response.transactions.[0].related_resources.[0].sale.links.[rel=self]")]
		public string PayPalSaleAPILinkToSelf { get; set; }

		[Display(Name = "PayPal Sale API Link to Refund", Description = "PayPal API Link to refund from PayPal payment (response.transactions.[0].related_resources.[0].sale.links.[rel=refund]")]
		public string PayPalSaleAPILinkToRefund { get; set; }

		[Display(Name = "PayPal Sale API Link to Refund", Description = "PayPal API Link to refund from PayPal payment (response.transactions.[0].related_resources.[0].sale.links.[rel=parent_payment]")]
		public string PayPalSaleAPILinkToParentPayment { get; set; }

		[Display(Name = "PayPal Shipping Address Recipient Name", Description = "Shipping recipient name from PayPal payment (response.transactions.[0].item_list.shipping_address.recipient_name")]
		public string PayPalShippingAddressRecipientName { get; set; }

		[Display(Name = "PayPal Shipping Address Line 1", Description = "Shipping address line 1 from PayPal payment (response.transactions.[0].item_list.shipping_address.line1")]
		public string PayPalShippingAddressLine1 { get; set; }

		[Display(Name = "PayPal Shipping Address Line 2", Description = "Shipping address line 2 from PayPal payment (response.transactions.[0].item_list.shipping_address.line2")]
		public string PayPalShippingAddressLine2 { get; set; }

		[Display(Name = "PayPal Shipping Address City", Description = "Shipping City from PayPal payment (response.transactions.[0].item_list.shipping_address.city")]
		public string PayPalShippingAddressCity { get; set; }

		[Display(Name = "PayPal Shipping Address State", Description = "Shipping State from PayPal payment (response.transactions.[0].item_list.shipping_address.state")]
		public string PayPalShippingAddressState { get; set; }

		[Display(Name = "PayPal Shipping Address Postal Code", Description = "Shipping (ZIP) Postal code from PayPal payment (response.transactions.[0].item_list.shipping_address.postal_code")]
		public string PayPalShippingAddressPostalCode { get; set; }

		[Display(Name = "PayPal Shipping Country Code", Description = "Shipping Country Code from PayPal payment (response.transactions.[0].item_list.shipping_address.country_code")]
		public string PayPalShippingAddressCountryCode { get; set; }

		[Display(Name = "PayPal Shipping Address Recipient Name", Description = "Shipping recipient name from PayPal payment (response.payer.payer_info.shipping_address.recipient_name")]
		public string PayPalPayerShippingAddressRecipientName { get; set; }

		[Display(Name = "PayPal Shipping Address Line 1", Description = "Shipping address line 1 from PayPal payment (response.payer.payer_info.shipping_address.line1")]
		public string PayPalPayerShippingAddressLine1 { get; set; }

		[Display(Name = "PayPal Shipping Address Line 2", Description = "Shipping address line 2 from PayPal payment (response.payer.payer_info.shipping_address.line2")]
		public string PayPalPayerShippingAddressLine2 { get; set; }

		[Display(Name = "PayPal Shipping Address City", Description = "Shipping City from PayPal payment (response.payer.payer_info.shipping_address.city")]
		public string PayPalPayerShippingAddressCity { get; set; }

		[Display(Name = "PayPal Shipping Address State", Description = "Shipping State from PayPal payment (response.payer.payer_info.shipping_address.state")]
		public string PayPalPayerShippingAddressState { get; set; }

		[Display(Name = "PayPal Shipping Address Postal Code", Description = "Shipping (ZIP) Postal code from PayPal payment (response.payer.payer_info.shipping_address.postal_code")]
		public string PayPalPayerShippingAddressPostalCode { get; set; }

		[Display(Name = "PayPal Shipping Country Code", Description = "Shipping Country Code from PayPal payment (response.payer.payer_info.shipping_address.country_code")]
		public string PayPalPayerShippingAddressCountryCode { get; set; }

	}
}