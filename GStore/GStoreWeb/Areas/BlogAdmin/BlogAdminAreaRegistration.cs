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
					name: "Stores-BlogAdmin_GStore",
					url: "Stores/{urlstorename}/BlogAdmin/GStore/{action}",
					defaults: new { controller = "GStore", action = "About" },
					namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
				);

				context.MapRoute(
					name: "Stores-BlogAdmin_Default",
					url: "Stores/{urlstorename}/BlogAdmin/{action}/{blogId}/{blogEntryId}",
					defaults: new { controller = "BlogAdmin", action = "Index", blogId = UrlParameter.Optional, blogEntryId = UrlParameter.Optional },
					namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
				);
			}

			context.MapRoute(
				name: "BlogAdmin_GStore",
				url: "BlogAdmin/GStore/{action}",
				defaults: new { controller = "GStore", action = "About" },
				namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
			);

			context.MapRoute(
				name: "BlogAdmin_Default",
				url: "BlogAdmin/{action}/{blogId}/{blogEntryId}",
				defaults: new { controller = "BlogAdmin", action = "Index", blogId = UrlParameter.Optional, blogEntryId = UrlParameter.Optional },
				namespaces: new[] { "GStoreWeb.Areas.BlogAdmin.Controllers" }
			);

		}

    }
}