using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Controllers
{
    public class PageController : BaseClass.PageBaseController
    {
        public ActionResult Index()
        {
			string rawUrl = Request.RawUrl;
			string viewName = base.CurrentPage.PageTemplate.ViewName;
			return View(viewName, "_Layout_" + base.CurrentPage.PageTemplate.LayoutName, base.CurrentPage);
        }

    }
}
