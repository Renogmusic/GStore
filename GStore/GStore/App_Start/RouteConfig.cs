using GStore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GStore
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Account",
				url: "Account/{action}/{id}",
				defaults: new { controller = "Account", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Manage",
				url: "Manage/{action}/{id}",
				defaults: new { controller = "Manage", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Notifications",
				url: "Notifications/{action}/{id}",
				defaults: new { controller = "Notifications", action = "Index", id = UrlParameter.Optional }
			);

			//routes.MapRoute(
			//	name: "Default",
			//	url: "{controller}/{action}/{id}",
			//	defaults: new { action = "Index", id = UrlParameter.Optional }
			//);

			//routes.MapRoute(
			//		name: "DynamicPageRoute",
			//		url: "{*DynamicPageUrl}",
			//		defaults: new { controller = "Page", action = "Index" },
			//		constraints: new { DynamicPageUrl = new DynamicPageConstraint() }
			//	);

			routes.MapRoute(
				name: "DefaultDynamicPageRoute",
				url: "{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "Index" }
			);
		}
	}

	public class DynamicPageConstraint : IRouteConstraint
	{
		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			if (values == null)
			{
				throw new ApplicationException("No route values");
			}

			Data.IGstoreDb db = Data.RepositoryFactory.StoreFrontRepository(httpContext, false, false);
			Models.StoreFront storeFront = db.GetCurrentStoreFront(httpContext.Request, true);

			string url = "/";
			if (values[parameterName] != null)
			{
				url = "/" + values[parameterName].ToString().ToLower();
			}

			Models.Page page = storeFront.GetCurrentPage(httpContext.Request, false);
			if (page != null)
			{
				return true;
			}
			return false;
		}
	}
}
