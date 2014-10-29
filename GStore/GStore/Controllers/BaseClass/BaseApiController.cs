using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GStore.Controllers.BaseClass
{
	public abstract class BaseApiController : ApiController
	{
		//protected override void OnException(ExceptionContext filterContext)
		//{
		//	ExceptionHandler(filterContext);
		//	base.OnException(filterContext);
		//}

		//private void ExceptionHandler(ExceptionContext filterContext)
		//{
		//	string action = filterContext.RouteData.Values["action"].ToString();
		//	if ((!filterContext.ExceptionHandled) && (action.ToLower() != "error"))
		//	{
		//		string controller = filterContext.RouteData.Values["controller"].ToString();
		//		string message = "Error Exception in " + controller + action
		//			+ " \n-Controller: " + controller
		//			+ " \n-Action: " + action;

		//		Exception ex = filterContext.Exception;
		//		if (ex != null)
		//		{
		//			message += " \n-Exception: " + ex.Message
		//				+ " \n-Exception.ToString: " + ex.ToString()
		//				+ " \n-Stack Trace: " + ex.StackTrace;
		//		}


		//		GStoreDbContext ctx = new GStoreDbContext(User);
		//		ctx.LogSystemEvent(HttpContext, controller + action, SystemEventLevel.ApplicationException, message);
		//		filterContext.ExceptionHandled = true;

		//		Response.Clear();
		//		if (string.IsNullOrEmpty(controller))
		//		{
		//			controller = "(unknown controller)";
		//		}
		//		if (string.IsNullOrEmpty(action))
		//		{
		//			action = "(unknown action)";
		//		}
		//		Response.Clear();
		//		View("Error", new HandleErrorInfo(filterContext.Exception, controller, action)).ExecuteResult(this.ControllerContext);
		//	}

		//}

	}
}