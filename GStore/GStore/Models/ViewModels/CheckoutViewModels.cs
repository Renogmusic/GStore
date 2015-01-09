using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GStore.Models.ViewModels
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
		}

		[Required]
		[MaxLength(150)]
		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		[Display(Name = "Email Address", Description = "Enter your email address for order confirmation and order tracking", Prompt = "Email Address")]
		public string EmailAddress { get; set; }

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
				this.AdddressL1 = info.AdddressL1;
				this.AdddressL2 = info.AdddressL2;
				this.City = info.City;
				this.EmailAddress = info.EmailAddress;
				this.FirstName = info.FirstName;
				this.LastName = info.LastName;
				this.State = info.State;
				this.ZIPCode = info.ZIPCode;
			}
		}

		[Required]
		[MaxLength(150)]
		[DataType(DataType.EmailAddress)]
		[EmailAddress]
		[Display(Name = "Email Address", Description = "Enter your email address for order confirmation and order tracking", Prompt = "Email Address")]
		public string EmailAddress { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "First Name", Description = "Enter your First Name", Prompt = "First Name")]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(50)]
		[Display(Name = "Last Name", Description = "Enter your Last Name", Prompt = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Address", Description = "Enter your street Address", Prompt = "Address")]
		public string AdddressL1 { get; set; }

		[MaxLength(100)]
		[Display(Name = "Suite/Apt #", Description = "Enter your Suite or apartment/unit number", Prompt = "Suite/Apt #")]
		public string AdddressL2 { get; set; }

		[MaxLength(50)]
		[Required]
		[Display(Name = "City", Description = "Enter your City", Prompt = "City")]
		public string City { get; set; }

		[Required]
		[Display(Name = "State", Description = "Enter your State", Prompt = "State")]
		[MaxLength(2)]
		public string State { get; set; }

		[Required]
		[Display(Name = "ZIP Code", Description = "Enter your ZIP Code", Prompt = "ZIP Code")]
		[Range(0, 99999)]
		[MaxLength(5)]
		public string ZIPCode { get; set; }

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
		public bool Continue { get; set; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			return new List<ValidationResult>();
		}
	}

	public class CheckoutConfirmOrderViewModel : CheckoutViewModelBase
	{
		public CheckoutConfirmOrderViewModel() : base() { }
		public CheckoutConfirmOrderViewModel(StoreFrontConfiguration config, Cart cart, string currentAction) : base(config, cart, currentAction) { }
		public bool Continue { get; set; }

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