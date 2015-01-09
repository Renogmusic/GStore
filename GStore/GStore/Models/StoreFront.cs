using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GStore.Models
{
	[Table("StoreFront")]
	public class StoreFront : BaseClasses.ClientRecord
	{
		#region Basic Fields

		[Key]
		[Display(Name = "Store Front Id")]
		public int StoreFrontId { get; set; }

		[Display(Name = "Order")]
		public int Order { get; set; }

		[Display(Name = "Next Order Number")]
		public int NextOrderNumber { get; set; }

		#endregion

		#region Child Record Navigation Properties

		[Display(Name = "Configurations")]
		public virtual ICollection<StoreFrontConfiguration> StoreFrontConfigurations { get; set; }

		[Display(Name = "Shopping Carts")]
		public virtual ICollection<Cart> Carts { get; set; }

		[Display(Name = "Discounts")]
		public virtual ICollection<Discount> Discounts { get; set; }

		[Display(Name = "Nav Bar Items")]
		public virtual ICollection<NavBarItem> NavBarItems { get; set; }

		[Display(Name = "Orders")]
		public virtual ICollection<Order> Orders { get; set; }

		[Display(Name = "Pages")]
		public virtual ICollection<Page> Pages { get; set; }

		[Display(Name = "Products")]
		public virtual ICollection<Product> Products { get; set; }

		[Display(Name = "Product Categories")]
		public virtual ICollection<ProductCategory> ProductCategories { get; set; }

		[Display(Name = "Store Bindings")]
		public virtual ICollection<StoreBinding> StoreBindings { get; set; }

		[Display(Name = "User Profiles")]
		public virtual ICollection<UserProfile> UserProfiles { get; set; }

		[Display(Name = "Notifications")]
		public virtual ICollection<Notification> Notifications { get; set; }

		[Display(Name = "Web Form Responses")]
		public virtual ICollection<WebFormResponse> WebFormResponses { get; set; }

		#endregion

	}
}