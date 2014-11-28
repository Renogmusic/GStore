﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace GStore.Models.BaseClasses
{
	/// <summary>
	/// Records with start date/end date and a link to client table and store front table (storefrontid, clientid and nav props)
	/// </summary>
	public abstract class StoreFrontLiveRecord : StoreFrontRecord
	{
		[Display(Name = "Start Date")]
		public DateTime StartDateTimeUtc { get; set; }

		[Display(Name = "End Date")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Is Pending")]
		public bool IsPending { get; set; }

	}
}