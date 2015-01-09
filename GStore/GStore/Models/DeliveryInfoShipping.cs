using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GStore.Data;
using System.Web.Mvc;

namespace GStore.Models
{
	public enum ShippingDeliveryMethod
	{
		[Display(Name = "Economy Shipping", Description = "")]
		Ground = 10,

		[Display(Name = "Express 2-Day Shipping", Description = "")]
		TwoDay = 11,

		[Display(Name = "Express Next Day Shipping", Description = "")]
		Express = 12,
	}

	[Table("DeliveryInfoShipping")]
	public class DeliveryInfoShipping : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Delivery Info Shipping Id")]
		public int DeliveryInfoShippingId { get; set; }

		[Required]
		[Display(Name = "Cart Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int CartId { get; set; }

		[ForeignKey("CartId")]
		public virtual Cart Cart { get; set; }

		public ShippingDeliveryMethod? ShippingDeliveryMethod { get; set; }

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

	}
}