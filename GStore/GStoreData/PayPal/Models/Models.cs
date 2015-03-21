using System;

namespace GStoreData.PayPal.Models
{
	//For reference see PayPal Rest API documentation
	//https://developer.paypal.com/docs/api/

	/// <summary>
	/// PayPal Oauth token data
	/// </summary>
	public struct PayPalOAuthTokenData
	{
		public string scope;

		/// <summary>
		/// OAuth token string
		/// </summary>
		public string access_token;
		public string token_type;
		public string app_id;
		public long expires_in;

		/// <summary>
		/// JSON from Oauth request
		/// </summary>
		[NonSerialized]
		public string Json;
	}

	/// <summary>
	/// payment object
	/// This object represents a payer’s funding instrument, such as a credit card or token that represents a credit card.
	/// </summary>
	public struct PayPalPaymentData
	{
		/// <summary>
		/// Payment intent. Must be set to sale for immediate payment, authorize to authorize a payment for capture later, or order to create an order. Required
		/// </summary>
		public string intent;

		/// <summary>
		/// Source of the funds for this payment represented by a PayPal account or a direct credit card. Required.
		/// </summary>
		public PayPalPayerData payer;

		/// <summary>
		/// Transactional details including the amount and item details. Required.
		/// </summary>
		public PayPalTransactionData[] transactions;

		/// <summary>
		/// Set of redirect URLs you provide only for PayPal-based payments. Required for PayPal payments.
		/// </summary>
		public PayPalRedirectUrlData redirect_urls;

		/// <summary>
		/// id string ID of the created payment Value assigned by PayPal. 
		/// </summary>
		public string id;

		/// <summary>
		/// create_time date_time Payment creation time as defined in RFC 3339 Section 5.6. Value assigned by PayPal 
		/// </summary>
		public string create_time;

		/// <summary>
		/// update_time date_time Time that the resource was last updated. Value assigned by PayPal.  
		/// </summary>
		public string update_time;

		/// <summary>
		/// Identifier for the payment experience.
		/// </summary>
		public string experience_profile_id;

		/// <summary>
		/// Payment state. Must be set to one of the one of the following: created; approved; failed; canceled; expired; pending. Value assigned by PayPal.
		/// </summary>
		public string state;

		public PayPalLinkData[] links;

		/// <summary>
		/// JSON of this object in full from PayPal
		/// </summary>
		[NonSerialized]
		public string Json;
	}

	/// <summary>
	/// payment_execution object 
	/// Important: This call only works after a buyer has approved the payment using the provided PayPal approval URL. For more information, learn how to accept a PayPal payment.
	/// Pass the payment id in the endpoint and include updated details as a payment_execution object in the body.
	/// </summary>
	public struct PayPalPaymentExecuteData
	{
		public PayPalPaymentExecuteData(string payer_id)
		{
			this.payer_id = payer_id;
			this.transactions = null;
		}

		/// <summary>
		/// The ID of the Payer, passed in the return_url by PayPal. Required.
		/// </summary>
		public string payer_id;

		/// <summary>
		/// Transactional details if updating a payment. Note that this instance of the transactions object accepts only the amount object.
		/// </summary>
		public PayPalTransactionData[] transactions;
	}

	/// <summary>
	/// PayPal HATEOAS link (action links)
	/// </summary>
	public struct PayPalLinkData
	{
		/// <summary>
		/// URL of the related HATEOAS link you can use for subsequent calls.
		/// </summary>
		public string href;

		/// <summary>
		/// Link relation that describes how this link relates to the previous call. Examples include self (get details of the current call), parent_payment (get details of the parent payment), or a related call such as execute or refund.
		/// </summary>
		public string rel;

		/// <summary>
		/// The HTTP method required for the related call.
		/// </summary>
		public string method;
	}
	
	/// <summary>
	/// redirect_urls object.  This object represents a set of redirect URLs you provide to PayPal for PayPal account payments
	/// </summary>
	public struct PayPalRedirectUrlData
	{
		/// <summary>
		/// The payer is redirected to this URL after approving the payment. Required for PayPal account payments.
		/// </summary>
		public string return_url;

		/// <summary>
		/// The payer is redirected to this URL after canceling the payment. Required for PayPal account payments.
		/// </summary>
		public string cancel_url;
	}
	
