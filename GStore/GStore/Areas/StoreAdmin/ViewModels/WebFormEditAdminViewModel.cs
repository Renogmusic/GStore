using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GStore.Data;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class WebFormEditAdminViewModel
	{
		public WebFormEditAdminViewModel()
		{
		}

		public WebFormEditAdminViewModel(StoreFront storeFront, UserProfile userProfile, WebForm webForm, string activeTab, bool isStoreAdminEdit = false, bool isReadOnly = false, bool isDeletePage = false, bool isCreatePage = false, string sortBy = "", bool? sortAscending = true)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm", "Web form cannot be null");
			}
			this.IsStoreAdminEdit = isStoreAdminEdit;
			this.IsActiveDirect = webForm.IsActiveDirect();
			this.IsActiveBubble = webForm.IsActiveBubble();
			this.IsReadOnly = isReadOnly;
			this.IsDeletePage = isDeletePage;
			this.IsCreatePage = isCreatePage;
			this.ActiveTab = activeTab;
			this.SortBy = sortBy;
			this.SortAscending = sortAscending;
			LoadValues(storeFront, userProfile, webForm);
		}

		protected void LoadValues(StoreFront storeFront, UserProfile userProfile, WebForm webForm)
		{
			if (webForm == null)
			{
				return;
			}

			this.WebForm = webForm;
			this.Client = webForm.Client;
			this.ClientId = (webForm.Client == null ? 0 : webForm.ClientId);
			this.CreateDateTimeUtc = webForm.CreateDateTimeUtc;
			this.CreatedBy = webForm.CreatedBy;
			this.CreatedBy_UserProfileId = webForm.CreatedBy_UserProfileId;
			this.Description = webForm.Description;
			this.DisplayTemplateName = webForm.DisplayTemplateName;
			this.EndDateTimeUtc = webForm.EndDateTimeUtc;
			this.FieldMdColSpan = webForm.FieldMdColSpan;
			this.FormFooterAfterSubmitHtml = webForm.FormFooterAfterSubmitHtml;
			this.FormFooterBeforeSubmitHtml = webForm.FormFooterBeforeSubmitHtml;
			this.FormHeaderHtml = webForm.FormHeaderHtml;
			this.IsPending = webForm.IsPending;
			this.LabelMdColSpan = webForm.LabelMdColSpan;
			this.Name = webForm.Name;
			this.Order = webForm.Order;
			this.Pages = webForm.Pages;
			this.StartDateTimeUtc = webForm.StartDateTimeUtc;
			this.SubmitButtonClass = webForm.SubmitButtonClass;
			this.SubmitButtonText = webForm.SubmitButtonText;
			this.Title = webForm.Title;
			this.UpdateDateTimeUtc = webForm.UpdateDateTimeUtc;
			this.UpdatedBy = webForm.UpdatedBy;
			this.UpdatedBy_UserProfileId = webForm.UpdatedBy_UserProfileId;
			this.WebFormFields = webForm.WebFormFields;
			this.WebFormId = webForm.WebFormId;
			this.WebFormResponses = webForm.WebFormResponses;
			this._webFormFieldViewModels = null;

		}

		public void FillFieldsFromViewModel(WebForm webFormToUpdate, WebFormFieldEditAdminViewModel[] webFormFields)
		{
			this.WebForm = webFormToUpdate;
			this.WebFormFields = webFormToUpdate.WebFormFields;
			this.webFormFieldPostData = webFormFields;
			this._webFormFieldViewModels = null;
		}
		public WebFormFieldEditAdminViewModel[] webFormFieldPostData { get; protected set; }

		[Editable(false)]
		[Display(Name = "Web Form", Description = "")]
		public WebForm WebForm { get; protected set; }

		[Editable(false)]
		[Display(Name="Client", Description="")]
		public Client Client { get; protected set; }

		[Editable(false)]
		[Display(Name = "Client Id", Description = "")]
		public int ClientId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created On", Description = "Date and time this form was created.")]
		public DateTime CreateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By", Description = "The User who created this form")]
		public UserProfile CreatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Created By User Id", Description = "The User who created this form")]
		public int CreatedBy_UserProfileId { get; protected set; }

		[Display(Name = "Description", Description = "Enter a Description of this form for your internal notes.")]
		[Required]
		public string Description { get; set; }
		
		[Required]
		[Display(Name = "Display Template", Description = "Display template for the form form fields. \nUse 'WebForm' for the system default display engine.")]
		public string DisplayTemplateName { get; set; }

		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this form to go INACTIVE on. \nIf this date is in the past, your form will not show on the page \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Field Column Span", Description = "Enter the number of columns to span for form fields. \nExample: 7")]
		public int FieldMdColSpan { get; set; }
		
		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Form Footer After Submit", Description = "Form footer shown after the submit button.")]
		public string FormFooterAfterSubmitHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Form Footer Before Submit", Description = "Form footer shown before the submit button.")]
		public string FormFooterBeforeSubmitHtml { get; set; }

		[AllowHtml]
		[DataType(DataType.Html)]
		[Display(Name = "Form Header", Description = "Form Header shown after the title and before form fields.")]
		public string FormHeaderHtml { get; set; }
		
		[Display(Name = "Inactive", Description = "Check this box to Inactivate a Form immediately. \nIf checked, this form will not be shown on any pages.")]
		public bool IsPending { get; set; }

		[Required]
		[Range(1, 12)]
		[Display(Name = "Label Column Span", Description = "Enter the number of columns to span for labels on form fields. \nExample: 3")]
		public int LabelMdColSpan { get; set; }

		[Required]
		[Display(Name = "Name", Description = "Name of the form for internal purposes.")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index in form list for this form. \nUse this to move a form up or down on your list.")]
		public int Order { get; set; }
		
		[Editable(false)]
		[Display(Name = "Pages", Description = "Pages that have this Form linked to them")]
		public ICollection<Page> Pages { get; protected set; }

		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this form to be ACTIVE on. \nIf this date is in the future, your form will not show on the page \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Display(Name = "Submit Button CSS Class", Description = "Style Sheet CSS Class for the submit button. \nExample: btn btn-primary.")]
		public string SubmitButtonClass { get; set; }

		[Display(Name = "Submit Button Text", Description = "Text shown on the submit button. \nExample: Submit.")]
		[Required]
		public string SubmitButtonText { get; set; }

		[Display(Name = "Title", Description = "Title of the web form. This is shown on the top of the form before the Header HTML. \nLeave blank for no title.")]
		public string Title { get; set; }

		[Editable(false)]
		[Display(Name = "Updated On", Description = "Date and time this form was last updated.")]
		public DateTime UpdateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By", Description = "The user that last updated this form.")]
		public UserProfile UpdatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By User Id", Description = "The user ID of the user that last updated this form.")]
		public int UpdatedBy_UserProfileId { get; protected set; }

		[Editable(false)]
		[Display(Name = "Web Form Fields", Description = "Web Form Fields for this form.")]
		public ICollection<WebFormField> WebFormFields { get; protected set; }

		[Editable(false)]
		[Display(Name = "Web Form Id", Description = "Internal ID value for this Form")]
		public int WebFormId { get; set; }

		[Editable(false)]
		[Display(Name = "Form Responses", Description = "Responses to this web form saved in the database.")]
		public ICollection<WebFormResponse> WebFormResponses { get; protected set; }

		[Editable(false)]
		public bool IsStoreAdminEdit { get; set; }

		[Editable(false)]
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

		[Editable(false)]
		public string SortBy { get; set; }

		[Editable(false)]
		public bool? SortAscending { get; set; }

		public List<WebFormFieldEditAdminViewModel> WebFormFieldEditAdminViewModels()
		{
			if (_webFormFieldViewModels != null)
			{
				return _webFormFieldViewModels;
			}
			if (this.WebFormFields == null)
			{
				return new List<WebFormFieldEditAdminViewModel>();
			}

			_webFormFieldViewModels = this.WebFormFields.AsQueryable().ApplySort(null, this.SortBy, this.SortAscending)
				.Select(fld => new WebFormFieldEditAdminViewModel(this, fld, 0)).ToList();

			int counter = 0;
			foreach (var field in _webFormFieldViewModels)
			{
				field.Index = counter;
				counter++;
			}
			return _webFormFieldViewModels;
		}
		protected List<WebFormFieldEditAdminViewModel> _webFormFieldViewModels = null;

	}
}