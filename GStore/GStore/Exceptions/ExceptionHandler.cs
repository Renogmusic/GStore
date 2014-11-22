using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using GStore.Models.Extensions;
using System.ComponentModel;

namespace GStore.Exceptions
{
	public enum ErrorPage
	{
		[Description("Error_BadRequest.html")]
		Error_BadRequest = 400,

		[Description("Error_GStoreNotFound.html")]
		Error_GStoreNotFound = 401,

		[Description("Error_GStoreInactive.html")]
		Error_GStoreInactive = 402,

		[Description("Error_NotFound.html")]
		Error_NotFound = 404,

		[Description("Error_HttpError.html")]
		Error_HttpError = 500,

		[Description("Error_InvalidOperation.html")]
		Error_InvalidOperation = 510,

		[Description("Error_AppError.html")]
		Error_AppError = 550,

		[Description("Error_UnknownError.html")]
		Error_UnknownError = 599
	}

	public class GStoreErrorInfo : System.Web.Mvc.HandleErrorInfo
	{

		public GStoreErrorInfo(ErrorPage errorPage, Exception exception, RouteData routeData, string controllerName, string actionName, string ipAddress, Models.StoreFront currentstoreFrontOrNull, string rawUrl, string url)
			: base(exception, controllerName, actionName)
		{
			this.ErrorPage = errorPage;
			this.ErrorCode = (int)errorPage;
			this.RouteDataSource = routeData.ToSourceString();
			this.IPAddress = ipAddress;
			this.CurrentstoreFrontOrNull = currentstoreFrontOrNull;
			this.RawUrl = rawUrl;
			this.Url = url;
		}

		public ErrorPage ErrorPage { get; protected set; }

		public int ErrorCode { get; protected set; }

		/// <summary>
		/// Gets the IP address of the client that threw the exception. String.Empty for unknown IP address
		/// </summary>
		public string IPAddress { get; protected set; }

		/// <summary>
		/// Source of exception using RouteData.ToSourceString()
		/// </summary>
		public string RouteDataSource { get; protected set; }

		/// <summary>
		/// URL of the page (also helpful is RawUrl)
		/// </summary>
		public string Url { get; protected set; }

		/// <summary>
		/// Raw URL of the page (also helpful is Url)
		/// </summary>
		public string RawUrl { get; protected set; }


		/// <summary>
		/// Gets the StoreFront that threw the exception. null if unknown storefront
		/// </summary>
		public Models.StoreFront CurrentstoreFrontOrNull { get; protected set; }
	}

	public static class ExceptionHandler
	{

		public static string ErrorPageFileName(this ErrorPage errorpage)
		{
			return GetEnumDescription(errorpage);
		}

		public static void HandleHttpException(HttpException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogHttpException(ex, context, routeData, controller);
			}

			int httpCode = ex.GetHttpCode();
			if (httpCode == 404)
			{
				if (Properties.Settings.Current.AppUseFriendlyErrorPages)
				{
					ShowErrorPage(ErrorPage.Error_NotFound, ex.GetHttpCode(), ex, clearError, context, controller);
				}
			}
			else if (httpCode == 400)
			{
				if (Properties.Settings.Current.AppUseFriendlyErrorPages)
				{
					ShowErrorPage(ErrorPage.Error_BadRequest, ex.GetHttpCode(), ex, clearError, context, controller);
				}
			}
			else
			{
				//unhandled HTTP code
				if (Properties.Settings.Current.AppUseFriendlyErrorPages)
				{
					ShowErrorPage(ErrorPage.Error_HttpError, ex.GetHttpCode(), ex, clearError, context, controller);
				}
			}
		}

