using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin
{
	public class StoreAdminAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "StoreAdmin";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			if (Settings.AppEnableStoresVirtualFolders)
			{
				context.MapRoute(
					name: "Stores-StoreAdmin_Root",
					url: "Stores/{urlstorename}/StoreAdmin",
					defaults: new { controller = "StoreAdmin", action = "Index" },
					namespaces: new[] { "GStore.Areas.StoreAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-StoreAdmin_GStore",
					url: "Stores/{urlstorename}/StoreAdmin/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStore.Areas.StoreAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-StoreAdmin_Default",
					url: "Stores/{urlstorename}/StoreAdmin/{controller}/{action}/{id}",
					defaults: new { action = "Manager", id = UrlParameter.Optional },
					namespaces: new[] { "GStore.Areas.StoreAdmin.Controllers" }
				);
			}


			context.MapRoute(
				name: "StoreAdmin_Root",
				url: "StoreAdmin",
				defaults: new { controller = "StoreAdmin", action = "Index" },
				namespaces: new[] { "GStore.Areas.StoreAdmin.Controllers" }
			);

			context.MapRoute(
				name: "StoreAdmin_GStore",
				url: "StoreAdmin/GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStore.Areas.StoreAdmin.Controllers" }
			);

			context.MapRoute(
				name: "StoreAdmin_Default",
				url: "StoreAdmin/{controller}/{action}/{id}",
				defaults: new { action = "Manager", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Areas.StoreAdmin.Controllers" }
			);



		}
	}
}