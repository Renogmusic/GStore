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
using GStore.Identity;

namespace GStore.Controllers.BaseClass
{
	public abstract class BaseController : Controller
	{
		protected abstract string ThemeFolderName { get; }

		protected IGstoreDb _dbContext = null;
		protected bool _throwErrorIfStoreFrontNotFound = true;
		protected bool _throwErrorIfUserProfileNotFound = false;
		protected bool _throwErrorIfAnonymous = false;
		protected bool _useInactiveStoreFrontAsActive = false;
		protected bool _useInactiveStoreFrontConfigAsActive = false;

		protected bool _currentStoreFrontError = false;

		public void SetErrorContext()
		{
			_throwErrorIfStoreFrontNotFound = false;
		}

		protected override void Initialize(System.Web.Routing.RequestContext requestContext)
		{
			if (requestContext.HttpContext.Session["entryDateTimeUtc"] == null)
			{
				requestContext.HttpContext.Session["entryRawUrl"] = requestContext.HttpContext.Request.RawUrl;
				requestContext.HttpContext.Session["entryUrl"] = requestContext.HttpContext.Request.Url.ToString();
				if (requestContext.HttpContext.Request.UrlReferrer != null)
				{
					requestContext.HttpContext.Session["entryReferrer"] = requestContext.HttpContext.Request.UrlReferrer.ToString();
				}
				requestContext.HttpContext.Session["entryDateTimeUtc"] = DateTime.UtcNow;
			}

			if (!string.IsNullOrEmpty(requestContext.HttpContext.Request.QueryString["DiscountCode"]))
			{
				requestContext.HttpContext.Session["DiscountCode"] = requestContext.HttpContext.Request.QueryString["DiscountCode"];
			}
			base.Initialize(requestContext);
		}

