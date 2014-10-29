using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace GStore.Models.BaseClasses
{
	/// <summary>
	/// Records with start date/end date and a link to client table (clientid and nav prop)
	/// </summary>
	//public abstract class ClientLiveRecord<TEntity> : ClientRecordBase where TEntity : ClientLiveRecord<TEntity>
	public abstract class ClientLiveRecord : ClientRecord
	{
		public DateTime StartDateTimeUtc { get; set; }
		public DateTime EndDateTimeUtc { get; set; }
		public bool IsPending { get; set; }

		public bool IsActiveDirect()
		{
			return IsActiveDirect(DateTime.UtcNow);
		}
		public bool IsActiveDirect(DateTime dateTime)
		{
			if (!IsPending && (StartDateTimeUtc < dateTime) && ( EndDateTimeUtc > dateTime))
			{
				return true;
			}
			return false;
		}
	}
}