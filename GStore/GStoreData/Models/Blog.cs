using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStoreData.Models
{
	[Table("Blog")]
	public class Blog : BaseClasses.StoreFrontRecord
	{
		[Key]
		[Display(Name = "Blog Id")]
		public int BlogId { get; set; }

		[Required]
		[MaxLength(250)]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Description")]
		public string Description { get; set; }

		[Required]
		[MaxLength(100)]
		[Index("UniqueRecord", IsUnique = true, Order = 3)]
		[Display(Name = "Url Name")]
		public string UrlName { get; set; }

		[Display(Name = "Image Name")]
		public string ImageName { get; set; }

		public int Order { get; set; }

		[Display(Name = "Auto-display Latest Entry")]
		public bool AutoDisplayLatestEntry { get; set; }

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
		[Display(Name = "List Header Html", Description = "Header HTML shown before the blog entries.")]
		public string ListHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "List Footer Html", Description = "Footer HTML shown after the blog entries.")]
		public string ListFooterHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Default Blog Entry Header Html", Description = "Default Blog entry Header HTML shown before the blog entry.")]
		public string DefaultEntryHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Default Blog Entry Footer Html", Description = "Default Blog entry Footer HTML shown after the blog entry.")]
		public string DefaultBlogFooterHtml { get; set; }

		[Display(Name = "Default Meta Tag Description", Description = "META Description tag for search engines. Description tag to describe the Category to search engines.\nLeave this blank to use the Store Front Description Meta Tag. \nThis is also used for products that do not have a Description Meta Tag.")]
		public string DefaultMetaDescription { get; set; }

		[Display(Name = "Default Meta Tag Keywords", Description = "META Keywords tags for search engines. Keywords separated by a space for search engines.\nLeave this blank to use the Store Front Keywords Meta Tag. \nThis is also used for products that do not have a Keywords Meta Tag defined.")]
		public string DefaultMetaKeywords { get; set; }

		public virtual ICollection<BlogEntry> BlogEntries { get; set; }

	}
}