using System;
using System.Collections.Generic;
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
		IGstoreDb NewContext(string userName, Models.StoreFront cachedStoreFront, Models.UserProfile cachedUserProfile);

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

		IGStoreRepository<Models.BadRequest> BadRequests { get; }
		IGStoreRepository<Models.Client> Clients { get; }
		IGStoreRepository<Models.ClientRole> ClientRoles { get; }
		IGStoreRepository<Models.ClientRoleAction> ClientRoleActions { get; }
		IGStoreRepository<Models.ClientUserRole> ClientUserRoles { get; }
		IGStoreRepository<Models.FileNotFoundLog> FileNotFoundLogs { get; }
		IGStoreRepository<Models.NavBarItem> NavBarItems { get; }
		IGStoreRepository<Models.Notification> Notifications { get; }
		IGStoreRepository<Models.NotificationLink> NotificationLinks { get; }
		IGStoreRepository<Models.Page> Pages { get; }
		IGStoreRepository<Models.PageSection> PageSections { get; }
		IGStoreRepository<Models.PageTemplate> PageTemplates { get; }
		IGStoreRepository<Models.PageTemplateSection> PageTemplateSections { get; }
		IGStoreRepository<Models.PageViewEvent> PageViewEvents { get; }
		IGStoreRepository<Models.Product> Products { get; }
		IGStoreRepository<Models.ProductCategory> ProductCategories { get; }
		IGStoreRepository<Models.SecurityEvent> SecurityEvents { get; }
		IGStoreRepository<Models.StoreFront> StoreFronts { get; }
		IGStoreRepository<Models.StoreBinding> StoreBindings { get; }
		IGStoreRepository<Models.SystemEvent> SystemEvents { get; }
		IGStoreRepository<Models.Theme> Themes { get; }
		IGStoreRepository<Models.UserActionEvent> UserActionEvents { get; }
		IGStoreRepository<Models.UserProfile> UserProfiles { get; }
		IGStoreRepository<Models.ValueList> ValueLists { get; }
		IGStoreRepository<Models.ValueListItem> ValueListItems { get; }
		IGStoreRepository<Models.WebForm> WebForms { get; }
		IGStoreRepository<Models.WebFormField> WebFormFields { get; }
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
		/// Deletes an entity, runs a DBSet.Find (by id), then DBSet.Remove
		/// </summary>
		/// <param name="entity"></param>
		bool Delete(TEntity entity, bool throwErrorIfNotFound = false);

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