﻿using System.Web.Mvc;
using System.Web.Routing;
using GStoreData;

namespace GStoreWeb
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			if (Settings.AppEnableStoresVirtualFolders)
			{
				routes.MapRoute(
					name: "Stores-Account",
					url: "Stores/{urlstorename}/Account/{action}/{id}",
					defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-GStore",
					url: "Stores/{urlstorename}/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Profile",
					url: "Stores/{urlstorename}/Profile/{action}/{id}",
					defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Notifications",
					url: "Stores/{urlstorename}/Notifications/{action}/{id}",
					defaults: new { controller = "Notifications", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Products",
					url: "Stores/{urlstorename}/Products/{urlName}",
					defaults: new { controller = "Catalog", action = "ViewProductByName", urlName = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Bundles",
					url: "Stores/{urlstorename}/Bundles/{urlName}",
					defaults: new { controller = "Catalog", action = "ViewBundleByName", urlName = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Category",
					url: "Stores/{urlstorename}/Category/{urlName}",
					defaults: new { controller = "Catalog", action = "ViewCategoryByName", urlName = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Catalog",
					url: "Stores/{urlstorename}/Catalog/{action}/{id}",
					defaults: new { controller = "Catalog", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Cart",
					url: "Stores/{urlstorename}/Cart/{action}/{id}",
					defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Checkout",
					url: "Stores/{urlstorename}/Checkout/{action}/{id}",
					defaults: new { controller = "Checkout", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Chat",
					url: "Stores/{urlstorename}/Chat/{action}/{id}",
					defaults: new { controller = "Chat", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				//routes.MapRoute(
				//	name: "Stores-Blog",
				//	url: "Stores/{urlstorename}/Blog/{blogUrlName}/{blogEntryUrlName}",
				//	defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				//	namespaces: new[] { "GStoreWeb.Controllers" }
				//);

				routes.MapRoute(
					name: "Stores-OrderStatus",
					url: "Stores/{urlstorename}/OrderStatus/{action}/{id}",
					defaults: new { controller = "OrderStatus", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Images",
					url: "Stores/{urlstorename}/Images/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "Images", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Audio",
					url: "Stores/{urlstorename}/Audio/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "Audio", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Styles",
					url: "Stores/{urlstorename}/Styles/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "Styles", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-JS",
					url: "Stores/{urlstorename}/JS/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "Scripts", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Themes",
					url: "Stores/{urlstorename}/Themes/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "Themes", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Fonts",
					url: "Stores/{urlstorename}/Fonts/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "Fonts", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-Pages",
					url: "Stores/{urlstorename}/Pages/{*path}",
					defaults: new { controller = "Page", action = "HtmlFile", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-CatalogContent",
					url: "Stores/{urlstorename}/CatalogContent/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "CatalogContent", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-WebPages",
					url: "Stores/{urlstorename}/WebPages/{*path}",
					defaults: new { controller = "StoreFrontFile", action = "Pages", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-DynamicPageFormSubmitRoute",
					url: "Stores/{urlstorename}/SubmitForm/{*DynamicPageUrl}",
					defaults: new { controller = "Page", action = "SubmitForm", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-DynamicPageViewRoute",
					url: "Stores/{urlstorename}/View/{*DynamicPageUrl}",
					defaults: new { controller = "Page", action = "Details", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-DynamicPageEditRoute",
					url: "Stores/{urlstorename}/Edit/{*DynamicPageUrl}",
					defaults: new { controller = "Page", action = "Edit", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-DynamicPageEditPostRoute",
					url: "Stores/{urlstorename}/UpdatePageAjax/{*DynamicPageUrl}",
					defaults: new { controller = "Page", action = "UpdatePageAjax", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-ShareByEmailRoute",
					url: "Stores/{urlstorename}/ShareByEmail/{*DynamicPageUrl}",
					defaults: new { controller = "Page", action = "ShareByEmail", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-DynamicPageEditSectionPostRoute",
					url: "Stores/{urlstorename}/UpdateSectionAjax/{*DynamicPageUrl}",
					defaults: new { controller = "Page", action = "UpdateSectionAjax", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-HomeRoute",
					url: "Stores/{urlstorename}",
					defaults: new { controller = "Home", action = "Index", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);

				routes.MapRoute(
					name: "Stores-DynamicPageRoute",
					url: "Stores/{urlstorename}/{*DynamicPageUrl}",
					defaults: new { controller = "Page", action = "Display", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Controllers" }
				);
			}

			routes.MapRoute(
				name: "Account",
				url: "Account/{action}/{id}",
				defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "GStore",
				url: "GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Profile",
				url: "Profile/{action}/{id}",
				defaults: new { controller = "Profile", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Notifications",
				url: "Notifications/{action}/{id}",
				defaults: new { controller = "Notifications", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Products",
				url: "Products/{urlName}",
				defaults: new { controller = "Catalog", action = "ViewProductByName", urlName = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Bundles",
				url: "Bundles/{urlName}",
				defaults: new { controller = "Catalog", action = "ViewBundleByName", urlName = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Category",
				url: "Category/{urlName}",
				defaults: new { controller = "Catalog", action = "ViewCategoryByName", urlName = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Catalog",
				url: "Catalog/{action}/{id}",
				defaults: new { controller = "Catalog", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Cart",
				url: "Cart/{action}/{id}",
				defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Checkout",
				url: "Checkout/{action}/{id}",
				defaults: new { controller = "Checkout", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Chat",
				url: "Chat/{action}/{id}",
				defaults: new { controller = "Chat", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Blog",
				url: "Blog/{blogUrlName}/{blogEntryUrlName}",
				defaults: new { controller = "Blog", action = "Index", blogUrlName = UrlParameter.Optional, blogEntryUrlName = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "OrderStatus",
				url: "OrderStatus/{action}/{id}",
				defaults: new { controller = "OrderStatus", action = "Index", id = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);


			routes.MapRoute(
				name: "Images",
				url: "Images/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Images", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Audio",
				url: "Audio/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Audio", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Styles",
				url: "Styles/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Styles", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "JS",
				url: "JS/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Scripts", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Themes",
				url: "Themes/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Themes", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Fonts",
				url: "Fonts/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Fonts", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "CatalogContent",
				url: "CatalogContent/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "CatalogContent", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "Pages",
				url: "Pages/{*path}",
				defaults: new { controller = "Page", action = "HtmlFile", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "WebPages",
				url: "WebPages/{*path}",
				defaults: new { controller = "StoreFrontFile", action = "Pages", path = UrlParameter.Optional, stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicPageViewRoute",
				url: "View/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "Details", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicPageEditRoute",
				url: "Edit/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "Edit", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicPageFormSubmitRoute",
				url: "SubmitForm/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "SubmitForm", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicPageEditPostRoute",
				url: "UpdatePageAjax/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "UpdatePageAjax", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicPageEditSectionPostRoute",
				url: "UpdateSectionAjax/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "UpdateSectionAjax", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicWebFormEditPostRoute",
				url: "UpdateWebFormAjax/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "UpdateWebFormAjax", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicWebFormFieldEditPostRoute",
				url: "UpdateWebFormFieldAjax/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "UpdateWebFormFieldAjax", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "ShareByEmailRoute",
				url: "ShareByEmail/{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "ShareByEmail", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "HomeRoute",
				url: "",
				defaults: new { controller = "Home", action = "Index", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

			routes.MapRoute(
				name: "DynamicPageRoute",
				url: "{*DynamicPageUrl}",
				defaults: new { controller = "Page", action = "Display", stores = UrlParameter.Optional, urlstorename = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Controllers" }
			);

		}
	}

}