	/// <summary>
	/// payer object
	/// This object includes information about the payer including payment method, funding instruments, and details about the payer.
	/// </summary>
	public struct PayPalPayerData
	{
		/// <summary>
		/// Payment method used. Must be either credit_card or paypal. Required.
		/// </summary>
		public string payment_method;

		/// <summary>
		/// A list of funding instruments for the current payment
		/// </summary>
		public PayPalFundingInstrumentData[] funding_instruments;

		/// <summary>
		/// PayPal fills this field after executing payment.
		/// Information related to the payer.
		/// </summary>
		public PayPalPayerInfoData? payer_info;

		/// <summary>
		/// Status of the payer’s PayPal account. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. Allowed values: VERIFIED or UNVERIFIED.
		/// </summary>
		public string status;
	}

	/// <summary>
	/// payer_info object
	/// This object is pre-filled by PayPal when the payment_method is paypal.
	/// </summary>
	public struct PayPalPayerInfoData
	{
		/// <summary>
		/// Email address representing the payer. 127 characters max.
		/// </summary>
		public string email;
		
		/// <summary>
		/// First name of the payer. Value assigned by PayPal.
		/// </summary>
		public string first_name;
		
		/// <summary>
		///  Last name of the payer. Value assigned by PayPal. 
		/// </summary>
		public string last_name;
		
		/// <summary>
		/// PayPal assigned Payer ID. Value assigned by PayPal. 
		/// </summary>
		public string payer_id;
		
		/// <summary>
		/// Phone number representing the payer. 20 characters max.
		/// </summary>
		public string phone;

		/// <summary>
		/// Shipping address of payer PayPal account. Value assigned by PayPal. 
		/// </summary>
		public PayPalShippingAddressData? shipping_address;

		/// <summary>
		/// Payer’s tax ID type. Allowed values: BR_CPF or BR_CNPJ. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date.
		/// </summary>
		public string tax_id_type;
		
		/// <summary>
		/// Payer’s tax ID. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date.
		/// </summary>
		public string tax_id;
	}

	/// <summary>
	/// funding_instrument object
	/// This object represents a payer’s funding instrument (credit card).
	/// </summary>
	public struct PayPalFundingInstrumentData
	{
		/// <summary>
		/// Credit card details. Required if creating a funding instrument. 
		/// </summary>
		public PayPalCreditCardData credit_card;

		/// <summary>
		/// Token for credit card details stored with PayPal. You can use this in place of a credit card. Required if not passing credit card details. 
		/// </summary>
		//public PayPalCreditCardToken credit_card_token;
	}

	/// <summary>
	/// credit_card object
	/// This object represents a payer’s funding instrument, such as a credit card or token that represents a credit card.
	/// </summary>
	public struct PayPalCreditCardData
	{
		/// <summary>
		/// ID of the credit card. This ID is provided in the response when storing credit cards. Required if using a stored credit card
		/// </summary>
		public string id;

		/// <summary>
		/// A unique identifier that you can assign and track when storing a credit card or using a stored credit card. This ID can help to avoid unintentional use or misuse of credit cards. This ID can be any value you would like to associate with the saved card, such as a UUID, username, or email address. This is being deprecated in favor of the external_customer_id property.
		/// </summary>
		//public string payer_id;

		/// <summary>
		/// Credit card number. Numeric characters only with no spaces or punctuation. The string must conform with modulo and length required by each credit card type. Redacted in responses. Required. 
		/// </summary>
		public string number;

		/// <summary>
		/// Credit card type. Valid types are: visa, mastercard, discover, amex Required.
		/// </summary>
		public string type;
		
		/// <summary>
		/// Expiration month with no leading zero. Acceptable values are 1 through 12. Required.
		/// </summary>
		public string expire_month;
		
		/// <summary>
		/// 4-digit expiration year. Required.
		/// </summary>
		public string expire_year;

		/// <summary>
		/// 3-4 digit card validation code.
		/// </summary>
		public string ccv2;

		/// <summary>
		/// Cardholder’s first name.
		/// </summary>
		public string first_name;
		
		/// <summary>
		/// Cardholder’s last name.
		/// </summary>
		public string last_name;

		/// <summary>
		/// Billing address associated with card
		/// </summary>
		public PayPalAddressData? billing_address;

