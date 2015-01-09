using GStore.Data;
using GStore.Identity;
using GStore.Models;
using GStore.Models.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

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

		public static StoreFrontConfiguration CurrentStoreFrontConfig(this HtmlHelper htmlHelper, bool throwErrorIfNotFound)
		{
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				throw new ApplicationException("Controller not found. Make sure controller inherits from Controllers.BaseClass.BaseController or a descendant");
			}
			if (throwErrorIfNotFound)
			{
				return controller.CurrentStoreFrontConfigOrThrow;
			}
			return controller.CurrentStoreFrontConfigOrNull;
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
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				return Properties.Settings.Current.AppDefaultLayoutName;
			}

			return controller.LayoutNameToUse;

		}

		/// <summary>
		/// Returns the current theme folder for the controller, or the system default if error
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static string ThemeFolderName(this HtmlHelper htmlHelper)
		{
			Controllers.BaseClass.BaseController controller = htmlHelper.ViewContext.Controller as Controllers.BaseClass.BaseController;
			if (controller == null)
			{
				return Properties.Settings.Current.AppDefaultThemeFolderName;
			}

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
		public static MvcHtmlString ActionSortLinkFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string action, bool useShortName = true)
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

			return htmlHelper.ActionSortLink(displayName, action, expressionText);
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
		public static MvcHtmlString ActionSortLinkForItem<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string action, bool removeItemName, bool useShortName = true, string TabName = "", int? id = null)
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

			return htmlHelper.ActionSortLink(displayName, action, expressionText, TabName, id);
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
		public static MvcHtmlString ActionSortLinkFor<TModel, TValue>(this HtmlHelper<System.Collections.Generic.IEnumerable<TModel>> htmlHelper, Expression<Func<TModel, TValue>> expression, string action, bool useShortName = true)
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
		public static MvcHtmlString ActionSortLink(this HtmlHelper helper, string linkText, string action, string sortField, string TabName = "", int? id = null)
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

			MvcHtmlString returnValue = null;
			if (id.HasValue)
			{
				if (!string.IsNullOrWhiteSpace(TabName))
				{
					returnValue = helper.ActionLink(linkText + (currentFieldIsSorted ? "*" : ""), action, new { id= id.Value, SortBy = sortField, SortAscending = linkWillSortUp, Tab = TabName }, new { @title = title });
				}
				else
				{
					returnValue = helper.ActionLink(linkText + (currentFieldIsSorted ? "*" : ""), action, new { id = id.Value, SortBy = sortField, SortAscending = linkWillSortUp }, new { @title = title });
				}
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(TabName))
				{
					returnValue = helper.ActionLink(linkText + (currentFieldIsSorted ? "*" : ""), action, new { SortBy = sortField, SortAscending = linkWillSortUp, Tab = TabName }, new { @title = title });
				}
				else
				{
					returnValue = helper.ActionLink(linkText + (currentFieldIsSorted ? "*" : ""), action, new { SortBy = sortField, SortAscending = linkWillSortUp }, new { @title = title });
				}
			}

			if (!string.IsNullOrEmpty(sortClue))
			{
				returnValue = new MvcHtmlString(returnValue.ToHtmlString() + helper.Encode(sortClue));
			}

			return returnValue;
		}

		public static MvcHtmlString DisplayShortNameFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string name = metadata.ShortDisplayName ?? metadata.DisplayName ?? metadata.PropertyName;
			return new MvcHtmlString(htmlHelper.Encode(name));
		}

		public static MvcHtmlString DisplayShortName(this HtmlHelper htmlHelper, string expression)
		{
			ModelMetadata metadata = ModelMetadata.FromStringExpression(expression, htmlHelper.ViewData);
			string name = metadata.ShortDisplayName ?? metadata.DisplayName ?? metadata.PropertyName;
			return new MvcHtmlString(htmlHelper.Encode(name));
		}

		public static MvcHtmlString ShortLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			return ShortLabelHelper(htmlHelper, metadata, htmlFieldName, null, null);
		}

		public static MvcHtmlString ShortLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			IDictionary<string, object> typedHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return ShortLabelHelper(htmlHelper, metadata, htmlFieldName, null, typedHtmlAttributes);
		}

		public static MvcHtmlString ShortLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			IDictionary<string, object> typedHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return ShortLabelHelper(htmlHelper, metadata, htmlFieldName, labelText, typedHtmlAttributes);
		}

		internal static MvcHtmlString ShortLabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText = null, IDictionary<string, object> htmlAttributes = null)
        {
            string resolvedLabelText = labelText ?? metadata.ShortDisplayName ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            if (String.IsNullOrEmpty(resolvedLabelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
            tag.SetInnerText(resolvedLabelText);
            tag.MergeAttributes(htmlAttributes, replaceExisting: true);
            return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
        }

		/// <summary>
		/// Renders a label with help text from the "Description" data attribute if found using classes help-label and help-label-top
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString HelpLabelTopFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			return HelpLabelHelper(htmlHelper, metadata, htmlFieldName, null, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class = "help-label help-label-top" }));
		}

		/// <summary>
		/// Renders a label with help text from the "Description" data attribute if found
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString HelpLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			return HelpLabelHelper(htmlHelper, metadata, htmlFieldName, null, HtmlHelper.AnonymousObjectToHtmlAttributes(new { @class="help-label" }));
		}

		/// <summary>
		/// Renders a label with help text from the "Description" data attribute if found
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString HelpLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			IDictionary<string, object> typedHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return HelpLabelHelper(htmlHelper, metadata, htmlFieldName, null, typedHtmlAttributes);
		}

		/// <summary>
		/// Renders a label with help text from the "Description" data attribute if found
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString HelpLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string labelText, object htmlAttributes)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
			IDictionary<string, object> typedHtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			return HelpLabelHelper(htmlHelper, metadata, htmlFieldName, labelText, typedHtmlAttributes);
		}

		internal static MvcHtmlString HelpLabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText = null, IDictionary<string, object> htmlAttributes = null)
		{
			string resolvedLabelText = labelText ?? metadata.Description;
			if (string.IsNullOrEmpty(resolvedLabelText))
			{
				return MvcHtmlString.Empty;
			}

			TagBuilder tag = new TagBuilder("label");
			tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
			tag.InnerHtml = resolvedLabelText.ToHtmlLines();
			tag.MergeAttributes(htmlAttributes, replaceExisting: true);
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		/// <summary>
		/// Displays a help label (Description attribute) in a pop-over triggered by focus anchored to the body tag with help label inside and displayname as the title
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="label"></param>
		/// <param name="topLineText"></param>
		/// <param name="bottomLineText"></param>
		/// <returns></returns>
		public static MvcHtmlString HelpLabelPopoverForModel(this HtmlHelper htmlHelper, string label = "?", string topLineText = "", string bottomLineText = "")
		{
			ModelMetadata metadata = htmlHelper.ViewData.ModelMetadata;
			if (string.IsNullOrEmpty(metadata.Description))
			{
				return MvcHtmlString.Empty;
			}

			string propertyName = (string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName);

			string message = (string.IsNullOrEmpty(topLineText) ? "" : topLineText + "\n")
				+ (string.IsNullOrEmpty(metadata.Description) ? "" : metadata.Description + "\n")
				+ (string.IsNullOrEmpty(bottomLineText) ? "" : bottomLineText + "\n");

			string html = "<a href=\"javascript://\" tabindex=\"100\" class=\"help-label-popup\" role=\"button\" data-toggle=\"popover\" data-html=\"true\" data-trigger=\"focus\" "
				+ "data-container=\"body\" onclick=\"return false;\""
				+ "title=\"" + propertyName.ToHtmlAttribute() + "\" "
				+ "data-content=\"" + message.ToHtmlLines().ToHtmlAttribute() + "\">" + label.ToHtmlLines() + "</a>";
			return new MvcHtmlString(html);
		}

		/// <summary>
		/// Displays a help label (Description attribute) in a pop-over triggered by focus anchored to the body tag with help label inside and displayname as the title
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <param name="label"></param>
		/// <param name="topLineText"></param>
		/// <param name="bottomLineText"></param>
		/// <returns></returns>
		public static MvcHtmlString HelpLabelPopoverFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string label = "?", string topLineText = "", string bottomLineText = "")
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			if (string.IsNullOrEmpty(metadata.Description))
			{
				return MvcHtmlString.Empty;
			}

			string propertyName = (string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName);

			string message = (string.IsNullOrEmpty(topLineText) ? "" : topLineText + "\n")
				+ (string.IsNullOrEmpty(metadata.Description) ? "" : metadata.Description + "\n")
				+ (string.IsNullOrEmpty(bottomLineText) ? "" : bottomLineText + "\n");

			string html = "<a href=\"javascript://\" tabindex=\"0\" class=\"help-label-popup\" role=\"button\" data-toggle=\"popover\" data-html=\"true\" data-trigger=\"focus\" " 
				+ "data-container=\"body\" onclick=\"return false;\""
				+ "title=\"" + propertyName.ToHtmlAttribute() + "\" " 
				+ "data-content=\"" + message.ToHtmlLines().ToHtmlAttribute() + "\">" + label.ToHtmlLines() +"</a>";
			return new MvcHtmlString(html);
		}

		/// <summary>
		/// Returns a display description (help text) for a model property, empty string if no description
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString DisplayDescriptionFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

			if (string.IsNullOrEmpty(metadata.Description))
			{
				return new MvcHtmlString(string.Empty);
			}
			return new MvcHtmlString(metadata.Description);
		}

		public static MvcHtmlString ValidationMessageForModel(this HtmlHelper htmlHelper, bool addTextDangerClass = true, bool removeHtmlFieldPrefix = true)
		{
			return htmlHelper.ValidationMessageForModel(null, addTextDangerClass);
		}

		public static MvcHtmlString ValidationMessageForModel(this HtmlHelper htmlHelper, object htmlAttributes, bool addTextDangerClass = true, bool removeHtmlFieldPrefix = true)
		{
			ModelMetadata metadata = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metadata.PropertyName;

			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			if (addTextDangerClass)
			{
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "text-danger " + htmlAttribs["class"];
				}
				else
				{
					htmlAttribs.Add("class", "text-danger");
				}
			}

			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			return htmlHelper.ValidationMessage(fieldName, htmlAttribs);
		}

		/// <summary>
		/// Returns a dropdownlist with values from an enum using the current model
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="optionLabel"></param>
		/// <param name="htmlAttributes"></param>
		/// <returns></returns>
		public static MvcHtmlString EnumDropDownListForModel(this HtmlHelper<Enum> htmlHelper, string optionLabel = null, bool addFormControlClass = true, bool removeHtmlFieldPrefix = true)
		{
			return htmlHelper.EnumDropDownListForModel(optionLabel, null,  addFormControlClass, removeHtmlFieldPrefix);
		}

		/// <summary>
		/// Returns a dropdownlist with values from an enum using the current model
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="optionLabel"></param>
		/// <param name="htmlAttributes"></param>
		/// <returns></returns>
		public static MvcHtmlString EnumDropDownListForModel(this HtmlHelper<Enum> htmlHelper, string optionLabel, object htmlAttributes, bool addFormControlClass = true, bool removeHtmlFieldPrefix = true)
		{
			ModelMetadata metadata = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metadata.PropertyName;
			string displayName = (string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName);

			EnumInfo enumInfo = htmlHelper.EnumIntInfoListForModel<Enum>();
			if (!enumInfo.HasValue || enumInfo.Values.Count == 0)
			{
				return null;
			}
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			if (addFormControlClass)
			{
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "form-control " + htmlAttribs["class"];
				}
				else
				{
					htmlAttribs.Add("class", "form-control");
				}
			}
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}

			List<EnumInfoValue> orderedEnumValues = enumInfo.Values.OrderBy(v => v.Order).ThenBy(v => v.Name).ToList();
			IEnumerable<SelectListItem> options = orderedEnumValues.Select(v => new SelectListItem() { Value = v.Value.ToString(), Text = v.DisplayName, Selected = v.Value == htmlHelper.ViewData.Model });
			return htmlHelper.DropDownList(fieldName, options, optionLabel, htmlAttribs);
		}

		/// <summary>
		/// Returns a radio button list with values from an enum using the current model
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="optionLabel"></param>
		/// <param name="htmlAttributes"></param>
		/// <returns></returns>
		public static MvcHtmlString EnumRadioButtonListForModel(this HtmlHelper<Enum> htmlHelper, string htmlSeparator = "<br/>", bool selectFirst = true, bool addFormControlClass = false, bool removeHtmlFieldPrefix = true)
		{
			return htmlHelper.EnumRadioButtonListForModel(null, htmlSeparator, selectFirst, addFormControlClass, removeHtmlFieldPrefix);
		}

		/// <summary>
		/// Returns a radio button list with values from an enum using the current model
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="optionLabel"></param>
		/// <param name="htmlAttributes"></param>
		/// <returns></returns>
		public static MvcHtmlString EnumRadioButtonListForModel(this HtmlHelper<Enum> htmlHelper, object htmlAttributes, string htmlSeparator = "<br/>", bool selectFirst = true, bool addFormControlClass = false, bool removeHtmlFieldPrefix = true)
		{
			ModelMetadata metadata = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metadata.PropertyName;
			string displayName = (string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName);

			EnumInfo enumInfo = htmlHelper.EnumIntInfoListForModel<Enum>();
			if (!enumInfo.HasValue || enumInfo.Values.Count == 0)
			{
				return null;
			}
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			if (addFormControlClass)
			{
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "form-control " + htmlAttribs["class"];
				}
				else
				{
					htmlAttribs.Add("class", "form-control");
				}
			}
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			List<EnumInfoValue> orderedEnumValues = enumInfo.Values.OrderBy(v => v.Order).ThenBy(v => v.Name).ToList();
			List<SelectListItem> options = orderedEnumValues.Select(v => new SelectListItem() { Value = v.Value.ToString(), Text = v.DisplayName }).ToList();

			if (htmlHelper.ViewData.Model == null)
			{
				if (selectFirst)
				{
					//null model select first item
					options.First().Selected = true;
				}
			}
			else
			{
				SelectListItem firstMatch = options.FirstOrDefault(o => o.Value == htmlHelper.ViewData.Model.ToString());
				if (firstMatch == null)
				{
					if (selectFirst)
					{
						//no match select first item
						options.First().Selected = true;
					}
				}
				else
				{
					firstMatch.Selected = true;
				}
			}

			StringBuilder html = new StringBuilder();
			
			foreach (SelectListItem option in options)
			{
				string id = fieldName + "_" + option.Value;
				htmlAttribs["id"] = id;
				html.AppendLine(htmlHelper.RadioButton(fieldName, option.Value, option.Selected, htmlAttribs).ToHtmlString());
				html.AppendLine("<label for=\"" + id + "\">" + option.Text.ToHtml() + "</label>");
				html.AppendLine(htmlSeparator);
			}
			return new MvcHtmlString(html.ToString());
		}

		/// <summary>
		/// Returns a watermark for the current field. Gets data from Prompt display attribute
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static string WatermarkFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

			if (string.IsNullOrEmpty(metadata.Watermark))
			{
				return metadata.DisplayName ?? metadata.PropertyName;
			}
			return metadata.Watermark;
		}

		/// <summary>
		/// Returns a watermark for the current field. Gets data from Prompt display attribute
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static string WatermarkForModel(this HtmlHelper htmlHelper)
		{
			ModelMetadata metadata = htmlHelper.ViewData.ModelMetadata;
			if (string.IsNullOrEmpty(metadata.Watermark))
			{
				return metadata.DisplayName ?? metadata.PropertyName;
			}
			return metadata.Watermark;
		}

		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxWithWatermarkFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
		{
			return htmlHelper.TextBoxWithWatermarkFor(expression, null);
		}

		public static MvcHtmlString LabelWithRequiredForModel(this HtmlHelper htmlHelper)
		{
			return htmlHelper.LabelWithRequiredForModel(null);
		}

		public static MvcHtmlString LabelWithRequiredForModel(this HtmlHelper htmlHelper, object htmlAttributes)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string label = metaData.DisplayName ?? metaData.PropertyName;
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			if (metaData.IsRequired)
			{
				label += " *";
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "text-required " + htmlAttribs["class"];
				}
				else
				{
					htmlAttribs["class"] = "text-required";
				}
			}
			return htmlHelper.LabelForModel(label, htmlAttribs);
		}

		public static MvcHtmlString RequiredForModel(this HtmlHelper htmlHelper)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			if (metaData.IsRequired)
			{
				return new MvcHtmlString("<span class=\"text-required\">(*)</span>");
			}
			return null;
		}

		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <param name="htmlAttributes"></param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxWithWatermarkFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
		{
			string watermark = htmlHelper.WatermarkFor(expression);
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			htmlAttribs.Add("placeholder", watermark);
			return htmlHelper.TextBoxFor(expression, htmlAttribs);
		}


		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute, adds class for form-control unless doNotAddFormControl is true
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxWithWatermarkForModel(this HtmlHelper htmlHelper, bool setSizeToMaxLength = true, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false)
		{
			return htmlHelper.TextBoxWithWatermarkForModel(null, setSizeToMaxLength, removeHtmlFieldPrefix, doNotAddFormControlClass);
		}

		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute, adds class for form-control unless doNotAddFormControl is true
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxWithWatermarkForModel(this HtmlHelper htmlHelper, object htmlAttributes, bool setSizeToMaxLength = true, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName;
			string displayName = (string.IsNullOrEmpty(metaData.DisplayName) ? metaData.PropertyName : metaData.DisplayName);

			string watermark = metaData.Watermark ?? ("Enter " + (metaData.DisplayName ?? metaData.PropertyName));
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			
			htmlAttribs.Add("placeholder", watermark);

			if (!doNotAddFormControlClass)
			{
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "form-control " + htmlAttribs["class"].ToString();
				}
				else
				{
					htmlAttribs.Add("class", "form-control");
				}
			}

			int? maxLength = htmlHelper.MaxLengthAttributeForModel();
			if (maxLength.HasValue)
			{
				htmlAttribs["maxlength"] = maxLength.Value;
				if (setSizeToMaxLength)
				{
					htmlAttribs["size"] = maxLength.Value;
				}
			}

			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			
			return htmlHelper.TextBox(fieldName, htmlHelper.ViewData.Model, null, htmlAttribs);
		}

		public static MvcHtmlString TextAreaWithWatermarkForModel(this HtmlHelper htmlHelper, int cols = 40, int rows = 10, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false)
		{
			return htmlHelper.TextAreaWithWatermarkForModel(null, cols, rows, removeHtmlFieldPrefix, doNotAddFormControlClass);
		}

		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute, adds class for form-control unless doNotAddFormControl is true
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString TextAreaWithWatermarkForModel(this HtmlHelper htmlHelper, object htmlAttributes, int cols = 40, int rows = 10, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName;
			string displayName = (string.IsNullOrEmpty(metaData.DisplayName) ? metaData.PropertyName : metaData.DisplayName);

			string watermark = metaData.Watermark ?? ("Enter " + (metaData.DisplayName ?? metaData.PropertyName));
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			htmlAttribs.Add("placeholder", watermark);

			if (!doNotAddFormControlClass)
			{
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "form-control " + htmlAttribs["class"].ToString();
				}
				else
				{
					htmlAttribs.Add("class", "form-control");
				}
			}

			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}

			return htmlHelper.TextArea(fieldName, htmlHelper.ViewData.Model as string, rows, cols, htmlAttribs);
		}

		public static MvcHtmlString FieldNameForModel(this HtmlHelper htmlHelper)
		{
			return new MvcHtmlString(htmlHelper.ViewData.ModelMetadata.PropertyName);
		}

		public static MvcHtmlString DropDownListForModel(this HtmlHelper htmlHelper, IEnumerable<SelectListItem> selectList, string optionLabel = null, bool allowNull = false, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool addIdToPropertyName = true)
		{
			return htmlHelper.DropDownListForModel(selectList, null, optionLabel, allowNull, removeHtmlFieldPrefix, doNotAddFormControlClass, addIdToPropertyName);
		}

		public static MvcHtmlString DropDownListForModel(this HtmlHelper htmlHelper, IEnumerable<SelectListItem> selectList,  object htmlAttributes, string optionLabel = null, bool allowNull = false, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool addIdToPropertyName = true)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName;
			if (addIdToPropertyName)
			{
				fieldName += "Id";
			}
			
			string displayName = (string.IsNullOrEmpty(metaData.DisplayName) ? metaData.PropertyName : metaData.DisplayName);

			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (!doNotAddFormControlClass)
			{
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "form-control " + htmlAttribs["class"].ToString();
				}
				else
				{
					htmlAttribs.Add("class", "form-control");
				}
			}

			if (allowNull)
			{
				htmlAttribs.Add("data-val", "false");
			}

			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}

			if (selectList == null)
			{
				selectList = new List<SelectListItem>();
			}
			return htmlHelper.DropDownList(fieldName, selectList, optionLabel, htmlAttribs);
		}


		public static MvcHtmlString CheckboxForModel(this HtmlHelper htmlHelper, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false)
		{
			return htmlHelper.CheckboxForModel(null, removeHtmlFieldPrefix, doNotAddFormControlClass);
		}

		public static MvcHtmlString CheckboxForModel(this HtmlHelper htmlHelper, object htmlAttributes, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName;
			string displayName = (string.IsNullOrEmpty(metaData.DisplayName) ? metaData.PropertyName : metaData.DisplayName);

			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (!doNotAddFormControlClass)
			{
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "form-control " + htmlAttribs["class"].ToString();
				}
				else
				{
					htmlAttribs.Add("class", "form-control");
				}
			}

			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}

			return htmlHelper.CheckBox(fieldName, (htmlHelper.ViewData.Model as bool?) ?? false, htmlAttribs);
		}


		public static int? MaxLengthAttributeForModel(this HtmlHelper htmlHelper)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string propertyName = metaData.PropertyName;
			Type containerType = metaData.ContainerType;

			MaxLengthAttribute[] maxLengthAttribs = containerType.GetProperty(propertyName).GetCustomAttributes<MaxLengthAttribute>(false).ToArray();
			if (maxLengthAttribs == null || maxLengthAttribs.Length == 0)
			{
				return null;
			}
			return maxLengthAttribs[0].Length;
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
		/// Returns a set of meta data about a field in the model, use overload for ienumerable models
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
		/// Returns a set of meta data about a field in the model, this is for ienumerable models
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

		/// <summary>
		/// Sends an email and adds client signature to the end of it
		/// </summary>
		/// <param name="client"></param>
		/// <param name="toEmail"></param>
		/// <param name="toName"></param>
		/// <param name="subject"></param>
		/// <param name="textBody"></param>
		/// <param name="htmlBody"></param>
		/// <param name="urlHost"></param>
		public static bool SendEmail(Client client, string toEmail, string toName, string subject, string textBody, string htmlBody, string urlHost)
		{
			if (!Properties.Settings.Current.AppEnableEmail)
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

			return true;
		}

		/// <summary>
		/// Sends a SMS text message and adds client signature to the end
		/// </summary>
		/// <param name="client"></param>
		/// <param name="toPhoneNumber"></param>
		/// <param name="textBody"></param>
		/// <param name="urlHost"></param>
		public static bool SendSms(Client client, string toPhoneNumber, string textBody, string urlHost)
		{
			if (!Properties.Settings.Current.AppEnableSMS)
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
			return true;

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

		public static string UrlResolved(this Page page, UrlHelper urlHelper)
		{
			string url = page.Url.Trim('~').Trim('/');
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

		public static string UrlResolved(this PageEditViewModel pageEditViewModel, UrlHelper urlHelper)
		{
			string url = pageEditViewModel.Url.Trim('~').Trim('/');
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

		/// <summary>
		/// Renders hidden fields for CreatedBy_UserProfileId, UpdatedBy_UserProfileId, CreateDateTimeUtc, UpdateDateTimeUtc
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString HiddenAuditFieldsOptional<TModel>(this HtmlHelper<TModel> htmlHelper) where TModel : Models.BaseClasses.AuditFieldsUserProfileOptional
		{
			if (!(htmlHelper.ViewContext.Controller is Controllers.BaseClass.BaseController))
			{
				throw new ApplicationException("htmlHelper.ViewContext.Controller must inherit from base controller for this method: HiddenAuditFields<AuditFieldsAllRequired>");
			}
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
		public static MvcHtmlString DisplayPageSection(this HtmlHelper<PageViewModel> htmlHelper, string sectionName, int index, string description, string defaultRawHtmlValue, bool editInTop, bool editInBottom)
		{
			if (index < 1)
			{
				throw new ArgumentOutOfRangeException("Index", "Template Error! Page Template Section Index must be 1 or greater");
			}
			if (string.IsNullOrWhiteSpace(sectionName))
			{
				throw new ArgumentOutOfRangeException("Index", "Template Error! Page Section Name cannot be blank");
			}
			if (string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentOutOfRangeException("Index", "Template Error! Section Description cannot be blank");
			}

			PageViewModel pageViewModel = htmlHelper.ViewData.Model;
			if (pageViewModel.ForTemplateSyncOnly)
			{
				if (!pageViewModel.PageTemplateIdForSync.HasValue)
				{
					throw new ArgumentNullException("PageTemplateIdForSync", "PageTemplateIdForSync must be specified when ForTemplateSyncOnly is true");
				}
				int pageTemplateId = pageViewModel.PageTemplateIdForSync.Value;
				IGstoreDb dbforSync = htmlHelper.GStoreDb();
				PageTemplate template = dbforSync.PageTemplates.SingleOrDefault(pt => pt.PageTemplateId == pageTemplateId);
				if (template == null)
				{
					throw new ApplicationException("Page Template not found by id: " + pageTemplateId);
				}
				PageTemplateSection sectionTest = template.Sections.Where(pts => pts.Name.ToLower() == sectionName.ToLower()).SingleOrDefault();
				if (sectionTest == null)
				{
					sectionTest = dbforSync.CreatePageTemplateSection(pageTemplateId, sectionName, 1000 + index, description, defaultRawHtmlValue, template.ClientId, editInTop, editInBottom, htmlHelper.CurrentUserProfile(true));
					return new MvcHtmlString("<span class=\"text-info\"><strong>New Section '" + htmlHelper.Encode(sectionTest.Name) + "' [" + sectionTest.PageTemplateSectionId + "] Created</strong></span><br/>");
				}
				bool update = false;
				if (!string.IsNullOrEmpty(defaultRawHtmlValue) && (sectionTest.DefaultRawHtmlValue != defaultRawHtmlValue))
				{
					sectionTest.DefaultRawHtmlValue = defaultRawHtmlValue;
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
				if (update)
				{
					dbforSync.PageTemplateSections.Update(sectionTest);
					dbforSync.SaveChanges();
					return new MvcHtmlString("<span class=\"text-info\"><strong>Section Updated '" + htmlHelper.Encode(sectionTest.Name) + "' [" + sectionTest.PageTemplateSectionId + "]</strong></span><br/>");
				}
			}

			if (pageViewModel.Page == null)
			{
				throw new ArgumentNullException("Page", "Page cannot be null except in ForTemplateSyncOnly which is false");
			}
			Page page = pageViewModel.Page;
			PageTemplate pageTemplate = page.PageTemplate;

			PageTemplateSection pageTemplateSection = pageTemplate.Sections.Where(pts => pts.Name.ToLower() == sectionName.ToLower()).SingleOrDefault();

			if (pageTemplateSection == null)
			{
				System.Diagnostics.Trace.WriteLine("--Auto-creating page template section. Template: " + pageTemplate.Name + " [" + pageTemplate.PageTemplateId + "] Section Name: " + sectionName);
				IGstoreDb db = htmlHelper.GStoreDb();
				pageTemplateSection = db.CreatePageTemplateSection(pageTemplate.PageTemplateId, sectionName, 1000 + index, description, defaultRawHtmlValue, pageTemplate.ClientId, editInTop, editInBottom, db.SeedAutoMapUserBestGuess());
			}

			PageSection pageSection = page.Sections.AsQueryable().WhereIsActive()
				.Where(ps => ps.PageTemplateSectionId == pageTemplateSection.PageTemplateSectionId)
				.OrderBy(ps => ps.Order).ThenBy(ps => ps.PageSectionId)
				.FirstOrDefault();

			if (!pageViewModel.EditMode)
			{
				return pageTemplateSection.HtmlDisplay(pageSection, htmlHelper);
			}
			return pageTemplateSection.Editor(page, pageSection, index, htmlHelper.ViewData.Model.AutoPost, htmlHelper);

		}

		/// <summary>
		/// Returns HTML value of the section section
		/// </summary>
		/// <param name="pageSection"></param>
		/// <returns></returns>
		public static MvcHtmlString HtmlDisplay<TModel>(this PageTemplateSection pageTemplateSection, PageSection pageSection, HtmlHelper<TModel> htmlHelper)
		{
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
					value = HttpUtility.HtmlEncode(htmlHelper.ReplaceVariables(pageSection.PlainText, string.Empty)).Replace("\n", "<br/>\n");
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

			Models.ViewModels.PageSectionEditViewModel viewModel = new Models.ViewModels.PageSectionEditViewModel(pageTemplateSection, page, pageSection, index, autoSubmit);

			return htmlHelper.EditorFor(model => viewModel);

		}

		public static string ReplaceVariables(this HtmlHelper htmlHelper, string text, string nullValue)
		{
			Client client = htmlHelper.CurrentClient(false);
			StoreFrontConfiguration storeFront = htmlHelper.CurrentStoreFrontConfig(false);
			UserProfile userProfile = htmlHelper.CurrentUserProfile(false);
			Page page = htmlHelper.CurrentPage(false);

			return text.ReplaceVariables(nullValue, client, storeFront, userProfile, page);
		}

		public static string ReplaceVariables(this string text, string nullValue, Client client, StoreFrontConfiguration storeFrontConfig, UserProfile userProfile, Page page)
		{
			if (nullValue == null)
			{
				throw new ArgumentNullException("nullValue");
			}

			string clientNullValue = null;
			if (client == null)
			{
				clientNullValue = nullValue;
			}

			text = text.Replace("::client.name::", clientNullValue ?? client.Name)
				.Replace("::client.clientid::", clientNullValue ?? client.ClientId.ToString())
				.Replace("::client.sendgridmailfromemail::", clientNullValue ?? client.SendGridMailFromEmail)
				.Replace("::client.sendgridmailfromname::", clientNullValue ?? client.SendGridMailFromName)
				.Replace("::client.twiliofromphone::", clientNullValue ?? client.TwilioFromPhone)
				.Replace("::client.twiliosmsfromemail::", clientNullValue ?? client.TwilioSmsFromEmail)
				.Replace("::client.twiliosmsfromname::", clientNullValue ?? client.TwilioSmsFromName);
			
			string storeFrontNullValue = null;
			if (storeFrontConfig == null)
			{
				storeFrontNullValue = nullValue;
			}
			text = text.Replace("::storefront.name::", storeFrontNullValue ?? storeFrontConfig.Name)
				.Replace("::storefront.clientid::", storeFrontNullValue ?? storeFrontConfig.StoreFrontId.ToString())
				.Replace("::storefront.accountadmin.fullname::", storeFrontNullValue ?? storeFrontConfig.AccountAdmin.FullName)
				.Replace("::storefront.accountadmin.email::", storeFrontNullValue ?? storeFrontConfig.AccountAdmin.Email)
				.Replace("::storefront.registerednotify.fullname::", storeFrontNullValue ?? storeFrontConfig.RegisteredNotify.FullName)
				.Replace("::storefront.registerednotify.email::", storeFrontNullValue ?? storeFrontConfig.RegisteredNotify.Email)
				.Replace("::storefront.welcomeperson.fullname::", storeFrontNullValue ?? storeFrontConfig.WelcomePerson.FullName)
				.Replace("::storefront.welcomeperson.email::", storeFrontNullValue ?? storeFrontConfig.WelcomePerson.Email)
				.Replace("::storefront.publicurl::", storeFrontNullValue ?? storeFrontConfig.PublicUrl)
				.Replace("::storefront.htmlfooter::", storeFrontNullValue ?? storeFrontConfig.HtmlFooter);

			string userProfileNullValue = null;
			if (userProfile == null)
			{
				userProfileNullValue = nullValue;
			}
			text = text.Replace("::userprofile.email::", userProfileNullValue ?? userProfile.Email)
				.Replace("::userprofile.fullname::", userProfileNullValue ?? userProfile.FullName)
				.Replace("::userprofile.username::", userProfileNullValue ?? userProfile.UserName)
				.Replace("::userprofile.UserProfileId::", userProfileNullValue ?? userProfile.UserProfileId.ToString());

			string pageNullValue = null;
			if (page == null)
			{
				pageNullValue = nullValue;
			}
			text = text.Replace("::page.title::", pageNullValue ?? page.PageTitle)
				.Replace("::page.name::", pageNullValue ?? page.Name)
				.Replace("::page.pageid::", pageNullValue ?? page.PageId.ToString())
				.Replace("::page.url::", pageNullValue ?? page.Url);

			DateTime today = DateTime.Today;
			return text.Replace("::date::", today.ToString())
				.Replace("::shortdate::", today.ToShortDateString().ToString())
				.Replace("::shorttime::", DateTime.Now.ToShortTimeString())
				.Replace("::dayofweek::", today.DayOfWeek.ToString())
				.Replace("::monthname::", today.ToString("MMMM"))
				.Replace("::dayofmonth::", today.Day.ToString())
				.Replace("::year::", today.ToString("YYYY"));
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


		public static MenuViewModel MenuViewModel(this HtmlHelper htmlHelper, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile)
		{
			return new MenuViewModel(storeFrontConfiguration, userProfile, htmlHelper.ViewContext.HttpContext.Session.SessionID);
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
		/// Returns a string, or the default value if the string is null or empty
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string OrDefault(this string value, string defaultValue)
		{
			if (value == null || string.IsNullOrEmpty(value))
			{
				return defaultValue;
			}
			return value;
		}

		public static string IfMatch(this string value, string valueToMatch, string returnIfMatch, string returnIfNoMatch = "", bool caseInsensitive = true)
		{
			if (value == null || string.IsNullOrEmpty(value))
			{
				if (string.IsNullOrEmpty(valueToMatch))
				{
					return returnIfMatch;
				}
				else
				{
					return returnIfNoMatch;
				}
			}

			valueToMatch = valueToMatch ?? string.Empty;
			if (caseInsensitive)
			{
				value = value.ToLower();
				valueToMatch = valueToMatch.ToLower();
			}
			if (value == valueToMatch)
			{
				return returnIfMatch;
			}
			return returnIfNoMatch;
		}

		public static string IfNoMatch(this string value, string valueToMatch, string returnIfMatch, string returnIfNoMatch = "", bool caseInsensitive = true)
		{
			//reverse returns to make the opposite of a match
			return value.IfMatch(valueToMatch, returnIfNoMatch, returnIfMatch, caseInsensitive);
		}

		/// <summary>
		/// Returns a delimiter if string is not null or empty
		/// Converts a null string into string.empty
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string DelimiterIfNotEmpty(this string value, string delimiter)
		{
			if (value == null || string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			return delimiter;
		}

		/// <summary>
		/// Returns the original string plus a delimiter if string is not null or empty
		/// Converts a null string into string.empty
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string AndDelimiter(this string value, string delimiter)
		{
			if (value == null || string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}
			return value + delimiter;
		}

		/// <summary>
		/// Returns HTML Encoded character
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToHtml(this char value)
		{
			return HttpUtility.HtmlEncode(value);
		}

		/// <summary>
		/// Returns HTML Encoded string
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToHtml(this string value)
		{
			return HttpUtility.HtmlEncode(value);
		}

		/// <summary>
		/// Returns HTML Encoded string and replaces line feeds (new lines) with BR tag
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToHtmlLines(this string value)
		{
			return HttpUtility.HtmlEncode(value).Replace("\n", "<br/>");
		}

		/// <summary>
		/// Returns HTML attribute Encoded string
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToHtmlAttribute(this string value)
		{
			return HttpUtility.HtmlAttributeEncode(value);
		}

		/// <summary>
		/// Returns JavaScript String Encoded string
		/// </summary>
		/// <param name="value"></param>
		/// <param name="addDoubleQuotes"></param>
		/// <returns></returns>
		public static MvcHtmlString ToJavaScriptMvcString(this string value, bool addDoubleQuotes = false)
		{
			return new MvcHtmlString(HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes).Replace("\n", "\\n"));
		}

		/// <summary>
		/// Returns JavaScript String Encoded string
		/// </summary>
		/// <param name="value"></param>
		/// <param name="addDoubleQuotes"></param>
		/// <returns></returns>
		public static string ToJavaScriptString(this string value, bool addDoubleQuotes = false)
		{
			return HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes).Replace("\n", "\\n");
		}

		/// <summary>
		/// Returns HTML Encoded string in MvcHtmlString format
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static MvcHtmlString ToMvcHtml(this string value)
		{
			return new MvcHtmlString(HttpUtility.HtmlEncode(value));
		}

		/// <summary>
		/// Returns HTML attribute Encoded string in MvcHtmlString format
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static MvcHtmlString ToMvcHtmlAttribute(this string value)
		{
			return new MvcHtmlString(HttpUtility.HtmlAttributeEncode(value));
		}

		/// <summary>
		/// Returns Javascript String Encoded string in MvcHtmlString format
		/// </summary>
		/// <param name="value"></param>
		/// <param name="addDoubleQuotes"></param>
		/// <returns></returns>
		public static MvcHtmlString ToMvcJavaScriptString(this string value, bool addDoubleQuotes = false)
		{
			return new MvcHtmlString(HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes));
		}

		public static object GetModelStateValue(this HtmlHelper htmlHelper, string key, Type destinationType)
		{
			ModelState modelState;
			if (htmlHelper.ViewData.ModelState.TryGetValue(key, out modelState))
			{
				if (modelState.Value != null)
				{
					return modelState.Value.ConvertTo(destinationType, null /* culture */);
				}
			}
			return null;
		}

		/// <summary>
		/// Returns a string useful for file names from a date/time value in yyyy-mm-dd_hh_mm_ss
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToFileSafeString(this DateTime value)
		{
			return value.ToString("yyyy-MM-dd_hh_mm_ss");
		}

		/// <summary>
		/// Gets a display attribute for an object without using HTML Helper
		/// works for any object, if display attribute is not found, will return property name
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="model"></param>
		/// <param name="fieldName"></param>
		/// <param name="shortName"></param>
		/// <returns></returns>
		public static string GetDisplayName<TModel>(this TModel model, string fieldName, bool shortName = false) where TModel : class
		{
			PropertyInfo prop = model.GetType().GetProperty(fieldName);
			if (prop == null)
			{
				throw new ApplicationException("Property '" + fieldName + "' not found in this object: " + model.GetType().FullName);
			}
			DisplayAttribute displayAttribute = prop.GetCustomAttribute<DisplayAttribute>();
			if (displayAttribute != null)
			{
				if (shortName && !(string.IsNullOrEmpty(displayAttribute.ShortName)))
				{
					return displayAttribute.ShortName;
				}
				if (!string.IsNullOrEmpty(displayAttribute.Name))
				{
					return displayAttribute.Name;
				}
			}

			return prop.Name;
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

		public static string ToFileName(this string value)
		{
			return HttpUtility.UrlEncode(value);
		}

		public static string Action(this RouteData routeData)
		{
			if (routeData == null)
			{
				return null;
			}
			return routeData.Values["action"].ToString();
		}

		public static string Controller(this RouteData routeData)
		{
			if (routeData == null)
			{
				return null;
			}
			return routeData.Values["controller"].ToString();
		}

		public static string Area(this RouteData routeData)
		{
			if (routeData == null)
			{
				return null;
			}
			if (routeData.DataTokens.ContainsKey("area"))
			{
				return routeData.DataTokens["area"].ToString();
			}
			return null;
		}



	}
}