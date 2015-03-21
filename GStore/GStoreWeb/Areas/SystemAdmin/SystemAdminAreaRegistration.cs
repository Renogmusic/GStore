using System.Web.Mvc;
using GStoreData;

namespace GStoreWeb.Areas.SystemAdmin
{
	public class SystemAdminAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "SystemAdmin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			if (Settings.AppEnableStoresVirtualFolders)
			{
				context.MapRoute(
					name: "Stores-SystemAdmin_GStore",
					url: "Stores/{urlstorename}/SystemAdmin/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStoreWeb.Areas.SystemAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-SystemAdmin_default",
					url: "Stores/{urlstorename}/SystemAdmin/Client/{ClientId}/{controller}/{action}/{id}",
					defaults: new { controller = "SystemAdmin", action = "Index", ClientId = 0, id = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Areas.SystemAdmin.Controllers" }
				);
			}

			context.MapRoute(
				name: "SystemAdmin_GStore",
				url: "SystemAdmin/GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStoreWeb.Areas.SystemAdmin.Controllers" }
			);

			context.MapRoute(
				name: "SystemAdmin_default",
				url: "SystemAdmin/Client/{ClientId}/{controller}/{action}/{id}",
				defaults: new { controller = "SystemAdmin", action = "Index", ClientId = 0, id = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Areas.SystemAdmin.Controllers" }
			);

			//context.MapRoute(
			//	name: "SystemAdmin_WithClientId",
			//	url: "SystemAdmin/{controller}/{action}/{id}",
			//	defaults: new { controller = "SystemAdmin", action = "Index", ClientId = 0, id = UrlParameter.Optional },
			//	namespaces: new[] { "GStoreWeb.Areas.SystemAdmin.Controllers" }
			//);

		}
	}
}