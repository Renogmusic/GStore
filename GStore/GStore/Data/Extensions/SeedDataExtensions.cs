using GStore.Identity;
using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Data
{
	public static class SeedDataExtensions
	{
		public static void AddSeedData(this IGstoreDb storeDb)
		{

			string adminUserName = "admin@domain.com";
			string adminEmail = "admin@domain.com";
			string adminPhone = "888-555-1212";
			string adminPassword = "password";
			string adminFullName = "System Admin";

			string browserUserName = "user@domain.com";
			string browserEmail = "user@domain.com";
			string browserPhone = "888-555-1212";
			string browserPassword = "password";
			string browserFullName = "John Doe User";


			string layout = Properties.Settings.Current.AppDefaultLayoutName;
			string pageTemplateViewName = Properties.Settings.Current.AppDefaultPageTemplateViewName;
			string preferedThemeName = "StarterTheme";
			bool loadSampleProducts = Properties.Settings.Current.AppSeedSampleProducts;


			if (HttpContext.Current != null)
			{
				EventLogExtensions.CreateEventLogFolders(HttpContext.Current);
			}

			Identity.AspNetIdentityContext ctx = new Identity.AspNetIdentityContext();
			ctx.CreateRoleIfNotExists("AccountAdmin");
			ctx.CreateRoleIfNotExists("NotificationAdmin");
			ctx.CreateRoleIfNotExists("StoreAdmin");
			ctx.CreateRoleIfNotExists("ClientAdmin");
			ctx.CreateRoleIfNotExists("SystemAdmin");
			Identity.AspNetIdentityUser adminAspNetUser = ctx.CreateSystemAdminUserIfNotExists(adminUserName, adminEmail, adminPhone, adminPassword);
			Identity.AspNetIdentityUser browserUser = ctx.CreateUserIfNotExists(browserUserName, browserEmail, browserPhone, browserPassword);
			ctx.Dispose();


			UserProfile adminProfile = storeDb.GetUserProfileByEmail(adminUserName, false);
			//Create admin user profile if it does not exist
			if (adminProfile == null)
			{
				adminProfile = storeDb.CreateSeedAdminProfile(adminAspNetUser.Id, adminUserName, adminEmail, adminFullName);
			}
			storeDb.UserName = adminProfile.UserName;
			storeDb.CachedUserProfile = adminProfile;

			string virtualPathToThemes = "~/Content/Server/Themes";
			if (storeDb.Themes.IsEmpty())
			{
				storeDb.CreateSeedThemes(virtualPathToThemes);
			}
			if (storeDb.Themes.IsEmpty())
			{
				throw new ApplicationException("No themes found; add themes to " + virtualPathToThemes + "/(ThemeName) folders");
			}
			Theme selectedTheme = storeDb.Themes.Where(t => t.Name == preferedThemeName.ToLower()).FirstOrDefault();
			if (selectedTheme == null)
			{
				//select random theme
				int themeCount = storeDb.Themes.All().Count();
				Random rndNumber = new Random();
				int randomThemeIndex = rndNumber.Next(1, themeCount);
				selectedTheme = storeDb.Themes.All().OrderBy(t => t.Order).Skip(randomThemeIndex - 1).First();
			}

			if (storeDb.Clients.IsEmpty())
			{
				Client newClient = storeDb.CreateSeedClient("New Company", "StarterClient");
			}
			Client firstClient = storeDb.Clients.All().First();

			if (storeDb.StoreFronts.IsEmpty())
			{
				string storeFrontName = "New Storefront";
				string storeFrontFolder = "StarterStoreFront";
				StoreFront newStoreFront = storeDb.CreateSeedStoreFront(storeFrontName, storeFrontFolder, firstClient, adminProfile, selectedTheme, layout);
			}
			StoreFront firstStoreFront = storeDb.StoreFronts.All().First();
			storeDb.CachedStoreFront = firstStoreFront;
			firstClient = firstStoreFront.Client;

			if (storeDb.StoreBindings.IsEmpty())
			{
				StoreBinding storeBinding = storeDb.CreateSeedStoreBindingToCurrentUrl(firstStoreFront);
			}

			if (storeDb.PageTemplates.IsEmpty())
			{
				PageTemplate newPageTemplate = storeDb.CreateSeedPageTemplate(layout + " Template", layout, pageTemplateViewName);
			}

			PageTemplate firstPageTemplate = storeDb.PageTemplates.All().First();

			if (storeDb.Pages.IsEmpty())
			{
				Page page1 = storeDb.CreateSeedPage("New Home Page", string.Empty, "/", 100, firstStoreFront, firstPageTemplate);

				Page page2 = storeDb.CreateSeedPage("About Us", "About Us", "/About", 200, firstStoreFront, firstPageTemplate);
				NavBarItem aboutLink = storeDb.CreateSeedNavBarItem("About Us", page2.PageId, false, null, firstStoreFront);

				Page page3 = storeDb.CreateSeedPage("Contact Us", "Contact Us", "/Contact", 300, firstStoreFront, firstPageTemplate);
				NavBarItem contactLink = storeDb.CreateSeedNavBarItem("Contact Us", page3.PageId, false, null, firstStoreFront);

				Page page4 = storeDb.CreateSeedPage("Questions?", "Questions?", "/Answers", 400, firstStoreFront, firstPageTemplate);
				NavBarItem answersLink = storeDb.CreateSeedNavBarItem("Questions?", page4.PageId, false, null, firstStoreFront);

				Page page5 = storeDb.CreateSeedPage("Location", "Location", "/Location", 500, firstStoreFront, firstPageTemplate);
				NavBarItem locationLink = storeDb.CreateSeedNavBarItem("Location", page5.PageId, false, null, firstStoreFront);

				//NavBarItem aboutGStoreLink = storeDb.CreateSeedNavBarItemForAction("About GStore", 9999, "About", "GStore", string.Empty, false, null, firstStoreFront);

			}

			//add browser user
			UserProfile browserProfile = browserUser.GetUserProfile(storeDb, false);
			if (browserProfile == null)
			{
				browserProfile = storeDb.CreateSeedUserProfile(browserUser.Id, browserUserName, browserEmail, browserFullName, firstStoreFront);
			}

			if (loadSampleProducts && storeDb.ProductCategories.IsEmpty())
			{
				ProductCategory topCat = storeDb.CreateSeedProductCategory("Computers and Tablets", "Computers_And_Tablets", 100, false, null, firstStoreFront);
				ProductCategory notebooks = storeDb.CreateSeedProductCategory("Notebooks", "Notebooks", 200, false, topCat, firstStoreFront);

				ProductCategory studentNotebooks = storeDb.CreateSeedProductCategory("Student Notebooks", "Student_Notebooks", 220, false, notebooks, firstStoreFront);
				
				ProductCategory gamingNotebooks = storeDb.CreateSeedProductCategory("Gaming Notebooks", "Gaming_Notebooks", 230, false, notebooks, firstStoreFront);
				ProductCategory desktops = storeDb.CreateSeedProductCategory("Desktops", "Desktops", 300, false, topCat, firstStoreFront);
				ProductCategory gamingDesktops = storeDb.CreateSeedProductCategory("Gaming Desktops", "Gaming_Desktops", 350, false, desktops, firstStoreFront);
				ProductCategory officeDesktops = storeDb.CreateSeedProductCategory("Office Desktops", "Office_Desktops", 380, false, desktops, firstStoreFront);
				ProductCategory valueDesktops = storeDb.CreateSeedProductCategory("Value Desktops", "Value_Desktops", 390, false, desktops, firstStoreFront);
				ProductCategory budgetDesktops = storeDb.CreateSeedProductCategory("Budget Desktops", "Budget_Desktops", 390, false, valueDesktops, firstStoreFront);
				ProductCategory refurbishedDesktops = storeDb.CreateSeedProductCategory("Refurbished Desktops", "Refurbished_Desktops", 395, false, valueDesktops, firstStoreFront);
				ProductCategory offLeaseDesktops = storeDb.CreateSeedProductCategory("Off-Lease Desktops", "Off_Lease_Desktops", 398, false, refurbishedDesktops, firstStoreFront);
				ProductCategory manufacturerRefurbishedDesktops = storeDb.CreateSeedProductCategory("Manufacturer Refurbished", "Manufacturer_Refurbished_Desktops", 399, false, refurbishedDesktops, firstStoreFront);

				ProductCategory tablets = storeDb.CreateSeedProductCategory("Tablets", "Tablets", 400, false, topCat, firstStoreFront);

				//Defer category count updating until the whole set of products is loaded instead of one by one to save cpu cycles
				Product productA1 = storeDb.CreateSeedProduct("Toshiba A205", "Toshiba_A205", 100, gamingNotebooks, firstStoreFront, false);
				Product productA2 = storeDb.CreateSeedProduct("Lenovo B512A", "Levovo_B512A", 200, gamingNotebooks, firstStoreFront, false);
				Product productA3 = storeDb.CreateSeedProduct("Acer A512A", "Acer_A512A", 300, gamingNotebooks, firstStoreFront, false);

				Product productB1 = storeDb.CreateSeedProduct("Toshiba A205-S", "Toshiba_A205_S", 100, studentNotebooks, firstStoreFront, false);
				Product productB2 = storeDb.CreateSeedProduct("Lenovo B512B", "Levovo_B512B", 200, studentNotebooks, firstStoreFront, false);
				Product productB3 = storeDb.CreateSeedProduct("Acer A512B", "Acer_A512B", 300, studentNotebooks, firstStoreFront, false);

				Product productC1 = storeDb.CreateSeedProduct("HP Pavillion B11212", "HP_Pavillion_B11212", 990, offLeaseDesktops, firstStoreFront, false);
				Product productC2 = storeDb.CreateSeedProduct("HP Pavillion B666", "HP_Pavillion_B666", 300, offLeaseDesktops, firstStoreFront, false);


				//run defered category calc
				storeDb.RecalculateProductCategoryActiveCount(firstStoreFront);
				
			}


		}

		public static UserProfile CreateSeedAdminProfile(this IGstoreDb storeDb, string adminUserId, string adminUserName, string adminEmail, string adminFullName)
		{
			UserProfile adminProfile = storeDb.UserProfiles.Create();
			adminProfile.UserId = adminUserId;
			//adminProfile.StoreFront = storeFront;
			adminProfile.AllowUsersToSendSiteMessages = true;
			adminProfile.Email = adminEmail;
			adminProfile.FullName = adminFullName;
			adminProfile.NotifyAllWhenLoggedOn = false;
			adminProfile.NotifyOfSiteUpdatesToEmail = true;
			adminProfile.SendMoreInfoToEmail = false;
			adminProfile.SendSiteMessagesToEmail = true;
			adminProfile.SendSiteMessagesToSms = false;
			adminProfile.SubscribeToNewsletterEmail = true;
			adminProfile.UserName = adminUserName;
			adminProfile.IsPending = false;
			adminProfile.Order = 1;
			adminProfile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			adminProfile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			storeDb.UserProfiles.Add(adminProfile);
			storeDb.SaveChangesEx(true, false, false, false);

			return adminProfile;
		}

		public static UserProfile CreateSeedUserProfile(this IGstoreDb storeDb, string userId, string userName, string email, string fullName, StoreFront storeFront)
		{
			UserProfile profile = storeDb.UserProfiles.Create();
			profile.UserId = userId;
			profile.StoreFrontId = storeFront.StoreFrontId;
			profile.ClientId = storeFront.ClientId;
			profile.AllowUsersToSendSiteMessages = true;
			profile.Email = email;
			profile.FullName = fullName;
			profile.NotifyAllWhenLoggedOn = true;
			profile.NotifyOfSiteUpdatesToEmail = true;
			profile.SendMoreInfoToEmail = false;
			profile.SendSiteMessagesToEmail = true;
			profile.SendSiteMessagesToSms = false;
			profile.SubscribeToNewsletterEmail = true;
			profile.UserName = userName;
			profile.IsPending = true;
			profile.Order = 100;
			profile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			profile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			storeDb.UserProfiles.Add(profile);
			storeDb.SaveChangesEx(true, false, false, false);

			return profile;
		}

		public static void CreateSeedThemes(this IGstoreDb storeDb, string virtualPath)
		{
			string path = string.Empty;
			if (HttpContext.Current == null)
			{
				string assemblyPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
				string directoryName = System.IO.Path.GetDirectoryName(assemblyPath);
				path = System.IO.Path.Combine(directoryName, ".." + virtualPath.TrimStart('~').Replace('/', '\\')).Replace("%20", " ");
			}
			else
			{
				path = HttpContext.Current.Server.MapPath(virtualPath);
			}

			System.IO.DirectoryInfo themesFolder = new System.IO.DirectoryInfo(path);
			IEnumerable<System.IO.DirectoryInfo> themeFolders = themesFolder.EnumerateDirectories();
			int counter = 0;
			foreach (System.IO.DirectoryInfo themeFolder in themeFolders)
			{
				counter ++;
				Theme theme = storeDb.Themes.Create();
				theme.Name = themeFolder.Name;
				theme.Order = 2000 + counter;
				theme.FolderName = themeFolder.Name;
				theme.Description = themeFolder.Name + " theme";
				theme.IsPending = false;
				theme.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				theme.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

				storeDb.Themes.Add(theme);
			}
			storeDb.SaveChangesEx(true, false, false, false);
		}

		public static Client CreateSeedClient(this IGstoreDb storeDb, string clientName, string clientFolder)
		{
			Client client = storeDb.Clients.Create();
			client.IsPending = false;
			client.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			client.Name = clientName;
			client.Order = 100;
			client.Folder = clientFolder;
			client.UseSendGridEmail = false;
			client.UseTwilioSms = false;
			client.EnableNewUserRegisteredBroadcast = true;
			client.EnablePageViewLog = true;
			storeDb.Clients.Add(client);
			storeDb.SaveChangesEx(true, false, false, false);

			return client;
		}

		public static StoreFront CreateSeedStoreFront(this IGstoreDb storeDb, string storeFrontName, string storeFrontFolder, Client client, UserProfile adminProfile, Theme selectedTheme, string layout)
		{
			StoreFront storeFront = storeDb.StoreFronts.Create();

			storeFront.Name = storeFrontName;
			storeFront.Folder = storeFrontFolder;
			storeFront.IsPending = false;
			storeFront.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			storeFront.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeFront.Client = client;
			storeFront.Order = 100;
			storeFront.AccountAdmin = adminProfile;
			storeFront.RegisteredNotify = adminProfile;
			storeFront.WelcomePerson = adminProfile;
			storeFront.MetaApplicationName = storeFrontName;
			storeFront.MetaApplicationTileColor = "#880088";
			storeFront.MetaDescription = "New GStore Storefront " + storeFrontName;
			storeFront.MetaKeywords = "GStore Storefront " + storeFrontName;
			storeFront.AdminLayoutName = layout;
			storeFront.AdminTheme = selectedTheme; 
			storeFront.AccountLayoutName = layout;
			storeFront.AccountTheme = selectedTheme; 
			storeFront.ProfileLayoutName = layout;
			storeFront.ProfileTheme = selectedTheme;
			storeFront.NotificationsLayoutName = layout;
			storeFront.NotificationsTheme = selectedTheme;
			storeFront.CatalogLayoutName = layout;
			storeFront.CatalogTheme = selectedTheme;
			storeFront.DefaultNewPageLayoutName = layout;
			storeFront.DefaultNewPageTheme = selectedTheme;
			storeFront.CatalogPageInitialLevels = 6;
			storeFront.NavBarCatalogMaxLevels = 6;
			storeFront.NavBarItemsMaxLevels = 6;
			storeFront.CatalogCategoryColLg = 3;
			storeFront.CatalogCategoryColMd = 4;
			storeFront.CatalogCategoryColSm = 6;
			storeFront.CatalogProductColLg = 2;
			storeFront.CatalogProductColMd = 3;
			storeFront.CatalogProductColSm = 6;
			storeFront.HtmlFooter = storeFrontName;
			storeFront.NavBarShowRegisterLink = true;
			storeFront.NavBarRegisterLinkText = "Sign-Up";
			storeFront.AccountLoginShowRegisterLink = true;
			storeFront.AccountLoginRegisterLinkText = "Sign-up";

			if (HttpContext.Current == null)
			{
				storeFront.PublicUrl = "http://localhost:55520/";
			}
			else
			{

				Uri url = HttpContext.Current.Request.Url;
				string publicUrl = "http://" + url.Host;
				if (!url.IsDefaultPort)
				{
					publicUrl += ":" + url.Port;
				}
				publicUrl += HttpContext.Current.Request.ApplicationPath;
				storeFront.PublicUrl = publicUrl;
			}
			storeDb.StoreFronts.Add(storeFront);
			storeDb.SaveChangesEx(true, false, false, false);

			return storeFront;

		}

		public static StoreBinding CreatAutoMapStoreBindingToCurrentUrl(this IGstoreDb storeDb, Controllers.BaseClass.BaseController baseController)
		{
			if (HttpContext.Current == null)
			{
				throw new ApplicationException("Cannot create auto-map binding when HttpContext.Current is null");
			}
			HttpRequestBase request = baseController.Request;

			UserProfile profile = storeDb.SeedAutoMapUserBestGuess();
			StoreFront storeFront = storeDb.SeedAutoMapStoreFrontBestGuess();

			IGstoreDb systemDb = storeDb.NewContext(profile.UserName, storeFront, profile);
			StoreBinding binding = systemDb.CreateSeedStoreBindingToCurrentUrl(storeFront);

			string message = "--Bindings auto-mapped to StoreFront '" + binding.StoreFront.Name + "' [" + binding.StoreFront.StoreFrontId + "]"
				+ " For HostName: " + binding.HostName + " Port: " + binding.Port + " RootPath: " + binding.RootPath
				+ " UseUrlStoreName: " + binding.UseUrlStoreName.ToString() + " UrlStoreName: " + binding.UrlStoreName.ToString()
				+ " From RawUrl: " + request.RawUrl + " QueryString: " + request.QueryString + " ContentLength: " + request.ContentLength
				+ " HTTPMethod: " + request.HttpMethod + " Client IP: " + request.UserHostAddress;

			System.Diagnostics.Trace.WriteLine(message);
			EventLogExtensions.LogSystemEvent(systemDb, baseController.HttpContext, baseController.RouteData, baseController.RouteData.ToSourceString(), SystemEventLevel.Information, message, string.Empty, string.Empty, string.Empty, baseController);

			return binding;

		}

		public static StoreBinding CreatAutoMapStoreBindingToCatchAll(this IGstoreDb storeDb, GStore.Controllers.BaseClass.BaseController baseController)
		{
			UserProfile profile = storeDb.SeedAutoMapUserBestGuess();
			StoreFront storeFront = storeDb.SeedAutoMapStoreFrontBestGuess();

			IGstoreDb systemDb = storeDb;
			systemDb.UserName = profile.UserName;
			systemDb.CachedStoreFront = storeFront;
			systemDb.CachedUserProfile = profile;
			string urlStoreName = baseController.RouteData.UrlStoreName();
			StoreBinding binding = systemDb.CreateSeedStoreBindingToCatchAll(storeFront, urlStoreName);

			HttpRequestBase request = baseController.Request;

			string message = "--Bindings Catch-All auto-mapped to StoreFront '" + binding.StoreFront.Name + "' [" + binding.StoreFront.StoreFrontId + "]"
				+ " For HostName: " + request.BindingHostName() + " Port: " + request.BindingPort() + " RootPath: " + request.BindingRootPath() + " UrlStoreName: " + request.BindingUrlStoreName()
				+ " UseUrlStoreName: " + binding.UseUrlStoreName.ToString() + " UrlStoreName: " + binding.UrlStoreName
				+ " From RawUrl: " + request.RawUrl + " QueryString: " + request.QueryString + " ContentLength: " + request.ContentLength
				+ " HTTPMethod: " + request.HttpMethod + " Client IP: " + request.UserHostAddress;

			System.Diagnostics.Trace.WriteLine(message);

			EventLogExtensions.LogSystemEvent(systemDb, baseController.HttpContext, baseController.RouteData, baseController.RouteData.ToSourceString(), SystemEventLevel.Information, message, string.Empty, string.Empty, string.Empty, baseController);
			return binding;

		}

		public static StoreFront SeedAutoMapStoreFrontBestGuess(this IGstoreDb storeDb)
		{

			//find a suitable store front for auto-map
			if (storeDb.StoreFronts.IsEmpty())
			{
				throw new ApplicationException("No storefronts in database for auto-mapping. Be sure database is seeded.");
			}

			StoreFront storeFrontFirstActive = storeDb.StoreFronts.All().WhereIsActive().OrderByDescending(sf=>sf.StoreFrontId).FirstOrDefault();
			if (storeFrontFirstActive != null)
			{
				return storeFrontFirstActive;
			}

			var activeClientInactiveStoreFrontQuery = from sf in storeDb.StoreFronts.All()
													  join c in storeDb.Clients.All().WhereIsActive()
													  on sf.ClientId equals c.ClientId
													  orderby sf.StoreFrontId descending, c.ClientId descending
													  select sf;

			//same qurery in lambda syntax
			//var activeClientInactiveStoreFrontQuery2 = storeDb.StoreFronts.All().Join(
			//	storeDb.Clients.All().WhereIsActive(),
			//	sf => sf.ClientId,
			//	c => c.ClientId,
			//	(sf, c) => sf
			//	);

			StoreFront storeFrontFirstInactiveWithActiveClient = activeClientInactiveStoreFrontQuery.FirstOrDefault();
			if (storeFrontFirstInactiveWithActiveClient != null)
			{
				return storeFrontFirstInactiveWithActiveClient;
			}

			//no active storefront, pick the first inactive one
			return storeDb.StoreFronts.All().OrderBy(sf => sf.StoreFrontId).FirstOrDefault();
		}


		public static UserProfile SeedAutoMapUserBestGuess(this IGstoreDb storeDb)
		{

			//find a suitable user for auto-map
			if (storeDb.UserProfiles.IsEmpty())
			{
				throw new ApplicationException("No users in database. Cannot auto-map. Be sure to seed database.");
			}

			string userName = storeDb.UserName;
			if (!string.IsNullOrEmpty(userName ))
			{
				UserProfile currentUserIfAdmin = storeDb.UserProfiles.All().WhereIsActive().Where(up => up.ClientId == null && up.UserName == userName).OrderBy(up => up.UserProfileId).FirstOrDefault();
				if (currentUserIfAdmin != null)
				{
					return currentUserIfAdmin;
				}
			}
			
			UserProfile profileNullClientIdAndActive = storeDb.UserProfiles.All().WhereIsActive().Where(up => up.ClientId == null).OrderBy(up => up.UserProfileId).FirstOrDefault();
			if (profileNullClientIdAndActive != null)
			{
				return profileNullClientIdAndActive;
			}

			UserProfile profileActiveAny = storeDb.UserProfiles.All().WhereIsActive().OrderBy(up => up.UserProfileId).FirstOrDefault();
			if (profileActiveAny != null)
			{
				return profileActiveAny;
			}

			//no active profiles, pick first inactive one
			return storeDb.UserProfiles.All().OrderBy(up => up.UserProfileId).First();

		}

		public static StoreBinding CreateSeedStoreBindingToCatchAll(this IGstoreDb storeDb, StoreFront storeFront, string urlStoreName)
		{
			StoreBinding storeBinding = storeDb.StoreBindings.Create();
			storeBinding.ClientId = storeFront.ClientId;
			storeBinding.StoreFrontId = storeFront.StoreFrontId;
			storeBinding.IsPending = false;
			storeBinding.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			storeBinding.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			storeBinding.HostName = "*";
			storeBinding.Port = 0;
			storeBinding.RootPath = "*";

			if (!string.IsNullOrEmpty(urlStoreName))
			{
				storeBinding.UseUrlStoreName = true;
				storeBinding.UrlStoreName = "*";
			}
			storeDb.StoreBindings.Add(storeBinding);
			storeDb.SaveChangesEx(true, false, false, false);

			return storeBinding;
		}

		public static StoreBinding CreateSeedStoreBindingToCurrentUrl(this IGstoreDb storeDb, StoreFront storeFront)
		{
			StoreBinding storeBinding = storeDb.StoreBindings.Create();
			storeBinding.ClientId = storeFront.ClientId;
			storeBinding.StoreFrontId = storeFront.StoreFrontId;
			storeBinding.IsPending = false;
			storeBinding.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			storeBinding.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeBinding.RootPath = "/";
			if (HttpContext.Current == null)
			{
				storeBinding.HostName = "*";
				storeBinding.Port = 0;
				storeBinding.RootPath = "*";
			}
			else
			{
				Uri url = HttpContext.Current.Request.Url;
				storeBinding.HostName = url.Host;
				storeBinding.Port = (url.IsDefaultPort ? 80 : (int?)url.Port);
				storeBinding.RootPath = HttpContext.Current.Request.ApplicationPath;
				storeBinding.UseUrlStoreName = (!string.IsNullOrEmpty(HttpContext.Current.Request.RequestContext.RouteData.UrlStoreName()));
				storeBinding.UrlStoreName = HttpContext.Current.Request.RequestContext.RouteData.UrlStoreName();
			}
			storeDb.StoreBindings.Add(storeBinding);
			storeDb.SaveChangesEx(true, false, false, false);

			return storeBinding;
		}

		public static PageTemplate CreateSeedPageTemplate(this IGstoreDb storeDb, string name, string layout, string viewName)
		{
			PageTemplate pageTemplate = storeDb.PageTemplates.Create();
			pageTemplate.Name = "Auto-generated Page Template";
			pageTemplate.Description = "Auto-generated Page Template";
			pageTemplate.LayoutName = layout;
			pageTemplate.ViewName = viewName;
			pageTemplate.IsPending = false;
			pageTemplate.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplate.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeDb.PageTemplates.Add(pageTemplate);
			storeDb.SaveChangesEx(true, false, false, false);

			return pageTemplate;
		}

		public static Page CreateAutoHomePage(this IGstoreDb db, HttpRequestBase request, StoreFront storeFront, Controllers.BaseClass.BaseController baseController)
		{

			UserProfile userProfile = db.SeedAutoMapUserBestGuess();
			db.CachedStoreFront = null;
			db.CachedUserProfile = userProfile;
			db.UserName = userProfile.UserName;

			PageTemplate pageTemplate = null;
			if (!db.PageTemplates.IsEmpty())
			{
				//look for match for storefront default new page layout name
				pageTemplate = db.PageTemplates.All().Where(pt => pt.LayoutName == storeFront.DefaultNewPageLayoutName).OrderBy(pt => pt.Order).ThenByDescending(pt => pt.PageTemplateId).FirstOrDefault();
				if (pageTemplate == null)
				{
					pageTemplate = db.PageTemplates.All().OrderBy(pt => pt.Order).ThenByDescending(pt => pt.PageTemplateId).FirstOrDefault();
				}
			}
			else
			{
				//no page templates in database, create a seed one
				pageTemplate = db.CreateSeedPageTemplate(storeFront.DefaultNewPageLayoutName + " Template", storeFront.DefaultNewPageLayoutName, Properties.Settings.Current.AppDefaultPageTemplateViewName);
			}

			Page page = db.CreateSeedPage(storeFront.Name, storeFront.Name, "/", 1000, storeFront, pageTemplate);

			string message = "--Auto-Created Home Page for StoreFront '" + storeFront.Name + "' [" + storeFront.StoreFrontId + "]"
				+ " For HostName: " + request.BindingHostName() + " Port: " + request.BindingPort() + " RootPath: " + request.BindingRootPath()
				+ " From RawUrl: " + request.RawUrl + " QueryString: " + request.QueryString + " ContentLength: " + request.ContentLength
				+ " HTTPMethod: " + request.HttpMethod + " Client IP: " + request.UserHostAddress;

			System.Diagnostics.Trace.WriteLine(message);

			EventLogExtensions.LogSystemEvent(db, baseController.HttpContext, baseController.RouteData, baseController.RouteData.ToSourceString(), SystemEventLevel.Information, message, string.Empty, string.Empty, string.Empty, baseController);

			return page;
		}

		public static Page CreateSeedPage(this IGstoreDb storeDb, string name, string pageTitle, string url, int order, StoreFront storeFront, PageTemplate pageTemplate)
		{
			Page page = storeDb.Pages.Create();
			page.ClientId = storeFront.ClientId;
			page.PageTemplateId = pageTemplate.PageTemplateId;
			page.Order = order;
			page.Name = name;
			page.PageTitle = pageTitle;
			page.ForRegisteredOnly = false;
			page.IsPending = false;
			page.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			page.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			page.StoreFrontId = storeFront.StoreFrontId;
			page.Url = url;
			storeDb.Pages.Add(page);
			storeDb.SaveChangesEx(true, false, false, false);

			return page;

		}

		public static NavBarItem CreateSeedNavBarItem(this IGstoreDb storeDb, string name, int pageId, bool forRegisteredOnly, NavBarItem parentNavBarItem, StoreFront storeFront)
		{
			NavBarItem newItem = storeDb.NavBarItems.Create();
			newItem.Client = storeFront.Client;
			newItem.StoreFront = storeFront;
			newItem.IsPage = true;
			newItem.PageId = pageId;
			newItem.Name = name;
			newItem.Order = 100;
			newItem.IsPending = false;
			newItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			newItem.ForRegisteredOnly = forRegisteredOnly;
			newItem.UseDividerAfterOnMenu = false;
			newItem.UseDividerBeforeOnMenu = true;

			if (parentNavBarItem != null)
			{
				newItem.ParentNavBarItem = parentNavBarItem;
			}
			storeDb.NavBarItems.Add(newItem);
			storeDb.SaveChangesEx(true, false, false, false);

			return newItem;
		}

		public static NavBarItem CreateSeedNavBarItemForAction(this IGstoreDb storeDb, string name, int order, string action, string controller, string area, bool forRegisteredOnly, NavBarItem parentNavBarItem, StoreFront storeFront)
		{
			NavBarItem newItem = storeDb.NavBarItems.Create();
			newItem.Client = storeFront.Client;
			newItem.StoreFront = storeFront;
			newItem.IsAction = true;
			newItem.Action = action;
			newItem.Controller = controller;
			newItem.Area = area;
			newItem.Name = name;
			newItem.Order = order;
			newItem.IsPending = false;
			newItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			newItem.ForRegisteredOnly = forRegisteredOnly;
			newItem.UseDividerAfterOnMenu = false;
			newItem.UseDividerBeforeOnMenu = true;

			if (parentNavBarItem != null)
			{
				newItem.ParentNavBarItem = parentNavBarItem;
			}
			storeDb.NavBarItems.Add(newItem);
			storeDb.SaveChangesEx(true, false, false, false);

			return newItem;
		}

		public static ProductCategory CreateSeedProductCategory(this IGstoreDb storeDb, string name, string urlName, int order, bool showIfEmpty, ProductCategory parentProductCategory, StoreFront storeFront)
		{
			ProductCategory category = storeDb.ProductCategories.Create();
			category.Client = storeFront.Client;
			category.StoreFront = storeFront;
			category.AllowChildCategoriesInMenu = true;
			category.Name = name;
			category.UrlName = urlName;
			category.Order = order;
			category.ImageName = urlName + ".png";
			category.ShowIfEmpty = showIfEmpty;
			category.ShowInMenu = true;
			category.IsPending = false;
			category.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			category.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			category.UseDividerAfterOnMenu = false;
			category.UseDividerBeforeOnMenu = true;

			if (parentProductCategory != null)
			{
				category.ParentCategory = parentProductCategory;
			}
			storeDb.ProductCategories.Add(category);
			storeDb.SaveChangesEx(true, false, false, false);

			return category;
		}

		public static Product CreateSeedProduct(this IGstoreDb storeDb, string name, string urlName, int order, ProductCategory category, StoreFront storeFront, bool updateCategoryCounts = true)
		{
			Product product = storeDb.Products.Create();
			product.Client = storeFront.Client;
			product.StoreFront = storeFront;
			product.Name = name;
			product.UrlName = urlName;
			product.Order = order;
			product.IsPending = false;
			product.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			product.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			product.Category = category;

			storeDb.Products.Add(product);
			storeDb.SaveChangesEx(true, false, false, updateCategoryCounts);

			return product;
		}
	}
}