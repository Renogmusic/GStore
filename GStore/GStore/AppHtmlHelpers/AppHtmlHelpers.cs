using GStore.Data;
using GStore.Identity;
using GStore.Models;
using GStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GStore.AppHtmlHelpers
{
	public static class AppHtmlHelper
	{
		public static string GStoreVersionNumber(this HtmlHelper htmlHelper)
		{
			System.Reflection.Assembly assembly = typeof(GStore.Models.StoreFront).Assembly;
			return assembly.GetName().Version.ToString();
		}

		public static string GStoreVersionDateString(this HtmlHelper htmlHelper, bool useFileDate = false)
		{
			System.Reflection.Assembly assembly = typeof(GStore.Models.StoreFront).Assembly;
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
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				throw new ApplicationException("Controller not found. Make sure controller inherits from Controllers.BaseClass.BaseController or a descendant");
			}
			if (throwErrorIfNotFound)
			{
				return controller.CurrentStoreFrontOrThrow;
			}
			return controller.CurrentStoreFrontOrNull;
		}

		public static Client CurrentClient(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				throw new ApplicationException("Controller not found. Make sure controller inherits from Controllers.BaseClass.BaseController or a descendant");
			}
			if (throwErrorIfNotFound)
			{
				return controller.CurrentClientOrThrow;
			}
			return controller.CurrentClientOrNull;
		}

		public static UserProfile CurrentUserProfile(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				if (throwErrorIfNotFound)
				{
					throw new ApplicationException("Base Controller not found. Make sure controller inherits from Controllers.BaseClass.PageBaseController or a descendant");
				}
				return null;
			}

			if (throwErrorIfNotFound)
			{
				return controller.CurrentUserProfileOrThrow;
			}
			return controller.CurrentUserProfileOrNull;

		}

		public static StoreBinding GetCurrentStoreBinding(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				if (throwErrorIfNotFound)
				{
					throw new ApplicationException("Base Controller not found. Make sure controller inherits from Controllers.BaseClass.PageBaseController or a descendant");
				}
				return null;
			}

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
			Controllers.BaseClass.PageBaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.PageBaseController;
			if (controller == null)
			{
				if (throwErrorIfNotFound)
				{
					throw new ApplicationException("Page Controller not found. Make sure controller inherits from Controllers.BaseClass.PageBaseController or a descendant");
				}
				return null;
			}
			
			if (throwErrorIfNotFound)
			{
				return controller.CurrentPageOrThrow;
			}
			return controller.CurrentPageOrNull;
		}

		public static Data.IGstoreDb GStoreDb(this HtmlHelper htmlHelper)
		{
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				throw new ApplicationException("Controller not found. Make sure controller inherits from Controllers.BaseClass.BaseController or a descendant");
			}
			return controller.GStoreDb;
		}

		public static string LayoutNameToUse(this HtmlHelper htmlHelper)
		{
			Controllers.BaseClass.PageBaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.PageBaseController;
			if (controller == null)
			{
				return Properties.Settings.Current.AppDefaultLayoutName;
			}

			return controller.LayoutNameToUse;

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
		public static MvcHtmlString ActionSortLinkFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string action)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, new ViewDataDictionary<TModel>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName ?? (metadata.PropertyName ?? expressionText.Split(new char[] { '.' }).Last<string>());

			string[] dotValues = expressionText.Split('.');
			if (dotValues.Count() > 1)
			{
				string firstItem = dotValues[0];
				displayName = htmlHelper.DisplayName(firstItem).ToHtmlString() + " " + displayName;
			}

			return htmlHelper.ActionSortLink(displayName, action, expressionText);
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
		public static MvcHtmlString ActionSortLinkFor<TModel, TValue>(this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression, string action)
		{

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, new ViewDataDictionary<TModel>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName ?? (metadata.PropertyName ?? expressionText.Split(new char[] { '.' }).Last<string>());

			string[] dotValues = expressionText.Split('.');
			if (dotValues.Count() > 1)
			{
				string firstItem = dotValues[0];
				displayName = htmlHelper.DisplayName(firstItem).ToHtmlString() + " " + displayName;
			}

			return htmlHelper.ActionSortLink(displayName, action, expressionText);
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
		public static MvcHtmlString ActionSortLink(this HtmlHelper helper, string linkText, string action, string sortField)
		{

			HttpRequestBase request = helper.ViewContext.HttpContext.Request;

			bool currentFieldIsSorted = false;
			bool linkWillSortUp = true;
			bool? currentSortUp = null;

			if (string.IsNullOrEmpty(request["SortBy"]) || request["SortBy"].ToLower() != sortField.ToLower())
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

			MvcHtmlString returnValue = helper.ActionLink(linkText + (currentFieldIsSorted ? "*" : ""), action, new { SortBy = sortField, SortAscending = linkWillSortUp }, new { @title = title });
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

			List<UserMessage> messages = new List<UserMessage>();
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

			returnValue.AppendLine("<!-- User Messages -->");
			returnValue.AppendLine("<script>");
			foreach (UserMessage userMessage in userMessages)
			{
				returnValue.AppendLine("AddUserMessage(" + userMessage.Title.ToJsValue() + ", " + userMessage.Message.ToJsValue() + ", " + userMessage.MessageType.ToString().ToLower().ToJsValue() + ");");
			}
			returnValue.AppendLine("</script>");

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
			returnValue.AppendLine("</script>");

			return new MvcHtmlString(returnValue.ToString());
		}

		public static MvcHtmlString RenderAnnouncements<TModel>(this HtmlHelper<TModel> htmlHelper)
		{
			StringBuilder returnValue = new StringBuilder();

			List<UserMessage> messages = new List<UserMessage>();
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

			List<UserMessage> messages = new List<UserMessage>();
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
		public static MvcHtmlString GoogleAnalyticsWebPropertyIdValueJs(this HtmlHelper htmlHelper, StoreFront storeFront)
		{
			if (storeFront == null || !storeFront.EnableGoogleAnalytics)
			{
				return new MvcHtmlString("null");
			}

			string value = "'" + htmlHelper.JavaScriptEncode(storeFront.GoogleAnalyticsWebPropertyId, false) + "'";
			return new MvcHtmlString(value);
		}

		public static MvcHtmlString RenderNotificationLink<TModel>(this HtmlHelper<TModel> htmlHelper, Models.NotificationLink link, int counter)
		{
			return new MvcHtmlString(link.NotificationLinkTag(counter));
		}


		public static MvcHtmlString DisplayTextMaxLines<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, int maxLines = 4, int lineLength = 40)
		{
			return htmlHelper.DisplayFor(expression, "DisplayTextMaxLines", new { MaxLines = maxLines, LineLength = lineLength });
		}


		/// <summary>
		/// Simple file dropdown helper, use overload for more options most are optional
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <param name="virtualPathDefault"></param>
		/// <returns></returns>
		public static MvcHtmlString FileHelperFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string virtualPathDefault)
		{
			return FileHelperFor(htmlHelper, expression, null, virtualPathDefault, string.Empty, null);
		}

		public static MvcHtmlString FileHelperFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null, string virtualPathDefault = "", string virtualPathFieldName = "", IQueryable<string> usedFileQuery = null, string fieldNameSuffix = "_FileHelper")
		{

			string virtualPath = virtualPathDefault;
			if (!string.IsNullOrEmpty(virtualPathFieldName))
			{
				MvcHtmlString virtualPathData = htmlHelper.Value(virtualPathFieldName);
				if (!string.IsNullOrEmpty(virtualPathData.ToString()))
				{
					virtualPath = virtualPathData.ToString();
				}
			}

			string fieldName = htmlHelper.MetaModel(expression).PropertyName;
			string currentValue = string.Empty;

			MvcHtmlString fieldValueData = htmlHelper.Value(fieldName);
			if (!string.IsNullOrEmpty(fieldValueData.ToString()))
			{
				currentValue = fieldValueData.ToString();
			}

			string filePath = htmlHelper.ViewContext.HttpContext.Server.MapPath(virtualPath);

			DirectoryInfo dir = new DirectoryInfo(filePath);
			List<FileInfo> fileInfos = new List<FileInfo>();
			if (!dir.Exists)
			{
				htmlHelper.ViewBag.Message = "Path not found: " + virtualPath;
			}
			else
			{
				fileInfos = dir.GetFiles("*.mp3").ToList();
			}

			string[] existingValues = null;
			if (usedFileQuery != null)
			{
				existingValues = usedFileQuery.ToArray();
			}

			Debug.Print(fileInfos.Count() + " existing files in " + HttpUtility.HtmlEncode(virtualPath));
			List<SelectListItem> selectList = new List<SelectListItem>();

			SelectListGroup group1Selected = new SelectListGroup();
			group1Selected.Name = "--Currently Selected File";
			SelectListGroup group2Unused = new SelectListGroup();
			group2Unused.Name = "--Files Available";
			SelectListGroup group3InUse = new SelectListGroup();
			group3InUse.Name = "--Files That are Already in Use";
			SelectListGroup group4Unknown = new SelectListGroup();
			group4Unknown.Name = "--Unknown status";

			foreach (FileInfo file in fileInfos)
			{
				string text = file.Name;
				string value = file.Name;
				bool isSelected = false;
				SelectListGroup group = group4Unknown;
				if (existingValues != null)
				{
					if (existingValues.Contains(value))
					{
						text += " [in use]";
						group = group3InUse;
					}
					else
					{
						text = " [Unused] " + text;
						group = group2Unused;
					}

				}

				if (file.Name.ToLower() == currentValue.ToLower())
				{
					isSelected = true;
					text = "[selected] " + text;
					group = group1Selected;
				}


				selectList.Add(new SelectListItem() { Text = text, Value = value, Selected = isSelected, Group = group });
			}

			selectList = selectList.OrderBy(sl => (sl.Group.Name)).ToList();

			return htmlHelper.DropDownList(fieldName + fieldNameSuffix, selectList, htmlAttributes: htmlAttributes);

		}

		/// <summary>
		/// Returns a set of meta data about a field in the model, use overload for enum models
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ModelMetadata MetaModel<TModel, TValue>(
							 this HtmlHelper<TModel> htmlHelper,
							 Expression<Func<TModel, TValue>> expression)
		{
			return ModelMetadata.FromLambdaExpression<TModel, TValue>(expression, htmlHelper.ViewData);
		}

		/// <summary>
		/// Returns a set of meta data about a field in the model, this is for enum models
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ModelMetadata MetaModel<TModel, TValue>(
							 this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> htmlHelper,
							 Expression<Func<TModel, TValue>> expression)
		{
			var name = ExpressionHelper.GetExpressionText(expression);
			name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(() => Activator.CreateInstance<TModel>(), typeof(TModel), name);
			return metadata;
		}

		public static void SendEmail(Client client, string toEmail, string toName, string subject, string textBody, string htmlBody, string urlHost)
		{
			if (!Properties.Settings.Current.AppEnableEmail)
			{
				return;
			}
			if (!client.UseSendGridEmail)
			{
				return;
			}

			string mailFromEmail = client.SendGridMailFromEmail;
			string mailFromName = client.SendGridMailFromName;

			System.Net.Mail.MailAddress fromAddress = new System.Net.Mail.MailAddress(mailFromEmail, mailFromName);
			System.Net.Mail.MailAddress[] toAddresses = { new System.Net.Mail.MailAddress(toEmail, toName) };

			textBody += "\n\n" + mailFromName
				+ "\n" + mailFromEmail
				+ "\n\nSent from http://" + urlHost;

			htmlBody += "<br/><hr/><br/>" + mailFromName
				+ "<br/>" + mailFromEmail
				+ "<br/><br/>Sent from " + urlHost;

			SendGrid.SendGridMessage myMessage = new SendGrid.SendGridMessage(fromAddress, toAddresses, subject, htmlBody, textBody);

			System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(
				client.SendGridMailAccount,
				client.SendGridMailPassword
					   );

			// Create a Web transport for sending email.
			SendGrid.Web transportWeb = new SendGrid.Web(credentials);
			transportWeb.Deliver(myMessage);
		}

		public static void SendSms(Client client, string toPhoneNumber, string textBody, string urlHost)
		{
			if (!Properties.Settings.Current.AppEnableSMS)
			{
				return;
			}
			if (!client.UseTwilioSms)
			{
				return;
			}


			string smsFromEmail = client.TwilioSmsFromEmail;
			string smsFromName = client.TwilioSmsFromName;

			textBody += "\n\n" + smsFromName
				+ "\n" + smsFromEmail
				+ "\n\nSent from http://" + urlHost;

			Twilio.TwilioRestClient Twilio = new Twilio.TwilioRestClient(
				client.TwilioSid,
				client.TwilioToken
		   );
			var result = Twilio.SendMessage(
				client.TwilioFromPhone,
				toPhoneNumber,
				textBody
				);

			// Status is one of Queued, Sending, Sent, Failed or null if the number is not valid
			Trace.TraceInformation(result.Status);

		}

		public static MvcHtmlString CatalogMenuItemStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels)
		{
			StringBuilder html = new StringBuilder();
			UrlHelper urlHelper = htmlHelper.UrlHelper();
			if (level == 1 && category.HasChildMenuItems(maxLevels))
			{
				//for dropdown categories, make bootstrap dropdown menu for root
				html.AppendLine(Tab(3 + level * 2) + "<li class=\"dropdown CatalogMenu CatalogMenuLevel" + level + "\">"
					+ "\n" + Tab(4 + level * 2) + "<a href=\"#\""
						+ " class=\"dropdown-toggle\" data-toggle=\"dropdown\" title=\"" +
						htmlHelper.AttributeEncode(category.Entity.Name)
						+ "\">"
						+ htmlHelper.Encode(category.Entity.Name)
						+ "<span class=\"caret\"></span>"
					+ "</a>");
			}
			else
			{
				//regular Leaf category no dropdown
				if (level != 1 && category.Entity.UseDividerBeforeOnMenu)
				{
					html.AppendLine(Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n");
				}
				html.AppendLine(Tab(3 + level * 2)
					+ "<li class=\"CatalogMenu CatalogMenuLevel" + level + "\">"
					+ "\n" + Tab(4 + level * 2)
						+ "<a href=\""
							+ urlHelper.Action("ViewCategoryByName", "Catalog", new { urlName = category.Entity.UrlName })
							+ "\""
							+ " title=\"" +
							htmlHelper.AttributeEncode(category.Entity.Name)
						+ "\">"
						+ (level <= 2 ? string.Empty : RepeatString("&nbsp;&nbsp;&nbsp;", (level - 2)))
						+ htmlHelper.Encode(category.Entity.Name)
					+ "</a>");
				if (category.Entity.UseDividerAfterOnMenu)
				{
					html.AppendLine(Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n");
				}
			}
			return new MvcHtmlString(html.ToString());
		}

		public static MvcHtmlString NavBarItemMenuItemStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> navBarItem, int level, int maxLevels)
		{
			StringBuilder html = new StringBuilder();
			UrlHelper urlHelper = htmlHelper.UrlHelper();

			navBarItem.Entity.Url(urlHelper);
			string targetTag = string.Empty;
			if (navBarItem.Entity.OpenInNewWindow)
			{
				targetTag = " target=\"_blank\"";
			}

			if (level == 1 && navBarItem.HasChildMenuItems(maxLevels))
			{
				//for dropdown categories, make bootstrap dropdown menu for root
				html.AppendLine(Tab(3 + level * 2) + "<li class=\"dropdown NavBarItem NavBarItemLevel" + level + "\">"
					+ "\n" + Tab(4 + level * 2) + "<a href=\"#\""
						+ " class=\"dropdown-toggle\" data-toggle=\"dropdown\" title=\"" +
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
					html.AppendLine(Tab(3 + level * 2) + "<li class=\"divider NavBarItem NavBarItemLevel" + level + "\"></li>\n");
				}

				html.AppendLine(Tab(3 + level * 2)
					+ "<li class=\"NavBarItem NavBarItemLevel" + level + "\">"
					+ "\n" + Tab(4 + level * 2)
						+ "<a href=\""
							+ navBarItem.Entity.Url(urlHelper)
							+ "\""
							+ " title=\"" +
							htmlHelper.AttributeEncode(navBarItem.Entity.Name)
						+ targetTag + "\">"
						+ (level <= 2 ? string.Empty : RepeatString("&nbsp;&nbsp;&nbsp;", (level - 2)))
						+ htmlHelper.Encode(navBarItem.Entity.Name)
					+ "</a>");
			}
			return new MvcHtmlString(html.ToString());
		}

		public static MvcHtmlString CatalogMenuItemEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels)
		{
			string html = Tab(3 + level * 2) + "</li>\n";
			if (category.Entity.UseDividerAfterOnMenu && level != 1)
			{
				html += Tab(3 + level * 2) + "<li class=\"divider CatalogMenu CatalogMenuLevel" + level + "\"></li>\n";
			}
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString NavBarItemMenuItemEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> navBarItem, int level, int maxLevels)
		{
			string html = Tab(3 + level * 2) + "</li>\n";
			if (navBarItem.Entity.UseDividerAfterOnMenu && level != 1)
			{
				html += Tab(3 + level * 2) + "<li class=\"divider NavBarItem NavBarItemLevel" + level + "\"></li>\n";
			}
			return new MvcHtmlString(html);
		}

		public static MvcHtmlString CatalogMenuChildContainerStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = Tab(4 + level * 2) + "<ul class=\"dropdown-menu CatalogMenuChildContainer CatalogMenuChildContainerLevel" + level + "\" role=\"menu\">\n";
			}
			else
			{
				html += Tab(4 + level * 2) + "<li class=\"CatalogMenu CatalogMenuLevel" + level + "\">\n";
			}

			return new MvcHtmlString(html);
		}

		public static MvcHtmlString NavBarItemChildContainerStart<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> NavBarItem, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = Tab(4 + level * 2) + "<ul class=\"dropdown-menu NavBarItemChildContainer NavBarItemChildContainerLevel" + level + "\" role=\"menu\">\n";
			}
			else
			{
				html += Tab(4 + level * 2) + "<li class=\"NavBarItem NavBarItemLevel" + level + "\">\n";
			}

			return new MvcHtmlString(html);
		}

		public static MvcHtmlString CatalogMenuChildContainerEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<ProductCategory> category, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = Tab(4 + level * 2) + "</ul>\n";
			}
			else
			{
				html = Tab(4 + level * 2) + "</li>\n";
			}

			return new MvcHtmlString(html);
		}

		public static MvcHtmlString NavBarItemChildContainerEnd<TModel>(this HtmlHelper<TModel> htmlHelper, TreeNode<NavBarItem> NavBarItem, int level, int maxLevels)
		{
			string html = string.Empty;
			if (level == 1)
			{
				html = Tab(4 + level * 2) + "</ul>\n";
			}
			else
			{
				html = Tab(4 + level * 2) + "</li>\n";
			}

			return new MvcHtmlString(html);
		}

		public static string Url(this NavBarItem navBarItem, UrlHelper urlHelper)
		{
			if (navBarItem.IsAction)
			{
				if (!string.IsNullOrEmpty(navBarItem.Area))
				{
					return urlHelper.Action(navBarItem.Action, navBarItem.Controller, new { Area = navBarItem.Area });
				}
				return urlHelper.Action(navBarItem.Action, navBarItem.Controller);
			}
			else if (navBarItem.IsPage)
			{
				string url = navBarItem.Page.Url.Trim('~').Trim('/');
				string currentRawUrl = urlHelper.RequestContext.HttpContext.Request.RawUrl.Trim('/').ToLower();
				string appRoot = urlHelper.RequestContext.HttpContext.Request.ApplicationPath.Trim('/').ToLower();
				if (appRoot.Length != 0 && currentRawUrl.StartsWith(appRoot))
				{
					url = appRoot + "/" + url;
				}
				string urlStoreName = urlHelper.RequestContext.RouteData.UrlStoreName();
				if (!string.IsNullOrEmpty(urlStoreName))
				{
					url = "Stores/" + urlStoreName + "/" + url;
				}

				return "/" + url;
			}
			else if (navBarItem.IsLocalHRef)
			{
				return urlHelper.GStoreLocalUrl(navBarItem.LocalHRef);
			}
			else if (navBarItem.IsRemoteHRef)
			{
				if (navBarItem.RemoteHRef.ToLower().StartsWith("http://") || navBarItem.RemoteHRef.ToLower().StartsWith("mailto:"))
				{
					return navBarItem.RemoteHRef;
				}
				return "http://" + navBarItem.RemoteHRef;
			}
			throw new ApplicationException("Unknown navBarItem type. NavBarItemId: " + navBarItem.NavBarItemId);
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

		public static string Tab(int tabs)
		{
			return new string('\t', tabs);
		}

		public static string RepeatString(string value, int count)
		{
			return string.Concat(Enumerable.Repeat(value, count));
		}

		/// <summary>
		/// Renders hidden fields for CreatedBy_UserProfileId, UpdatedBy_UserProfileId, CreateDateTimeUtc, UpdateDateTimeUtc
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString HiddenAuditFields<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel : Models.BaseClasses.AuditFieldsAllRequired
		{
			if (!(htmlHelper.ViewContext.Controller is Controllers.BaseClass.BaseController))
			{
				throw new ApplicationException("htmlHelper.ViewContext.Controller must inherit from base controller for this method: HiddenAuditFields<AuditFieldsAllRequired>");
			}
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

		public static UrlHelper UrlHelper(this HtmlHelper htmlHelper)
		{
			return new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
		}

		public static string ToJsValue(this string value, bool addDoubleQuotes = true)
		{
			return HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes);
		}

		public static MvcHtmlString JavaScriptEncode(this HtmlHelper htmlHelper, string value, bool addDoubleQuotes = false)
		{
			return new MvcHtmlString(HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes));
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
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string GStoreLocalUrl(this UrlHelper urlHelper, string localUrl)
		{
			string url = localUrl.Trim('~').Trim('/');
			string currentRawUrl = urlHelper.RequestContext.HttpContext.Request.RawUrl.Trim('/').ToLower();
			string appRoot = urlHelper.RequestContext.HttpContext.Request.ApplicationPath.Trim('/').ToLower();
			if (appRoot.Length != 0 && currentRawUrl.StartsWith(appRoot))
			{
				url = appRoot + "/" + url;
			}
			string urlStoreName = urlHelper.RequestContext.RouteData.UrlStoreName();
			if (!string.IsNullOrEmpty(urlStoreName))
			{
				url = "Stores/" + urlStoreName + "/" + url;
			}

			return "/" + url;
		}

		/// <summary>
		/// Returns a string representing the number of bytes, kb, mb, gb, tb
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToByteString(this long byteCount)
		{
			long value = byteCount;
			if (value < 1024)
			{
				return value + " B";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value + " KB";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value + " MB";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value + " GB";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value + " TB";
			}
			return value + " (over limit)";

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
		public static MvcHtmlString DisplayPageSection(this HtmlHelper<PageViewModel> htmlHelper, string sectionName, int index)
		{
			PageViewModel pageViewModel = htmlHelper.ViewData.Model;
			if (pageViewModel.Page == null)
			{
				throw new ApplicationException("Page cannot be null");
			}
			Page page = pageViewModel.Page;
			PageTemplate pageTemplate = page.PageTemplate;

			PageTemplateSection pageTemplateSection = pageTemplate.Sections.Where(pts => pts.Name.ToLower() == sectionName.ToLower()).SingleOrDefault();

			if (pageTemplateSection == null)
			{
				System.Diagnostics.Trace.WriteLine("--Auto-creating page template section. Template: " + pageTemplate.Name + " [" + pageTemplate.PageTemplateId + "] Section Name: " + sectionName);
				IGstoreDb db = htmlHelper.GStoreDb();
				pageTemplateSection = db.CreatePageTemplateSection(pageTemplate.PageTemplateId, sectionName, 1000 + index, false, sectionName + " section", db.SeedAutoMapUserBestGuess());
			}

			PageSection pageSection = page.Sections.AsQueryable().WhereIsActive()
				.Where(ps => ps.PageTemplateSectionId == pageTemplateSection.PageTemplateSectionId)
				.OrderBy(ps => ps.Order).ThenBy(ps => ps.PageSectionId)
				.FirstOrDefault();

			if (!pageViewModel.EditMode)
			{
				return pageTemplateSection.HtmlDisplay(pageSection, htmlHelper);
			}
			return pageTemplateSection.Editor(page, pageSection, index, htmlHelper);

		}

		/// <summary>
		/// Returns HTML value of the section section
		/// </summary>
		/// <param name="pageSection"></param>
		/// <returns></returns>
		public static MvcHtmlString HtmlDisplay<TModel>(this PageTemplateSection pageTemplateSection, PageSection pageSection, HtmlHelper<TModel> htmlHelper)
		{
			string value = string.Empty;
			if (pageSection != null)
			{
				if (pageSection.HasNothing)
				{
					value = string.Empty;
				}
				else if (pageSection.HasRawHtml && !string.IsNullOrEmpty(pageSection.RawHtml))
				{
					value = pageSection.RawHtml;
				}
				else if (pageSection.HasPlainText && !string.IsNullOrEmpty(pageSection.PlainText))
				{
					value = HttpUtility.HtmlEncode(pageSection.PlainText).Replace("\n", "<br/>\n");
				}
			}
			return new MvcHtmlString(value);
		}

		public static MvcHtmlString Editor<TModel>(this PageTemplateSection pageTemplateSection, Page page, PageSection pageSection, int index, HtmlHelper<TModel> htmlHelper) where TModel : PageViewModel
		{

			string value = string.Empty;
			StringBuilder html = new StringBuilder();
			if (pageSection != null)
			{
				if (pageSection.HasRawHtml && !string.IsNullOrEmpty(pageSection.RawHtml))
				{
					value = pageSection.RawHtml;
				}
				else if (pageSection.HasPlainText && !string.IsNullOrEmpty(pageSection.PlainText))
				{
					value = HttpUtility.HtmlEncode(pageSection.PlainText).Replace("\n", "<br/>\n");
				}
			}

			Models.ViewModels.PageSectionEditViewModel viewModel = new Models.ViewModels.PageSectionEditViewModel(pageTemplateSection, page, pageSection, index);

			return htmlHelper.EditorFor(model => viewModel);

		}

		public static MenuViewModel MenuViewModel(this HtmlHelper htmlHelper, StoreFront storeFront, UserProfile userProfile)
		{
			return new MenuViewModel(storeFront, userProfile);
		}

	}
}