using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GStore.Data;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Areas.CatalogAdmin.ViewModels
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

			this.DigitalDownload = product.DigitalDownload;
			this.MaxQuantityPerOrder = product.MaxQuantityPerOrder;
			this.MetaDescription = product.MetaDescription;
			this.MetaKeywords = product.MetaKeywords;
			this.BaseListPrice = product.BaseListPrice;
			this.BaseUnitPrice = product.BaseUnitPrice;
			this.DigitalDownloadFileName = product.DigitalDownloadFileName;
			this.SampleAudioCaption = product.SampleAudioCaption;
			this.SampleAudioFileName = product.SampleAudioFileName;
			this.SampleDownloadCaption = product.SampleDownloadCaption;
			this.SampleDownloadFileName = product.SampleDownloadFileName;
			this.SampleImageCaption = product.SampleImageCaption;
			this.SampleImageFileName = product.SampleImageFileName;
			this.SummaryHtml = product.SummaryHtml;
			this.DescriptionHtml = product.DescriptionHtml;
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

		public bool ReturnToFrontEnd { get; set; }

		[Required]
		[Key]
		[Display(Name = "Product Id", Description = "internal id number for the Product")]
		public int ProductId { get; set; }

		[Editable(false)]
		[Display(Name = "Product", Description = "")]
		public Product Product { get; protected set; }

		[Display(Name = "For Registered Users Only", Description = "Check this box to make this Category appear only for registered users")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Users Only", Description = "Check this box to make this Category appear only for anonymous users")]
		public bool ForAnonymousOnly { get; set; }

		[Required]
		[Display(Name = "Name", Description = "Name of the category. This name appears on the menu as text.")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "URL Name", Description = "Unique URL name of the category. This is the name as shown in the web browser address bar and links.")]
		public string UrlName { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index in the menu for this item. \nUse this to move a Category up or down on the list.")]
		public int Order { get; set; }

		[Display(Name = "Image Name", Description = "Name of the image file to use for this category.")]
		public string ImageName { get; set; }

		[Display(Name = "Product Category Id", Description = "Category for this product")]
		public int ProductCategoryId { get; set; }

		[Editable(false)]
		[Display(Name = "Category", Description = "Category for this product")]
		public ProductCategory Category { get; protected set; }

		[Display(Name = "Digital Download")]
		public bool DigitalDownload { get; set; }

		[Display(Name = "Max Quantity Per Order or 0 for no limit")]
		public int MaxQuantityPerOrder { get; set; }

		[Display(Name = "Meta Tag Description")]
		public string MetaDescription { get; set; }

		[Display(Name = "Meta Tag Keywords")]
		public string MetaKeywords { get; set; }

		[Required]
		public decimal BaseUnitPrice { get; set; }

		[Required]
		public decimal BaseListPrice { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		public string SummaryHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		public string DescriptionHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		public string FooterHtml { get; set; }

		[DataType(DataType.Text)]
		public string DigitalDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleImageFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleImageCaption { get; set; }

		[DataType(DataType.Text)]
		public string SampleAudioFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleAudioCaption { get; set; }

		[DataType(DataType.Text)]
		public string SampleDownloadFileName { get; set; }

		[DataType(DataType.Text)]
		public string SampleDownloadCaption { get; set; }


		#region StoreFrontRecord fields

		[Editable(false)]
		[Display(Name="Client", Description="")]
		public Client Client { get; protected set; }

		[Editable(false)]
		[Display(Name = "Client Id", Description = "")]
		public int ClientId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created On", Description = "Date and time this Category was created.")]
		public DateTime CreateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By", Description = "The User who created this Category")]
		public UserProfile CreatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By User Id", Description = "The User who created this Category")]
		public int CreatedBy_UserProfileId { get; protected set; }

		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Category to go INACTIVE on. \nIf this date is in the past, your Category will not show on the page \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to Inactivate a Category immediately. \nIf checked, this Category will not be shown on any pages.")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Category to be ACTIVE on. \nIf this date is in the future, your Category will not show on the page \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Store Front", Description = "Store Front for this Menu Item")]
		public StoreFront StoreFront { get; protected set; }

		[Editable(false)]
		[Display(Name = "Store Front Id", Description = "Store Front for this Menu Item")]
		public int StoreFrontId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated On", Description = "Date and time this Category was last updated.")]
		public DateTime UpdateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By", Description = "The user that last updated this Category.")]
		public UserProfile UpdatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By User Id", Description = "The user ID of the user that last updated this Category.")]
		public int UpdatedBy_UserProfileId { get; protected set; }

		[Editable(false)]
		public bool IsActiveDirect { get; set; }

		[Editable(false)]
		public bool IsActiveBubble { get; set; }

		#endregion


		#region IValidatableObject Members

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> result = new List<ValidationResult>();
			string fieldName = "UrlName";
			string fieldDisplay = null;
			if (!this.UrlName.IsValidUrlName())
			{
				fieldName = "UrlName";
				fieldDisplay = this.GetDisplayName("UrlName");
				result.Add(new ValidationResult(fieldDisplay + " value of '" + this.UrlName + "' is invalid. You cannot use any of the following characters: " + UrlName.InvalidUrlNameCharacters(), new string[] { fieldName }));
				this.UrlName = this.UrlName.FixUrlName();
			}
			return result;
		}

		#endregion
	}
}