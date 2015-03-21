using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStoreData.AppHtmlHelpers;
using GStoreData.ControllerBase;
using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.Models.BaseClasses;

namespace GStoreData
{
	public static class SeedDataExtensions
	{
		public static void AddSeedData(this IGstoreDb storeDb, bool force = false)
		{
			if (Settings.AppDoNotSeedDatabase && !force)
			{
				return;
			}

			string adminUserName = "admin@domain.com";
			string adminEmail = "admin@domain.com";
			string adminPhone = "888-555-1212";
			string adminPassword = "password";
			string adminFullName = "System Admin";
			string adminAddressLine1 = "1234 West Hollywood Blvd";
			string adminAddressLine2 = "Suite 104";
			string adminCity = "Venice Beach";
			string adminState = "CA";
			string adminPostalCode = "99999";
			CountryCodeEnum adminCountryCode = CountryCodeEnum.US;

			string browserUserName = "user@domain.com";
			string browserEmail = "user@domain.com";
			string browserPhone = "888-555-1212";
			string browserPassword = "password";
			string browserFullName = "John Doe User";
			string browserAddressLine1 = "1234 Main St";
			string browserAddressLine2 = null;
			string browserCity = "Los Angeles";
			string browserState = "CA";
			string browserPostalCode = "99999";
			CountryCodeEnum browserCountryCode = CountryCodeEnum.US;

			string pageTemplateViewName = Settings.AppDefaultPageTemplateViewName;
			string pageTemplateName = Settings.AppDefaultPageTemplateName;
			string preferedThemeName = Settings.AppDefaultThemeFolderName;
			bool loadSampleProducts = Settings.AppSeedSampleProducts;


			if (HttpContext.Current != null)
			{
				EventLogExtensions.CreateEventLogFolders(HttpContext.Current);
			}

			Identity.AspNetIdentityContext ctx = new Identity.AspNetIdentityContext();
			ctx.Database.Initialize(true);
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
				adminProfile = storeDb.CreateSeedAdminProfile(adminAspNetUser.Id, adminUserName, adminEmail, adminFullName, adminAddressLine1, adminAddressLine2, adminCity, adminState, adminPostalCode, adminCountryCode);
			}
			storeDb.UserName = adminProfile.UserName;
			storeDb.CachedUserProfile = adminProfile;

			if (storeDb.Clients.IsEmpty())
			{
				Client newClient = storeDb.CreateSeedClient("Sample Company", "SampleClient");
			}
			Client firstClient = storeDb.Clients.All().First();

