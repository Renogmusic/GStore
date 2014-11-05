using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GStore.Models.BaseClasses
{
	public abstract class ClientRecord: AuditFieldsAllRequired
	{
		public int ClientId { get; set; }

		[ForeignKey("ClientId")]
		public virtual Client Client { get; set; }

		public string ClientVirtualDirectoryToMap()
		{
			return "/Content/Clients/" + HttpUtility.UrlEncode(this.Client.Folder);
		}

	}
}