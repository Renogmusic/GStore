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
                name: "StoreAdmin_default",
                url: "StoreAdmin/{controller}/{action}/{id}",
				defaults: new {controller = "StoreAdmin", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}