		public static void HandleHttpCompileException(HttpCompileException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogHttpCompileException(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_HttpError, ex.GetHttpCode(), ex, clearError, context, controller);
			}
		}

		public static void HandleHttpParseException(HttpParseException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogHttpParseException(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_HttpError, ex.GetHttpCode(), ex, clearError, context, controller);
			}
		}

		public static void HandleAppException(ApplicationException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogAppException(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_AppError, (int)HttpStatusCode.InternalServerError, ex, clearError, context, controller);
			}
		}

		public static void HandleInvalidOperationException(InvalidOperationException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogInvalidOperationException(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_InvalidOperation, (int)HttpStatusCode.InternalServerError, ex, clearError, context, controller);
			}
		}

		public static void HandleDynamicPageInactiveException(DynamicPageInactiveException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogFileNotFound(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_NotFound, (int)HttpStatusCode.NotFound, ex, clearError, context, controller);
			}
		}

		public static void HandleDynamicPageNotFoundException(DynamicPageNotFoundException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogFileNotFound(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_NotFound, (int)HttpStatusCode.NotFound, ex, clearError, context, controller);
			}
		}

		public static void HandleNoMatchingBindingException(NoMatchingBindingException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			if (Properties.Settings.Current.AppEnableBindingAutoMapToFirstStoreFront && !Data.RepositoryFactory.SystemWideRepository(controller.User).StoreFronts.IsEmpty())
			{
				throw new ApplicationException("BaseController did not auto-map properly. Check auto-map routing in GStoreDb property of base controller.", ex);
			}

			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogFileNotFound(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_GStoreNotFound, (int)HttpStatusCode.OK, ex, clearError, context, controller);
			}
		}

		public static void HandleStoreFrontInactiveException(StoreFrontInactiveException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogFileNotFound(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_GStoreInactive, (int)HttpStatusCode.OK, ex, clearError, context, controller);
			}
		}

		public static void HandleUnknownException(Exception ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string rawUrl = context.Request.RawUrl;
			string url = context.Request.Url.ToString();
			if (Properties.Settings.Current.AppLogSystemEventsToDb || Properties.Settings.Current.AppLogSystemEventsToFile)
			{
				LogUnknownException(ex, context, routeData, controller);
			}
			if (Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				ShowErrorPage(ErrorPage.Error_UnknownError, (int)HttpStatusCode.InternalServerError, ex, clearError, context, controller);
			}
		}

		private static void LogHttpException(HttpException ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			int httpCode = ex.GetHttpCode();
			string message = string.Empty;
			if (httpCode == 404)
			{
				LogFileNotFound(ex, context, routeData, controller);
				return;
			}
			if ((httpCode == 400) && (routeData != null))
			{
				//if 400 came from a controller, handle it in the bad request log, not system (error) log
				LogBadRequest(ex, context, routeData, controller);
				return;
			}

			message = "Http Exception " + httpCode + " " + ex.GetHtmlErrorMessage()
				+ "  \n-Message: " + ex.Message
				+ "  \n-Source: " + ex.Source
				+ "  \n-WebEventCode: " + ex.WebEventCode
				+ "  \n-ErrorCode: " + ex.ErrorCode
				+ "  \n-TargetSiteName: " + ex.TargetSite.Name
				+ "  \n-StackTrace: " + ex.StackTrace;

			LogException(message, ex, Models.SystemEventLevel.Error, context, routeData, controller);
		}

		private static void LogHttpCompileException(HttpCompileException ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			int httpCode = ex.GetHttpCode();
			string message = string.Empty;

			message = "Http Compile Exception " + httpCode + " " + ex.GetHtmlErrorMessage()
				+ "  \n-Message: " + ex.Message
				+ "  \n-Source: " + ex.Source
				+ "  \n-SourceCode: " + ex.SourceCode
				+ "  \n-FileName: " + ex.Results.Errors.ToString()
				+ "  \n-Compile Error Count: " + ex.Results.Errors.Count
				+ "  \n-Compile Errors: " + ex.Results.Errors.ToString()
				+ "  \n-WebEventCode: " + ex.WebEventCode
				+ "  \n-ErrorCode: " + ex.ErrorCode
				+ "  \n-TargetSiteName: " + ex.TargetSite.Name
				+ "  \n-StackTrace: " + ex.StackTrace;

			LogException(message, ex, Models.SystemEventLevel.Error, context, routeData, controller);
		}

		private static void LogHttpParseException(HttpParseException ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			int httpCode = ex.GetHttpCode();
			string message = string.Empty;

			message = "Http Parse Exception " + httpCode
				+ "  \n-Message: " + ex.Message
				+ "  \n-Source: " + ex.Source
				+ "  \n-VirtualPath: " + ex.VirtualPath
				+ "  \n-FileName: " + ex.FileName + " (" + ex.Line + ")"
				+ "  \n-Parser Error Count: " + ex.ParserErrors.Count;

			int counter = 0;
			foreach (ParserError item in ex.ParserErrors)
			{
				counter++;
				string errorText = item.VirtualPath + " (" + item.Line + ") " + item.ErrorText;
				message += "  \n-Parser Error [" + counter + "]: " + errorText;
			}

			message += "  \n-Parser Errors: " + ex.ParserErrors.ToString()
				+ "  \n-WebEventCode: " + ex.WebEventCode
				+ "  \n-ErrorCode: " + ex.ErrorCode
				+ "  \n-TargetSiteName: " + ex.TargetSite.Name
				+ "  \n-StackTrace: " + ex.StackTrace
				+ " Html Message: " + ex.GetHtmlErrorMessage();

			LogException(message, ex, Models.SystemEventLevel.Error, context, routeData, controller);
		}

		private static void LogAppException(ApplicationException ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string message = "App Exception " + ex.Message
				+ " \n-Message: " + ex.Message
				+ " \n-Source: " + ex.Source
				+ " \n-TargetSiteName: " + ex.TargetSite.Name
				+ " \n-StackTrace: " + ex.StackTrace;

			LogException(message, ex, Models.SystemEventLevel.ApplicationException, context, routeData, controller);
		}

		private static void LogInvalidOperationException(InvalidOperationException ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string message = "Invalid Operation Exception " + ex.Message
				+ " \n-Message: " + ex.Message
				+ " \n-Source: " + ex.Source
				+ " \n-TargetSiteName: " + ex.TargetSite.Name
				+ " \n-StackTrace: " + ex.StackTrace;

			LogException(message, ex, Models.SystemEventLevel.InvalidOperationException, context, routeData, controller);
		}

		private static void LogUnknownException(Exception ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string message = "Unknown Exception " + ex.Message
				+ " \n-Message: " + ex.Message
				+ " \n-Source: " + ex.Source
				+ " \n-TargetSiteName: " + ex.TargetSite.Name
				+ " \n-StackTrace: " + ex.StackTrace;

			LogException(message, ex, Models.SystemEventLevel.Error, context, routeData, controller);
		}

		private static void LogException(string message, Exception ex, Models.SystemEventLevel systemEventLevel, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string source = ex.Source;
			Data.IGstoreDb db = Data.RepositoryFactory.SystemWideRepository(context.User);
			string exceptionMessage = ex.Message;
			string baseExceptionMessage = ex.GetBaseException().Message;
			string baseExceptionToString = ex.GetBaseException().ToString();
			db.LogSystemEvent(context.Request.RequestContext.HttpContext, routeData, source, systemEventLevel, message, exceptionMessage, baseExceptionMessage, baseExceptionToString, controller);
		}

		private static void LogFileNotFound(Exception ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string source = ex.Source;
			Data.IGstoreDb db = Data.RepositoryFactory.SystemWideRepository(context.User);
			db.LogFileNotFound(context.Request.RequestContext.HttpContext, routeData, controller);
		}

		private static void LogBadRequest(HttpException ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			string source = ex.Source;
			Data.IGstoreDb db = Data.RepositoryFactory.SystemWideRepository(context.User);
			db.LogBadRequest(context.Request.RequestContext.HttpContext, routeData, controller);
		}

		/// <summary>
		/// Attempts to display error page.  1st try is the current controller's TryDisplayErrorView method, 2nd try is GStore controller Error action, 3rd plan is display a static file from ErrorPages
		/// </summary>
		/// <param name="pagePath"></param>
		private static void ShowErrorPage(ErrorPage errorPage, int httpStatusCode, Exception ex, bool clearError, HttpContext context, Controllers.BaseClass.BaseController controller)
		{
			if (!Properties.Settings.Current.AppUseFriendlyErrorPages)
			{
				return;
			}

			if (controller != null)
			{
				//execute error view on controller
				if (TryDisplayControllerErrorView(errorPage, httpStatusCode, ex, clearError, context, controller))
				{
					return;
				}
			}

			if (TryExecuteAppErrorController(errorPage, httpStatusCode, ex, clearError, context, controller))
			{
				return;
			}
			if (TryDisplayStaticPage(errorPage, httpStatusCode, ex, clearError, context, controller))
			{
				return;
			}
		}

		public static bool TryDisplayControllerErrorView(ErrorPage errorPage, int httpStatusCode, Exception ex, bool clearError, HttpContext context, Controllers.BaseClass.BaseController controller)
		{
			string url = string.Empty;
			string rawUrl = string.Empty;
			if (context != null)
			{
				url = context.Request.Url.ToString();
				rawUrl = context.Request.RawUrl;
			}

			try
			{
				if (controller == null)
				{
					return false;
				}

				System.Diagnostics.Trace.WriteLine("--Executing controller.TryDisplayErrorView for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Original Exception: " + ex.Message);
				bool resultSuccess = controller.TryDisplayErrorView(ex, errorPage, httpStatusCode, false);
				System.Diagnostics.Trace.WriteLine("--" + (resultSuccess ? "Success" : "Failure") + " executing controller.TryDisplayErrorView. Result: " + resultSuccess.ToString() + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Original Exception: " + ex.Message);
				if (resultSuccess)
				{
					if (clearError)
					{
						context.ClearError();
					}
					return true;
				}
				return false;
			}
			catch (Exception exController)
			{
				System.Diagnostics.Trace.WriteLine("--Error executing controller.TryDisplayErrorView for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Exception: " + exController.Message + " Original Exception: " + ex.Message);
				return false;
			}
		}

		public static bool TryExecuteAppErrorController(ErrorPage errorPage, int httpStatusCode, Exception ex, bool clearError, HttpContext context, Controllers.BaseClass.BaseController controller)
		{
			string url = string.Empty;
			string rawUrl = string.Empty;
			if (context != null)
			{
				url = context.Request.Url.ToString();
				rawUrl = context.Request.RawUrl;
			}

			var routeData = new RouteData();
			
			//if we can find the current area from the url, we can use a area-specific error handler to incorporate area layout/design
			string area = DetermineAreaFromRequest(context.Request);
			if (!string.IsNullOrEmpty(area))
			{
				routeData.DataTokens.Add("area", area);
			}
			routeData.Values["controller"] = "GStore";
			routeData.Values["action"] = "AppError";
			routeData.Values["exception"] = ex;
			routeData.Values["errorpage"] = errorPage;
			routeData.Values["httpStatusCode"] = httpStatusCode;

			System.Web.Mvc.IController errorController = null;
			if (area.ToLower() == "storeadmin")
			{
				errorController = new GStore.Areas.StoreAdmin.Controllers.GStoreController();
			}
			else if (area.ToLower() == "systemadmin")
			{
				errorController = new GStore.Areas.SystemAdmin.Controllers.GStoreController();
			}
			else
			{
				errorController = new GStore.Controllers.GStoreController();
			}
			
			var rc = new RequestContext(new HttpContextWrapper(context), routeData);
			try
			{
				System.Diagnostics.Trace.WriteLine("--Executing error controller for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Original Exception: " + ex.Message);
				errorController.Execute(rc);
				System.Diagnostics.Trace.WriteLine("--Success executing error controller for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Original Exception: " + ex.Message);
				if (clearError)
				{
					context.ClearError();
				}
				return true;
			}
			catch (Exception exErrorController)
			{
				System.Diagnostics.Trace.WriteLine("--Failure. Error executing error controller for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Exception: " + exErrorController.Message + " Original Exception: " + ex.Message);
				return false;
			}
		}

		public static bool TryDisplayStaticPage(ErrorPage errorPage, int httpStatusCode, Exception ex, bool clearError, HttpContext context, Controllers.BaseClass.BaseController controller)
		{
			string url = string.Empty;
			string rawUrl = string.Empty;
			if (context != null)
			{
				url = context.Request.Url.ToString();
				rawUrl = context.Request.RawUrl;
			}

			if (controller != null && controller.CurrentStoreFrontOrNull != null)
			{
				//start with storefront error pages folder
				Models.StoreFront storeFront = controller.CurrentStoreFrontOrThrow;

				string customErrorFolder = storeFront.StoreFrontVirtualDirectoryToMap(controller.Request.ApplicationPath) + "/ErrorPages/";
				string customErrorPath = customErrorFolder + errorPage.ErrorPageFileName();
				bool customErrorFileExists = false;
				if (System.IO.File.Exists(context.Server.MapPath(customErrorPath)))
				{
					customErrorFileExists = true;
				}
				else
				{
					//if store front error page not found, look for client custom error page
					customErrorFolder = storeFront.Client.ClientVirtualDirectoryToMap() + "/ErrorPages/";
					customErrorPath = customErrorFolder + errorPage.ErrorPageFileName();
					if (System.IO.File.Exists(context.Server.MapPath(customErrorPath)))
					{
						customErrorFileExists = true;
					}
				}

				if (customErrorFileExists)
				{
					try
					{
						context.Response.Clear();
						context.Response.StatusCode = httpStatusCode;
						System.Diagnostics.Trace.WriteLine("--Displaying custom error static page: " + customErrorPath + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Original Exception: " + ex.Message);
						context.Server.Execute(customErrorPath);
						context.Response.Flush();
						System.Diagnostics.Trace.WriteLine("--Success displaying custom error static page: " + customErrorPath + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Original Exception: " + ex.Message);
						if (clearError)
						{
							context.ClearError();
						}
						return true;

					}
					catch (Exception exCustomPageError)
					{
						System.Diagnostics.Trace.WriteLine("--Failure. Error displaying custom error static page: " + customErrorPath + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Exception: " + exCustomPageError.Message + " Original Exception: " + ex.Message);
						string error = exCustomPageError.Message;
					}
				}
			}

			//unknown client or client does not have a custom error page, show server static error page
			string serverErrorFolder = "~/Content/Server/ErrorPages/";
			string serverErrorPath = serverErrorFolder + errorPage.ErrorPageFileName();
			if (!System.IO.File.Exists(context.Server.MapPath(serverErrorPath)))
			{
				System.Diagnostics.Trace.WriteLine("--Failure. Error: Server error static page not found: " + serverErrorPath + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Original Exception: " + ex.Message);
				return false;
			}

			try
			{
				context.Response.Clear();
				context.Response.StatusCode = httpStatusCode;
				System.Diagnostics.Trace.WriteLine("--Displaying server error static page: " + serverErrorPath + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl);
				context.Server.Execute(serverErrorPath);
				context.Response.Flush();
				System.Diagnostics.Trace.WriteLine("--Success displaying server error static page: " + serverErrorPath + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl);
				if (clearError)
				{
					context.Server.ClearError();
				}
				return true;
			}
			catch (Exception exServerPageError)
			{
				System.Diagnostics.Trace.WriteLine("--Failure. Error displaying server error static page: " + serverErrorPath + " for ErrorPage: " + errorPage.ToString() + " Url: " + url + " RawUrl: " + rawUrl + " Exception: " + exServerPageError.Message + " Original Exception: " + ex.Message);
				return false;
			}
		}


		private static string GetEnumDescription(Enum value)
		{
			System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes != null && attributes.Length > 0)
			{
				return attributes[0].Description;
			}
			else
			{
				return value.ToString();
			}
		}

		private static string DetermineAreaFromRequest(HttpRequest request)
		{
			string url = request.Url.AbsolutePath.Trim('/').ToLower();
			string relativeUrl = url;
			string appPath = request.ApplicationPath.Trim('/');
			if (!string.IsNullOrEmpty(appPath))
			{
				relativeUrl = url.Remove(0, appPath.Length);
			}

			if (relativeUrl.StartsWith("storeadmin"))
			{
				return "StoreAdmin";
			}
			else if (relativeUrl.StartsWith("systemadmin"))
			{
				return "SystemAdmin";
			}

			return string.Empty;

		}


	}
}