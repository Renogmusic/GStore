using System;
using System.Web.Mvc;
using GStoreData.Models;

namespace GStoreWeb.Controllers
{
	public class HomeController : AreaBaseController.RootAreaBaseController
    {
		// GET: Home
        public ActionResult Index()
        {
			//get current storefront
			StoreFrontConfiguration config = CurrentStoreFrontConfigOrNull;

			if (config == null)
			{
				return BounceToPage();
			}
			if (config.HomePageUseCatalog)
			{
				return BounceToCatalog();
			}
			return BounceToPage();
        }

		private ActionResult BounceToPage()
		{
			PageController pageController = new PageController();
			System.Web.Routing.RouteData routeData = this.RouteData;
			routeData.Values["action"] = "Display";
			routeData.Values["controller"] = "Page";
			pageController.ControllerContext = this.ControllerContext;
			return pageController.Display();
		}
		private ActionResult BounceToCatalog()
		{
			CatalogController catalogController = new CatalogController();
			System.Web.Routing.RouteData routeData = this.RouteData;
			routeData.Values["action"] = "Index";
			routeData.Values["controller"] = "Catalog";
			catalogController.ControllerContext = new ControllerContext(HttpContext, routeData, catalogController);
			return catalogController.Index();
		}

		protected override string ThemeFolderName
		{
			get { throw new NotImplementedException(); }
		}
	}
}