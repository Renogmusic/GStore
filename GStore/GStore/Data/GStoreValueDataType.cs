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
		[Display(Name = "Text - Single Line", GroupName = "Text")]
		SingleLineText = 3000,

		/// <summary>
		/// Multiple lines of text (long description)
		/// </summary>
		[Display(Name = "Text - Multiple lines", GroupName = "Text")]
		MultiLineText = 3100,

		/// <summary>
		/// "Yes/No or Checkbox Value"
		/// </summary>
		[Display(Name = "Checkbox Yes/No Value", GroupName = "Yes/No")]
		CheckboxYesNo = 1000,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Single selection dropdown list", GroupName="ValueList")]
		ValueListItemDropdown = 4000,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Single selection radio buttons", GroupName = "ValueList")]
		ValueListItemRadio = 4100,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Multiple Selection Checkboxes", GroupName = "ValueList")]
		ValueListItemMultiCheckbox = 4200,

		/// <summary>
		/// Integer whole number positive or negative
		/// </summary>
		[Display(Name = "Whole number (integer) positive or negative", GroupName = "Number")]
		Integer = 1100,

		/// <summary>
		/// Currency (money)
		/// </summary>
		[Display(Name = "Decimal or Currency", GroupName = "Number")]
		Decimal = 1200,

		/// <summary>
		/// Range of two whole numbers
		/// </summary>
		[Display(Name = "Range of two Whole Numbers (integers)", GroupName = "Number Range")]
		IntegerRange = 2000,

		/// <summary>
		/// Range of two currencies (money) 
		/// </summary>
		[Display(Name = "Range of two Decimal or Currency numbers", GroupName = "Number Range")]
		DecimalRange = 2100,

		/// <summary>
		/// HTML Markup (including links, formatting)
		/// </summary>
		[Display(Name = "HTML Markup (including links and formatting)", GroupName = "Text")]
		Html = 3300,

		/// <summary>
		/// External Link to a web page
		/// </summary>
		[Display(Name = "External Link to a web page", GroupName = "Link")]
		ExternalLinkToPage = 8000,

		/// <summary>
		/// External Link to an image
		/// </summary>
		[Display(Name = "External Link to an image", GroupName = "Link")]
		ExternalLinkToImage = 8100,

		/// <summary>
		/// Internal Link to an MVC action
		/// </summary>
		[Display(Name = "Internal Link to an MVC action", GroupName = "Link")]
		InternalLinkToAction = 9000,

		/// <summary>
		/// Internal Link to a Page (by ID)
		/// </summary>
		[Display(Name = "Internal Link to a Page (by ID)", GroupName = "Link")]
		InternalLinkToPageById = 9100,

		/// <summary>
		/// Internal Link to a Page (by url)
		/// </summary>
		[Display(Name = "Internal Link to a Page (by url)", GroupName = "Link")]
		InternalLinkToPageByUrl = 9200,

		/// <summary>
		/// Internal Link to an Image (by url)
		/// </summary>
		[Display(Name = "Internal Link to an Image (by url)", GroupName = "Link")]
		InternalLinkToImageByUrl = 9300,
	}
}
