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
	public class CategoryEditAdminViewModel: IValidatableObject
	{
		public CategoryEditAdminViewModel()
		{
		}

		public CategoryEditAdminViewModel(ProductCategory productCategory, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (productCategory == null)
			{
				throw new ArgumentNullException("productCategory", "Product Category cannot be null");
			}
			LoadValues(userProfile, productCategory);
		}

		public CategoryEditAdminViewModel(ProductCategory productCategory, UserProfile userProfile, string activeTab, bool isCreatePage = false, bool isSimpleCreatePage = false, bool isEditPage = false, bool isDetailsPage = false, bool isDeletePage = false)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (productCategory == null)
			{
				throw new ArgumentNullException("productCategory", "Product Category cannot be null");
			}
			this.IsCreatePage = isCreatePage;
			this.IsSimpleCreatePage = isSimpleCreatePage;
			this.IsEditPage = isEditPage;
			this.IsDetailsPage = isDetailsPage;
			this.IsDeletePage = isDeletePage;

			LoadValues(userProfile, productCategory);
		}

		protected void LoadValues(UserProfile userProfile, ProductCategory productCategory)
		{
			if (productCategory == null)
			{
				return;
			}
			this.IsActiveDirect = productCategory.IsActiveDirect();
			this.IsActiveBubble = productCategory.IsActiveBubble();

			this.ProductCategory = productCategory;
			this.Client = productCategory.Client;
			this.ClientId = (productCategory.Client == null ? 0 : productCategory.ClientId);
			this.CreateDateTimeUtc = productCategory.CreateDateTimeUtc;
			this.CreatedBy = productCategory.CreatedBy;
			this.CreatedBy_UserProfileId = productCategory.CreatedBy_UserProfileId;
			this.EndDateTimeUtc = productCategory.EndDateTimeUtc;
			this.ForRegisteredOnly = productCategory.ForRegisteredOnly;
			this.ForAnonymousOnly = productCategory.ForAnonymousOnly;
			this.IsPending = productCategory.IsPending;
			this.Name = productCategory.Name;
			this.UrlName = productCategory.UrlName;
			this.ProductCategoryId = productCategory.ProductCategoryId;
			this.Order = productCategory.Order;
			this.ParentCategory = productCategory.ParentCategory;
			this.ParentCategoryId = productCategory.ParentCategoryId;
			this.StartDateTimeUtc = productCategory.StartDateTimeUtc;
			this.StoreFront = productCategory.StoreFront;
			this.StoreFrontId = productCategory.StoreFrontId;
			this.UpdateDateTimeUtc = productCategory.UpdateDateTimeUtc;
			this.UpdatedBy = productCategory.UpdatedBy;
			this.UpdatedBy_UserProfileId = productCategory.UpdatedBy_UserProfileId;
			this.UseDividerAfterOnMenu = productCategory.UseDividerAfterOnMenu;
			this.UseDividerBeforeOnMenu = productCategory.UseDividerBeforeOnMenu;
			this.ShowInMenu = productCategory.ShowInMenu;
			this.ShowIfEmpty = productCategory.ShowIfEmpty;
			this.AllowChildCategoriesInMenu = productCategory.AllowChildCategoriesInMenu;
			this.ImageName = productCategory.ImageName;
			this.CategoryDetailTemplate = productCategory.CategoryDetailTemplate;
			this.ProductListTemplate = productCategory.ProductListTemplate;
			this.ProductDetailTemplate = productCategory.ProductDetailTemplate;
			this.ChildCategoryHeaderHtml = productCategory.ChildCategoryHeaderHtml;
			this.ChildCategoryFooterHtml = productCategory.ChildCategoryFooterHtml;
			this.ProductHeaderHtml = productCategory.ProductHeaderHtml;
			this.ProductFooterHtml = productCategory.ProductFooterHtml;
			this.NoProductsMessageHtml = productCategory.NoProductsMessageHtml;
		}

		public void FillListsIfEmpty(Client client, StoreFront storeFront)
		{

		}

		public void UpdateProductCategoryAndParent(ProductCategory productCategory)
		{
			this.ProductCategory = productCategory;
			this.ParentCategory = productCategory.ParentCategory;
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
		[Display(Name = "Category Id", Description = "internal id number for the Category")]
		public int ProductCategoryId { get; set; }

		[Editable(false)]
		[Display(Name = "Category Item", Description = "")]
		public ProductCategory ProductCategory { get; protected set; }

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

		[Display(Name = "Parent Category", Description = "Parent Category; use this to make a category into a sub-category.")]
		public ProductCategory ParentCategory { get; protected set; }

		[Display(Name = "Parent Category Id", Description = "Parent Category Id; use this to make a category into a sub-category.")]
		public int? ParentCategoryId { get; set; }

		[Display(Name = "Add Divider Before", Description = "Check this box to add a divider before this item in a dropdown menu.")]
		public bool UseDividerAfterOnMenu { get; set; }

		[Display(Name = "Add Divider After", Description = "Check this box to add a divider after this item in a dropdown menu.")]
		public bool UseDividerBeforeOnMenu { get; set; }

		[Display(Name = "Show In Menu", Description = "Check this box to show this category in the top menu.")]
		public bool ShowInMenu { get; set; }

		[Display(Name = "Image Name", Description = "Name of the image file to use for this category.")]
		public string ImageName { get; set; }

		[Display(Name = "Show If Empty", Description = "Check this box to show this category even if it has no products. Uncheck this box to only show this category when it has active products.")]
		public bool ShowIfEmpty { get; set; }

		[Display(Name = "Show Children in Menu", Description = "Check this box to show child categories in the menu, uncheck to not show child categories.")]
		public bool AllowChildCategoriesInMenu { get; set; }

		[Required]
		[Display(Name = "Category Details Template", Description = "Template for Category details.")]
		public CategoryListTemplateEnum CategoryDetailTemplate { get; set; }

		[Required]
		[Display(Name = "Product List Template", Description = "Template for product list in the category page.")]
		public ProductListTemplateEnum ProductListTemplate { get; set; }

		[Required]
		[Display(Name = "Product Details Template", Description = "Template for product details.")]
		public ProductDetailTemplateEnum ProductDetailTemplate { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Child Category Header Html", Description = "Header HTML shown before Child Categories in this category.")]
		public string ChildCategoryHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Child Category Footer Html", Description = "Footer HTML shown after Child Categories in this category.")]
		public string ChildCategoryFooterHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Product Header Html", Description = "Header HTML shown before products in this category.")]
		public string ProductHeaderHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Product Footer Html", Description = "Footer HTML shown after products in this category.")]
		public string ProductFooterHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "No Products Message Html", Description = "Message shown when there are no products in this category.")]
		public string NoProductsMessageHtml { get; set; }


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