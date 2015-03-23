

namespace GStoreData.ListProvider
{
	/// <summary>
	/// Partially implemented LIST based unit of work (DBContext equivalent) good for mocks, testing, and in-memory data access
	/// </summary>
	public class ListContext : IGstoreDb
	{
		public IGStoreRepository<Models.BadRequest> BadRequests { get { return new GenericGStoreListSourceRepository<Models.BadRequest>(); } }
		public IGStoreRepository<Models.Cart> Carts { get { return new GenericGStoreListSourceRepository<Models.Cart>(); } }
		public IGStoreRepository<Models.CartBundle> CartBundles { get { return new GenericGStoreListSourceRepository<Models.CartBundle>(); } }
		public IGStoreRepository<Models.CartItem> CartItems { get { return new GenericGStoreListSourceRepository<Models.CartItem>(); } }
		public IGStoreRepository<Models.CartPaymentInfo> CartPaymentInfos { get { return new GenericGStoreListSourceRepository<Models.CartPaymentInfo>(); } }
		public IGStoreRepository<Models.Client> Clients { get { return new GenericGStoreListSourceRepository<Models.Client>(); } }
		public IGStoreRepository<Models.ClientRole> ClientRoles { get { return new GenericGStoreListSourceRepository<Models.ClientRole>(); } }
		public IGStoreRepository<Models.ClientRoleAction> ClientRoleActions { get { return new GenericGStoreListSourceRepository<Models.ClientRoleAction>(); } }
		public IGStoreRepository<Models.ClientUserRole> ClientUserRoles { get { return new GenericGStoreListSourceRepository<Models.ClientUserRole>(); } }
		public IGStoreRepository<Models.DeliveryInfoDigital> DeliveryInfoDigitals { get { return new GenericGStoreListSourceRepository<Models.DeliveryInfoDigital>(); } }
		public IGStoreRepository<Models.DeliveryInfoShipping> DeliveryInfoShippings { get { return new GenericGStoreListSourceRepository<Models.DeliveryInfoShipping>(); } }
		public IGStoreRepository<Models.Discount> Discounts { get { return new GenericGStoreListSourceRepository<Models.Discount>(); } }
		public IGStoreRepository<Models.EmailSent> EmailsSent { get { return new GenericGStoreListSourceRepository<Models.EmailSent>(); } }
		public IGStoreRepository<Models.FileNotFoundLog> FileNotFoundLogs { get { return new GenericGStoreListSourceRepository<Models.FileNotFoundLog>(); } }
		public IGStoreRepository<Models.GiftCard> GiftCards { get { return new GenericGStoreListSourceRepository<Models.GiftCard>(); } }
		public IGStoreRepository<Models.NavBarItem> NavBarItems { get { return new GenericGStoreListSourceRepository<Models.NavBarItem>(); } }
		public IGStoreRepository<Models.Notification> Notifications { get { return new GenericGStoreListSourceRepository<Models.Notification>(); } }
		public IGStoreRepository<Models.NotificationLink> NotificationLinks { get { return new GenericGStoreListSourceRepository<Models.NotificationLink>(); } }
		public IGStoreRepository<Models.Order> Orders { get { return new GenericGStoreListSourceRepository<Models.Order>(); } }
		public IGStoreRepository<Models.OrderBundle> OrderBundles { get { return new GenericGStoreListSourceRepository<Models.OrderBundle>(); } }
		public IGStoreRepository<Models.OrderItem> OrderItems { get { return new GenericGStoreListSourceRepository<Models.OrderItem>(); } }
		public IGStoreRepository<Models.Page> Pages { get { return new GenericGStoreListSourceRepository<Models.Page>(); } }
		public IGStoreRepository<Models.PageSection> PageSections { get { return new GenericGStoreListSourceRepository<Models.PageSection>(); } }
		public IGStoreRepository<Models.PageTemplate> PageTemplates { get { return new GenericGStoreListSourceRepository<Models.PageTemplate>(); } }
		public IGStoreRepository<Models.PageTemplateSection> PageTemplateSections { get { return new GenericGStoreListSourceRepository<Models.PageTemplateSection>(); } }
		public IGStoreRepository<Models.PageViewEvent> PageViewEvents { get { return new GenericGStoreListSourceRepository<Models.PageViewEvent>(); } }
		public IGStoreRepository<Models.Payment> Payments { get { return new GenericGStoreListSourceRepository<Models.Payment>(); } }
		public IGStoreRepository<Models.Product> Products { get { return new GenericGStoreListSourceRepository<Models.Product>(); } }
		public IGStoreRepository<Models.ProductBundle> ProductBundles { get { return new GenericGStoreListSourceRepository<Models.ProductBundle>(); } }
		public IGStoreRepository<Models.ProductBundleItem> ProductBundleItems { get { return new GenericGStoreListSourceRepository<Models.ProductBundleItem>(); } }
		public IGStoreRepository<Models.ProductCategory> ProductCategories { get { return new GenericGStoreListSourceRepository<Models.ProductCategory>(); } }
		public IGStoreRepository<Models.SecurityEvent> SecurityEvents { get { return new GenericGStoreListSourceRepository<Models.SecurityEvent>(); } }
		public IGStoreRepository<Models.SmsSent> SmssSent { get { return new GenericGStoreListSourceRepository<Models.SmsSent>(); } }
		public IGStoreRepository<Models.StoreFront> StoreFronts { get { return new GenericGStoreListSourceRepository<Models.StoreFront>(); } }
		public IGStoreRepository<Models.StoreFrontConfiguration> StoreFrontConfigurations { get { return new GenericGStoreListSourceRepository<Models.StoreFrontConfiguration>(); } }
		public IGStoreRepository<Models.StoreBinding> StoreBindings { get { return new GenericGStoreListSourceRepository<Models.StoreBinding>(); } }
		public IGStoreRepository<Models.SystemEvent> SystemEvents { get { return new GenericGStoreListSourceRepository<Models.SystemEvent>(); } }
		public IGStoreRepository<Models.Theme> Themes { get { return new GenericGStoreListSourceRepository<Models.Theme>(); } }
		public IGStoreRepository<Models.UserActionEvent> UserActionEvents { get { return new GenericGStoreListSourceRepository<Models.UserActionEvent>(); } }
		public IGStoreRepository<Models.UserProfile> UserProfiles { get { return new GenericGStoreListSourceRepository<Models.UserProfile>(); } }
		public IGStoreRepository<Models.ValueList> ValueLists { get { return new GenericGStoreListSourceRepository<Models.ValueList>(); } }
		public IGStoreRepository<Models.ValueListItem> ValueListItems { get { return new GenericGStoreListSourceRepository<Models.ValueListItem>(); } }
		public IGStoreRepository<Models.WebForm> WebForms { get { return new GenericGStoreListSourceRepository<Models.WebForm>(); } }
		public IGStoreRepository<Models.WebFormField> WebFormFields { get { return new GenericGStoreListSourceRepository<Models.WebFormField>(); } }
		public IGStoreRepository<Models.WebFormFieldResponse> WebFormFieldResponses { get { return new GenericGStoreListSourceRepository<Models.WebFormFieldResponse>(); } }
		public IGStoreRepository<Models.WebFormResponse> WebFormResponses { get { return new GenericGStoreListSourceRepository<Models.WebFormResponse>(); } }
	
