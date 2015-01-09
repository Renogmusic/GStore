using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Routing;

namespace GStore.Models.BaseClasses
{
	public abstract class EventLogBase : AuditFieldsUserProfileOptional
	{
		public string ServerName { get; set; }
		public string ApplicationPath { get; set; }
		public string HostName { get; set; }
		public string HttpMethod { get; set; }
		public bool IsSecureConnection { get; set; }
		public string UserHostAddress { get; set; }
		public string UrlReferrer { get; set; }
		public string UserAgent { get; set; }

		public int? ClientId { get; set; }
		public int? StoreFrontId { get; set; }

		public string RawUrl { get; set; }
		public string Url { get; set; }
		public string Querystring { get; set; }
		public string Source { get; set; }

		public string Area { get; set; }
		public string Controller { get; set; }
		public string ActionName { get; set; }
		public string ActionParameters { get; set; }

		public bool Anonymous { get; set; }
		public string UserId { get; set; }
		public int? UserProfileId { get; set; }
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string Message { get; set; }
		public string SessionId { get; set; }
	}
}