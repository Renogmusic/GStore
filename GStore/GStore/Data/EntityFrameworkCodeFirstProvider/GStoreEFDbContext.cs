namespace GStore.Data.EntityFrameworkCodeFirstProvider
{
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.Data.Entity;
	using System.Data.Entity.Infrastructure;
	using System.Data.Entity.Validation;
	using System.Linq;
	using System.Diagnostics;
	using System.Security.Principal;
	using System.Collections.Generic;
	using GStore.Models;
	using GStore.Data;
	using System.Data.Entity.ModelConfiguration.Conventions;

	public partial class GStoreEFDbContext : DbContext, IGstoreDb
	{
		#region Table DBSets and repository interface

		//Tables: note table name is used from attribute on class model to create tables
		//Interface; for repository if new tables are added, be sure to add them to repository interface

		public virtual DbSet<BadRequest> BadRequestsTable { get; set; }
		public IGStoreRepository<Models.BadRequest> BadRequests { get { return new GenericGStoreEFEntity<Models.BadRequest>(this); } }

		public virtual DbSet<Client> ClientsTable { get; set; }
		public IGStoreRepository<Models.Client> Clients { get { return new GenericGStoreEFEntity<Models.Client>(this); } }

		public virtual DbSet<ClientRole> ClientRolesTable { get; set; }
		public IGStoreRepository<Models.ClientRole> ClientRoles { get { return new GenericGStoreEFEntity<Models.ClientRole>(this); } }

		public virtual DbSet<ClientRoleAction> ClientRoleActionsTable { get; set; }
		public IGStoreRepository<Models.ClientRoleAction> ClientRoleActions { get { return new GenericGStoreEFEntity<Models.ClientRoleAction>(this); } }
		
		public virtual DbSet<ClientUserRole> ClientUserRolesTable { get; set; }
		public IGStoreRepository<Models.ClientUserRole> ClientUserRoles { get { return new GenericGStoreEFEntity<Models.ClientUserRole>(this); } }
		
		public virtual DbSet<FileNotFoundLog> FileNotFoundLogsTable { get; set; }
		public IGStoreRepository<Models.FileNotFoundLog> FileNotFoundLogs { get { return new GenericGStoreEFEntity<Models.FileNotFoundLog>(this); } }
		
		public virtual DbSet<NavBarItem> NavBarItemsTable { get; set; }
		public IGStoreRepository<Models.NavBarItem> NavBarItems { get { return new GenericGStoreEFEntity<Models.NavBarItem>(this); } }
		
		public virtual DbSet<Notification> NotificationsTable { get; set; }
		public IGStoreRepository<Models.Notification> Notifications { get { return new GenericGStoreEFEntity<Models.Notification>(this); } }
		
		public virtual DbSet<NotificationLink> NotificationLinksTable { get; set; }
		public IGStoreRepository<Models.NotificationLink> NotificationLinks { get { return new GenericGStoreEFEntity<Models.NotificationLink>(this); } }
		
		public virtual DbSet<Page> PagesTable { get; set; }
		public IGStoreRepository<Models.Page> Pages { get { return new GenericGStoreEFEntity<Models.Page>(this); } }
		
		public virtual DbSet<PageSection> PageSectionsTable { get; set; }
		public IGStoreRepository<Models.PageSection> PageSections { get { return new GenericGStoreEFEntity<Models.PageSection>(this); } }
		
		public virtual DbSet<PageTemplate> PageTemplatesTable { get; set; }
		public IGStoreRepository<Models.PageTemplate> PageTemplates { get { return new GenericGStoreEFEntity<Models.PageTemplate>(this); } }
		
		public virtual DbSet<PageTemplateSection> PageTemplateSectionsTable { get; set; }
		public IGStoreRepository<Models.PageTemplateSection> PageTemplateSections { get { return new GenericGStoreEFEntity<Models.PageTemplateSection>(this); } }
		
		public virtual DbSet<PageViewEvent> PageViewEventsTable { get; set; }
		public IGStoreRepository<Models.PageViewEvent> PageViewEvents { get { return new GenericGStoreEFEntity<Models.PageViewEvent>(this); } }
		
		public virtual DbSet<Product> ProductsTable { get; set; }
		public IGStoreRepository<Models.Product> Products { get { return new GenericGStoreEFEntity<Models.Product>(this); } }
		
		public virtual DbSet<ProductCategory> ProductCategoriesTable { get; set; }
		public IGStoreRepository<Models.ProductCategory> ProductCategories { get { return new GenericGStoreEFEntity<Models.ProductCategory>(this); } }
		
		public virtual DbSet<SecurityEvent> SecurityEventsTable { get; set; }
		public IGStoreRepository<Models.SecurityEvent> SecurityEvents { get { return new GenericGStoreEFEntity<Models.SecurityEvent>(this); } }

		public virtual DbSet<StoreBinding> StoreBindingsTable { get; set; }
		public IGStoreRepository<Models.StoreBinding> StoreBindings { get { return new GenericGStoreEFEntity<Models.StoreBinding>(this); } }

		public virtual DbSet<StoreFront> StoreFrontsTable { get; set; }
		public IGStoreRepository<Models.StoreFront> StoreFronts { get { return new GenericGStoreEFEntity<Models.StoreFront>(this); } }

		public virtual DbSet<SystemEvent> SystemEventsTable { get; set; }
		public IGStoreRepository<Models.SystemEvent> SystemEvents { get { return new GenericGStoreEFEntity<Models.SystemEvent>(this); } }
		
		public virtual DbSet<Theme> ThemesTable { get; set; }
		public IGStoreRepository<Models.Theme> Themes { get { return new GenericGStoreEFEntity<Models.Theme>(this); } }
		
		public virtual DbSet<UserActionEvent> UserActionEventsTable { get; set; }
		public IGStoreRepository<Models.UserActionEvent> UserActionEvents { get { return new GenericGStoreEFEntity<Models.UserActionEvent>(this); } }
		
		public virtual DbSet<UserProfile> UserProfilesTable { get; set; }
		public IGStoreRepository<Models.UserProfile> UserProfiles { get { return new GenericGStoreEFEntity<Models.UserProfile>(this); } }

		public virtual DbSet<ValueList> ValueListsTable { get; set; }
		public IGStoreRepository<Models.ValueList> ValueLists { get { return new GenericGStoreEFEntity<Models.ValueList>(this); } }

		public virtual DbSet<ValueListItem> ValueListItemsTable { get; set; }
		public IGStoreRepository<Models.ValueListItem> ValueListItems { get { return new GenericGStoreEFEntity<Models.ValueListItem>(this); } }

		#endregion

		#region IGStoreDb Repository Interface

		public IGstoreDb NewContext()
		{
			return new GStoreEFDbContext(UserName, CachedStoreFront, CachedUserProfile);
		}

		/// <summary>
		/// Creates a new db context with a specified user name, erases user cache on the new context
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public IGstoreDb NewContext(string userName)
		{
			return new GStoreEFDbContext(userName, CachedStoreFront, null);
		}

		/// <summary>
		/// Creates a new db context using specified user name, cached store front and cached user profile to pre-load cache
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="cachedStoreFront"></param>
		/// <param name="cachedUserProfile"></param>
		/// <returns></returns>
		public IGstoreDb NewContext(string userName, Models.StoreFront cachedStoreFront, Models.UserProfile cachedUserProfile)
		{
			return new GStoreEFDbContext(userName, cachedStoreFront, cachedUserProfile);
		}

		public IGstoreDb GStoreDb { get { return this; } }

		public TEntity Refresh<TEntity>(TEntity entity) where TEntity : Models.BaseClasses.GStoreEntity, new()
		{
			this.Entry(entity).Reload();
			return entity;
		}

		#endregion


		public string UserName 
		{ 
			get
			{
				return _userName;
			}
			set
			{
				_userName = value;
				_cachedUserProfile = null;
			}
		}
		protected string _userName = string.Empty;

		public Models.StoreFront CachedStoreFront { get; set; }

		private UserProfile _cachedUserProfile = null;
		public Models.UserProfile CachedUserProfile 
		{
			get { return _cachedUserProfile; }
			set
			{ 
				_cachedUserProfile = value; 
				if (_cachedUserProfile != null)
				{
					UserName = _cachedUserProfile.UserName;
				}
			}
		}

		public GStoreEFDbContext()
			: base("name=GStore.Properties.Settings.GStoreDB")
		{
		}

		public GStoreEFDbContext(IPrincipal user)
			: base("name=GStore.Properties.Settings.GStoreDB")
		{
			this.UserName = user.Identity.Name;
		}

		public GStoreEFDbContext(string userName)
			: base("name=GStore.Properties.Settings.GStoreDB")
		{
			this.UserName = userName;
		}

		public GStoreEFDbContext(string userName, StoreFront cachedStoreFront, UserProfile cachedUserProfile)
			: base("name=GStore.Properties.Settings.GStoreDB")
		{
			this.UserName = userName;
			this.CachedStoreFront = cachedStoreFront;
			this.CachedUserProfile = cachedUserProfile;
		}


		/// <summary>
		/// Saves changes, but does NOT update auditable records or send notifications for new notifications (used for updating fields without affecting updatedate/updateuserid)
		/// </summary>
		/// <returns></returns>
		public int SaveChangesDirect()
		{
			return SaveChangesEx(false, false, false, false);
		}

		/// <summary>
		/// Saves changes and updates auditable records with updated audit datetime and user id
		/// </summary>
		/// <returns></returns>
		public override int SaveChanges()
		{
			return SaveChangesEx(true, true, true, true);
		}

		/// <summary>
		/// Runs Savechanges on the context, plus other automatic options, defaults are all true
		/// </summary>
		/// <param name="updateAuditableRecords"></param>
		/// <param name="runEmailNotifications"></param>
		/// <param name="runSmsNotifications"></param>
		/// <param name="updateCategoryCounts"></param>
		/// <returns></returns>
		public int SaveChangesEx(bool updateAuditableRecords, bool runEmailNotifications, bool runSmsNotifications, bool updateCategoryCounts)
		{
			ChangeTracker.DetectChanges();
			List<Notification> notificationsToProcess = new List<Notification>();
			List<StoreFront> storeFrontsToRecalculate = new List<StoreFront>();

			if (ChangeTracker.HasChanges())
			{
				foreach (DbEntityEntry item in ChangeTracker.Entries())
				{
					if (updateAuditableRecords && (item.State == EntityState.Added || item.State == EntityState.Modified) && item.Entity is Models.BaseClasses.AuditFieldsAllRequired)
					{
						Models.BaseClasses.AuditFieldsAllRequired record = item.Entity as Models.BaseClasses.AuditFieldsAllRequired;
						UserProfile userProfile = this.GetCurrentUserProfile(true,true);
						record.UpdateAuditFields(userProfile);
					}

					if (updateAuditableRecords && (item.State == EntityState.Added || item.State == EntityState.Modified) && item.Entity is Models.BaseClasses.AuditFieldsUserProfileOptional)
					{
						Models.BaseClasses.AuditFieldsUserProfileOptional recordOptional = item.Entity as Models.BaseClasses.AuditFieldsUserProfileOptional;
						UserProfile userProfileOptional = this.GetCurrentUserProfile(false, false);
						recordOptional.UpdateAuditFields(userProfileOptional);
					}

					if (updateCategoryCounts && item.Entity is Models.Product)
					{
						StoreFront storeFront = ((Product)item.Entity).StoreFront;
						if (!storeFrontsToRecalculate.Contains(storeFront))
						{
							storeFrontsToRecalculate.Add(storeFront);
						}
					}

					if (updateCategoryCounts && item.Entity is Models.ProductCategory)
					{
						StoreFront storeFront = ((ProductCategory)item.Entity).StoreFront;
						if (!storeFrontsToRecalculate.Contains(storeFront))
						{
							storeFrontsToRecalculate.Add(storeFront);
						}
					}

					//new notification, process email and sms notification 
					if (item.Entity is Notification && item.State == EntityState.Added && (runEmailNotifications || runSmsNotifications))
					{
						notificationsToProcess.Add(item.Entity as Notification);
					}
				}
			}

			int returnValue = -1;
			try
			{
				returnValue = base.SaveChanges();
			}
			catch (DbEntityValidationException ex)
			{

				string errorDetails = "Error saving to database.";
				if (ex.EntityValidationErrors != null && ex.EntityValidationErrors.Count() > 0)
				{
					errorDetails += "\nEntity Errors: " + ex.EntityValidationErrors.Count();
					foreach (DbEntityValidationResult valError in ex.EntityValidationErrors)
					{
						DbEntityEntry errorRow = valError.Entry;
						if (errorRow != null)
						{
							errorDetails += "\n\tEntity:" + errorRow.Entity.ToString();
						}
						else
						{
							errorDetails += "\n\tUnknown Entity";
						}
						errorDetails += " (" + errorRow.State.ToString() + ")";
						errorDetails += " Errors: " + valError.ValidationErrors.Count();

						foreach (DbValidationError dbValError in valError.ValidationErrors)
						{
							errorDetails += "\n\t\tError in " + dbValError.PropertyName + ": " + dbValError.ErrorMessage;
						}

						foreach (string propName in valError.Entry.CurrentValues.PropertyNames)
						{
							object fieldObjectValue = valError.Entry.CurrentValues[propName];
							string fieldValue = (fieldObjectValue == null ? "(null)" : fieldObjectValue.ToString());
							errorDetails += "\n\t\t" + propName + " = " + fieldValue;
						}
					}
				}
				errorDetails += " \n BaseException: " + ex.GetBaseException().ToString();
				ApplicationException exNew = new ApplicationException(errorDetails, ex);
				Debug.WriteLine(errorDetails);
				throw exNew;
			}
			catch (DbUpdateException exUpdate)
			{
				string errorDetails = "Error updating database.";
				if (exUpdate.Entries != null && exUpdate.Entries.Count() > 0)
				{
					errorDetails += "\nEntity Error Entries: " + exUpdate.Entries.Count();
					foreach (DbEntityEntry errorEntry in exUpdate.Entries)
					{
						if (errorEntry.Entity != null)
						{
							errorDetails += "\n\tEntity:" + errorEntry.Entity.ToString();
						}
						else
						{
							errorDetails += "\n\tUnknown Entity";
						}
						errorDetails += " (" + errorEntry.State.ToString() + ")";
					}

				}

				errorDetails += " \n BaseException: " + exUpdate.GetBaseException().ToString();
				ApplicationException exNew = new ApplicationException(errorDetails, exUpdate);
				Debug.WriteLine(errorDetails);
				throw exNew;
			}
			catch(Exception ex)
			{
				if (ex is System.Data.SqlClient.SqlException || ex.GetBaseException() is System.Data.SqlClient.SqlException)
				{
					throw new Exceptions.DatabaseErrorException("SQL Client Error in SaveChangesEx: " + ex.Message, ex);
				}
				else
				{
					throw new Exceptions.DatabaseErrorException("Unknown Database Error in SaveChangesEx: " + ex.Message, ex);
				}
			}

			if (notificationsToProcess.Count != 0)
			{
				foreach (Notification item in notificationsToProcess)
				{
					Data.StoreFrontExtensions.ProcessEmailAndSmsNotifications(this, item, runEmailNotifications, runSmsNotifications);
				}
			}

			if (storeFrontsToRecalculate.Count != 0)
			{
				foreach (StoreFront item in storeFrontsToRecalculate)
				{
					Data.StoreFrontExtensions.RecalculateProductCategoryActiveCount(this, item);
				}
			}

			return returnValue;

		}

		public System.Data.Entity.DbSet<GStore.Areas.StoreAdmin.ViewModels.StoreFrontConfigViewModel> StoreFrontConfigViewModels { get; set; }

	}
}