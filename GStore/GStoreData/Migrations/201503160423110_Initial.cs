namespace GStoreData.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BadRequest",
                c => new
                    {
                        BadRequestId = c.Int(nullable: false, identity: true),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BadRequestId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 255),
                        UserName = c.String(nullable: false, maxLength: 255),
                        Email = c.String(nullable: false, maxLength: 255),
                        StoreFrontId = c.Int(),
                        ClientId = c.Int(),
                        Order = c.Int(nullable: false),
                        FullName = c.String(nullable: false),
                        SignupNotes = c.String(),
                        EntryUrl = c.String(),
                        EntryRawUrl = c.String(),
                        EntryReferrer = c.String(),
                        EntryDateTime = c.DateTime(nullable: false),
                        TimeZoneId = c.String(),
                        RegisterWebFormResponseId = c.Int(),
                        SendMoreInfoToEmail = c.Boolean(nullable: false),
                        AddressLine1 = c.String(maxLength: 100),
                        AddressLine2 = c.String(maxLength: 100),
                        City = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 12),
                        CountryCode = c.Int(),
                        AllowUsersToSendSiteMessages = c.Boolean(nullable: false),
                        NotifyAllWhenLoggedOn = c.Boolean(nullable: false),
                        NotifyOfSiteUpdatesToEmail = c.Boolean(nullable: false),
                        SubscribeToNewsletterEmail = c.Boolean(nullable: false),
                        SendSiteMessagesToEmail = c.Boolean(nullable: false),
                        SendSiteMessagesToSms = c.Boolean(nullable: false),
                        LastLogonDateTimeUtc = c.DateTime(),
                        SentMoreInfoToEmailDateTimeUtc = c.DateTime(),
                        LastSiteUpdateSentToEmailDateTimeUtc = c.DateTime(),
                        LastNewsletterSentToEmailDateTimeUtc = c.DateTime(),
                        LastSiteMessageSentToEmailDateTimeUtc = c.DateTime(),
                        LastLockoutFailureNoticeDateTimeUtc = c.DateTime(),
                        LastOrderAdminVisitDateTimeUtc = c.DateTime(),
                        LastCatalogAdminVisitDateTimeUtc = c.DateTime(),
                        LastStoreAdminVisitDateTimeUtc = c.DateTime(),
                        LastSystemAdminVisitDateTimeUtc = c.DateTime(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.WebFormResponse", t => t.RegisterWebFormResponseId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.UserId, unique: true, name: "UniqueRecord")
                .Index(t => t.UserName, unique: true)
                .Index(t => t.Email, unique: true)
                .Index(t => t.StoreFrontId)
                .Index(t => t.ClientId)
                .Index(t => t.RegisterWebFormResponseId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.StoreFrontConfiguration",
                c => new
                    {
                        StoreFrontConfigurationId = c.Int(nullable: false, identity: true),
                        Folder = c.String(nullable: false, maxLength: 100),
                        Order = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ConfigurationName = c.String(nullable: false, maxLength: 100),
                        PublicUrl = c.String(nullable: false, maxLength: 200),
                        TimeZoneId = c.String(nullable: false, maxLength: 50),
                        HtmlFooter = c.String(maxLength: 250),
                        HomePageUseCatalog = c.Boolean(nullable: false),
                        NavBarItemsMaxLevels = c.Int(nullable: false),
                        AccountAdmin_UserProfileId = c.Int(nullable: false),
                        RegisteredNotify_UserProfileId = c.Int(nullable: false),
                        WelcomePerson_UserProfileId = c.Int(nullable: false),
                        OrderAdmin_UserProfileId = c.Int(nullable: false),
                        MetaApplicationName = c.String(maxLength: 250),
                        MetaApplicationTileColor = c.String(maxLength: 25),
                        MetaDescription = c.String(maxLength: 1000),
                        MetaKeywords = c.String(maxLength: 1000),
                        BodyTopScriptTag = c.String(),
                        BodyBottomScriptTag = c.String(),
                        EnableGoogleAnalytics = c.Boolean(nullable: false),
                        GoogleAnalyticsWebPropertyId = c.String(maxLength: 50),
                        DefaultNewPageThemeId = c.Int(nullable: false),
                        CatalogThemeId = c.Int(nullable: false),
                        AccountThemeId = c.Int(nullable: false),
                        NotificationsThemeId = c.Int(nullable: false),
                        ProfileThemeId = c.Int(nullable: false),
                        AdminThemeId = c.Int(nullable: false),
                        UseShoppingCart = c.Boolean(nullable: false),
                        CartNavShowCartToAnonymous = c.Boolean(nullable: false),
                        CartNavShowCartToRegistered = c.Boolean(nullable: false),
                        CartNavShowCartWhenEmpty = c.Boolean(nullable: false),
                        CartRequireLogin = c.Boolean(nullable: false),
                        CartThemeId = c.Int(nullable: false),
                        CartPageTitle = c.String(),
                        CartPageHeading = c.String(),
                        CartEmptyMessage = c.String(),
                        CartCheckoutButtonLabel = c.String(),
                        CartItemColumnLabel = c.String(),
                        CartItemVariantColumnShow = c.Boolean(nullable: false),
                        CartItemVariantColumnLabel = c.String(),
                        CartItemListPriceColumnShow = c.Boolean(nullable: false),
                        CartItemListPriceColumnLabel = c.String(),
                        CartItemUnitPriceColumnShow = c.Boolean(nullable: false),
                        CartItemUnitPriceColumnLabel = c.String(),
                        CartItemQuantityColumnShow = c.Boolean(nullable: false),
                        CartItemQuantityColumnLabel = c.String(),
                        CartItemListPriceExtColumnShow = c.Boolean(nullable: false),
                        CartItemListPriceExtColumnLabel = c.String(),
                        CartItemUnitPriceExtColumnShow = c.Boolean(nullable: false),
                        CartItemUnitPriceExtColumnLabel = c.String(),
                        CartItemDiscountColumnShow = c.Boolean(nullable: false),
                        CartItemDiscountColumnLabel = c.String(),
                        CartBundleShowIncludedItems = c.Boolean(nullable: false),
                        CartBundleShowPriceOfIncludedItems = c.Boolean(nullable: false),
                        CartItemTotalColumnShow = c.Boolean(nullable: false),
                        CartItemTotalColumnLabel = c.String(),
                        CartOrderDiscountCodeSectionShow = c.Boolean(nullable: false),
                        CartOrderDiscountCodeLabel = c.String(),
                        CartOrderDiscountCodeApplyButtonText = c.String(),
                        CartOrderDiscountCodeRemoveButtonText = c.String(),
                        CartOrderItemCountShow = c.Boolean(nullable: false),
                        CartOrderItemCountLabel = c.String(),
                        CartOrderSubtotalShow = c.Boolean(nullable: false),
                        CartOrderSubtotalLabel = c.String(),
                        CartOrderTaxShow = c.Boolean(nullable: false),
                        CartOrderTaxLabel = c.String(),
                        CartOrderShippingShow = c.Boolean(nullable: false),
                        CartOrderShippingLabel = c.String(),
                        CartOrderHandlingShow = c.Boolean(nullable: false),
                        CartOrderHandlingLabel = c.String(),
                        CartOrderDiscountShow = c.Boolean(nullable: false),
                        CartOrderDiscountLabel = c.String(),
                        CartOrderTotalLabel = c.String(),
                        CheckoutThemeId = c.Int(nullable: false),
                        CheckoutLogInOrGuestWebFormId = c.Int(),
                        CheckoutDeliveryInfoDigitalOnlyWebFormId = c.Int(),
                        CheckoutDeliveryInfoShippingWebFormId = c.Int(),
                        CheckoutDeliveryMethodWebFormId = c.Int(),
                        CheckoutPaymentInfoWebFormId = c.Int(),
                        CheckoutConfirmOrderWebFormId = c.Int(),
                        OrdersThemeId = c.Int(nullable: false),
                        OrderAdminThemeId = c.Int(nullable: false),
                        CatalogAdminThemeId = c.Int(nullable: false),
                        CatalogTitle = c.String(nullable: false),
                        CatalogLayout = c.Int(nullable: false),
                        CatalogHeaderHtml = c.String(),
                        CatalogFooterHtml = c.String(),
                        CatalogRootListTemplate = c.Int(nullable: false),
                        CatalogRootHeaderHtml = c.String(),
                        CatalogRootFooterHtml = c.String(),
                        NavBarCatalogMaxLevels = c.Int(nullable: false),
                        CatalogPageInitialLevels = c.Int(nullable: false),
                        CatalogCategoryColLg = c.Int(nullable: false),
                        CatalogCategoryColMd = c.Int(nullable: false),
                        CatalogCategoryColSm = c.Int(nullable: false),
                        CatalogProductColLg = c.Int(nullable: false),
                        CatalogProductColMd = c.Int(nullable: false),
                        CatalogProductColSm = c.Int(nullable: false),
                        CatalogProductBundleColLg = c.Int(nullable: false),
                        CatalogProductBundleColMd = c.Int(nullable: false),
                        CatalogProductBundleColSm = c.Int(nullable: false),
                        NotFoundError_PageId = c.Int(),
                        StoreError_PageId = c.Int(),
                        NavBarShowRegisterLink = c.Boolean(nullable: false),
                        NavBarRegisterLinkText = c.String(maxLength: 50),
                        AccountLoginShowRegisterLink = c.Boolean(nullable: false),
                        AccountLoginRegisterLinkText = c.String(maxLength: 50),
                        Register_WebFormId = c.Int(),
                        RegisterSuccess_PageId = c.Int(),
                        Orders_AutoAcceptPaid = c.Boolean(nullable: false),
                        PaymentMethod_PayPal_Enabled = c.Boolean(nullable: false),
                        PaymentMethod_PayPal_UseLiveServer = c.Boolean(nullable: false),
                        PaymentMethod_PayPal_Client_Id = c.String(),
                        PaymentMethod_PayPal_Client_Secret = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StoreFrontConfigurationId)
                .ForeignKey("dbo.UserProfile", t => t.AccountAdmin_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Theme", t => t.AccountThemeId)
                .ForeignKey("dbo.Theme", t => t.AdminThemeId)
                .ForeignKey("dbo.Theme", t => t.CartThemeId)
                .ForeignKey("dbo.Theme", t => t.CatalogAdminThemeId)
                .ForeignKey("dbo.Theme", t => t.CatalogThemeId)
                .ForeignKey("dbo.WebForm", t => t.CheckoutConfirmOrderWebFormId)
                .ForeignKey("dbo.WebForm", t => t.CheckoutDeliveryInfoDigitalOnlyWebFormId)
                .ForeignKey("dbo.WebForm", t => t.CheckoutDeliveryInfoShippingWebFormId)
                .ForeignKey("dbo.WebForm", t => t.CheckoutDeliveryMethodWebFormId)
                .ForeignKey("dbo.WebForm", t => t.CheckoutLogInOrGuestWebFormId)
                .ForeignKey("dbo.WebForm", t => t.CheckoutPaymentInfoWebFormId)
                .ForeignKey("dbo.Theme", t => t.CheckoutThemeId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Theme", t => t.DefaultNewPageThemeId)
                .ForeignKey("dbo.Page", t => t.NotFoundError_PageId)
                .ForeignKey("dbo.Theme", t => t.NotificationsThemeId)
                .ForeignKey("dbo.UserProfile", t => t.OrderAdmin_UserProfileId)
                .ForeignKey("dbo.Theme", t => t.OrderAdminThemeId)
                .ForeignKey("dbo.Theme", t => t.OrdersThemeId)
                .ForeignKey("dbo.Theme", t => t.ProfileThemeId)
                .ForeignKey("dbo.UserProfile", t => t.RegisteredNotify_UserProfileId)
                .ForeignKey("dbo.Page", t => t.RegisterSuccess_PageId)
                .ForeignKey("dbo.WebForm", t => t.Register_WebFormId)
                .ForeignKey("dbo.Page", t => t.StoreError_PageId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.WelcomePerson_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.ConfigurationName }, unique: true, name: "UniqueRecord")
                .Index(t => t.AccountAdmin_UserProfileId)
                .Index(t => t.RegisteredNotify_UserProfileId)
                .Index(t => t.WelcomePerson_UserProfileId)
                .Index(t => t.OrderAdmin_UserProfileId)
                .Index(t => t.DefaultNewPageThemeId)
                .Index(t => t.CatalogThemeId)
                .Index(t => t.AccountThemeId)
                .Index(t => t.NotificationsThemeId)
                .Index(t => t.ProfileThemeId)
                .Index(t => t.AdminThemeId)
                .Index(t => t.CartThemeId)
                .Index(t => t.CheckoutThemeId)
                .Index(t => t.CheckoutLogInOrGuestWebFormId)
                .Index(t => t.CheckoutDeliveryInfoDigitalOnlyWebFormId)
                .Index(t => t.CheckoutDeliveryInfoShippingWebFormId)
                .Index(t => t.CheckoutDeliveryMethodWebFormId)
                .Index(t => t.CheckoutPaymentInfoWebFormId)
                .Index(t => t.CheckoutConfirmOrderWebFormId)
                .Index(t => t.OrdersThemeId)
                .Index(t => t.OrderAdminThemeId)
                .Index(t => t.CatalogAdminThemeId)
                .Index(t => t.NotFoundError_PageId)
                .Index(t => t.StoreError_PageId)
                .Index(t => t.Register_WebFormId)
                .Index(t => t.RegisterSuccess_PageId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Theme",
                c => new
                    {
                        ThemeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        FolderName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ThemeId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Folder = c.String(nullable: false, maxLength: 100),
                        Order = c.Int(nullable: false),
                        TimeZoneId = c.String(nullable: false, maxLength: 50),
                        EnablePageViewLog = c.Boolean(nullable: false),
                        EnableNewUserRegisteredBroadcast = c.Boolean(nullable: false),
                        UseSendGridEmail = c.Boolean(nullable: false),
                        SendGridMailAccount = c.String(),
                        SendGridMailPassword = c.String(),
                        SendGridMailFromEmail = c.String(),
                        SendGridMailFromName = c.String(),
                        UseTwilioSms = c.Boolean(nullable: false),
                        TwilioSid = c.String(),
                        TwilioToken = c.String(),
                        TwilioFromPhone = c.String(),
                        TwilioSmsFromName = c.String(),
                        TwilioSmsFromEmail = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.ClientId, unique: true, name: "UniqueRecord")
                .Index(t => t.Name, unique: true)
                .Index(t => t.Folder, unique: true)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ClientRole",
                c => new
                    {
                        ClientRoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        IsPublicUserRole = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientRoleId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ClientRoleAction",
                c => new
                    {
                        ClientRoleActionId = c.Int(nullable: false, identity: true),
                        ClientRoleId = c.Int(nullable: false),
                        GStoreActionId = c.Int(nullable: false),
                        GStoreActionName = c.String(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientRoleActionId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.ClientRole", t => t.ClientRoleId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.ClientRoleId, t.GStoreActionId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ClientUserRole",
                c => new
                    {
                        ClientUserRoleId = c.Int(nullable: false, identity: true),
                        UserProfileId = c.Int(nullable: false),
                        ClientRoleId = c.Int(nullable: false),
                        ScopeStoreFrontId = c.Int(),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ClientUserRoleId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.ClientRole", t => t.ClientRoleId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.ScopeStoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UserProfileId)
                .Index(t => new { t.ClientId, t.UserProfileId, t.ClientRoleId, t.ScopeStoreFrontId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.StoreFront",
                c => new
                    {
                        StoreFrontId = c.Int(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        NextOrderNumber = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StoreFrontId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId }, unique: true)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Cart",
                c => new
                    {
                        CartId = c.Int(nullable: false, identity: true),
                        SessionId = c.String(nullable: false, maxLength: 100),
                        UserProfileId = c.Int(),
                        OrderId = c.Int(),
                        Email = c.String(),
                        FullName = c.String(),
                        UserAgent = c.String(nullable: false),
                        IPAddress = c.String(nullable: false, maxLength: 15),
                        EntryUrl = c.String(),
                        EntryRawUrl = c.String(),
                        EntryReferrer = c.String(),
                        EntryDateTimeUtc = c.DateTime(nullable: false),
                        DiscountCodesAttempted = c.String(),
                        DiscountCodesFailed = c.String(),
                        DiscountCode = c.String(),
                        DiscountId = c.Int(),
                        ItemCount = c.Int(nullable: false),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Handling = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AllItemsAreDigitalDownload = c.Boolean(nullable: false),
                        DigitalDownloadItemCount = c.Int(nullable: false),
                        ShippingItemCount = c.Int(nullable: false),
                        LoginOrGuestWebFormResponseId = c.Int(),
                        DeliveryInfoDigitalId = c.Int(),
                        DeliveryInfoShippingId = c.Int(),
                        CartPaymentInfoId = c.Int(),
                        ConfirmOrderWebFormResponseId = c.Int(),
                        StatusEmailedContents = c.Boolean(nullable: false),
                        StatusStartedCheckout = c.Boolean(nullable: false),
                        StatusSelectedLogInOrGuest = c.Boolean(nullable: false),
                        StatusCompletedDeliveryInfo = c.Boolean(nullable: false),
                        StatusSelectedDeliveryMethod = c.Boolean(nullable: false),
                        StatusPaymentInfoConfirmed = c.Boolean(nullable: false),
                        StatusPlacedOrder = c.Boolean(nullable: false),
                        StatusPrintedConfirmation = c.Boolean(nullable: false),
                        StatusEmailedConfirmation = c.Boolean(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CartId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.WebFormResponse", t => t.ConfirmOrderWebFormResponseId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Discount", t => t.DiscountId)
                .ForeignKey("dbo.WebFormResponse", t => t.LoginOrGuestWebFormResponseId)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.SessionId, t.UserProfileId, t.OrderId }, unique: true, name: "UniqueRecord")
                .Index(t => t.DiscountId)
                .Index(t => t.LoginOrGuestWebFormResponseId)
                .Index(t => t.ConfirmOrderWebFormResponseId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.CartBundle",
                c => new
                    {
                        CartBundleId = c.Int(nullable: false, identity: true),
                        CartId = c.Int(nullable: false),
                        ProductBundleId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CartBundleId)
                .ForeignKey("dbo.Cart", t => t.CartId)
                .ForeignKey("dbo.ProductBundle", t => t.ProductBundleId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.CartId, t.ProductBundleId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.CartItem",
                c => new
                    {
                        CartItemId = c.Int(nullable: false, identity: true),
                        CartId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        CartBundleId = c.Int(),
                        ProductBundleItemId = c.Int(),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ListPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceExt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ListPriceExt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductVariantInfo = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CartItemId)
                .ForeignKey("dbo.CartBundle", t => t.CartBundleId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.ProductBundleItem", t => t.ProductBundleItemId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.Cart", t => t.CartId, cascadeDelete: true)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.CartId, t.ProductId, t.CartBundleId }, unique: true, name: "UniqueRecord")
                .Index(t => t.ProductBundleItemId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        UrlName = c.String(nullable: false, maxLength: 100),
                        ForRegisteredOnly = c.Boolean(nullable: false),
                        ForAnonymousOnly = c.Boolean(nullable: false),
                        AvailableForPurchase = c.Boolean(nullable: false),
                        RequestAQuote_Show = c.Boolean(nullable: false),
                        RequestAQuote_Label = c.String(),
                        RequestAQuote_PageId = c.Int(),
                        MetaDescription = c.String(),
                        MetaKeywords = c.String(),
                        ProductCategoryId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        ImageName = c.String(),
                        DigitalDownload = c.Boolean(nullable: false),
                        MaxQuantityPerOrder = c.Int(nullable: false),
                        BaseUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BaseListPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ThemeId = c.Int(),
                        ProductDetailTemplate = c.Int(),
                        SummaryCaption = c.String(),
                        SummaryHtml = c.String(),
                        TopDescriptionCaption = c.String(),
                        TopDescriptionHtml = c.String(),
                        BottomDescriptionCaption = c.String(),
                        BottomDescriptionHtml = c.String(),
                        FooterHtml = c.String(),
                        DigitalDownloadFileName = c.String(),
                        SampleImageFileName = c.String(),
                        SampleImageCaption = c.String(),
                        SampleAudioFileName = c.String(),
                        SampleAudioCaption = c.String(),
                        SampleDownloadFileName = c.String(),
                        SampleDownloadCaption = c.String(),
                        TopLinkHref = c.String(),
                        TopLinkLabel = c.String(),
                        BottomLinkHref = c.String(),
                        BottomLinkLabel = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategoryId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Page", t => t.RequestAQuote_PageId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Theme", t => t.ThemeId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.UrlName }, unique: true, name: "UniqueRecord")
                .Index(t => t.RequestAQuote_PageId)
                .Index(t => t.ProductCategoryId)
                .Index(t => t.ThemeId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ProductCategory",
                c => new
                    {
                        ProductCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        UrlName = c.String(nullable: false, maxLength: 100),
                        ImageName = c.String(),
                        Order = c.Int(nullable: false),
                        ParentCategoryId = c.Int(),
                        AllowChildCategoriesInMenu = c.Boolean(nullable: false),
                        ShowInMenu = c.Boolean(nullable: false),
                        HideInMenuIfEmpty = c.Boolean(nullable: false),
                        DisplayForDirectLinks = c.Boolean(nullable: false),
                        ShowInCatalogIfEmpty = c.Boolean(nullable: false),
                        ForRegisteredOnly = c.Boolean(nullable: false),
                        ForAnonymousOnly = c.Boolean(nullable: false),
                        UseDividerBeforeOnMenu = c.Boolean(nullable: false),
                        UseDividerAfterOnMenu = c.Boolean(nullable: false),
                        DirectActiveCount = c.Int(nullable: false),
                        ChildActiveCount = c.Int(nullable: false),
                        ThemeId = c.Int(),
                        CategoryDetailTemplate = c.Int(nullable: false),
                        ProductListTemplate = c.Int(nullable: false),
                        ProductDetailTemplate = c.Int(nullable: false),
                        ProductBundleDetailTemplate = c.Int(nullable: false),
                        ChildCategoryHeaderHtml = c.String(),
                        ChildCategoryFooterHtml = c.String(),
                        ProductHeaderHtml = c.String(),
                        ProductFooterHtml = c.String(),
                        ProductBundleHeaderHtml = c.String(),
                        ProductBundleFooterHtml = c.String(),
                        NoProductsMessageHtml = c.String(),
                        DefaultSummaryCaption = c.String(),
                        DefaultTopDescriptionCaption = c.String(),
                        DefaultBottomDescriptionCaption = c.String(),
                        DefaultSampleImageCaption = c.String(),
                        DefaultSampleAudioCaption = c.String(),
                        DefaultSampleDownloadCaption = c.String(),
                        MetaDescription = c.String(),
                        MetaKeywords = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductCategoryId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.ProductCategory", t => t.ParentCategoryId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Theme", t => t.ThemeId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.UrlName }, unique: true, name: "UniqueRecord")
                .Index(t => t.ParentCategoryId)
                .Index(t => t.ThemeId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ProductBundle",
                c => new
                    {
                        ProductBundleId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        UrlName = c.String(nullable: false, maxLength: 100),
                        ImageName = c.String(),
                        ForRegisteredOnly = c.Boolean(nullable: false),
                        ForAnonymousOnly = c.Boolean(nullable: false),
                        AvailableForPurchase = c.Boolean(nullable: false),
                        RequestAQuote_Show = c.Boolean(nullable: false),
                        RequestAQuote_Label = c.String(),
                        RequestAQuote_PageId = c.Int(),
                        MetaDescription = c.String(),
                        MetaKeywords = c.String(),
                        ProductCategoryId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        MaxQuantityPerOrder = c.Int(nullable: false),
                        ThemeId = c.Int(),
                        ProductBundleDetailTemplate = c.Int(),
                        SummaryCaption = c.String(),
                        SummaryHtml = c.String(),
                        TopDescriptionCaption = c.String(),
                        TopDescriptionHtml = c.String(),
                        BottomDescriptionCaption = c.String(),
                        BottomDescriptionHtml = c.String(),
                        FooterHtml = c.String(),
                        TopLinkHref = c.String(),
                        TopLinkLabel = c.String(),
                        BottomLinkHref = c.String(),
                        BottomLinkLabel = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductBundleId)
                .ForeignKey("dbo.ProductCategory", t => t.ProductCategoryId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Page", t => t.RequestAQuote_PageId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Theme", t => t.ThemeId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.UrlName }, unique: true, name: "UniqueRecord")
                .Index(t => t.RequestAQuote_PageId)
                .Index(t => t.ProductCategoryId)
                .Index(t => t.ThemeId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ProductBundleItem",
                c => new
                    {
                        ProductBundleItemId = c.Int(nullable: false, identity: true),
                        ProductBundleId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        BaseUnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BaseListPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductVariantInfo = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProductBundleItemId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.ProductBundle", t => t.ProductBundleId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.ProductBundleId, t.ProductId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Page",
                c => new
                    {
                        PageId = c.Int(nullable: false, identity: true),
                        PageTemplateId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Order = c.Int(nullable: false),
                        ThemeId = c.Int(nullable: false),
                        Url = c.String(nullable: false, maxLength: 250),
                        ForRegisteredOnly = c.Boolean(nullable: false),
                        ForAnonymousOnly = c.Boolean(nullable: false),
                        MetaDescription = c.String(maxLength: 2000),
                        MetaApplicationName = c.String(maxLength: 200),
                        MetaApplicationTileColor = c.String(maxLength: 20),
                        MetaKeywords = c.String(maxLength: 2000),
                        PageTitle = c.String(maxLength: 200),
                        BodyTopScriptTag = c.String(),
                        BodyBottomScriptTag = c.String(),
                        WebFormId = c.Int(),
                        WebFormThankYouTitle = c.String(maxLength: 200),
                        WebFormThankYouMessage = c.String(maxLength: 2000),
                        WebFormSuccessPageId = c.Int(),
                        WebFormSaveToDatabase = c.Boolean(nullable: false),
                        WebFormSaveToFile = c.Boolean(nullable: false),
                        WebFormSendToEmail = c.Boolean(nullable: false),
                        WebFormEmailToAddress = c.String(maxLength: 200),
                        WebFormEmailToName = c.String(maxLength: 200),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PageId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.PageTemplate", t => t.PageTemplateId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.Theme", t => t.ThemeId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.WebForm", t => t.WebFormId)
                .Index(t => t.PageTemplateId)
                .Index(t => t.ThemeId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.Url }, unique: true, name: "UniqueRecord")
                .Index(t => t.WebFormId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.NavBarItem",
                c => new
                    {
                        NavBarItemId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Order = c.Int(nullable: false),
                        htmlAttributes = c.String(),
                        OpenInNewWindow = c.Boolean(nullable: false),
                        IsAction = c.Boolean(nullable: false),
                        Action = c.String(),
                        Controller = c.String(),
                        Area = c.String(),
                        ActionIdParam = c.Int(),
                        IsPage = c.Boolean(nullable: false),
                        PageId = c.Int(),
                        IsLocalHRef = c.Boolean(nullable: false),
                        LocalHRef = c.String(),
                        IsRemoteHRef = c.Boolean(nullable: false),
                        RemoteHRef = c.String(),
                        UseDividerAfterOnMenu = c.Boolean(nullable: false),
                        UseDividerBeforeOnMenu = c.Boolean(nullable: false),
                        ForRegisteredOnly = c.Boolean(nullable: false),
                        ForAnonymousOnly = c.Boolean(nullable: false),
                        ParentNavBarItemId = c.Int(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NavBarItemId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Page", t => t.PageId)
                .ForeignKey("dbo.NavBarItem", t => t.ParentNavBarItemId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.PageId)
                .Index(t => t.ParentNavBarItemId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.PageTemplate",
                c => new
                    {
                        PageTemplateId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Order = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        ViewName = c.String(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PageTemplateId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.PageTemplateSection",
                c => new
                    {
                        PageTemplateSectionId = c.Int(nullable: false, identity: true),
                        PageTemplateId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Order = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        DefaultRawHtmlValue = c.String(),
                        PreTextHtml = c.String(),
                        PostTextHtml = c.String(),
                        DefaultTextCssClass = c.String(),
                        EditInTop = c.Boolean(nullable: false),
                        EditInBottom = c.Boolean(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PageTemplateSectionId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.PageTemplate", t => t.PageTemplateId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.PageTemplateId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.PageSection",
                c => new
                    {
                        PageSectionId = c.Int(nullable: false, identity: true),
                        PageId = c.Int(nullable: false),
                        PageTemplateSectionId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        UseDefaultFromTemplate = c.Boolean(nullable: false),
                        HasNothing = c.Boolean(nullable: false),
                        HasPlainText = c.Boolean(nullable: false),
                        PlainText = c.String(),
                        HasRawHtml = c.Boolean(nullable: false),
                        RawHtml = c.String(),
                        TextCssClass = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PageSectionId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Page", t => t.PageId)
                .ForeignKey("dbo.PageTemplateSection", t => t.PageTemplateSectionId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.PageId, t.PageTemplateSectionId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.WebForm",
                c => new
                    {
                        WebFormId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Order = c.Int(nullable: false),
                        Description = c.String(nullable: false),
                        Title = c.String(maxLength: 100),
                        FormHeaderHtml = c.String(),
                        FormFooterBeforeSubmitHtml = c.String(),
                        FormFooterAfterSubmitHtml = c.String(),
                        SubmitButtonText = c.String(nullable: false, maxLength: 20),
                        SubmitButtonClass = c.String(nullable: false, maxLength: 50),
                        DisplayTemplateName = c.String(nullable: false, maxLength: 25),
                        LabelMdColSpan = c.Int(nullable: false),
                        FieldMdColSpan = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WebFormId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.WebFormField",
                c => new
                    {
                        WebFormFieldId = c.Int(nullable: false, identity: true),
                        WebFormId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Order = c.Int(nullable: false),
                        LabelText = c.String(nullable: false, maxLength: 100),
                        Watermark = c.String(maxLength: 100),
                        HelpLabelTopText = c.String(maxLength: 100),
                        HelpLabelBottomText = c.String(maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 200),
                        IsRequired = c.Boolean(nullable: false),
                        DataType = c.Int(nullable: false),
                        DataTypeString = c.String(maxLength: 50),
                        ValueListId = c.Int(),
                        ValueListNullText = c.String(),
                        TextAreaRows = c.Int(),
                        TextAreaColumns = c.Int(),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                        WebFormResponse_WebFormResponseId = c.Int(),
                    })
                .PrimaryKey(t => t.WebFormFieldId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.ValueList", t => t.ValueListId)
                .ForeignKey("dbo.WebForm", t => t.WebFormId)
                .ForeignKey("dbo.WebFormResponse", t => t.WebFormResponse_WebFormResponseId)
                .Index(t => new { t.ClientId, t.WebFormId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.ValueListId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId)
                .Index(t => t.WebFormResponse_WebFormResponseId);
            
            CreateTable(
                "dbo.ValueList",
                c => new
                    {
                        ValueListId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(nullable: false, maxLength: 200),
                        Order = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ValueListId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ValueListItem",
                c => new
                    {
                        ValueListItemId = c.Int(nullable: false, identity: true),
                        ValueListId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        IsInteger = c.Boolean(nullable: false),
                        IntegerValue = c.Int(),
                        IsString = c.Boolean(nullable: false),
                        StringValue = c.String(),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ValueListItemId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.ValueList", t => t.ValueListId)
                .Index(t => new { t.ClientId, t.ValueListId, t.Name }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.WebFormFieldResponse",
                c => new
                    {
                        WebFormFieldResponseId = c.Int(nullable: false, identity: true),
                        WebFormResponseId = c.Int(nullable: false),
                        WebFormFieldId = c.Int(nullable: false),
                        WebFormName = c.String(nullable: false),
                        WebFormOrder = c.Int(nullable: false),
                        WebFormFieldName = c.String(nullable: false),
                        WebFormFieldOrder = c.Int(nullable: false),
                        WebFormFieldLabelText = c.String(nullable: false),
                        DataType = c.Int(nullable: false),
                        DataTypeString = c.String(maxLength: 50),
                        Value1String = c.String(),
                        Value2String = c.String(),
                        Value1Int = c.Int(),
                        Value2Int = c.Int(),
                        Value1Bool = c.Boolean(),
                        Value2Bool = c.Boolean(),
                        Value1Decimal = c.Decimal(precision: 18, scale: 2),
                        Value2Decimal = c.Decimal(precision: 18, scale: 2),
                        Value1ValueListItemId = c.Int(),
                        Value1ValueListItemName = c.String(),
                        Value1ValueListItemIdList = c.String(),
                        Value1ValueListItemNameList = c.String(),
                        Value1PageId = c.Int(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WebFormFieldResponseId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.ValueListItem", t => t.Value1PageId)
                .ForeignKey("dbo.ValueListItem", t => t.Value1ValueListItemId)
                .ForeignKey("dbo.WebFormField", t => t.WebFormFieldId)
                .ForeignKey("dbo.WebFormResponse", t => t.WebFormResponseId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.WebFormResponseId, t.WebFormFieldId }, unique: true, name: "UniqueRecord")
                .Index(t => t.Value1ValueListItemId)
                .Index(t => t.Value1PageId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.WebFormResponse",
                c => new
                    {
                        WebFormResponseId = c.Int(nullable: false, identity: true),
                        WebFormId = c.Int(nullable: false),
                        PageId = c.Int(),
                        BodyText = c.String(),
                        Subject = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.WebFormResponseId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Page", t => t.PageId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.WebForm", t => t.WebFormId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.WebFormResponseId }, unique: true, name: "UniqueRecord")
                .Index(t => t.WebFormId)
                .Index(t => t.PageId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.ProductReview",
                c => new
                    {
                        ProductReviewId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        UserProfileId = c.Int(nullable: false),
                        StarRating = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Title = c.String(nullable: false, maxLength: 200),
                        Body = c.String(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                        UserProfile_UserProfileId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductReviewId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.ProductId, t.UserProfileId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId)
                .Index(t => t.UserProfile_UserProfileId);
            
            CreateTable(
                "dbo.CartPaymentInfo",
                c => new
                    {
                        CartPaymentInfoId = c.Int(nullable: false),
                        CartId = c.Int(nullable: false),
                        PaymentSource = c.Int(nullable: false),
                        PayPalPaymentId = c.String(),
                        PayPalReturnPaymentId = c.String(),
                        PayPalReturnPayerId = c.String(),
                        PayPalReturnToken = c.String(),
                        Json = c.String(),
                        WebFormResponseId = c.Int(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CartPaymentInfoId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.WebFormResponse", t => t.WebFormResponseId)
                .ForeignKey("dbo.Cart", t => t.CartPaymentInfoId)
                .Index(t => t.CartPaymentInfoId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.CartId }, unique: true, name: "UniqueRecord")
                .Index(t => t.WebFormResponseId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.DeliveryInfoDigital",
                c => new
                    {
                        DeliveryInfoDigitalId = c.Int(nullable: false),
                        CartId = c.Int(nullable: false),
                        EmailAddress = c.String(),
                        FullName = c.String(),
                        WebFormResponseId = c.Int(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeliveryInfoDigitalId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.WebFormResponse", t => t.WebFormResponseId)
                .ForeignKey("dbo.Cart", t => t.DeliveryInfoDigitalId)
                .Index(t => t.DeliveryInfoDigitalId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.CartId }, unique: true, name: "UniqueRecord")
                .Index(t => t.WebFormResponseId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.DeliveryInfoShipping",
                c => new
                    {
                        DeliveryInfoShippingId = c.Int(nullable: false),
                        CartId = c.Int(nullable: false),
                        ShippingDeliveryMethod = c.Int(),
                        EmailAddress = c.String(nullable: false, maxLength: 150),
                        FullName = c.String(nullable: false, maxLength: 100),
                        AdddressL1 = c.String(nullable: false, maxLength: 100),
                        AdddressL2 = c.String(maxLength: 100),
                        City = c.String(nullable: false, maxLength: 50),
                        State = c.String(nullable: false, maxLength: 50),
                        PostalCode = c.String(nullable: false, maxLength: 12),
                        CountryCode = c.Int(nullable: false),
                        WebFormResponseId = c.Int(),
                        DeliveryMethodWebFormResponseId = c.Int(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DeliveryInfoShippingId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.WebFormResponse", t => t.DeliveryMethodWebFormResponseId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.WebFormResponse", t => t.WebFormResponseId)
                .ForeignKey("dbo.Cart", t => t.DeliveryInfoShippingId)
                .Index(t => t.DeliveryInfoShippingId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.CartId }, unique: true, name: "UniqueRecord")
                .Index(t => t.WebFormResponseId)
                .Index(t => t.DeliveryMethodWebFormResponseId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Discount",
                c => new
                    {
                        DiscountId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        Order = c.Int(nullable: false),
                        MaxUses = c.Int(nullable: false),
                        UseCount = c.Int(nullable: false),
                        MinSubtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PercentOff = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FlatDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FreeShipping = c.Boolean(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                        FreeProduct_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.DiscountId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Product", t => t.FreeProduct_ProductId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.Code }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId)
                .Index(t => t.FreeProduct_ProductId);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false),
                        FullName = c.String(nullable: false),
                        OriginalCartId = c.Int(),
                        SessionId = c.String(nullable: false, maxLength: 100),
                        UserProfileId = c.Int(),
                        DeliveryInfoDigitalId = c.Int(),
                        DeliveryInfoShippingId = c.Int(),
                        AllItemsAreDigitalDownload = c.Boolean(nullable: false),
                        DigitalDownloadItemCount = c.Int(nullable: false),
                        ShippingItemCount = c.Int(nullable: false),
                        UserAgent = c.String(nullable: false),
                        IPAddress = c.String(nullable: false, maxLength: 15),
                        EntryUrl = c.String(),
                        EntryRawUrl = c.String(),
                        EntryReferrer = c.String(),
                        EntryDateTimeUtc = c.DateTime(nullable: false),
                        DiscountCodesAttempted = c.String(),
                        DiscountCodesFailed = c.String(),
                        DiscountCode = c.String(),
                        DiscountId = c.Int(),
                        ItemCount = c.Int(nullable: false),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Handling = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RefundedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StatusOrderEditedByUser = c.Boolean(nullable: false),
                        StatusOrderEditedByMerchant = c.Boolean(nullable: false),
                        StatusOrderCancelledByUser = c.Boolean(nullable: false),
                        StatusOrderCancelledByMerchant = c.Boolean(nullable: false),
                        StatusOrderAccepted = c.Boolean(nullable: false),
                        StatusOrderPaymentProcessed = c.Boolean(nullable: false),
                        StatusOrderShipped = c.Boolean(nullable: false),
                        StatusOrderDelivered = c.Boolean(nullable: false),
                        StatusOrderDownloaded = c.Boolean(nullable: false),
                        StatusOrderFeedbackReceived = c.Boolean(nullable: false),
                        StatusOrderItemsReturned = c.Boolean(nullable: false),
                        LoginOrGuestWebFormResponseId = c.Int(),
                        PaymentInfoWebFormResponseId = c.Int(),
                        ConfirmOrderWebFormResponseId = c.Int(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.WebFormResponse", t => t.ConfirmOrderWebFormResponseId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.DeliveryInfoDigital", t => t.DeliveryInfoDigitalId)
                .ForeignKey("dbo.DeliveryInfoShipping", t => t.DeliveryInfoShippingId)
                .ForeignKey("dbo.Discount", t => t.DiscountId)
                .ForeignKey("dbo.WebFormResponse", t => t.LoginOrGuestWebFormResponseId)
                .ForeignKey("dbo.WebFormResponse", t => t.PaymentInfoWebFormResponseId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.OrderNumber }, unique: true, name: "UniqueRecord")
                .Index(t => t.UserProfileId)
                .Index(t => t.DeliveryInfoDigitalId)
                .Index(t => t.DeliveryInfoShippingId)
                .Index(t => t.DiscountId)
                .Index(t => t.LoginOrGuestWebFormResponseId)
                .Index(t => t.PaymentInfoWebFormResponseId)
                .Index(t => t.ConfirmOrderWebFormResponseId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Notification",
                c => new
                    {
                        NotificationId = c.Int(nullable: false, identity: true),
                        ToUserProfileId = c.Int(nullable: false),
                        FromUserProfileId = c.Int(nullable: false),
                        To = c.String(nullable: false),
                        From = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        Importance = c.String(nullable: false),
                        Message = c.String(nullable: false),
                        Read = c.Boolean(nullable: false),
                        UrlHost = c.String(nullable: false),
                        BaseUrl = c.String(nullable: false),
                        OrderId = c.Int(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.FromUserProfileId)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.ToUserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.NotificationId }, unique: true, name: "UniqueRecord")
                .Index(t => t.ToUserProfileId)
                .Index(t => t.FromUserProfileId)
                .Index(t => t.OrderId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.NotificationLink",
                c => new
                    {
                        NotificationLinkId = c.Int(nullable: false, identity: true),
                        NotificationId = c.Int(nullable: false),
                        Order = c.Int(nullable: false),
                        IsExternal = c.Boolean(nullable: false),
                        Url = c.String(nullable: false),
                        LinkText = c.String(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationLinkId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .ForeignKey("dbo.Notification", t => t.NotificationId, cascadeDelete: true)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.NotificationId, t.Order }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.OrderBundle",
                c => new
                    {
                        OrderBundleId = c.Int(nullable: false, identity: true),
                        CartBundleId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                        ProductBundleId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderBundleId)
                .ForeignKey("dbo.CartBundle", t => t.CartBundleId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.ProductBundle", t => t.ProductBundleId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CartBundleId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.OrderId, t.ProductBundleId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.OrderItem",
                c => new
                    {
                        OrderItemId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        OrderBundleId = c.Int(),
                        ProductBundleItemId = c.Int(),
                        CartItemId = c.Int(nullable: false),
                        IsDigitalDownload = c.Boolean(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ListPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UnitPriceExt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ListPriceExt = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemRefundedAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductVariantInfo = c.String(),
                        StatusItemEditedByUser = c.Boolean(nullable: false),
                        StatusItemEditedByMerchant = c.Boolean(nullable: false),
                        StatusItemCancelledByUser = c.Boolean(nullable: false),
                        StatusItemCancelledByMerchant = c.Boolean(nullable: false),
                        StatusItemAccepted = c.Boolean(nullable: false),
                        StatusItemPaymentReceived = c.Boolean(nullable: false),
                        StatusItemShipped = c.Boolean(nullable: false),
                        StatusItemDelivered = c.Boolean(nullable: false),
                        StatusItemFeedbackReceived = c.Boolean(nullable: false),
                        StatusItemReturned = c.Boolean(nullable: false),
                        StatusItemDownloaded = c.Boolean(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderItemId)
                .ForeignKey("dbo.CartItem", t => t.CartItemId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.OrderBundle", t => t.OrderBundleId)
                .ForeignKey("dbo.Product", t => t.ProductId)
                .ForeignKey("dbo.ProductBundleItem", t => t.ProductBundleItemId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId)
                .Index(t => t.OrderBundleId)
                .Index(t => t.ProductBundleItemId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.CartItemId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.Payment",
                c => new
                    {
                        PaymentId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(),
                        CartId = c.Int(nullable: false),
                        AmountPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentSource = c.Int(nullable: false),
                        IsProcessed = c.Boolean(nullable: false),
                        ProcessDateTimeUtc = c.DateTime(),
                        TransactionId = c.String(),
                        Json = c.String(),
                        PaymentFailed = c.Boolean(nullable: false),
                        FailureException = c.String(),
                        PayPalPaymentResource = c.String(),
                        PayPalState = c.String(),
                        PayPalIntent = c.String(),
                        PayPalCreateTime = c.String(),
                        PayPalUpdateTime = c.String(),
                        PayPalPaymentMethod = c.String(),
                        PayPalIsDirectCreditCardPayment = c.Boolean(nullable: false),
                        PayPalDirectCreditCardNumber = c.String(),
                        PayPalDirectCreditCardType = c.String(),
                        PayPalDirectCreditCardExpireMonth = c.String(),
                        PayPalDirectCreditCardExpireYear = c.String(),
                        PayPalDirectCreditCardFirstName = c.String(),
                        PayPalDirectCreditCardLastName = c.String(),
                        PayPalPayerEmail = c.String(),
                        PayPalPayerFirstName = c.String(),
                        PayPalPayerLastName = c.String(),
                        PayPalPayerId = c.String(),
                        PayPalPaymentResourceLink = c.String(),
                        PayPalTransactionTotal = c.String(),
                        PayPalTransactionCurrency = c.String(),
                        PayPalTransactionDescription = c.String(),
                        PayPalSaleId = c.String(),
                        PayPalSaleCreateTime = c.String(),
                        PayPalSaleUpdateTime = c.String(),
                        PayPalSaleAmountTotal = c.String(),
                        PayPalSaleAmountCurrency = c.String(),
                        PayPalSalePaymentMode = c.String(),
                        PayPalSaleState = c.String(),
                        PayPalSaleProtectionEligibility = c.String(),
                        PayPalSaleProtectionEligibilityType = c.String(),
                        PayPalSaleTransactionFeeValue = c.String(),
                        PayPalSaleTransactionFeeCurrency = c.String(),
                        PayPalSaleAPILinkToSelf = c.String(),
                        PayPalSaleAPILinkToRefund = c.String(),
                        PayPalSaleAPILinkToParentPayment = c.String(),
                        PayPalShippingAddressRecipientName = c.String(),
                        PayPalShippingAddressLine1 = c.String(),
                        PayPalShippingAddressLine2 = c.String(),
                        PayPalShippingAddressCity = c.String(),
                        PayPalShippingAddressState = c.String(),
                        PayPalShippingAddressPostalCode = c.String(),
                        PayPalShippingAddressCountryCode = c.String(),
                        PayPalPayerShippingAddressRecipientName = c.String(),
                        PayPalPayerShippingAddressLine1 = c.String(),
                        PayPalPayerShippingAddressLine2 = c.String(),
                        PayPalPayerShippingAddressCity = c.String(),
                        PayPalPayerShippingAddressState = c.String(),
                        PayPalPayerShippingAddressPostalCode = c.String(),
                        PayPalPayerShippingAddressCountryCode = c.String(),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.Cart", t => t.CartId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.Order", t => t.OrderId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.PaymentId }, unique: true, name: "UniqueRecord")
                .Index(t => t.OrderId)
                .Index(t => t.CartId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.StoreBinding",
                c => new
                    {
                        StoreBindingId = c.Int(nullable: false, identity: true),
                        HostName = c.String(nullable: false, maxLength: 250),
                        Port = c.Int(),
                        RootPath = c.String(nullable: false, maxLength: 250),
                        UseUrlStoreName = c.Boolean(nullable: false),
                        UrlStoreName = c.String(),
                        Order = c.Int(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StoreBindingId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.StoreBindingId }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.EmailSent",
                c => new
                    {
                        EmailSentId = c.Int(nullable: false, identity: true),
                        ToName = c.String(nullable: false),
                        ToAddress = c.String(nullable: false),
                        FromName = c.String(nullable: false),
                        FromAddress = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        HtmlBody = c.String(nullable: false),
                        HtmlSignature = c.String(nullable: false),
                        TextBody = c.String(nullable: false),
                        TextSignature = c.String(nullable: false),
                        Success = c.Boolean(nullable: false),
                        ExceptionString = c.String(),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EmailSentId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.FileNotFoundLog",
                c => new
                    {
                        FileNotFoundLogId = c.Int(nullable: false, identity: true),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.FileNotFoundLogId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.GiftCard",
                c => new
                    {
                        GiftCardId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        Order = c.Int(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UseCount = c.Int(nullable: false),
                        LastUsedDateTimeUtc = c.DateTime(nullable: false),
                        StoreFrontId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(nullable: false),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(nullable: false),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.GiftCardId)
                .ForeignKey("dbo.Client", t => t.ClientId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.StoreFront", t => t.StoreFrontId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => new { t.ClientId, t.StoreFrontId, t.Code }, unique: true, name: "UniqueRecord")
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.PageViewEvent",
                c => new
                    {
                        PageViewEventID = c.Int(nullable: false, identity: true),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PageViewEventID)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.SecurityEvent",
                c => new
                    {
                        SecurityEventID = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        LevelText = c.String(nullable: false),
                        Success = c.Boolean(nullable: false),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SecurityEventID)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.SmsSent",
                c => new
                    {
                        EmailSentId = c.Int(nullable: false, identity: true),
                        ToPhone = c.String(nullable: false),
                        FromPhone = c.String(nullable: false),
                        TextBody = c.String(nullable: false),
                        TextSignature = c.String(nullable: false),
                        Success = c.Boolean(nullable: false),
                        ExceptionString = c.String(),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.EmailSentId)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.SystemEvent",
                c => new
                    {
                        SystemEventID = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        LevelText = c.String(nullable: false),
                        ExceptionMessage = c.String(),
                        BaseExceptionMessage = c.String(),
                        BaseExceptionToString = c.String(),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SystemEventID)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
            CreateTable(
                "dbo.UserActionEvent",
                c => new
                    {
                        UserActionEventID = c.Int(nullable: false, identity: true),
                        Category = c.Int(nullable: false),
                        Action = c.Int(nullable: false),
                        Label = c.String(nullable: false),
                        Success = c.Boolean(nullable: false),
                        CartId = c.Int(),
                        CategoryUrlName = c.String(),
                        ProductUrlName = c.String(),
                        ProductBundleUrlName = c.String(),
                        DiscountCode = c.String(),
                        NotificationId = c.Int(),
                        EmailAddress = c.String(),
                        SmsPhone = c.String(),
                        OrderNumber = c.String(),
                        OrderItemId = c.Int(),
                        PageId = c.Int(),
                        UploadFileName = c.String(),
                        ServerName = c.String(),
                        ApplicationPath = c.String(),
                        HostName = c.String(),
                        HttpMethod = c.String(),
                        IsSecureConnection = c.Boolean(nullable: false),
                        UserHostAddress = c.String(),
                        UrlReferrer = c.String(),
                        UserAgent = c.String(),
                        ClientId = c.Int(),
                        StoreFrontId = c.Int(),
                        RawUrl = c.String(),
                        Url = c.String(),
                        Querystring = c.String(),
                        Source = c.String(),
                        Area = c.String(),
                        Controller = c.String(),
                        ActionName = c.String(),
                        ActionParameters = c.String(),
                        Anonymous = c.Boolean(nullable: false),
                        UserId = c.String(),
                        UserProfileId = c.Int(),
                        UserName = c.String(),
                        FullName = c.String(),
                        Message = c.String(),
                        SessionId = c.String(),
                        CreateDateTimeUtc = c.DateTime(nullable: false),
                        CreatedBy_UserProfileId = c.Int(),
                        UpdateDateTimeUtc = c.DateTime(nullable: false),
                        UpdatedBy_UserProfileId = c.Int(),
                        IsPending = c.Boolean(nullable: false),
                        StartDateTimeUtc = c.DateTime(nullable: false),
                        EndDateTimeUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserActionEventID)
                .ForeignKey("dbo.UserProfile", t => t.CreatedBy_UserProfileId)
                .ForeignKey("dbo.UserProfile", t => t.UpdatedBy_UserProfileId)
                .Index(t => t.CreatedBy_UserProfileId)
                .Index(t => t.UpdatedBy_UserProfileId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserActionEvent", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.UserActionEvent", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.SystemEvent", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.SystemEvent", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.SmsSent", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.SmsSent", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.SecurityEvent", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.SecurityEvent", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageViewEvent", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageViewEvent", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.GiftCard", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.GiftCard", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.GiftCard", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.GiftCard", "ClientId", "dbo.Client");
            DropForeignKey("dbo.FileNotFoundLog", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.FileNotFoundLog", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.EmailSent", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.EmailSent", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.BadRequest", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.BadRequest", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "RegisterWebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.ProductReview", "UserProfile_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "ClientId", "dbo.Client");
            DropForeignKey("dbo.StoreFrontConfiguration", "WelcomePerson_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreFrontConfiguration", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreFrontConfiguration", "StoreError_PageId", "dbo.Page");
            DropForeignKey("dbo.StoreFrontConfiguration", "Register_WebFormId", "dbo.WebForm");
            DropForeignKey("dbo.StoreFrontConfiguration", "RegisterSuccess_PageId", "dbo.Page");
            DropForeignKey("dbo.StoreFrontConfiguration", "RegisteredNotify_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreFrontConfiguration", "ProfileThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "OrdersThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "OrderAdminThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "OrderAdmin_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreFrontConfiguration", "NotificationsThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "NotFoundError_PageId", "dbo.Page");
            DropForeignKey("dbo.StoreFrontConfiguration", "DefaultNewPageThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreFrontConfiguration", "ClientId", "dbo.Client");
            DropForeignKey("dbo.StoreFrontConfiguration", "CheckoutThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "CheckoutPaymentInfoWebFormId", "dbo.WebForm");
            DropForeignKey("dbo.StoreFrontConfiguration", "CheckoutLogInOrGuestWebFormId", "dbo.WebForm");
            DropForeignKey("dbo.StoreFrontConfiguration", "CheckoutDeliveryMethodWebFormId", "dbo.WebForm");
            DropForeignKey("dbo.StoreFrontConfiguration", "CheckoutDeliveryInfoShippingWebFormId", "dbo.WebForm");
            DropForeignKey("dbo.StoreFrontConfiguration", "CheckoutDeliveryInfoDigitalOnlyWebFormId", "dbo.WebForm");
            DropForeignKey("dbo.StoreFrontConfiguration", "CheckoutConfirmOrderWebFormId", "dbo.WebForm");
            DropForeignKey("dbo.StoreFrontConfiguration", "CatalogThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "CatalogAdminThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "CartThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "AdminThemeId", "dbo.Theme");
            DropForeignKey("dbo.StoreFrontConfiguration", "AccountThemeId", "dbo.Theme");
            DropForeignKey("dbo.Theme", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Theme", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Client", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Theme", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Client", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientRole", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientRole", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientUserRole", "UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientUserRole", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientUserRole", "ScopeStoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.UserProfile", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.StoreFront", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreFrontConfiguration", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.StoreBinding", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreBinding", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.StoreBinding", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreBinding", "ClientId", "dbo.Client");
            DropForeignKey("dbo.StoreFront", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.StoreFront", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Cart", "UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Cart", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Cart", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Order", "UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Order", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Order", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Payment", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Payment", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Payment", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Payment", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Payment", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Payment", "CartId", "dbo.Cart");
            DropForeignKey("dbo.Order", "PaymentInfoWebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.OrderBundle", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.OrderBundle", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.OrderBundle", "ProductBundleId", "dbo.ProductBundle");
            DropForeignKey("dbo.OrderItem", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.OrderItem", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.OrderItem", "ProductBundleItemId", "dbo.ProductBundleItem");
            DropForeignKey("dbo.OrderItem", "ProductId", "dbo.Product");
            DropForeignKey("dbo.OrderItem", "OrderBundleId", "dbo.OrderBundle");
            DropForeignKey("dbo.OrderItem", "OrderId", "dbo.Order");
            DropForeignKey("dbo.OrderItem", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.OrderItem", "ClientId", "dbo.Client");
            DropForeignKey("dbo.OrderItem", "CartItemId", "dbo.CartItem");
            DropForeignKey("dbo.OrderBundle", "OrderId", "dbo.Order");
            DropForeignKey("dbo.OrderBundle", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.OrderBundle", "ClientId", "dbo.Client");
            DropForeignKey("dbo.OrderBundle", "CartBundleId", "dbo.CartBundle");
            DropForeignKey("dbo.Notification", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Notification", "ToUserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Notification", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Notification", "OrderId", "dbo.Order");
            DropForeignKey("dbo.NotificationLink", "NotificationId", "dbo.Notification");
            DropForeignKey("dbo.NotificationLink", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.NotificationLink", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.NotificationLink", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.NotificationLink", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Notification", "FromUserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Notification", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Notification", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Order", "LoginOrGuestWebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.Order", "DiscountId", "dbo.Discount");
            DropForeignKey("dbo.Order", "DeliveryInfoShippingId", "dbo.DeliveryInfoShipping");
            DropForeignKey("dbo.Order", "DeliveryInfoDigitalId", "dbo.DeliveryInfoDigital");
            DropForeignKey("dbo.Order", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Order", "ConfirmOrderWebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.Order", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Cart", "OrderId", "dbo.Order");
            DropForeignKey("dbo.Cart", "LoginOrGuestWebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.Cart", "DiscountId", "dbo.Discount");
            DropForeignKey("dbo.Discount", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Discount", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Discount", "FreeProduct_ProductId", "dbo.Product");
            DropForeignKey("dbo.Discount", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Discount", "ClientId", "dbo.Client");
            DropForeignKey("dbo.DeliveryInfoShipping", "DeliveryInfoShippingId", "dbo.Cart");
            DropForeignKey("dbo.DeliveryInfoShipping", "WebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.DeliveryInfoShipping", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.DeliveryInfoShipping", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.DeliveryInfoShipping", "DeliveryMethodWebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.DeliveryInfoShipping", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.DeliveryInfoShipping", "ClientId", "dbo.Client");
            DropForeignKey("dbo.DeliveryInfoDigital", "DeliveryInfoDigitalId", "dbo.Cart");
            DropForeignKey("dbo.DeliveryInfoDigital", "WebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.DeliveryInfoDigital", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.DeliveryInfoDigital", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.DeliveryInfoDigital", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.DeliveryInfoDigital", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Cart", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Cart", "ConfirmOrderWebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.Cart", "ClientId", "dbo.Client");
            DropForeignKey("dbo.CartPaymentInfo", "CartPaymentInfoId", "dbo.Cart");
            DropForeignKey("dbo.CartPaymentInfo", "WebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.CartPaymentInfo", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.CartPaymentInfo", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.CartPaymentInfo", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.CartPaymentInfo", "ClientId", "dbo.Client");
            DropForeignKey("dbo.CartItem", "CartId", "dbo.Cart");
            DropForeignKey("dbo.CartBundle", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.CartBundle", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.CartBundle", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.CartBundle", "ClientId", "dbo.Client");
            DropForeignKey("dbo.CartItem", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.CartItem", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.CartItem", "ProductId", "dbo.Product");
            DropForeignKey("dbo.Product", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Product", "ThemeId", "dbo.Theme");
            DropForeignKey("dbo.Product", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Product", "RequestAQuote_PageId", "dbo.Page");
            DropForeignKey("dbo.ProductReview", "UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductReview", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductReview", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.ProductReview", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductReview", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductReview", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Product", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Product", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ProductCategory", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategory", "ThemeId", "dbo.Theme");
            DropForeignKey("dbo.ProductCategory", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Product", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.ProductBundle", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductBundle", "ThemeId", "dbo.Theme");
            DropForeignKey("dbo.ProductBundle", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.ProductBundle", "RequestAQuote_PageId", "dbo.Page");
            DropForeignKey("dbo.WebFormField", "WebFormResponse_WebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.WebFormFieldResponse", "WebFormResponseId", "dbo.WebFormResponse");
            DropForeignKey("dbo.WebFormResponse", "WebFormId", "dbo.WebForm");
            DropForeignKey("dbo.WebFormResponse", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.WebFormResponse", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.WebFormResponse", "PageId", "dbo.Page");
            DropForeignKey("dbo.WebFormResponse", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.WebFormResponse", "ClientId", "dbo.Client");
            DropForeignKey("dbo.WebFormFieldResponse", "WebFormFieldId", "dbo.WebFormField");
            DropForeignKey("dbo.WebFormFieldResponse", "Value1ValueListItemId", "dbo.ValueListItem");
            DropForeignKey("dbo.WebFormFieldResponse", "Value1PageId", "dbo.ValueListItem");
            DropForeignKey("dbo.WebFormFieldResponse", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.WebFormFieldResponse", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.WebFormFieldResponse", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.WebFormFieldResponse", "ClientId", "dbo.Client");
            DropForeignKey("dbo.WebFormField", "WebFormId", "dbo.WebForm");
            DropForeignKey("dbo.WebFormField", "ValueListId", "dbo.ValueList");
            DropForeignKey("dbo.ValueListItem", "ValueListId", "dbo.ValueList");
            DropForeignKey("dbo.ValueListItem", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ValueListItem", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ValueListItem", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ValueList", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ValueList", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ValueList", "ClientId", "dbo.Client");
            DropForeignKey("dbo.WebFormField", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.WebFormField", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.WebFormField", "ClientId", "dbo.Client");
            DropForeignKey("dbo.WebForm", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Page", "WebFormId", "dbo.WebForm");
            DropForeignKey("dbo.WebForm", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.WebForm", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Page", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Page", "ThemeId", "dbo.Theme");
            DropForeignKey("dbo.Page", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.Page", "PageTemplateId", "dbo.PageTemplate");
            DropForeignKey("dbo.PageTemplate", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageTemplateSection", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageTemplateSection", "PageTemplateId", "dbo.PageTemplate");
            DropForeignKey("dbo.PageSection", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageSection", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.PageSection", "PageTemplateSectionId", "dbo.PageTemplateSection");
            DropForeignKey("dbo.PageSection", "PageId", "dbo.Page");
            DropForeignKey("dbo.PageSection", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageSection", "ClientId", "dbo.Client");
            DropForeignKey("dbo.PageTemplateSection", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageTemplateSection", "ClientId", "dbo.Client");
            DropForeignKey("dbo.PageTemplate", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.PageTemplate", "ClientId", "dbo.Client");
            DropForeignKey("dbo.NavBarItem", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.NavBarItem", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.NavBarItem", "ParentNavBarItemId", "dbo.NavBarItem");
            DropForeignKey("dbo.NavBarItem", "PageId", "dbo.Page");
            DropForeignKey("dbo.NavBarItem", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.NavBarItem", "ClientId", "dbo.Client");
            DropForeignKey("dbo.Page", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.Page", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ProductBundleItem", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductBundleItem", "StoreFrontId", "dbo.StoreFront");
            DropForeignKey("dbo.ProductBundleItem", "ProductBundleId", "dbo.ProductBundle");
            DropForeignKey("dbo.ProductBundleItem", "ProductId", "dbo.Product");
            DropForeignKey("dbo.ProductBundleItem", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductBundleItem", "ClientId", "dbo.Client");
            DropForeignKey("dbo.CartItem", "ProductBundleItemId", "dbo.ProductBundleItem");
            DropForeignKey("dbo.ProductBundle", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductBundle", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ProductBundle", "ProductCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.CartBundle", "ProductBundleId", "dbo.ProductBundle");
            DropForeignKey("dbo.ProductCategory", "ParentCategoryId", "dbo.ProductCategory");
            DropForeignKey("dbo.ProductCategory", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ProductCategory", "ClientId", "dbo.Client");
            DropForeignKey("dbo.CartItem", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.CartItem", "ClientId", "dbo.Client");
            DropForeignKey("dbo.CartItem", "CartBundleId", "dbo.CartBundle");
            DropForeignKey("dbo.CartBundle", "CartId", "dbo.Cart");
            DropForeignKey("dbo.ClientUserRole", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientUserRole", "ClientRoleId", "dbo.ClientRole");
            DropForeignKey("dbo.ClientUserRole", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ClientRoleAction", "UpdatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientRoleAction", "CreatedBy_UserProfileId", "dbo.UserProfile");
            DropForeignKey("dbo.ClientRoleAction", "ClientRoleId", "dbo.ClientRole");
            DropForeignKey("dbo.ClientRoleAction", "ClientId", "dbo.Client");
            DropForeignKey("dbo.ClientRole", "ClientId", "dbo.Client");
            DropForeignKey("dbo.StoreFrontConfiguration", "AccountAdmin_UserProfileId", "dbo.UserProfile");
            DropIndex("dbo.UserActionEvent", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.UserActionEvent", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.SystemEvent", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.SystemEvent", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.SmsSent", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.SmsSent", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.SecurityEvent", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.SecurityEvent", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.PageViewEvent", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.PageViewEvent", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.GiftCard", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.GiftCard", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.GiftCard", "UniqueRecord");
            DropIndex("dbo.FileNotFoundLog", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.FileNotFoundLog", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.EmailSent", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.EmailSent", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.StoreBinding", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.StoreBinding", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.StoreBinding", "UniqueRecord");
            DropIndex("dbo.Payment", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Payment", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Payment", new[] { "CartId" });
            DropIndex("dbo.Payment", new[] { "OrderId" });
            DropIndex("dbo.Payment", "UniqueRecord");
            DropIndex("dbo.OrderItem", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.OrderItem", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.OrderItem", "UniqueRecord");
            DropIndex("dbo.OrderItem", new[] { "ProductBundleItemId" });
            DropIndex("dbo.OrderItem", new[] { "OrderBundleId" });
            DropIndex("dbo.OrderItem", new[] { "ProductId" });
            DropIndex("dbo.OrderItem", new[] { "OrderId" });
            DropIndex("dbo.OrderBundle", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.OrderBundle", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.OrderBundle", "UniqueRecord");
            DropIndex("dbo.OrderBundle", new[] { "CartBundleId" });
            DropIndex("dbo.NotificationLink", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.NotificationLink", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.NotificationLink", "UniqueRecord");
            DropIndex("dbo.Notification", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Notification", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Notification", new[] { "OrderId" });
            DropIndex("dbo.Notification", new[] { "FromUserProfileId" });
            DropIndex("dbo.Notification", new[] { "ToUserProfileId" });
            DropIndex("dbo.Notification", "UniqueRecord");
            DropIndex("dbo.Order", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Order", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Order", new[] { "ConfirmOrderWebFormResponseId" });
            DropIndex("dbo.Order", new[] { "PaymentInfoWebFormResponseId" });
            DropIndex("dbo.Order", new[] { "LoginOrGuestWebFormResponseId" });
            DropIndex("dbo.Order", new[] { "DiscountId" });
            DropIndex("dbo.Order", new[] { "DeliveryInfoShippingId" });
            DropIndex("dbo.Order", new[] { "DeliveryInfoDigitalId" });
            DropIndex("dbo.Order", new[] { "UserProfileId" });
            DropIndex("dbo.Order", "UniqueRecord");
            DropIndex("dbo.Discount", new[] { "FreeProduct_ProductId" });
            DropIndex("dbo.Discount", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Discount", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Discount", "UniqueRecord");
            DropIndex("dbo.DeliveryInfoShipping", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.DeliveryInfoShipping", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.DeliveryInfoShipping", new[] { "DeliveryMethodWebFormResponseId" });
            DropIndex("dbo.DeliveryInfoShipping", new[] { "WebFormResponseId" });
            DropIndex("dbo.DeliveryInfoShipping", "UniqueRecord");
            DropIndex("dbo.DeliveryInfoShipping", new[] { "DeliveryInfoShippingId" });
            DropIndex("dbo.DeliveryInfoDigital", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.DeliveryInfoDigital", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.DeliveryInfoDigital", new[] { "WebFormResponseId" });
            DropIndex("dbo.DeliveryInfoDigital", "UniqueRecord");
            DropIndex("dbo.DeliveryInfoDigital", new[] { "DeliveryInfoDigitalId" });
            DropIndex("dbo.CartPaymentInfo", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.CartPaymentInfo", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.CartPaymentInfo", new[] { "WebFormResponseId" });
            DropIndex("dbo.CartPaymentInfo", "UniqueRecord");
            DropIndex("dbo.CartPaymentInfo", new[] { "CartPaymentInfoId" });
            DropIndex("dbo.ProductReview", new[] { "UserProfile_UserProfileId" });
            DropIndex("dbo.ProductReview", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductReview", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductReview", "UniqueRecord");
            DropIndex("dbo.WebFormResponse", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.WebFormResponse", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.WebFormResponse", new[] { "PageId" });
            DropIndex("dbo.WebFormResponse", new[] { "WebFormId" });
            DropIndex("dbo.WebFormResponse", "UniqueRecord");
            DropIndex("dbo.WebFormFieldResponse", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.WebFormFieldResponse", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.WebFormFieldResponse", new[] { "Value1PageId" });
            DropIndex("dbo.WebFormFieldResponse", new[] { "Value1ValueListItemId" });
            DropIndex("dbo.WebFormFieldResponse", "UniqueRecord");
            DropIndex("dbo.ValueListItem", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ValueListItem", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ValueListItem", "UniqueRecord");
            DropIndex("dbo.ValueList", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ValueList", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ValueList", "UniqueRecord");
            DropIndex("dbo.WebFormField", new[] { "WebFormResponse_WebFormResponseId" });
            DropIndex("dbo.WebFormField", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.WebFormField", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.WebFormField", new[] { "ValueListId" });
            DropIndex("dbo.WebFormField", "UniqueRecord");
            DropIndex("dbo.WebForm", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.WebForm", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.WebForm", "UniqueRecord");
            DropIndex("dbo.PageSection", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.PageSection", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.PageSection", "UniqueRecord");
            DropIndex("dbo.PageTemplateSection", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.PageTemplateSection", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.PageTemplateSection", "UniqueRecord");
            DropIndex("dbo.PageTemplate", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.PageTemplate", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.PageTemplate", "UniqueRecord");
            DropIndex("dbo.NavBarItem", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.NavBarItem", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.NavBarItem", new[] { "ParentNavBarItemId" });
            DropIndex("dbo.NavBarItem", new[] { "PageId" });
            DropIndex("dbo.NavBarItem", "UniqueRecord");
            DropIndex("dbo.Page", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Page", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Page", new[] { "WebFormId" });
            DropIndex("dbo.Page", "UniqueRecord");
            DropIndex("dbo.Page", new[] { "ThemeId" });
            DropIndex("dbo.Page", new[] { "PageTemplateId" });
            DropIndex("dbo.ProductBundleItem", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductBundleItem", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductBundleItem", "UniqueRecord");
            DropIndex("dbo.ProductBundle", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductBundle", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductBundle", new[] { "ThemeId" });
            DropIndex("dbo.ProductBundle", new[] { "ProductCategoryId" });
            DropIndex("dbo.ProductBundle", new[] { "RequestAQuote_PageId" });
            DropIndex("dbo.ProductBundle", "UniqueRecord");
            DropIndex("dbo.ProductCategory", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategory", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ProductCategory", new[] { "ThemeId" });
            DropIndex("dbo.ProductCategory", new[] { "ParentCategoryId" });
            DropIndex("dbo.ProductCategory", "UniqueRecord");
            DropIndex("dbo.Product", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Product", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Product", new[] { "ThemeId" });
            DropIndex("dbo.Product", new[] { "ProductCategoryId" });
            DropIndex("dbo.Product", new[] { "RequestAQuote_PageId" });
            DropIndex("dbo.Product", "UniqueRecord");
            DropIndex("dbo.CartItem", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.CartItem", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.CartItem", new[] { "ProductBundleItemId" });
            DropIndex("dbo.CartItem", "UniqueRecord");
            DropIndex("dbo.CartBundle", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.CartBundle", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.CartBundle", "UniqueRecord");
            DropIndex("dbo.Cart", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Cart", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Cart", new[] { "ConfirmOrderWebFormResponseId" });
            DropIndex("dbo.Cart", new[] { "LoginOrGuestWebFormResponseId" });
            DropIndex("dbo.Cart", new[] { "DiscountId" });
            DropIndex("dbo.Cart", "UniqueRecord");
            DropIndex("dbo.StoreFront", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.StoreFront", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.StoreFront", new[] { "ClientId", "StoreFrontId" });
            DropIndex("dbo.ClientUserRole", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ClientUserRole", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ClientUserRole", "UniqueRecord");
            DropIndex("dbo.ClientRoleAction", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ClientRoleAction", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ClientRoleAction", "UniqueRecord");
            DropIndex("dbo.ClientRole", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.ClientRole", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.ClientRole", "UniqueRecord");
            DropIndex("dbo.Client", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Client", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Client", new[] { "Folder" });
            DropIndex("dbo.Client", new[] { "Name" });
            DropIndex("dbo.Client", "UniqueRecord");
            DropIndex("dbo.Theme", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.Theme", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.Theme", "UniqueRecord");
            DropIndex("dbo.StoreFrontConfiguration", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "RegisterSuccess_PageId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "Register_WebFormId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "StoreError_PageId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "NotFoundError_PageId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CatalogAdminThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "OrderAdminThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "OrdersThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CheckoutConfirmOrderWebFormId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CheckoutPaymentInfoWebFormId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CheckoutDeliveryMethodWebFormId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CheckoutDeliveryInfoShippingWebFormId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CheckoutDeliveryInfoDigitalOnlyWebFormId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CheckoutLogInOrGuestWebFormId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CheckoutThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CartThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "AdminThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "ProfileThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "NotificationsThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "AccountThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "CatalogThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "DefaultNewPageThemeId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "OrderAdmin_UserProfileId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "WelcomePerson_UserProfileId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "RegisteredNotify_UserProfileId" });
            DropIndex("dbo.StoreFrontConfiguration", new[] { "AccountAdmin_UserProfileId" });
            DropIndex("dbo.StoreFrontConfiguration", "UniqueRecord");
            DropIndex("dbo.UserProfile", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.UserProfile", new[] { "CreatedBy_UserProfileId" });
            DropIndex("dbo.UserProfile", new[] { "RegisterWebFormResponseId" });
            DropIndex("dbo.UserProfile", new[] { "ClientId" });
            DropIndex("dbo.UserProfile", new[] { "StoreFrontId" });
            DropIndex("dbo.UserProfile", new[] { "Email" });
            DropIndex("dbo.UserProfile", new[] { "UserName" });
            DropIndex("dbo.UserProfile", "UniqueRecord");
            DropIndex("dbo.BadRequest", new[] { "UpdatedBy_UserProfileId" });
            DropIndex("dbo.BadRequest", new[] { "CreatedBy_UserProfileId" });
            DropTable("dbo.UserActionEvent");
            DropTable("dbo.SystemEvent");
            DropTable("dbo.SmsSent");
            DropTable("dbo.SecurityEvent");
            DropTable("dbo.PageViewEvent");
            DropTable("dbo.GiftCard");
            DropTable("dbo.FileNotFoundLog");
            DropTable("dbo.EmailSent");
            DropTable("dbo.StoreBinding");
            DropTable("dbo.Payment");
            DropTable("dbo.OrderItem");
            DropTable("dbo.OrderBundle");
            DropTable("dbo.NotificationLink");
            DropTable("dbo.Notification");
            DropTable("dbo.Order");
            DropTable("dbo.Discount");
            DropTable("dbo.DeliveryInfoShipping");
            DropTable("dbo.DeliveryInfoDigital");
            DropTable("dbo.CartPaymentInfo");
            DropTable("dbo.ProductReview");
            DropTable("dbo.WebFormResponse");
            DropTable("dbo.WebFormFieldResponse");
            DropTable("dbo.ValueListItem");
            DropTable("dbo.ValueList");
            DropTable("dbo.WebFormField");
            DropTable("dbo.WebForm");
            DropTable("dbo.PageSection");
            DropTable("dbo.PageTemplateSection");
            DropTable("dbo.PageTemplate");
            DropTable("dbo.NavBarItem");
            DropTable("dbo.Page");
            DropTable("dbo.ProductBundleItem");
            DropTable("dbo.ProductBundle");
            DropTable("dbo.ProductCategory");
            DropTable("dbo.Product");
            DropTable("dbo.CartItem");
            DropTable("dbo.CartBundle");
            DropTable("dbo.Cart");
            DropTable("dbo.StoreFront");
            DropTable("dbo.ClientUserRole");
            DropTable("dbo.ClientRoleAction");
            DropTable("dbo.ClientRole");
            DropTable("dbo.Client");
            DropTable("dbo.Theme");
            DropTable("dbo.StoreFrontConfiguration");
            DropTable("dbo.UserProfile");
            DropTable("dbo.BadRequest");
        }
    }
}
