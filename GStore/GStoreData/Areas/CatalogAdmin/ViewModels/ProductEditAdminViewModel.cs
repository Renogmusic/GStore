using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreData.Areas.CatalogAdmin.ViewModels
{
	public class ProductEditAdminViewModel: IValidatableObject
	{
		public ProductEditAdminViewModel()
		{
		}

		public ProductEditAdminViewModel(Product product, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (product == null)
			{
				throw new ArgumentNullException("product", "Product cannot be null");
			}
			LoadValues(userProfile, product);
		}

		public ProductEditAdminViewModel(Product product, UserProfile userProfile, string activeTab, bool isCreatePage = false, bool isSimpleCreatePage = false, bool isEditPage = false, bool isDetailsPage = false, bool isDeletePage = false)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (product == null)
			{
				throw new ArgumentNullException("product", "Product cannot be null");
			}
			this.IsCreatePage = isCreatePage;
			this.IsSimpleCreatePage = isSimpleCreatePage;
			this.IsEditPage = isEditPage;
			this.IsDetailsPage = isDetailsPage;
			this.IsDeletePage = isDeletePage;
			this.ActiveTab = activeTab;
			LoadValues(userProfile, product);
		}

		protected void LoadValues(UserProfile userProfile, Product product)
		{
			if (product == null)
			{
				return;
			}
			this.IsActiveDirect = product.IsActiveDirect();
			this.IsActiveBubble = product.IsActiveBubble();

			this.Product = product;
			this.Client = product.Client;
			this.ClientId = (product.Client == null ? 0 : product.ClientId);
			this.CreateDateTimeUtc = product.CreateDateTimeUtc;
			this.CreatedBy = product.CreatedBy;
			this.CreatedBy_UserProfileId = product.CreatedBy_UserProfileId;
			this.EndDateTimeUtc = product.EndDateTimeUtc;
			this.ForRegisteredOnly = product.ForRegisteredOnly;
			this.ForAnonymousOnly = product.ForAnonymousOnly;
			this.IsPending = product.IsPending;
			this.Name = product.Name;
			this.UrlName = product.UrlName;
			this.ImageName = product.ImageName;
			this.ProductId = product.ProductId;
			this.Order = product.Order;
			this.Category = product.Category;
			this.ProductCategoryId = product.ProductCategoryId;
			this.StartDateTimeUtc = product.StartDateTimeUtc;
			this.StoreFront = product.StoreFront;
			this.StoreFrontId = product.StoreFrontId;
			this.UpdateDateTimeUtc = product.UpdateDateTimeUtc;
			this.UpdatedBy = product.UpdatedBy;
			this.UpdatedBy_UserProfileId = product.UpdatedBy_UserProfileId;

			this.MaxQuantityPerOrder = product.MaxQuantityPerOrder;
			this.MetaDescription = product.MetaDescription;
			this.MetaKeywords = product.MetaKeywords;

			this.AvailableForPurchase = product.AvailableForPurchase;
			this.RequestAQuote_Show = product.RequestAQuote_Show;
			this.RequestAQuote_Label = product.RequestAQuote_Label;
			this.RequestAQuote_PageId = product.RequestAQuote_PageId;
			this.RequestAQuote_Page = product.RequestAQuote_Page;
			this.BaseListPrice = product.BaseListPrice;
			this.BaseUnitPrice = product.BaseUnitPrice;
			this.Theme = product.Theme;
			this.ThemeId = product.ThemeId;
			this.ProductDetailTemplate = product.ProductDetailTemplate;
			this.DigitalDownload = product.DigitalDownload;
			this.DigitalDownloadFileName = product.DigitalDownloadFileName;
			this.SampleAudioCaption = product.SampleAudioCaption;
			this.SampleAudioFileName = product.SampleAudioFileName;
			this.SampleDownloadCaption = product.SampleDownloadCaption;
			this.SampleDownloadFileName = product.SampleDownloadFileName;
			this.SampleImageCaption = product.SampleImageCaption;
			this.SampleImageFileName = product.SampleImageFileName;
			this.SummaryCaption = product.SummaryCaption;
			this.SummaryHtml = product.SummaryHtml;
			this.TopDescriptionCaption = product.TopDescriptionCaption;
			this.TopDescriptionHtml = product.TopDescriptionHtml;
			this.TopLinkHref = product.TopLinkHref;
			this.TopLinkLabel = product.TopLinkLabel;
			this.BottomDescriptionCaption = product.BottomDescriptionCaption;
			this.BottomDescriptionHtml = product.BottomDescriptionHtml;
			this.BottomLinkHref = product.BottomLinkHref;
			this.BottomLinkLabel = product.BottomLinkLabel;
			this.FooterHtml = product.FooterHtml;
		}

		public void FillListsIfEmpty(Client client, StoreFront storeFront)
		{

		}

		public void UpdateProduct(Product product)
		{
			this.Product = product;
			this.Category = product.Category;
		}

		[Editable(false)]
		public bool IsSimpleCreatePage { get; set; }

		[Editable(false)]
		public bool IsCreatePage { get; set; }

		[Editable(false)]
		public bool IsEditPage { get; set; }
		
		[Editable(false)]
		public bool IsDetailsPage { get; set; }

		[Editable(false)]
		public bool IsDeletePage { get; set; }

		public string ActiveTab { get; set; }

		public bool ReturnToFrontEnd { get; set; }

		[Required]
		[Key]
		[Display(Name = "Product Id", Description = "internal id number for the Product")]
		public int ProductId { get; set; }

		[Editable(false)]
		[Display(Name = "Product", Description = "")]
		public Product Product { get; protected set; }

		[Display(Name = "For Registered Users Only", Description = "Check this box to make this Product appear only for registered users")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Users Only", Description = "Check this box to make this Product appear only for anonymous users")]
		public bool ForAnonymousOnly { get; set; }

		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "Name", Description = "Name of the Product. This name appears on the menu as text.")]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "URL Name", Description = "Unique URL name of the Product. This is the name as shown in the web browser address bar and links.")]
		public string UrlName { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index in the menu for this item. \nUse this to move a Product up or down on the list.")]
		public int Order { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Product Main Image", Description = "Name of the image file to use for this Product.")]
		public string ImageName { get; set; }

		[Display(Name = "Product Category Id", Description = "Category for this product")]
		public int ProductCategoryId { get; set; }

		[Editable(false)]
		[Display(Name = "Category", Description = "Category for this product")]
		public ProductCategory Category { get; protected set; }

		[Required]
		[Range(0, 1000)]
		[Display(Name = "Max Quantity Per Order or 0 for no limit", Description="Maximum number of this product that can be ordered in one order, or 0 for no limit.")]
		public int MaxQuantityPerOrder { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Meta Tag Description", Description = "META Description tag for search engines. Description tag to describe the page to search engines.\nLeave this blank to use the Category Description Meta Tag, or Store Front Description Meta Tag (when category description meta tag is blank)")]
		public string MetaDescription { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Meta Tag Keywords", Description = "META Keywords tags for search engines. Keywords separated by a space for search engines.\nLeave this blank to use the Category Keywords Meta Tag, or Store Front Keywords Meta Tag (when category keywords meta tag is blank)")]
		public string MetaKeywords { get; set; }

		[Display(Name = "Available For Purchase", Description = "Check this box if the product is available for purchase (add to cart or buy now). Uncheck this box if product cannot be purchased online.")]
		public bool AvailableForPurchase { get; set; }

		[Display(Name = "Show Request a Quote button", Description="Check this box to show the Request A Quote button to link to the request a quote page.")]
		public bool RequestAQuote_Show { get; set; }

		[Display(Name = "Request a Quote button label", Description = "Enter text for the Request A Quote button.\nExample: 'Get a Quote'")]
		public string RequestAQuote_Label { get; set; }

		[Display(Name = "Request A Quote page", Description = "Page to use for the 'Request A Quote' button on this product.")]
		public int? RequestAQuote_PageId { get; set; }

		[Display(Name = "Request A Quote page", Description = "Page to use for the 'Request A Quote' button on this product.")]
		public Page RequestAQuote_Page { get; protected set; }

		[Display(Name = "Unit Price", Description="Unit price for 1 of this product. Leave this blank to not show pricing.")]
		public decimal? BaseUnitPrice { get; set; }

		[Display(Name = "List Unit Price", Description = "List price for 1 of this product (may be more than the unit price actually charged). Leave this blank to not show pricing.")]
		public decimal? BaseListPrice { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Summary Caption", Description = "Caption for the Summary field of the product display page.\nLeave this field blank to use the Category default Summary Caption.\nExample: 'Summary' or 'Overview'")]
		public string SummaryCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Summary Html", Description="Summary description of this product.")]
		public string SummaryHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Top Description Caption", Description = "Caption for the Top Description field of the product display page.\nLeave this field blank to use the Category default Top Description Caption.\nExample: 'Description' or 'Details'")]
		public string TopDescriptionCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Top Description Html", Description = "Top Description for this product.")]
		public string TopDescriptionHtml { get; set; }

		[DataType(DataType.Url)]
		[Display(Name = "Top Link URL", Description = "Link URL for the top link that appears before the top description field. Leave this blank if there is no link.")]
		public string TopLinkHref { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Top Link Label", Description = "Link Label for the top link that appears before the top description field. Leave this blank if there is no link.")]
		public string TopLinkLabel { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Bottom Description Caption", Description = "Caption for the Bottom Description field of the product display page.\nLeave this field blank to use the Category default Bottom Description Caption.\nExample: 'Description' or 'Details'")]
		public string BottomDescriptionCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Bottom Description Html", Description = "Bottom Description for this product.")]
		public string BottomDescriptionHtml { get; set; }

		[DataType(DataType.Url)]
		[Display(Name = "Bottom Link URL", Description = "Link URL for the bottom link that appears at the end of the product display. Leave this blank if there is no link.")]
		public string BottomLinkHref { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Bottom Link Label", Description = "Link Label for the bottom link that appears after the bottom description field. Leave this blank if there is no link.")]
		public string BottomLinkLabel { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Footer Html", Description = "Footer information shown at the bottom of the product details page.")]
		public string FooterHtml { get; set; }

		[Display(Name = "Theme", Description = "Theme for Product Details page, or blank to use the category or store catalog theme")]
		public int? ThemeId { get; set; }

		[Display(Name = "Theme", Description = "Theme for Product Details page, or blank to use the category or store catalog theme")]
		public Theme Theme { get; protected set; }

		[Display(Name = "Product Details Template", Description = "Template for product details. Is set, this overrides the Details Template set on the category.")]
		public ProductDetailTemplateEnum? ProductDetailTemplate { get; set; }

		[Display(Name = "Digital Download", Description="Digital Download. Check this box if this product has a digital download when purchased. Be sure to set the Digital Download file name.")]
		public bool DigitalDownload { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Digital Download File", Description="Digital Download File. If this is a digital download, select the file to send the user when purchased. Be sure to check the Digital Download box as well.")]
		public string DigitalDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Image File", Description = "Sample Image File. If you have a sample image for this product, include it here to show it on the product details page.")]
		public string SampleImageFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Image Caption", Description = "Sample Image Caption. Caption shown under the sample image file.\nLeave this field blank to use the Category default Sample Image Caption.\nExample: Photo of this product.")]
		public string SampleImageCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Audio File", Description = "Sample Audio File. If you have a sample audio file for this product, include it here to show it on the product details page.")]
		public string SampleAudioFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Audio Caption", Description = "Sample Audio Caption. Caption shown under the sample audio file.\nLeave this field blank to use the Category default Sample Audio Caption.\nExample: 'Sample recording'")]
		public string SampleAudioCaption { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Download File", Description = "Sample Download File. If you have a sample download file for this product, include it here to show it on the product details page.")]
		public string SampleDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Sample Download Caption", Description = "Sample Download Caption. Caption shown under the sample download file.\nLeave this field blank to use the Category default Sample Download Caption.\nExample: PDF user's guide.")]
		public string SampleDownloadCaption { get; set; }


		#region StoreFrontRecord fields

		[Editable(false)]
		[Display(Name="Client", Description="")]
		public Client Client { get; protected set; }

		[Editable(false)]
		[Display(Name = "Client Id", Description = "")]
		public int ClientId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created On", Description = "Date and time this Product was created.")]
		public DateTime CreateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By", Description = "The User who created this Product")]
		public UserProfile CreatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By User Id", Description = "The User who created this Product")]
		public int CreatedBy_UserProfileId { get; protected set; }

		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Product to go INACTIVE on. \nIf this date is in the past, your Product will not show on the page \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to Inactivate a Product immediately. \nIf checked, this Product will not be shown on any pages.")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Product to be ACTIVE on. \nIf this date is in the future, your Product will not show on the page \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Store Front", Description = "Store Front for this Menu Item")]
		public StoreFront StoreFront { get; protected set; }

		[Editable(false)]
		[Display(Name = "Store Front Id", Description = "Store Front for this Menu Item")]
		public int StoreFrontId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated On", Description = "Date and time this Product was last updated.")]
		public DateTime UpdateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By", Description = "The user that last updated this Product.")]
		public UserProfile UpdatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By User Id", Description = "The user ID of the user that last updated this Product.")]
		public int UpdatedBy_UserProfileId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Is Active Direct", Description = "If checked, this record is currently active. If unchecked, this record is NOT active.")]
		public bool IsActiveDirect { get; set; }

		[Editable(false)]
		[Display(Name = "Is Active Bubble", Description = "If checked, this record and its related records are currently active. If unchecked, this record or its parent records are NOT active.")]
		public bool IsActiveBubble { get; set; }

		#endregion


		#region IValidatableObject Members

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> result = new List<ValidationResult>();
			if (!this.UrlName.IsValidUrlName())
			{
				string fieldName = "UrlName";
				string fieldDisplay = this.GetDisplayName("UrlName");
				result.Add(new ValidationResult(fieldDisplay + " value of '" + this.UrlName + "' is invalid. You cannot use any of the following characters: " + UrlName.InvalidUrlNameCharacters(), new string[] { fieldName }));
				this.UrlName = this.UrlName.FixUrlName();
			}

			if (this.DigitalDownload && string.IsNullOrWhiteSpace(this.DigitalDownloadFileName))
			{
				HttpContext http = HttpContext.Current;
				if (http == null || http.Request.Files["DigitalDownloadFileName_File"] == null || http.Request.Files["DigitalDownloadFileName_File"].ContentLength == 0)
				{
					string dlCheckboxName = this.GetDisplayName("DigitalDownload");
					string dlFileName = this.GetDisplayName("DigitalDownloadFileName");

					result.Add(new ValidationResult(dlFileName + " is required when '" + dlCheckboxName + "' is checked.", new string[] { "DigitalDownloadFileName" }));
				}
			}
			if (this.RequestAQuote_Show && this.RequestAQuote_PageId == null)
			{
				string blankFieldName = "RequestAQuote_PageId";
				string fieldDisplayBlank = this.GetDisplayName("RequestAQuote_PageId");
				string fieldDisplayCheckbox = this.GetDisplayName("RequestAQuote_PageId");

				result.Add(new ValidationResult("'" + fieldDisplayBlank + "' must be selected when the '" + fieldDisplayCheckbox + "' box is checked.", new string[] { blankFieldName }));
			}

			if (this.AvailableForPurchase && this.BaseUnitPrice == null)
			{
				string blankFieldName = "BaseUnitPrice";
				string fieldDisplayBlank = this.GetDisplayName("BaseUnitPrice");
				string fieldDisplayCheckbox = this.GetDisplayName("AvailableForPurchase");

				result.Add(new ValidationResult("'" + fieldDisplayBlank + "' must be selected when the '" + fieldDisplayCheckbox + "' box is checked.", new string[] { blankFieldName }));
			}

			return result;
		}

		#endregion
	}
}