			string virtualPathToThemes = "~/Content/Server/Themes";
			if (storeDb.Themes.IsEmpty())
			{
				storeDb.CreateSeedThemes(virtualPathToThemes, firstClient);
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

			if (storeDb.StoreFronts.IsEmpty())
			{
				StoreFront newStoreFront = storeDb.CreateSeedStoreFront(firstClient, adminProfile);
			}
			StoreFront firstStoreFront = storeDb.StoreFronts.All().First();
			storeDb.CachedStoreFront = firstStoreFront;

			//config
			StoreFrontConfiguration firstStoreFrontConfig = firstStoreFront.CurrentConfigOrAny();
			if (firstStoreFrontConfig == null)
			{
				string storeFrontName = "Sample Storefront";
				string storeFrontFolder = "SampleStoreFront";
				firstStoreFrontConfig = storeDb.CreateSeedStoreFrontConfig(firstStoreFront, storeFrontName, storeFrontFolder, adminProfile, selectedTheme);
			}

			firstClient = firstStoreFront.Client;

			if (storeDb.StoreBindings.IsEmpty())
			{
				StoreBinding storeBinding = storeDb.CreateSeedStoreBindingToCurrentUrl(firstStoreFront);
			}

			if (storeDb.PageTemplates.IsEmpty())
			{
				string virtualPathToPageTemplates = "~/Views/Page";
				PageTemplate newPageTemplate = storeDb.CreateSeedPageTemplate(pageTemplateName, pageTemplateViewName, firstClient);
				storeDb.CreateSeedPageTemplates(virtualPathToPageTemplates, firstClient);
			}

			PageTemplate firstPageTemplate = storeDb.PageTemplates.All().First();

			if (storeDb.Pages.IsEmpty())
			{
				Page homePage = storeDb.CreateSeedPage("New Home Page", string.Empty, "/", 100, firstStoreFrontConfig, firstPageTemplate);

				Page aboutUsPage = storeDb.CreateSeedPage("About Us", "About Us", "/About", 200, firstStoreFrontConfig, firstPageTemplate);
				NavBarItem aboutLink = storeDb.CreateSeedNavBarItem("About Us", aboutUsPage.PageId, false, null, firstStoreFront);

				Page contactUsPage = storeDb.CreateSeedPage("Contact Us", "Contact Us", "/Contact", 300, firstStoreFrontConfig, firstPageTemplate);
				NavBarItem contactUsLink = storeDb.CreateSeedNavBarItem("Contact Us", contactUsPage.PageId, false, null, firstStoreFront);

				Page answersPage = storeDb.CreateSeedPage("Answers", "Answers", "/Answers", 400, firstStoreFrontConfig, firstPageTemplate);
				NavBarItem answersLink = storeDb.CreateSeedNavBarItem("Questions?", answersPage.PageId, false, null, firstStoreFront);

				Page locationPage = storeDb.CreateSeedPage("Location", "Location", "/Location", 500, firstStoreFrontConfig, firstPageTemplate);
				NavBarItem locationLink = storeDb.CreateSeedNavBarItem("Location", locationPage.PageId, false, null, firstStoreFront);

				if (storeDb.WebForms.IsEmpty())
				{
					//add sample WebForms for Contact Us and Register
					WebForm registerForm = storeDb.CreateSeedWebForm("Sample Register Form", "Sample Register Form", firstClient);
					WebFormField customField = storeDb.CreateSeedWebFormField(registerForm, "How did you find us?", description:"How did you find us?", helpLabelBottomText: "Enter the name of the web site or person that referred you to us");

					WebForm contactForm = storeDb.CreateSeedWebForm("Sample Contact Form", "Sample Contact Form", firstClient);
					WebFormField contactName = storeDb.CreateSeedWebFormField(contactForm, "Your Name", 100, isRequired: true, helpLabelBottomText: "Please enter your Name");
					WebFormField contactEmail = storeDb.CreateSeedWebFormField(contactForm, "Your Email Address", 101, isRequired: true, helpLabelBottomText: "Please enter your Email Address", dataType: GStoreValueDataType.EmailAddress);
					WebFormField contactPhone = storeDb.CreateSeedWebFormField(contactForm, "Phone (optional)", 102, isRequired: false, helpLabelTopText: "If you would like us to reach you by phone, please enter your phone number below.");
					WebFormField contactMessage = storeDb.CreateSeedWebFormField(contactForm, "Message", 103, dataType: GStoreValueDataType.MultiLineText, isRequired: true, helpLabelTopText: "Enter a message below");

					contactUsPage.WebForm = contactForm;
					contactUsPage.WebFormId = contactForm.WebFormId;
					contactUsPage.WebFormSaveToDatabase = true;
					contactUsPage.WebFormSaveToFile = true;
					contactUsPage.WebFormSuccessPageId = homePage.PageId;
					contactUsPage = storeDb.Pages.Update(contactUsPage);

					firstStoreFrontConfig.Register_WebFormId = registerForm.WebFormId;
					firstStoreFrontConfig.RegisterWebForm = registerForm;
					firstStoreFrontConfig = storeDb.StoreFrontConfigurations.Update(firstStoreFrontConfig);
					storeDb.SaveChangesEx(true, false, false, false);

				}
			}

			//add sample discount codes
			if (storeDb.Discounts.IsEmpty())
			{
				Discount test1 = storeDb.CreateSeedDiscount("test1", 0, null, false, 0, 0, 10, firstStoreFront);
				Discount test2 = storeDb.CreateSeedDiscount("test2", 0, null, false, 0, 0, 10, firstStoreFront);
				Discount test3 = storeDb.CreateSeedDiscount("test3", 0, null, false, 0, 0, 10, firstStoreFront);

			}

			//add browser user
			UserProfile browserProfile = browserUser.GetUserProfile(storeDb, false);
			if (browserProfile == null)
			{
				browserProfile = storeDb.CreateSeedUserProfile(firstStoreFront, browserUser.Id, browserUserName, browserEmail, browserFullName, browserAddressLine1, browserAddressLine2, browserCity, browserState, browserPostalCode, browserCountryCode);
			}

			if (loadSampleProducts && storeDb.ProductCategories.IsEmpty())
			{
				ProductCategory topCatComputers = storeDb.CreateSeedProductCategory("Computers and Tablets", "Computers-And-Tablets", 100, true, null, firstStoreFront, true);

				ProductCategory topCatTvs = storeDb.CreateSeedProductCategory("TV's", "TVs", 110, true, null, firstStoreFront, true);
				ProductCategory topCatCameras = storeDb.CreateSeedProductCategory("Cameras", "Cameras", 110, true, null, firstStoreFront, true);
				ProductCategory notebooks = storeDb.CreateSeedProductCategory("Notebooks", "Notebooks", 200, true, topCatComputers, firstStoreFront, true);

				ProductCategory studentNotebooks = storeDb.CreateSeedProductCategory("Student Notebooks", "Student-Notebooks", 220, true, notebooks, firstStoreFront, true);

				ProductCategory gamingNotebooks = storeDb.CreateSeedProductCategory("Gaming Notebooks", "Gaming-Notebooks", 230, true, notebooks, firstStoreFront, true);
				ProductCategory desktops = storeDb.CreateSeedProductCategory("Desktops", "Desktops", 300, true, topCatComputers, firstStoreFront, true);
				ProductCategory gamingDesktops = storeDb.CreateSeedProductCategory("Gaming Desktops", "Gaming-Desktops", 350, true, desktops, firstStoreFront, true);
				ProductCategory officeDesktops = storeDb.CreateSeedProductCategory("Office Desktops", "Office-Desktops", 380, true, desktops, firstStoreFront, true);
				ProductCategory valueDesktops = storeDb.CreateSeedProductCategory("Value Desktops", "Value-Desktops", 390, true, desktops, firstStoreFront, true);
				ProductCategory budgetDesktops = storeDb.CreateSeedProductCategory("Budget Desktops", "Budget-Desktops", 390, true, valueDesktops, firstStoreFront, true);
				ProductCategory refurbishedDesktops = storeDb.CreateSeedProductCategory("Refurbished Desktops", "Refurbished-Desktops", 395, true, valueDesktops, firstStoreFront, true);
				ProductCategory offLeaseDesktops = storeDb.CreateSeedProductCategory("Off-Lease Desktops", "Off-Lease-Desktops", 398, true, refurbishedDesktops, firstStoreFront, true);
				ProductCategory manufacturerRefurbishedDesktops = storeDb.CreateSeedProductCategory("Manufacturer Refurbished", "Manufacturer-Refurbished-Desktops", 399, true, refurbishedDesktops, firstStoreFront, true);

				ProductCategory tablets = storeDb.CreateSeedProductCategory("Tablets", "Tablets", 400, true, topCatComputers, firstStoreFront, true);

				//Defer category count updating until the whole set of products is loaded instead of one by one to save cpu cycles
				Product productA1 = storeDb.CreateSeedProduct("Toshiba A205", "Toshiba-A205", 100, 9, gamingNotebooks, firstStoreFront, true, true, 229M, 249M, false);
				Product productA2 = storeDb.CreateSeedProduct("Lenovo B512A", "Levovo-B512A", 200, 9, gamingNotebooks, firstStoreFront, true, true, 215M, 245M, false);
				Product productA3 = storeDb.CreateSeedProduct("Acer A512A", "Acer-A512A", 300, 9, gamingNotebooks, firstStoreFront, true, true, 339M, 399M, false);

				Product productB1 = storeDb.CreateSeedProduct("Toshiba A205-S", "Toshiba-A205-S", 100, 9, studentNotebooks, firstStoreFront, true, true, 349M, 389M, false);
				Product productB2 = storeDb.CreateSeedProduct("Lenovo B512B", "Levovo-B512B", 200, 3, studentNotebooks, firstStoreFront, true, true, 305M, 330M, false);
				Product productB3 = storeDb.CreateSeedProduct("Acer A512B", "Acer-A512B", 300, 2, studentNotebooks, firstStoreFront, true, true, 399.95M, 449.99M, false);

				Product productC1 = storeDb.CreateSeedProduct("HP Pavillion B11212", "HP-Pavillion-B11212", 990, 3, offLeaseDesktops, firstStoreFront, true, true, 649.95M, 699.95M, false);
				Product productC2 = storeDb.CreateSeedProduct("HP Pavillion B666", "HP-Pavillion-B666", 300, 9, offLeaseDesktops, firstStoreFront, true, true, 549.95M, 589.95M, false);

				Product productTv1 = storeDb.CreateSeedProduct("RCA 56\" LCD TV", "RCA-56-LCD", 100, 1, topCatTvs, firstStoreFront, true, true, 699.95M, 749.95M, false);
				Product productTv2 = storeDb.CreateSeedProduct("Samsung 13\" DVD-Combo", "Samsung-13-DVD", 200, 99, topCatTvs, firstStoreFront, true, true, 129.95M, 199.95M, false);

				Product productCameraG6 = storeDb.CreateSeedProduct("Canon Powershot G6", "Canon-Powershot-G6", 100, 20, topCatCameras, firstStoreFront, true, true, 39.95M, 49.95M, false);
				Product productCameraG5 = storeDb.CreateSeedProduct("Canon Powershot G5", "Canon-Powershot-G5", 200, 20, topCatCameras, firstStoreFront, true, true, 29.95M, 37.95M, false);
				Product productCameraG4 = storeDb.CreateSeedProduct("Canon Powershot G4", "Canon-Powershot-G4", 300, 20, topCatCameras, firstStoreFront, true, true, 19.95M, 29.95M, false);

				Product productCameraBag = storeDb.CreateSeedProduct("Canon Powershot Camera Bag", "Canon-Powershot-Carry-Case", 400, 20, topCatCameras, firstStoreFront, true, true, 9.95M, 15.95M, false);
				Product productCameraMemory = storeDb.CreateSeedProduct("Canon Powershot Memory Card", "Canon-Powershot-Memory-Card", 500, 20, topCatCameras, firstStoreFront, true, true, 14.95M, 19.95M, false);

				ProductBundle g6StarterBundle = storeDb.CreateSeedProductBundle("Canon Powershot G6 Starter Kit", "Canon-Powershot-G6-Starter-Kit", 100, 20, true, topCatCameras, firstStoreFront, true, false);

				ProductBundleItem bundleItem1 = storeDb.CreateSeedProductBundleItem(g6StarterBundle, productCameraG6, 1, firstStoreFront, 1, 35.95M, 49.95M, false);
				ProductBundleItem bundleItem2 = storeDb.CreateSeedProductBundleItem(g6StarterBundle, productCameraBag, 2, firstStoreFront, 1, 4.95M, 15.95M, false);
				ProductBundleItem bundleItem3 = storeDb.CreateSeedProductBundleItem(g6StarterBundle, productCameraMemory, 3, firstStoreFront, 2, 5.95M, 19.95M, false);

				//run defered category calc
				storeDb.RecalculateProductCategoryActiveCount(firstStoreFront);
				
			}

		}


