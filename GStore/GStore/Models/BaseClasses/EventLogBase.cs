using System;
using System.Collections.Generic;
using System.Linq;
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

		public string Controller { get; set; }
		public string ActionName { get; set; }
		public string ActionParameters { get; set; }

		public bool Anonymous { get; set; }
		public string UserId { get; set; }
		public int? UserProfileId { get; set; }
		public string UserName { get; set; }
		public string FullName { get; set; }
		public string Message { get; set; }

		public void SetBasicFields(HttpContextBase httpContext, RouteData routeData, string source, string message, bool anonymous, UserProfile profile, Controllers.BaseClass.BaseController controller)
		{
			string siteId = httpContext.ApplicationInstance.Server.MachineName
				+ ":" + System.Web.Hosting.HostingEnvironment.SiteName
				+ httpContext.Request.ApplicationPath;

			if (controller != null)
			{
				this.StoreFrontId = controller.CurrentStoreFrontIdOrNull;
				this.ClientId = controller.CurrentClientIdOrNull;
			}

			if (routeData != null)
			{
				this.Controller = routeData.Values["controller"].ToString();
				this.ActionName = routeData.Values["action"].ToString();
				this.ActionParameters = string.Empty;

				bool isFirst = true;
				foreach (var item in routeData.Values)
				{
					if (!isFirst)
					{
						this.ActionParameters += ", ";
					}
					this.ActionParameters += item.Key + " = " + item.Value;
					isFirst = false;
				}
			}
			else
			{
				this.Controller = string.Empty;
				this.ActionName = string.Empty;
				this.ActionParameters = string.Empty;
			}

			this.ServerName = httpContext.Server.MachineName;
			this.ApplicationPath = httpContext.Request.ApplicationPath;
			this.HostName = httpContext.Request.Url.Host;
			this.HttpMethod = httpContext.Request.HttpMethod;
			this.IsSecureConnection = httpContext.Request.IsSecureConnection;
			this.UserHostAddress = httpContext.Request.UserHostAddress;
			this.UrlReferrer = (httpContext.Request.UrlReferrer == null ? "" : httpContext.Request.UrlReferrer.ToString());
			this.UserAgent = httpContext.Request.UserAgent;
			this.RawUrl = httpContext.Request.RawUrl;
			this.Url = httpContext.Request.Url.ToString();
			this.Querystring = httpContext.Request.QueryString.ToString();
			this.Source = source;
			this.Message = message;
			this.Anonymous = anonymous;

			if (profile == null)
			{
				this.UserId = null;
				this.UserName = null;
				this.UserProfileId = null;
				this.FullName = null;
			}
			else
			{
				this.UserId = profile.UserId;
				this.UserName = profile.UserName;
				this.UserProfileId = profile.UserProfileId;
				this.FullName = profile.FullName;
			}


		}

	}
}