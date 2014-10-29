using GStore.Controllers.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
	[Authorize(Roles="AccountAdmin,NotificationAdmin,StoreAdmin,ClientAdmin,SystemAdmin")]
    public class StoreAdminController : BaseClasses.AdminBaseController
    {
        // GET: StoreAdmin/Home
        public ActionResult Index()
        {

            return View();
        }
    }
}