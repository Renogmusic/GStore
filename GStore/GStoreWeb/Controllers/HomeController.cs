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
				return HomePageResult();
			}
			if (config.HomePageUseCatalog)
			{
				return CatalogIndexResult();
			}
			if (config.HomePageUseBlog)
			{
				return BlogIndexResult();
			}

			return HomePageResult();
        }

		protected ActionResult HomePageResult()
		{
			PageController pageController = new PageController();
			System.Web.Routing.RouteData routeData = this.RouteData;
			routeData.Values["action"] = "Display";
			routeData.Values["controller"] = "Page";
			routeData.DataTokens["area"] = "";

			pageController.ControllerContext = this.ControllerContext;
			pageController.TempData = this.TempData;
			return pageController.Display();
		}

		protected ActionResult CatalogIndexResult()
		{
			CatalogController catalogController = new CatalogController();
			System.Web.Routing.RouteData routeData = this.RouteData;
			routeData.Values["action"] = "Index";
			routeData.Values["controller"] = "Catalog";
			routeData.DataTokens["area"] = "";

			catalogController.ControllerContext = new ControllerContext(HttpContext, routeData, catalogController);
			catalogController.TempData = this.TempData;
			return catalogController.Index();
		}

		protected ActionResult BlogIndexResult()
		{
			BlogController blogController = new BlogController();
			System.Web.Routing.RouteData routeData = this.RouteData;
			routeData.Values["action"] = "Index";
			routeData.Values["controller"] = "Blog";
			routeData.DataTokens["area"] = "";

			blogController.ControllerContext = new ControllerContext(HttpContext, routeData, blogController);
			blogController.TempData = this.TempData;
			return blogController.Index(null, null);
		}

		protected override string ThemeFolderName
		{
			get { throw new NotImplementedException(); }
		}
	}
}