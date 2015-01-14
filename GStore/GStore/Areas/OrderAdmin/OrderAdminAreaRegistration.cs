using System.Web.Mvc;

namespace GStore.Areas.OrderAdmin
{
    public class OrderAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OrderAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
			if (Properties.Settings.Current.AppEnableStoresVirtualFolders)
			{
				context.MapRoute(
					name: "Stores-OrderAdmin_Root",
					url: "Stores/{urlstorename}/OrderAdmin",
					defaults: new { controller = "OrderAdmin", action = "Index" },
					namespaces: new[] { "GStore.Areas.OrderAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-OrderAdmin_GStore",
					url: "Stores/{urlstorename}/OrderAdmin/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStore.Areas.OrderAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-OrderAdmin_Default",
					url: "Stores/{urlstorename}/OrderAdmin/{controller}/{action}/{id}",
					defaults: new { action = "Manager", id = UrlParameter.Optional },
					namespaces: new[] { "GStore.Areas.OrderAdmin.Controllers" }
				);
			}

			context.MapRoute(
				name: "OrderAdmin_Root",
				url: "OrderAdmin",
				defaults: new { controller = "OrderAdmin", action = "Index" },
				namespaces: new[] { "GStore.Areas.OrderAdmin.Controllers" }
			);

			context.MapRoute(
				name: "OrderAdmin_GStore",
				url: "OrderAdmin/GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStore.Areas.OrderAdmin.Controllers" }
			);

			context.MapRoute(
				name: "OrderAdmin_Default",
				url: "OrderAdmin/{controller}/{action}/{id}",
				defaults: new { action = "Manager", id = UrlParameter.Optional },
				namespaces: new[] { "GStore.Areas.OrderAdmin.Controllers" }
			);


        }
    }
}