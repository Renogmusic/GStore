using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Models.Extensions;

namespace GStore.Data.ListProvider
{
	/// <summary>
	/// Partially implemented LIST based unit of work (DBContext equivalent) good for mocks, testing, and in-memory data access
	/// </summary>
	public class ListContext : IGstoreDb
	{
		public string UserName { get; set; }
		public Models.StoreFront CachedStoreFront { get; set; }

		private Models.UserProfile _cachedUserProfile = null;
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

		//get current store front and clientid
		public ListContext(System.Security.Principal.IPrincipal user)
		{
			UserName = user.Identity.Name;
			CreateRepositories();
		}

		public ListContext(string userName)
		{
			UserName = userName;
			CreateRepositories();
		}

		public ListContext(string userName, Models.StoreFront cachedStoreFront, Models.UserProfile cachedUserProfile)
		{
			UserName = userName;
			CachedStoreFront = cachedStoreFront;
			CachedUserProfile = cachedUserProfile;
			CreateRepositories();
		}

		public ListContext()
		{
			CreateRepositories();
		}

		public IGstoreDb NewContext()
		{
			//use same context for all lists until commit and separate contexts are coded
			return new ListContext(UserName, CachedStoreFront, CachedUserProfile);
		}

		public IGstoreDb NewContext(string userName)
		{
			//use same context for all lists until commit and separate contexts are coded
			return new ListContext(userName, CachedStoreFront, CachedUserProfile);
		}

		public IGstoreDb NewContext(string userName, Models.StoreFront cachedStoreFront, Models.UserProfile cachedUserProfile)
		{
			//use same context for all lists until commit and separate contexts are coded
			return new ListContext(userName, cachedStoreFront, cachedUserProfile);
		}

		/// <summary>
		/// Changes are merged as they are added in real time, no "Commit" built in yet, no processing of notifications built in yet
		/// </summary>
		/// <returns></returns>
		public int SaveChanges() { return 0; }

		/// <summary>
		/// Changes are merged as they are added in real time, no "Commit" built in yet, no processing of notifications built in yet
		/// </summary>
		/// <returns></returns>
		public int SaveChangesDirect() { return 0; }

		private void CreateRepositories()
		{
			//nothing to do in this, lists are static members for each listsource repository; keep across instances
		}

		public IGStoreRepository<Models.BadRequest> BadRequests { get { return new GenericGStoreListSourceRepository<Models.BadRequest>(); } }
		public IGStoreRepository<Models.Client> Clients { get { return new GenericGStoreListSourceRepository<Models.Client>(); } }
		public IGStoreRepository<Models.FileNotFoundLog> FileNotFoundLogs { get { return new GenericGStoreListSourceRepository<Models.FileNotFoundLog>(); } }
		public IGStoreRepository<Models.Notification> Notifications { get { return new GenericGStoreListSourceRepository<Models.Notification>(); } }
		public IGStoreRepository<Models.NotificationLink> NotificationLinks { get { return new GenericGStoreListSourceRepository<Models.NotificationLink>(); } }
		public IGStoreRepository<Models.Page> Pages { get { return new GenericGStoreListSourceRepository<Models.Page>(); } }
		public IGStoreRepository<Models.PageTemplate> PageTemplates { get { return new GenericGStoreListSourceRepository<Models.PageTemplate>(); } }
		public IGStoreRepository<Models.PageTemplateSection> PageTemplateSections { get { return new GenericGStoreListSourceRepository<Models.PageTemplateSection>(); } }
		public IGStoreRepository<Models.PageViewEvent> PageViewEvents { get { return new GenericGStoreListSourceRepository<Models.PageViewEvent>(); } }
		public IGStoreRepository<Models.SecurityEvent> SecurityEvents { get { return new GenericGStoreListSourceRepository<Models.SecurityEvent>(); } }
		public IGStoreRepository<Models.StoreFront> StoreFronts { get { return new GenericGStoreListSourceRepository<Models.StoreFront>(); } }
		public IGStoreRepository<Models.StoreBinding> StoreBindings { get { return new GenericGStoreListSourceRepository<Models.StoreBinding>(); } }
		public IGStoreRepository<Models.SystemEvent> SystemEvents { get { return new GenericGStoreListSourceRepository<Models.SystemEvent>(); } }
		public IGStoreRepository<Models.Theme> Themes { get { return new GenericGStoreListSourceRepository<Models.Theme>(); } }
		public IGStoreRepository<Models.UserActionEvent> UserActionEvents { get { return new GenericGStoreListSourceRepository<Models.UserActionEvent>(); } }
		public IGStoreRepository<Models.UserProfile> UserProfiles { get { return new GenericGStoreListSourceRepository<Models.UserProfile>(); } }
	
	}
}