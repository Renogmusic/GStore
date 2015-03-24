using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace GStoreData.AppHtmlHelpers
{
	public struct EnumInfo
	{
		public string PropertyName;
		public string DisplayName;
		public string DisplayDescription;
		public Type EnumType;
		public bool HasValue;
		public int? Order;
		public bool IsNullable;
		public List<EnumInfoValue> Values;
	}

	public struct EnumInfoValue
	{
		public string Name;
		public Enum Value;
		public string DisplayName;
		public string DisplayDescription;
		public DisplayAttribute FirstDisplayAttribute;
		public int? Order;
		public int? IntValue;
	}

	public static class EnumHelper
	{
		public static EnumInfo EnumIntInfoListForModel<TModel>(this HtmlHelper<TModel> htmlHelper)
		{
			ModelMetadata modelMetaData = htmlHelper.ViewData.ModelMetadata;
			Type modelType = modelMetaData.ModelType;

			//handle nullable enums by getting the underlying type
			Type underlyingType = Nullable.GetUnderlyingType(modelType); // checks for a nullable enum type
			Type enumType = null;
			bool isNullable = false;
			if (underlyingType == null)
			{
				isNullable = false;
				enumType = modelType;
			}
			else
			{
				isNullable = true;
				enumType = underlyingType;
			}

			List<EnumInfoValue> list = new List<EnumInfoValue>();

			string[] enumNames = Enum.GetNames(enumType);
			foreach (string name in enumNames)
			{
				Enum value = (Enum)Enum.Parse(enumType, name);
				list.Add(value.ToEnumInfoValue());

			}

			list = list.ApplyDefaultSort().ToList();

			EnumInfo info = new EnumInfo();
			info.DisplayDescription = modelMetaData.Description;
			info.DisplayName = modelMetaData.DisplayName ?? modelMetaData.PropertyName;
			info.EnumType = enumType;
			info.HasValue = true;
			info.IsNullable = isNullable;
			info.PropertyName = modelMetaData.PropertyName;
			info.Order = modelMetaData.Order;
			info.Values = list;

			return info;
		}

		public static EnumInfoValue ToEnumInfoValue(this Enum value)
		{
			EnumInfoValue enumInfo = new EnumInfoValue();
			enumInfo.Name = value.ToString();
			enumInfo.Value = value;

			if (value == null)
			{
				return enumInfo;
			}

			var fieldInfo = value.GetType().GetField(value.ToString());
			if (fieldInfo == null)
			{
				//field not found
				return enumInfo;
			}
			DisplayAttribute[] displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
			if (displayAttributes == null || displayAttributes.Count() == 0)
			{
				enumInfo.DisplayName = value.ToString();
				return enumInfo;
			}

			enumInfo.DisplayDescription = displayAttributes[0].GetDescription();
			enumInfo.DisplayName = displayAttributes[0].GetName() ?? value.ToString();
			enumInfo.FirstDisplayAttribute = displayAttributes[0];
			enumInfo.Order = displayAttributes[0].GetOrder();

			enumInfo.IntValue = Convert.ToInt32(value);
			return enumInfo;
		}

		/// <summary>
		/// Returns the display name of a enum value, using the Display attribute, or the enum name
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ToDisplayName(this Enum value)
		{
			if (value == null)
			{
				return "(blank)";
			}

			var fieldInfo = value.GetType().GetField(value.ToString());
			if (fieldInfo == null)
			{
				//field not found
				return "(blank)";
			}
			var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
			if (descriptionAttributes == null)
			{
				return value.ToString();
			}

			return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
		}

		public static string ToDisplayDescriptionOrBlankIfNone(this Enum target)
		{
			var fieldInfo = target.GetType().GetField(target.ToString());
			var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
			if (descriptionAttributes == null)
			{
				return string.Empty;
			}

			return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Description : string.Empty;
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
			return htmlHelper.EnumDropDownListForModel(optionLabel, null, addFormControlClass, removeHtmlFieldPrefix);
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

			fieldName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}
			string selectedValue = null;
			if (htmlHelper.ViewData.Model != null)
			{
				selectedValue = htmlHelper.ViewData.Model.ToString();
			}

			List<EnumInfoValue> orderedEnumValues = enumInfo.Values.ToList();
			IEnumerable<SelectListItem> options = orderedEnumValues.Select
				(v => new SelectListItem() { Value = v.Value.ToString(), Text = (v.Value.ToString() == selectedValue ? "[SELECTED] " : "") + v.DisplayName, Selected = v.Value.ToString() == selectedValue }
				);
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

			fieldName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (removeHtmlFieldPrefix)
			{
				htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix = "";
			}

			List<SelectListItem> options = enumInfo.Values.Select(v => new SelectListItem() { Value = v.Value.ToString(), Text = v.DisplayName }).ToList();

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
					firstMatch.Text = "[SELECTED] " + firstMatch.Text;
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

		public static IOrderedEnumerable<EnumInfoValue> ApplyDefaultSort(this IEnumerable<EnumInfoValue> values)
		{
			return values.OrderBy(e => e.Order ?? 99999).ThenBy(e => e.IntValue).ThenBy(e => e.Name);
		}
	}
}