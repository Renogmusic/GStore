using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("Page")]
	public class Page : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Editable(false)]
		[Display(Name = "Page Id")]
		public int PageId { get; set; }

		[Display(Name = "Page Template Id")]
		[Required]
		public int PageTemplateId { get; set; }

		[ForeignKey("PageTemplateId")]
		[Display(Name = "Page Template")]
		public virtual PageTemplate PageTemplate { get; set; }

		[Required]
		[MaxLength(200)]
		public string Name { get; set; }

		public int Order { get; set; }

		[ForeignKey("ThemeId") ]
		public virtual Theme Theme { get; set;}

		[Display(Name="Theme Id")]
		public int ThemeId { get; set; }

		[Required]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[MaxLength(250)]
		public string Url { get; set; }

		[Display(Name = "For Registered Only", ShortName = "Registered Only")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Only", ShortName = "Anonymous Only")]
		public bool ForAnonymousOnly { get; set; }

		[Display(Name = "Meta Tag Description")]
		[MaxLength(2000)]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Application Name")]
		[MaxLength(200)]
		public string MetaApplicationName { get; set; }

		[Display(Name = "Meta Tag Application Tile Color")]
		[MaxLength(20)]
		public string MetaApplicationTileColor { get; set; }
	
		[Display(Name = "Meta Tag Keywords")]
		[MaxLength(2000)]
		public string MetaKeywords { get; set; }

		[Display(Name = "Page Title")]
		[MaxLength(200)]
		public string PageTitle { get; set; }

		[Display(Name = "Body Top Script Tag")]
		[MaxLength(10000)]
		[DataType(DataType.MultilineText)]
		public string BodyTopScriptTag { get; set; }

		[Display(Name = "Body Bottom Script Tag")]
		[MaxLength(10000)]
		public string BodyBottomScriptTag { get; set; }

		[Display(Name = "Web Form Id")]
		public int? WebFormId { get; set; }

		[ForeignKey("WebFormId")]
		[Display(Name = "Web Form")]
		public virtual WebForm WebForm { get; set; }

		[Display(Name = "Web Form Success Thank You Message Title")]
		[MaxLength(200)]
		public string WebFormThankYouTitle { get; set; }

		[AllowHtml]
		[Display(Name = "Web Form Success Thank You Message Body")]
		[MaxLength(2000)]
		[DataType(DataType.Html)]
		public string WebFormThankYouMessage { get; set; }

		[Display(Name = "Web Form Success Page Id")]
		public int? WebFormSuccessPageId { get; set; }

		[Display(Name = "Web Form Save to Database")]
		public bool WebFormSaveToDatabase { get; set; }

		[Display(Name = "Web Form Save to File")]
		public bool WebFormSaveToFile { get; set; }

		[Display(Name = "Web Form Send To Email")]
		public bool WebFormSendToEmail { get; set; }

		//[ForeignKey("WebFormSuccessPageId")]
		//[Display(Name = "Web Form Success Page")]
		//public virtual Page WebFormSuccessPage { get; set; }

		[Display(Name = "Web Form Email To Address")]
		[MaxLength(200)]
		public string WebFormEmailToAddress { get; set; }

		[Display(Name = "Web Form Email To Name")]
		[MaxLength(200)]
		public string WebFormEmailToName { get; set; }

		[Display(Name = "Sections")]
		public virtual ICollection<PageSection> Sections { get; set; }

		[Display(Name = "Web Form Responses")]
		public virtual ICollection<WebFormResponse> WebFormResponses { get; set; }

		[Display(Name = "Nav Bar Items")]
		public virtual ICollection<NavBarItem> NavBarItems { get; set; }

	}
}