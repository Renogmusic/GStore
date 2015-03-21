using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GStoreData.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace GStoreData.ViewModels
{
	public class ProfileViewModel
	{
		public bool HasPassword { get; set; }
		public IList<UserLoginInfo> Logins { get; set; }
		public string PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public bool TwoFactor { get; set; }
		public bool BrowserRemembered { get; set; }
		public bool EmailConfirmed { get; set; }
		public Identity.AspNetIdentityUser AspNetIdentityUser { get; set; }
		public UserProfile UserProfile { get; set; }
	}

	public class ProfileLoginsViewModel
	{
		public IList<UserLoginInfo> CurrentLogins { get; set; }
		public IList<AuthenticationDescription> OtherLogins { get; set; }
	}

	public class FactorViewModel
	{
		public string Purpose { get; set; }
	}

	public class SetPasswordViewModel
	{
		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class ChangePasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class AddPhoneNumberViewModel
	{
		[Required]
		[Phone]
		[Display(Name = "Phone Number", Description="Enter your phone number. Select a country, then type your number here and it will be auto-formatted.")]
		public string Number { get; set; }
	}

	public class VerifyPhoneNumberViewModel
	{
		[Required]
		[Display(Name = "Code")]
		public string Code { get; set; }

		[Required]
		[Phone]
		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; }
	}

	public class UserProfileAddressViewModel
	{
		public UserProfileAddressViewModel() { }
		public UserProfileAddressViewModel(UserProfile profile)
		{
			this.AddressLine1 = profile.AddressLine1;
			this.AddressLine2 = profile.AddressLine2;
			this.City = profile.City;
			this.State = profile.State;
			this.PostalCode = profile.PostalCode;
			this.CountryCode = profile.CountryCode;
		}

		[Required]
		[MaxLength(100)]
		[Display(Name = "Address Line 1")]
		public string AddressLine1 { get; set; }

		[MaxLength(100)]
		[Display(Name = "Address Line 2")]
		public string AddressLine2 { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "City")]
		public string City { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "State / Province")]
		public string State { get; set; }

		[Required]
		[MaxLength(12)]
		[Display(Name = "ZIP Code / Postal Code")]
		public string PostalCode { get; set; }

		[Required]
		[Display(Name = "Country")]
		public CountryCodeEnum? CountryCode { get; set; }

	}

	public class ConfigureTwoFactorViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
	}
}