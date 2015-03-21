using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace GStoreData.Models
{
	[Table("DeliveryInfoDigital")]
	public class DeliveryInfoDigital : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Delivery Info Digital Id")]
		public int DeliveryInfoDigitalId { get; set; }

		[Required]
		[Display(Name = "Cart Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int CartId { get; set; }

		[ForeignKey("CartId")]
		public virtual Cart Cart { get; set; }

		public string EmailAddress { get; set; }

		public string FullName { get; set; }

		public int? WebFormResponseId { get; set; }

		[ForeignKey("WebFormResponseId")]
		public virtual WebFormResponse WebFormResponse { get; set; }
	}
}