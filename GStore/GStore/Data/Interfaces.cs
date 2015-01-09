using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace GStore.Data
{
	/// <summary>
	/// Repository interface for DB and list Dbcontext and Unit of Work abstractions for a DBContext or mock
	/// </summary>
	public interface IGstoreDb
	{
		/// <summary>
		/// User name of the current user for audit fields
		/// </summary>
		string UserName { get; set; }

		/// <summary>
		/// Get/Set Cached value of the current store front, null if no value. Used for extension methods to avoid repeated lookups for StoreFront
		/// </summary>
		Models.StoreFront CachedStoreFront { get; set; }

		/// <summary>
		/// Get/Set Cached value of the current store front, null if no value. Used for extension methods to avoid repeated lookups for StoreFront
		/// </summary>
		Models.StoreFrontConfiguration CachedStoreFrontConfig { get; set; }

		/// <summary>
		/// Get/Set Cached value of the current user profile, null if no value. Used for extension methods to avoid repeated lookups for UserProfile
		/// </summary>
		Models.UserProfile CachedUserProfile { get; set; }

		
		/// <summary>
		/// Forces an entity to update from the database. DOES NOT UPDATE NAVIGATION PROPERTIES!
		/// If you have an issue with navigation properties, be sure you're using the same context to add and view data
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		TEntity Refresh<TEntity>(TEntity entity) where TEntity : Models.BaseClasses.GStoreEntity, new();

		/// <summary>
		/// Creates a new Unit of work (Context) with current user name
		/// </summary>
		/// <returns></returns>
		IGstoreDb NewContext();

		/// <summary>
		/// Creates a new Unit of work (Context) with a specific user name
		/// </summary>
		/// <returns></returns>
		IGstoreDb NewContext(string userName);

		/// <summary>
		/// Creates a new Unit of work (Context) with a specific user name and client id and storefrontid
		/// </summary>
		/// <returns></returns>
		IGstoreDb NewContext(string userName, Models.StoreFront cachedStoreFront, Models.StoreFrontConfiguration cachedStoreFrontConfig, Models.UserProfile cachedUserProfile);

		/// <summary>
		/// Saves changes to the context, similar to DBContext.SaveChanges.  This will process notifications and Before and After triggers to update records and audit fields
		/// </summary>
		/// <returns></returns>
		int SaveChanges();
		
		/// <summary>
		/// Saves changes to the context, but does not run any before or after triggers, and does not update audit fields or process notifications
		/// </summary>
		/// <returns></returns>
		int SaveChangesDirect();

		/// <summary>
		/// Extended SaveChanges call with parameters to decide which before and after triggers will run
		/// </summary>
		/// <param name="updateAuditableRecords">If true, will update all auditable records in the context with current user and datetime</param>
		/// <param name="runEmailNotifications">If true, will process notifications and send emails to the users necessary</param>
		/// <param name="runSmsNotifications">If true, will process SMS notifications and send SMS text messages to the users necessary</param>
		/// <param name="updateCategoryCounts">If true, will recalculate category active product counts when a catagory or product is updated</param>
		/// <returns></returns>
		int SaveChangesEx(bool updateAuditableRecords, bool runEmailNotifications, bool runSmsNotifications, bool updateCategoryCounts);

		/// <summary>
		/// Calls the database initializer even if it has been called before
		/// </summary>
		void Initialize(bool force);

		[Display(Name = "Bad Requests", Description = "Bad and Invalid Requests (HTTP Errors thrown by MVC actions) system-wide")]
		IGStoreRepository<Models.BadRequest> BadRequests { get; }

		[Display(Name = "Carts", Description = "Shopping Carts for a store front.")]
		IGStoreRepository<Models.Cart> Carts { get; }

		[Display(Name = "Cart Items", Description = "Products in shopping carts for a store front.")]
		IGStoreRepository<Models.CartItem> CartItems { get; }

		[Display(Name = "Clients", Description = "Main Record in database. \nIndividual entities that can be tracked like an 'account' or 'company' for billing.")]
		IGStoreRepository<Models.Client> Clients { get; }

		[Display(Name = "Client Roles", Description = "Named 'user groups' for assigning permissions for a client.")]
		IGStoreRepository<Models.ClientRole> ClientRoles { get; }

		[Display(Name = "Client Role Actions", Description = "Individual permissions assigned to Roles for a client.")]
		IGStoreRepository<Models.ClientRoleAction> ClientRoleActions { get; }

		[Display(Name = "Client User Roles", Description = "Individual User assignments to a Client Role for a client.")]
		IGStoreRepository<Models.ClientUserRole> ClientUserRoles { get; }

		[Display(Name = "Delivery Info Digital", Description = "Delivery Info entered at checkout for digital delivery for a store front.")]
		IGStoreRepository<Models.DeliveryInfoDigital> DeliveryInfoDigitals { get; }

		[Display(Name = "Delivery Info Shipping", Description = "Delivery Info entered at checkout for shipping for a store front.")]
		IGStoreRepository<Models.DeliveryInfoShipping> DeliveryInfoShippings { get; }

		[Display(Name = "Discounts", Description = "Discount Codes and Coupon Codes for a store front.")]
		IGStoreRepository<Models.Discount> Discounts { get; }

		[Display(Name = "File Not Found Logs", Description = "File Not Found (404) errors including MVC and HTTP Application 404 errors system-wide.")]
		IGStoreRepository<Models.FileNotFoundLog> FileNotFoundLogs { get; }

		[Display(Name = "Nav Bar Items", Description = "Site Menu Items for a store front.")]
		IGStoreRepository<Models.NavBarItem> NavBarItems { get; }

		[Display(Name = "Notifications", Description = "Notifications (messages) in the site messaging system for a store front.")]
		IGStoreRepository<Models.Notification> Notifications { get; }

		[Display(Name = "Notification Links", Description = "Links within Notification messages for a store front.")]
		IGStoreRepository<Models.NotificationLink> NotificationLinks { get; }

		[Display(Name = "Orders", Description = "Orders placed for a store front.")]
		IGStoreRepository<Models.Order> Orders { get; }

		[Display(Name = "Order Items", Description = "Items (products) in an order for a store front.")]
		IGStoreRepository<Models.OrderItem> OrderItems { get; }

		[Display(Name = "Pages", Description = "Dynamic Web Pages for a store front.")]
		IGStoreRepository<Models.Page> Pages { get; }

		[Display(Name = "Page Sections", Description = "Dynamic Web Page content sections for a store front.")]
		IGStoreRepository<Models.PageSection> PageSections { get; }

		[Display(Name = "Page Templates", Description = "Dynamic Web Page templates for a client.")]
		IGStoreRepository<Models.PageTemplate> PageTemplates { get; }

		[Display(Name = "Page Template Sections", Description = "Dynamic Web Page Template sections for a client.")]
		IGStoreRepository<Models.PageTemplateSection> PageTemplateSections { get; }

		[Display(Name = "Page View Events", Description = "Page Views logged (if page view logging is enabled) system-wide.")]
		IGStoreRepository<Models.PageViewEvent> PageViewEvents { get; }

		[Display(Name = "Payments", Description = "Payment Info entered at checkout in an order for a store front.")]
		IGStoreRepository<Models.Payment> Payments { get; }

		[Display(Name = "Products", Description = "Sellable Products for a store front.")]
		IGStoreRepository<Models.Product> Products { get; }

		[Display(Name = "Product Categories", Description = "Category groups for menus and listing of Products for a store front.")]
		IGStoreRepository<Models.ProductCategory> ProductCategories { get; }

		[Display(Name = "Product Reviews", Description = "Product reviews by users for a store front.")]
		IGStoreRepository<Models.ProductReview> ProductReviews { get; }

		[Display(Name = "Security Events", Description = "Security events like login/logoff/lockout/new user system-wide.")]
		IGStoreRepository<Models.SecurityEvent> SecurityEvents { get; }

		[Display(Name = "Store Fronts", Description = "Store Front - main record that web sites are centered on linked to a client.")]
		IGStoreRepository<Models.StoreFront> StoreFronts { get; }

		[Display(Name = "Store Front Configurations", Description = "Long list of settings (and multiple configurations) for a store front.")]
		IGStoreRepository<Models.StoreFrontConfiguration> StoreFrontConfigurations { get; }

		[Display(Name = "Store Bindings", Description = "HostName, Port, Application Path and Folder Name settings to map incoming requests for a store front.")]
		IGStoreRepository<Models.StoreBinding> StoreBindings { get; }

		[Display(Name = "System Events", Description = "System events like system errors system-wide.")]
		IGStoreRepository<Models.SystemEvent> SystemEvents { get; }

		[Display(Name = "Themes", Description = "Bootstrap CSS Themes for a client.")]
		IGStoreRepository<Models.Theme> Themes { get; }

		[Display(Name = "User Action Events", Description = "Events logged for certain user actions system-wide.")]
		IGStoreRepository<Models.UserActionEvent> UserActionEvents { get; }

		[Display(Name = "User Profiles", Description = "User Profiles and settings in addition to login/password in AspNetUser for a store front.")]
		IGStoreRepository<Models.UserProfile> UserProfiles { get; }

		[Display(Name = "Value Lists", Description = "Lists of values for selection boxes for a client.")]
		IGStoreRepository<Models.ValueList> ValueLists { get; }

		[Display(Name = "Value List Items", Description = "Individual values of Value Lists in selection boxes for a client.")]
		IGStoreRepository<Models.ValueListItem> ValueListItems { get; }

		[Display(Name = "Web Forms", Description = "Web Forms that allow user input on a dynamic page for a client.")]
		IGStoreRepository<Models.WebForm> WebForms { get; }

		[Display(Name = "Web Form Fields", Description = "Individual fields on Web Forms for a client.")]
		IGStoreRepository<Models.WebFormField> WebFormFields { get; }

		[Display(Name = "Web Form Field Responses", Description = "Responses to an individual field on a web form response for a store front.")]
		IGStoreRepository<Models.WebFormFieldResponse> WebFormFieldResponses { get; }

		[Display(Name = "Web Form Responses", Description = "Response to a web form for a store front.")]
		IGStoreRepository<Models.WebFormResponse> WebFormResponses { get; }
	}

	/// <summary>
	/// Generic interface for GStore entities to common interfaces for repository objects (tables)
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IGStoreRepository<TEntity> where TEntity : Models.BaseClasses.GStoreEntity, new()
	{
		/// <summary>
		/// Returns true if this table is empty (runs a .Any and returns true if no values exist, false if there are values in the table) 
		/// </summary>
		/// <returns></returns>
		bool IsEmpty();

		/// <summary>
		/// Creates a new entity object, does not add it to context (call .Add to add to context when done setting values). Returns a dynamic proxy
		/// </summary>
		/// <returns></returns>
		TEntity Create();

		/// <summary>
		/// Creates a new entity from a viewmodel or an entity POCO and maps property names and returns a dynamic proxy
		/// </summary>
		/// <param name="valuesToCopy"></param>
		/// <returns></returns>
		TEntity Create(TEntity valuesToCopy);

		/// <summary>
		/// Adds a new entity to the context, returns a dynamic proxy
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		TEntity Add(TEntity entity);

		/// <summary>
		/// Finds an entity by the id value (key) similar to dbcontext.find
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		TEntity FindById(int id);

		/// <summary>
		/// Deletes an entity from the context by it's id value, runs a find by id, then remove from the dbset
		/// </summary>
		/// <param name="id"></param>
		/// <param name="throwErrorIfNotFound"></param>
		bool DeleteById(int id, bool throwErrorIfNotFound = false);

		/// <summary>
		/// Deletes an entity from the context by it's id value, runs a find by id, then remove from the dbset
		/// </summary>
		/// <param name="id"></param>
		/// <param name="throwErrorIfNotFound"></param>
		bool Delete(TEntity entity, bool throwErrorIfNotFound = false);

		/// <summary>
		/// Deletes a range of entities calling DBSet.RemoveRange
		/// </summary>
		/// <param name="entity"></param>
		bool DeleteRange(IEnumerable<TEntity> entities);

		/// <summary>
		/// Marks an entity as modified, similar to marking db context entry as modified, returns object updated (will convert a POCO to a dynamic proxy)
		/// </summary>
		/// <param name="entity"></param>
		TEntity Update(TEntity entity);

		/// <summary>
		/// Returns a single entity or throws an error if none is found or more than one is found
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TEntity Single(Expression<Func<TEntity, bool>> predicate);

		/// <summary>
		/// Returns a single entity or default (null) like SingleOrDefault
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

		/// <summary>
		/// IQueryable .Where function
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

		/// <summary>
		/// IQueryable set of all values, use this when all values are needed for a table or you need to apply custom LINQ queries
		/// </summary>
		/// <returns></returns>
		IQueryable<TEntity> All();
		
		/// <summary>
		/// Runs adds an .Include call for each parameter to add tables to the queryable result set
		/// </summary>
		/// <param name="includeProperties"></param>
		/// <returns></returns>
		IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

		void SetKeyFieldValue(TEntity entity, int id);

		int GetKeyFieldValue(TEntity entity);

		string KeyFieldPropertyName(TEntity entity);
	}
}