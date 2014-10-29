using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers.BaseClasses
{
	[Authorize(Roles = "AccountAdmin,NotificationAdmin,StoreAdmin,ClientAdmin,SystemAdmin")]
	public class AdminBaseController : GStore.Controllers.BaseClass.BaseController
	{
	}
}