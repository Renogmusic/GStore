using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using GStore.Data;
using GStore.Identity;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Models.ViewModels
{
	public class WebFormFieldEditViewModel
	{
		public WebFormFieldEditViewModel()
		{
			//model binder will use this option
			SetDefaults(null);
		}

		public WebFormFieldEditViewModel(WebFormEditViewModel webFormEditViewModel, WebFormField webFormField, int index)
		{
			if (webFormEditViewModel == null)
			{
				throw new ArgumentNullException("webFormEditViewModel");
			}

			if (webFormEditViewModel.WebForm == null)
			{
				throw new ArgumentNullException("webFormEditViewModel.WebForm", "webFormEditViewModel.WebForm cannot be null. make sure page edit sets this after post");
			}

			this.Index = index;

			this.WebFormEditViewModel = webFormEditViewModel;
			this.WebForm = webFormEditViewModel.WebForm;
			this.WebFormId = webFormEditViewModel.WebForm.WebFormId;
			this.IsStoreAdminEdit = webFormEditViewModel.IsStoreAdminEdit;
			this.IsReadOnly = webFormEditViewModel.IsReadOnly;
			this.IsDeletePage = webFormEditViewModel.IsDeletePage;
			this.IsCreatePage = webFormEditViewModel.IsCreatePage;
			this.ActiveTab = webFormEditViewModel.ActiveTab;

			SetDefaults(webFormEditViewModel);
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

		protected void SetDefaults(WebFormEditViewModel webFormEditViewModel)
		{
			if (webFormEditViewModel == null)
			{
				return;
			}

			this.WebForm = webFormEditViewModel.WebForm;
			this.WebFormId = webFormEditViewModel.WebForm.WebFormId;
			this.IsPending = false;
			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			this.StartDateTimeUtc = DateTime.UtcNow.AddYears(100);
			this.DataType = GStoreValueDataType.SingleLineText;
			this.DataTypeString = this.DataType.ToDisplayName();
			this.Description = "new field";
			this.IsRequired = false;
			this.LabelText = "Text";
			this.HelpLabelBottomText = "Enter text here";
			this.Order = 9000;
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
		public WebFormEditViewModel WebFormEditViewModel { get; protected set; }
	
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


	}
}