using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GStoreData.AppHtmlHelpers;

namespace GStoreData.Identity
{
	public class AuthorizeSystemAdmin : System.Web.Mvc.AuthorizeAttribute
	{
		/// <summary>
		/// Verifies the current logged on user is a system admin, otherwise prompts to log in
		/// </summary>
		public AuthorizeSystemAdmin()
			: base()
		{
			base.Roles = "SystemAdmin";
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			return base.AuthorizeCore(httpContext);
		}

		protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
		{
			//returns 401 result
			if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				filterContext.Controller.TempData.AddUserMessage("Log in required", "Please log in to access this page", UserMessageType.Warning);
			}
			else
			{
				filterContext.Controller.TempData.AddUserMessage("No Access", "Sorry, you do not have access to the System Admin area of the site."
					+ "<br/> Action: " + filterContext.RouteData.ToSourceString() 
					+ "<br/> Url: " + filterContext.HttpContext.Request.Url.ToString().ToHtml(), UserMessageType.Danger);
			}

			if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				base.HandleUnauthorizedRequest(filterContext);
			}
			else
			{
				RouteValueDictionary routeValues = new RouteValueDictionary(new { controller = "Account", action = "Unauthorized", area = "" });
				filterContext.Result = new RedirectToRouteResult(routeValues);
			}

		}

		public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
		{
			//set any private variables/state
			base.OnAuthorization(filterContext);
		}

		protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
		{
			//set any private variables/state
			return base.OnCacheAuthorization(httpContext);
		}


	}
}