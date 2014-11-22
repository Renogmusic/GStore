using GStore.Models;
using GStore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Identity
{
	public class AuthorizeGStoreAction : System.Web.Mvc.AuthorizeAttribute
	{
		protected List<Identity.GStoreAction> _actions;
		protected bool _allowAnyMatch = false;

		protected bool _treatInactiveStoreFrontAsActive = false;

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

			Data.IGstoreDb db = Data.RepositoryFactory.StoreFrontRepository(httpContext);
			UserProfile userProfile = db.GetCurrentUserProfile(true, true);
			StoreFront storeFront = db.GetCurrentStoreFront(httpContext.Request, false, _treatInactiveStoreFrontAsActive);
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
			base.HandleUnauthorizedRequest(filterContext);
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