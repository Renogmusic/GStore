using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GStore.Data;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("Payment")]
	public class Payment : BaseClasses.StoreFrontRecordUserProfileOptional
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Payment Id")]
		public int PaymentId { get; set; }

		[Required]
		[Display(Name = "Cart Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int CartId { get; set; }

		[ForeignKey("CartId")]
		public virtual Cart Cart { get; set; }

		public bool IsProcessed { get; set; }

		public string TransactionId { get; set; }

	}
}