using GStore.Controllers.BaseClass;
using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers.BaseClasses
{
	[UserHasAnyAdminPermission]
	public class AdminBaseController : GStore.Controllers.BaseClass.BaseController
	{
		
	}
}