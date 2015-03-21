using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class CheckoutLogInOrGuestViewModel : CheckoutViewModelBase
	{
		public CheckoutLogInOrGuestViewModel() : base() { }
		public CheckoutLogInOrGuestViewModel(StoreFrontConfiguration config, Cart cart, string currentAction) : base(config, cart, currentAction) { }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}
	}

	public class CheckoutDeliveryInfoDigitalOnlyViewModel : CheckoutViewModelBase
	{
		public CheckoutDeliveryInfoDigitalOnlyViewModel() : base() { }
		public CheckoutDeliveryInfoDigitalOnlyViewModel(StoreFrontConfiguration config, Cart cart, string currentAction) : base(config, cart, currentAction)
		{
			this.EmailAddress = cart.Email;
			this.FullName = cart.FullName;
		}

		[Required]
		[MaxLength(150)]
		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		[Display(Name = "Email Address", Description = "Enter your email address for order confirmation and order tracking", Prompt = "Email Address")]
		public string EmailAddress { get; set; }

		[Required]
		[MaxLength(150)]
		[DataType(DataType.Text)]
		[Display(Name = "Your Name", Description = "Enter your full name for order confirmation and order tracking", Prompt = "Your Name")]
		public string FullName { get; set; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}
	}

	public class CheckoutDeliveryInfoShippingViewModel : CheckoutViewModelBase
	{
		public CheckoutDeliveryInfoShippingViewModel() : base() { }
		public CheckoutDeliveryInfoShippingViewModel(StoreFrontConfiguration config, Cart cart, string currentAction) : base(config, cart, currentAction)
		{
			this.EmailAddress = cart.Email;
			if (cart.DeliveryInfoShipping != null)
			{
				DeliveryInfoShipping info = cart.DeliveryInfoShipping;
				this.EmailAddress = info.EmailAddress;
				this.FullName = info.FullName;
				this.AdddressL1 = info.AdddressL1;
				this.AdddressL2 = info.AdddressL2;
				this.City = info.City;
				this.State = info.State;
				this.PostalCode = info.PostalCode;
				this.CountryCode = info.CountryCode;
			}
			else if (cart.UserProfile != null)
			{
				this.FullName = cart.UserProfile.FullName;
				this.AdddressL1 = cart.UserProfile.AddressLine1;
				this.AdddressL2 = cart.UserProfile.AddressLine2;
				this.City = cart.UserProfile.City;
				this.State = cart.UserProfile.State;
				this.PostalCode = cart.UserProfile.PostalCode;
				this.CountryCode = cart.UserProfile.CountryCode;
			}
		}

		[Required]
		[MaxLength(150)]
		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		[Display(Name = "Email Address", Description = "Enter your email address for order confirmation and order tracking", Prompt = "Email Address")]
		public string EmailAddress { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Name", Description = "Enter your name, or the name of the recipient", Prompt = "Name")]
		public string FullName { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Address", Description = "Enter your street Address", Prompt = "Address")]
		public string AdddressL1 { get; set; }

		[MaxLength(100)]
		[Display(Name = "Suite/Apt #", Description = "Enter your Suite or apartment/unit number", Prompt = "Suite/Apt #")]
		public string AdddressL2 { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "City", Description = "Enter your City", Prompt = "City")]
		public string City { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "State / Province", Description = "Enter your State", Prompt = "State")]
		public string State { get; set; }

		[Required]
		[MaxLength(12)]
		[Display(Name = "ZIP Code / Postal Code", Description = "Enter your ZIP Code or Postal Code", Prompt = "ZIP Code / Postal Code")]
		public string PostalCode { get; set; }

		[Required]
		[Display(Name = "Country")]
		public CountryCodeEnum? CountryCode { get; set; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}
	}

	public class CheckoutDeliveryMethodShippingViewModel : CheckoutViewModelBase
	{
		public CheckoutDeliveryMethodShippingViewModel() : base() { }
		public CheckoutDeliveryMethodShippingViewModel(StoreFrontConfiguration config, Cart cart, string currentAction) : base(config, cart, currentAction)
		{
			if (cart.DeliveryInfoShipping == null)
			{
				throw new ArgumentNullException("cart.DeliveryInfoShipping");
			}
			this.ShippingDeliveryMethod = cart.DeliveryInfoShipping.ShippingDeliveryMethod;
		}

		[Required]
		[Display(Name = "Shipping Method", Description = "Select a Shipping Method")]
		public ShippingDeliveryMethod? ShippingDeliveryMethod { get; set; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}
	}

	public class CheckoutPaymentInfoViewModel : CheckoutViewModelBase
	{
		public CheckoutPaymentInfoViewModel() : base() { }
		public CheckoutPaymentInfoViewModel(StoreFrontConfiguration config, Cart cart, string currentAction) : base(config, cart, currentAction) { }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}
	}

	public class CheckoutConfirmOrderViewModel : CheckoutViewModelBase
	{
		public CheckoutConfirmOrderViewModel() : base() { }
		public CheckoutConfirmOrderViewModel(StoreFrontConfiguration config, Cart cart, string currentAction) : base(config, cart, currentAction) { }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}
	}

	public abstract class CheckoutViewModelBase : IValidatableObject
	{
		public CheckoutViewModelBase() { }
		public CheckoutViewModelBase(StoreFrontConfiguration config, Cart cart, string currentAction)
		{
			this.CurrentAction = currentAction;
			this.Config = config;
			this.Cart = cart;
		}
		public void UpdateForRepost(StoreFrontConfiguration config, Cart cart, string currentAction)
		{
			this.Config = config;
			this.Cart = cart;
			this.CurrentAction = currentAction;
		}

		[Editable(false)]
		public string CurrentAction { get; protected set; }

		[Editable(false)]
		public Cart Cart { get; protected set; }

		[Editable(false)]
		public StoreFrontConfiguration Config { get; protected set; }
		
		public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
	}
}