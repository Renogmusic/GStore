using System;
using System.Collections.Generic;
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
		public DateTime StartDateTimeUtc { get; set; }
		public DateTime EndDateTimeUtc { get; set; }
		public bool IsPending { get; set; }

	}
}