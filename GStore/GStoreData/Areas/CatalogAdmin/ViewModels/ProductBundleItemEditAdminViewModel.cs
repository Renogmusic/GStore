using System;
using System.ComponentModel.DataAnnotations;
using GStoreData.Models;

namespace GStoreData.Areas.CatalogAdmin.ViewModels
{
	public class ProductBundleItemEditAdminViewModel
	{
		public ProductBundleItemEditAdminViewModel()
		{
		}

		public ProductBundleItemEditAdminViewModel(ProductBundleItem productBundleItem, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (productBundleItem == null)
			{
				throw new ArgumentNullException("productBundleItem");
			}
			LoadValues(userProfile, productBundleItem);
		}

		protected void LoadValues(UserProfile userProfile, ProductBundleItem productBundleItem)
		{
			if (productBundleItem == null)
			{
				return;
			}

			this.IsActiveDirect = productBundleItem.IsActiveDirect();
			this.IsActiveBubble = productBundleItem.IsActiveBubble();

			this.ProductBundleItemId = productBundleItem.ProductBundleItemId;

			this.ProductBundle = productBundleItem.ProductBundle;
			this.ProductBundleItem = productBundleItem;

			this.Order = productBundleItem.Order;
			this.Quantity = productBundleItem.Quantity;
			this.ProductVariantInfo = productBundleItem.ProductVariantInfo;

			this.BaseUnitPrice = productBundleItem.BaseUnitPrice;
			this.BaseListPrice = productBundleItem.BaseListPrice;

			this.IsPending = productBundleItem.IsPending;
			this.EndDateTimeUtc = productBundleItem.EndDateTimeUtc;
			this.StartDateTimeUtc = productBundleItem.StartDateTimeUtc;
		}

		[Required]
		[Key]
		[Display(Name = "Product Bundle Item Id", Description = "internal id number for the Product Bundle Item")]
		public int ProductBundleItemId { get; set; }

		[Editable(false)]
		[Display(Name = "ProductBundle", Description = "")]
		public ProductBundle ProductBundle { get; protected set; }

		[Editable(false)]
		[Display(Name = "ProductBundleItem", Description = "")]
		public ProductBundleItem ProductBundleItem { get; protected set; }

		[Required]
		[Display(Name = "Order", Description = "Index in the menu for this item. \nUse this to move a Product Bundle up or down on the list.")]
		public int Order { get; set; }

		[Required]
		[Range(0, 1000)]
		[Display(Name = "Max Quantity Per Order or 0 for no limit", Description = "Maximum number of this product Bundle that can be ordered in one order, or 0 for no limit.")]
		public int Quantity { get; set; }

		[Display(Name = "Product Variant Info", Description = "Variant information for this product.")]
		public string ProductVariantInfo { get; set; }

		
		[Display(Name = "Unit Price", Description = "Price each for this item in the bundle.\nNote: if quantity is 2 or more per bundle, the system will multiply this price by the quantity to determine unit price for the bundle.")]
		public decimal? BaseUnitPrice { get; set; }

		[Display(Name = "List Price", Description = "List Price each for this item in the bundle.\nNote: if quantity is 2 or more per bundle, the system will multiply this price by the quantity to determine list price for the bundle.")]
		public decimal? BaseListPrice { get; set; }

		#region StoreFrontRecord fields

		[Display(Name = "Inactive", Description = "Check this box to Inactivate a Product Bundle immediately. \nIf checked, this Product Bundle will not be shown on any pages.")]
		public bool IsPending { get; set; }

		[Display(Name = "Start Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Product Bundle to be ACTIVE on. \nIf this date is in the future, your Product Bundle will not show on the page \nExample: 1/1/2000 12:00 PM")]
		public DateTime StartDateTimeUtc { get; set; }

		[Display(Name = "End Date and Time in UTC", Description = "Enter the date and time in UTC time you want this Product Bundle to go INACTIVE on. \nIf this date is in the past, your Product Bundle will not show on the page \nExample: 12/31/2199 11:59 PM")]
		public DateTime EndDateTimeUtc { get; set; }

		[Editable(false)]
		[Display(Name = "Is Active Direct", Description = "If checked, this record is currently active. If unchecked, this record is NOT active.")]
		public bool IsActiveDirect { get; protected set; }

		[Editable(false)]
		[Display(Name = "Is Active Bubble", Description = "If checked, this record and its related records are currently active. If unchecked, this record or its parent records are NOT active.")]
		public bool IsActiveBubble { get; protected set; }

		#endregion

	}
}