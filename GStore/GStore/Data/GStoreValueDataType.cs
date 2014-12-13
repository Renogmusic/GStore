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
		/// "Yes/No or Checkbox Value"
		/// </summary>
		[Display(Name="Yes/No or Checkbox Value")]
		YesNo = 1000,

		/// <summary>
		/// Integer whole number positive or negative
		/// </summary>
		[Display(Name = "Integer whole number positive or negative")]
		Integer = 1100,

		/// <summary>
		/// Currency (money)
		/// </summary>
		[Display(Name = "Decimal or Currency")]
		Decimal = 1200,

		/// <summary>
		/// Range of two whole numbers
		/// </summary>
		[Display(Name = "Range of two Integer whole numbers")]
		IntegerRange = 2000,

		/// <summary>
		/// Range of two currencies (money) 
		/// </summary>
		[Display(Name = "Range of two Decimal or Currency numbers")]
		DecimalRange = 2100,

		/// <summary>
		/// Single Line of Text
		/// </summary>
		[Display(Name = "Text - Single Line")]
		SingleLineText = 3000,

		/// <summary>
		/// Multiple lines of text (long description)
		/// </summary>
		[Display(Name = "Text - Multiple lines")]
		MultiLineText = 3100,

		/// <summary>
		/// HTML Markup (including links, formatting)
		/// </summary>
		[Display(Name = "HTML Markup (including links, formatting)")]
		Html = 3300,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Single selection dropdown list")]
		ValueListItemDropdown = 4000,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Single selection radio buttons")]
		ValueListItemRadio = 4100,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Display(Name = "Value List - Multiple Selection Checkboxes")]
		ValueListItemMultiCheckbox = 4200,

		/// <summary>
		/// External Link to a web page
		/// </summary>
		[Display(Name = "External Link to a web page")]
		ExternalLinkToPage = 8000,

		/// <summary>
		/// External Link to an image
		/// </summary>
		[Display(Name = "External Link to an image")]
		ExternalLinkToImage = 8100,

		/// <summary>
		/// Internal Link to an MVC action
		/// </summary>
		[Display(Name = "Internal Link to an MVC action")]
		InternalLinkToAction = 9000,

		/// <summary>
		/// Internal Link to a Page (by ID)
		/// </summary>
		[Display(Name = "Internal Link to a Page (by ID)")]
		InternalLinkToPageById = 9100,

		/// <summary>
		/// Internal Link to a Page (by url)
		/// </summary>
		[Display(Name = "Internal Link to a Page (by url)")]
		InternalLinkToPageByUrl = 9200,

		/// <summary>
		/// Internal Link to an Image (by url)
		/// </summary>
		[Display(Name = "Internal Link to an Image (by url)")]
		InternalLinkToImageByUrl = 9300,
	}
}
