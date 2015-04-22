using System.Web.Mvc;
using GStoreData;

namespace GStoreWeb.Areas.BlogAdmin
{
    public class BlogAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BlogAdmin";
            }
        }

		public override void RegisterArea(AreaRegistrationContext context)
		{
			if (Settings.AppEnableStoresVirtualFolders)
			{
				context.MapRoute(
					name: "Stores-BlogAdmin_Root",
					url: "Stores/{urlstorename}/BlogAdmin",
					defaults: new { controller = "BlogAdmin", action = "Index" },
					namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-BlogAdmin_GStore",
					url: "Stores/{urlstorename}/BlogAdmin/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-BlogAdmin_Default",
					url: "Stores/{urlstorename}/BlogAdmin/{controller}/{action}/{id}",
					defaults: new { action = "Manager", id = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
				);
			}

			context.MapRoute(
				name: "BlogAdmin_Root",
				url: "BlogAdmin",
				defaults: new { controller = "BlogAdmin", action = "Index" },
				namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
			);

			context.MapRoute(
				name: "BlogAdmin_GStore",
				url: "BlogAdmin/GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
			);

			context.MapRoute(
				name: "BlogAdmin_Default",
				url: "BlogAdmin/{controller}/{action}/{id}",
				defaults: new { action = "Manager", id = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
			);

		}

    }
}