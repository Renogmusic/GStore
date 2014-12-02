using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GStore.Data
{
	public enum GStoreAttributeDataType
	{
		/// <summary>
		/// "Yes/No or Checkbox Value"
		/// </summary>
		[Description("Yes/No or Checkbox Value")]
		YesNo = 1000,

		/// <summary>
		/// Integer whole number positive or negative
		/// </summary>
		[Description("Integer whole number positive or negative")]
		Integer = 1100,

		/// <summary>
		/// Currency (money)
		/// </summary>
		[Description("Currency (money)")]
		Currency = 1100,

		/// <summary>
		/// Range of two whole numbers
		/// </summary>
		[Description("Range of two whole numbers")]
		IntegerRange = 2000,

		/// <summary>
		/// Range of two currencies (money) 
		/// </summary>
		[Description("Range of two whole numbers")]
		CurrencyRange = 2100,

		/// <summary>
		/// Single Line of Text
		/// </summary>
		[Description("Single Line of Text")]
		SingleLineText = 3000,

		/// <summary>
		/// Multiple lines of text (long description)
		/// </summary>
		[Description("Multiple lines of text (long description)")]
		MultiLineText = 3100,

		/// <summary>
		/// HTML Markup (including links, formatting)
		/// </summary>
		[Description("HTML Markup (including links, formatting)")]
		Html = 3300,

		/// <summary>
		/// Value from a predefined list of values
		/// </summary>
		[Description("Value from a predefined list of values")]
		ValueListItem = 4000,

		/// <summary>
		/// External Link to a web page
		/// </summary>
		[Description("External Link to a web page")]
		ExternalLinkToPage = 8000,

		/// <summary>
		/// External Link to an image
		/// </summary>
		[Description("External Link to an image")]
		ExternalLinkToImage = 8100,

		/// <summary>
		/// Internal Link to an MVC action
		/// </summary>
		[Description("Internal Link to an MVC action")]
		InternalLinkToAction = 9000,

		/// <summary>
		/// Internal Link to a Page (by ID)
		/// </summary>
		[Description("Internal Link to a Page (by ID)")]
		InternalLinkToPageById = 9100,

		/// <summary>
		/// Internal Link to a Page (by url)
		/// </summary>
		[Description("Internal Link to a Page (by url)")]
		InternalLinkToPageByUrl = 9200,

		/// <summary>
		/// Internal Link to an Image (by url)
		/// </summary>
		[Description("Internal Link to an Image (by url)")]
		InternalLinkToImageByUrl = 9300,
	}
}
