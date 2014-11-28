using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using GStore.Data;
using GStore.Models;
using GStore.AppHtmlHelpers;
using GStore.Exceptions;

namespace GStore.Controllers.BaseClass
{
	public abstract class BaseController : Controller
	{
		/// <summary>
		/// Layout name to use for display, used internally by LayoutNameToUse. this property may throw an error, then LayoutNameToUse will handle it
		/// </summary>
		protected abstract string LayoutName { get; }

		protected IGstoreDb _dbContext = null;
		protected bool _throwErrorIfStoreFrontNotFound = true;
		protected bool _throwErrorIfUserProfileNotFound = false;
		protected bool _throwErrorIfAnonymous = false;
		protected bool _useInactiveStoreFrontAsActive = false;

		protected bool _currentStoreFrontError = false;

		public void SetErrorContext()
		{
			_throwErrorIfStoreFrontNotFound = false;
		}

		/// <summary>
		/// Returns the controller layout name, or handles errors and decides which layout to use
		/// </summary>
		public virtual string LayoutNameToUse
		{
			get
			{
				try 
				{	        
					return LayoutName;
				}
				catch (Exceptions.DynamicPageInactiveException dpiEx)
				{
					//dynamic page is inactive, return storefront default layout
					return dpiEx.StoreFront.DefaultNewPageLayoutName;
				}
				catch (Exceptions.DynamicPageNotFoundException dpnfEx)
				{
					//dynamic page not found, return storefront default layout
					return dpnfEx.StoreFront.DefaultNewPageLayoutName;
				}
				catch (Exceptions.NoMatchingBindingException)
				{
					//no storefront found, return app default layout
					return Properties.Settings.Current.AppDefaultLayoutName;
				}
				catch (Exceptions.StoreFrontInactiveException)
				{
					//storefront is inactive, return app default layout
					return Properties.Settings.Current.AppDefaultLayoutName;
				}
				catch (Exception)
				{
					//unknown exception, use storefront default layout
					if (_currentStoreFrontError || CurrentStoreFrontOrNull == null)
					{
						//couldn't get storefront, use app default layout
						return Properties.Settings.Current.AppDefaultLayoutName;
					}
					return CurrentStoreFrontOrThrow.DefaultNewPageLayoutName;
				}
			}
		}

		public virtual bool LogActionsAsPageViews
		{
			get
			{
				return _logActionsAsPageViews;
			}
			protected set
			{
				_logActionsAsPageViews = value;
			}
		}
		protected bool _logActionsAsPageViews = true;


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
		public virtual IGstoreDb GStoreDb
		{
			get
			{
				if (_dbContext == null)
				{
					_dbContext = Data.RepositoryFactory.StoreFrontRepository(HttpContext);
				}
				return _dbContext;
			}
			set
			{
				_dbContext = value;
			}
		}

