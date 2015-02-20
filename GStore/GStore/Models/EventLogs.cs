using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
namespace GStore.Models
{
	[Table("SystemEvent")]
	public class SystemEvent : BaseClasses.EventLogBase
	{
		[Key]
		public int SystemEventID { get; set; }

		public int Level { get; set; }

		[Required]
		public string LevelText { get; set; }

		public string ExceptionMessage { get; set; }
		public string BaseExceptionMessage { get; set; }
		public string BaseExceptionToString { get; set; }
	}

	[Table("SecurityEvent")]
	public class SecurityEvent : BaseClasses.EventLogBase
	{
		[Key]
		public int SecurityEventID { get; set; }
		public int Level { get; set; }

		[Required]
		public string LevelText { get; set; }
		public bool Success { get; set; }

	}

	[Table("PageViewEvent")]
	public class PageViewEvent : BaseClasses.EventLogBase
	{
		[Key]
		public int PageViewEventID { get; set; }
	}

	[Table("UserActionEvent")]
	public class UserActionEvent : BaseClasses.EventLogBase
	{
		[Key]
		public int UserActionEventID { get; set; }
		
		[Required]
		public UserActionCategoryEnum Category { get; set; }
		
		[Required]
		public UserActionActionEnum Action { get; set; }
		
		[Required]
		public string Label { get; set; }

		[Required]
		public bool Success { get; set; }

		public int? CartId { get; set; }
		public string ProductUrlName { get; set; }
		public string CategoryUrlName { get; set; }
		public string DiscountCode { get; set; }
		public int? NotificationId { get; set; }
		public string EmailAddress { get; set; }
		public string SMSPhone { get; set; }
		public string OrderNumber { get; set; }
		public int? OrderItemId { get; set; }
		public int? PageId { get; set; }
		public string UploadFileName { get; set; }

	}

	[Table("FileNotFoundLog")]
	public class FileNotFoundLog : BaseClasses.EventLogBase
	{
		[Key]
		public int FileNotFoundLogId { get; set; }
	}

	[Table("BadRequest")]
	public class BadRequest : BaseClasses.EventLogBase
	{
		[Key]
		public int BadRequestId { get; set; }
	}

	public enum SystemEventLevel : int
	{
		Information = 1000,
		Warning = 2000,
		Error = 3000,
		CriticalError = 4000,
		ApplicationException = 9000,
		InvalidOperationException = 9100,
		UnknownException = 9999
	}

	public enum SecurityEventLevel : int
	{
		LoginSuccess = 100,
		LoginFailure = 200,
		LoginFailureNoStoreFront = 205,
		LoginFailureStoreFrontInactive = 207,
		LoginFailureNoStoreFrontConfig = 210,
		LoginFailureStoreFrontConfigInactive = 211,
		LoginNeedsVerification = 220,
		LogOff = 230,
		VerificationCodeSent = 250,
		VerificationCodeSuccess = 270,
		VerificationCodeFailedBadCode = 280,
		VerificationCodeFailedLockedOut = 290,
		Lockout = 300,
		NewRegister = 400,
		EmailConfirmed = 500,
		EmailConfirmFailed = 510,
		EmailConfirmFailedUnknownUser = 520,
		PhoneConfirmed = 600,
		ForgotPasswordSuccess = 700,
		ForgotPasswordFailed = 710,
		ForgotPasswordFailedUnknownUser = 710,
		ForgotPasswordFailedProfileNotFound = 720,
		PasswordResetSuccess = 800,
		PasswordResetFailed = 810,
		PasswordResetFailedUnknownUser = 820,
		NotAuthorized = 2400,
		FileNotFound = 2500,
		BadRequest = 2600,
	}

	public enum UserActionCategoryEnum : int
	{
		Cart = 100,
		Catalog = 200,
		Checkout = 300,
		GStore = 400,
		Notifications = 500,
		Orders = 600,
		Page = 700,
		Profile = 800,
		StoreFrontFile = 900
	}