		public static UserProfile CreateSeedAdminProfile(this IGstoreDb storeDb, string adminUserId, string adminUserName, string adminEmail, string adminFullName, string addressLine1, string addressLine2, string city, string state, string postalCode, CountryCodeEnum countryCode)
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
			adminProfile.AddressLine1 = addressLine1;
			adminProfile.AddressLine2 = addressLine2;
			adminProfile.City = city;
			adminProfile.State = state;
			adminProfile.PostalCode = postalCode;
			adminProfile.CountryCode = countryCode;
			adminProfile.IsPending = false;
			adminProfile.Order = 1;
			adminProfile.EntryDateTime = DateTime.UtcNow;
			adminProfile.EntryRawUrl = "";
			adminProfile.EntryReferrer= "";
			adminProfile.EntryUrl = "";
			adminProfile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			adminProfile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			storeDb.UserProfiles.Add(adminProfile);
			storeDb.SaveChangesEx(true, false, false, false);

			return adminProfile;
		}

		public static UserProfile CreateSeedUserProfile(this IGstoreDb storeDb, StoreFront storeFront, string userId, string userName, string email, string fullName, string addressLine1, string addressLine2, string city, string state, string postalCode, CountryCodeEnum countryCode)
		{
			UserProfile profile = storeDb.UserProfiles.Create();
			profile.UserId = userId;
			profile.StoreFrontId = storeFront.StoreFrontId;
			profile.ClientId = storeFront.ClientId;
			profile.AllowUsersToSendSiteMessages = true;
			profile.Email = email;
			profile.EntryDateTime = DateTime.UtcNow;
			profile.EntryRawUrl = "";
			profile.EntryReferrer = "";
			profile.EntryUrl = "";
			profile.FullName = fullName;
			profile.NotifyAllWhenLoggedOn = true;
			profile.NotifyOfSiteUpdatesToEmail = true;
			profile.SendMoreInfoToEmail = false;
			profile.SendSiteMessagesToEmail = true;
			profile.SendSiteMessagesToSms = false;
			profile.SubscribeToNewsletterEmail = true;
			profile.AddressLine1 = addressLine1;
			profile.AddressLine2 = addressLine2;
			profile.City = city;
			profile.State = state;
			profile.PostalCode = postalCode;
			profile.CountryCode = countryCode;
			profile.UserName = userName;
			profile.IsPending = false;
			profile.Order = 100;
			profile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			profile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			storeDb.UserProfiles.Add(profile);
			storeDb.SaveChangesEx(true, false, false, false);

			return profile;
		}

