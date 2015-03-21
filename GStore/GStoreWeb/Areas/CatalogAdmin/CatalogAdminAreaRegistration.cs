using System.Web.Mvc;
using GStoreData;

namespace GStoreWeb.Areas.CatalogAdmin
{
    public class CatalogAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CatalogAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
			if (Settings.AppEnableStoresVirtualFolders)
			{
				context.MapRoute(
					name: "Stores-CatalogAdmin_Root",
					url: "Stores/{urlstorename}/CatalogAdmin",
					defaults: new { controller = "CatalogAdmin", action = "Index" },
					namespaces: new[] { "GStoreWeb.Areas.CatalogAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-CatalogAdmin_GStore",
					url: "Stores/{urlstorename}/CatalogAdmin/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStoreWeb.Areas.CatalogAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-CatalogAdmin_Default",
					url: "Stores/{urlstorename}/CatalogAdmin/{controller}/{action}/{id}",
					defaults: new { action = "Manager", id = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Areas.CatalogAdmin.Controllers" }
				);
			}

			context.MapRoute(
				name: "CatalogAdmin_Root",
				url: "CatalogAdmin",
				defaults: new { controller = "CatalogAdmin", action = "Index" },
				namespaces: new[] { "GStoreWeb.Areas.CatalogAdmin.Controllers" }
			);

			context.MapRoute(
				name: "CatalogAdmin_GStore",
				url: "CatalogAdmin/GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStoreWeb.Areas.CatalogAdmin.Controllers" }
			);

			context.MapRoute(
				name: "CatalogAdmin_Default",
				url: "CatalogAdmin/{controller}/{action}/{id}",
				defaults: new { action = "Manager", id = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Areas.CatalogAdmin.Controllers" }
			);

		}
    }
}