		public virtual string ThemeFolderNameToUse
		{
			get
			{
				try
				{
					try
					{
						return ThemeFolderName;
					}
					catch (Exceptions.DynamicPageInactiveException dpiEx)
					{
						//dynamic page is inactive, return storefront default theme
						return dpiEx.StoreFront.CurrentConfigOrAny().DefaultNewPageTheme.FolderName;
					}
					catch (Exceptions.DynamicPageNotFoundException dpnfEx)
					{
						//dynamic page not found, return storefront default theme
						return dpnfEx.StoreFront.CurrentConfigOrAny().DefaultNewPageTheme.FolderName;
					}
					catch (Exceptions.NoMatchingBindingException)
					{
						//no storefront found, return app default theme
						return Settings.AppDefaultThemeFolderName;
					}
					catch (Exceptions.StoreFrontInactiveException)
					{
						//storefront is inactive, return app default theme
						return Settings.AppDefaultThemeFolderName;
					}
					catch (Exception)
					{
						//unknown exception, use storefront default theme
						if (_currentStoreFrontError || CurrentStoreFrontOrNull == null)
						{
							//couldn't get storefront, use app default layout
							return Settings.AppDefaultThemeFolderName;
						}
						return CurrentStoreFrontOrThrow.CurrentConfigOrAny().DefaultNewPageTheme.FolderName;
					}
				}
				catch (Exception)
				{
					return CurrentStoreFrontOrThrow.CurrentConfigOrAny().DefaultNewPageTheme.FolderName;
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

					return GStoreDb.GetCurrentStoreFront(Request, _throwErrorIfStoreFrontNotFound, _useInactiveStoreFrontAsActive, _useInactiveStoreFrontConfigAsActive);
				}
				catch (Exceptions.StoreFrontInactiveException exSFI)
				{
					if (_useInactiveStoreFrontAsActive)
					{
						GStoreDb.CachedStoreFront = exSFI.StoreFront;
						GStoreDb.CachedStoreFrontConfig = exSFI.StoreFront.CurrentConfigOrAny();
						return exSFI.StoreFront;
					}
					_currentStoreFrontError = true;
					throw;
				}
				catch (Exceptions.NoMatchingBindingException exNMB)
				{
					if (GStoreDb.StoreFronts.IsEmpty())
					{
						AspNetIdentityContext identityCtx = GStore.Identity.AspNetIdentityContext.Create();
						identityCtx.Initialize(true);
						GStoreDb.Initialize(true);
						GStoreDb.SeedDatabase();
					}
					if (GStoreDb.StoreFronts.IsEmpty())
					{
						throw new NoMatchingBindingException("No Store Fronts in database. Be sure database is seeded. You can log into system admin section and create a storefront or run the seed database command.\n" + exNMB.Message, exNMB.Uri);
					}
					if (!Settings.AppEnableBindingAutoMapToFirstStoreFront)
					{
						_currentStoreFrontError = true;
						throw new NoMatchingBindingException("No Store Front found matching current site, and auto-map is disabled. Either this site is invalid or wrong bindings exist in database. Turn on Auto-Binding-Map by setting Settings.AppEnableBindingAutoMapToFirstStoreFront to true \n" + exNMB.Message, exNMB.Uri);
					}
					try
					{
						StoreBinding binding = GStoreDb.AutoMapBinding(this);
					}
					catch (Exception)
					{
						_currentStoreFrontError = true;
						throw new NoMatchingBindingException("Auto-Map failed. You will have to log into the system admin section and set a binding manually for this url.\n" + exNMB.Message, exNMB.Uri);
					}
					try
					{
						return GStoreDb.GetCurrentStoreFront(Request, _throwErrorIfStoreFrontNotFound, _useInactiveStoreFrontAsActive, _useInactiveStoreFrontConfigAsActive);
					}
					catch (Exception)
					{
						_currentStoreFrontError = true;
						throw;
					} 
				}
				catch (Exceptions.DatabaseErrorException dbEx)
				{
					if (!_databaseErrorRetry)
					{
						AspNetIdentityContext identityCtx = GStore.Identity.AspNetIdentityContext.Create();
						identityCtx.Initialize(true);
						GStoreDb.Initialize(true);
						GStoreDb.SeedDatabase();
						_databaseErrorRetry = true;
						return CurrentStoreFrontOrThrow;
					}

					System.Diagnostics.Trace.WriteLine("--Database Error in CurrentStoreFront: " + dbEx.Message);
					System.Diagnostics.Trace.Indent();
					System.Diagnostics.Trace.WriteLine("-- inner exception: " + dbEx.InnerException.ToString());
					System.Diagnostics.Trace.Unindent();
					_currentStoreFrontError = true;
					throw;
				}
				catch (Exception ex)
				{
					_currentStoreFrontError = true; 
					throw new ApplicationException("Cannot find active current store front", ex);
				}
			}
		}
		private bool _databaseErrorRetry = false;

