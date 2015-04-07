using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace GStoreData.AppHtmlHelpers
{
	public enum PopoverPlacementEnum : int
	{
		left = 0,
		top = 1,
		right = 2,
		bottom = 3
	}

	public static class MvcHtmlHelper
	{
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

		public static MvcHtmlString HelpLabelForModel(this HtmlHelper htmlHelper)
		{
			return htmlHelper.HelpLabelForModel(null);
		}

		public static MvcHtmlString HelpLabelForModel(this HtmlHelper htmlHelper, object htmlAttributes)
		{
			ModelMetadata metadata = htmlHelper.ViewData.ModelMetadata;
			string htmlFieldName = metadata.PropertyName;
			string labelText = metadata.Description;
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
			if (htmlAttributes == null)
			{
				htmlAttributes = new RouteValueDictionary();
				htmlAttributes.Add("class", "help-label");
			}
			else
			{
				if (!htmlAttributes.ContainsKey("class"))
				{
					htmlAttributes.Add("class", "help-label");
				}
				else if (!(htmlAttributes["class"] as string).ToLower().Contains("help-label"))
				{
					htmlAttributes["class"] = "help-label " + htmlAttributes["class"].ToString();
				}
			}

			TagBuilder tag = new TagBuilder("label");
			tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
			tag.InnerHtml = resolvedLabelText.ToHtmlLines();
			tag.MergeAttributes(htmlAttributes, replaceExisting: true);
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public static MvcHtmlString HelpLabelPopover(this HtmlHelper htmlHelper, string displayName, string helpText, string label = "?", string topLineText = "", string bottomLineText = "", PopoverPlacementEnum placement = PopoverPlacementEnum.right)
		{
			string dataContent = (string.IsNullOrEmpty(topLineText) ? "" : topLineText + "\n")
				+ (string.IsNullOrEmpty(helpText) ? "" : helpText + "\n")
				+ (string.IsNullOrEmpty(bottomLineText) ? "" : bottomLineText + "\n");

			string title = "Help for " + displayName;

			return htmlHelper.HelpLabelPopoverHelper(title, dataContent, label, placement);
		}

		/// <summary>
		/// Displays a help label (Description attribute) in a pop-over triggered by focus anchored to the body tag with help label inside and displayname as the title
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="label"></param>
		/// <param name="topLineText"></param>
		/// <param name="bottomLineText"></param>
		/// <returns></returns>
		public static MvcHtmlString HelpLabelPopoverForModel(this HtmlHelper htmlHelper, string label = "?", string topLineText = "", string bottomLineText = "", PopoverPlacementEnum placement = PopoverPlacementEnum.right)
		{
			ModelMetadata metadata = htmlHelper.ViewData.ModelMetadata;
			if (string.IsNullOrEmpty(metadata.Description))
			{
				return MvcHtmlString.Empty;
			}

			string propertyName = (string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName);

			string dataContent = (string.IsNullOrEmpty(topLineText) ? "" : topLineText + "\n")
				+ (string.IsNullOrEmpty(metadata.Description) ? "" : metadata.Description + "\n")
				+ (string.IsNullOrEmpty(bottomLineText) ? "" : bottomLineText + "\n");

			string title = "Help for " + propertyName;
			return htmlHelper.HelpLabelPopoverHelper(title, dataContent, label, placement);

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
		public static MvcHtmlString HelpLabelPopoverFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, string label = "?", string topLineText = "", string bottomLineText = "", PopoverPlacementEnum placement = PopoverPlacementEnum.right )
		{
			ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
			if (string.IsNullOrEmpty(metadata.Description))
			{
				return MvcHtmlString.Empty;
			}

			string propertyName = (string.IsNullOrEmpty(metadata.DisplayName) ? metadata.PropertyName : metadata.DisplayName);
			string title = "Help for " + propertyName;

			string dataContent = (string.IsNullOrEmpty(topLineText) ? "" : topLineText + "\n")
				+ (string.IsNullOrEmpty(metadata.Description) ? "" : metadata.Description + "\n")
				+ (string.IsNullOrEmpty(bottomLineText) ? "" : bottomLineText + "\n");

			return htmlHelper.HelpLabelPopoverHelper(title, dataContent, label, placement);
		}

		public static MvcHtmlString HelpLabelPopoverHelper(this HtmlHelper htmlHelper, string title, string dataContent, string label = "?", PopoverPlacementEnum placement = PopoverPlacementEnum.right)
		{
			return new MvcHtmlString(
			"<a href=\"javascript://\" tabindex=\"0\" class=\"help-label-popup\" role=\"button\" data-toggle=\"popover\" data-html=\"true\" data-trigger=\"focus\" "
				+ "data-container=\"body\" onclick=\"return false;\" "
				+ "data-placement=\"" + placement.ToString() + "\" "
				+ "title=\"" + title.ToHtmlAttribute() + "\" "
				+ "data-content=\"" + dataContent.ToHtmlLines().ToHtmlAttribute() + "\">" + label.ToHtmlLines() + "</a>"
				);
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

		public static MvcHtmlString ValidationMessageForModel(this HtmlHelper htmlHelper, bool addTextDangerClass = true, bool addSpanNoWrap = true, bool removeHtmlFieldPrefix = true)
		{
			return htmlHelper.ValidationMessageForModel(null, addTextDangerClass, removeHtmlFieldPrefix);
		}

		public static MvcHtmlString ValidationMessageForModel(this HtmlHelper htmlHelper, object htmlAttributes, bool addTextDangerClass = true, bool addSpanNoWrap = true, bool removeHtmlFieldPrefix = true)
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

			if (string.IsNullOrEmpty(fieldName))
			{
				fieldName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			}
			string oldHtmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			MvcHtmlString result = null;
			if (!addSpanNoWrap)
			{
				result = htmlHelper.ValidationMessage(fieldName, htmlAttribs);
			}
			else
			{
				result = new MvcHtmlString("<span style=\"white-space: nowrap\">" + htmlHelper.ValidationMessage(fieldName, htmlAttribs).ToHtmlString() + "</span>");
			}
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = oldHtmlFieldPrefix;
			}

			return result;
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
		public static MvcHtmlString TextBoxWithWatermarkFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, bool mergeHtmlAttributesFromViewData = false, bool doNotAddFormControlClass = false)
		{
			return htmlHelper.TextBoxWithWatermarkFor(expression, null, mergeHtmlAttributesFromViewData, doNotAddFormControlClass);
		}

		public static MvcHtmlString LabelWithRequiredForModel(this HtmlHelper htmlHelper)
		{
			return htmlHelper.LabelWithRequiredForModel(null);
		}

		public static MvcHtmlString LabelWithRequiredForModel(this HtmlHelper htmlHelper, object htmlAttributes)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string labelText = htmlHelper.ViewData.LabelText() ?? metaData.DisplayName ?? metaData.PropertyName;
			if (string.IsNullOrEmpty(labelText))
			{
				labelText = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			}
			if (string.IsNullOrEmpty(labelText))
			{
				throw new ApplicationException("Could not find label text in ViewData['LabelText'], MetaData.DisplayName, MetaData.PropertyName or HTML.ViewData.TemplateInfo");
			}


			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (metaData.IsRequired || htmlHelper.ViewData.Required())
			{
				labelText += " *";
				if (htmlAttribs.ContainsKey("class"))
				{
					htmlAttribs["class"] = "text-required " + htmlAttribs["class"];
				}
				else
				{
					htmlAttribs["class"] = "text-required";
				}
			}

			return htmlHelper.LabelForModel(labelText, htmlAttribs);
		}

		public static MvcHtmlString RequiredForModel(this HtmlHelper htmlHelper)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			if (metaData.IsRequired || htmlHelper.ViewData.Required())
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
		public static MvcHtmlString TextBoxWithWatermarkFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes, bool mergeHtmlAttributesFromViewData = false, bool doNotAddFormControlClass = false)
		{
			string watermark = htmlHelper.WatermarkFor(expression);
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			if (!htmlAttribs.ContainsKey("placeholder"))
			{
				htmlAttribs.Add("placeholder", watermark);
			}
			if (mergeHtmlAttributesFromViewData)
			{
				htmlAttribs.Merge(htmlHelper.ViewData["htmlAttributes"]);
			}
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
			return htmlHelper.TextBoxFor(expression, htmlAttribs);
		}


		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute, adds class for form-control unless doNotAddFormControl is true
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxWithWatermarkForModel(this HtmlHelper htmlHelper, bool setSizeToMaxLength = true, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true)
		{
			return htmlHelper.TextBoxWithWatermarkForModel(null, setSizeToMaxLength, removeHtmlFieldPrefix, doNotAddFormControlClass, mergeHtmlAttributesFromViewData);
		}

		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute, adds class for form-control unless doNotAddFormControl is true
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString TextBoxWithWatermarkForModel(this HtmlHelper htmlHelper, object htmlAttributes, bool setSizeToMaxLength = true, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true, bool useRequiredAttrib = true)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName;
			string displayName = htmlHelper.ViewData.DisplayName();
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = htmlHelper.ViewData.LabelText();
			}
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = string.IsNullOrEmpty(metaData.DisplayName) ? metaData.PropertyName : metaData.DisplayName;
			}

			bool isRequired = metaData.IsRequired || htmlHelper.ViewData.Required();

			string watermark = metaData.Watermark ?? ("Enter " + (displayName) + (isRequired ? " (Required)" : ""));
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (mergeHtmlAttributesFromViewData)
			{
				htmlAttribs.Merge(htmlHelper.ViewData["htmlAttributes"]);
			}

			if (!htmlAttribs.ContainsKey("placeholder"))
			{
				htmlAttribs.Add("placeholder", watermark);
			}

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


			int? viewDataMaxLength = htmlHelper.ViewData.MaxLength();
			int? maxLengthForModel = htmlHelper.MaxLengthAttributeForModel();
			if (viewDataMaxLength.HasValue || maxLengthForModel.HasValue)
			{
				htmlAttribs["maxlength"] = viewDataMaxLength ?? maxLengthForModel;
				if (setSizeToMaxLength)
				{
					htmlAttribs["size"] = viewDataMaxLength ?? maxLengthForModel;
				}
			}

			if (isRequired && useRequiredAttrib)
			{
				htmlAttribs["Required"] = "required";
			}

			if (mergeHtmlAttributesFromViewData)
			{
				htmlAttribs.Merge(htmlHelper.ViewData["htmlAttributes"]);
			}

			fieldName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			string oldHtmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			MvcHtmlString result = htmlHelper.TextBox(fieldName, htmlHelper.ViewData.Model, null, htmlAttribs);
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = oldHtmlFieldPrefix;
			}
			return result;

		}

		public static MvcHtmlString PasswordWithWatermarkForModel(this HtmlHelper htmlHelper, bool setSizeToMaxLength = true, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true)
		{
			return htmlHelper.PasswordWithWatermarkForModel(null, setSizeToMaxLength, removeHtmlFieldPrefix, doNotAddFormControlClass, mergeHtmlAttributesFromViewData);
		}

		public static MvcHtmlString PasswordWithWatermarkForModel(this HtmlHelper htmlHelper, object htmlAttributes, bool setSizeToMaxLength = true, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName;
			string displayName = (string.IsNullOrEmpty(metaData.DisplayName) ? metaData.PropertyName : metaData.DisplayName);

			string watermark = metaData.Watermark ?? ("Enter " + (metaData.DisplayName ?? metaData.PropertyName) + ((metaData.IsRequired || htmlHelper.ViewData.Required()) ? " (Required)" : ""));
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (!htmlAttribs.ContainsKey("placeholder"))
			{
				htmlAttribs.Add("placeholder", watermark);
			}

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

			if (mergeHtmlAttributesFromViewData)
			{
				htmlAttribs.Merge(htmlHelper.ViewData["htmlAttributes"]);
			}

			fieldName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			string oldHtmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			MvcHtmlString result = htmlHelper.Password(fieldName, htmlHelper.ViewData.Model, htmlAttribs);
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = oldHtmlFieldPrefix;
			}
			return result;

		}


		public static MvcHtmlString TextAreaWithWatermarkForModel(this HtmlHelper htmlHelper, int cols = 40, int rows = 10, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true)
		{
			return htmlHelper.TextAreaWithWatermarkForModel(null, cols, rows, doNotAddFormControlClass, mergeHtmlAttributesFromViewData);
		}

		/// <summary>
		/// Returns a TextBox with a watermark for the current field. Watermark from Prompt display attribute, adds class for form-control unless doNotAddFormControl is true
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <returns></returns>
		public static MvcHtmlString TextAreaWithWatermarkForModel(this HtmlHelper htmlHelper, object htmlAttributes, int cols = 40, int rows = 10, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName;
			string displayName = (string.IsNullOrEmpty(metaData.DisplayName) ? metaData.PropertyName : metaData.DisplayName);

			string watermark = metaData.Watermark ?? ("Enter " + (metaData.DisplayName ?? metaData.PropertyName));
			RouteValueDictionary htmlAttribs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

			if (!htmlAttribs.ContainsKey("placeholder"))
			{
				htmlAttribs.Add("placeholder", watermark);
			}

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

			if (mergeHtmlAttributesFromViewData)
			{
				htmlAttribs.Merge(htmlHelper.ViewData["htmlAttributes"]);
			}

			return htmlHelper.TextArea("", htmlHelper.ViewData.Model as string, rows, cols, htmlAttribs);
		}

		public static MvcHtmlString FieldNameForModel(this HtmlHelper htmlHelper)
		{
			return new MvcHtmlString(htmlHelper.ViewData.ModelMetadata.PropertyName);
		}

		public static MvcHtmlString DropDownListForModel(this HtmlHelper htmlHelper, IEnumerable<SelectListItem> selectList, string optionLabel = null, bool allowNull = false, bool doNotAddFormControlClass = false, bool addIdToPropertyName = true, bool removeHtmlFieldPrefix = false, bool mergeHtmlAttributesFromViewData = true, bool addUnselectedZero = false)
		{
			return htmlHelper.DropDownListForModel(selectList, null, optionLabel, allowNull, doNotAddFormControlClass, addIdToPropertyName, removeHtmlFieldPrefix, mergeHtmlAttributesFromViewData, addUnselectedZero);
		}

		public static MvcHtmlString DropDownListForModel(this HtmlHelper htmlHelper, IEnumerable<SelectListItem> selectList, object htmlAttributes, string optionLabel = null, bool allowNull = false, bool doNotAddFormControlClass = false, bool addIdToPropertyName = true, bool removeHtmlFieldPrefix = false, bool mergeHtmlAttributesFromViewData = true, bool addUnselectedZero = false)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string fieldName = metaData.PropertyName ?? "";
			if (addIdToPropertyName && !fieldName.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase))
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

			if (mergeHtmlAttributesFromViewData)
			{
				htmlAttribs.Merge(htmlHelper.ViewData["htmlAttributes"]);
			}

			if (selectList == null)
			{
				List<SelectListItem> newSelectList = new List<SelectListItem>();
				if (htmlHelper.ViewData.Model != null)
				{
					if (htmlHelper.ViewData.Model.ToString() != "0" || addUnselectedZero)
					{
						newSelectList.Add(new SelectListItem() { Selected = true, Text = "[selected] Unknown value '" + htmlHelper.ViewData.Model.ToString() + "'", Value = htmlHelper.ViewData.Model.ToString() });
					}
				}
				selectList = newSelectList;
			}
			else
			{
				if ((htmlHelper.ViewData.Model != null) && (!selectList.Any(s => s.Selected)))
				{
					if (htmlHelper.ViewData.Model.ToString() != "0" || addUnselectedZero)
					{
						List<SelectListItem> newSelectList = new List<SelectListItem>();
						newSelectList.Add(new SelectListItem() { Selected = true, Text = "[selected] Unknown value '" + htmlHelper.ViewData.Model.ToString() + "'", Value = htmlHelper.ViewData.Model.ToString() });
						newSelectList.AddRange(selectList);
						selectList = newSelectList;	
					}
				}
			}

			string dropdownField = "";
			string oldHtmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
				dropdownField = fieldName;
			}
			MvcHtmlString result = htmlHelper.DropDownList(dropdownField, selectList, optionLabel, htmlAttribs);
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = oldHtmlFieldPrefix;
			}
			return result;
		}


		public static MvcHtmlString CheckboxForModel(this HtmlHelper htmlHelper, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true)
		{
			return htmlHelper.CheckboxForModel(null, removeHtmlFieldPrefix, doNotAddFormControlClass, mergeHtmlAttributesFromViewData);
		}

		public static MvcHtmlString CheckboxForModel(this HtmlHelper htmlHelper, object htmlAttributes, bool removeHtmlFieldPrefix = true, bool doNotAddFormControlClass = false, bool mergeHtmlAttributesFromViewData = true)
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

			if (mergeHtmlAttributesFromViewData)
			{
				htmlAttribs.Merge(htmlHelper.ViewData["htmlAttributes"]);
			}

			fieldName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			string oldHtmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			if (string.IsNullOrEmpty(fieldName))
			{
				fieldName = metaData.PropertyName;
			}
			if (string.IsNullOrEmpty(fieldName))
			{
				fieldName = htmlHelper.IdForModel().ToHtmlString();
			}
			MvcHtmlString result = htmlHelper.CheckBox(fieldName, (htmlHelper.ViewData.Model as bool?) ?? false, htmlAttribs);
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = oldHtmlFieldPrefix;
			}
			return result;
			
		}


		public static int? MaxLengthAttributeForModel(this HtmlHelper htmlHelper)
		{
			ModelMetadata metaData = htmlHelper.ViewData.ModelMetadata;
			string propertyName = metaData.PropertyName;
			Type containerType = metaData.ContainerType;

			if (containerType == null)
			{
				return null;
			}
			var property = containerType.GetProperty(propertyName);
			if (property == null)
			{
				return null;
			}

			var attributes = property.GetCustomAttributes<MaxLengthAttribute>(false);
			if (attributes == null)
			{
				return null;
			}
			MaxLengthAttribute[] maxLengthAttribs = attributes.ToArray();
			if (maxLengthAttribs == null || maxLengthAttribs.Length == 0)
			{
				return null;
			}
			return maxLengthAttribs[0].Length;
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

		public static string Tab(this HtmlHelper htmlHelper, int tabCount)
		{
			return new string('\t', tabCount);
		}

		public static string RepeatString(this HtmlHelper htmlHelper, string value, int count)
		{
			return string.Concat(Enumerable.Repeat(value, count));
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

		/// <summary>
		/// Returns a string representing the number of bytes, kb, mb, gb, tb
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToByteString(this int byteCount)
		{
			return ToByteString((long)byteCount);
		}

		/// <summary>
		/// Returns a string representing the number of bytes, kb, mb, gb, tb
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToByteString(this long byteCount)
		{
			if (byteCount < 1)
			{
				// 0 or negative number
				return "0";
			}
			decimal value = byteCount;
			if (value < 1024)
			{
				return value + " B";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value.ToString("N2") + " KB";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value.ToString("N2") + " MB";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value.ToString("N2") + " GB";
			}
			value = value / 1024;
			if (value < 1024)
			{
				return value.ToString("N2") + " TB";
			}
			return value.ToString() + " (over 1,024 TB limit)";

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
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
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

		public static string FileExtension(this string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}
			int lastDot = fileName.LastIndexOf('.');
			if (lastDot == -1)
			{
				return null;
			}

			return fileName.Substring(lastDot + 1).ToUpper();
		}

		public static bool FileExtensionIsImage(this string fileName)
		{
			return fileName.FileExtensionIsAny("png", "jpg", "gif", "jpeg", "bmp");
		}

		public static bool FileExtensionIsAudio(this string fileName)
		{
			return fileName.FileExtensionIsAny("mp3", "wma", "ogg", "wav");
		}

		public static string FileMimeType(this string fileName)
		{
			return MimeMapping.GetMimeMapping(fileName);
		}

		public static bool FileExtensionIs(this string fileName, string extension)
		{
			string fileExtension = fileName.FileExtension();
			if (string.IsNullOrEmpty(fileExtension))
			{
				return false;
			}

			if (fileExtension.ToLower() == extension.ToLower())
			{
				return true;
			}
			return false;
		}

		public static bool FileExtensionIsAny(this string fileName, params string[] extensions)
		{
			string fileExtension = fileName.FileExtension();
			if (string.IsNullOrEmpty(fileExtension))
			{
				return false;
			}

			fileExtension = fileExtension.ToLower();

			foreach (string ext in extensions)
			{
				if (fileExtension == ext.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the route data action
		/// </summary>
		/// <param name="routeData"></param>
		/// <returns></returns>
		public static string Action(this RouteData routeData)
		{
			if (routeData == null)
			{
				return null;
			}
			return routeData.Values["action"].ToString();
		}

		/// <summary>
		/// Sets the routedata action
		/// </summary>
		/// <param name="routeData"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void Action(this RouteData routeData, string action)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (routeData.Values.ContainsKey("action"))
			{
				routeData.Values["action"] = action;
			}
			else
			{
				routeData.Values.Add("action", action);
			}
			
		}

		/// <summary>
		/// Gets the route data controller
		/// </summary>
		/// <param name="routeData"></param>
		/// <returns></returns>
		public static string Controller(this RouteData routeData)
		{
			if (routeData == null)
			{
				return null;
			}
			return routeData.Values["controller"].ToString();
		}

		/// <summary>
		/// Sets the routedata controller
		/// </summary>
		/// <param name="routeData"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void Controller(this RouteData routeData, string controller)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (routeData.Values.ContainsKey("controller"))
			{
				routeData.Values["controller"] = controller;
			}
			else
			{
				routeData.Values.Add("controller", controller);
			}

		}

		/// <summary>
		/// Gets the route data area
		/// </summary>
		/// <param name="routeData"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Sets the routedata area
		/// </summary>
		/// <param name="routeData"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void Area(this RouteData routeData, string area)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (routeData.DataTokens.ContainsKey("area"))
			{
				routeData.DataTokens["area"] = area;
			}
			else
			{
				routeData.DataTokens.Add("area", area);
			}
		}

		/// <summary>
		/// returns a stack trace with highlighting applicable inside a PRE element
		/// </summary>
		/// <param name="stackTrace"></param>
		/// <returns></returns>
		public static MvcHtmlString StackTraceWithHighlight(this HtmlHelper htmlHelper, string stackTrace)
		{
			if (string.IsNullOrEmpty(stackTrace))
			{
				return null;
			}
			string[] lines = stackTrace.Split(new string[] {"\n"}, StringSplitOptions.None);
			
			StringBuilder html = new StringBuilder();
			foreach (string line in lines)
			{
				if (line.Contains("line ") || line.Contains("ASP.") || line.Contains("GStore.") || line.Contains("GStoreWeb.") || line.Contains("GStoreData."))
				{
					html.Append("<span class='StackHighlight'>" + line.ToHtml() + "</span>");
				}
				else
				{
					html.Append(line.ToHtml());
				}
			}

			return new MvcHtmlString(html.ToString());
		}

		/// <summary>
		/// returns an Exception.ToString() with highlighting applicable inside a PRE element
		/// </summary>
		/// <param name="stackTrace"></param>
		/// <returns></returns>
		public static MvcHtmlString ExceptionToStringWithHighlight(this HtmlHelper htmlHelper, string exceptionToString)
		{
			if (string.IsNullOrEmpty(exceptionToString))
			{
				return null;
			}
			string[] lines = exceptionToString.Split(new string[] { "\n" }, StringSplitOptions.None);

			StringBuilder html = new StringBuilder();
			foreach (string line in lines)
			{
				if (line.Contains("line ") || line.Contains("ASP.") || line.Contains("GStore."))
				{
					html.Append("<span class='ExceptionHighlight'>" + line.ToHtml() + "</span>");
				}
				else
				{
					html.Append(line.ToHtml());
				}
			}

			return new MvcHtmlString(html.ToString());
		}

		/// <summary>
		/// Reads and renders an HTML file to the output stream like a render partial
		/// </summary>
		/// <param name="fullFilePath"></param>
		/// <returns></returns>
		public static void RenderFilePartial(this HtmlHelper htmlHelper, string fullFilePath)
		{
			if (string.IsNullOrEmpty(fullFilePath))
			{
				throw new ArgumentNullException("fullFilePath");
			}

			if (!System.IO.File.Exists(fullFilePath))
			{
				throw new ApplicationException("File not found: " + fullFilePath);
			}

			htmlHelper.ViewContext.Writer.Write(System.IO.File.ReadAllText(fullFilePath));
		}

		public static MvcHtmlString Repeat(this HtmlHelper htmlHelper, string html, int count)
		{
			StringBuilder htmlOut = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				htmlOut.Append(html);
			}
			return new MvcHtmlString(htmlOut.ToString());

		}

		public static TimeZoneInfo ServerTimeZone(this HtmlHelper htmlHelper)
		{
			return ServerTimeZone();
		}

		public static TimeZoneInfo ServerTimeZone()
		{
			return TimeZoneInfo.Local;
		}

		public static TimeZoneInfo UtcTimeZone(this HtmlHelper htmlHelper)
		{
			return UtcTimeZone();
		}

		public static TimeZoneInfo UtcTimeZone()
		{
			return TimeZoneInfo.Utc;
		}

		public static string ToShortName(this TimeZoneInfo timeZone)
		{
			if (timeZone == null)
			{
				return "(none)";
			}

			string timeZoneName = timeZone.IsDaylightSavingTime(DateTime.Now)
						  ? timeZone.DaylightName
						  : timeZone.StandardName;

			string output = string.Empty;

			string[] timeZoneWords = timeZoneName.Split(' ');
			foreach (string timeZoneWord in timeZoneWords)
			{
				if (timeZoneWord == "UTC")
				{
					output += timeZoneWord;
				}
				else if (timeZoneWord == "US")
				{
					output += timeZoneWord + " ";
				}
				else if (timeZoneWord[0] != '(')
				{
					output += timeZoneWord[0];
				}
				else
				{
					output += timeZoneWord;
				}
			}
			return output;
		}

		/// <summary>
		/// Returns True if the user is authenticated and logged in, False if anonymous
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static bool IsRegistered(this System.Security.Principal.IPrincipal user)
		{
			if (user == null)
			{
				return false;
			}
			return user.Identity.IsAuthenticated;
		}

		/// <summary>
		/// Returns True if the user is anonymous, False if user is authenticated and logged in
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static bool IsAnonymous(this System.Security.Principal.IPrincipal user)
		{
			return !user.IsRegistered();
		}

		/// <summary>
		/// Merges HTML Attributes collections
		/// The 2nd parameter is used as a defaults, if a value exists in both objects, the value in the firts parameter takes precedence
		/// Please note this function will just update the 1st parameter collection, it will not create a new dictionary
		/// If both parameters are null, htmlAttribs will be returned untouched (null)
		/// If the 1st param is null and 2nd is not, the 1st param will be set to a new dictionary copy of the 2nd param
		/// Also note the next overload that uses a dictionary
		/// </summary>
		/// <param name="htmlAttribs"></param>
		/// <param name="attribsToMerge">Anonymous type to merge</param>
		/// <returns></returns>
		public static void Merge(this RouteValueDictionary htmlAttribs, object attribsToMerge)
		{
			if (attribsToMerge == null)
			{
				return;
			}
			RouteValueDictionary htmlAttribsToMerge = HtmlHelper.AnonymousObjectToHtmlAttributes(attribsToMerge);

			htmlAttribs.Merge(htmlAttribsToMerge);
		}

		/// <summary>
		/// Merges HTML Attributes collections
		/// The 2nd parameter is used as a defaults, if a value exists in both objects, the value in the firts parameter takes precedence
		/// Please note this function will just update the 1st parameter collection, it will not create a new dictionary
		/// If both parameters are null, htmlAttribs will be returned untouched (null)
		/// If the 1st param is null and 2nd is not, the 1st param will be set to a new dictionary copy of the 2nd param
		/// Also note the next overload that uses an anonymous type
		/// </summary>
		/// <param name="htmlAttribs"></param>
		/// <param name="attribsToMerge">Anonymous type to merge</param>
		/// <returns></returns>
		public static void Merge(this RouteValueDictionary htmlAttribs, RouteValueDictionary htmlAttribsToMerge)
		{
			if (htmlAttribsToMerge == null)
			{
				return;
			}

			if (htmlAttribs == null)
			{
				htmlAttribs = new RouteValueDictionary(htmlAttribsToMerge);
				return;
			}

			foreach (string key in htmlAttribsToMerge.Keys)
			{
				if (!htmlAttribs.ContainsKey(key))
				{
					htmlAttribs.Add(key, htmlAttribsToMerge[key]);
				}
			}
		}


		/// <summary>
		/// Returns true if the request has files posted
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static bool HasFiles(this HttpRequestBase request)
		{
			if (request.Files == null || request.Files.Count == 0)
			{
				return false;
			}
			foreach (string key in request.Files.AllKeys)
			{
				if (request.Files[key] != null && request.Files[key].ContentLength > 0)
				{
					return true;
				}
			}

			return false;
		}

		public static string ErrorDetails(this ModelStateDictionary modelState)
		{
			StringBuilder errors = new StringBuilder();
			foreach (string key in modelState.Keys)
			{
				ModelState stateValue = null;
				if (modelState.TryGetValue(key, out stateValue))
				{
					foreach (ModelError error in stateValue.Errors)
					{
						errors.AppendLine(key + ": " + error.ErrorMessage + (error.Exception == null ? "" : error.Exception.ToString()));
					}
				}
			}

			return errors.ToString();
		}
	}
}