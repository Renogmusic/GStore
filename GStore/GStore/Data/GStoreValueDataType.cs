using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace GStore.Data
{

	public enum GStoreValueDataType
	{

		/// <summary>
		/// Single Line of Text
		/// </summary>
		[Display(Name = "Email Address", GroupName = "Text", Order = 50)]
		EmailAddress = 50,

		/// <summary>
		/// Single Line of Text
		/// </summary>
		[Display(Name = "URL", GroupName = "Text", Order = 60)]
		Url = 60,

		/// <summary>
		/// Single Line of Text
		/// </summary>
		[Display(Name = "Text - Single Line", GroupName = "Text", Order = 100)]
		SingleLineText = 3000,

		/// <summary>
		/// Multiple lines of text (long description)
		/// </summary>
		[Display(Name = "Text - Multiple lines", GroupName = "Text", Order = 200)]
		MultiLineText = 3100,

		/// <summary>
		/// "Yes/No or Checkbox Value"
		/// </summary>
		[Display(Name = "Checkbox Yes/No Value", GroupName = "Yes/No", Order = 300)]
		CheckboxYesNo = 1000,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Single selection dropdown list", GroupName = "ValueList", Order = 400)]
		ValueListItemDropdown = 4000,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Single selection radio buttons", GroupName = "ValueList", Order = 500)]
		ValueListItemRadio = 4100,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Multiple Selection Checkboxes", GroupName = "ValueList", Order = 600)]
		ValueListItemMultiCheckbox = 4200,

		/// <summary>
		/// Integer whole number positive or negative
		/// </summary>
		[Display(Name = "Whole number (integer) positive or negative", GroupName = "Number", Order = 700)]
		Integer = 1100,

		/// <summary>
		/// Currency (money)
		/// </summary>
		[Display(Name = "Decimal or Currency", GroupName = "Number", Order = 800)]
		Decimal = 1200,

		/// <summary>
		/// Range of two whole numbers
		/// </summary>
		[Display(Name = "Range of two Whole Numbers (integers)", GroupName = "Number Range", Order = 900)]
		IntegerRange = 2000,

		/// <summary>
		/// Range of two currencies (money) 
		/// </summary>
		[Display(Name = "Range of two Decimal or Currency numbers", GroupName = "Number Range", Order = 1000)]
		DecimalRange = 2100,

		/// <summary>
		/// HTML Markup (including links, formatting)
		/// </summary>
		[Display(Name = "HTML Markup (including links and formatting)", GroupName = "Text", Order = 1100)]
		Html = 3300,

		/// <summary>
		/// External Link to a web page
		/// </summary>
		[Display(Name = "External Link to a web page", GroupName = "Link", Order = 1200)]
		ExternalLinkToPage = 8000,

		/// <summary>
		/// External Link to an image
		/// </summary>
		[Display(Name = "External Link to an image", GroupName = "Link", Order = 1300)]
		ExternalLinkToImage = 8100,

		/// <summary>
		/// Internal Link to a Page (by ID)
		/// </summary>
		[Display(Name = "Internal Link to a Page (by ID)", GroupName = "Link", Order = 1500)]
		InternalLinkToPageById = 9100,

		/// <summary>
		/// Internal Link to a Page (by url)
		/// </summary>
		[Display(Name = "Internal Link to a Page (by url)", GroupName = "Link", Order = 1600)]
		InternalLinkToPageByUrl = 9200,

		/// <summary>
		/// Internal Link to an Image (by url)
		/// </summary>
		[Display(Name = "Internal Link to an Image (by url)", GroupName = "Link", Order = 1700)]
		InternalLinkToImageByUrl = 9300,
	}

	public enum AccountLayout
	{
		[Display(Name = "Default Layout", Order = 1)]
		Default = 0
	}
}