		public virtual StoreFrontConfiguration CurrentStoreFrontConfigOrNull
		{
			get
			{
				if (_currentStoreFrontError)
				{
					return null;
				}
				try
				{
					if (_useInactiveStoreFrontConfigAsActive)
					{
						return CurrentStoreFrontConfigOrAny;
					}
					else
					{
						return CurrentStoreFrontConfigOrThrow;
					}
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public virtual StoreFrontConfiguration CurrentStoreFrontConfigOrAny
		{
			get
			{
				if (_currentStoreFrontError)
				{
					return null;
				}
				try
				{
					StoreFront storeFront = CurrentStoreFrontOrNull;
					if (storeFront == null)
					{
						return null;
					}

					return storeFront.CurrentConfigOrAny();
				}
				catch (Exception)
				{
					return null;
				}
			}
		}

		public virtual StoreFrontConfiguration CurrentStoreFrontConfigOrThrow
		{
			get
			{
				if (_currentstoreFrontConfiguration != null)
				{
					return _currentstoreFrontConfiguration;
				}
				StoreFront storeFront = CurrentStoreFrontOrThrow;
				_currentstoreFrontConfiguration = storeFront.CurrentConfig();
				if (_currentstoreFrontConfiguration == null && _useInactiveStoreFrontConfigAsActive)
				{
					_currentstoreFrontConfiguration = CurrentStoreFrontConfigOrAny;
				}
				if (_currentstoreFrontConfiguration == null)
				{
					throw new ApplicationException("Active StoreFront Configuration not found for store front id [" + storeFront.StoreFrontId + "] Client '" + storeFront.Client.Name + "' [" + storeFront.ClientId + "]");
				}
				return _currentstoreFrontConfiguration;
			}
		}
		protected StoreFrontConfiguration _currentstoreFrontConfiguration = null;

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

		/// <summary>
		/// Throws a HTTP 400 BadRequest exception
		/// </summary>
		/// <param name="statusDescription"></param>
		/// <returns></returns>
		protected virtual PartialViewResult HttpBadRequestPartial(string statusDescription = null)
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
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleDynamicPageInactiveException(exDPI, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					return;
				}
				else if (filterContext.Exception.GetBaseException() is DynamicPageNotFoundException)
				{
					DynamicPageNotFoundException exDPNF = filterContext.Exception.GetBaseException() as DynamicPageNotFoundException;
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleDynamicPageNotFoundException(exDPNF, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					return;
				}
				else if (filterContext.Exception.GetBaseException() is NoMatchingBindingException)
				{
					NoMatchingBindingException exNMB = filterContext.Exception.GetBaseException() as NoMatchingBindingException;
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleNoMatchingBindingException(exNMB, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					return;
				}
				else if (filterContext.Exception.GetBaseException() is StoreFrontInactiveException)
				{
					StoreFrontInactiveException exSFI = filterContext.Exception.GetBaseException() as StoreFrontInactiveException;
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleStoreFrontInactiveException(exSFI, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
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
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleHttpCompileException(httpCompileEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
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
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleHttpParseException(httpParseEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
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
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleHttpException(httpEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					return;
				}
				else if (filterContext.Exception is ApplicationException)
				{
					ApplicationException appEx = filterContext.Exception as ApplicationException;
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleAppException(appEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
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
					filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleInvalidOperationException(ioEx, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
					return;
				}

				//Unknown exception handler
				//exceptions that are expected or common should hit the above exception handlers.
				Exception ex = filterContext.Exception;
				filterContext.ExceptionHandled = Exceptions.ExceptionHandler.HandleUnknownException(ex, false, filterContext.HttpContext.ApplicationInstance.Context, RouteData, this);
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
				StoreFrontConfiguration currentStoreFrontConfigOrNull = null;
				_throwErrorIfAnonymous = false;
				_throwErrorIfStoreFrontNotFound = false;
				_throwErrorIfUserProfileNotFound = false;
				_useInactiveStoreFrontAsActive = true;
				_useInactiveStoreFrontConfigAsActive = true;
				try
				{
					currentStoreFrontOrNull = CurrentStoreFrontOrNull;
					currentStoreFrontConfigOrNull = CurrentStoreFrontConfigOrNull;
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

				if (string.IsNullOrEmpty(Request["NotFound"]) && errorPage == ErrorPage.Error_NotFound && CurrentStoreFrontIdOrNull != null && currentStoreFrontConfigOrNull.NotFoundErrorPage != null && currentStoreFrontConfigOrNull.NotFoundErrorPage.IsActiveBubble())
				{
					string urlRedirect = currentStoreFrontConfigOrNull.NotFoundErrorPage.UrlResolved(Url) + "?NotFound=1";
					AddUserMessage("Page Not Found", "Sorry, the URL you were looking for was not found. '" + Request.Url.ToString().ToHtml() + "'", UserMessageType.Danger);
					new RedirectResult(urlRedirect, false).ExecuteResult(this.ControllerContext);
					return true;
				}

				if (string.IsNullOrEmpty(Request["StoreError"]) && (errorPage == ErrorPage.Error_AppError || errorPage == ErrorPage.Error_BadRequest || errorPage == ErrorPage.Error_HttpError || errorPage == ErrorPage.Error_InvalidOperation || errorPage == ErrorPage.Error_UnknownError) && CurrentStoreFrontIdOrNull != null && currentStoreFrontConfigOrNull.StoreErrorPage != null && currentStoreFrontConfigOrNull.StoreErrorPage.IsActiveBubble())
				{
					string urlRedirect = currentStoreFrontConfigOrNull.StoreErrorPage.UrlResolved(Url) + "?StoreError=1";
					AddUserMessage("Server Error", "Sorry, there was a server error processing your request for URL '" + Request.Url.ToString().ToHtml() + "'", UserMessageType.Danger);
					new RedirectResult(urlRedirect, false).ExecuteResult(this.ControllerContext);
					return true;
				}

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