		/// <summary>
		/// A unique identifier of the customer to whom this bank account belongs. Generated and provided by the facilitator. This is now used in favor of payer_id when creating or using a stored funding instrument in the vault. 
		/// </summary>
		public string external_customer_id;

		/// <summary>
		/// A user-provided, optional field that functions as a unique identifier for the merchant holding the card. Note that this has no relation to PayPal merchant id
		/// </summary>
		public string merchant_id;

		/// <summary>
		/// A unique identifier of the bank account resource. Generated and provided by the facilitator so it can be used to restrict the usage of the bank account to the specific merchant.
		/// </summary>
		public string external_card_id;

		/// <summary>
		/// Resource creation time in ISO8601 date-time format (ex: 1994-11-05T13:15:30Z).
		/// </summary>
		public string create_time;

		/// <summary>
		/// Resource update time in ISO8601 date-time format (ex: 1994-11-05T13:15:30Z).
		/// </summary>
		public string update_time;

		/// <summary>
		/// State of the credit card funding instrument: expired or ok. Value assigned by PayPal. 
		/// </summary>
		public string state;

		/// <summary>
		/// Funding instrument expiration date. Value assigned by PayPal. 
		/// </summary>
		public string valid_until;

	}

	/// <summary>
	/// address object
	/// Base Address object used as billing address in a payment or extended for Shipping Address.
	/// </summary>
	public struct PayPalAddressData
	{
		/// <summary>
		/// line1 string Line 1 of the Address (eg. number, street, etc). Required.  
		/// </summary>
		public string line1;

		/// <summary>
		/// line2 string Optional line 2 of the Address (eg. suite, apt #, etc.). 
		/// </summary>
		public string line2;

		/// <summary>
		/// city string City name. Required.  
		/// </summary>
		public string city;
		
		/// <summary>
		/// country_code string 2 letter country code. Required.  
		/// </summary>
		public string country_code;
		
		/// <summary>
		/// state string 2 letter code for US states, and the equivalent for other countries. 
		/// </summary>
		public string state;
		
		/// <summary>
		/// postal_code string Zip code or equivalent is usually required for countries that have them. For list of countries that do not have postal codes please refer to http://en.wikipedia.org/wiki/Postal_code
		/// </summary>
		public string postal_code;
		
		/// <summary>
		/// phone string Phone number in E.123 format. 
		/// </summary>
		public string phone;
	}

	public struct PayPalShippingAddressData
	{
		/// <summary>
		/// Name of the recipient at this address. 50 characters max. Required
		/// </summary>
		public string recipient_name;

		/// <summary>
		/// Address type. Must be one of the following: residential, business, or mailbox. 
		/// </summary>
		public string type;

		/// <summary>
		/// Line 1 of the address (e.g., Number, street, etc). 100 characters max. Required.
		/// </summary>
		public string line1;

		/// <summary>
		/// Line 2 of the address (e.g., Suite, apt #, etc). 100 characters max.
		/// </summary>
		public string line2;

		/// <summary>
		/// City name. 50 characters max. Required. 
		/// </summary>
		public string city;

		/// <summary>
		///  2 characters max. Required.
		/// </summary>
		public string country_code;

		/// <summary>
		/// Zip code or equivalent is usually required for countries that have them. 20 characters max. Required in certain countries.
		/// </summary>
		public string postal_code;

		/// <summary>
		/// 2-letter code for US states, and the equivalent for other countries. 100 characters max
		/// </summary>
		public string state;


		/// <summary>
		/// Phone number in E.123 format. 50 characters max.
		/// </summary>
		public string phone;
	}

	/// <summary>
	/// transaction object
	/// This object provides payment transactions details.
	/// </summary>
	public struct PayPalTransactionData
	{
		public PayPalTransactionData(decimal total, string description, PayPalItemData[] items)
		{
			this.description = description;
			this.amount.currency = "USD";
			this.amount.total = total.ToString("N2");
			this.amount.details = null;
			this.related_resources = null;
			this.soft_descriptor = null;
			this.invoice_number = null;
			this.custom = null;
			if (items == null)
			{
				this.item_list = null;
			}
			else
			{

				this.item_list = new PayPalItemListData(items);
			}
		}

		/// <summary>
		/// Amount being collected . Required.
		/// </summary>
		public PayPalAmountData amount;
		
		/// <summary>
		/// Description of transaction. 127 characters max.
		/// </summary>
		public string description;
		
