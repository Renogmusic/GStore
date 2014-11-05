using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using GStore.Data;
using GStore.Models.Extensions;
using GStore.Models;
using GStore.AppHtmlHelpers;

namespace GStore.Controllers.BaseClass
{
	public abstract class BaseController : Controller
	{
		protected IGstoreDb _dbContext = null;

		public BaseController()
		{
		}

		public BaseController(IGstoreDb dbContext)
		{
			_dbContext = dbContext;
		}

		/// <summary>
		/// GStore DB Repository (already initialized by the base controller object)
		/// </summary>
		public IGstoreDb GStoreDb
		{  
			get
			{
				if (_dbContext == null)
				{
					try
					{
						_dbContext = Data.RepositoryFactory.StoreFrontRepository(HttpContext, true, false);
					}
					catch (Exception ex)
					{
						throw new ApplicationException("Error getting Store Front repository", ex);
					}
				}
				return _dbContext;
			}
			set
			{
				_dbContext = value;
			}
		}

		public StoreFront CurrentStoreFront
		{
			get
			{
				try
				{
					return GStoreDb.GetCurrentStoreFront(Request, true);
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Cannot find active current store front", ex);
				}
			}
		}

		/// <summary>
		/// Used only in exception handler
		/// </summary>
		public int? CurrentStoreFrontIdOrNull
		{
			get
			{
				StoreFront storeFront = GStoreDb.GetCurrentStoreFront(Request, false);
				if (storeFront == null)
				{
					return null;
				}
				return storeFront.StoreFrontId;
			}
		}

		public Client CurrentClient
		{
			get
			{
				try
				{
					return CurrentStoreFront.Client;
				}
				catch (Exception ex)
				{
					throw new ApplicationException("Cannot find active current client", ex);
				}
			}
		}

		/// <summary>
		/// Used only in exception handler
		/// </summary>
		public int? CurrentClientIdOrNull
		{
			get
			{
				if (CurrentStoreFrontIdOrNull == null)
				{
					return null;
				}
				return CurrentStoreFront.Client.ClientId;
			}
		}

		public UserProfile CurrentUserProfile
		{
			get
			{
				return GStoreDb.GetCurrentUserProfile(false);
			}
		}

		/// <summary>
		/// Used only in exception handler
		/// </summary>
		public int? CurrentUserProfileIdOrNull
		{
			get
			{
				if (CurrentUserProfile == null)
				{
					return null;
				}
				return CurrentUserProfile.UserProfileId;
			}
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			ExceptionHandler(filterContext);
			base.OnException(filterContext);
		}

        protected new ActionResult HttpNotFound(string statusDescription = null)
        {
			throw new HttpException((int)HttpStatusCode.NotFound, statusDescription ?? "");
        }

		protected HttpUnauthorizedResult HttpUnauthorized(string statusDescription = null)
		{
			throw new HttpException((int)System.Net.HttpStatusCode.Unauthorized, statusDescription ?? "");
		}

		protected HttpUnauthorizedResult HttpForbidden(string statusDescription = null)
		{
			throw new HttpException((int)System.Net.HttpStatusCode.Forbidden, statusDescription ?? "");
		}

		protected class HttpStatusCodeResult : ActionResult
		{
			public HttpStatusCodeResult(HttpStatusCode code)
			{
				throw new HttpException((int)code, "");
			}

			public override void ExecuteResult(ControllerContext context)
			{
				throw new NotImplementedException();
			}
		}

		private void ExceptionHandler(ExceptionContext filterContext)
		{
			if (!Properties.Settings.Default.AppFriendlyExceptionHandling)
			{
				return;
			}

			string action = filterContext.RouteData.Values["action"].ToString();
			bool exceptionLogged = false;

			if ((!filterContext.ExceptionHandled) && (action.ToLower() != "error"))
			{
				if (filterContext.Exception is HttpCompileException)
				{
					HttpCompileException httpCompileEx = filterContext.Exception as HttpCompileException;
					Exceptions.ExceptionHandler.HandleHttpCompileException(httpCompileEx, false, true, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					exceptionLogged = true;
					return;
				}
				else if (filterContext.Exception is HttpParseException)
				{
					HttpParseException httpParseEx = filterContext.Exception as HttpParseException;
					Exceptions.ExceptionHandler.HandleHttpParseException(httpParseEx, false, true, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					exceptionLogged = true;
					return;
				}
				else if (filterContext.Exception is HttpException)
				{
					HttpException httpEx = filterContext.Exception as HttpException;
					Exceptions.ExceptionHandler.HandleHttpException(httpEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}

				string controller = filterContext.RouteData.Values["controller"].ToString();
				string message = "Error Exception in " + controller + "->" + action
					+ " \n-Controller: " + controller
					+ " \n-Action: " + action;

				Exception ex = filterContext.Exception;
				if (ex != null)
				{
					message += " \n-Exception: " + ex.Message
						+ " \n-Exception.ToString: " + ex.ToString()
						+ " \n-Stack Trace: " + ex.StackTrace;
				}

				if (!exceptionLogged)
				{
					_dbContext.LogSystemEvent(HttpContext, RouteData, controller + " -> " + action, Models.SystemEventLevel.ApplicationException, message, this);
				}

				if (string.IsNullOrEmpty(controller))
				{
					controller = "(unknown controller)";
				}
				if (string.IsNullOrEmpty(action))
				{
					action = "(unknown action)";
				}

				Response.Clear();
				filterContext.ExceptionHandled = true;
				//set status code = 500, or other code?
				View("Error", new HandleErrorInfo(filterContext.Exception, controller, action)).ExecuteResult(this.ControllerContext);
			}

		}


		/// <summary>
		/// Adds a message to the user's user message alerts (top)
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="userMessageType"></param>
		public void AddUserMessage(string title, string message, UserMessageType userMessageType)
		{
			if (!this.TempData.ContainsKey("UserMessages"))
			{
				this.TempData.Add("UserMessages", new List<UserMessage>());
			}

			List<UserMessage> userMessages = (List<UserMessage>)this.TempData["UserMessages"];
			userMessages.Add(new UserMessage(title, message, userMessageType));
		}

		/// <summary>
		/// Adds a message to the footer message alerts (bottom)
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="userMessageType"></param>
		public void AddUserMessageBottom(string title, string message, UserMessageType userMessageType)
		{
			if (!this.TempData.ContainsKey("UserMessagesBottom"))
			{
				this.TempData.Add("UserMessagesBottom", new List<UserMessage>());
			}

			List<UserMessage> userMessagesBottom = (List<UserMessage>)this.TempData["UserMessagesBottom"];
			userMessagesBottom.Add(new UserMessage(title, message, userMessageType));
		}

		public void AddGaEvent(string category, string action, string label)
		{
			if (!this.TempData.ContainsKey("GaEvents"))
			{
				this.TempData.Add("GaEvents", new List<GaEvent>());
			}

			List<GaEvent> gaEvents = (List<GaEvent>)this.TempData["GaEvents"];
			gaEvents.Add(new GaEvent(category, action, label));
		}

	}
}