using System.Web.Mvc;

namespace GStore.Areas.SystemAdmin
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
			if (Properties.Settings.Current.AppEnableStoresVirtualFolders)
			{
				context.MapRoute(
					name: "Stores-SystemAdmin_GStore",
					url: "Stores/{urlstorename}/SystemAdmin/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStore.Areas.SystemAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-SystemAdmin_default",
					url: "Stores/{urlstorename}/SystemAdmin/{controller}/{action}/{id}",
					defaults: new { controller = "SystemAdmin", action = "Index", id = UrlParameter.Optional },
					namespaces: new[] { "GStore.Areas.SystemAdmin.Controllers" }
				);
			}

			context.MapRoute(
				name: "SystemAdmin_GStore",
				url: "SystemAdmin/GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStore.Areas.SystemAdmin.Controllers" }
			);

			context.MapRoute(
				name: "SystemAdmin_default",
				url: "SystemAdmin/{controller}/{action}/{id}",
				defaults: new { controller = "SystemAdmin", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Areas.SystemAdmin.Controllers" }
			);

		}
	}
}