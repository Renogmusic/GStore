using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Models.Extensions;

namespace GStore.Areas.SystemAdmin.Controllers
{
    public class SystemAdminController : BaseClasses.SystemAdminBaseController
    {
        // GET: SystemAdmin/SystemAdmin
        public ActionResult Index()
        {
			Models.StoreFront storeFront = null;
			try
			{
				 storeFront = GStoreDb.GetCurrentStoreFront(Request, true, false);
			}
			catch(Exceptions.StoreFrontInactiveException exSFI)
			{
				AddUserMessage("Error getting current storefront: Inactive", exSFI.Message, AppHtmlHelpers.UserMessageType.Danger);
			}
			catch(Exceptions.NoMatchingBindingException exNMB)
			{
				AddUserMessage("Error getting current storefront: No matching bindings", exNMB.Message, AppHtmlHelpers.UserMessageType.Danger);
			}
			catch (Exception ex)
			{
				AddUserMessage("Error getting current storefront: unknown error", ex.Message, AppHtmlHelpers.UserMessageType.Danger);
			}

            return View("Index");
        }

	}
}