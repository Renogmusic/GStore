using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreData.Areas.CatalogAdmin.ViewModels
{
	public class ProductBundleEditAdminViewModel: IValidatableObject
	{
		public ProductBundleEditAdminViewModel()
		{
		}

		public ProductBundleEditAdminViewModel(ProductBundle productBundle, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle", "Product Bundle cannot be null");
			}
			LoadValues(userProfile, productBundle);
		}

		public ProductBundleEditAdminViewModel(ProductBundle productBundle, UserProfile userProfile, string activeTab, bool isCreatePage = false, bool isEditPage = false, bool isDetailsPage = false, bool isDeletePage = false)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle", "Product Bundle cannot be null");
			}
			this.IsCreatePage = isCreatePage;
			this.IsEditPage = isEditPage;
			this.IsDetailsPage = isDetailsPage;
			this.IsDeletePage = isDeletePage;
			this.ActiveTab = activeTab;
			LoadValues(userProfile, productBundle);
		}

		protected void LoadValues(UserProfile userProfile, ProductBundle productBundle)
		{
			if (productBundle == null)
			{
				return;
			}
			this.IsActiveDirect = productBundle.IsActiveDirect();
			this.IsActiveBubble = productBundle.IsActiveBubble();

			this.ProductBundle = productBundle;
			this.Client = productBundle.Client;
			this.ClientId = (productBundle.Client == null ? 0 : productBundle.ClientId);
			this.CreateDateTimeUtc = productBundle.CreateDateTimeUtc;
			this.CreatedBy = productBundle.CreatedBy;
			this.CreatedBy_UserProfileId = productBundle.CreatedBy_UserProfileId;
			this.EndDateTimeUtc = productBundle.EndDateTimeUtc;
			this.ForRegisteredOnly = productBundle.ForRegisteredOnly;
			this.ForAnonymousOnly = productBundle.ForAnonymousOnly;
			this.IsPending = productBundle.IsPending;
			this.Name = productBundle.Name;
			this.UrlName = productBundle.UrlName;
			this.ImageName = productBundle.ImageName;
			this.ProductBundleId = productBundle.ProductBundleId;
			this.Order = productBundle.Order;
			this.Category = productBundle.Category;
			this.ProductCategoryId = productBundle.ProductCategoryId;
			this.StartDateTimeUtc = productBundle.StartDateTimeUtc;
			this.StoreFront = productBundle.StoreFront;
			this.StoreFrontId = productBundle.StoreFrontId;
			this.UpdateDateTimeUtc = productBundle.UpdateDateTimeUtc;
			this.UpdatedBy = productBundle.UpdatedBy;
			this.UpdatedBy_UserProfileId = productBundle.UpdatedBy_UserProfileId;

			this.MaxQuantityPerOrder = productBundle.MaxQuantityPerOrder;
			this.MetaDescription = productBundle.MetaDescription;
			this.MetaKeywords = productBundle.MetaKeywords;

			this.AvailableForPurchase = productBundle.AvailableForPurchase;
			this.RequestAQuote_Show = productBundle.RequestAQuote_Show;
			this.RequestAQuote_Label = productBundle.RequestAQuote_Label;
			this.RequestAQuote_PageId = productBundle.RequestAQuote_PageId;
			this.RequestAQuote_Page = productBundle.RequestAQuote_Page;
			this.Theme = productBundle.Theme;
			this.ThemeId = productBundle.ThemeId;
			this.ProductBundleDetailTemplate = productBundle.ProductBundleDetailTemplate;
			this.SummaryCaption = productBundle.SummaryCaption;
			this.SummaryHtml = productBundle.SummaryHtml;
			this.TopDescriptionCaption = productBundle.TopDescriptionCaption;
			this.TopDescriptionHtml = productBundle.TopDescriptionHtml;
			this.TopLinkHref = productBundle.TopLinkHref;
			this.TopLinkLabel = productBundle.TopLinkLabel;
			this.BottomDescriptionCaption = productBundle.BottomDescriptionCaption;
			this.BottomDescriptionHtml = productBundle.BottomDescriptionHtml;
			this.BottomLinkHref = productBundle.BottomLinkHref;
			this.BottomLinkLabel = productBundle.BottomLinkLabel;
			this.FooterHtml = productBundle.FooterHtml;
			this.ProductTypeSingle = productBundle.ProductTypeSingle;
			this.ProductTypePlural = productBundle.ProductTypePlural;
		}

		public void FillListsIfEmpty(Client client, StoreFront storeFront)
		{

		}

		public void UpdateProductBundle(ProductBundle productBundle)
		{
			this.ProductBundle = productBundle;
			this.Category = productBundle.Category;
		}

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
		[Display(Name = "Product Bundle Id", Description = "internal id number for the Product Bundle")]
		public int ProductBundleId { get; set; }

		[Editable(false)]
		[Display(Name = "ProductBundle", Description = "")]
		public ProductBundle ProductBundle { get; protected set; }

		[Display(Name = "For Registered Users Only", Description = "Check this box to make this Product Bundle appear only for registered users")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Users Only", Description = "Check this box to make this Product Bundle appear only for anonymous users")]
		public bool ForAnonymousOnly { get; set; }

		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "Name", Description = "Name of the Product Bundle. This name appears on the menu as text.")]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "URL Name", Description = "Unique URL name of the Product Bundle. This is the name as shown in the web browser address bar and links.")]
		public string UrlName { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index in the menu for this item. \nUse this to move a Product Bundle up or down on the list.")]
		public int Order { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Product Bundle Main Image", Description = "Name of the image file to use for this Product Bundle.")]
		public string ImageName { get; set; }

		[Display(Name = "Product Category Id", Description = "Category for this product Bundle")]
		public int ProductCategoryId { get; set; }

		[Editable(false)]
		[Display(Name = "Category", Description = "Category for this product Bundle")]
		public ProductCategory Category { get; protected set; }

		[Required]
		[Range(0, 1000)]
		[Display(Name = "Max Quantity Per Order or 0 for no limit", Description = "Maximum number of this product Bundle that can be ordered in one order, or 0 for no limit.")]
		public int MaxQuantityPerOrder { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Meta Tag Description", Description = "META Description tag for search engines. Description tag to describe the page to search engines.\nLeave this blank to use the Category Description Meta Tag, or Store Front Description Meta Tag (when category description meta tag is blank)")]
		public string MetaDescription { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Meta Tag Keywords", Description = "META Keywords tags for search engines. Keywords separated by a space for search engines.\nLeave this blank to use the Category Keywords Meta Tag, or Store Front Keywords Meta Tag (when category keywords meta tag is blank)")]
		public string MetaKeywords { get; set; }

		[Display(Name = "Available For Purchase", Description = "Check this box if the product Bundle is available for purchase (add to cart or buy now). Uncheck this box if product Bundle cannot be purchased online.")]
		public bool AvailableForPurchase { get; set; }

		[Display(Name = "Show Request a Quote button", Description="Check this box to show the Request A Quote button to link to the request a quote page.")]
		public bool RequestAQuote_Show { get; set; }

		[Display(Name = "Request a Quote button label", Description = "Enter text for the Request A Quote button.\nExample: 'Get a Quote'")]
		public string RequestAQuote_Label { get; set; }

		[Display(Name = "Request A Quote page", Description = "Page to use for the 'Request A Quote' button on this product Bundle.")]
		public int? RequestAQuote_PageId { get; set; }

		[Display(Name = "Request A Quote page", Description = "Page to use for the 'Request A Quote' button on this product Bundle.")]
		public Page RequestAQuote_Page { get; protected set; }

		[DataType(DataType.Text)]
		[Display(Name = "Summary Caption", Description = "Caption for the Summary field of the product Bundle display page.\nLeave this field blank to use the Category default Summary Caption.\nExample: 'Summary' or 'Overview'")]
		public string SummaryCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Summary Html", Description = "Summary description of this product Bundle.")]
		public string SummaryHtml { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Top Description Caption", Description = "Caption for the Top Description field of the product Bundle display page.\nLeave this field blank to use the Category default Top Description Caption.\nExample: 'Description' or 'Details'")]
		public string TopDescriptionCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Top Description Html", Description = "Top Description for this product Bundle.")]
		public string TopDescriptionHtml { get; set; }

		[DataType(DataType.Url)]
		[Display(Name = "Top Link URL", Description = "Link URL for the top link that appears before the top description field. Leave this blank if there is no link. Use the full URL\nExample: http://www.google.com")]
		public string TopLinkHref { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Top Link Label", Description = "Link Label for the top link that appears before the top description field. Leave this blank if there is no link.")]
		public string TopLinkLabel { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Bottom Description Caption", Description = "Caption for the Bottom Description field of the product Bundle display page.\nLeave this field blank to use the Category default Bottom Description Caption.\nExample: 'Description' or 'Details'")]
		public string BottomDescriptionCaption { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Bottom Description Html", Description = "Bottom Description for this product Bundle.")]
		public string BottomDescriptionHtml { get; set; }

		[DataType(DataType.Url)]
		[Display(Name = "Bottom Link URL", Description = "Link URL for the bottom link that appears at the end of the product Bundle display. Leave this blank if there is no link.")]
		public string BottomLinkHref { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Bottom Link Label", Description = "Link Label for the bottom link that appears after the bottom description field. Leave this blank if there is no link.")]
		public string BottomLinkLabel { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Footer Html", Description = "Footer information shown at the bottom of the product Bundle details page.")]
		public string FooterHtml { get; set; }

		[Display(Name = "Theme", Description = "Theme for Product Bundle Details page, or blank to use the category or store catalog theme")]
		public int? ThemeId { get; set; }

		[Display(Name = "Theme", Description = "Theme for Product Bundle Details page, or blank to use the category or store catalog theme")]
		public Theme Theme { get; protected set; }

		[Display(Name = "Product Bundle Details Template", Description = "Template for product Bundle details. Is set, this overrides the Details Template set on the category.")]
		public ProductBundleDetailTemplateEnum? ProductBundleDetailTemplate { get; set; }

		[MaxLength(100)]
		[Display(Name = "Product Type Single", Description = "Singular Name used when showing a Product for this bundle. \nExample: Product, Item, Song, Toy.\nDefault: Item\nLeave this blank to use the category default setting.")]
		public string ProductTypeSingle { get; set; }

		[MaxLength(100)]
		[Display(Name = "Product Type Plural", Description = "Singular Name used when showing a list of Products for this bundle. \nExample: Products, Items, Songs, Toys.\nDefault: Items\nLeave this blank to use the category default setting.")]
		public string ProductTypePlural { get; set; }

		#region StoreFrontRecord fields

		[Editable(false)]
		[Display(Name="Client", Description="")]
		public Client Client { get; protected set; }

		[Editable(false)]
		[Display(Name = "Client Id", Description = "")]
		public int ClientId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created On", Description = "Date and time this Product Bundle was created.")]
		public DateTime CreateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By", Description = "The User who created this Product Bundle")]
		public UserProfile CreatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By User Id", Description = "The User who created this Product Bundle")]
		public int CreatedBy_UserProfileId { get; protected set; }

		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Product Bundle to go INACTIVE on. \nIf this date is in the past, your Product Bundle will not show on the page \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to Inactivate a Product Bundle immediately. \nIf checked, this Product Bundle will not be shown on any pages.")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Product Bundle to be ACTIVE on. \nIf this date is in the future, your Product Bundle will not show on the page \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Store Front", Description = "Store Front for this Menu Item")]
		public StoreFront StoreFront { get; protected set; }

		[Editable(false)]
		[Display(Name = "Store Front Id", Description = "Store Front for this Menu Item")]
		public int StoreFrontId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated On", Description = "Date and time this Product Bundle was last updated.")]
		public DateTime UpdateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By", Description = "The user that last updated this Product Bundle.")]
		public UserProfile UpdatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By User Id", Description = "The user ID of the user that last updated this Product Bundle.")]
		public int UpdatedBy_UserProfileId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Is Active Direct", Description = "If checked, this record is currently active. If unchecked, this record is NOT active.")]
		public bool IsActiveDirect { get; protected set; }

		[Editable(false)]
		[Display(Name = "Is Active Bubble", Description = "If checked, this record and its related records are currently active. If unchecked, this record or its parent records are NOT active.")]
		public bool IsActiveBubble { get; protected set; }

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

			if (this.RequestAQuote_Show && this.RequestAQuote_PageId == null)
			{
				string blankFieldName = "RequestAQuote_PageId";
				string fieldDisplayBlank = this.GetDisplayName("RequestAQuote_PageId");
				string fieldDisplayCheckbox = this.GetDisplayName("RequestAQuote_PageId");

				result.Add(new ValidationResult("'" + fieldDisplayBlank + "' must be selected when the '" + fieldDisplayCheckbox + "' box is checked.", new string[] { blankFieldName }));
			}

			return result;
		}

		#endregion
	}
}