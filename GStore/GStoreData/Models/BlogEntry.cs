using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("BlogEntry")]
	public class BlogEntry : BaseClasses.StoreFrontRecord
	{
		[Key]
		public int BlogEntryId { get; set; }

		[Display(Name = "Blog Id")]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		public int BlogId { get; set; }

		[ForeignKey("BlogId")]
		[Display(Name = "Blog")]
		public virtual Blog Blog { get; set; }

		[Required]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Required]
		[MaxLength(100)]
		[Index("UniqueRecord", IsUnique = true, Order = 4)]
		[Display(Name = "Url Name")]
		public string UrlName { get; set; }

		[Display(Name = "Image Name")]
		public string ImageName { get; set; }

		public int Order { get; set; }

		[Required]
		[Display(Name = "Official Posted Date and Time in UTC")]
		public DateTime PostDateTimeUtc { get; set; }

		[Display(Name = "For Registered Users Only")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Users Only")]
		public bool ForAnonymousOnly { get; set; }

		[Display(Name = "Show in List even if user does not have permission to view")]
		public bool ShowInListEvenIfNoPermission { get; set; }

		[Display(Name = "Theme Id", Description = "Theme for Category Details page and products that do not have a theme defined. Leave blank to use the store catalog theme")]
		public int? ThemeId { get; set; }

		[Display(Name = "Theme", Description = "Theme for Category Details page and products that do not have a theme defined. Leave blank to use the store catalog theme")]
		public virtual Theme Theme { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Header Html", Description = "Header HTML shown before the blog entry.")]
		public string HeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Footer Html", Description = "Footer HTML shown after the blog entry.")]
		public string FooterHtml { get; set; }

		[Display(Name = "Meta Tag Description", Description = "META Description tag for search engines. Description tag to describe the Category to search engines.\nLeave this blank to use the Store Front Description Meta Tag. \nThis is also used for products that do not have a Description Meta Tag.")]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords", Description = "META Keywords tags for search engines. Keywords separated by a space for search engines.\nLeave this blank to use the Store Front Keywords Meta Tag. \nThis is also used for products that do not have a Keywords Meta Tag defined.")]
		public string MetaKeywords { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Body 1", Description = "1st Body Section.")]
		public string Body1 { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Body 2", Description = "2nd Body Section.")]
		public string Body2 { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Body 3", Description = "3rd Body Section.")]
		public string Body3 { get; set; }

	}
}