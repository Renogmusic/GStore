using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Models.Extensions;
using GStore.Models;

namespace GStore.Controllers.BaseClass
{
	public class UserHasAnyAdminPermission : System.Web.Mvc.AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (!base.AuthorizeCore(httpContext))
			{
				//base says user is not logged in, return false
				return false;
			}

			if (httpContext.User.IsInRole("SystemAdmin"))
			{
				return true;
			}

			Data.IGstoreDb db = Data.RepositoryFactory.SystemWideRepository(httpContext.User);
			UserProfile userProfile = db.GetCurrentUserProfile(true);
			StoreFront storeFront = db.GetCurrentStoreFront(httpContext.Request, true);
			return storeFront.HasAnyAdminPermission(userProfile);
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