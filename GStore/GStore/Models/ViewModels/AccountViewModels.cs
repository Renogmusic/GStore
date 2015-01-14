using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GStore.Models.ViewModels
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "Email", Description="Enter your Email Address")]
		public string Email { get; set; }
		public bool? CheckingOut { get; set; }
	}

	public class ExternalLoginListViewModel
	{
		public string ReturnUrl { get; set; }
		public bool? CheckingOut { get; set; }
	}

	public class SendCodeViewModel
	{
		public string SelectedProvider { get; set; }
		public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
		public string ReturnUrl { get; set; }
		public bool RememberMe { get; set; }
		public bool? CheckingOut { get; set; }
	}

	public class VerifyCodeViewModel
	{
		[Required]
		public string Provider { get; set; }

		[Required]
		[Display(Name = "Code", Description="Enter your verification Code from your Email or Text Message.", Prompt="Enter Your Verification Code (Required)")]
		public string Code { get; set; }
		public string ReturnUrl { get; set; }

		[Display(Name = "Remember this browser?", Description="Check this box to remember this browser log in\nUncheck this box if you are on a shared computer.")]
		public bool RememberBrowser { get; set; }

		[Display(Name = "Remember this browser?", Description = "Check this box to remember this browser log in\nUncheck this box if you are on a shared computer.")]
		public bool RememberMe { get; set; }
		public bool? CheckingOut { get; set; }
	}

	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Email Address", Description = "Enter your Email Address to reset your password.")]
		public string Email { get; set; }
		public bool? CheckingOut { get; set; }
	}

	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Email", Description = "Enter your Email Address to log in.")]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password", Description = "Enter your Password to log in")]
		public string Password { get; set; }

		[Display(Name = "Remember me", Description = "Check this box to remember your login")]
		public bool RememberMe { get; set; }

		public bool? CheckingOut { get; set; }
	}

	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email", Description = "Enter your Email Address")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Your Name", Description = "Enter your Full Name. \nExample: John Doe")]
		public string FullName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password", Description = "Enter a Password for this site")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password", Description = "Confirm your Password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Display(Name = "Email me Site Updates", Description = "Check this box to get Email when this site is updated")]
		public bool NotifyOfSiteUpdates { get; set; }

		[Display(Name = "Send Me More Info", Description = "Check this box if you would like more information sent to you by Email")]
		public bool SendMeMoreInfo { get; set; }

		[Display(Name = "Leave us a note", Description = "Leave us a message to help us assist you better")]
		[DataType(DataType.MultilineText)]
		public string SignupNotes { get; set; }

		public bool? CheckingOut { get; set; }
	}

	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email", Description = "Enter your Email Address to reset your password.", Prompt = "Enter Your Email Address (Required)")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password", Description = "Enter a new password with at least 6 characters.", Prompt = "Enter a new Password (Required)")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password", Description="Comfirm your password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Display(Name = "Password Reset Code", Description="Password Reset Code")]
		public string Code { get; set; }

		public bool? CheckingOut { get; set; }
	}

	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email", Description="Enter your Email address to reset your password.", Prompt="Enter your Email Address (Required)")]
		public string Email { get; set; }

		public bool? CheckingOut { get; set; }
	}
}
