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
	using System.Data.Entity.ModelConfiguration.Conventions;

	public class GStoreEFDbContext : DbContext, IGstoreDb
	{
		#region IGStore interface methods

		public IGStoreRepository<Models.BadRequest> BadRequests { get { return new GenericGStoreEFEntity<Models.BadRequest>(this); } }
		public IGStoreRepository<Models.Client> Clients { get { return new GenericGStoreEFEntity<Models.Client>(this); } }
		public IGStoreRepository<Models.ClientUserRole> ClientUserRoles { get { return new GenericGStoreEFEntity<Models.ClientUserRole>(this); } }
		public IGStoreRepository<Models.FileNotFoundLog> FileNotFoundLogs { get { return new GenericGStoreEFEntity<Models.FileNotFoundLog>(this); } }
		public IGStoreRepository<Models.NavBarItem> NavBarItems { get { return new GenericGStoreEFEntity<Models.NavBarItem>(this); } }
		public IGStoreRepository<Models.Notification> Notifications { get { return new GenericGStoreEFEntity<Models.Notification>(this); } }
		public IGStoreRepository<Models.NotificationLink> NotificationLinks { get { return new GenericGStoreEFEntity<Models.NotificationLink>(this); } }
		public IGStoreRepository<Models.Page> Pages { get { return new GenericGStoreEFEntity<Models.Page>(this); } }
		public IGStoreRepository<Models.PageSection> PageSections { get { return new GenericGStoreEFEntity<Models.PageSection>(this); } }
		public IGStoreRepository<Models.PageTemplate> PageTemplates { get { return new GenericGStoreEFEntity<Models.PageTemplate>(this); } }
		public IGStoreRepository<Models.PageTemplateSection> PageTemplateSections { get { return new GenericGStoreEFEntity<Models.PageTemplateSection>(this); } }
		public IGStoreRepository<Models.PageViewEvent> PageViewEvents { get { return new GenericGStoreEFEntity<Models.PageViewEvent>(this); } }
		public IGStoreRepository<Models.Product> Products { get { return new GenericGStoreEFEntity<Models.Product>(this); } }
		public IGStoreRepository<Models.ProductCategory> ProductCategories { get { return new GenericGStoreEFEntity<Models.ProductCategory>(this); } }
		public IGStoreRepository<Models.SecurityEvent> SecurityEvents { get { return new GenericGStoreEFEntity<Models.SecurityEvent>(this); } }
		public IGStoreRepository<Models.StoreBinding> StoreBindings { get { return new GenericGStoreEFEntity<Models.StoreBinding>(this); } }
		public IGStoreRepository<Models.StoreFront> StoreFronts { get { return new GenericGStoreEFEntity<Models.StoreFront>(this); } }
		public IGStoreRepository<Models.StoreFrontUserRole> StoreFrontUserRoles { get { return new GenericGStoreEFEntity<Models.StoreFrontUserRole>(this); } }
		public IGStoreRepository<Models.SystemEvent> SystemEvents { get { return new GenericGStoreEFEntity<Models.SystemEvent>(this); } }
		public IGStoreRepository<Models.Theme> Themes { get { return new GenericGStoreEFEntity<Models.Theme>(this); } }
		public IGStoreRepository<Models.UserActionEvent> UserActionEvents { get { return new GenericGStoreEFEntity<Models.UserActionEvent>(this); } }
		public IGStoreRepository<Models.UserProfile> UserProfiles { get { return new GenericGStoreEFEntity<Models.UserProfile>(this); } }

		#endregion

		#region Table DBSets for model creation

		//Tables: note table name is used from attribute on class model to create tables
		//Interface; for repository if new tables are added, be sure to add them to repository interface
		public virtual DbSet<BadRequest> BadRequestsTable { get; set; }
		public virtual DbSet<Client> ClientsTable { get; set; }
		public virtual DbSet<ClientUserRole> ClientUserRolesTable { get; set; }
		public virtual DbSet<FileNotFoundLog> FileNotFoundLogsTable { get; set; }
		public virtual DbSet<NavBarItem> NavBarItemsTable { get; set; }
		public virtual DbSet<Notification> NotificationsTable { get; set; }
		public virtual DbSet<NotificationLink> NotificationLinksTable { get; set; }
		public virtual DbSet<Page> PagesTable { get; set; }
		public virtual DbSet<PageSection> PageSectionsTable { get; set; }
		public virtual DbSet<PageTemplate> PageTemplatesTable { get; set; }
		public virtual DbSet<PageTemplateSection> PageTemplateSectionsTable { get; set; }
		public virtual DbSet<PageViewEvent> PageViewEventsTable { get; set; }
		public virtual DbSet<Product> ProductsTable { get; set; }
		public virtual DbSet<ProductCategory> ProductCategoriesTable { get; set; }
		public virtual DbSet<SecurityEvent> SecurityEventsTable { get; set; }
		public virtual DbSet<SystemEvent> SystemEventsTable { get; set; }
		public virtual DbSet<StoreBinding> StoreBindingsTable { get; set; }
		public virtual DbSet<StoreFront> StoreFrontsTable { get; set; }
		public virtual DbSet<StoreFrontUserRole> StoreFrontUserRolesTable { get; set; }
		public virtual DbSet<Theme> ThemesTable { get; set; }
		public virtual DbSet<UserActionEvent> UserActionEventsTable { get; set; }
		public virtual DbSet<UserProfile> UserProfilesTable { get; set; }

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

		#endregion


		public string UserName { get; set; }
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

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			modelBuilder.Entity<UserProfile>()
				.HasOptional(up => up.UpdatedBy)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<UserProfile>()
				.HasOptional(up => up.CreatedBy)
				.WithMany()
				.WillCascadeOnDelete(false);

			//modelBuilder.Entity<UserProfile>()
			//	.HasOptional(up => up.CreatedBy)
			//	.WithMany()
			//	.HasForeignKey(up => up.CreatedBy_UserProfileId)
			//	.WillCascadeOnDelete(false);

			//modelBuilder.Entity<UserProfile>()
			//	.HasOptional(up => up.UpdatedBy)
			//	.WithMany()
			//	.HasForeignKey(up => up.UpdatedBy_UserProfileId)
			//	.WillCascadeOnDelete(false);


			//modelBuilder.Entity<StoreFront>()
			//	.HasRequired(s => s.Client)
			//	.WithMany(c => c.StoreFronts)
			//	.HasForeignKey(fk => fk.ClientId)
			//	.WillCascadeOnDelete(false);

			modelBuilder.Entity<StoreFront>()
				.HasMany(sf => sf.UserProfiles)
				.WithOptional(up => up.StoreFront)
				.HasForeignKey(up => up.StoreFrontId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<StoreFront>()
				.HasRequired(s => s.AccountAdmin)
				.WithMany(u => u.AccountAdminStoreFronts)
				.HasForeignKey(fk => fk.AccountAdmin_UserProfileId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<StoreFront>()
				.HasRequired(s => s.RegisteredNotify)
				.WithMany(u => u.RegisteredNotifyStoreFronts)
				.HasForeignKey(fk => fk.RegisteredNotify_UserProfileId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<StoreFront>()
				.HasRequired(s => s.WelcomePerson)
				.WithMany(u => u.WelcomeStoreFronts)
				.HasForeignKey(fk => fk.WelcomePerson_UserProfileId)
				.WillCascadeOnDelete(false);

			base.OnModelCreating(modelBuilder);

			// http://blogs.msdn.com/b/adonet/archive/2010/12/06/ef-feature-ctp5-fluent-api-samples.aspx
			//			Standard one to many:

			//modelBuilder.Entity<Product>() 
			//	.HasRequired(p => p.PrimaryCategory) 
			//	.WithMany(c => c.Products) 
			//	.HasForeignKey(p => p.PrimaryCategoryCode);

			//The same relationship can also be configured from the other end (this has the same effect as the above code):

			//modelBuilder.Entity<Category>() 
			//	.HasMany(c => c.Products) 
			//	.WithRequired(p => p.PrimaryCategory) 
			//	.HasForeignKey(p => p.PrimaryCategoryCode);




			//Relationship with only one navigation property:

			//modelBuilder.Entity<OrderLine>() 
			//	.HasRequired(l => l.Product) 
			//	.WithMany() 
			//	.HasForeignKey(l => l.ProductId); 


			//Switch cascade delete off:

			//modelBuilder.Entity<Category>() 
			//	.HasMany(c => c.Products) 
			//	.WithRequired(p => p.PrimaryCategory) 
			//	.HasForeignKey(p => p.PrimaryCategoryCode) 
			//	.WillCascadeOnDelete(false);


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

					if (updateAuditableRecords && item.Entity is Models.BaseClasses.AuditFieldsAllRequired)
					{
						Models.BaseClasses.AuditFieldsAllRequired record = item.Entity as Models.BaseClasses.AuditFieldsAllRequired;
						UserProfile userProfile = this.CachedUserProfile;
						record.Update(userProfile);
					}

					if (updateAuditableRecords && item.Entity is Models.BaseClasses.AuditFieldsUserProfileOptional)
					{
						Models.BaseClasses.AuditFieldsUserProfileOptional recordOptional = item.Entity as Models.BaseClasses.AuditFieldsUserProfileOptional;
						UserProfile userProfileOptional = this.CachedUserProfile;
						recordOptional.Update(userProfileOptional);
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
					}
				}
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
				ApplicationException exNew = new ApplicationException(errorDetails, exUpdate);
				Debug.WriteLine(errorDetails);
				throw exNew;
			}

			if (notificationsToProcess.Count != 0)
			{
				foreach (Notification item in notificationsToProcess)
				{
					Models.Extensions.NotificationExtensions.ProcessEmailAndSmsNotifications(this, item, runEmailNotifications, runSmsNotifications);
				}
			}

			if (storeFrontsToRecalculate.Count != 0)
			{
				foreach (StoreFront item in storeFrontsToRecalculate)
				{
					Models.Extensions.GStoreDBExtensions.RecalculateProductCategoryActiveCount(this, item);
				}
			}

			return returnValue;

		}

	}
}