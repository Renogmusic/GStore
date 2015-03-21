using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("StoreFrontConfiguration")]
	public class StoreFrontConfiguration : BaseClasses.StoreFrontRecord
	{
		#region Basic Fields

		[Key]
		[Display(Name = "Store Front Configuration Id")]
		public int StoreFrontConfigurationId { get; set; }

		[Required]
		[MaxLength(100)]
		public string Folder { get; set; }

		public int Order { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		[MaxLength(100)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public string ConfigurationName { get; set; }

		[DataType(DataType.Url)]
		[Required]
		[Display(Name = "Public Url")]
		[MaxLength(200)]
		public string PublicUrl { get; set; }

		[Required]
		[Display(Name = "Time Zone Id")]
		[MaxLength(50)]
		public string TimeZoneId { get; set; }

		[AllowHtml]
		[Display(Name = "Footer Html")]
		[MaxLength(250)]
		[DataType(DataType.Html)]
		public string HtmlFooter { get; set; }

		[Display(Name = "Use Catalog as Home Page")]
		public bool HomePageUseCatalog { get; set; }

		[Display(Name = "Nav Bar Items Max Levels")]
		[Range(1, 6)]
		public int NavBarItemsMaxLevels { get; set; }

		#endregion
		
		#region Admins

		[Display(Name = "Account Admin Id", Description = "User who sends locked out and password changed notifications.")]
		public int AccountAdmin_UserProfileId { get; set; }

		[ForeignKey("AccountAdmin_UserProfileId")]
		[Display(Name = "Account Admin", Description = "User who sends locked out and password changed notifications.")]
		public virtual UserProfile AccountAdmin { get; set; }

		[Display(Name = "Registered Notify Id", Description = "User gets notified of new user registrations with sign-up details")]
		public int RegisteredNotify_UserProfileId { get; set; }

		[ForeignKey("RegisteredNotify_UserProfileId")]
		[Display(Name = "Registered Notify", Description = "User gets notified of new user registrations with sign-up details")]
		public virtual UserProfile RegisteredNotify { get; set; }

		[Display(Name = "Welcome Person Id", Description = "User who sends a welcome message to newly registered users.")]
		public int WelcomePerson_UserProfileId { get; set; }

		[ForeignKey("WelcomePerson_UserProfileId")]
		[Display(Name = "Welcome Person", Description = "User who sends a welcome message to newly registered users.")]
		public virtual UserProfile WelcomePerson { get; set; }

		[Display(Name = "Order Admin Id", Description = "User who get notified of new orders and sends order notification and emails.")]
		public int OrderAdmin_UserProfileId { get; set; }

		[ForeignKey("OrderAdmin_UserProfileId")]
		[Display(Name = "Order Admin", Description = "User who get notified of new orders and sends order notification and emails.")]
		public virtual UserProfile OrderAdmin { get; set; }


		#endregion
		
		#region Meta Tags

		[Display(Name = "Meta Tag App Name")]
		[MaxLength(250)]
		public string MetaApplicationName { get; set; }

		[Display(Name = "Meta Tag Tile Color")]
		[MaxLength(25)]
		public string MetaApplicationTileColor { get; set; }

		[Display(Name = "Meta Tag Description")]
		[MaxLength(1000)]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords")]
		[MaxLength(1000)]
		public string MetaKeywords { get; set; }

		[AllowHtml]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Body Top Script Tag")]
		public string BodyTopScriptTag { get; set; }

		[AllowHtml]
		[DataType(DataType.MultilineText)]
		[Display(Name = "Body Bottom Script Tag")]
		public string BodyBottomScriptTag { get; set; }

		#endregion
		
		#region Tracking

		[Display(Name = "Enable Google Analytics")]
		public bool EnableGoogleAnalytics { get; set; }

		[Display(Name = "Google Analytics Web Property Id")]
		[MaxLength(50)]
		public string GoogleAnalyticsWebPropertyId { get; set; }

		#endregion
		
		#region Themes

		[Required]
		[Display(Name = "Default Theme Id")]
		public int DefaultNewPageThemeId { get; set; }

		[Display(Name = "Default Theme")]
		[ForeignKey("DefaultNewPageThemeId")]
		public virtual Theme DefaultNewPageTheme { get; set; }

		[Required]
		[Display(Name = "Store Catalog Theme Id")]
		public int CatalogThemeId { get; set; }

		[Display(Name = "Store Catalog Theme")]
		[ForeignKey("CatalogThemeId")]
		public virtual Theme CatalogTheme { get; set; }

		[Required]
		[Display(Name = "Account Theme Id")]
		public int AccountThemeId { get; set; }

		[Display(Name = "Account Theme")]
		[ForeignKey("AccountThemeId")]
		public virtual Theme AccountTheme { get; set; }

		[Required]
		[Display(Name = "Notifications Theme Id")]
		public int NotificationsThemeId { get; set; }

		[Display(Name = "Notifications Theme")]
		[ForeignKey("NotificationsThemeId")]
		public virtual Theme NotificationsTheme { get; set; }

		[Required]
		[Display(Name = "Profile Theme Id")]
		public int ProfileThemeId { get; set; }

		[Display(Name = "Profile Theme")]
		[ForeignKey("ProfileThemeId")]
		public virtual Theme ProfileTheme { get; set; }

		[Required]
		[Display(Name = "Store Admin Theme Id")]
		public int AdminThemeId { get; set; }

		[Display(Name = "Store Admin Theme")]
		[ForeignKey("AdminThemeId")]
		public virtual Theme AdminTheme { get; set; }

		#endregion

		#region Cart

		[Display(Name = "Use Shopping Cart", Description = "Check this box to use the built-in shopping cart. If unchecked, items can be ordered one at a time.\nDefault: checked.")]
		public bool UseShoppingCart { get; set; }

		[Display(Name = "Show Cart in top menu to Anonymous Users")]
		public bool CartNavShowCartToAnonymous { get; set; }

		[Display(Name = "Show Cart in top menu for Registered Users")]
		public bool CartNavShowCartToRegistered { get; set; }

		[Display(Name = "Show Cart in top menu when empty")]
		public bool CartNavShowCartWhenEmpty { get; set; }

		[Display(Name = "Require Login to Add to Cart", Description = "Check this box to require the user to log in when adding to cart. \nDefault: Unchecked.")]
		public bool CartRequireLogin { get; set; }

		[Required]
		[Display(Name = "Cart Theme Id")]
		public int CartThemeId { get; set; }

		[Display(Name = "Cart Theme")]
		[ForeignKey("CartThemeId")]
		public virtual Theme CartTheme { get; set; }

		[Display(Name = "Cart Page Title")]
		public string CartPageTitle { get; set; }

		[Display(Name = "Cart Page Heading")]
		public string CartPageHeading { get; set; }

		[Display(Name = "Cart Empty Message")]
		public string CartEmptyMessage { get; set; }

		[Display(Name = "Cart Checkout Button Label", Description = "Text shown on the checkout button. \nExample: 'Check Out' or 'Continue to Check out'")]
		public string CartCheckoutButtonLabel { get; set; }

		[Display(Name = "CartItemColumnLabel")]
		public string CartItemColumnLabel { get; set; }

		[Display(Name = "CartItemVariantColumnShow")]
		public bool CartItemVariantColumnShow { get; set; }

		[Display(Name = "CartItemVariantColumnLabel")]
		public string CartItemVariantColumnLabel { get; set; }

		[Display(Name = "CartItemListPriceColumnShow")]
		public bool CartItemListPriceColumnShow { get; set; }

		[Display(Name = "CartItemListPriceColumnLabel")]
		public string CartItemListPriceColumnLabel { get; set; }

		[Display(Name = "CartItemUnitPriceColumnShow")]
		public bool CartItemUnitPriceColumnShow { get; set; }

		[Display(Name = "CartItemUnitPriceColumnLabel")]
		public string CartItemUnitPriceColumnLabel { get; set; }

		[Display(Name = "CartItemQuantityColumnShow")]
		public bool CartItemQuantityColumnShow { get; set; }

		[Display(Name = "CartItemQuantityColumnLabel")]
		public string CartItemQuantityColumnLabel { get; set; }

		[Display(Name = "CartItemListPriceExtColumnShow")]
		public bool CartItemListPriceExtColumnShow { get; set; }

		[Display(Name = "CartItemListPriceExtColumnLabel")]
		public string CartItemListPriceExtColumnLabel { get; set; }

		[Display(Name = "CartItemUnitPriceExtColumnShow")]
		public bool CartItemUnitPriceExtColumnShow { get; set; }

		[Display(Name = "CartItemUnitPriceExtColumnLabel")]
		public string CartItemUnitPriceExtColumnLabel { get; set; }

		[Display(Name = "CartItemDiscountColumnShow")]
		public bool CartItemDiscountColumnShow { get; set; }

		[Display(Name = "CartItemDiscountColumnLabel")]
		public string CartItemDiscountColumnLabel { get; set; }

		[Display(Name = "CartBundleShowIncludedItems")]
		public bool CartBundleShowIncludedItems { get; set; }

		[Display(Name = "CartBundleShowPriceOfIncludedItems")]
		public bool CartBundleShowPriceOfIncludedItems { get; set; }

		[Display(Name = "CartItemTotalColumnShow")]
		public bool CartItemTotalColumnShow { get; set; }

		[Display(Name = "CartItemTotalColumnLabel")]
		public string CartItemTotalColumnLabel { get; set; }

		[Display(Name = "CartOrderDiscountCodeSectionShow")]
		public bool CartOrderDiscountCodeSectionShow { get; set; }

		[Display(Name = "CartOrderDiscountCodeLabel")]
		public string CartOrderDiscountCodeLabel { get; set; }

		[Display(Name = "CartOrderDiscountCodeApplyButtonText")]
		public string CartOrderDiscountCodeApplyButtonText { get; set; }

		[Display(Name = "CartOrderDiscountCodeRemoveButtonText")]
		public string CartOrderDiscountCodeRemoveButtonText { get; set; }

		[Display(Name = "CartOrderItemCountShow")]
		public bool CartOrderItemCountShow { get; set; }

		[Display(Name = "CartOrderItemCountLabel")]
		public string CartOrderItemCountLabel { get; set; }

		[Display(Name = "CartOrderSubtotalShow")]
		public bool CartOrderSubtotalShow { get; set; }

		[Display(Name = "CartOrderSubtotalLabel")]
		public string CartOrderSubtotalLabel { get; set; }

		[Display(Name = "CartOrderTaxShow")]
		public bool CartOrderTaxShow { get; set; }

		[Display(Name = "CartOrderTaxLabel")]
		public string CartOrderTaxLabel { get; set; }

		[Display(Name = "CartOrderShippingShow")]
		public bool CartOrderShippingShow { get; set; }

		[Display(Name = "CartOrderShippingLabel")]
		public string CartOrderShippingLabel { get; set; }

		[Display(Name = "CartOrderHandlingShow")]
		public bool CartOrderHandlingShow { get; set; }

		[Display(Name = "CartOrderHandlingLabel")]
		public string CartOrderHandlingLabel { get; set; }

		[Display(Name = "CartOrderDiscountShow")]
		public bool CartOrderDiscountShow { get; set; }

		[Display(Name = "CartOrderDiscountLabel")]
		public string CartOrderDiscountLabel { get; set; }

		[Display(Name = "CartOrderTotalLabel")]
		public string CartOrderTotalLabel { get; set; }

		#endregion

		#region Checkout

		[Required]
		[Display(Name = "Checkout Theme Id")]
		public int CheckoutThemeId { get; set; }

		[Display(Name = "Checkout Theme")]
		[ForeignKey("CheckoutThemeId")]
		public virtual Theme CheckoutTheme { get; set; }

		[Display(Name = "Checkout Log in or Guest Web Form Id", Description = "Web Form custom fields for Log in or guest page")]
		public int? CheckoutLogInOrGuestWebFormId { get; set; }

		[ForeignKey("CheckoutLogInOrGuestWebFormId")]
		[Display(Name = "Checkout Log in or Guest Web Form", Description = "Web Form custom fields for Log in or guest page")]
		public virtual WebForm CheckoutLogInOrGuestWebForm { get; set; }

		[Display(Name = "Checkout Delivery Info Digital Only Web Form Id", Description = "Web Form custom fields for digital only orders")]
		public int? CheckoutDeliveryInfoDigitalOnlyWebFormId { get; set; }

		[ForeignKey("CheckoutDeliveryInfoDigitalOnlyWebFormId")]
		[Display(Name = "Delivery Info Digital Only Web Form", Description = "Web Form custom fields for digital only orders")]
		public virtual WebForm CheckoutDeliveryInfoDigitalOnlyWebForm { get; set; }

		[Display(Name = "Checkout Delivery Info Shipping Web Form Id", Description = "Web Form custom fields for shipping orders")]
		public int? CheckoutDeliveryInfoShippingWebFormId { get; set; }

		[ForeignKey("CheckoutDeliveryInfoShippingWebFormId")]
		[Display(Name = "Delivery Info Shipping Web Form", Description = "Web Form custom fields for shipping orders")]
		public virtual WebForm CheckoutDeliveryInfoShippingWebForm { get; set; }

		[Display(Name = "Checkout Delivery Method Web Form Id", Description = "Web Form custom fields for delivery method")]
		public int? CheckoutDeliveryMethodWebFormId { get; set; }

		[ForeignKey("CheckoutDeliveryMethodWebFormId")]
		[Display(Name = "Delivery Method Web Form", Description = "Web Form custom fields for delivery method")]
		public virtual WebForm CheckoutDeliveryMethodWebForm { get; set; }

		[Display(Name = "Checkout Payment Info Web Form Id", Description = "Web Form custom fields for Payment info")]
		public int? CheckoutPaymentInfoWebFormId { get; set; }

		[ForeignKey("CheckoutPaymentInfoWebFormId")]
		[Display(Name = "Payment Info Web Form", Description = "Web Form custom fields for Payment info")]
		public virtual WebForm CheckoutPaymentInfoWebForm { get; set; }

		[Display(Name = "Checkout Confirm Order Web Form Id", Description = "Web Form custom fields for Confirm Order")]
		public int? CheckoutConfirmOrderWebFormId { get; set; }

		[ForeignKey("CheckoutConfirmOrderWebFormId")]
		[Display(Name = "Confirm Order Web Form", Description = "Web Form custom fields for Confirm Order")]
		public virtual WebForm CheckoutConfirmOrderWebForm { get; set; }

		#endregion

		#region Order Status

		[Required]
		[Display(Name = "Order Status Theme Id")]
		public int OrdersThemeId { get; set; }

		[Display(Name = "Order Status Theme")]
		[ForeignKey("OrdersThemeId")]
		public virtual Theme OrdersTheme { get; set; }

		#endregion

		#region Order Admin

		[Required]
		[Display(Name = "Order Admin Theme Id")]
		public int OrderAdminThemeId { get; set; }

		[Display(Name = "Order Admin Theme")]
		[ForeignKey("OrderAdminThemeId")]
		public virtual Theme OrderAdminTheme { get; set; }

		#endregion

		#region Catalog Admin

		[Required]
		[Display(Name = "Catalog Admin Theme Id")]
		public int CatalogAdminThemeId { get; set; }

		[Display(Name = "Catalog Admin Theme")]
		[ForeignKey("CatalogAdminThemeId")]
		public virtual Theme CatalogAdminTheme { get; set; }

		#endregion

		#region Catalog Layout

		[Required]
		[Display(Name = "Catalog Title", Description = "Catalog Title shown on all catalog pages.")]
		public string CatalogTitle { get; set; }

		[Required]
		[Display(Name = "Catalog Layout", Description="Catalog Layout template setting.")]
		public CatalogLayoutEnum CatalogLayout { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Header Html", Description = "Catalog Header shown on all catalog pages.")]
		public string CatalogHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Footer Html", Description = "Catalog Footer shown on all catalog pages.")]
		public string CatalogFooterHtml { get; set; }

		[Required]
		[Display(Name = "Catalog Home List Template", Description = "Top (root) catalog list template.")]
		public CategoryListTemplateEnum CatalogRootListTemplate { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Home Header Html", Description = "Catalog Header shown on the Home (root) catalog page.")]
		public string CatalogRootHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Catalog Home Footer Html", Description = "Catalog Footer shown on the Home (root) catalog page.")]
		public string CatalogRootFooterHtml { get; set; }


		[Range(0, 6)]
		[Display(Name = "Nav Bar Catalog Max Levels")]
		public int NavBarCatalogMaxLevels { get; set; }

		[Range(1, 6)]
		[Display(Name = "Catalog Page Initial Levels")]
		public int CatalogPageInitialLevels { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Category col-lg")]
		public int CatalogCategoryColLg { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Category col-md")]
		public int CatalogCategoryColMd { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Category col-sm")]
		public int CatalogCategoryColSm { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Product col-lg")]
		public int CatalogProductColLg { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Product col-md")]
		public int CatalogProductColMd { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Product col-sm")]
		public int CatalogProductColSm { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Bundle col-lg")]
		public int CatalogProductBundleColLg { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Bundle col-md")]
		public int CatalogProductBundleColMd { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Bundle col-sm")]
		public int CatalogProductBundleColSm { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Bundle Item col-lg")]
		public int CatalogProductBundleItemColLg { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Bundle Item col-md")]
		public int CatalogProductBundleItemColMd { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Catalog Bundle Item col-sm")]
		public int CatalogProductBundleItemColSm { get; set; }

		#endregion
		
		#region Error Pages

		[Display(Name = "Not Found Error Page Id")]
		public int? NotFoundError_PageId { get; set; }

		/// <summary>
		/// File Not Found 404 Store Error Page or null if none (use system default 404 page)
		/// </summary>
		[ForeignKey("NotFoundError_PageId")]
		[Display(Name = "Not Found Error Page")]
		public virtual Page NotFoundErrorPage { get; set; }

		[Display(Name = "Store Error Page Id")]
		public int? StoreError_PageId { get; set; }

		/// <summary>
		/// Store Error Page (for any error other than not found 404) or null if none (use system default 404 page)
		/// </summary>
		[ForeignKey("StoreError_PageId")]
		[Display(Name = "Store Error Page")]
		public virtual Page StoreErrorPage { get; set; }

		#endregion

		#region Registration

		[Display(Name = "Show Register link on Nav Bar")]
		public bool NavBarShowRegisterLink { get; set; }

		[Display(Name = "Nav Bar Register link text")]
		[MaxLength(50)]
		public string NavBarRegisterLinkText { get; set; }

		[Display(Name = "Show Register link on Login Page")]
		public bool AccountLoginShowRegisterLink { get; set; }

		[Display(Name = "Register link text on Login Page")]
		[MaxLength(50)]
		public string AccountLoginRegisterLinkText { get; set; }

		[Display(Name = "Register Web Form Id")]
		public int? Register_WebFormId { get; set; }

		[ForeignKey("Register_WebFormId")]
		[Display(Name = "Register Web Form")]
		public virtual WebForm RegisterWebForm { get; set; }

		[Display(Name = "Register Success Page Id")]
		public int? RegisterSuccess_PageId { get; set; }

		[ForeignKey("RegisterSuccess_PageId")]
		[Display(Name = "Register Success Page")]
		public virtual Page RegisterSuccessPage { get; set; }

		#endregion

		#region Order and Payment Settings

		[Display(Name = "Auto-Accept orders if paid", Description = "Check this box to automatically accept all orders that are paid. Uncheck this box to manually accept and confirm paid orders.")]
		public bool Orders_AutoAcceptPaid { get; set; }

		[Display(Name = "Payment Method - Use PayPal", Description = "Check this box to use PayPal as a payment method.\nIf you do not have any payment methods selected, the user will order and you will have to contact them for payment info.")]
		public bool PaymentMethod_PayPal_Enabled { get; set; }

		public bool PaymentMethod_PayPal_UseLiveServer { get; set; }

		public string PaymentMethod_PayPal_Client_Id { get; set; }

		public string PaymentMethod_PayPal_Client_Secret { get; set; }

		#endregion

	}

	public enum CatalogLayoutEnum : int
	{
		[Display(Name="Simple Blocked", Description="Blocks for products categories, products, with single photo")]
		SimpleBlocked = 0,

		[Display(Name = "eCommerce", Description = "Typical eCommerce layout with categories on the left side and 4 products per row")]
		ECommerce = 100,

		[Display(Name = "eCommerce with image rotator", Description = "eCommerce layout with ratating images and multiple images per product. Categories on the left side and 4 products per row")]
		ECommerceWithRotator = 100,

	}

}