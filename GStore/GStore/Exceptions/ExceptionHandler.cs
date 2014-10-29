using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using GStore.Models.Extensions;

namespace GStore
{
	public static class ExceptionHandler
	{
		public static void HandleHttpException(HttpException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			if (!Properties.Settings.Default.AppFriendlyExceptionHandling)
			{
				return;
			}

			string url = context.Request.RawUrl;

			LogHttpException(ex, context, routeData, controller);

			int httpCode = ex.GetHttpCode();
			if (controller != null)
			if (httpCode == 404)
			{
				ShowErrorPage("NotFound.html", ex.GetHttpCode(), clearError, context, controller);
			}
			else if (httpCode == 400)
			{
				ShowErrorPage("BadRequest.html", ex.GetHttpCode(), clearError, context, controller);
			}
			else
			{
				//unhandled HTTP code, allow IIS to handle the error
				ShowErrorPage("HttpError.html", ex.GetHttpCode(), clearError, context, controller);
			}
		}

		public static void HandleHttpCompileException(HttpCompileException ex, bool clearError, bool showFriendlyError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			if (!Properties.Settings.Default.AppFriendlyExceptionHandling)
			{
				return;
			}

			string url = context.Request.RawUrl;

			LogHttpCompileException(ex, context, routeData, controller);
			if (showFriendlyError)
			{
				ShowErrorPage("HttpError.html", ex.GetHttpCode(), clearError, context, controller);
			}
		}

		public static void HandleHttpParseException(HttpParseException ex, bool clearError, bool showFriendlyError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			if (!Properties.Settings.Default.AppFriendlyExceptionHandling)
			{
				return;
			}

			string url = context.Request.RawUrl;

			LogHttpParseException(ex, context, routeData, controller);
			if (showFriendlyError)
			{
				ShowErrorPage("HttpError.html", ex.GetHttpCode(), clearError, context, controller);
			}
		}

		public static void HandleAppException(ApplicationException ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			if (!Properties.Settings.Default.AppFriendlyExceptionHandling)
			{
				return;
			}
			LogAppException(ex, context, routeData, controller);
			ShowErrorPage("AppError.html", (int)HttpStatusCode.InternalServerError, clearError, context, controller);
			//controllers have their own exception handlers, but this will handle webapi and WebForms or anything else in the app
		}

		public static void HandleUnknownException(Exception ex, bool clearError, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
		{
			if (!Properties.Settings.Default.AppFriendlyExceptionHandling)
			{
				return;
			}
			LogUnknownException(ex, context, routeData, controller);
			ShowErrorPage("UnknownError.html", (int)HttpStatusCode.InternalServerError, clearError, context, controller);
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
			db.LogSystemEvent(context.Request.RequestContext.HttpContext, routeData, source, systemEventLevel, message, controller);
		}

		private static void LogFileNotFound(HttpException ex, HttpContext context, RouteData routeData, Controllers.BaseClass.BaseController controller)
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
		/// Attempts a server execute of a specific page. if client page is not found, tries server page. If successful calls Server.ClearError when clearError = true, if unsuccessful, just returns for IIS to handle
		/// </summary>
		/// <param name="pagePath"></param>
		private static void ShowErrorPage(string errorPage, int code, bool clearError, HttpContext context, Controllers.BaseClass.BaseController controller)
		{
			try
			{
				context.Response.Clear();
				context.Response.StatusCode = code;
				try
				{
					if (controller == null)
					{
						throw new ApplicationException("No controller to get context from");
					}
					if (controller.CurrentStoreFrontIdOrNull == null)
					{
						throw new ApplicationException("No store front from controller");
					}
					string clientErrorFolder = "~/Content/Clients/" + controller.CurrentClient.Folder + "/" + controller.CurrentStoreFront.Folder + "/ErrorPages/";
					string clientErrorPage = clientErrorFolder + errorPage;
					if (!System.IO.File.Exists(context.Server.MapPath(clientErrorPage)))
					{
						throw new ApplicationException("File not found");
					}
					context.Server.Execute(clientErrorPage);
				}
				catch (Exception)
				{
					context.Response.Clear();
					context.Response.StatusCode = code;
					context.Server.Execute("~/Content/Server/ErrorPages/" + errorPage);
				}
				if (clearError)
				{
					context.Server.ClearError();
				}
			}
			catch (Exception)
			{
				//if execute fails on 404 or other error, return without clearing error to allow IIS to handle the error
				return;
			}

		}
	}
}