		public void Initialize(bool force)
		{
			//does nothing
		}

		public string UserName { get; set; }
		public Models.StoreFront CachedStoreFront { get; set; }
		public Models.StoreFrontConfiguration CachedStoreFrontConfig { get; set; }

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

		public ListContext(string userName, Models.StoreFront cachedStoreFront, Models.StoreFrontConfiguration cachedStoreFrontConfig, Models.UserProfile cachedUserProfile)
		{
			UserName = userName;
			CachedStoreFront = cachedStoreFront;
			CachedStoreFrontConfig = cachedStoreFrontConfig;
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
			return new ListContext(UserName, CachedStoreFront, CachedStoreFrontConfig, CachedUserProfile);
		}

		public IGstoreDb NewContext(string userName)
		{
			//use same context for all lists until commit and separate contexts are coded
			return new ListContext(userName, CachedStoreFront, CachedStoreFrontConfig, CachedUserProfile);
		}

		public IGstoreDb NewContext(string userName, Models.StoreFront cachedStoreFront, Models.StoreFrontConfiguration cachedStoreFrontConfig, Models.UserProfile cachedUserProfile)
		{
			//use same context for all lists until commit and separate contexts are coded
			return new ListContext(userName, cachedStoreFront, cachedStoreFrontConfig, cachedUserProfile);
		}


		public TEntity Refresh<TEntity>(TEntity entity) where TEntity : Models.BaseClasses.GStoreEntity, new()
		{
			//does nothing
			return entity;
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

		public int SaveChangesEx(bool updateAuditableRecords, bool runEmailNotifications, bool runSmsNotifications, bool updateCategoryCounts) { return 0; }

		private void CreateRepositories()
		{
			//nothing to do in this, lists are static members for each listsource repository; keep across instances
		}
	}
}