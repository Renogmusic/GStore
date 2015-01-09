using GStore.Models;
using GStore.Identity;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Models.ViewModels
{
	[NotMapped()]
	public class CartConfigViewModel
	{
		public CartConfigViewModel() { }

		public CartConfigViewModel(StoreFrontConfiguration storeFrontConfig, bool isEditPage, bool isViewPage)
		{
			this.IsEditPage = isEditPage;
			this.IsViewPage = isViewPage;

			this.StoreFront = storeFrontConfig.StoreFront;
			this.StoreFrontId = storeFrontConfig.StoreFrontId;
			this.StoreFrontConfigurationId = storeFrontConfig.StoreFrontConfigurationId;
			this.ConfigurationName = storeFrontConfig.ConfigurationName;
			this.StoreFrontName = storeFrontConfig.Name;
			this.UseShoppingCart = storeFrontConfig.UseShoppingCart;
			this.CartNavShowCartToAnonymous = storeFrontConfig.CartNavShowCartToAnonymous;
			this.CartNavShowCartToRegistered = storeFrontConfig.CartNavShowCartToRegistered;
			this.CartNavShowCartWhenEmpty = storeFrontConfig.CartNavShowCartWhenEmpty;
			this.CartRequireLogin = storeFrontConfig.CartRequireLogin;
			this.CartLayoutName = storeFrontConfig.CartLayoutName;
			this.CartTheme = storeFrontConfig.CartTheme;
			this.CartThemeId = storeFrontConfig.CartThemeId;
			this.CartPageTitle = storeFrontConfig.CartPageTitle;
			this.CartPageHeading = storeFrontConfig.CartPageHeading;
			this.CartEmptyMessage = storeFrontConfig.CartEmptyMessage;
			this.CartItemColumnLabel = storeFrontConfig.CartItemColumnLabel;
			this.CartItemVariantColumnShow = storeFrontConfig.CartItemVariantColumnShow;
			this.CartItemVariantColumnLabel = storeFrontConfig.CartItemVariantColumnLabel;
			this.CartItemListPriceColumnShow = storeFrontConfig.CartItemListPriceColumnShow;
			this.CartItemListPriceColumnLabel = storeFrontConfig.CartItemListPriceColumnLabel;
			this.CartItemUnitPriceColumnShow = storeFrontConfig.CartItemUnitPriceColumnShow;
			this.CartItemUnitPriceColumnLabel = storeFrontConfig.CartItemUnitPriceColumnLabel;
			this.CartItemQuantityColumnShow = storeFrontConfig.CartItemQuantityColumnShow;
			this.CartItemQuantityColumnLabel = storeFrontConfig.CartItemQuantityColumnLabel;
			this.CartItemListPriceExtColumnShow = storeFrontConfig.CartItemListPriceExtColumnShow;
			this.CartItemListPriceExtColumnLabel = storeFrontConfig.CartItemListPriceExtColumnLabel;
			this.CartItemUnitPriceExtColumnShow = storeFrontConfig.CartItemUnitPriceExtColumnShow;
			this.CartItemUnitPriceExtColumnLabel = storeFrontConfig.CartItemUnitPriceExtColumnLabel;
			this.CartItemDiscountColumnShow = storeFrontConfig.CartItemDiscountColumnShow;
			this.CartItemDiscountColumnLabel = storeFrontConfig.CartItemDiscountColumnLabel;
			this.CartItemTotalColumnShow = storeFrontConfig.CartItemTotalColumnShow;
			this.CartItemTotalColumnLabel = storeFrontConfig.CartItemTotalColumnLabel;
			this.CartOrderDiscountCodeSectionShow = storeFrontConfig.CartOrderDiscountCodeSectionShow;
			this.CartOrderDiscountCodeLabel = storeFrontConfig.CartOrderDiscountCodeLabel;
			this.CartOrderDiscountCodeApplyButtonText = storeFrontConfig.CartOrderDiscountCodeApplyButtonText;
			this.CartOrderDiscountCodeRemoveButtonText = storeFrontConfig.CartOrderDiscountCodeRemoveButtonText;
			this.CartOrderItemCountShow = storeFrontConfig.CartOrderItemCountShow;
			this.CartOrderItemCountLabel = storeFrontConfig.CartOrderItemCountLabel;
			this.CartOrderSubtotalShow = storeFrontConfig.CartOrderSubtotalShow;
			this.CartOrderSubtotalLabel = storeFrontConfig.CartOrderSubtotalLabel;
			this.CartOrderTaxShow = storeFrontConfig.CartOrderTaxShow;
			this.CartOrderTaxLabel = storeFrontConfig.CartOrderTaxLabel;
			this.CartOrderShippingShow = storeFrontConfig.CartOrderShippingShow;
			this.CartOrderShippingLabel = storeFrontConfig.CartOrderShippingLabel;
			this.CartOrderHandlingShow = storeFrontConfig.CartOrderHandlingShow;
			this.CartOrderHandlingLabel = storeFrontConfig.CartOrderHandlingLabel;
			this.CartOrderDiscountShow = storeFrontConfig.CartOrderDiscountShow;
			this.CartOrderDiscountLabel = storeFrontConfig.CartOrderDiscountLabel;
			this.CartOrderTotalLabel = storeFrontConfig.CartOrderTotalLabel;
		}

		public bool IsEditPage { get; protected set; }

		public bool IsViewPage { get; protected set; }

		[Display(Name = "Store Front", Description = "Store Front")]
		public StoreFront StoreFront { get; protected set; }

		[Required]
		[Display(Name = "Store Front Id", Description = "Internal Store Front Id Number")]
		public int StoreFrontId { get; set; }

		[Key]
		[Display(Name = "Store Front Configuration Id", Description = "Internal Store Front Configuration Id Number")]
		public int StoreFrontConfigurationId { get; set; }

		[Display(Name = "Configuration Name", Description = "Internal Store Front Configuration Name")]
		public string ConfigurationName { get; protected set; }

		[Display(Name = "Store Front Name", Description = "Store Front Name")]
		public string StoreFrontName { get; protected set; }

		[Display(Name = "Use Shopping Cart", Description = "Check this box to use the built-in shopping cart. If unchecked, items can be ordered one at a time.\nDefault: checked.")]
		public bool UseShoppingCart { get; set; }

		[Display(Name = "Show Cart to Anonymous Users", Description = "Check this box to show the cart link to anonymous users in the top menu. \nDefault: checked.")]
		public bool CartNavShowCartToAnonymous { get; set; }

		[Display(Name = "Show Cart to Registered Users", Description = "Check this box to show the cart link to registered users in the top menu. \nDefault: checked.")]
		public bool CartNavShowCartToRegistered { get; set; }

		[Display(Name = "Show Cart if Empty", Description = "Check this box to show the cart link in the top menu even if the cart is empty. \nDefault: checked.")]
		public bool CartNavShowCartWhenEmpty { get; set; }

		[Display(Name = "Require Login to Add to Cart", Description = "Check this box to require the user to log in when adding to cart. \nDefault: Unchecked.")]
		public bool CartRequireLogin { get; set; }

		[Display(Name = "CartLayoutName", Description = "Always 'Default'")]
		public string CartLayoutName { get; set; }

		[Display(Name = "Theme", Description = "Theme used for the cart and order pages")]
		public Theme CartTheme { get; set; }

		[Display(Name = "Theme Id", Description = "Theme used for the cart and order pages")]
		public int CartThemeId { get; set; }

		[Display(Name = "Page Title", Description = "Cart page title in the browser bar and favorites. \nExample: Cart")]
		public string CartPageTitle { get; set; }

		[Display(Name = "Page Heading", Description = "Cart page heading shown on top of the cart. \nExample: Your Shopping Cart")]
		public string CartPageHeading { get; set; }

		[Display(Name = "Cart Empty Message", Description = "Message shown when the cart is empty. \nExample: Your cart is empty.")]
		public string CartEmptyMessage { get; set; }

		[Display(Name = "Item Column Label", Description = "Label for Item on each item in cart. \nExample: Item, or Product")]
		public string CartItemColumnLabel { get; set; }

		[Display(Name = "Show Variant Column", Description = "Check this box to show the product variant column")]
		public bool CartItemVariantColumnShow { get; set; }

		[Display(Name = "Variant Column Label", Description = "Label shown on the product variant column\nExample: Type")]
		public string CartItemVariantColumnLabel { get; set; }

		[Display(Name = "Show List Price column", Description = "Check this box to show the List Price column for each item in the cart.")]
		public bool CartItemListPriceColumnShow { get; set; }

		[Display(Name = "List Price Column Label", Description = "Label for List Price on each item in cart. \nExample: List Price")]
		public string CartItemListPriceColumnLabel { get; set; }

		[Display(Name = "Show Unit Price column", Description = "Check this box to show the Unit Price column for each item in the cart.")]
		public bool CartItemUnitPriceColumnShow { get; set; }

		[Display(Name = "Unit Price Column Label", Description = "Label for the Unit Price column for order items. \nExample: Price")]
		public string CartItemUnitPriceColumnLabel { get; set; }

		[Display(Name = "Show Quantity column", Description = "Check this box to show the Quantity column for each item in the cart.")]
		public bool CartItemQuantityColumnShow { get; set; }

		[Display(Name = "Quantity Column Label", Description = "Label for the Quantity column for order items. \nExample: Quantity, Items")]
		public string CartItemQuantityColumnLabel { get; set; }

		[Display(Name = "Show List Price Ext column", Description = "Check this box to show the List Price Ext column for each item in the cart.")]
		public bool CartItemListPriceExtColumnShow { get; set; }

		[Display(Name = "List Price Ext Column Label", Description = "Label for List Price Ext on each item in cart. \nExample: List Price Ext")]
		public string CartItemListPriceExtColumnLabel { get; set; }

		[Display(Name = "Show Unit Price Ext Column", Description = "Check this box to show the Unit Price Ext column on each item in cart")]
		public bool CartItemUnitPriceExtColumnShow { get; set; }

		[Display(Name = "Unit Price Ext Column Label", Description = "Label for the Unit Price Ext column for each item in cart. \nExample: Unit Price Ext, Cost")]
		public string CartItemUnitPriceExtColumnLabel { get; set; }

		[Display(Name = "Show Line Discount column", Description = "Check this box to show the Line Discount column for each item in the cart.")]
		public bool CartItemDiscountColumnShow { get; set; }

		[Display(Name = "Line Discount Column Label", Description = "Label for Line Discount on each item in cart. \nExample: Discount, or Your Savings")]
		public string CartItemDiscountColumnLabel { get; set; }

		[Display(Name = "Show Line Total column", Description = "Check this box to show the Line Total column for each item in the cart.")]
		public bool CartItemTotalColumnShow { get; set; }

		[Display(Name = "Line Total Column Label", Description = "Label for Line Total on each item in cart. \nExample: Line Total")]
		public string CartItemTotalColumnLabel { get; set; }

		[Display(Name = "Use Discount Codes", Description = "Check this box to show the Discount Code section on the order form to apply discount codes")]
		public bool CartOrderDiscountCodeSectionShow { get; set; }

		[Display(Name = "Discount Code Label", Description = "Label for the Discount Code section. \nExample: Discount Code, Remove Code")]
		public string CartOrderDiscountCodeLabel { get; set; }

		[Display(Name = "Discount Code Apply Button Label", Description = "Label for the Apply button on the Discount Code section. \nExample: Add Coupon, Submit Code, Use Coupon")]
		public string CartOrderDiscountCodeApplyButtonText { get; set; }

		[Display(Name = "Discount Code Remove Button Label", Description = "Label for the remove button when removing a discount code. \nExample: Remove Discount, Remove Code")]
		public string CartOrderDiscountCodeRemoveButtonText { get; set; }

		[Display(Name = "Show Item Count Row", Description = "Check this box to show the Order Item Count row on the order total section")]
		public bool CartOrderItemCountShow { get; set; }

		[Display(Name = "Order Item Count Label", Description = "Label for the Order Item Count row. \nExample: Number of items")]
		public string CartOrderItemCountLabel { get; set; }

		[Display(Name = "Show SubTotal Row", Description = "Check this box to show the SubTotal row on the order total section")]
		public bool CartOrderSubtotalShow { get; set; }

		[Display(Name = "SubTotal Label", Description = "Label on the sub-total line\nExample: Sub-total")]
		public string CartOrderSubtotalLabel { get; set; }

		[Display(Name = "Show Tax Row", Description = "Check this box to show the Tax row on the order total section")]
		public bool CartOrderTaxShow { get; set; }

		[Display(Name = "Tax Label", Description = "Label on the tax line\nExample: Tax")]
		public string CartOrderTaxLabel { get; set; }

		[Display(Name = "Show Shipping Row", Description = "Check this box to show the Shipping row on the order total section")]
		public bool CartOrderShippingShow { get; set; }

		[Display(Name = "Shipping Label", Description = "Label on the Shipping line. \nExample: Shipping")]
		public string CartOrderShippingLabel { get; set; }

		[Display(Name = "Show Handling Row", Description = "Check this box to show the Handling row on the order total section")]
		public bool CartOrderHandlingShow { get; set; }

		[Display(Name = "Handling Label", Description = "Label for Handling in order total. \nExample: Handling")]
		public string CartOrderHandlingLabel { get; set; }

		[Display(Name = "Show Order Discount Row", Description = "Check this box to show the Order Discount row on the order total section")]
		public bool CartOrderDiscountShow { get; set; }

		[Display(Name = "Discount Code Label", Description = "Label for the Discount Code section. \nExample: Discount Code or Coupon Code")]
		public string CartOrderDiscountLabel { get; set; }

		[Display(Name = "Order Total Label", Description = "Label for the Order Total row. \nExample: Order Total")]
		public string CartOrderTotalLabel { get; set; }

	}
}