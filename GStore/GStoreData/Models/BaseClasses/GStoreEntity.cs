﻿using System;

using System.ComponentModel.DataAnnotations;

namespace GStoreData.Models.BaseClasses
{
	public abstract class GStoreEntity
	{
		[Display(Name = "Inactive", Description = "If this box is checked, this record is made inactive immediately.")]
		public bool IsPending { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		[Display(Name = "Start Date/Time", Description = "Date and Time in UTC time to start this record as active.\nIf this date is in the future, this record will not be active until the entered date and time is reached.\nIf this date is in the past, this record will always be active unless it reaches the end date or IsPending is checked.")]
		public DateTime StartDateTimeUtc { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		[Display(Name = "End Date/Time", Description = "Date and Time in UTC time to Stop this record as active.\nIf this date is in the past, this record will be inactive immediately.\nIf this date is in the future, this record will go Inactive when the date is reached.")]
		public DateTime EndDateTimeUtc { get; set; }

	}
}