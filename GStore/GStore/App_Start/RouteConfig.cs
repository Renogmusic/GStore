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
				defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "GStore",
				url: "GStore/{action}",
				defaults: new { controller = "GStore", action = "About"},
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Profile",
				url: "Profile/{action}/{id}",
				defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Notifications",
				url: "Notifications/{action}/{id}",
				defaults: new { controller = "Notifications", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Products",
				url: "Products/{urlName}",
				defaults: new { controller = "Catalog", action = "ViewProductByName", urlName = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Category",
				url: "Category/{urlName}",
				defaults: new { controller = "Catalog", action = "ViewCategoryByName", urlName = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Catalog",
				url: "Catalog/{action}/{id}",
				defaults: new { controller = "Catalog", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Images",
				url: "Images/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Images", path = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Styles",
				url: "Styles/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Styles", path = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "JS",
				url: "JS/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Scripts", path = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Themes",
				url: "Themes/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Themes", path = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "Fonts",
				url: "Fonts/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Fonts", path = UrlParameter.Optional },
				namespaces: new[] { "GStore.Controllers" }
			);

			//routes.MapRoute(
			//	name: "Default",
			//	url: "{controller}/{action}/{id}",
			//	defaults: new { action = "Index", id = UrlParameter.Optional }
			//);

			routes.MapRoute(
				name: "DynamicPageEditRoute",
				url: "Edit/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "Edit" },
				namespaces: new[] { "GStore.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicPageRoute",
				url: "{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "Display" },
				namespaces: new[] { "GStore.Controllers" }
			);
		}
	}

	//public class DynamicPageConstraint : IRouteConstraint
	//{
	//	public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
	//	{
	//		if (values == null)
	//		{
	//			throw new ApplicationException("No route values");
	//		}

	//		Data.IGstoreDb db = Data.RepositoryFactory.StoreFrontRepository(httpContext);
	//		Models.StoreFront storeFront = db.GetCurrentStoreFront(httpContext.Request, true, false);

	//		string url = "/";
	//		if (values[parameterName] != null)
	//		{
	//			url = "/" + values[parameterName].ToString().ToLower();
	//		}

	//		Models.Page page = storeFront.GetCurrentPage(httpContext.Request, false);
	//		if (page != null)
	//		{
	//			return true;
	//		}
	//		return false;
	//	}
	//}
}
