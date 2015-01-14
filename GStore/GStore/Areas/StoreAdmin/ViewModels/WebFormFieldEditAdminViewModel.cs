using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;
using GStore.Identity;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;
using GStore.Models;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class WebFormFieldEditAdminViewModel: IValidatableObject
	{
		public WebFormFieldEditAdminViewModel()
		{
			//model binder will use this option
			SetDefaults(null);
		}

		public WebFormFieldEditAdminViewModel(WebFormEditAdminViewModel WebFormEditAdminViewModel, WebFormField webFormField, int index)
		{
			if (WebFormEditAdminViewModel == null)
			{
				throw new ArgumentNullException("WebFormEditAdminViewModel");
			}

			if (WebFormEditAdminViewModel.WebForm == null)
			{
				throw new ArgumentNullException("WebFormEditAdminViewModel.WebForm", "WebFormEditAdminViewModel.WebForm cannot be null. make sure page edit sets this after post");
			}

			this.Index = index;

			this.WebFormEditAdminViewModel = WebFormEditAdminViewModel;
			this.WebForm = WebFormEditAdminViewModel.WebForm;
			this.WebFormId = WebFormEditAdminViewModel.WebForm.WebFormId;
			this.IsStoreAdminEdit = WebFormEditAdminViewModel.IsStoreAdminEdit;
			this.IsReadOnly = WebFormEditAdminViewModel.IsReadOnly;
			this.IsDeletePage = WebFormEditAdminViewModel.IsDeletePage;
			this.IsCreatePage = WebFormEditAdminViewModel.IsCreatePage;
			this.ActiveTab = WebFormEditAdminViewModel.ActiveTab;

			SetDefaults(WebFormEditAdminViewModel);
			if (webFormField != null)
			{
				this.Name = webFormField.Name;
				this.DataType = webFormField.DataType;
				this.DataTypeString = webFormField.DataTypeString;
				this.Description = webFormField.Description;
				this.EndDateTimeUtc = webFormField.EndDateTimeUtc;
				this.HelpLabelBottomText = webFormField.HelpLabelBottomText;
				this.HelpLabelTopText = webFormField.HelpLabelTopText;
				this.IsPending = webFormField.IsPending;
				this.IsRequired = webFormField.IsRequired;
				this.LabelText = webFormField.LabelText;
				this.Watermark = webFormField.Watermark;
				this.Order = webFormField.Order;
				this.StartDateTimeUtc = webFormField.StartDateTimeUtc;
				this.TextAreaColumns = webFormField.TextAreaColumns;
				this.TextAreaRows = webFormField.TextAreaRows;
				this.ValueList = webFormField.ValueList;
				this.ValueListId = webFormField.ValueListId;
				this.WebFormFieldId = webFormField.WebFormFieldId;
				this.WebFormFieldResponses = webFormField.WebFormFieldResponses;

				this.IsActiveDirect = webFormField.IsActiveDirect();
				this.IsActiveBubble = webFormField.IsActiveBubble();

				FillListsIfEmpty(webFormField.Client);
			
			}
		}

		protected void SetDefaults(WebFormEditAdminViewModel WebFormEditAdminViewModel)
		{
			if (WebFormEditAdminViewModel == null)
			{
				return;
			}

			this.Name = "New Field";
			this.Order = 100;
			if (WebFormEditAdminViewModel.WebForm != null && WebFormEditAdminViewModel.WebForm.WebFormFields.Count > 0)
			{
				this.Order = WebFormEditAdminViewModel.WebFormFields.Max(wff => wff.Order) + 10;
				bool nameIsDirty = WebFormEditAdminViewModel.WebForm.WebFormFields.Any(wff => wff.Name.ToLower() == this.Name.ToLower());
				if (nameIsDirty)
				{
					int counter = 1;
					do
					{
						counter++;
						this.Name = "New Field " + counter;
						nameIsDirty = WebFormEditAdminViewModel.WebForm.WebFormFields.Any(wff => wff.Name.ToLower() == this.Name.ToLower());
					} while (nameIsDirty);
				}
				
			}

			this.WebForm = WebFormEditAdminViewModel.WebForm;
			this.WebFormId = WebFormEditAdminViewModel.WebForm.WebFormId;
			this.IsPending = false;
			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			this.StartDateTimeUtc = DateTime.UtcNow.AddYears(100);
			this.DataType = GStoreValueDataType.SingleLineText;
			this.DataTypeString = this.DataType.ToDisplayName();
			this.Description = this.Name;
			this.IsRequired = false;
			this.LabelText = "Text";
			this.Watermark = this.Name;
			this.HelpLabelBottomText = "Enter text here";
			this.TextAreaColumns = null;
			this.TextAreaRows = null;
		}

		public void FillListsIfEmpty(Client client)
		{
			if (this.ValueListSelectList == null)
			{
				this.ValueListSelectList = client.ValueLists.AsQueryable().ToSelectList(this.ValueListId).ToList();
			}
		}

		[Key]
		[Display(Name = "Web Form Field Id", Description="Internal Id for field")]
		public int WebFormFieldId { get; set; }

		[Editable(false)]
		[Required]
		[Display(Name = "Web Form Id", Description="Internal Web Form ID of the form this field is on")]
		public int WebFormId { get; set; }

		[Editable(false)]
		[Display(Name = "Web Form", Description = "Web Form ID this field is on")]
		public WebForm WebForm { get; protected set; }

		[Editable(false)]
		[Display(Name = "Web Form View Model", Description = "Web Form this field is on")]
		public WebFormEditAdminViewModel WebFormEditAdminViewModel { get; protected set; }
	
		[Required]
		[Display(Name = "Name", Description="Name of field for internal reference")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index number for this field in the form")]
		public int Order { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Label Text", Description = "Text to show on form as a label for this field")]
		public string LabelText { get; set; }

		[Required]
		[MaxLength(100)]
		[Display(Name = "Watermark Text", Description = "Text to show on text boxes when they are empty. Useful for user input. \nExample: Enter your Email address.")]
		public string Watermark { get; set; }

		[MaxLength(100)]
		[Display(Name = "Help Label Top Text", Description = "Help information to show on top of this field")]
		public string HelpLabelTopText { get; set; }

		[MaxLength(100)]
		[Display(Name = "Help Label Bottom Text", Description = "Help information to show on the bottom of this field")]
		public string HelpLabelBottomText { get; set; }

		[Required]
		[MaxLength(200)]
		[Display(Name = "Description", Description="Internal description of this field for your reference")]
		public string Description { get; set; }

		[Display(Name = "Is Required", Description="If checked, this field is required and the user cannot continue until they enter a value.")]
		public bool IsRequired { get; set; }

		[Required]
		[Display(Name = "Data Type", Description = "The type of input allowed in this field.")]
		public GStoreValueDataType DataType { get; set; }

		[Editable(false)]
		[MaxLength(50)]
		[Display(Name = "Data Type Name", Description = "Internal Name of the type of input allowed in this field.")]
		public string DataTypeString { get; set; }

		[Display(Name = "Value List Id", Description="A value list to use as selections for this field.")]
		public int? ValueListId { get; set; }

		[Editable(false)]
		[Display(Name = "Value List", Description = "A value list to use as selections for this field.")]
		public virtual ValueList ValueList { get; protected set; }

		[Display(Name = "Text Area Rows", Description = "Number of rows to display if this field is multi-line text")]
		public int? TextAreaRows { get; set; }

		[Display(Name = "Text Area Columns", Description = "Number of columns to display if this field is multi-line text")]
		public int? TextAreaColumns { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to make this Field Inactive immediately")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date and Time Utc", Description = "Enter the time in UTC time to start using this form field \nExample: 1/1/2000 12:00 AM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Display(Name = "End Date and Time Utc", Description = "Enter the time in UTC time to stop using this form field \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Responses")]
		public virtual ICollection<WebFormFieldResponse> WebFormFieldResponses { get; protected set; }

		[Editable(false)]
		public bool IsStoreAdminEdit { get; set; }

		[Editable(false)]
		[Display(Name = "Status", Description = "If status is INACTIVE, this field will not show on any forms.")]
		public bool IsActiveDirect { get; set; }

		[Editable(false)]
		public bool IsActiveBubble { get; set; }

		[Editable(false)]
		public bool IsReadOnly { get; set; }

		[Editable(false)]
		public bool IsDeletePage { get; set; }

		[Editable(false)]
		public bool IsCreatePage { get; set; }

		[Editable(false)]
		public string ActiveTab { get; set; }

		public int Index { get; set; }

		public List<SelectListItem> ValueListSelectList { get; set; }



		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> result = new List<ValidationResult>();

			string dataTypeLabel = this.GetDisplayName("DataType");
			string dataTypeDisplayName = this.DataType.ToDisplayName();
			string fieldName = null;
			string fieldDisplay = null;

			switch (this.DataType)	
			{
				case GStoreValueDataType.EmailAddress:
					break;
				case GStoreValueDataType.Url:
					break;
				case GStoreValueDataType.SingleLineText:
					break;
				case GStoreValueDataType.MultiLineText:
					break;
				case GStoreValueDataType.CheckboxYesNo:
					break;

				case GStoreValueDataType.ValueListItemDropdown:
					if (!this.ValueListId.HasValue)
					{
						fieldName = "ValueListId";
						fieldDisplay = this.GetDisplayName(fieldName);
						result.Add(new ValidationResult(fieldDisplay + " is required for " + dataTypeLabel + " - " + dataTypeDisplayName, new string[] { fieldName }));
					}
					break;
				case GStoreValueDataType.ValueListItemRadio:
					if (!this.ValueListId.HasValue)
					{
						fieldName = "ValueListId";
						fieldDisplay = this.GetDisplayName(fieldName);
						result.Add(new ValidationResult(fieldDisplay + " is required for " + dataTypeLabel + " - " + dataTypeDisplayName, new string[] { fieldName }));
					}
					break;
				case GStoreValueDataType.ValueListItemMultiCheckbox:
					if (!this.ValueListId.HasValue)
					{
						fieldName = "ValueListId";
						fieldDisplay = this.GetDisplayName(fieldName);
						result.Add(new ValidationResult(fieldDisplay + " is required for " + dataTypeLabel + " - " + dataTypeDisplayName, new string[] { fieldName }));
					}
					break;
				case GStoreValueDataType.Integer:
					break;
				case GStoreValueDataType.Decimal:
					break;
				case GStoreValueDataType.IntegerRange:
					break;
				case GStoreValueDataType.DecimalRange:
					break;
				case GStoreValueDataType.Html:
					break;
				case GStoreValueDataType.ExternalLinkToPage:
					break;
				case GStoreValueDataType.ExternalLinkToImage:
					break;
				case GStoreValueDataType.InternalLinkToPageById:
					break;
				case GStoreValueDataType.InternalLinkToPageByUrl:
					break;
				case GStoreValueDataType.InternalLinkToImageByUrl:
					break;
				default:
					break;
			}

			return result;

		}
	}
}