		/// <summary>
		/// Items and related shipping address within a transaction.
		/// </summary>
		public PayPalItemListData? item_list;
		
		/// <summary>
		/// Financial transactions related to a payment.
		/// array of sale, authorization, capture, or refund, objects Financial transactions related to a payment
		/// </summary>
		public PayPalTransactionRelatedResourceData[] related_resources;
		
		/// <summary>
		/// Invoice number used to track the payment. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. 256 characters max.
		/// </summary>
		public string invoice_number;

		/// <summary>
		/// Free-form field for the use of clients. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. 256 characters max.
		/// </summary>
		public string custom;

		/// <summary>
		/// Soft descriptor used when charging this funding source. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. 22 characters max.
		/// </summary>
		public string soft_descriptor;

		/// <summary>
		/// Payment options requested for this purchase unit
		/// Optional payment method type. If specified, the transaction will go through for only instant payment. Allowed values: INSTANT_FUNDING_SOURCE. Only for use with the paypal payment_method, not relevant for the credit_card payment_method.
		/// </summary>
		//public PayPalPaymentOptionsData payment_options;

	}
	
	/// <summary>
	/// amount object
	/// </summary>
	public struct PayPalAmountData
	{
		/// <summary>
		/// 3-letter currency code. PayPal does not support all currencies. Required.
		/// </summary>
		public string currency;

		/// <summary>
		/// Total amount charged from the payer to the payee. In case of a refund, this is the refunded amount to the original payer from the payee. 10 characters max with support for 2 decimal places. Required. 
		/// </summary>
		public string total;

		/// <summary>
		/// Additional details related to a payment amount.
		/// </summary>
		public PayPalAmountDetailsData? details;

		public decimal ToDecimal(bool returnZeroIfBlank = false, bool returnZeroIfInvalid = false)
		{
			if (string.IsNullOrEmpty(this.total))
			{
				if (returnZeroIfBlank)
				{
					return 0M;
				}
				throw new ApplicationException("Total is empty");
			}
			decimal result;
			if (decimal.TryParse(this.total, out result))
			{
				return result;
			}
			if (returnZeroIfInvalid)
			{
				return 0M;
			}
			throw new ApplicationException("Cannot convert total '" + this.total + "' to decimal");
		}
	}

	public struct PayPalTransactionFeeData
	{
		public string value;

		/// <summary>
		/// Always USD
		/// </summary>
		public string currency;

	}

	/// <summary>
	/// details object
	/// This object defines amount details
	/// </summary>
	public struct PayPalAmountDetailsData
	{
		/// <summary>
		/// Amount charged for shipping. 10 characters max with support for 2 decimal places.
		/// </summary>
		public string shipping;

		/// <summary>
		/// Amount of the subtotal of the items. Required if line items are specified. 10 characters max, with support for 2 decimal places.
		/// </summary>
		public string subtotal;
		
		/// <summary>
		/// Amount charged for tax. 10 characters max with support for 2 decimal places.
		/// </summary>
		public string tax;
		
		/// <summary>
		/// Amount being charged for the handling fee. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date
		/// </summary>
		public string handling_fee;
		
		/// <summary>
		/// Amount being charged for the insurance fee. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date
		/// </summary>
		public string insurance;
		
		/// <summary>
		/// Amount being discounted for the shipping fee. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date.
		/// </summary>
		public string shipping_discount;
	}

	/// <summary>
	/// Related_resources.
	/// array of sale, authorization, capture, or refund, objects Financial transactions related to a payment
	/// </summary>
	public struct PayPalTransactionRelatedResourceData
	{
		/// <summary>
		/// PayPal sale data
		/// </summary>
		public PayPalSaleData sale;
	}

	/// <summary>
	/// sale object
	/// This object defines a sale object.
	/// </summary>
	public struct PayPalSaleData
	{
		/// <summary>
		/// ID of the sale transaction. (API only, not related to PayPal transaction in account)
		/// </summary>
		public string id;

		/// <summary>
		/// Details about the collected amount.
		/// </summary>
		public PayPalAmountData amount;

		/// <summary>
		/// Description of sale.
		/// </summary>
		public string description;

		/// <summary>
		/// Time of sale as defined in RFC 3339 Section 5.6  Value assigned by PayPal
		/// </summary>
		public string create_time;

