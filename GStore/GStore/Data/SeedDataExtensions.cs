using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Models;
using GStore.Models.Extensions;
using GStore.Identity;

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

			string browserUserName = "name@domain.com";
			string browserEmail = "name@domain.com";
			string browserPhone = "888-555-1212";
			string browserPassword = "password";
			string browserFullName = "John Doe User";

			string layout = "Bootstrap";
			string templateViewName = "NewStore";
			string preferedThemeName = "StarterTheme";

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
				StoreBinding storeBinding = storeDb.CreateSeedStoreBinding(firstStoreFront);
			}

			if (storeDb.PageTemplates.IsEmpty())
			{
				PageTemplate newPageTemplate = storeDb.CreateSeedPageTemplate(layout, templateViewName);
			}

			PageTemplate firstPageTemplate = storeDb.PageTemplates.All().First();

			if (storeDb.Pages.IsEmpty())
			{
				Page page1 = storeDb.CreateSeedPage("New Home Page", string.Empty, "/", 100, firstStoreFront, firstPageTemplate);

				Page page2 = storeDb.CreateSeedPage("About Us", "About Us", "/About", 200, firstStoreFront, firstPageTemplate);

				Page page3 = storeDb.CreateSeedPage("Contact Us", "Contact Us", "/Contact", 300, firstStoreFront, firstPageTemplate);

				Page page4 = storeDb.CreateSeedPage("Questions?", "Questions?", "/Answers", 400, firstStoreFront, firstPageTemplate);
			}

			//add browser user
			UserProfile browserProfile = browserUser.GetUserProfile(storeDb, false);
			if (browserProfile == null)
			{
				browserProfile = storeDb.CreateSeedUserProfile(browserUser.Id, browserUserName, browserEmail, browserFullName, firstStoreFront);
			}

			//add menu items
			if (storeDb.NavBarItems.IsEmpty())
			{
				NavBarItem aboutLink = storeDb.CreateSeedNavBarItem("About Us", "/About", false, null, firstStoreFront);
				NavBarItem contactLink = storeDb.CreateSeedNavBarItem("Contact Us", "/Contact", false, null, firstStoreFront);
				NavBarItem answersLink = storeDb.CreateSeedNavBarItem("Questions?", "/Answers", false, null, firstStoreFront);
			}

			//add product categories
			if (storeDb.ProductCategories.IsEmpty())
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
			adminProfile.AllowUsersToSendSiteMessages = false;
			adminProfile.Email = adminEmail;
			adminProfile.FullName = adminFullName;
			adminProfile.NotifyAllWhenLoggedOn = false;
			adminProfile.NotifyOfSiteUpdatesToEmail = true;
			adminProfile.SendMoreInfoToEmail = false;
			adminProfile.SendSiteMessagesToEmail = true;
			adminProfile.SendSiteMessagesToSms = false;
			adminProfile.SubscribeToNewsletterEmail = true;
			adminProfile.UserName = adminUserName;
			adminProfile.Active = true;
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
			profile.StoreFront = storeFront;
			profile.AllowUsersToSendSiteMessages = false;
			profile.Email = email;
			profile.FullName = fullName;
			profile.NotifyAllWhenLoggedOn = false;
			profile.NotifyOfSiteUpdatesToEmail = true;
			profile.SendMoreInfoToEmail = false;
			profile.SendSiteMessagesToEmail = true;
			profile.SendSiteMessagesToSms = false;
			profile.SubscribeToNewsletterEmail = true;
			profile.UserName = userName;
			profile.Active = true;
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
				storeDb.Themes.Add(theme);
			}
			storeDb.SaveChangesEx(true, false, false, false);
		}

		public static Client CreateSeedClient(this IGstoreDb storeDb, string clientName, string clientFolder)
		{
			Client client = storeDb.Clients.Create();
			client.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			client.Name = clientName;
			client.Folder = clientFolder;
			client.UseSendGridEmail = false;
			client.UseTwilioSms = false;
			storeDb.Clients.Add(client);
			storeDb.SaveChangesEx(true, false, false, false);

			return client;
		}

		public static StoreFront CreateSeedStoreFront(this IGstoreDb storeDb, string storeFrontName, string storeFrontFolder, Client client, UserProfile adminProfile, Theme selectedTheme, string layout)
		{
			StoreFront storeFront = storeDb.StoreFronts.Create();

			storeFront.Name = storeFrontName;
			storeFront.Folder = storeFrontFolder;
			storeFront.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			storeFront.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeFront.Client = client;
			storeFront.AccountAdmin = adminProfile;
			storeFront.RegisteredNotify = adminProfile;
			storeFront.WelcomePerson = adminProfile;
			storeFront.Theme = selectedTheme;
			storeFront.MetaApplicationName = storeFrontName;
			storeFront.MetaApplicationTileColor = "#880088";
			storeFront.MetaDescription = "New GStore Storefront " + storeFrontName;
			storeFront.MetaKeywords = "GStore Storefront " + storeFrontName;
			storeFront.AdminLayout = layout;
			storeFront.AccountLayout = layout;
			storeFront.ManageLayout = layout;
			storeFront.NotificationsLayout = layout;
			storeFront.CatalogLayout = layout;
			storeFront.CatalogPageInitialLevels = 6;
			storeFront.NavBarCatalogMaxLevels = 6;
			storeFront.NavBarItemsMaxLevels = 6;
			storeFront.CatalogCategoryColLg = 3;
			storeFront.CatalogCategoryColMd = 4;
			storeFront.CatalogCategoryColSm = 6;
			storeFront.CatalogProductColLg = 2;
			storeFront.CatalogProductColMd = 3;
			storeFront.CatalogProductColSm = 6;

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

		public static StoreBinding CreateSeedStoreBinding(this IGstoreDb storeDb, StoreFront storeFront)
		{
			StoreBinding storeBinding = storeDb.StoreBindings.Create();
			storeBinding.Client = storeFront.Client;
			storeBinding.StoreFrontId = storeFront.StoreFrontId;
			storeBinding.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			storeBinding.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeBinding.RootPath = "/";
			if (HttpContext.Current == null)
			{
				storeBinding.HostName = "localhost";
				storeBinding.Port = 55520;
				storeBinding.RootPath = "/";
			}
			else
			{
				Uri url = HttpContext.Current.Request.Url;
				storeBinding.HostName = url.Host;
				storeBinding.Port = (url.IsDefaultPort ? 80 : (int?)url.Port);
				storeBinding.RootPath = HttpContext.Current.Request.ApplicationPath;
			}
			storeDb.StoreBindings.Add(storeBinding);
			storeDb.SaveChangesEx(true, false, false, false);

			return storeBinding;
		}

		public static PageTemplate CreateSeedPageTemplate(this IGstoreDb storeDb, string layout, string viewName)
		{
			PageTemplate pageTemplate = storeDb.PageTemplates.Create();
			pageTemplate.Name = "Auto-generated Page Template";
			pageTemplate.Description = "Auto-generated Page Template";
			pageTemplate.LayoutName = layout;
			pageTemplate.ViewName = viewName;
			storeDb.PageTemplates.Add(pageTemplate);
			storeDb.SaveChangesEx(true, false, false, false);

			return pageTemplate;
		}

		public static Page CreateSeedPage(this IGstoreDb storeDb, string name, string pageTitle, string url, int order, StoreFront storeFront, PageTemplate pageTemplate)
		{
			Page page = storeDb.Pages.Create();
			page.Client = storeFront.Client;
			page.PageTemplate = pageTemplate;
			page.Order = 100;
			page.Name = name;
			page.PageTitle = pageTitle;
			page.Public = true;
			page.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
			page.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			page.StoreFront = storeFront;
			page.Url = url;
			storeDb.Pages.Add(page);
			storeDb.SaveChangesEx(true, false, false, false);

			return page;

		}

		public static NavBarItem CreateSeedNavBarItem(this IGstoreDb storeDb, string name, string localHRef, bool forRegisteredOnly, NavBarItem parentNavBarItem, StoreFront storeFront)
		{
			NavBarItem newItem = storeDb.NavBarItems.Create();
			newItem.Client = storeFront.Client;
			newItem.StoreFront = storeFront;
			newItem.IsLocalHRef = true;
			newItem.LocalHRef = localHRef;
			newItem.Name = name;
			newItem.Order = 100;
			newItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			newItem.ForRegisteredOnly = forRegisteredOnly;
			newItem.UseDividerAfterOnMenu = true;
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
			category.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			category.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			category.UseDividerAfterOnMenu = true;
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
			product.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			product.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			product.Category = category;

			storeDb.Products.Add(product);
			storeDb.SaveChangesEx(true, false, false, updateCategoryCounts);

			return product;
		}

	}
}