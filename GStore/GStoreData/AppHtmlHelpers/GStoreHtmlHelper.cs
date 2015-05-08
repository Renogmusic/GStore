using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using GStoreData.ControllerBase;
using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.ViewModels;

namespace GStoreData.AppHtmlHelpers
{
	public static class GStoreHtmlHelper
	{
		public static string GStoreVersionNumber(this HtmlHelper htmlHelper)
		{
			System.Reflection.Assembly assembly = typeof(Models.StoreFront).Assembly;
			return assembly.GetName().Version.ToString();
		}

		public static string GStoreVersionDateString(this HtmlHelper htmlHelper, bool useFileDate = false)
		{
			System.Reflection.Assembly assembly = typeof(Models.StoreFront).Assembly;
			Version version = assembly.GetName().Version;
			DateTime buildDate = new DateTime(2000, 1, 1, 0, 0, 0)
				.AddDays(version.Build)
				.AddSeconds(version.Revision * 2);

			if (TimeZone.IsDaylightSavingTime(buildDate, TimeZone.CurrentTimeZone.GetDaylightChanges(buildDate.Year)))
			{
				buildDate = buildDate.AddHours(1);
			}

			return buildDate.ToString();

		}

		public static StoreFront CurrentStoreFront(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			BaseController controller = htmlHelper.BaseControllerOrThrow();
			if (throwErrorIfNotFound)
			{
				return controller.CurrentStoreFrontOrThrow;
			}
			return controller.CurrentStoreFrontOrNull;
		}

		public static Client CurrentClient(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			BaseController controller = htmlHelper.BaseControllerOrThrow();

			if (throwErrorIfNotFound)
			{
				return controller.CurrentClientOrThrow;
			}
			return controller.CurrentClientOrNull;
		}

		public static StoreFrontConfiguration CurrentStoreFrontConfig(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			BaseController controller = htmlHelper.BaseControllerOrThrow();

			if (throwErrorIfNotFound)
			{
				return controller.CurrentStoreFrontConfigOrThrow;
			}
			return controller.CurrentStoreFrontConfigOrNull;
		}


		public static UserProfile CurrentUserProfile(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			BaseController controller = htmlHelper.BaseControllerOrThrow();

			if (throwErrorIfNotFound)
			{
				return controller.CurrentUserProfileOrThrow;
			}
			return controller.CurrentUserProfileOrNull;

		}

		public static StoreBinding GetCurrentStoreBinding(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			BaseController controller = htmlHelper.BaseControllerOrThrow();

			StoreBinding binding = controller.GStoreDb.GetCurrentStoreBindingOrNull(htmlHelper.ViewContext.HttpContext.Request);
			if (throwErrorIfNotFound && binding == null)
			{
				throw new ApplicationException("Store Binding not found for " 
					+ "Binding Host Name: " + htmlHelper.ViewContext.HttpContext.Request.BindingHostName()
					+ "Binding Port: " + htmlHelper.ViewContext.HttpContext.Request.BindingPort()
					+ "Binding Root Path: " + htmlHelper.ViewContext.HttpContext.Request.BindingRootPath());
			}
			return binding;
		}

		public static Page CurrentPage(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			PageBaseController controller = htmlHelper.PageBaseControllerOrThrow(throwErrorIfNotFound);
			if (controller == null)
			{
				return null;
			}

			if (throwErrorIfNotFound)
			{
				return controller.CurrentPageOrThrow;
			}
			return controller.CurrentPageOrNull;
		}

		public static IGstoreDb GStoreDb(this HtmlHelper htmlHelper)
		{
			return htmlHelper.BaseControllerOrThrow().GStoreDb;
		}

		public static string LayoutNameToUse(this HtmlHelper htmlHelper)
		{
			return Settings.AppDefaultLayoutName;
		}

		/// <summary>
		/// Returns the current theme folder for the controller, or the system default if error
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static string ThemeFolderName(this HtmlHelper htmlHelper)
		{
			Theme theme = htmlHelper.ViewData.Theme();
			if (theme != null)
			{
				return theme.FolderName;
			}
			BaseController controller = htmlHelper.BaseControllerOrThrow();
			return controller.ThemeFolderNameToUse;
		}


		public static bool UserHasPermission(this HtmlHelper htmlHelper, Identity.GStoreAction action)
		{
			return htmlHelper.CurrentStoreFront(false).Authorization_IsAuthorized(htmlHelper.CurrentUserProfile(false), action);
		}

		public static bool UserHasPermissionToAny(this HtmlHelper htmlHelper, params GStoreAction[] actions)
		{
			if (actions.Count() == 0)
			{
				throw new ApplicationException("UserHasPermissionToAny called but no action (permission) was supplied.  Be sure call to UserHasPermission has at least one action to check permission for.");
			}
			foreach (GStoreAction action in actions)
			{
				if (htmlHelper.CurrentStoreFront(false).Authorization_IsAuthorized(htmlHelper.CurrentUserProfile(false), action))
				{
					return true;
				}
			}
			return false;
		}

		public static bool UserHasAllPermissionToAll(this HtmlHelper htmlHelper, params GStoreAction[] actions)
		{
			if (actions.Count() == 0)
			{
				throw new ApplicationException("UserHasAllPermissionToAll called but no action (permission) was supplied.  Be sure call to UserHasPermission has at least one action to check permission for.");
			}
			foreach (GStoreAction action in actions)
			{
				if (!htmlHelper.CurrentStoreFront(false).Authorization_IsAuthorized(htmlHelper.CurrentUserProfile(false), action))
				{
					return false;
				}
			}
			return true;
		}

		public static bool UserIsSystemAdmin(this HtmlHelper htmlHelper)
		{
			UserProfile profile = htmlHelper.CurrentUserProfile(false);
			if (profile == null)
			{
				return false;
			}
			return profile.AspNetIdentityUserIsInRoleSystemAdmin();
		}

		/// <summary>
		/// Creates a sort link that runs an action
		/// internally uses Request["SortBy"] and Request["SortAscending"]
		/// the controller action target needs to have parameters string SortBy and bool? SortAscending
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static MvcHtmlString ActionSortLinkFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string action, bool useShortName = true, object additionalRouteData = null)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, new ViewDataDictionary<TModel>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = (useShortName ? metadata.ShortDisplayName : null) ?? metadata.DisplayName ?? (metadata.PropertyName ?? expressionText.Split(new char[] { '.' }).Last<string>());

			string[] dotValues = expressionText.Split('.');
			if (dotValues.Count() > 1)
			{
				string firstItem = dotValues[0];
				if (useShortName)
				{
					displayName = htmlHelper.DisplayShortName(firstItem).ToHtmlString() + " " + displayName;
				}
				else
				{
					displayName = htmlHelper.DisplayName(firstItem).ToHtmlString() + " " + displayName;
				}
			}