	public enum UserActionActionEnum : int
	{
		[Display(Name = "", Description = "", GroupName = "Cart")]
		Cart_View = 10010,

		[Display(Name = "", Description = "", GroupName = "Cart")]
		Cart_AddToCartSuccess = 10021,

		[Display(Name = "", Description = "", GroupName = "Cart")]
		Cart_AddToCartFailure = 10022,

		[Display(Name = "", Description = "", GroupName = "Cart")]
		Cart_RemoveFromCart = 10028,

		[Display(Name = "", Description = "", GroupName = "Cart")]
		Cart_ApplyDiscountCodeSuccess = 10031,

		[Display(Name = "", Description = "", GroupName = "Cart")]
		Cart_ApplyDiscountCodeFailure = 10032,

		[Display(Name = "", Description = "", GroupName = "Cart")]
		Cart_CheckoutStarted = 10090,

		[Display(Name = "", Description = "", GroupName = "Catalog")]
		Catalog_ViewCategory = 20010,

		[Display(Name = "", Description = "", GroupName = "Catalog")]
		Catalog_ViewCategoryNotFound = 20011,

		[Display(Name = "", Description = "", GroupName = "Catalog")]
		Catalog_ViewProduct = 20020,

		[Display(Name = "", Description = "", GroupName = "Catalog")]
		Catalog_ViewProductNotFound = 20021,

		[Display(Name = "", Description = "", GroupName = "Checkout")]
		Checkout_Started = 30010,

		[Display(Name = "", Description = "", GroupName = "Checkout")]
		Checkout_SelectedLogInOrGuest = 30020,

		[Display(Name = "", Description = "", GroupName = "Checkout")]
		Checkout_CompletedDeliveryInfo = 30030,

		[Display(Name = "", Description = "", GroupName = "Checkout")]
		Checkout_SelectedDeliveryMethod = 30040,

		[Display(Name = "", Description = "", GroupName = "Checkout")]
		Checkout_EnteredPaymentInfo = 30050,

		[Display(Name = "", Description = "", GroupName = "Checkout")]
		Checkout_PlacedOrder = 30060,

		[Display(Name = "", Description = "", GroupName = "GStore")]
		GStore_ViewAbout = 40001,

		[Display(Name = "", Description = "", GroupName = "Notifications")]
		Notifications_Send = 50011,

		[Display(Name = "", Description = "", GroupName = "Notifications")]
		Notifications_View = 50012,

		[Display(Name = "", Description = "", GroupName = "Notifications")]
		Notifications_Delete = 50013,

		[Display(Name = "", Description = "", GroupName = "Notifications")]
		Notifications_SentToEmail = 50014,

		[Display(Name = "", Description = "", GroupName = "Notifications")]
		Notifications_SentToSms = 50015,

		[Display(Name = "", Description = "", GroupName = "Order Status")]
		Orders_View = 60010,

		[Display(Name = "", Description = "", GroupName = "Order Status")]
		Orders_NotFound = 60020,

		[Display(Name = "", Description = "", GroupName = "Order Status")]
		Orders_DigitalDownload_Success = 60030,

		[Display(Name = "", Description = "", GroupName = "Order Status")]
		Orders_DigitalDownload_Failure = 60040,

		[Display(Name = "", Description = "", GroupName = "Page")]
		Page_Edited = 70010,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_UpdateNotifications = 80010,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_SendTestEmail = 80011,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_SendTestSms = 80012,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_AddPhoneNumber = 80020,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_VerifyPhoneNumber_GetCode = 80021,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_VerifyPhoneNumber_VerifyCode = 80022,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_RemovePhoneNumber = 80023,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_PasswordChanged = 80030,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_ConfirmEmailSent = 80040,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_EnableTwoFactorAuth = 80090,

		[Display(Name = "", Description = "", GroupName = "Profile")]
		Profile_DisableTwoFactorAuth = 80091,

		[Display(Name = "", Description = "", GroupName = "Store Front")]
		StoreFrontFile_Uploaded = 90010,

	}

}