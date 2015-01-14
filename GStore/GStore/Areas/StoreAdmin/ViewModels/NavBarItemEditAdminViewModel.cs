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
	public class NavBarItemEditAdminViewModel: IValidatableObject
	{
		public NavBarItemEditAdminViewModel()
		{
		}

		public NavBarItemEditAdminViewModel(NavBarItem navBarItem, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (navBarItem == null)
			{
				throw new ArgumentNullException("navBarItem", "Nav Bar Item cannot be null");
			}
			LoadValues(userProfile, navBarItem);
		}

		public NavBarItemEditAdminViewModel(NavBarItem navBarItem, UserProfile userProfile, string activeTab, bool isCreatePage = false, bool isSimpleCreatePage = false, bool isEditPage = false, bool isDetailsPage = false, bool isDeletePage = false, bool returnToManager = false)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (navBarItem == null)
			{
				throw new ArgumentNullException("navBarItem", "Nav Bar Item cannot be null");
			}
			this.IsCreatePage = isCreatePage;
			this.IsSimpleCreatePage = isSimpleCreatePage;
			this.IsEditPage = isEditPage;
			this.IsDetailsPage = isDetailsPage;
			this.IsDeletePage = isDeletePage;
			this.ReturnToManager = returnToManager;

			LoadValues(userProfile, navBarItem);
		}

		protected void LoadValues(UserProfile userProfile, NavBarItem navBarItem)
		{
			if (navBarItem == null)
			{
				return;
			}
			this.IsActiveDirect = navBarItem.IsActiveDirect();
			this.IsActiveBubble = navBarItem.IsActiveBubble();

			this.Action = navBarItem.Action;
			this.ActionIdParam = navBarItem.ActionIdParam;
			this.Area = navBarItem.Area;
			this.Controller = navBarItem.Controller;
			this.Client = navBarItem.Client;
			this.ClientId = (navBarItem.Client == null ? 0 : navBarItem.ClientId);
			this.CreateDateTimeUtc = navBarItem.CreateDateTimeUtc;
			this.CreatedBy = navBarItem.CreatedBy;
			this.CreatedBy_UserProfileId = navBarItem.CreatedBy_UserProfileId;
			this.EndDateTimeUtc = navBarItem.EndDateTimeUtc;
			this.ForRegisteredOnly = navBarItem.ForRegisteredOnly;
			this.ForAnonymousOnly = navBarItem.ForAnonymousOnly;
			this.htmlAttributes = navBarItem.htmlAttributes;
			this.IsAction = navBarItem.IsAction;
			this.IsLocalHRef = navBarItem.IsLocalHRef;
			this.IsPage = navBarItem.IsPage;
			this.IsPending = navBarItem.IsPending;
			this.IsRemoteHRef = navBarItem.IsRemoteHRef;
			this.LocalHRef = navBarItem.LocalHRef;
			this.Name = navBarItem.Name;
			this.NavBarItemId = navBarItem.NavBarItemId;
			this.OpenInNewWindow = navBarItem.OpenInNewWindow;
			this.Order = navBarItem.Order;
			this.Page = navBarItem.Page;
			this.PageId = navBarItem.PageId;
			this.ParentNavBarItem = navBarItem.ParentNavBarItem;
			this.ParentNavBarItemId = navBarItem.ParentNavBarItemId;
			this.RemoteHRef = navBarItem.RemoteHRef;
			this.StartDateTimeUtc = navBarItem.StartDateTimeUtc;
			this.StoreFront = navBarItem.StoreFront;
			this.StoreFrontId = navBarItem.StoreFrontId;
			this.UpdateDateTimeUtc = navBarItem.UpdateDateTimeUtc;
			this.UpdatedBy = navBarItem.UpdatedBy;
			this.UpdatedBy_UserProfileId = navBarItem.UpdatedBy_UserProfileId;
			this.UseDividerAfterOnMenu = navBarItem.UseDividerAfterOnMenu;
			this.UseDividerBeforeOnMenu = navBarItem.UseDividerBeforeOnMenu;

		}

		public void FillListsIfEmpty(Client client, StoreFront storeFront)
		{

		}

		public void UpdateParentNavBarItem(NavBarItem navBarItem)
		{
			this.ParentNavBarItem = navBarItem;
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

		[Editable(false)]
		public bool ReturnToManager { get; set; }


		[Required]
		[Key]
		[Display(Name = "Menu Item Id", Description = "internal id number for the menu item")]
		public int NavBarItemId { get; set; }

		[Editable(false)]
		[Display(Name = "Menu Item", Description = "")]
		public NavBarItem NavBarItem { get; protected set; }

		[Display(Name = "For Registered Users Only", Description = "Check this box to make this menu item appear only for registered users")]
		public bool ForRegisteredOnly { get; set; }

		[Display(Name = "For Anonymous Users Only", Description = "Check this box to make this menu item appear only for anonymous users")]
		public bool ForAnonymousOnly { get; set; }

		[Display(Name = "HTML Attributes", Description = "HTML Attributes for this menu item")]
		public string htmlAttributes { get; set; }

		[Required]
		[Display(Name = "Name", Description = "Unique name of the menu item. This name appears on the menu as text.")]
		public string Name { get; set; }

		[Display(Name = "Open In a New Window", Description = "Check this box to open a new window when this menu item is clicked.")]
		public bool OpenInNewWindow { get; set; }

		[Required]
		[Display(Name = "Order", Description = "Index in the menu for this item. \nUse this to move a menu item up or down on the list.")]
		public int Order { get; set; }

		[Display(Name = "Parent Menu Item", Description = "Parent Menu item; use this to make an item into a sub-menu of another item.")]
		public NavBarItem ParentNavBarItem { get; protected set; }

		[Display(Name = "Parent Menu Item Id", Description = "Parent Menu item Id; use this to make an item into a sub-menu of another item.")]
		public int? ParentNavBarItemId { get; set; }

		[Display(Name = "Add Divider Before", Description = "Check this box to add a divider before this item in a dropdown menu.")]
		public bool UseDividerAfterOnMenu { get; set; }

		[Display(Name = "Add Divider After", Description = "Check this box to add a divider after this item in a dropdown menu.")]
		public bool UseDividerBeforeOnMenu { get; set; }


		[Display(Name = "Is MVC Action", Description = "Check this box to link this menu item to run an MVC action")]
		public bool IsAction { get; set; }

		[Display(Name = "MVC Action Name", Description = "MVC Action Name when Is MVC Action is checked")]
		public string Action { get; set; }

		[Display(Name = "MVC Action Id Parameter", Description = "MVC Action Id parameter when Is MVC Action is checked")]
		public int? ActionIdParam { get; set; }

		[Display(Name = "MVC Area", Description = "Area Name or blank when Is MVC Action is checked")]
		public string Area { get; set; }

		[Display(Name = "MVC Controller", Description = "MVC Controller Name or blank when Is MVC Action is checked")]
		public string Controller { get; set; }


		[Display(Name = "Is Local Link", Description = "Check this box to link this menu item to a local URL")]
		public bool IsLocalHRef { get; set; }

		[Display(Name = "Local URL", Description = "If Is Local Link is checked, enter the local URL starting with '/' \nExample: /myimage.png")]
		public string LocalHRef { get; set; }

	
		[Display(Name = "Is Page Link", Description = "Check this box to link this menu item to a site page")]
		public bool IsPage { get; set; }

		[Display(Name = "Page", Description = "If Is Page Link is checked, select the page this menu item will link to")]
		public Page Page { get; protected set; }

		[Display(Name = "Page Id", Description = "If Is Page Link is checked, select the page this menu item will link to")]
		public int? PageId { get; set; }


		[Display(Name = "Is Remote Link", Description = "Check this box to link this menu item to a URL on another web site.")]
		public bool IsRemoteHRef { get; set; }

		[Url]
		[Display(Name = "Remote URL", Description = "If Is Remote Link is checked, enter the full URL \nExample: http://www.google.com")]
		public string RemoteHRef { get; set; }


		#region StoreFrontRecord fields

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

		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this form to go INACTIVE on. \nIf this date is in the past, your form will not show on the page \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Display(Name = "Inactive", Description = "Check this box to Inactivate a Form immediately. \nIf checked, this form will not be shown on any pages.")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this form to be ACTIVE on. \nIf this date is in the future, your form will not show on the page \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Store Front", Description = "Store Front for this Menu Item")]
		public StoreFront StoreFront { get; protected set; }

		[Editable(false)]
		[Display(Name = "Store Front Id", Description = "Store Front for this Menu Item")]
		public int StoreFrontId { get; protected set; }

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
		public bool IsActiveDirect { get; set; }

		[Editable(false)]
		public bool IsActiveBubble { get; set; }

		#endregion


		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> result = new List<ValidationResult>();
			string checkboxName = null;
			string fieldName = null;
			string fieldDisplay = null;

			if (!this.IsAction && !this.IsLocalHRef && !this.IsPage && !this.IsRemoteHRef)
			{
				string isActionLabel = this.GetDisplayName("IsAction");
				string isLocalHRefLabel = this.GetDisplayName("IsLocalHRef");
				string isPageLabel = this.GetDisplayName("IsPage");
				string isRemoteHRefLabel = this.GetDisplayName("IsRemoteHRef");

				result.Add(new ValidationResult("You must select one of the following: '" + isActionLabel + "', '" + isLocalHRefLabel + "', '" + isPageLabel + "', or '" + isRemoteHRefLabel + "'.", new string[] { "IsAction", "IsLocalHRef", "IsPage", "IsRemoteHRef" }));
			}

			if (this.IsAction)
			{
				checkboxName = this.GetDisplayName("IsAction");
				if (string.IsNullOrWhiteSpace(this.Action))
				{
					fieldName = "Action";
					fieldDisplay = this.GetDisplayName(fieldName);
					result.Add(new ValidationResult(fieldDisplay + " is required when '" + checkboxName + "' is checked", new string[] { fieldName }));
				}
				if (string.IsNullOrWhiteSpace(this.Controller))
				{
					fieldName = "Controller";
					fieldDisplay = this.GetDisplayName(fieldName);
					result.Add(new ValidationResult(fieldDisplay + " is required when '" + checkboxName + "' is checked", new string[] { fieldName }));
				}
			}

			if (this.IsLocalHRef)
			{
				checkboxName = this.GetDisplayName("IsLocalHRef");
				if (string.IsNullOrWhiteSpace(this.LocalHRef))
				{
					fieldName = "LocalHRef";
					fieldDisplay = this.GetDisplayName(fieldName);
					result.Add(new ValidationResult(fieldDisplay + " is required when '" + checkboxName + "' is checked", new string[] { fieldName }));
				}
				else if (!this.LocalHRef.StartsWith("/"))
				{
					fieldName = "LocalHRef";
					fieldDisplay = this.GetDisplayName(fieldName);
					result.Add(new ValidationResult(fieldDisplay + " must start with a slash '/' character", new string[] { fieldName }));
				}

			}

			if (!this.IsSimpleCreatePage && this.IsPage)
			{
				checkboxName = this.GetDisplayName("IsPage");
				if (!this.PageId.HasValue || this.PageId.Value == 0)
				{
					fieldName = "PageId";
					fieldDisplay = this.GetDisplayName(fieldName);
					result.Add(new ValidationResult(fieldDisplay + " is required when '" + checkboxName + "' is checked", new string[] { fieldName }));
				}
			}

			if (this.IsSimpleCreatePage && this.IsPage)
			{
				checkboxName = this.GetDisplayName("IsPage");
				if (!this.PageId.HasValue || this.PageId.Value == 0)
				{
					fieldName = "PageId";
					fieldDisplay = this.GetDisplayName(fieldName);
					result.Add(new ValidationResult(fieldDisplay + " is required", new string[] { fieldName }));
				}
			}

			if (this.IsRemoteHRef)
			{
				checkboxName = this.GetDisplayName("IsRemoteHRef");
				if (string.IsNullOrWhiteSpace(this.RemoteHRef))
				{
					fieldName = "RemoteHRef";
					fieldDisplay = this.GetDisplayName(fieldName);
					result.Add(new ValidationResult(fieldDisplay + " is required when '" + checkboxName + "' is checked", new string[] { fieldName }));
				}
			}

			if (this.ForAnonymousOnly && this.ForRegisteredOnly)
			{
				string anonymousLabel = this.GetDisplayName("ForAnonymousOnly");
				string forRegisteredLabel = this.GetDisplayName("ForRegisteredOnly");
				result.Add(new ValidationResult("You can only select '" + anonymousLabel + "' or '" + forRegisteredLabel + "' or none, but not both", new string[] { "ForAnonymousOnly", "ForRegisteredOnly"}));
			}

			return result;
		}
	}
}