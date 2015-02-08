using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace GStore.AppHtmlHelpers
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
			IEnumerable<TModel> enumValues = Enum.GetValues(enumType).Cast<TModel>();
			foreach (TModel field in enumValues)
			{
				Enum value = field as Enum;
				list.Add(value.ToEnumInfoValue());
			}

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

	}
}