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
				name: "StoreAdmin_Managers",
				url: "StoreAdmin/{controller}/{action}/{id}",
				defaults: new { action = "Manager", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Areas.StoreAdmin.Controllers" }

			);

		}
	}
}