using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	public enum ShippingDeliveryMethod
	{
		[Display(Name = "Standard Shipping", Description = "")]
		Economy = 10,

		[Display(Name = "Expedited Shipping", Description = "")]
		Expedited = 11,
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

		[MaxLength(50)]
		[Required]
		[Display(Name = "City", Description = "Enter your City", Prompt = "City")]
		public string City { get; set; }

		[Required]
		[Display(Name = "State", Description = "Enter your State", Prompt = "State")]
		[MaxLength(50)]
		public string State { get; set; }

		[Required]
		[MaxLength(12)]
		[Display(Name = "ZIP Code / Postal Code", Description = "Enter your ZIP Code / Postal Code", Prompt = "ZIP Code / Postal Code")]
		public string PostalCode { get; set; }

		public CountryCodeEnum CountryCode { get; set; }

		public int? WebFormResponseId { get; set; }

		[ForeignKey("WebFormResponseId")]
		public virtual WebFormResponse WebFormResponse { get; set; }

		public int? DeliveryMethodWebFormResponseId { get; set; }

		[ForeignKey("DeliveryMethodWebFormResponseId")]
		public virtual WebFormResponse DeliveryMethodWebFormResponse { get; set; }

	}
}