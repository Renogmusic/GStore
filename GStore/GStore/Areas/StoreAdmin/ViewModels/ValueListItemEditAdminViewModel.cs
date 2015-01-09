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
	public class ValueListItemEditAdminViewModel
	{
		public ValueListItemEditAdminViewModel()
		{
			//model binder will use this option
			SetDefaults(null);
		}

		public ValueListItemEditAdminViewModel(ValueListEditAdminViewModel valueListViewModel, ValueListItem valueListItem, int index)
		{
			if (valueListViewModel == null)
			{
				throw new ArgumentNullException("valueListViewModel");
			}

			if (valueListViewModel.ValueList == null)
			{
				throw new ArgumentNullException("valueListViewModel.ValueList", "valueListViewModel.ValueList cannot be null. make sure edit page sets this after post");
			}

			this.Index = index;

			this.ValueListEditAdminViewModel = valueListViewModel;
			this.ValueList = valueListViewModel.ValueList;
			this.ValueListId = valueListViewModel.ValueList.ValueListId;

			this.IsStoreAdminEdit = valueListViewModel.IsStoreAdminEdit;
			this.IsReadOnly = valueListViewModel.IsReadOnly;
			this.IsDeletePage = valueListViewModel.IsDeletePage;
			this.IsCreatePage = valueListViewModel.IsCreatePage;
			this.ActiveTab = valueListViewModel.ActiveTab;

			FillListsIfEmpty(valueListItem.Client);

			if (valueListItem == null)
			{
				SetDefaults(valueListViewModel);
			}
			else
			if (valueListItem != null)
			{
				this.Name = valueListItem.Name;
				this.Order = valueListItem.Order;
				this.Description = valueListItem.Description;
				this.IsInteger = valueListItem.IsInteger;
				this.IntegerValue = valueListItem.IntegerValue;
				this.IsString = valueListItem.IsString;
				this.StringValue = valueListItem.StringValue;
				this.ValueListItemId = valueListItem.ValueListItemId;

				this.IsPending = valueListItem.IsPending;
				this.StartDateTimeUtc = valueListItem.StartDateTimeUtc;
				this.EndDateTimeUtc = valueListItem.EndDateTimeUtc;
				this.IsActiveDirect = valueListItem.IsActiveDirect();
				this.IsActiveBubble = valueListItem.IsActiveBubble();
			}
		}

		protected void SetDefaults(ValueListEditAdminViewModel valueListViewModel)
		{
			if (valueListViewModel == null)
			{
				return;
			}
			this.Name = "New Item";
			this.Description = "New Item";
			this.Order = (valueListViewModel.ValueListItems.Count == 0 ? 100 : valueListViewModel.ValueListItems.Max(vl => vl.Order) + 10);

			this.IsString = true;
			this.StringValue = "New Item Value";
			this.IsPending = false;
			this.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			this.StartDateTimeUtc = DateTime.UtcNow.AddYears(100);
		}

		public void FillListsIfEmpty(Client client)
		{
		}

		[Editable(false)]
		public bool IsReadOnly { get; set; }

		[Editable(false)]
		public bool IsDeletePage { get; set; }

		[Editable(false)]
		public bool IsCreatePage { get; set; }

		[Editable(false)]
		public string ActiveTab { get; set; }

		public int Index { get; set; }

		[Key]
		[Display(Name = "Value List Item Id", Description="Internal Id for List Item")]
		public int ValueListItemId { get; set; }

		[Editable(false)]
		[Required]
		[Display(Name = "Value List Id", Description="Internal Value List Id of the list this item is on")]
		public int ValueListId { get; set; }

		[Editable(false)]
		[Display(Name = "Value List", Description = "Value List this item is on")]
		public ValueList ValueList { get; protected set; }

		[Editable(false)]
		[Display(Name = "Value List View Model", Description = "Value List this item is on")]
		public ValueListEditAdminViewModel ValueListEditAdminViewModel { get; protected set; }
	
		[Required]
		[Display(Name = "Name", Description="Name of item for internal reference")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index number for this item in the list")]
		public int Order { get; set; }

		[Required]
		[MaxLength(200)]
		[Display(Name = "Description", Description = "Internal description of this item for your reference")]
		public string Description { get; set; }

		[Display(Name = "Is Number", Description = "Check this box if this item is a number value")]
		public bool IsInteger { get; set; }

		[Display(Name = "Number Value", Description = "Number Value of this item")]
		public int? IntegerValue { get; set; }
	
		[Display(Name = "Is Text", Description = "Check this box if this item is a text value")]
		public bool IsString { get; set; }

		[Display(Name = "String Value", Description = "Text Value of this item")]
		public string StringValue { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to make this Item Inactive immediately")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date and Time Utc", Description = "Enter the time in UTC time to start using this item \nExample: 1/1/2000 12:00 AM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Display(Name = "End Date and Time Utc", Description = "Enter the time in UTC time to stop using this item \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Editable(false)]
		public bool IsStoreAdminEdit { get; set; }

		[Editable(false)]
		[Display(Name = "Status", Description = "If status is INACTIVE, this item will not show on any lists.")]
		public bool IsActiveDirect { get; set; }

		[Display(Name = "Status Including Parents", Description = "If status is INACTIVE, this item will not show on any lists.")]
		public bool IsActiveBubble { get; set; }

	}
}