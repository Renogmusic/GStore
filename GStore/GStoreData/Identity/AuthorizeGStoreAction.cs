using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreData.Identity
{
	public class AuthorizeGStoreAction : System.Web.Mvc.AuthorizeAttribute
	{
		protected List<Identity.GStoreAction> _actions;
		protected bool _allowAnyMatch = false;

		protected bool _treatInactiveStoreFrontAsActive = false;
		protected bool _treatInactiveStoreFrontConfigAsActive = true;

		/// <summary>
		/// Verifies the current logged on user has permission to a specific GStoreAction for the current StoreFront, otherwise prompts to log in
		/// </summary>
		/// <param name="action"></param>
		public AuthorizeGStoreAction(Identity.GStoreAction action)
		{
			_actions = new List<GStoreAction>();
			_actions.Add(action);
			_allowAnyMatch = true;
		}

		/// <summary>
		/// Verifies the current logged on user has permission to a list of GStoreActions for the current StoreFront, if not, prompts to log in
		/// If allowAnyMatch is true, this is an OR test for each option.  If allowAnyMatch = false, this is an AND test for all options
		/// </summary>
		/// <param name="allowAnyMatch">If allowAnyMatch is true, this is an OR test for each option.  If allowAnyMatch = false, this is an AND test for all options</param>
		/// <param name="actions"></param>
		public AuthorizeGStoreAction(bool allowAnyMatch, params Identity.GStoreAction[] actions)
		{
			_actions = actions.ToList();
			_allowAnyMatch = allowAnyMatch;
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (_actions == null || _actions.Count == 0)
			{
				throw new ApplicationException("AuthorizeGStoreAction was called with no action specified. You must specify at least one or more actions to the constructor.");
			}
			if (!httpContext.User.Identity.IsAuthenticated)
			{
				return false;
			}
				
			if (httpContext.User.IsInRole("SystemAdmin"))
			{
				return true;
			}

			IGstoreDb db = RepositoryFactory.StoreFrontRepository(httpContext);
			UserProfile userProfile = db.GetCurrentUserProfile(true, true);
			StoreFront storeFront = db.GetCurrentStoreFront(httpContext.Request, false, _treatInactiveStoreFrontAsActive, _treatInactiveStoreFrontConfigAsActive);
			if (storeFront == null)
			{
				//no storefront, 
				return AuthorizationExtensions.Authorization_IsAuthorized(null, userProfile, _allowAnyMatch, _actions.ToArray());
			}
			return storeFront.Authorization_IsAuthorized(userProfile, _allowAnyMatch, _actions.ToArray());
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