		public static void CreateSeedPageTemplates(this IGstoreDb storeDb, string virtualPath, Client client)
		{
			if (storeDb == null)
			{
				throw new ArgumentNullException("db");
			}
			if (string.IsNullOrEmpty(virtualPath))
			{
				throw new ArgumentNullException("virtualPath");
			}
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}

			string path = string.Empty;
			if (HttpContext.Current == null)
			{
				string assemblyPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
				string directoryName = System.IO.Path.GetDirectoryName(assemblyPath).Replace("GStore\\GStoreData\\", "GStore\\GStoreWeb\\");
				path = System.IO.Path.Combine(directoryName, "..\\.." + virtualPath.TrimStart('~').Replace('/', '\\')).Replace("%20", " ");
				if (!System.IO.Directory.Exists(path))
				{
					throw new ApplicationException("Page Templates folder could not be found in file system at path: " + path + ". Please run the web site first to populate the database.");
				}
			}
			else
			{
				path = HttpContext.Current.Server.MapPath(virtualPath);
			}

			if (!System.IO.Directory.Exists(path))
			{
				throw new ApplicationException("Page Templates folder could not be found in file system web server at path: " + path + ".");
			}

			System.IO.DirectoryInfo folder = new System.IO.DirectoryInfo(path);
			IEnumerable<System.IO.FileInfo> files = folder.EnumerateFiles("Page *.cshtml");
			int counter = 0;
			foreach (System.IO.FileInfo file in files)
			{
				string name = file.Name.Substring(5, ((file.Name.Length - 5) - 7)) + " Template";
				if (!client.PageTemplates.Any(pt => pt.Name.ToLower() == name.ToLower()))
				{
					counter++;
					PageTemplate pageTemplate = storeDb.PageTemplates.Create();
					pageTemplate.Name = name;
					pageTemplate.ViewName = file.Name.Replace(".cshtml", "");
					pageTemplate.Order = 2000 + counter;
					pageTemplate.Description = pageTemplate.Name;
					pageTemplate.IsPending = false;
					pageTemplate.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					pageTemplate.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					pageTemplate.Client = client;
					pageTemplate.ClientId = client.ClientId;

					storeDb.PageTemplates.Add(pageTemplate);
				}

			}
			storeDb.SaveChangesEx(true, false, false, false);
		}

		public static void CreateSeedThemes(this IGstoreDb storeDb, string virtualPath, Client client)
		{
			string path = string.Empty;
			if (HttpContext.Current == null)
			{
				string assemblyPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
				string directoryName = System.IO.Path.GetDirectoryName(assemblyPath).Replace("GStore\\GStoreData\\", "GStore\\GStoreWeb\\");
				path = System.IO.Path.Combine(directoryName, "..\\.." + virtualPath.TrimStart('~').Replace('/', '\\')).Replace("%20", " ");
				if (!System.IO.Directory.Exists(path))
				{
					throw new ApplicationException("Themes folder could not be found in file system at path: " + path + ". Please run the web site first to populate the database.");
				}
			}
			else
			{
				path = HttpContext.Current.Server.MapPath(virtualPath);
			}

			if (!System.IO.Directory.Exists(path))
			{
				throw new ApplicationException("Themes folder could not be found in file system web server at path: " + path + ".");
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
				theme.Client = client;
				theme.ClientId = client.ClientId;

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
			client.TimeZoneId = Settings.AppDefaultTimeZoneId;
			storeDb.Clients.Add(client);
			storeDb.SaveChangesEx(true, false, false, false);

			return client;
		}

		public static StoreFront CreateSeedStoreFront(this IGstoreDb storeDb, Client client, UserProfile adminProfile)
		{
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}

			StoreFront storeFront = storeDb.StoreFronts.Create();
			storeFront.SetDefaultsForNew(client);
			storeDb.StoreFronts.Add(storeFront);
			storeDb.SaveChangesEx(true, false, false, false);

			return storeFront;

		}

		public static StoreFrontConfiguration CreateSeedStoreFrontConfig(this IGstoreDb storeDb, StoreFront storeFront, string storeFrontName, string storeFrontFolder, UserProfile adminProfile, Theme selectedTheme)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			StoreFrontConfiguration storeFrontConfig = storeDb.StoreFrontConfigurations.Create();

			storeFrontConfig.StoreFront = storeFront;
			storeFrontConfig.StoreFrontId = storeFront.StoreFrontId;
			storeFrontConfig.Name = storeFrontName;
			storeFrontConfig.ConfigurationName = "Default";
			storeFrontConfig.Folder = storeFrontFolder;
			storeFrontConfig.IsPending = false;
			storeFrontConfig.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			storeFrontConfig.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeFrontConfig.Client = storeFront.Client;
			storeFrontConfig.ClientId = storeFront.ClientId;
			storeFrontConfig.Order = storeFront.StoreFrontConfigurations.Count == 0 ? 100 : storeFront.StoreFrontConfigurations.Max(sf => sf.Order) + 10;
			storeFrontConfig.TimeZoneId = storeFront.Client.TimeZoneId;

			storeFrontConfig.AccountAdmin = adminProfile;
			storeFrontConfig.RegisteredNotify = adminProfile;
			storeFrontConfig.WelcomePerson = adminProfile;
			storeFrontConfig.OrderAdmin = adminProfile;

			storeFrontConfig.MetaApplicationName = storeFrontName;
			storeFrontConfig.MetaApplicationTileColor = "#880088";
			storeFrontConfig.MetaDescription = "New GStore Storefront " + storeFrontName;
			storeFrontConfig.MetaKeywords = "GStore Storefront " + storeFrontName;
			storeFrontConfig.AdminTheme = selectedTheme;
			storeFrontConfig.AccountTheme = selectedTheme;
			storeFrontConfig.ProfileTheme = selectedTheme;
			storeFrontConfig.NotificationsTheme = selectedTheme;
			storeFrontConfig.CatalogTheme = selectedTheme;
			storeFrontConfig.CatalogAdminTheme = selectedTheme;
			storeFrontConfig.DefaultNewPageTheme = selectedTheme;
			storeFrontConfig.CatalogPageInitialLevels = 6;
			storeFrontConfig.CatalogTitle = storeFrontConfig.Name + " Catalog";
			storeFrontConfig.CatalogLayout = CatalogLayoutEnum.SimpleBlocked;
			storeFrontConfig.CatalogHeaderHtml = null;
			storeFrontConfig.CatalogFooterHtml = null;
			storeFrontConfig.CatalogRootListTemplate = CategoryListTemplateEnum.Default ;
			storeFrontConfig.CatalogRootHeaderHtml = null;
			storeFrontConfig.CatalogRootFooterHtml = null;

			storeFrontConfig.NavBarCatalogMaxLevels = 6;
			storeFrontConfig.NavBarItemsMaxLevels = 6;
			storeFrontConfig.CatalogCategoryColLg = 3;
			storeFrontConfig.CatalogCategoryColMd = 4;
			storeFrontConfig.CatalogCategoryColSm = 6;
			storeFrontConfig.CatalogProductColLg = 2;
			storeFrontConfig.CatalogProductColMd = 3;
			storeFrontConfig.CatalogProductColSm = 6;
			storeFrontConfig.CatalogProductBundleColLg = 3;
			storeFrontConfig.CatalogProductBundleColMd = 4;
			storeFrontConfig.CatalogProductBundleColSm = 6;
			storeFrontConfig.CatalogProductBundleItemColLg = 3;
			storeFrontConfig.CatalogProductBundleItemColMd = 4;
			storeFrontConfig.CatalogProductBundleItemColSm = 6;

			storeFrontConfig.HtmlFooter = storeFrontName;
			storeFrontConfig.HomePageUseCatalog = true;

			storeFrontConfig.NavBarShowRegisterLink = true;
			storeFrontConfig.NavBarRegisterLinkText = "Sign-Up";
			storeFrontConfig.AccountLoginShowRegisterLink = true;
			storeFrontConfig.AccountLoginRegisterLinkText = "Sign-up";
			
			if (HttpContext.Current == null)
			{
				storeFrontConfig.PublicUrl = "http://localhost:55520/";
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
				storeFrontConfig.PublicUrl = publicUrl;
			}

			storeFrontConfig.ApplyDefaultCartConfig();
			storeFrontConfig.ApplyDefaultCheckoutConfig();
			storeFrontConfig.ApplyDefaultOrdersConfig();
			storeFrontConfig.ApplyDefaultPaymentMethodConfig();

			storeFrontConfig.CheckoutTheme = selectedTheme;
			storeFrontConfig.OrdersTheme = selectedTheme;
			storeFrontConfig.CatalogTheme = selectedTheme;
			storeFrontConfig.OrderAdminTheme = selectedTheme;

			storeDb.StoreFrontConfigurations.Add(storeFrontConfig);
			storeDb.SaveChangesEx(true, false, false, false);

			return storeFrontConfig;

		}

		public static StoreBinding CreatAutoMapStoreBindingToCurrentUrl(this IGstoreDb storeDb, GStoreData.ControllerBase.BaseController baseController)
		{
			if (HttpContext.Current == null)
			{
				throw new ApplicationException("Cannot create auto-map binding when HttpContext.Current is null");
			}
			HttpRequestBase request = baseController.Request;

			UserProfile profile = storeDb.SeedAutoMapUserBestGuess();
			StoreFront storeFront = storeDb.SeedAutoMapStoreFrontBestGuess();

			IGstoreDb systemDb = storeDb.NewContext(profile.UserName, storeFront, storeFront.CurrentConfigOrAny(), profile);
			StoreBinding binding = systemDb.CreateSeedStoreBindingToCurrentUrl(storeFront);

			string message = "--Bindings auto-mapped to StoreFront '" + binding.StoreFront.CurrentConfigOrAny().Name + "' [" + binding.StoreFront.StoreFrontId + "]"
				+ " For HostName: " + binding.HostName + " Port: " + binding.Port + " RootPath: " + binding.RootPath
				+ " UseUrlStoreName: " + binding.UseUrlStoreName.ToString() + " UrlStoreName: " + binding.UrlStoreName.ToString()
				+ " From RawUrl: " + request.RawUrl + " QueryString: " + request.QueryString + " ContentLength: " + request.ContentLength
				+ " HTTPMethod: " + request.HttpMethod + " Client IP: " + request.UserHostAddress;

			System.Diagnostics.Trace.WriteLine(message);
			EventLogExtensions.LogSystemEvent(systemDb, baseController.HttpContext, baseController.RouteData, baseController.RouteData.ToSourceString(), SystemEventLevel.Information, message, string.Empty, string.Empty, string.Empty, baseController);

			return binding;

		}

		public static StoreBinding CreatAutoMapStoreBindingToCatchAll(this IGstoreDb storeDb, BaseController baseController)
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

			string message = "--Bindings Catch-All auto-mapped to StoreFront '" + binding.StoreFront.CurrentConfigOrAny().Name + "' [" + binding.StoreFront.StoreFrontId + "]"
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

		public static PageTemplate CreateSeedPageTemplate(this IGstoreDb storeDb, string name, string viewName, Client client)
		{
			PageTemplate pageTemplate = storeDb.PageTemplates.Create();
			pageTemplate.Name = name;
			pageTemplate.Description = name;
			pageTemplate.ViewName = viewName;
			pageTemplate.IsPending = false;
			pageTemplate.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplate.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			pageTemplate.Client = client;
			pageTemplate.ClientId = client.ClientId;
			storeDb.PageTemplates.Add(pageTemplate);
			storeDb.SaveChangesEx(true, false, false, false);

			return pageTemplate;
		}

		public static WebForm CreateSeedWebForm(this IGstoreDb storeDb, string name, string description, Client client)
		{
			WebForm webForm = storeDb.WebForms.Create();
			webForm.SetDefaultsForNew(client);
			webForm.Client = client;
			webForm.Name = name;
			webForm.Description = description;
			storeDb.WebForms.Add(webForm);
			storeDb.SaveChangesEx(true, false, false, false);

			return webForm;
		}

		public static WebFormField CreateSeedWebFormField(this IGstoreDb storeDb, WebForm webForm, string name, int order = 1000, string description = null, bool isRequired = false, string helpLabelBottomText = "", string helpLabelTopText = "", GStoreValueDataType? dataType = null)
		{
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}

			WebFormField webFormField = storeDb.WebFormFields.Create();
			webFormField.SetDefaultsForNew(webForm);
			webFormField.Client = webForm.Client;

			webFormField.Name = name;
			webFormField.Description = description ?? name;
			webFormField.IsRequired = isRequired;
			webFormField.LabelText = name;
			webFormField.Order = order;
			webFormField.Watermark = webFormField.LabelText + (webFormField.IsRequired ? " (Required)" : "");
			webFormField.HelpLabelBottomText = helpLabelBottomText;
			webFormField.HelpLabelTopText = helpLabelTopText;
			if (dataType.HasValue)
			{
				webFormField.DataType = dataType.Value;
				webFormField.DataTypeString = dataType.ToDisplayName();
			}

			storeDb.WebFormFields.Add(webFormField);
			storeDb.SaveChangesEx(true, false, false, false);

			return webFormField;
		}

		public static Page CreateAutoHomePage(this IGstoreDb db, HttpRequestBase request, StoreFrontConfiguration storeFrontConfig, GStoreData.ControllerBase.BaseController baseController)
		{
			if (db == null)
			{
				throw new ArgumentNullException("db");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (baseController == null)
			{
				throw new ArgumentNullException("baseController");
			}
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}

			UserProfile userProfile = db.SeedAutoMapUserBestGuess();
			db.CachedStoreFront = null;
			db.CachedUserProfile = userProfile;
			db.UserName = userProfile.UserName;

			PageTemplate pageTemplate = null;
			if (!db.PageTemplates.IsEmpty())
			{
				pageTemplate = db.PageTemplates.Where(pt => pt.ClientId == storeFrontConfig.ClientId).ApplyDefaultSort().FirstOrDefault();
			}
			else
			{
				//no page templates in database, create seed one
				pageTemplate = db.CreateSeedPageTemplate(Settings.AppDefaultPageTemplateName, Settings.AppDefaultPageTemplateViewName, storeFrontConfig.Client);
			}

			Page page = db.CreateSeedPage(storeFrontConfig.Name, storeFrontConfig.Name, "/", 1000, storeFrontConfig, pageTemplate);

			string message = "--Auto-Created Home Page for StoreFront '" + storeFrontConfig.Name + "' [" + storeFrontConfig.StoreFrontId + "]"
				+ " For HostName: " + request.BindingHostName() + " Port: " + request.BindingPort() + " RootPath: " + request.BindingRootPath()
				+ " From RawUrl: " + request.RawUrl + " QueryString: " + request.QueryString + " ContentLength: " + request.ContentLength
				+ " HTTPMethod: " + request.HttpMethod + " Client IP: " + request.UserHostAddress;

			System.Diagnostics.Trace.WriteLine(message);

			EventLogExtensions.LogSystemEvent(db, baseController.HttpContext, baseController.RouteData, baseController.RouteData.ToSourceString(), SystemEventLevel.Information, message, string.Empty, string.Empty, string.Empty, baseController);

			return page;
		}

		public static Page CreateSeedPage(this IGstoreDb storeDb, string name, string pageTitle, string url, int order, StoreFrontConfiguration storeFrontConfig, PageTemplate pageTemplate)
		{
			Page page = storeDb.Pages.Create();
			page.ClientId = storeFrontConfig.ClientId;
			page.PageTemplateId = pageTemplate.PageTemplateId;
			page.Theme = storeFrontConfig.DefaultNewPageTheme;
			page.ThemeId = storeFrontConfig.DefaultNewPageThemeId;
			page.Order = order;
			page.Name = name;
			page.PageTitle = pageTitle;
			page.ForAnonymousOnly = false;
			page.ForRegisteredOnly = false;
			page.IsPending = false;
			page.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			page.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			page.StoreFrontId = storeFrontConfig.StoreFrontId;
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

		public static ProductCategory CreateSeedProductCategory(this IGstoreDb storeDb, string name, string urlName, int order, bool hideInMenuIfEmpty, ProductCategory parentProductCategory, StoreFront storeFront, bool linkRandomImage)
		{
			ProductCategory category = storeDb.ProductCategories.Create();
			category.Client = storeFront.Client;
			category.StoreFront = storeFront;
			category.AllowChildCategoriesInMenu = true;
			category.Name = name;
			category.UrlName = urlName;
			category.Order = order;
			category.ImageName = "";
			category.ShowInMenu = true;
			category.HideInMenuIfEmpty = hideInMenuIfEmpty;
			category.DisplayForDirectLinks = true;
			category.ShowInCatalogIfEmpty = false;
			category.IsPending = false;
			category.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			category.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			category.UseDividerAfterOnMenu = false;
			category.UseDividerBeforeOnMenu = true;

			if (parentProductCategory != null)
			{
				category.ParentCategory = parentProductCategory;
			}
			if (linkRandomImage)
			{
				category.ImageName = category.RandomImageName();
			}

			storeDb.ProductCategories.Add(category);
			storeDb.SaveChangesEx(true, false, false, false);

			return category;
		}



		public static Product CreateSeedProduct(this IGstoreDb storeDb, string name, string urlName, int order, int maxQuantityPerOrder, ProductCategory category, StoreFront storeFront, bool availableForPurchase, bool linkRandomImage, decimal baseUnitPrice, decimal baseListPrice, bool updateCategoryCounts = true)
		{
			Product product = storeDb.Products.Create();
			product.Client = storeFront.Client;
			product.StoreFront = storeFront;
			product.Name = name;
			if (!urlName.IsValidUrlName())
			{
				urlName = urlName.FixUrlName();
			}
			product.UrlName = urlName;
			product.Order = order;
			product.MaxQuantityPerOrder = maxQuantityPerOrder;
			product.IsPending = false;
			product.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			product.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			product.Category = category;
			product.BaseListPrice = baseListPrice;
			product.BaseUnitPrice = baseUnitPrice;
			product.SummaryCaption= "Summary for " + product.Name;
			product.SummaryHtml = "Sample product summary for " + product.Name;
			product.TopDescriptionCaption= "Description for " + product.Name;
			product.TopDescriptionHtml = "Sample product description for " + product.Name;
			product.TopLinkLabel = null;
			product.TopLinkHref = null;
			product.BottomDescriptionCaption = "Details for " + product.Name;
			product.BottomDescriptionHtml = "Sample product details for " + product.Name;
			product.BottomLinkLabel = null;
			product.BottomLinkHref = null;
			product.ProductDetailTemplate = ProductDetailTemplateEnum.Default;
			product.AvailableForPurchase = availableForPurchase;
			if (linkRandomImage)
			{
				product.ImageName = product.RandomImageName();
			}

			storeDb.Products.Add(product);
			storeDb.SaveChangesEx(true, false, false, updateCategoryCounts);

			return product;
		}

		public static ProductBundle CreateSeedProductBundle(this IGstoreDb storeDb, string name, string urlName, int order, int maxQuantityPerOrder, bool linkRandomImage, ProductCategory category, StoreFront storeFront, bool availableForPurchase, bool updateCategoryCounts = true)
		{
			ProductBundle productBundle = storeDb.ProductBundles.Create();
			productBundle.Client = storeFront.Client;
			productBundle.StoreFront = storeFront;
			productBundle.Name = name;
			if (!urlName.IsValidUrlName())
			{
				urlName = urlName.FixUrlName();
			}
			productBundle.UrlName = urlName;
			productBundle.Order = order;
			productBundle.MaxQuantityPerOrder = maxQuantityPerOrder;
			productBundle.IsPending = false;
			productBundle.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			productBundle.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			productBundle.Category = category;
			productBundle.SummaryCaption = "Summary for " + productBundle.Name;
			productBundle.SummaryHtml = "Sample product summary for " + productBundle.Name;
			productBundle.TopDescriptionCaption = "Description for " + productBundle.Name;
			productBundle.TopDescriptionHtml = "Sample product description for " + productBundle.Name;
			productBundle.TopLinkLabel = null;
			productBundle.TopLinkHref = null;
			productBundle.BottomDescriptionCaption = "Details for " + productBundle.Name;
			productBundle.BottomDescriptionHtml = "Sample product details for " + productBundle.Name;
			productBundle.BottomLinkLabel = null;
			productBundle.BottomLinkHref = null;
			productBundle.ProductBundleDetailTemplate = ProductBundleDetailTemplateEnum.Default;
			productBundle.AvailableForPurchase = availableForPurchase;
			productBundle.ForAnonymousOnly = false;
			productBundle.ForRegisteredOnly = false;

			if (linkRandomImage)
			{
				productBundle.ImageName = productBundle.RandomImageName();
			}

			storeDb.ProductBundles.Add(productBundle);
			storeDb.SaveChangesEx(true, false, false, updateCategoryCounts);

			return productBundle;
		}

		public static ProductBundleItem CreateSeedProductBundleItem(this IGstoreDb storeDb, ProductBundle productBundle, Product product, int order, StoreFront storeFront, int quantity, decimal baseUnitPrice, decimal baseListPrice, bool updateCategoryCounts = true)
		{
			ProductBundleItem productBundleItem = storeDb.ProductBundleItems.Create();
			productBundleItem.Client = storeFront.Client;
			productBundleItem.StoreFront = storeFront;

			productBundleItem.ProductBundleId = productBundle.ProductBundleId;
			productBundleItem.ProductId = product.ProductId;

			productBundleItem.BaseListPrice = baseListPrice;
			productBundleItem.BaseUnitPrice = baseUnitPrice;
			productBundleItem.Quantity = quantity;
			productBundleItem.Order = order;
			productBundleItem.ProductBundleId = productBundle.ProductBundleId;

			productBundleItem.IsPending = false;
			productBundleItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			productBundleItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			
			storeDb.ProductBundleItems.Add(productBundleItem);
			storeDb.SaveChangesEx(true, false, false, updateCategoryCounts);

			return productBundleItem;
		}



		public static Discount CreateSeedDiscount(this IGstoreDb storeDb, string code, decimal flatDiscount, Product freeProductOrNull, bool freeShipping, int maxUsesZeroForNoLimit, decimal minSubTotal, decimal percentOff, StoreFront storeFront)
		{
			Discount record = storeDb.Discounts.Create();
			record.Client = storeFront.Client;
			record.StoreFront = storeFront;
			record.Code = code;
			record.FlatDiscount = flatDiscount;
			record.FreeProduct = freeProductOrNull;
			record.FreeShipping = freeShipping;
			record.MaxUses = maxUsesZeroForNoLimit;
			record.MinSubtotal = minSubTotal;
			record.Order = (storeDb.Discounts.IsEmpty() ? 100 : storeDb.Discounts.All().Max(d => d.Order) + 10);
			record.PercentOff = percentOff;
			record.UseCount = 0;
			record.IsPending = false;
			record.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			record.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			storeDb.Discounts.Add(record);
			storeDb.SaveChangesEx(true, false, false, false);

			return record;
		}
	}
}