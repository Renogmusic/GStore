using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GStore.Data;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.ViewModels
{
	public class ValueListEditAdminViewModel
	{
		public ValueListEditAdminViewModel()
		{
		}

		public ValueListEditAdminViewModel(StoreFront storeFront, UserProfile userProfile, ValueList valueList, string activeTab, bool isStoreAdminEdit = false, bool isReadOnly = false, bool isDeletePage = false, bool isCreatePage = false, string sortBy = "", bool? sortAscending = true)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (valueList == null)
			{
				throw new ArgumentNullException("valueList", "valueList cannot be null");
			}
			this.IsStoreAdminEdit = isStoreAdminEdit;
			this.IsActiveDirect = valueList.IsActiveDirect();
			this.IsActiveBubble = valueList.IsActiveBubble();
			this.IsReadOnly = isReadOnly;
			this.IsDeletePage = isDeletePage;
			this.IsCreatePage = isCreatePage;
			this.ActiveTab = activeTab;
			this.SortBy = sortBy;
			this.SortAscending = sortAscending;
			LoadValues(storeFront, userProfile, valueList);
		}

		protected void LoadValues(StoreFront storeFront, UserProfile userProfile, ValueList valueList)
		{
			if (valueList == null)
			{
				return;
			}

			this.ValueList = valueList;
			this.Client = valueList.Client;
			this.ClientId = (valueList.Client == null ? 0 : valueList.ClientId);
			this.CreateDateTimeUtc = valueList.CreateDateTimeUtc;
			this.CreatedBy = valueList.CreatedBy;
			this.CreatedBy_UserProfileId = valueList.CreatedBy_UserProfileId;
			this.Description = valueList.Description;
			this.EndDateTimeUtc = valueList.EndDateTimeUtc;
			this.IsPending = valueList.IsPending;
			this.Name = valueList.Name;
			this.Order = valueList.Order;
			this.StartDateTimeUtc = valueList.StartDateTimeUtc;
			this.UpdateDateTimeUtc = valueList.UpdateDateTimeUtc;
			this.UpdatedBy = valueList.UpdatedBy;
			this.UpdatedBy_UserProfileId = valueList.UpdatedBy_UserProfileId;
			this.ValueListItems = valueList.ValueListItems;
			this.ValueListId = valueList.ValueListId;
			this._valueListItemEditAdminViewModels = null;

		}

		[Editable(false)]
		[Display(Name = "Value List", Description = "")]
		public ValueList ValueList { get; protected set; }

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

		[Required]
		[MaxLength(200)]
		[Display(Name = "Description", Description = "Description of this list for internal reference")]
		public string Description { get; set; }

		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this form to go INACTIVE on. \nIf this date is in the past, your form will not show on the page \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to Inactivate a Form immediately. \nIf checked, this form will not be shown on any pages.")]
		public bool IsPending { get; set; }

		[Required]
		[Display(Name = "Name", Description = "Name of the form for internal purposes.")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index in form list for this form. \nUse this to move a form up or down on your list.")]
		public int Order { get; set; }
		
		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this form to be ACTIVE on. \nIf this date is in the future, your form will not show on the page \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Updated On", Description = "Date and time this form was last updated.")]
		public DateTime UpdateDateTimeUtc { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By", Description = "The user that last updated this form.")]
		public UserProfile UpdatedBy { get; protected set; }

		[Editable(false)]
		[Display(Name = "Updated By User Id", Description = "The user ID of the user that last updated this form.")]
		public int UpdatedBy_UserProfileId { get; protected set; }

		[Display(Name = "Value List Id", Description = "Value List Id number.")]
		public int ValueListId { get; set; }

		[Editable(false)]
		[Display(Name = "Value List Items", Description = "Value List Items for this List.")]
		public ICollection<ValueListItem> ValueListItems { get; protected set; }

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

		public List<ValueListItemEditAdminViewModel> ValueListItemEditAdminViewModels()
		{
			if (_valueListItemEditAdminViewModels != null)
			{
				return _valueListItemEditAdminViewModels;
			}
			if (this.ValueListItems == null)
			{
				return new List<ValueListItemEditAdminViewModel>();
			}

			_valueListItemEditAdminViewModels = this.ValueListItems.AsQueryable().ApplyDefaultSort()
				.Select(vl => new ValueListItemEditAdminViewModel(this, vl, 0)).ToList();

			int counter = 0;
			foreach (var field in _valueListItemEditAdminViewModels)
			{
				field.Index = counter;
				counter++;
			}
			return _valueListItemEditAdminViewModels;
		}
		protected List<ValueListItemEditAdminViewModel> _valueListItemEditAdminViewModels = null;

	}
}