		/// <summary>
		/// State of the sale. One of the following: pending; completed; refunded; or partially_refunded. Value assigned by PayPal. 
		/// </summary>
		public string state;

		/// <summary>
		/// ID of the payment resource on which this transaction is based. Value assigned by PayPal.
		/// </summary>
		public string parent_payment;

		/// <summary>
		/// Time that the resource was last updated. Value assigned by PayPal
		/// </summary>
		public string update_time;

		/// <summary>
		/// Specifies payment mode of the transaction. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. Assigned in response. Allowed values: INSTANT_TRANSFER, MANUAL_BANK_TRANSFER, DELAYED_TRANSFER, or ECHECK.
		/// </summary>
		public string payment_mode;

		/// <summary>
		/// Reason the transaction is in pending state. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. Allowed values: PAYER-SHIPPING-UNCONFIRMED, MULTI-CURRENCY, RISK-REVIEW, REGULATORY-REVIEW, VERIFICATION-REQUIRED, ORDER, or OTHER.
		/// </summary>
		public string pending_reason;

		/// <summary>
		/// Reason code for the transaction state being Pending or Reversed. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. Assigned in response. Allowed values: CHARGEBACK, GUARANTEE, BUYER_COMPLAINT, REFUND, UNCONFIRMED_SHIPPING_ADDRESS, ECHECK, INTERNATIONAL_WITHDRAWAL, RECEIVING_PREFERENCE_MANDATES_MANUAL_ACTION, PAYMENT_REVIEW, REGULATORY_REVIEW, UNILATERAL, or VERIFICATION_REQUIRED.
		/// </summary>
		public string reason_code;

		/// <summary>
		/// Expected clearing time for eCheck transactions. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. Assigned in response.
		/// </summary>
		public string clearing_time;

		/// <summary>
		/// Protection eligibility of the payer. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. Allowed values: ELIGIBLE, PARTIALLY_ELIGIBLE, or INELIGIBLE.
		/// </summary>
		public string protection_eligibility;

		/// <summary>
		/// Protection eligibility type of the payer. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. Allowed values: ELIGIBLE, ITEM_NOT_RECEIVED_ELIGIBLE, INELIGIBLE, or UNAUTHORIZED_PAYMENT_ELIGIBLE.
		/// </summary>
		public string protection_eligibility_type;

		/// <summary>
		/// HATEOAS links related to this call. Value generated by PayPal.
		/// </summary>
		public PayPalLinkData[] links;

		/// <summary>
		/// Undocumented in PayPal API; transaction fees for processing
		/// </summary>
		public PayPalTransactionFeeData transaction_fee;
	}

	/// <summary>
	/// item_list object
	/// This object provides a list of items and the related shipping address within a transaction
	/// </summary>
	public struct PayPalItemListData
	{
		public PayPalItemListData(PayPalItemData[] items)
		{
			this.items = items;
			this.shipping_address = null;
		}

		/// <summary>
		/// List of items.
		/// </summary>
		public PayPalItemData[] items;

		/// <summary>
		/// Shipping address, if different than the payer address.
		/// </summary>
		public PayPalShippingAddressData? shipping_address;
	}

	/// <summary>
	/// item object
 	/// This object defines a item object. (product)
	/// </summary>
	public struct PayPalItemData
	{
		public PayPalItemData(string quantity, string name, string price, string sku)
		{
			this.quantity = quantity;
			this.name = name;
			this.price = price;
			this.sku = sku;
			this.currency = "USD";
			this.tax = null;
			this.description = null;
		}

		/// <summary>
		/// Number of a particular item. 10 characters max. Required.
		/// </summary>
		public string quantity;
		
		/// <summary>
		/// Item name. 127 characters max. Required.
		/// </summary>
		public string name;
		
		/// <summary>
		/// Item cost. 10 characters max. Required.
		/// </summary>
		public string price;
		
		/// <summary>
		/// 3-letter currency code. Required.
		/// </summary>
		public string currency;
		
		/// <summary>
		/// Stock keeping unit corresponding (SKU) to item. 50 characters max.
		/// </summary>
		public string sku;

		/// <summary>
		/// Description of the item. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date. 127 characters max.
		/// </summary>
		public string description;

		/// <summary>
		/// Tax of the item. Currently supported with paypal payment_method only, but available for credit_card payment_method at a later date.
		/// </summary>
		public string tax;
	}

}
