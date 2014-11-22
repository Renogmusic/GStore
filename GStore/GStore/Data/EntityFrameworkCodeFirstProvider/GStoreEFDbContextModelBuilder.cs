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
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GStore.Data.EntityFrameworkCodeFirstProvider
{
	public partial class GStoreEFDbContext
	{

		/// <summary>
		/// FluentApi foreign keys: http://blogs.msdn.com/b/adonet/archive/2010/12/06/ef-feature-ctp5-fluent-api-samples.aspx
		/// FluentApi Indexes http://msdn.microsoft.com/en-us/data/jj591617.aspx#PropertyIndex
		/// Data Annotations Indexes http://msdn.microsoft.com/en-us/data/jj591583#Index
		/// Multi-ColumnIndex SO http://stackoverflow.com/questions/21573550/entity-framework-6-setting-unique-constraint-with-fluent-api
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			#region Fix-up navigation properties

			modelBuilder.Entity<UserProfile>()
				.HasOptional(up => up.UpdatedBy)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<UserProfile>()
				.HasOptional(up => up.CreatedBy)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<ClientUserRole>()
				.HasRequired(cur => cur.UserProfile)
				.WithMany(u => u.ClientUserRoles)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<StoreFront>()
				.HasMany(sf => sf.UserProfiles)
				.WithOptional(up => up.StoreFront)
				.HasForeignKey(up => up.StoreFrontId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<StoreFront>()
				.HasRequired(s => s.Client)
				.WithMany(c => c.StoreFronts)
				.HasForeignKey(fk => fk.ClientId)
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

			modelBuilder.Entity<Page>()
				.HasRequired(pg => pg.StoreFront)
				.WithMany(sf => sf.Pages)
				.HasForeignKey(fk => fk.StoreFrontId)
				.WillCascadeOnDelete(false);


			modelBuilder.Entity<Notification>()
				.HasMany(not => not.NotificationLinks)
				.WithRequired(lnk => lnk.Notification)
				.WillCascadeOnDelete(true);

			modelBuilder.Entity<Notification>()
				.HasRequired(n => n.ToUserProfile)
				.WithMany(u => u.Notifications)
				.HasForeignKey(fk => fk.ToUserProfileId)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Notification>()
				.HasRequired(n => n.FromUserProfile)
				.WithMany(u => u.NotificationsSent)
				.HasForeignKey(fk => fk.FromUserProfileId)
				.WillCascadeOnDelete(false);

			#endregion

			#region Compound Unique key fix-up
			/// FluentApi Indexes http://msdn.microsoft.com/en-us/data/jj591617.aspx#PropertyIndex
			/// Data Annotations Indexes http://msdn.microsoft.com/en-us/data/jj591583#Index
			/// Multi-ColumnIndex SO http://stackoverflow.com/questions/21573550/entity-framework-6-setting-unique-constraint-with-fluent-api

			modelBuilder.Entity<StoreFront>()
				.Property(sf => sf.ClientId)
				.IsRequired()
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(new IndexAttribute("IX_ClientId_StoreFrontId", 1) { IsUnique = true }));

			modelBuilder.Entity<StoreFront>()
				.Property(sf => sf.StoreFrontId)
				.IsRequired()
				.HasColumnAnnotation(IndexAnnotation.AnnotationName,
					new IndexAnnotation(new IndexAttribute("IX_ClientId_StoreFrontId", 2) { IsUnique = true }));

			#endregion

			base.OnModelCreating(modelBuilder);

		}
	}
}