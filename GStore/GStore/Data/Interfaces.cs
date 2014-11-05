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
		string UserName { get; set; }
		Models.StoreFront CachedStoreFront { get; set; }
		Models.UserProfile CachedUserProfile { get; set; }

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

		int SaveChanges();
		int SaveChangesDirect();
		int SaveChangesEx(bool updateAuditableRecords, bool runEmailNotifications, bool runSmsNotifications, bool updateCategoryCounts);

		IGStoreRepository<Models.BadRequest> BadRequests { get; }
		IGStoreRepository<Models.Client> Clients { get; }
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
		IGStoreRepository<Models.StoreFrontUserRole> StoreFrontUserRoles { get; }
		IGStoreRepository<Models.StoreBinding> StoreBindings { get; }
		IGStoreRepository<Models.SystemEvent> SystemEvents { get; }
		IGStoreRepository<Models.Theme> Themes { get; }
		IGStoreRepository<Models.UserActionEvent> UserActionEvents { get; }
		IGStoreRepository<Models.UserProfile> UserProfiles { get; }

	}

	/// <summary>
	/// Generic interface for GStore entities to provide basic starter interfaces for objects
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IGStoreRepository<TEntity> where TEntity : class
	{
		IQueryable<TEntity> All();
		bool IsEmpty();
		IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
		IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
		TEntity Create();
		TEntity FindById(int id);
		TEntity Single(Expression<Func<TEntity, bool>> predicate);
		TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
		TEntity Add(TEntity entity);
		void Delete(TEntity entity);
		void DeleteById(int id, bool throwErrorIfNotFound = false);
		void Update(TEntity entity);
	}


}