			return htmlHelper.ActionSortLink(displayName, action, expressionText, additionalRouteData: additionalRouteData);
		}

		/// <summary>
		/// Creates a sort link that runs an action
		/// internally uses Request["SortBy"] and Request["SortAscending"]
		/// the controller action target needs to have parameters string SortBy and bool? SortAscending
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static MvcHtmlString ActionSortLinkFor<TModel, TValue>(this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression, string action, bool useShortName = true, object additionalRouteData = null)
		{

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, new ViewDataDictionary<TModel>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = (useShortName ? metadata.ShortDisplayName : null) ?? metadata.DisplayName ?? (metadata.PropertyName ?? expressionText.Split(new char[] { '.' }).Last<string>());

			string[] dotValues = expressionText.Split('.');
			if (dotValues.Count() > 1)
			{
				string firstItem = dotValues[0];
				if (useShortName)
				{
					displayName = htmlHelper.DisplayShortName(firstItem).ToHtmlString() + " " + displayName;
				}
				else
				{
					displayName = htmlHelper.DisplayName(firstItem).ToHtmlString() + " " + displayName;
				}
			}

			return htmlHelper.ActionSortLink(displayName, action, expressionText, additionalRouteData: additionalRouteData);
		}

		/// <summary>
		/// Creates a sort link that runs an action ; uses a child object such as  model => yourobject.Name  and cuts out yourobjectname
		/// internally uses Request["SortBy"] and Request["SortAscending"]
		/// the controller action target needs to have parameters string SortBy and bool? SortAscending
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static MvcHtmlString ActionSortLinkForItem<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string action, bool removeItemName, bool useShortName = true, string TabName = "", int? id = null, object additionalRouteData = null)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, new ViewDataDictionary<TModel>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = (useShortName ? metadata.ShortDisplayName : null) ?? metadata.DisplayName ?? (metadata.PropertyName ?? expressionText.Split(new char[] { '.' }).Last<string>());

			string[] dotValues = expressionText.Split('.');
			if (dotValues.Count() > 1)
			{
				string firstItem = dotValues[0];
				if (useShortName)
				{
					displayName = htmlHelper.DisplayShortName(firstItem).ToHtmlString() + " " + displayName;
				}
				else
				{
					displayName = htmlHelper.DisplayName(firstItem).ToHtmlString() + " " + displayName;
				}
			}

			if (removeItemName && dotValues.Count() > 1)
			{
				expressionText = expressionText.Substring(expressionText.IndexOf('.') + 1);
			}

			return htmlHelper.ActionSortLink(displayName, action, expressionText, TabName, id, additionalRouteData: additionalRouteData);
		}

		/// <summary>
		/// Creates a sort link that runs an action
		/// internally uses Request["SortBy"] and Request["SortAscending"]
		/// the controller action target needs to have parameters string SortBy and bool? SortAscending
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="linkText"></param>
		/// <param name="action"></param>
		/// <param name="sortField"></param>
		/// <returns></returns>
		public static MvcHtmlString ActionSortLink(this HtmlHelper helper, string linkText, string action, string sortField, string TabName = "", int? id = null, object additionalRouteData = null)
		{

			HttpRequestBase request = helper.ViewContext.HttpContext.Request;

			bool currentFieldIsSorted = false;
			bool linkWillSortUp = true;
			bool? currentSortUp = null;

			if (string.IsNullOrEmpty(request["SortBy"]) || request["SortBy"].ToLower() != (sortField ?? string.Empty).ToLower())
			{
				//fresh sort
				//this field is not part of the sort
			}
			else
			{
				//this field is being sorted

				currentFieldIsSorted = true;
				//no ascending specified, ascending is implied
				if (string.IsNullOrEmpty(request["SortAscending"]) || request["SortAscending"].ToLower() == "true")
				{
					currentSortUp = true;
					//re-sorting, reverse order
					linkWillSortUp = false;
				}
				else
				{
					currentSortUp = false;
				}

			}

			string sortClue = string.Empty;
			string title = "Sort by " + linkText;
			if (currentFieldIsSorted)
			{
				if (currentSortUp.HasValue && currentSortUp.Value)
				{
					sortClue = " ↑";
					title += " Descending";
				}
				else
				{
					sortClue = " ↓";
					title += " Ascending";
				}

			}

			RouteValueDictionary routeData = null;
			if (additionalRouteData == null)
			{
				routeData = new RouteValueDictionary();
			}
			else
			{
				routeData = HtmlHelper.AnonymousObjectToHtmlAttributes(additionalRouteData);
			}

			routeData["SortBy"] = sortField;
			routeData["SortAscending"] = linkWillSortUp;
			if (!string.IsNullOrEmpty(TabName))
			{
				routeData["TabName"] = TabName;
			}
			if (id.HasValue)
			{
				routeData["Id"] = id.Value;
			}

			RouteValueDictionary htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(new { @title = title });

			string finalLinkText = linkText + (currentFieldIsSorted ? "*" : "");

			MvcHtmlString returnValue = helper.ActionLink(finalLinkText, action, routeData, htmlAttributes);

			if (!string.IsNullOrEmpty(sortClue))
			{
				returnValue = new MvcHtmlString(returnValue.ToHtmlString() + helper.Encode(sortClue));
			}

			return returnValue;
		}

		/// <summary>
		/// Renders alerts from viewbag and tempdata if any exist (messages to user)
		/// </summary>
		/// <returns></returns>
		public static MvcHtmlString RenderUserMessages<TModel>(this HtmlHelper<TModel> htmlHelper)
		{
			StringBuilder returnValue = new StringBuilder();

			TempDataDictionary tempData = htmlHelper.ViewContext.TempData;
			if (!tempData.ContainsKey("UserMessages"))
			{
				return new MvcHtmlString("");
			}
			//get user messages and remove them from the queue
			List<UserMessage> userMessages = (List<UserMessage>)tempData["UserMessages"];
			if (userMessages.Count == 0)
			{
				return new MvcHtmlString("");
			}

			returnValue.AppendLine("<!-- Start of User Messages -->");
			returnValue.AppendLine("<script>");
			foreach (UserMessage userMessage in userMessages)
			{
				returnValue.AppendLine("AddUserMessage(" + userMessage.Title.ToJsValue() + ", " + userMessage.Message.ToJsValue() + ", " + userMessage.MessageType.ToString().ToLower().ToJsValue() + ");");
			}
			userMessages.Clear();
			returnValue.AppendLine("</script>");
			returnValue.AppendLine("<!-- End of User Messages -->");

			return new MvcHtmlString(returnValue.ToString());
		}

		public static MvcHtmlString RenderUserMessagesBottom<TModel>(this HtmlHelper<TModel> htmlHelper)
		{
			StringBuilder returnValue = new StringBuilder();

			List<UserMessage> messages = new List<UserMessage>();
			TempDataDictionary tempData = htmlHelper.ViewContext.TempData;
			if (!tempData.ContainsKey("UserMessagesBottom"))
			{
				return new MvcHtmlString("");
			}
			//get user messages and remove them from the queue
			List<UserMessage> userMessagesBottom = (List<UserMessage>)tempData["UserMessagesBottom"];
			if (userMessagesBottom.Count == 0)
			{
				return new MvcHtmlString("");
			}
			returnValue.AppendLine("<!-- User Messages Bottom -->");
			returnValue.AppendLine("<script>");
			foreach (UserMessage userMessage in userMessagesBottom)
			{
				returnValue.AppendLine("AddUserMessageBottom(" + userMessage.Title.ToJsValue() + ", " + userMessage.Message.ToJsValue() + ", " + userMessage.MessageType.ToString().ToLower().ToJsValue() + ");");
			}
			userMessagesBottom.Clear();
			returnValue.AppendLine("</script>");

			return new MvcHtmlString(returnValue.ToString());
		}

		public static MvcHtmlString RenderAnnouncements<TModel>(this HtmlHelper<TModel> htmlHelper)
		{
			StringBuilder returnValue = new StringBuilder();

			TempDataDictionary tempData = htmlHelper.ViewContext.TempData;
			if (!tempData.ContainsKey("Announcements"))
			{
				return new MvcHtmlString("");
			}
			//get user messages ad remove them from the queue
			List<string> announcements = (List<string>)tempData["Announcements"];
			if (announcements.Count == 0)
			{
				return new MvcHtmlString("");
			}

			returnValue.AppendLine("<script>");
			foreach (string value in announcements)
			{
				returnValue.AppendLine("AddAnnouncement('" + HttpUtility.JavaScriptStringEncode(value) + "');");
			}
			returnValue.AppendLine("</script>");

			return new MvcHtmlString(returnValue.ToString());
		}

		public static MvcHtmlString RenderGaEvents<TModel>(this HtmlHelper<TModel> htmlHelper)
		{
			StringBuilder returnValue = new StringBuilder();

			TempDataDictionary tempData = htmlHelper.ViewContext.TempData;
			if (!tempData.ContainsKey("GaEvents"))
			{
				return new MvcHtmlString("");
			}
			//get user messages ad remove them from the queue
			List<GaEvent> gaEvents = (List<GaEvent>)tempData["GaEvents"];
			if (gaEvents.Count == 0)
			{
				return new MvcHtmlString("");
			}

			returnValue.AppendLine("<script>");
			returnValue.AppendLine("$(document).ready(function () {");

			foreach (GaEvent value in gaEvents)
			{
				string category = value.Category;
				string action = value.Action;
				string label = value.Label;

				returnValue.AppendLine("GaEvent("
					+ "'" + HttpUtility.JavaScriptStringEncode(category) + "'"
					+ ",'" + HttpUtility.JavaScriptStringEncode(action) + "'"
					+ ",'" + HttpUtility.JavaScriptStringEncode(label) + "'"
					+ ");");
			}
			returnValue.AppendLine("})");
			returnValue.AppendLine("</script>");

			return new MvcHtmlString(returnValue.ToString());
		}

		/// <summary>
		/// Writes a GaEvent script for an onclick event or anywhere a script is needed to track a google event
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="category"></param>
		/// <param name="action"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public static MvcHtmlString GaEventScript(this HtmlHelper htmlHelper, string category, string action, string label)
		{
			string script = "GaEvent("
				+ "'" + HttpUtility.JavaScriptStringEncode(category) + "'"
				+ ",'" + HttpUtility.JavaScriptStringEncode(action) + "'"
				+ ",'" + HttpUtility.JavaScriptStringEncode(label) + "'"
				+ ");";

			return new MvcHtmlString(script);

		}

		/// <summary>
		/// Javascript value for the google Analytics web property id; or "null" (no quotes) if none
		/// examples:   return  null  if EnableGoogleAnalytics = false, returns  'UA-example' if property id in storefront is UA-Example
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static MvcHtmlString GoogleAnalyticsWebPropertyIdValueJs(this HtmlHelper htmlHelper, StoreFrontConfiguration storeFrontConfig)
		{
			if (storeFrontConfig == null || !storeFrontConfig.EnableGoogleAnalytics)
			{
				return new MvcHtmlString("null");
			}

			string value = "'" + htmlHelper.JavaScriptEncode(storeFrontConfig.GoogleAnalyticsWebPropertyId, false) + "'";
			return new MvcHtmlString(value);
		}

		public static MvcHtmlString RenderNotificationLink<TModel>(this HtmlHelper<TModel> htmlHelper, Models.NotificationLink link, int counter)
		{
			return new MvcHtmlString(link.NotificationLinkTag(counter));
		}

		/// <summary>
		/// This method is used only for DBContext emails, use controller.SendEmail for other email sending
		/// </summary>
		/// <param name="db"></param>
		/// <returns></returns>
		public static bool SendEmailFromDBContext(this IGstoreDb db, Client client, string toAddress, string toName, string subject, string textBody, string htmlBody, string urlHost)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}

			string fromAddress = client.SendGridMailFromEmail;
			string fromName = client.SendGridMailFromName;
			string textSignature = EmailTextSignature(fromName, fromAddress, urlHost);
			string htmlSignature = EmailHtmlSignature(fromName, fromAddress, urlHost);

			bool result = false;
			try
			{
				result = SendEmailHelper(client, toAddress, toName, fromAddress, fromName, subject, htmlBody, htmlSignature, textBody, textSignature, urlHost);
			}
			catch (Exception ex)
			{
				db.LogEmailSent(null, null, null, toName, toAddress, fromName, fromAddress, subject, htmlBody, htmlSignature, textBody, textSignature, false, ex.ToString());
				throw;
			}

			db.LogEmailSent(null, null, null, toName, toAddress, fromName, fromAddress, subject, htmlBody, htmlSignature, textBody, textSignature, result, null);
			return result;
		}

		/// <summary>
		/// Sends an email and logs it to db or file if settings are set for it
		/// this logs failed email attempts as well as successful ones
		/// </summary>
		/// <param name="client"></param>
		/// <param name="db"></param>
		/// <param name="controller">Base controller or null if this is from db context and no controller is known</param>
		/// <param name="toAddress"></param>
		/// <param name="toName"></param>
		/// <param name="subject"></param>
		/// <param name="textBody"></param>
		/// <param name="htmlBody"></param>
		/// <param name="urlHost"></param>
		/// <returns></returns>
		public static bool SendEmail(this BaseController controller, string toAddress, string toName, string subject, string textBody, string htmlBody)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			Client client = controller.CurrentClientOrThrow;
			IGstoreDb db = controller.GStoreDb;

			string fromAddress = client.SendGridMailFromEmail;
			string fromName = client.SendGridMailFromName;
			string urlHost = controller.Request.Url.Host;
			string textSignature = EmailTextSignature(fromName, fromAddress, urlHost);
			string htmlSignature = EmailHtmlSignature(fromName, fromAddress, urlHost);

			bool result = false;
			try
			{
				result = SendEmailHelper(client, toAddress, toName, fromAddress, fromName, subject, htmlBody, htmlSignature, textBody, textSignature, urlHost);
			}
			catch (Exception ex)
			{
				db.LogEmailSent(controller.HttpContext, controller.RouteData, controller, toName, toAddress, fromName, fromAddress, subject, htmlBody, htmlSignature, textBody, textSignature, false, ex.ToString());
				throw;
			}

			db.LogEmailSent(controller.HttpContext, controller.RouteData, controller, toName, toAddress, fromName, fromAddress, subject, htmlBody, htmlSignature, textBody, textSignature, result, null);
			return result;
		}


		/// <summary>
		/// Sends an email and adds client signature to the end of it. Does not log, call SendEmail instead
		/// </summary>
		private static bool SendEmailHelper(Client client, string toAddress, string toName, string fromAddress, string fromName, string subject, string htmlBody, string htmlSignature, string textBody, string textSignature, string urlHost)
		{
			if (!Settings.AppEnableEmail)
			{
				return false;
			}

			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (!client.UseSendGridEmail)
			{
				return false;
			}

			System.Net.Mail.MailAddress fromMailAddress = new System.Net.Mail.MailAddress(fromAddress, fromName);
			System.Net.Mail.MailAddress[] toMailAddresses = { new System.Net.Mail.MailAddress(toAddress, toName) };

			if (string.IsNullOrEmpty(textSignature))
			{
				textSignature = EmailTextSignature(fromName, fromAddress, urlHost);
			}

			if (string.IsNullOrEmpty(htmlSignature))
			{
				htmlSignature = EmailHtmlSignature(fromName, fromAddress, urlHost);
			}

			textBody += textSignature;
			htmlBody += htmlSignature;

			SendGrid.SendGridMessage myMessage = new SendGrid.SendGridMessage(fromMailAddress, toMailAddresses, subject, htmlBody, textBody);

			System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(
				client.SendGridMailAccount,
				client.SendGridMailPassword
					   );

			// Create a Web transport for sending email.
			SendGrid.Web transportWeb = new SendGrid.Web(credentials);
			System.Threading.Tasks.Task sendMailTask = transportWeb.DeliverAsync(myMessage);
			sendMailTask.Wait(15000);

			return !sendMailTask.IsFaulted;
		}

		public static string EmailHtmlSignature(string fromName, string fromAddress, string urlHost)
		{
			return "<br/><hr/><br/>" + fromName
				+ "<br/>" + fromAddress
				+ "<br/><br/>Sent from " + urlHost;
		}

		public static string EmailTextSignature(string fromName, string fromAddress, string urlHost)
		{
			return "\n\n" + fromName
				+ "\n" + fromAddress
				+ "\n\nSent from http://" + urlHost;
		}

		public static bool SendSmsFromDBContext(this IGstoreDb db, Client client, string toPhone, string textBody, string urlHost)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}

			string fromPhone = client.TwilioFromPhone;
			string fromEmail = client.TwilioSmsFromEmail;
			string fromName = client.TwilioSmsFromName;

			string textSignature = SmsTextSignature(fromName, fromEmail, urlHost);

			bool result = false;
			try
			{
				result = SendSmsHelper(client, toPhone, textBody, textSignature, urlHost);
			}
			catch (Exception ex)
			{
				db.LogSmsSent(null, null, null, toPhone, fromPhone, textBody, textSignature, false, ex.ToString());
				throw;
			}

			db.LogSmsSent(null, null, null, toPhone, fromPhone, textBody, textSignature, result, null);
			return result;

		}

		public static bool SendSms(this BaseController controller, string toNumber, string textBody)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			Client client = controller.CurrentClientOrThrow;
			IGstoreDb db = controller.GStoreDb;

			string urlHost = controller.Request.Url.Host;

			string fromNumber = client.TwilioFromPhone;
			string fromEmail = client.TwilioSmsFromEmail;
			string fromName = client.TwilioSmsFromName;
			string textSignature = SmsTextSignature(fromName, fromEmail, urlHost);

			bool result = false;
			try
			{
				result = SendSmsHelper(client, toNumber, textBody, textSignature, urlHost);
			}
			catch (Exception ex)
			{
				db.LogSmsSent(controller.HttpContext, controller.RouteData, controller, toNumber, fromNumber, textBody, textSignature, false, ex.ToString());
				throw;
			}

			db.LogSmsSent(controller.HttpContext, controller.RouteData, controller, toNumber, fromNumber, textBody, textSignature, result, null);
			return result;
		}

		private static string SmsTextSignature(string smsFromName, string smsFromEmail, string urlHost)
		{
			return "\n\n" + smsFromName
							+ "\n" + smsFromEmail
							+ "\n\nSent from http://" + urlHost;
		}


		/// <summary>
		/// Sends a SMS text message and adds client signature to the end
		/// </summary>
		/// <param name="client"></param>
		/// <param name="toPhoneNumber"></param>
		/// <param name="textBody"></param>
		/// <param name="urlHost"></param>
		private static bool SendSmsHelper(Client client, string toNumber, string textBody, string textSignature, string urlHost)
		{
			if (!Settings.AppEnableSMS)
			{
				return false;
			}
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (!client.UseTwilioSms)
			{
				return false;
			}

			string smsFromEmail = client.TwilioSmsFromEmail;
			string smsFromName = client.TwilioSmsFromName;
			if (string.IsNullOrEmpty(textSignature))
			{
				textSignature = SmsTextSignature(smsFromName, smsFromEmail, urlHost);
			}
			textBody += textSignature;

			Twilio.TwilioRestClient Twilio = new Twilio.TwilioRestClient(
				client.TwilioSid,
				client.TwilioToken
		   );
			var result = Twilio.SendMessage(
				client.TwilioFromPhone,
				toNumber,
				textBody
				);

			// Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
			Trace.TraceInformation(result.Status);
			return true;

		}

		public static MvcHtmlString CatalogMenuItemStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels, bool isRegistered)
		{
			StringBuilder html = new StringBuilder();
			UrlHelper urlHelper = htmlHelper.UrlHelper();
			string accessKey = category.Entity.Name.Substring(0, 1);
			string displayName = category.Entity.Name;
			if (isRegistered)
			{
				displayName += " (" + category.Entity.ChildActiveCountForRegistered.ToString("N0") + ")";
			}
			else
			{
				displayName += " (" + category.Entity.ChildActiveCountForAnonymous.ToString("N0") + ")";
			}
			if (level == 1 && (category.HasChildMenuItems(maxLevels) || category.HasChildProductsToList(isRegistered)))
			{
				//for dropdown categories, make bootstrap dropdown menu for root
				html.AppendLine(htmlHelper.Tab(3 + level * 2) + "<li class=\"dropdown CatalogMenu CatalogMenuLevel" + level + "\">"
					+ "\n" + htmlHelper.Tab(4 + level * 2) + "<a href=\"#\""
						+ " class=\"dropdown-toggle\" data-toggle=\"dropdown\" accesskey=\"" + accessKey + "\" title=\"" +
						htmlHelper.AttributeEncode(displayName)
						+ "\">"
						+ htmlHelper.Encode(displayName)
						+ "<span class=\"caret\"></span>"
					+ "</a>");
			}
			else
			{
				//regular Leaf category no dropdown
				if (level != 1 && category.Entity.UseDividerBeforeOnMenu)
				{
					html.AppendLine(htmlHelper.Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n");
				}
				html.AppendLine(htmlHelper.Tab(3 + level * 2)
					+ "<li class=\"CatalogMenu CatalogMenuLevel" + level + "\">"
					+ "\n" + htmlHelper.Tab(4 + level * 2)
						+ "<a href=\""
							+ urlHelper.Action("ViewCategoryByName", "Catalog", new { urlName = category.Entity.UrlName })
							+ "\""
							+ " accesskey=\"" + accessKey + "\" title=\"" +
							htmlHelper.AttributeEncode(displayName)
						+ "\">"
						+ (level <= 2 ? string.Empty : htmlHelper.RepeatString("&nbsp;&nbsp;&nbsp;", (level - 2)))
						+ htmlHelper.Encode(displayName)
					+ "</a>");
				if (category.Entity.UseDividerAfterOnMenu)
				{
					html.AppendLine(htmlHelper.Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n");
				}
			}
			return new MvcHtmlString(html.ToString());
		}

		public static MvcHtmlString CatalogMenuItemAllLink<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels, bool isRegistered)
		{
			StringBuilder html = new StringBuilder();
			UrlHelper urlHelper = htmlHelper.UrlHelper();
			string accessKey = category.Entity.Name.Substring(0, 1);
			string displayName = category.Entity.Name;
			if (isRegistered)
			{
				displayName += " (" + category.Entity.ChildActiveCountForRegistered.ToString("N0") + ")";
			}
			else
			{
				displayName += " (" + category.Entity.ChildActiveCountForAnonymous.ToString("N0") + ")";
			}

			if (category.Entity.UseDividerBeforeOnMenu)
			{
				html.AppendLine(htmlHelper.Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n");
			}
			html.AppendLine(htmlHelper.Tab(3 + level * 2)
				+ "<li class=\"CatalogMenu CatalogMenuLevel" + level + "\">"
				+ "\n" + htmlHelper.Tab(4 + level * 2)
					+ "<a href=\""
						+ urlHelper.Action("ViewCategoryByName", "Catalog", new { urlName = category.Entity.UrlName })
						+ "\""
						+ " accesskey=\"" + accessKey + "\" title=\"" +
						htmlHelper.AttributeEncode(displayName)
					+ "\">"
					+ (level <= 2 ? string.Empty : htmlHelper.RepeatString("&nbsp;&nbsp;&nbsp;", (level - 2)))
					+ htmlHelper.Encode("- All -")
				+ "</a>");
			if (category.Entity.UseDividerAfterOnMenu)
			{
				html.AppendLine(htmlHelper.Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n");
			}

			return new MvcHtmlString(html.ToString());

		}

		public static MvcHtmlString NavBarItemMenuItemStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> navBarItem, int level, int maxLevels, bool noDropdown = false)
		{
			StringBuilder html = new StringBuilder();
			UrlHelper urlHelper = htmlHelper.UrlHelper();

			navBarItem.Entity.Url(urlHelper);
			string targetTag = string.Empty;
			if (navBarItem.Entity.OpenInNewWindow)
			{
				targetTag = " target=\"_blank\"";
			}

			string accessKey = navBarItem.Entity.Name.Substring(0, 1);

			if (!noDropdown && (level == 1) && navBarItem.HasChildMenuItems(maxLevels))
			{
				//for dropdown categories, make bootstrap dropdown menu for root
				html.AppendLine(htmlHelper.Tab(3 + level * 2) + "<li class=\"dropdown NavBarItem NavBarItemLevel" + level + "\">"
					+ "\n" + htmlHelper.Tab(4 + level * 2) + "<a href=\"#\""
						+ " class=\"dropdown-toggle\" data-toggle=\"dropdown\" accesskey=\"" + accessKey + "\" title=\"" +
						htmlHelper.AttributeEncode(navBarItem.Entity.Name)
						+ targetTag + "\">"
						+ htmlHelper.Encode(navBarItem.Entity.Name)
						+ "<span class=\"caret\"></span>"
					+ "</a>");
			}
			else
			{
				//regular Leaf NavBarItem no dropdown
				if (level != 1 && navBarItem.Entity.UseDividerBeforeOnMenu)
				{
					html.AppendLine(htmlHelper.Tab(3 + level * 2) + "<li class=\"divider NavBarItem NavBarItemLevel" + level + "\"></li>\n");
				}

				html.AppendLine(htmlHelper.Tab(3 + level * 2)
					+ "<li class=\"NavBarItem NavBarItemLevel" + level + "\">"
					+ "\n" + htmlHelper.Tab(4 + level * 2)
						+ "<a href=\""
							+ navBarItem.Entity.Url(urlHelper)
							+ "\""
							+ " accesskey=\"" + accessKey + "\" title=\"" +
							htmlHelper.AttributeEncode(navBarItem.Entity.Name)
						+ targetTag + "\">"
						+ (level <= 2 ? string.Empty : htmlHelper.RepeatString("&nbsp;&nbsp;&nbsp;", (level - 2)))
						+ htmlHelper.Encode(navBarItem.Entity.Name)
					+ "</a>");
			}
			return new MvcHtmlString(html.ToString());
		}

		public static MvcHtmlString CatalogMenuItemEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels)
		{
			string html = htmlHelper.Tab(3 + level * 2) + "</li>\n";
			if (category.Entity.UseDividerAfterOnMenu && level != 1)
			{
				html += htmlHelper.Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n";
			}
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString NavBarItemMenuItemEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> navBarItem, int level, int maxLevels)
		{
			string html = htmlHelper.Tab(3 + level * 2) + "</li>\n";
			if (navBarItem.Entity.UseDividerAfterOnMenu && level != 1)
			{
				html += htmlHelper.Tab(3 + level * 2) + "<li class=\"divider NavBarItem NavBarItemLevel" + level + "\"></li>\n";
			}
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString CatalogMenuChildContainerStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = htmlHelper.Tab(4 + level * 2) + "<ul class=\"dropdown-menu CatalogMenuChildContainer CatalogMenuChildContainerLevel" + level + "\" role=\"menu\">\n";
			}
			else
			{
				html = htmlHelper.Tab(4 + level * 2) + "<li class=\"CatalogMenu CatalogMenuLevel" + level + "\">\n";
			}

			return new MvcHtmlString(html);
		}

		public static MvcHtmlString CatalogMenuChildProductList<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels, bool isRegistered)
		{
			List<ProductBundle> bundles = category.Entity.ProductBundles.AsQueryable().WhereRegisteredAnonymousCheck(isRegistered).ApplyDefaultSort().ToList();
			List<ProductCategoryAltProductBundle> altBundles = category.Entity.CategoryAltProductBundles.AsQueryable().WhereRegisteredAnonymousCheck(isRegistered).ApplyDefaultSort().ToList();
			if (altBundles.Count != 0)
			{
				bundles.AddRange(altBundles.Select(b => b.ProductBundle));
				bundles = bundles.AsQueryable().ApplyDefaultSort().ToList();
			}

			List<Product> products = category.Entity.Products.AsQueryable().WhereRegisteredAnonymousCheck(isRegistered).ApplyDefaultSort().ToList();
			List<ProductCategoryAltProduct> altProducts = category.Entity.CategoryAltProducts.AsQueryable().WhereRegisteredAnonymousCheck(isRegistered).ApplyDefaultSort().ToList();
			if (altProducts.Count != 0)
			{
				products.AddRange(altProducts.Select(b => b.Product));
				products = products.AsQueryable().ApplyDefaultSort().ToList();
			}


			if (products.Count == 0 && bundles.Count == 0)
			{
				return null;
			}

			int counter = 0;
			StringBuilder html = new StringBuilder();

			foreach (ProductBundle bundle in bundles)
			{
				counter++;
				if (counter > 10)
				{
					break;
				}
				string bundleName = bundle.Name;
				string bundleUrl = bundle.BundleViewUrl(htmlHelper.UrlHelper());
				html.AppendLine(htmlHelper.Tab(4 + level * 2) + "<li class=\"CatalogMenu CatalogMenuLevel" + level + " CatalogNavBarProduct\">"
					+ "<a href=\"" + bundleUrl + "\">&nbsp;-&nbsp;" + bundleName.ToHtml() + "</a>"
					+ "</li>");
			}


			foreach (Product product in products)
			{
				counter++;
				if (counter > 10)
				{
					break;
				}
				string productName = product.Name;
				string productUrl = product.ProductViewUrl(htmlHelper.UrlHelper());
				html.Append(htmlHelper.Tab(4 + level * 2) + "<li class=\"CatalogMenu CatalogMenuLevel" + level + " CatalogNavBarProduct\">");
				if (level != 1)
				{
					html.Append(htmlHelper.RepeatString("&nbsp;", level * 2));
				}
				html.Append("<a href=\"" + productUrl + "\">&nbsp;-&nbsp;" + productName.ToHtml() + "</a>");
				html.AppendLine("</li>");
			}

			return new MvcHtmlString(html.ToString());
		}

		public static MvcHtmlString NavBarItemChildContainerStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> NavBarItem, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = htmlHelper.Tab(4 + level * 2) + "<ul class=\"dropdown-menu NavBarItemChildContainer NavBarItemChildContainerLevel" + level + "\" role=\"menu\">\n"
					+ NavBarItemMenuItemStart(htmlHelper, NavBarItem, level, maxLevels, true).ToHtmlString()
					+ NavBarItemMenuItemEnd(htmlHelper, NavBarItem, level, maxLevels).ToHtmlString();
			}
			else
			{
				html = htmlHelper.Tab(4 + level * 2) + "<li class=\"NavBarItem NavBarItemLevel" + level + "\">\n";
			}

			return new MvcHtmlString(html);
		}

		public static MvcHtmlString CatalogMenuChildContainerEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = htmlHelper.Tab(4 + level * 2) + "</ul>\n";
			}
			else
			{
				html = htmlHelper.Tab(4 + level * 2) + "</li>\n";
			}

			return new MvcHtmlString(html);
		}

		public static MvcHtmlString NavBarItemChildContainerEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> NavBarItem, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = htmlHelper.Tab(4 + level * 2) + "</ul>\n";
			}
			else
			{
				html = htmlHelper.Tab(4 + level * 2) + "</li>\n";
			}

			return new MvcHtmlString(html);
		}

		public static string UrlResolved(this Page page, UrlHelper urlHelper, bool absoluteLink = false)
		{
			return urlHelper.GStoreLocalUrl(page.Url, absoluteLink);
		}

		public static string UrlResolved(this PageEditViewModel pageEditViewModel, UrlHelper urlHelper, bool absoluteLink = false)
		{
			return urlHelper.GStoreLocalUrl(pageEditViewModel.Url, absoluteLink);
		}

		public static string Url(this NavBarItem navBarItem, UrlHelper urlHelper)
		{
			if (navBarItem.IsAction && !string.IsNullOrEmpty(navBarItem.Action) && !string.IsNullOrEmpty(navBarItem.Controller))
			{
				if (!string.IsNullOrEmpty(navBarItem.Area))
				{
					return urlHelper.Action(navBarItem.Action, navBarItem.Controller, new { Area = navBarItem.Area });
				}
				return urlHelper.Action(navBarItem.Action, navBarItem.Controller);
			}
			else if (navBarItem.IsPage && navBarItem.PageId.HasValue)
			{
				return navBarItem.Page.UrlResolved(urlHelper);
			}
			else if (navBarItem.IsLocalHRef && !string.IsNullOrEmpty(navBarItem.LocalHRef))
			{
				return urlHelper.GStoreLocalUrl(navBarItem.LocalHRef);
			}
			else if (navBarItem.IsRemoteHRef && !string.IsNullOrEmpty(navBarItem.RemoteHRef))
			{
				if (navBarItem.RemoteHRef.ToLower().StartsWith("http://") || navBarItem.RemoteHRef.ToLower().StartsWith("mailto:"))
				{
					return navBarItem.RemoteHRef;
				}
				return "http://" + navBarItem.RemoteHRef;
			}

			string errorMessage = "Nav Bar Item type is unknown or has no valid data. Nav Bar Item '" + navBarItem.Name + "' [" + navBarItem.NavBarItemId + "]";
			Debug.WriteLine("-- NavBarItem.Url error: " + errorMessage);

			return urlHelper.GStoreLocalUrl("/");
		}

		public static MvcHtmlString ProductCategoryWithParentLinks(this HtmlHelper htmlHelper, ProductCategory category, string catalogLinkText, bool level1IsLink, string htmlSeparator = " &gt; ", int maxLevels = 10)
		{
			//consider navigating from a treenode instead of recursing lazy nav props
			string htmlCatalogLink = string.Empty;
			if (!string.IsNullOrEmpty(catalogLinkText))
			{
				htmlCatalogLink = htmlHelper.ActionLink(catalogLinkText, "Index", "Catalog").ToHtmlString();
			}

			if (category == null)
			{
				return new MvcHtmlString(htmlCatalogLink);
			}

			string htmlLinks = RecurseProductCategoryParentLinks(category, 1, maxLevels, level1IsLink, htmlSeparator, htmlHelper).ToHtmlString();
			if (string.IsNullOrEmpty(htmlLinks) || string.IsNullOrEmpty(htmlCatalogLink))
			{
				return new MvcHtmlString(htmlCatalogLink + htmlLinks);
			}
			return new MvcHtmlString(htmlCatalogLink + htmlSeparator + htmlLinks);

		}

		private static MvcHtmlString RecurseProductCategoryParentLinks(ProductCategory category, int currentLevel, int maxLevels, bool level1IsLink, string htmlSeparator, HtmlHelper htmlHelper)
		{
			if (category == null)
			{
				return new MvcHtmlString(string.Empty);
			}

			MvcHtmlString link = null;
			if (currentLevel == 1 && !level1IsLink)
			{
				link = new MvcHtmlString(htmlHelper.Encode(category.Name));
			}
			else
			{
				link = htmlHelper.ActionLink(category.Name, "ViewCategoryByName", "Catalog", new { urlName = category.UrlName }, null);
			}

			if (category.ParentCategory != null && currentLevel < maxLevels)
			{
				MvcHtmlString parentHtml = RecurseProductCategoryParentLinks(category.ParentCategory, currentLevel + 1, maxLevels, level1IsLink, htmlSeparator, htmlHelper);
				return new MvcHtmlString(parentHtml.ToHtmlString() + "<span class=\"CategoryTopNavItem\">" + htmlSeparator + link.ToHtmlString() + "</span>");
			}

			return link;

		}

		/// <summary>
		/// Renders hidden fields for CreatedBy_UserProfileId, UpdatedBy_UserProfileId, CreateDateTimeUtc, UpdateDateTimeUtc
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString HiddenAuditFields<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel : Models.BaseClasses.AuditFieldsAllRequired
		{
			BaseController controller = htmlHelper.BaseControllerOrThrow();

			UserProfile profile = htmlHelper.CurrentUserProfile(true);
			int createdByUserProfileId = profile.UserProfileId;
			DateTime createDateTimeUtc = DateTime.UtcNow;
			if (htmlHelper.ViewData.Model != null)
			{
				createdByUserProfileId = htmlHelper.ViewData.Model.CreatedBy_UserProfileId;
				createDateTimeUtc = htmlHelper.ViewData.Model.CreateDateTimeUtc;
			}

			return new MvcHtmlString(htmlHelper.Hidden("CreatedBy_UserProfileId", createdByUserProfileId).ToHtmlString()
				+ htmlHelper.Hidden("CreateDateTimeUtc", createDateTimeUtc).ToHtmlString()
				+ htmlHelper.Hidden("UpdatedBy_UserProfileId", profile.UserProfileId).ToHtmlString()
				+ htmlHelper.Hidden("UpdateDateTimeUtc", DateTime.UtcNow).ToHtmlString());

		}

		/// <summary>
		/// Renders hidden fields for CreatedBy_UserProfileId, UpdatedBy_UserProfileId, CreateDateTimeUtc, UpdateDateTimeUtc
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString HiddenAuditFieldsOptional<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel : Models.BaseClasses.AuditFieldsUserProfileOptional
		{
			BaseController controller = htmlHelper.BaseControllerOrThrow();

			UserProfile profile = htmlHelper.CurrentUserProfile(true);
			int? createdByUserProfileId = profile.UserProfileId;
			DateTime createDateTimeUtc = DateTime.UtcNow;
			if (htmlHelper.ViewData.Model != null)
			{
				createdByUserProfileId = htmlHelper.ViewData.Model.CreatedBy_UserProfileId;
				createDateTimeUtc = htmlHelper.ViewData.Model.CreateDateTimeUtc;
			}

			return new MvcHtmlString(htmlHelper.Hidden("CreatedBy_UserProfileId", createdByUserProfileId).ToHtmlString()
				+ htmlHelper.Hidden("CreateDateTimeUtc", createDateTimeUtc).ToHtmlString()
				+ htmlHelper.Hidden("UpdatedBy_UserProfileId", profile.UserProfileId).ToHtmlString()
				+ htmlHelper.Hidden("UpdateDateTimeUtc", DateTime.UtcNow).ToHtmlString());

		}

		public static MvcHtmlString GStoreLocalLink(this HtmlHelper htmlHelper, string linkText, string localUrl)
		{
			return htmlHelper.GStoreLocalLink(linkText, localUrl, null);
		}

		public static MvcHtmlString GStoreLocalLink(this HtmlHelper htmlHelper, string linkText, string localUrl, string htmlAttributes)
		{
			string url = htmlHelper.UrlHelper().GStoreLocalUrl(localUrl);
			if (string.IsNullOrEmpty(htmlAttributes))
			{
				htmlAttributes = string.Empty;
			}
			else
			{
				htmlAttributes = " " + htmlAttributes;
			}

			string html = "<a href=\"" + url + "\"" + htmlAttributes + ">" + HttpUtility.HtmlEncode(linkText) + "</a>";
			return new MvcHtmlString(html);
		}

		/// <summary>
		/// Converts a local url like "/foo" into a local url using app path and /stores/storename if necessary
		/// Respects application root path, and StoresFolder if in use
		/// Set absoluteLink to create a fully qualified URL starting with http
		/// </summary>
		/// <param name="urlHelper">URL Helper</param>
		/// <param name="localUrl">Local URL may be "" or "/" or "~/" for root. slashes are trimmed</param>
		/// <param name="absoluteLink"></param>
		/// <returns></returns>
		public static string GStoreLocalUrl(this UrlHelper urlHelper, string localUrl, bool absoluteLink = false)
		{
			string urlTrimmed = localUrl.Trim('~').Trim('/');
			string url = urlTrimmed;
			string currentRawUrl = urlHelper.RequestContext.HttpContext.Request.RawUrl.Trim('/').ToLower();
			string appRoot = urlHelper.RequestContext.HttpContext.Request.ApplicationPath.Trim('/').ToLower();

			bool addAppRoot = false;
			bool addUrlStoreName = false;
			string urlStoreName = string.Empty;

			if (appRoot.Length != 0 && currentRawUrl.StartsWith(appRoot))
			{
				addAppRoot = true;
			}

			if (!localUrl.Trim('~').Trim('/').StartsWith("Content/"))
			{
				urlStoreName = urlHelper.RequestContext.RouteData.UrlStoreName();
				if (!string.IsNullOrEmpty(urlStoreName))
				{
					addUrlStoreName = true;
				}
			}

			if (absoluteLink)
			{
				Uri currentUrl = urlHelper.RequestContext.HttpContext.Request.Url;
				return currentUrl.Scheme + "://" + currentUrl.Host + (currentUrl.IsDefaultPort ? "" : ":" + currentUrl.Port) + "/" + url;
			}
			return "/" + (addAppRoot ? appRoot + "/" : "") + (addUrlStoreName ? "Stores/" + urlHelper.Encode(urlStoreName) + "/" : "") + url;
		}

		/// <summary>
		/// Displays a dynamic page section
		/// sectionName is the PageTemplateSections.Name
		/// Index is a 1-based index for the current section on the page. This index is used to keep scripts and updates in sync. Make sure to increment for every section
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="sectionName">sectionName is the PageTemplateSections.Name</param>
		/// <param name="index">Index is a 1-based index for the current section on the page. This index is used to keep scripts and updates in sync. Make sure to increment for every section</param>
		/// <returns></returns>
		public static MvcHtmlString DisplayPageSection(this HtmlHelper<PageViewModel> htmlHelper, string sectionName, int index, string description, string defaultRawHtmlValue, string preTextHtml, string postTextHtml, string defaultTextCssClass, bool editInTop, bool editInBottom)
		{

			PageSectionHelperResult pageSectionHelperResult = htmlHelper.PageSectionHelper(sectionName, index, description, defaultRawHtmlValue, preTextHtml, postTextHtml, defaultTextCssClass, null, false, editInTop, editInBottom);

			if (pageSectionHelperResult.IsHtmlResult)
			{
				return pageSectionHelperResult.MvcHtmlString;
			}
			if (!htmlHelper.ViewData.Model.EditMode)
			{
				return pageSectionHelperResult.PageTemplateSection.HtmlDisplay(pageSectionHelperResult.PageSection, htmlHelper);
			}
			return pageSectionHelperResult.PageTemplateSection.Editor(pageSectionHelperResult.Page, pageSectionHelperResult.PageSection, index, htmlHelper.ViewData.Model.AutoPost, htmlHelper);
		}

		/// <summary>
		/// Gets a value field for the current page stored in page content
		/// sectionName is the PageTemplateSections.Name
		/// Index is a 1-based index for the current section on the page. This index is used to keep scripts and updates in sync. Make sure to increment for every section
		/// If the string value is blank, returns default value
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="variableName">variableName is the same as sectionName (PageTemplateSections.Name)</param>
		/// <param name="index">Index is a 1-based index for the current section on the page. This index is used to keep scripts and updates in sync. Make sure to increment for every section</param>
		/// <returns></returns>
		public static string PageVariableString(this HtmlHelper<PageViewModel> htmlHelper, string variableName, int index, string description, string defaultValue, bool editInTop, bool editInBottom)
		{
			if (!editInTop && !editInBottom)
			{
				throw new ApplicationException("For page data, you must select either EditInTop or EditInBottom. Both cannot be false.");
			}

			PageSectionHelperResult pageSectionHelperResult = htmlHelper.PageSectionHelper(variableName, index, description, null, null, null, null, defaultValue, true, editInTop, editInBottom);

			if (pageSectionHelperResult.IsHtmlResult)
			{
				htmlHelper.BaseControllerOrThrow().AddUserMessage("Page Variable '" + variableName.ToHtml() + "'", pageSectionHelperResult.MvcHtmlString.ToHtmlString(), UserMessageType.Info);
				return null;
			}

			if (pageSectionHelperResult.PageSection != null && !string.IsNullOrEmpty(pageSectionHelperResult.PageSection.StringValue))
			{
				return pageSectionHelperResult.PageSection.StringValue;
			}
			return pageSectionHelperResult.PageTemplateSection.DefaultStringValue;
		}

		public struct PageSectionHelperResult
		{
			public PageSectionHelperResult(PageViewModel pageViewModel, PageTemplate pageTemplate, PageTemplateSection pageTemplateSection, Page page, PageSection pageSection)
			{
				this.PageViewModel = pageViewModel;
				this.PageTemplate = pageTemplate;
				this.PageTemplateSection = pageTemplateSection;
				this.Page = page;
				this.PageSection = pageSection;
				this.IsHtmlResult = false;
				this.MvcHtmlString = null;
			}

			public PageSectionHelperResult(MvcHtmlString htmlResult, PageViewModel pageViewModel)
			{
				this.PageViewModel = pageViewModel;
				this.PageTemplate = null;
				this.PageTemplateSection = null;
				this.Page = null;
				this.PageSection = null;
				this.IsHtmlResult = true;
				this.MvcHtmlString = htmlResult;
			}

			public PageViewModel PageViewModel;
			public PageSection PageSection;
			public Page Page;
			public PageTemplate PageTemplate;
			public PageTemplateSection PageTemplateSection;

			public bool IsHtmlResult;
			public MvcHtmlString MvcHtmlString;
		}

		private static PageSectionHelperResult PageSectionHelper(this HtmlHelper<PageViewModel> htmlHelper, string sectionName, int index, string description, string defaultRawHtmlValue, string preTextHtml, string postTextHtml, string defaultTextCssClass, string defaultStringValue, bool isVariable, bool editInTop, bool editInBottom)
		{
			if (index < 1)
			{
				throw new ArgumentOutOfRangeException("Index", "Template Error! Page Template Section Index must be 1 or greater");
			}
			if (string.IsNullOrWhiteSpace(sectionName))
			{
				throw new ArgumentOutOfRangeException("sectionName", "Template Error! Page Section Name cannot be blank");
			}
			if (string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentOutOfRangeException("description", "Template Error! Section Description cannot be blank");
			}

			bool autoSyncPageTemplateSection = htmlHelper.ViewData.PageTemplateAutoSync();

			PageViewModel pageViewModel = htmlHelper.ViewData.Model;
			if (pageViewModel == null)
			{
				throw new ArgumentNullException("pageViewModel");
			}

			if (pageViewModel.ForTemplateSyncOnly)
			{
				MvcHtmlString syncResult = SyncTemplateSectionHelper(htmlHelper, sectionName, index, description, defaultRawHtmlValue, preTextHtml, postTextHtml, defaultTextCssClass, defaultStringValue, isVariable, editInTop, editInBottom, pageViewModel);
				return new PageSectionHelperResult(syncResult, htmlHelper.ViewData.Model);
			}

			if (pageViewModel.Page == null)
			{
				throw new ArgumentNullException("Page", "Page cannot be null except in ForTemplateSyncOnly which is false");
			}
			Page page = pageViewModel.Page;
			PageTemplate pageTemplate = page.PageTemplate;
			PageTemplateSection pageTemplateSection = pageTemplate.Sections.Where(pts => pts.Name.ToLower() == sectionName.ToLower() && pts.IsVariable == isVariable).SingleOrDefault();

			if (pageTemplateSection == null)
			{
				System.Diagnostics.Trace.WriteLine("--Auto-creating page template section. Template: " + pageTemplate.Name + " [" + pageTemplate.PageTemplateId + "] Section Name: " + sectionName);
				IGstoreDb db = htmlHelper.GStoreDb();
				pageTemplateSection = db.CreatePageTemplateSection(pageTemplate.PageTemplateId, sectionName, 1000 + index, description, defaultRawHtmlValue, preTextHtml, postTextHtml, defaultTextCssClass, defaultStringValue, isVariable, editInTop, editInBottom, pageTemplate.ClientId, db.SeedAutoMapUserBestGuess());
			}
			else if (autoSyncPageTemplateSection)
			{
				//if flag is set to resync fields, update database if template has changed
				bool updateSection = false;

				if (pageTemplateSection.Description != description)
				{
					pageTemplateSection.Description = description;
					updateSection = true;
				}
				if (pageTemplateSection.DefaultRawHtmlValue != defaultRawHtmlValue)
				{
					pageTemplateSection.DefaultRawHtmlValue = defaultRawHtmlValue;
					updateSection = true;
				}
				if (pageTemplateSection.PreTextHtml != preTextHtml)
				{
					pageTemplateSection.PreTextHtml = preTextHtml;
					updateSection = true;
				}
				if (pageTemplateSection.PostTextHtml != postTextHtml)
				{
					pageTemplateSection.PostTextHtml = postTextHtml;
					updateSection = true;
				}
				if (pageTemplateSection.DefaultTextCssClass != defaultTextCssClass)
				{
					pageTemplateSection.DefaultTextCssClass = defaultTextCssClass;
					updateSection = true;
				}
				if (pageTemplateSection.DefaultStringValue != defaultStringValue)
				{
					pageTemplateSection.DefaultStringValue = defaultStringValue;
					updateSection = true;
				}
				if (pageTemplateSection.IsVariable != isVariable)
				{
					pageTemplateSection.IsVariable = isVariable;
					updateSection = true;
				}
				if (pageTemplateSection.EditInTop != editInTop)
				{
					pageTemplateSection.EditInTop = editInTop;
					updateSection = true;
				}
				if (pageTemplateSection.EditInBottom != editInBottom)
				{
					pageTemplateSection.EditInBottom = editInBottom;
					updateSection = true;
				}

				if (updateSection)
				{
					IGstoreDb db = htmlHelper.GStoreDb();
					UserProfile profile = htmlHelper.CurrentUserProfile(false);
					if (profile == null)
					{
						profile = db.SeedAutoMapUserBestGuess();
					}
					db.CachedUserProfile = profile;
					pageTemplateSection = db.PageTemplateSections.Update(pageTemplateSection);
					db.SaveChanges();
					db.CachedUserProfile = null;
				}
			}

			PageSection pageSection = page.Sections.AsQueryable().WhereIsActive()
				.Where(ps => ps.PageTemplateSectionId == pageTemplateSection.PageTemplateSectionId)
				.OrderBy(ps => ps.Order).ThenBy(ps => ps.PageSectionId)
				.FirstOrDefault();

			return new PageSectionHelperResult(pageViewModel, pageTemplate, pageTemplateSection, page, pageSection);
		}

		private static MvcHtmlString SyncTemplateSectionHelper(this HtmlHelper<PageViewModel> htmlHelper, string sectionName, int index, string description, string defaultRawHtmlValue, string preTextHtml, string postTextHtml, string defaultTextCssClass, string defaultStringValue, bool isVariable, bool editInTop, bool editInBottom, PageViewModel pageViewModel)
		{
			if (!pageViewModel.PageTemplateIdForSync.HasValue)
			{
				throw new ArgumentNullException("PageTemplateIdForSync", "PageTemplateIdForSync must be specified when ForTemplateSyncOnly is true");
			}
			int pageTemplateId = pageViewModel.PageTemplateIdForSync.Value;
			IGstoreDb db = htmlHelper.GStoreDb();
			PageTemplate template = db.PageTemplates.SingleOrDefault(pt => pt.PageTemplateId == pageTemplateId);
			if (template == null)
			{
				throw new ApplicationException("Page Template not found by id: " + pageTemplateId);
			}
			PageTemplateSection sectionTest = template.Sections.Where(pts => pts.Name.ToLower() == sectionName.ToLower()).SingleOrDefault();
			if (sectionTest == null)
			{
				sectionTest = db.CreatePageTemplateSection(pageTemplateId, sectionName, 1000 + index, description, defaultRawHtmlValue, preTextHtml, postTextHtml, defaultTextCssClass, defaultStringValue, isVariable, editInTop, editInBottom, template.ClientId, htmlHelper.CurrentUserProfile(true));
				return new MvcHtmlString("<span class=\"text-info\"><strong>New Section '" + htmlHelper.Encode(sectionTest.Name) + "' [" + sectionTest.PageTemplateSectionId + "] Created</strong></span><br/>");
			}
			bool update = false;
			if (sectionTest.DefaultRawHtmlValue != defaultRawHtmlValue)
			{
				sectionTest.DefaultRawHtmlValue = defaultRawHtmlValue;
				update = true;
			}
			if (sectionTest.PreTextHtml != preTextHtml)
			{
				sectionTest.PreTextHtml = preTextHtml;
				update = true;
			}
			if (sectionTest.PostTextHtml != postTextHtml)
			{
				sectionTest.PostTextHtml = postTextHtml;
				update = true;
			}
			if (sectionTest.DefaultTextCssClass != defaultTextCssClass)
			{
				sectionTest.DefaultTextCssClass = defaultTextCssClass;
				update = true;
			}
			if (sectionTest.DefaultStringValue != defaultStringValue)
			{
				sectionTest.DefaultStringValue = defaultStringValue;
				update = true;
			}
			if (sectionTest.IsVariable != isVariable)
			{
				sectionTest.IsVariable = isVariable;
				update = true;
			}

			if (sectionTest.Description != description)
			{
				sectionTest.Description = description;
				update = true;
			}
			if (sectionTest.Order != 1000 + index)
			{
				sectionTest.Order = 1000 + index;
				update = true;
			}
			if (sectionTest.EditInTop != editInTop)
			{
				sectionTest.EditInTop = editInTop;
				update = true;
			}
			if (sectionTest.EditInBottom != editInBottom)
			{
				sectionTest.EditInBottom = editInBottom;
				update = true;
			}

			if (!update)
			{
				return new MvcHtmlString("<span class=\"text-success\"><strong>Section OK '" + htmlHelper.Encode(sectionTest.Name) + "' [" + sectionTest.PageTemplateSectionId + "]</strong></span><br/>");
			}

			db.PageTemplateSections.Update(sectionTest);
			db.SaveChanges();
			return new MvcHtmlString("<span class=\"text-info\"><strong>Section Updated '" + htmlHelper.Encode(sectionTest.Name) + "' [" + sectionTest.PageTemplateSectionId + "]</strong></span><br/>");

		}

		/// <summary>
		/// Returns HTML value of the section
		/// </summary>
		/// <param name="pageSection"></param>
		/// <returns></returns>
		public static MvcHtmlString HtmlDisplay<TModel>(this PageTemplateSection pageTemplateSection, PageSection pageSection, HtmlHelper<TModel> htmlHelper)
		{
			if (pageTemplateSection == null)
			{
				throw new ArgumentNullException("pageTemplateSection");
			}

			string value = string.Empty;
			if (pageSection != null && !pageSection.UseDefaultFromTemplate)
			{
				if (pageSection.HasNothing)
				{
					value = string.Empty;
				}
				else if (pageSection.HasRawHtml && !string.IsNullOrEmpty(pageSection.RawHtml))
				{
					value = htmlHelper.ReplaceVariables(pageSection.RawHtml, string.Empty);
				}
				else if (pageSection.HasPlainText && !string.IsNullOrEmpty(pageSection.PlainText))
				{
					string cssClass = string.IsNullOrEmpty(pageSection.TextCssClass) ? pageTemplateSection.DefaultTextCssClass : pageSection.TextCssClass;
					value = (pageTemplateSection.PreTextHtml ?? "") + "<span class=\"" + cssClass + "\">" + HttpUtility.HtmlEncode(htmlHelper.ReplaceVariables(pageSection.PlainText, string.Empty)).Replace("\n", "<br/>\n") + "</span>" + (pageTemplateSection.PostTextHtml ?? "");
				}
			}
			else
			{
				value = htmlHelper.ReplaceVariables(pageTemplateSection.DefaultRawHtmlValue ?? string.Empty, string.Empty);
			}
			return new MvcHtmlString(value);
		}

		public static MvcHtmlString Editor<TModel>(this PageTemplateSection pageTemplateSection, Page page, PageSection pageSection, int index, bool autoSubmit, HtmlHelper<TModel> htmlHelper) where TModel : PageViewModel
		{
			ViewModels.PageSectionEditViewModel viewModel = new ViewModels.PageSectionEditViewModel(pageTemplateSection, page, pageSection, index, autoSubmit);
			return htmlHelper.EditorFor(model => viewModel, "PageSectionEditViewModel");
		}

		public static string ReplaceVariables(this HtmlHelper htmlHelper, string text, string nullValue = "")
		{
			if (text == null)
			{
				return null;
			}

			if (!text.HasVariables())
			{
				return text;
			}

			Client client = htmlHelper.CurrentClient(false);
			StoreFrontConfiguration storeFront = htmlHelper.CurrentStoreFrontConfig(false);
			UserProfile userProfile = htmlHelper.CurrentUserProfile(false);
			Page page = htmlHelper.CurrentPage(false);

			return text.ReplaceVariables(client, storeFront, userProfile, page, nullValue, true);
		}


		/// <summary>
		/// Checks if a string has variables to replace. Useful for dynamic HTML
		/// Leave prefix = "" to check for all variables, or set it to the prefix i.e. "client"  to check for client. variables
		/// </summary>
		/// <param name="text"></param>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public static bool HasVariables(this string text, string prefix = "")
		{
			if (text == null || string.IsNullOrWhiteSpace(text))
			{
				return false;
			}

			if (string.IsNullOrEmpty(prefix))
			{
				return text.Contains(fieldCode);
			}

			return text.Contains(fieldCode + prefix + ".");
		}

		public const string fieldCode = ":::";

		public static string ReplaceVariables(this string text, Client client, StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, Page page, string nullValue = "", bool confirmedVariables = false)
		{
			if (!confirmedVariables)
			{
				if (!text.HasVariables())
				{
					return text;
				}
			}

			if (text.HasVariables("client"))
			{
				string clientNullValue = null;
				if (client == null)
				{
					clientNullValue = nullValue;
				}

				text = text.Replace(fieldCode + "client.name" + fieldCode, clientNullValue ?? client.Name)
					.Replace(fieldCode + "client.clientid:::", clientNullValue ?? client.ClientId.ToString())
					.Replace(fieldCode + "client.sendgridmailfromemail:::", clientNullValue ?? client.SendGridMailFromEmail)
					.Replace(fieldCode + "client.sendgridmailfromname:::", clientNullValue ?? client.SendGridMailFromName)
					.Replace(fieldCode + "client.twiliofromphone:::", clientNullValue ?? client.TwilioFromPhone)
					.Replace(fieldCode + "client.twiliosmsfromemail:::", clientNullValue ?? client.TwilioSmsFromEmail)
					.Replace(fieldCode + "client.twiliosmsfromname:::", clientNullValue ?? client.TwilioSmsFromName);
			}
			
			if (text.HasVariables("storefront"))
			{
				string storeFrontNullValue = null;
				if (storeFrontConfig == null)
				{
					storeFrontNullValue = nullValue;
				}
				text = text.Replace(fieldCode + "storefront.name:::", storeFrontNullValue ?? storeFrontConfig.Name)
					.Replace(fieldCode + "storefront.clientid:::", storeFrontNullValue ?? storeFrontConfig.StoreFrontId.ToString())
					.Replace(fieldCode + "storefront.accountadmin.fullname:::", storeFrontNullValue ?? storeFrontConfig.AccountAdmin.FullName)
					.Replace(fieldCode + "storefront.accountadmin.email:::", storeFrontNullValue ?? storeFrontConfig.AccountAdmin.Email)
					.Replace(fieldCode + "storefront.registerednotify.fullname:::", storeFrontNullValue ?? storeFrontConfig.RegisteredNotify.FullName)
					.Replace(fieldCode + "storefront.registerednotify.email:::", storeFrontNullValue ?? storeFrontConfig.RegisteredNotify.Email)
					.Replace(fieldCode + "storefront.welcomeperson.fullname:::", storeFrontNullValue ?? storeFrontConfig.WelcomePerson.FullName)
					.Replace(fieldCode + "storefront.welcomeperson.email:::", storeFrontNullValue ?? storeFrontConfig.WelcomePerson.Email)
					.Replace(fieldCode + "storefront.orderadmin.fullname:::", storeFrontNullValue ?? storeFrontConfig.OrderAdmin.FullName)
					.Replace(fieldCode + "storefront.orderadmin.email:::", storeFrontNullValue ?? storeFrontConfig.OrderAdmin.Email)
					.Replace(fieldCode + "storefront.publicurl:::", storeFrontNullValue ?? storeFrontConfig.PublicUrl)
					.Replace(fieldCode + "storefront.htmlfooter:::", storeFrontNullValue ?? storeFrontConfig.HtmlFooter);
			}

			if (text.HasVariables("userprofile"))
			{
				string userProfileNullValue = null;
				if (userProfile == null)
				{
					userProfileNullValue = nullValue;
				}
				text = text.Replace(fieldCode + "userprofile.email:::", userProfileNullValue ?? userProfile.Email)
					.Replace(fieldCode + "userprofile.fullname:::", userProfileNullValue ?? userProfile.FullName)
					.Replace(fieldCode + "userprofile.username:::", userProfileNullValue ?? userProfile.UserName)
					.Replace(fieldCode + "userprofile.UserProfileId:::", userProfileNullValue ?? userProfile.UserProfileId.ToString());
			}

			if (text.HasVariables("page"))
			{
				string pageNullValue = null;
				if (page == null)
				{
					pageNullValue = nullValue;
				}
				text = text.Replace(fieldCode + "page.title:::", pageNullValue ?? page.PageTitle)
					.Replace(fieldCode + "page.name:::", pageNullValue ?? page.Name)
					.Replace(fieldCode + "page.pageid:::", pageNullValue ?? page.PageId.ToString())
					.Replace(fieldCode + "page.url:::", pageNullValue ?? page.Url);

			}

			if (text.HasVariables("date"))
			{
				DateTime today = DateTime.Today;
				text = text.Replace(fieldCode + "date.today:::", today.ToString())
					.Replace(fieldCode + "date.shortdate:::", today.ToShortDateString().ToString())
					.Replace(fieldCode + "date.shorttime:::", DateTime.UtcNow.ToLocalTime().ToShortTimeString())
					.Replace(fieldCode + "date.dayofweek:::", today.DayOfWeek.ToString())
					.Replace(fieldCode + "date.monthname:::", today.ToString("MMMM"))
					.Replace(fieldCode + "date.dayofmonth:::", today.Day.ToString())
					.Replace(fieldCode + "date.year:::", today.ToString("YYYY"));
			}

			return text;
		}

		public static MvcHtmlString DisplayPageForm<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel: PageViewModel
		{
			PageViewModel pageViewModel = htmlHelper.ViewData.Model;
			if (pageViewModel.ForTemplateSyncOnly || pageViewModel == null || pageViewModel.Page == null || pageViewModel.Page.WebFormId == null)
			{
				return new MvcHtmlString(string.Empty);
			}

			return htmlHelper.DisplayFor(model => model.Page.WebForm, htmlHelper.ViewData.Model.Page.WebForm.DisplayTemplateName);
		}


		public static MenuViewModel MenuViewModel(this HtmlHelper htmlHelper, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile, long? chatUserCount)
		{
			return new MenuViewModel(storeFrontConfiguration, userProfile, htmlHelper.ViewContext.HttpContext.Session.SessionID, chatUserCount);
		}

		public static MvcHtmlString LogFolderFileCount(this HtmlHelper htmlHelper, string logfolder)
		{
			string virtualPath = logfolder;
			string folderPath = htmlHelper.ViewContext.HttpContext.Server.MapPath(logfolder);

			if (!System.IO.Directory.Exists(folderPath))
			{
				return new MvcHtmlString("no folder");
			}

			string[] files = System.IO.Directory.GetFiles(folderPath);

			int fileCount = files.Count();

			return new MvcHtmlString(fileCount.ToString("N0") + " file" + (fileCount == 1 ? string.Empty : "s"));
		}

		/// <summary>
		/// Date and time of start of visit to site for this session
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public static DateTime? EntryDateTime(this HttpSessionStateBase session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return session["entryDateTimeUtc"] as DateTime?;
		}

		/// <summary>
		/// Raw Url for the first page on the site visited with this session
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public static string EntryRawUrl(this HttpSessionStateBase session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return session["entryRawUrl"] as string;
		}

		/// <summary>
		/// Url for the first page on the site visited with this session
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public static string EntryUrl(this HttpSessionStateBase session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return session["entryUrl"] as string;
		}

		/// <summary>
		/// URL Referrer for the first page on the site visited with this session
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public static string EntryReferrer(this HttpSessionStateBase session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return session["entryReferrer"] as string;
		}

		public static char[] InvalidUrlNameCharactersArray(this string urlName)
		{
			return new char[] { '/', '\\', '%', '?', '<', '>', '*', ':', '&', '"', '\'', ' ', '!' };
		}

		public static string InvalidUrlNameCharacters(this string urlName)
		{
			char[] invalidChars = urlName.InvalidUrlNameCharactersArray();
			return "'" + string.Join("', '", invalidChars) + "'";
		}

		public static bool IsValidUrlName(this string urlName)
		{
			foreach (char charX in urlName.InvalidUrlNameCharacters())
			{
				if (urlName.Contains(charX))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Replaces invalid urlname chatacters with -
		/// </summary>
		/// <param name="urlName"></param>
		/// <returns></returns>
		public static string FixUrlName(this string urlName)
		{
			char[] invalidChars = urlName.InvalidUrlNameCharactersArray();
			foreach (char charX in invalidChars)
			{
				urlName = urlName.Replace(charX, '-');
			}
			return urlName;
		}

		/// <summary>
		/// URLEncodes a string to make a valid file name
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToFileName(this string value)
		{
			return HttpUtility.UrlEncode(value);
		}

		/// <summary>
		/// Returns True/False whether the template should show HelpLabel as display text on the screen
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static bool UseHelpLabelText(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("UseHelpLabelText"))
			{
				return (viewData["UseHelpLabelText"] as bool?).Value;
			}
			return true;
		}

		/// <summary>
		/// Sets the value True/False whether the template should show HelpLabel as display text on the screen
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static void SetUseHelpLabelText(this ViewDataDictionary viewData, bool value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["UseHelpLabelText"] = value;
		}

		/// <summary>
		/// Returns True/False whether the template should show the Label for the field
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static bool ShowLabel(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("ShowLabel"))
			{
				return (viewData["ShowLabel"] as bool?).Value;
			}
			return true;
		}

		public static string LabelText(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("LabelText"))
			{
				return viewData["LabelText"] as string;
			}
			return null;
		}

		public static void LabelText(this ViewDataDictionary viewData, string value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["LabelText"] = value;
		}

		/// <summary>
		/// Returns True/False whether the template should show HelpLabel as display text on the screen
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static void SetShowLabel(this ViewDataDictionary viewData, bool value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["ShowLabel"] = value;
		}

		/// <summary>
		/// Gets the MaxLength for a textbox template
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static int? MaxLength(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			return viewData["MaxLength"] as int?;
		}

		/// <summary>
		/// Gets the DisplayName setting from View Data for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static string DisplayName(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("DisplayName"))
			{
				return viewData["DisplayName"] as string;
			}
			return null;
		}

		/// <summary>
		/// Sets the DisplayName setting from View Data for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static void DisplayName(this ViewDataDictionary viewData, string value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["DisplayName"] = value;
		}

		/// <summary>
		/// Gets the Required setting from View Data for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static bool Required(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("Required"))
			{
				return (viewData["Required"] as bool?) ?? false;
			}
			return false;
		}

		/// <summary>
		/// Sets the Required setting from View Data for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static void Required(this ViewDataDictionary viewData, bool value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["Required"] = value;
		}

		/// <summary>
		/// Sets the MaxLength for a textbox template
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static void SetMaxLength(this ViewDataDictionary viewData, int value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["MaxLength"] = value;
		}

		/// <summary>
		/// Returns True/False whether the template should show HelpLabel popover
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static bool UseHelpLabelPopover(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("UseHelpLabelPopover"))
			{
				return (viewData["UseHelpLabelPopover"] as bool?).Value;
			}
			return true;
		}

		/// <summary>
		/// Sets the value True/False whether the template should show HelpLabel popover
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static void SetUseHelpLabelPopover(this ViewDataDictionary viewData, bool value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["UseHelpLabelPopover"] = value;
		}

		/// <summary>
		/// Gets the focus field for layout to auto-focus when document loads
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="fieldId"></param>
		public static string FocusId(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("FocusId"))
			{
				return viewData["FocusId"] as string;
			}
			return null;
		}

		/// <summary>
		/// Sets the focus field for layout to auto-focus when document loads
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="fieldId"></param>
		public static void FocusId(this ViewDataDictionary viewData, string fieldId)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["FocusId"] = fieldId;
		}


		/// <summary>
		/// Gets the View data setting for SetFocusToFirstInput (default is true)
		/// </summary>
		/// <param name="viewData"></param>
		public static bool SetFocusToFirstInput(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("SetFocusToFirstInput"))
			{
				return (viewData["SetFocusToFirstInput"] as bool?) ?? true;
			}
			return true;
		}

		/// <summary>
		/// Gets the View data setting for SetFocusToFirstInput for system admin section where default is false
		/// </summary>
		/// <param name="viewData"></param>
		public static bool SetFocusToFirstInputDefaultFalse(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("SetFocusToFirstInput"))
			{
				return (viewData["SetFocusToFirstInput"] as bool?) ?? false;
			}
			return false;
		}

		/// <summary>
		/// Sets the View data setting for SetFocusToFirstInput (default is true)
		/// </summary>
		/// <param name="viewData"></param>
		public static void SetFocusToFirstInput(this ViewDataDictionary viewData, bool value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["SetFocusToFirstInput"] = value;
		}

		
		/// <summary>
		/// Returns the column offset MD to use before the label for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <returns></returns>
		public static int ColOffset(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("ColOffset"))
			{
				return (viewData["ColOffset"] as int?).Value;
			}
			return 1;
		}

		/// <summary>
		/// Sets ColOffset, ColLabel, and ColData for view data used in display templates
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="label"></param>
		/// <param name="data"></param>
		public static void SetColDisplay(this ViewDataDictionary viewData, int colOffset = 1, int colLabel = 3 , int colData = 7, bool useHelpLabelText = true, bool useHelpLabelPopover = true)
		{
			viewData.SetColOffset(colOffset);
			viewData.SetColLabel(colLabel);
			viewData.SetColData(colData);
			viewData.SetUseHelpLabelText(useHelpLabelText);
			viewData.SetUseHelpLabelPopover(useHelpLabelPopover);
		}

		/// <summary>
		/// Sets the column offset MD to use before the label for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static void SetColOffset(this ViewDataDictionary viewData, int value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["ColOffset"] = value;
		}

		/// <summary>
		/// Returns the column count to use on MD on the label for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <returns></returns>
		public static int ColLabel(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("ColLabel"))
			{
				return (viewData["ColLabel"] as int?).Value;
			}
			return 3;
		}

		/// <summary>
		/// Sets the column count to use on MD on the label for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static void SetColLabel(this ViewDataDictionary viewData, int value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["ColLabel"] = value;
		}

		/// <summary>
		/// Gets the column count to use on MD on the Data for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static int ColData(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("ColData"))
			{
				return (viewData["ColData"] as int?).Value;
			}
			return 7;
		}

		/// <summary>
		/// Sets the column count to use on MD on the Data for a field in the display templates
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="value"></param>
		public static void SetColData(this ViewDataDictionary viewData, int value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["ColData"] = value;
		}

		public static string OptionLabel(this ViewDataDictionary viewData)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			if (viewData.ContainsKey("OptionLabel"))
			{
				return viewData["OptionLabel"] as string;
			}
			return null;
		}

		public static void SetOptionLabel(this ViewDataDictionary viewData, string value)
		{
			if (viewData == null)
			{
				throw new ArgumentNullException("viewData");
			}
			viewData["OptionLabel"] = value;
		}

		/// <summary>
		/// Gets the page setting for PageTemplateAutoSync. Returns false if not set (default for performance)
		/// </summary>
		/// <param name="viewData"></param>
		/// <returns></returns>
		public static bool PageTemplateAutoSync(this ViewDataDictionary viewData)
		{
			return (viewData["PageTemplateAutoSync"] as bool?) ?? false;
		}

		/// <summary>
		/// Sets the page setting for PageTemplateAutoSync. default is false for performance
		/// </summary>
		/// <param name="viewData"></param>
		/// <returns></returns>
		public static void PageTemplateAutoSync(this ViewDataDictionary viewData, bool value)
		{
			viewData["PageTemplateAutoSync"] = value;
		}

		/// <summary>
		/// Gets the Theme if it is set in ViewData[Theme]
		/// </summary>
		/// <param name="viewData"></param>
		/// <returns></returns>
		public static Theme Theme(this ViewDataDictionary viewData)
		{
			return viewData["Theme"] as Theme;
		}

		/// <summary>
		/// Sets the Theme set in ViewData[Theme]
		/// </summary>
		/// <param name="viewData"></param>
		/// <param name="theme"></param>
		public static void Theme(this ViewDataDictionary viewData, Theme theme)
		{
			viewData["Theme"] = theme;
		}

		const string Session_BlogAdminVisitLogged = "BlogAdminVisitLogged";
		public static bool BlogAdminVisitLogged(this HttpSessionStateBase session)
		{
			bool? value = session[Session_BlogAdminVisitLogged] as bool?;
			if (value.HasValue && value.Value)
			{
				return true;
			}
			return false;
		}
		public static void BlogAdminVisitLogged(this HttpSessionStateBase session, bool value)
		{
			session[Session_BlogAdminVisitLogged] = value;
		}

		const string Session_CatalogAdminVisitLogged = "CatalogAdminVisitLogged";
		public static bool CatalogAdminVisitLogged(this HttpSessionStateBase session)
		{
			bool? value = session[Session_CatalogAdminVisitLogged] as bool?;
			if (value.HasValue && value.Value)
			{
				return true;
			}
			return false;
		}
		public static void CatalogAdminVisitLogged(this HttpSessionStateBase session, bool value)
		{
			session[Session_CatalogAdminVisitLogged] = value;
		}

		const string Session_OrderAdminVisitLogged = "OrderAdminVisitLogged";
		public static bool OrderAdminVisitLogged(this HttpSessionStateBase session)
		{
			bool? value = session[Session_OrderAdminVisitLogged] as bool?;
			if (value.HasValue && value.Value)
			{
				return true;
			}
			return false;
		}
		public static void OrderAdminVisitLogged(this HttpSessionStateBase session, bool value)
		{
			session[Session_OrderAdminVisitLogged] = value;
		}

		const string Session_StoreAdminVisitLogged = "StoreAdminVisitLogged";
		public static bool StoreAdminVisitLogged(this HttpSessionStateBase session)
		{
			bool? value = session[Session_StoreAdminVisitLogged] as bool?;
			if (value.HasValue && value.Value)
			{
				return true;
			}
			return false;
		}
		public static void StoreAdminVisitLogged(this HttpSessionStateBase session, bool value)
		{
			session[Session_StoreAdminVisitLogged] = value;
		}

		const string Session_SystemAdminVisitLogged = "SystemAdminVisitLogged";
		public static bool SystemAdminVisitLogged(this HttpSessionStateBase session)
		{
			bool? value = session[Session_SystemAdminVisitLogged] as bool?;
			if (value.HasValue && value.Value)
			{
				return true;
			}
			return false;
		}
		public static void SystemAdminVisitLogged(this HttpSessionStateBase session, bool value)
		{
			session[Session_SystemAdminVisitLogged] = value;
		}

		public static TimeZoneInfo StoreFrontTimeZone(this HtmlHelper htmlHelper)
		{
			StoreFrontConfiguration storeFrontConfig = htmlHelper.CurrentStoreFrontConfig(false);
			Client client = htmlHelper.CurrentClient(false);
			return StoreFrontTimeZone(storeFrontConfig, client);
		}

		public static TimeZoneInfo UserTimeZone(this HtmlHelper htmlHelper)
		{
			UserProfile userProfile = htmlHelper.CurrentUserProfile(false);
			if (userProfile != null && (!string.IsNullOrEmpty(userProfile.TimeZoneId)))
			{
				try
				{
					return TimeZoneInfo.FindSystemTimeZoneById(userProfile.TimeZoneId);
				}
				catch (Exception)
				{
					//user time zone is not found in system time zones
				}
			}
			StoreFrontConfiguration storeFrontConfig = htmlHelper.CurrentStoreFrontConfig(false);
			Client client = htmlHelper.CurrentClient(false);
			return StoreFrontTimeZone(storeFrontConfig, client);
		}

		public static TimeZoneInfo UserTimeZone(this UserProfile profile, StoreFrontConfiguration storeFrontConfig, Client client)
		{

			if (profile != null && !string.IsNullOrWhiteSpace(profile.TimeZoneId))
			{
				//user has a time zone set, try it
				try
				{
					return TimeZoneInfo.FindSystemTimeZoneById(profile.TimeZoneId);
				}
				catch (Exception)
				{
					//user time zone not found in system time zones
				}
			}

			return storeFrontConfig.StoreFrontTimeZone(client);
		}

		public static TimeZoneInfo StoreFrontTimeZone(this StoreFrontConfiguration storeFrontConfig, Client client)
		{
			if (storeFrontConfig != null)
			{
				if (!string.IsNullOrEmpty(storeFrontConfig.TimeZoneId))
				{
					try
					{
						return TimeZoneInfo.FindSystemTimeZoneById(storeFrontConfig.TimeZoneId);
					}
					catch (Exception)
					{
						//store front time zone is not found in system time zones
					}
				}
			}

			if (client != null)
			{
				if (!string.IsNullOrEmpty(client.TimeZoneId))
				{
					try
					{
						return TimeZoneInfo.FindSystemTimeZoneById(client.TimeZoneId);
					}
					catch (Exception)
					{
						//client time zone is not found in system time zones
					}
				}
			}

			if (string.IsNullOrEmpty(Settings.AppDefaultTimeZoneId))
			{
				return TimeZoneInfo.Local;
			}
			try
			{
				return TimeZoneInfo.FindSystemTimeZoneById(Settings.AppDefaultTimeZoneId);
			}
			catch (Exception)
			{
				//can't find the time zone in settings, return server local time
			}
			return TimeZoneInfo.Local;
		}

		public static TimeZoneInfo ClientTimeZone(this HtmlHelper htmlHelper)
		{
			Client client = htmlHelper.CurrentClient(false);
			return ClientTimeZone(client);
		}

		public static TimeZoneInfo ClientTimeZone(this Client client)
		{
			if (client != null)
			{
				if (!string.IsNullOrEmpty(client.TimeZoneId))
				{
					try
					{
						return TimeZoneInfo.FindSystemTimeZoneById(client.TimeZoneId);
					}
					catch (Exception)
					{
						//client time zone is not found in system time zones
					}
				}
			}

			if (string.IsNullOrEmpty(Settings.AppDefaultTimeZoneId))
			{
				return TimeZoneInfo.Local;
			}
			try
			{
				return TimeZoneInfo.FindSystemTimeZoneById(Settings.AppDefaultTimeZoneId);
			}
			catch (Exception)
			{
				//can't find the time zone in settings, return server local time
			}
			return TimeZoneInfo.Local;
		}

		public static TimeZoneInfo GStoreSystemDefaultTimeZone(this HtmlHelper htmlHelper)
		{
			return GStoreSystemDefaultTimeZone();
		}

		public static TimeZoneInfo GStoreSystemDefaultTimeZone()
		{
			if (string.IsNullOrEmpty(Settings.AppDefaultTimeZoneId))
			{
				return TimeZoneInfo.Local;
			}
			try
			{
				return TimeZoneInfo.FindSystemTimeZoneById(Settings.AppDefaultTimeZoneId);
			}
			catch (Exception)
			{
				//can't find the time zone in settings, return server local time
			}
			return TimeZoneInfo.Local;
		}

		public static string ToUserDateTimeString(this DateTime utcTime, HtmlHelper htmlHelper)
		{
			return utcTime.ToUserDateTime(htmlHelper).ToString() + " " + htmlHelper.UserTimeZone().ToShortName();
		}

		public static string ToUserDateTimeString(this DateTime utcTime, UserProfile profile, StoreFrontConfiguration storeFrontConfig, Client client)
		{
			return utcTime.ToUserDateTime(profile, storeFrontConfig, client).ToString() + " " + profile.UserTimeZone(storeFrontConfig, client).ToShortName();
		}

		public static DateTime ToUserDateTime(this DateTime utcTime, HtmlHelper htmlHelper)
		{
			TimeZoneInfo timeZone = htmlHelper.UserTimeZone();
			if (timeZone != null)
			{
				return TimeZoneInfo.ConvertTime(utcTime, MvcHtmlHelper.UtcTimeZone(), timeZone);
			}
			return utcTime.ToLocalTime();
		}

		public static DateTime ToUserDateTime(this DateTime utcTime, UserProfile profile, StoreFrontConfiguration storeFrontConfig, Client client)
		{
			TimeZoneInfo timeZone = profile.UserTimeZone(storeFrontConfig, client);
			if (timeZone != null)
			{
				return TimeZoneInfo.ConvertTime(utcTime, MvcHtmlHelper.UtcTimeZone(), timeZone);
			}
			return utcTime.ToLocalTime();
		}

		/// <summary>
		/// Returns a formatted string for date/time display with time zone
		/// </summary>
		/// <param name="utcTime"></param>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static string ToStoreDateTimeString(this DateTime utcTime, HtmlHelper htmlHelper)
		{
			return utcTime.ToStoreDateTime(htmlHelper) + " " + htmlHelper.StoreFrontTimeZone().ToShortName();
		}

		public static string ToStoreDateTimeString(this DateTime utcTime, StoreFrontConfiguration storeFrontConfig, Client client)
		{
			return utcTime.ToStoreDateTime(storeFrontConfig, client) + " " + storeFrontConfig.StoreFrontTimeZone(client).ToShortName();
		}

		public static DateTime ToStoreDateTime(this DateTime utcTime, HtmlHelper htmlHelper)
		{
			StoreFrontConfiguration storeFrontConfig = htmlHelper.CurrentStoreFrontConfig(false);
			Client client = htmlHelper.CurrentClient(false);
			return utcTime.ToStoreDateTime(storeFrontConfig, client);
		}

		public static DateTime ToStoreDateTime(this DateTime utcTime, StoreFrontConfiguration storeFrontConfig, Client client)
		{
			TimeZoneInfo storeFrontTimeZone = storeFrontConfig.StoreFrontTimeZone(client);

			if (storeFrontTimeZone != null)
			{
				return TimeZoneInfo.ConvertTime(utcTime, MvcHtmlHelper.UtcTimeZone(), storeFrontTimeZone);
			}
			return utcTime.ToLocalTime();
		}

		public static string ToClientDateTimeString(this DateTime utcTime, HtmlHelper htmlHelper)
		{
			return utcTime.ToClientDateTime(htmlHelper).ToString() + " " + htmlHelper.ClientTimeZone().ToShortName();
		}

		public static string ToClientDateTimeString(this DateTime utcTime, Client client)
		{
			return utcTime.ToClientDateTime(client).ToString() + " " + client.ClientTimeZone().ToShortName();
		}

		public static DateTime ToClientDateTime(this DateTime utcTime, HtmlHelper htmlHelper)
		{
			Client client = htmlHelper.CurrentClient(false);
			return utcTime.ToClientDateTime(client);
		}

		public static DateTime ToClientDateTime(this DateTime utcTime, Client client)
		{
			TimeZoneInfo clientTimeZone = client.ClientTimeZone();

			if (clientTimeZone != null)
			{
				return TimeZoneInfo.ConvertTime(utcTime, MvcHtmlHelper.UtcTimeZone(), clientTimeZone);
			}
			return utcTime.ToLocalTime();
		}

		public static string ToGStoreSystemDefaultDateTimeString(this DateTime utcTime, HtmlHelper htmlHelper)
		{
			return utcTime.ToGStoreSystemDefaultDateTime().ToString() + " " + htmlHelper.GStoreSystemDefaultTimeZone().ToShortName();
		}

		public static string ToGStoreSystemDefaultDateTimeString(this DateTime utcTime)
		{
			return utcTime.ToGStoreSystemDefaultDateTime().ToString() + " " + GStoreSystemDefaultTimeZone().ToShortName();
		}

		public static DateTime ToGStoreSystemDefaultDateTime(this DateTime utcTime)
		{
			return TimeZoneInfo.ConvertTime(utcTime, MvcHtmlHelper.UtcTimeZone(), GStoreSystemDefaultTimeZone());
		}

		public static MvcHtmlString MetaApplicationName(this HtmlHelper html)
		{
			return new MvcHtmlString(html.BaseControllerOrThrow().MetaApplicationName);
		}

		public static MvcHtmlString MetaApplicationTileColor(this HtmlHelper html)
		{
			return new MvcHtmlString(html.BaseControllerOrThrow().MetaApplicationTileColor);
		}

		public static MvcHtmlString MetaDescription(this HtmlHelper html)
		{
			return new MvcHtmlString(html.BaseControllerOrThrow().MetaDescription);
		}

		public static MvcHtmlString MetaKeywords(this HtmlHelper html)
		{
			return new MvcHtmlString(html.BaseControllerOrThrow().MetaKeywords);
		}

		public static MvcHtmlString BodyTopScriptTag(this HtmlHelper html)
		{
			return new MvcHtmlString(html.BaseControllerOrThrow().BodyTopScriptTag);
		}

		public static MvcHtmlString BodyBottomScriptTag(this HtmlHelper html)
		{
			return new MvcHtmlString(html.BaseControllerOrThrow().BodyBottomScriptTag);
		}

		public static BaseController BaseControllerOrThrow(this HtmlHelper html)
		{
			BaseController controller = html.ViewContext.Controller as BaseController;
			if (controller == null)
			{
				throw new ApplicationException("Controller not found. Make sure controller inherits from GStoreData.ControllerBase.BaseController or a descendant");
			}

			return controller;
		}

		public static PageBaseController PageBaseControllerOrThrow(this HtmlHelper html, bool throwErrorIfNotPageBase)
		{
			PageBaseController controller = html.ViewContext.Controller as PageBaseController;
			if (controller == null && throwErrorIfNotPageBase)
			{
				throw new ApplicationException("Page Controller not found. Make sure controller inherits from GStoreData.ControllerBase.PageBaseController or a descendant");
			}

			return controller;
		}

		public static string DetermineAreaFromRequest(this HttpRequestBase request)
		{
			return request.Url.DetermineAreaFromUrl(request.ApplicationPath);
		}
		public static string DetermineAreaFromRequest(this HttpRequest request)
		{
			return request.Url.DetermineAreaFromUrl(request.ApplicationPath);
		}

		public static string DetermineAreaFromUrl(this Uri currentUrl, string applicationPath)
		{
			string url = currentUrl.AbsolutePath.Trim('/').ToLower();
			string relativeUrl = url;
			string appPath = applicationPath.Trim('/');
			if (!string.IsNullOrEmpty(appPath))
			{
				relativeUrl = url.Remove(0, appPath.Length);
			}

			if (relativeUrl.StartsWith("blogadmin"))
			{
				return "BlogAdmin";
			}
			else if (relativeUrl.StartsWith("catalogadmin"))
			{
				return "CatalogAdmin";
			}
			else if (relativeUrl.StartsWith("orderadmin"))
			{
				return "OrderAdmin";
			}
			else if (relativeUrl.StartsWith("storeadmin"))
			{
				return "StoreAdmin";
			}
			else if (relativeUrl.StartsWith("systemadmin"))
			{
				return "SystemAdmin";
			}

			return string.Empty;

		}

		public static IEnumerable<SelectListItem> AddToBundleDropdownList(this Product product, StoreFront storeFront)
		{
			return product.NotInBundles(storeFront).Select(b => new SelectListItem() { 
				Value = b.ProductBundleId.ToString(), 
				Text = b.Name + " [" + b.ProductBundleId + "]" }
				);
		}

		public static List<ProductBundle> NotInBundles(this Product product, StoreFront storeFront)
		{
			return storeFront.ProductBundles.Except(product.ProductBundleItems.Select(pbi => pbi.ProductBundle)).AsQueryable().ApplyDefaultSort().ToList();
		}

		public static string DisplayNameWithCount(this ProductCategory category, bool isRegistered)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			if (isRegistered)
			{
				return category.Name + " (" + category.ChildActiveCountForRegistered.ToString("N0") + ")";
			}
			else
			{
				return category.Name + " (" + category.ChildActiveCountForAnonymous.ToString("N0") + ")";
			}
		}
	}
}