		public virtual StoreFront CurrentStoreFrontOrNull
		{
			get
			{
				if (_currentStoreFrontError)
				{
					return null;
				}
				try
				{
					return CurrentStoreFrontOrThrow;
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public virtual StoreFront CurrentStoreFrontOrThrow
		{
			get
			{
				try
				{
					if (_currentStoreFrontError)
					{
						return null;
					}
					//
					string rawUrl = Request.RawUrl;
					string url = Request.Url.ToString();

					return GStoreDb.GetCurrentStoreFront(Request, _throwErrorIfStoreFrontNotFound, _useInactiveStoreFrontAsActive);
				}
				catch (Exceptions.StoreFrontInactiveException exSFI)
				{
					if (_useInactiveStoreFrontAsActive)
					{
						GStoreDb.CachedStoreFront = exSFI.StoreFront;
						return exSFI.StoreFront;
					}
					_currentStoreFrontError = true;
					throw exSFI;
				}
				catch (Exceptions.NoMatchingBindingException exNMB)
				{
					if (!Properties.Settings.Current.AppEnableBindingAutoMapToFirstStoreFront)
					{
						_currentStoreFrontError = true;
						throw new NoMatchingBindingException("No Store Front found matching current site, and auto-map is disabled. Either this site is invalid or wrong bindings exist in database. Turn on Auto-Binding-Map by setting Settings.AppEnableBindingAutoMapToFirstStoreFront to true \n" + exNMB.Message, exNMB.Uri);
					}
					StoreBinding binding = GStoreDb.AutoMapBinding(this);
					try
					{
						return GStoreDb.GetCurrentStoreFront(Request, _throwErrorIfStoreFrontNotFound, _useInactiveStoreFrontAsActive);
					}
					catch (Exception)
					{
						_currentStoreFrontError = true;
						throw;
					} 
				}
				catch (Exceptions.DatabaseErrorException dbEx)
				{
					System.Diagnostics.Trace.WriteLine("--Database Error in CurrentStoreFront: " + dbEx.Message);
					System.Diagnostics.Trace.Indent();
					System.Diagnostics.Trace.WriteLine("-- inner exception: " + dbEx.InnerException.ToString());
					System.Diagnostics.Trace.Unindent();
					_currentStoreFrontError = true;
					throw dbEx;
				}
				catch (Exception ex)
				{
					_currentStoreFrontError = true; 
					throw new ApplicationException("Cannot find active current store front", ex);
				}
			}
		}

		/// <summary>
		/// Used only in exception handler
		/// </summary>
		public virtual int? CurrentStoreFrontIdOrNull
		{
			get
			{
				if (_currentStoreFrontError)
				{
					return null;
				}
				StoreFront storeFront = null;
				try
				{
					storeFront = CurrentStoreFrontOrThrow;
				}
				catch (Exception)
				{
					return null;
				}
				if (storeFront == null)
				{
					return null;
				}
				return storeFront.StoreFrontId;
			}
		}

		public virtual Client CurrentClientOrNull
		{
			get
			{
				if (CurrentStoreFrontOrNull == null)
				{
					return null;
				}
				return CurrentStoreFrontOrThrow.Client;
			}
		}

		public virtual Client CurrentClientOrThrow
		{
			get
			{
				return CurrentStoreFrontOrThrow.Client;
			}
		}

		/// <summary>
		/// Used only in exception handler
		/// </summary>
		public virtual int? CurrentClientIdOrNull
		{
			get
			{
				if (CurrentStoreFrontIdOrNull == null)
				{
					return null;
				}
				return CurrentStoreFrontOrThrow.Client.ClientId;
			}
		}

		public virtual UserProfile CurrentUserProfileOrThrow
		{
			get
			{
				return GStoreDb.GetCurrentUserProfile(true, true);
			}
		}

		public virtual UserProfile CurrentUserProfileOrNull
		{
			get
			{
				return GStoreDb.GetCurrentUserProfile(false, false);
			}
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			ExceptionHandler(filterContext);
			base.OnException(filterContext);
		}

		protected new virtual ActionResult HttpNotFound(string statusDescription = null)
		{
			throw new HttpException((int)HttpStatusCode.NotFound, statusDescription ?? "");
		}

		protected virtual ActionResult HttpUnauthorized(string statusDescription = null)
		{
			throw new HttpException((int)System.Net.HttpStatusCode.Unauthorized, statusDescription ?? "");
		}

		/// <summary>
		/// Throws a HTTP 403 (directory listing denied / forbidden) HTTP error
		/// </summary>
		/// <param name="statusDescription"></param>
		/// <returns></returns>
		protected virtual ActionResult HttpForbidden(string statusDescription = null)
		{
			throw new HttpException((int)System.Net.HttpStatusCode.Forbidden, statusDescription ?? "");
		}

		/// <summary>
		/// Throws a HTTP 400 BadRequest exception
		/// </summary>
		/// <param name="statusDescription"></param>
		/// <returns></returns>
		protected virtual ActionResult HttpBadRequest(string statusDescription = null)
		{
			throw new HttpException((int)System.Net.HttpStatusCode.BadRequest, statusDescription ?? "");
		}

		protected override void HandleUnknownAction(string actionName)
		{
			HttpNotFound("Unknown action: " + actionName + " in " + RouteData.ToSourceString());
			return;
		}

		protected virtual void ExceptionHandler(ExceptionContext filterContext)
		{
			if (!filterContext.ExceptionHandled)
			{
				if (filterContext.Exception.GetBaseException() is DynamicPageInactiveException)
				{
					DynamicPageInactiveException exDPI = filterContext.Exception.GetBaseException() as DynamicPageInactiveException;
					Exceptions.ExceptionHandler.HandleDynamicPageInactiveException(exDPI, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}
				else if (filterContext.Exception.GetBaseException() is DynamicPageNotFoundException)
				{
					DynamicPageNotFoundException exDPNF = filterContext.Exception.GetBaseException() as DynamicPageNotFoundException;
					Exceptions.ExceptionHandler.HandleDynamicPageNotFoundException(exDPNF, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}
				else if (filterContext.Exception.GetBaseException() is NoMatchingBindingException)
				{
					NoMatchingBindingException exNMB = filterContext.Exception.GetBaseException() as NoMatchingBindingException;
					Exceptions.ExceptionHandler.HandleNoMatchingBindingException(exNMB, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}
				else if (filterContext.Exception.GetBaseException() is StoreFrontInactiveException)
				{
					StoreFrontInactiveException exSFI = filterContext.Exception.GetBaseException() as StoreFrontInactiveException;
					Exceptions.ExceptionHandler.HandleStoreFrontInactiveException(exSFI, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}
				else if (filterContext.Exception is HttpCompileException || filterContext.Exception.GetBaseException() is HttpCompileException)
				{
					HttpCompileException httpCompileEx = null;
					if (filterContext.Exception is HttpCompileException)
					{
						httpCompileEx = filterContext.Exception as HttpCompileException;
					}
					else
					{
						httpCompileEx = filterContext.Exception.GetBaseException() as HttpCompileException;
					}
					Exceptions.ExceptionHandler.HandleHttpCompileException(httpCompileEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;

				}
				else if (filterContext.Exception is HttpParseException || filterContext.Exception.GetBaseException() is HttpParseException)
				{
					HttpParseException httpParseEx = null;
					if (filterContext.Exception is HttpCompileException)
					{
						httpParseEx = filterContext.Exception as HttpParseException;
					}
					else
					{
						httpParseEx = filterContext.Exception.GetBaseException() as HttpParseException;
					}
					Exceptions.ExceptionHandler.HandleHttpParseException(httpParseEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}
				else if (filterContext.Exception is HttpException || filterContext.Exception.GetBaseException() is HttpException)
				{
					HttpException httpEx = null;
					if (filterContext.Exception is HttpException)
					{
						httpEx = filterContext.Exception as HttpException;
					}
					else
					{
						httpEx = filterContext.Exception.GetBaseException() as HttpException;
					}
					Exceptions.ExceptionHandler.HandleHttpException(httpEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}
				else if (filterContext.Exception is ApplicationException)
				{
					ApplicationException appEx = filterContext.Exception as ApplicationException;
					Exceptions.ExceptionHandler.HandleAppException(appEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}
				else if (filterContext.Exception is InvalidOperationException || filterContext.Exception.GetBaseException() is InvalidOperationException)
				{
					InvalidOperationException ioEx = null;
					if (filterContext.Exception is InvalidOperationException)
					{
						ioEx = filterContext.Exception as InvalidOperationException;
					}
					else
					{
						ioEx = filterContext.Exception.GetBaseException() as InvalidOperationException;
					}
					Exceptions.ExceptionHandler.HandleInvalidOperationException(ioEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					filterContext.ExceptionHandled = true;
					return;
				}

				//Unknown exception handler
				//exceptions that are expected or common should hit the above exception handlers.
				Exception ex = filterContext.Exception;
				Exceptions.ExceptionHandler.HandleUnknownException(ex, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
				filterContext.ExceptionHandled = true;
				return;

			}
		}

		public virtual bool TryDisplayErrorView(Exception ex, Exceptions.ErrorPage errorPage, int httpStatusCode, bool throwErrorIfAny)
		{
			try
			{
				string areaName = (RouteData.DataTokens.ContainsKey("area") ? RouteData.DataTokens["area"].ToString() : "(unknown area)");
				string controllerName = (RouteData.Values.ContainsKey("controller") ? RouteData.Values["controller"].ToString() : "(unknown controller)");
				string actionName = (RouteData.Values.ContainsKey("action") ? RouteData.Values["action"].ToString() : "(unknown action)");
				string ipAddress = this.Request.UserHostAddress;
				StoreFront currentStoreFrontOrNull = null;
				_throwErrorIfAnonymous = false;
				_throwErrorIfStoreFrontNotFound = false;
				_throwErrorIfUserProfileNotFound = false;
				_useInactiveStoreFrontAsActive = true;
				try
				{
					currentStoreFrontOrNull = CurrentStoreFrontOrNull;
				}
				catch (Exception)
				{
				}

				Response.Clear();
				Response.StatusCode = httpStatusCode;
				string errorPageFileName = errorPage.ErrorPageFileName();
				if (!Enum.IsDefined(typeof(Exceptions.ErrorPage), errorPage))
				{
					return false;
				}

				string errorViewName = Enum.GetName(typeof(Exceptions.ErrorPage), errorPage);

				GStoreErrorInfo model = new GStoreErrorInfo(errorPage, ex, RouteData, controllerName, actionName, ipAddress, currentStoreFrontOrNull, Request.RawUrl, Request.Url.ToString());
				View(errorViewName, model).ExecuteResult(this.ControllerContext);

				return true;
			}
			catch (Exception exDisplay)
			{
				string message = "Error in Controller Error View."
					+ " \n --Controller: " + this.GetType().FullName
					+ " \n --Url: " + Request.Url.ToString()
					+ " \n --RawUrl: " + Request.RawUrl
					+ " \n --ErrorPage: " + "[" + ((int)errorPage).ToString() + "] " + errorPage.ToString()
					+ " \n --Exception: " + exDisplay.ToString()
					+ " \n --Source: " + exDisplay.Source
					+ " \n --TargetSiteName: " + exDisplay.TargetSite.Name
					+ " \n --StackTrace: " + exDisplay.StackTrace
					+ " \n --Original Exception: " + ex.Message.ToString()
					+ " \n --Original Source: " + ex.Source
					+ " \n --Original TargetSiteName: " + ex.TargetSite.Name
					+ " \n --Original StackTrace: " + ex.StackTrace;

				System.Diagnostics.Trace.WriteLine("--" + message);
				string exceptionMessage = exDisplay.Message;
				string baseExceptionMessage = ex.GetBaseException().Message;
				string baseExceptionToString = ex.GetBaseException().ToString();
				GStoreDb.LogSystemEvent(HttpContext, RouteData, RouteData.ToSourceString(), SystemEventLevel.ApplicationException, message, exceptionMessage, baseExceptionMessage, baseExceptionToString, this);
				if (throwErrorIfAny)
				{
					throw new ApplicationException(message, exDisplay);
				}
				return false;
			}
		}

		/// <summary>
		/// DO NOT USE!!! INSTEAD USE HttpBadRequest. This is only here to be compatible with the default scaffolder
		/// </summary>
		protected class HttpStatusCodeResult : ActionResult
		{
			public HttpStatusCodeResult(HttpStatusCode code)
			{

				string codeText = code.ToString();
				//Deprecated way of handling badrequest or other http messages from scaffolder. Use HttpBadRequest or other method directly
				throw new HttpException((int)code, codeText);
			}

			public override void ExecuteResult(ControllerContext context)
			{
				throw new NotImplementedException();
			}
		}


		/// <summary>
		/// Adds a message to the user's user message alerts (top)
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="userMessageType"></param>
		public virtual void AddUserMessage(string title, string message, UserMessageType userMessageType)
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
		public virtual void AddUserMessageBottom(string title, string message, UserMessageType userMessageType)
		{
			if (!this.TempData.ContainsKey("UserMessagesBottom"))
			{
				this.TempData.Add("UserMessagesBottom", new List<UserMessage>());
			}

			List<UserMessage> userMessagesBottom = (List<UserMessage>)this.TempData["UserMessagesBottom"];
			userMessagesBottom.Add(new UserMessage(title, message, userMessageType));
		}

		public virtual void AddGaEvent(string category, string action, string label)
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