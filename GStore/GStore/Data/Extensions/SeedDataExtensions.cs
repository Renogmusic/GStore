using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Models;
using GStore.Models.Extensions;
using GStore.Identity;

namespace GStore.Data.Extensions
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
				adminProfile = storeDb.UserProfiles.Create();
				adminProfile.UserId = adminAspNetUser.Id;
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
				storeDb.SaveChanges();
			}
			storeDb.CachedUserProfile = adminProfile;

			if (storeDb.Themes.IsEmpty())
			{
				string virtualPath = "~/Content/Themes";
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
				storeDb.SaveChanges();
			}
			if (storeDb.Themes.IsEmpty())
			{
				throw new ApplicationException("No themes found; add themes to /Content/themes/(ThemeName) folders");
			}
			Theme selectedTheme = storeDb.Themes.Where(t => t.Name == preferedThemeName.ToLower()).FirstOrDefault();
			if (selectedTheme == null)
			{
				int themeCount = storeDb.Themes.All().Count();
				Random rndNumber = new Random();
				int randomThemeIndex = rndNumber.Next(1, themeCount);
				selectedTheme = storeDb.Themes.All().OrderBy(t => t.Order).Skip(randomThemeIndex - 1).First();
			}


			if (storeDb.Clients.IsEmpty())
			{
				Client client = storeDb.Clients.Create();
				client.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
				client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				client.Name = "New Company";
				client.Folder = "StarterClient";
				client.UseSendGridEmail = false;
				client.UseTwilioSms = false;
				storeDb.Clients.Add(client);
				storeDb.SaveChanges();
			}
			Client firstClient = storeDb.Clients.All().First();

			if (storeDb.StoreFronts.IsEmpty())
			{
				StoreFront storeFront = storeDb.StoreFronts.Create();
				storeFront.Name = "New Storefront";
				storeFront.Folder = "StarterStoreFront";
				storeFront.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
				storeFront.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				storeFront.Client = firstClient;
				storeFront.AccountAdmin = adminProfile;
				storeFront.RegisteredNotify = adminProfile;
				storeFront.Admin = adminProfile;
				storeFront.WelcomePerson = adminProfile;
				storeFront.Theme = selectedTheme;
				storeFront.MetaApplicationName = "New Storefront";
				storeFront.MetaApplicationTileColor = "#880088";
				storeFront.MetaDescription = "New GStore Storefront";
				storeFront.MetaKeywords = "GStore Storefront";
				storeFront.AdminLayout = "Bootstrap";
				storeFront.AccountLayout = "Bootstrap";
				storeFront.ManageLayout = "Bootstrap";
				storeFront.NotificationsLayout = "Bootstrap";
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
				storeDb.SaveChanges();
			}
			StoreFront firstStoreFront = storeDb.StoreFronts.All().First();
			storeDb.CachedStoreFront = firstStoreFront;
			firstClient = firstStoreFront.Client;

			if (storeDb.StoreBindings.IsEmpty())
			{
				StoreBinding storeBinding = storeDb.StoreBindings.Create();
				storeBinding.ClientId = firstClient.ClientId;
				storeBinding.StoreFrontId = firstStoreFront.StoreFrontId;
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
				storeDb.SaveChanges();
			}

			if (storeDb.PageTemplates.IsEmpty())
			{
				PageTemplate pageTemplate = storeDb.PageTemplates.Create();
				pageTemplate.Name = "Auto-generated Page Template";
				pageTemplate.Description = "Auto-generated Page Template";
				pageTemplate.LayoutName = "Bootstrap";
				pageTemplate.ViewName = "NewStore";
				storeDb.PageTemplates.Add(pageTemplate);
				storeDb.SaveChanges();
			}
			PageTemplate firstPageTemplate = storeDb.PageTemplates.All().First();

			if (storeDb.Pages.IsEmpty())
			{
				Page page = storeDb.Pages.Create();
				page.Client = firstClient;
				page.PageTemplate = firstPageTemplate;
				page.Order = 100;
				page.Name = "New Home Page";
				page.PageTitle = string.Empty;
				page.Public = true;
				page.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
				page.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				page.StoreFront = firstStoreFront;
				page.Url = "/";
				storeDb.Pages.Add(page);

				Page page2 = storeDb.Pages.Create();
				page2.Client = firstClient;
				page2.PageTemplate = firstPageTemplate;
				page2.Order = 200;
				page2.Name = "About Page";
				page2.PageTitle = "About";
				page2.Public = true;
				page2.StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1);
				page2.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				page2.StoreFront = firstStoreFront;
				page2.Url = "/About";
				storeDb.Pages.Add(page2);

				storeDb.SaveChanges();
			}

			//add browser user
			UserProfile browserProfile = browserUser.GetUserProfile(storeDb, false);
			if (browserProfile == null)
			{
				browserProfile = storeDb.UserProfiles.Create();
				browserProfile.UserId = browserUser.Id;
				browserProfile.StoreFront = firstStoreFront;
				browserProfile.AllowUsersToSendSiteMessages = false;
				browserProfile.Email = browserEmail;
				browserProfile.FullName = browserFullName;
				browserProfile.NotifyAllWhenLoggedOn = true;
				browserProfile.NotifyOfSiteUpdatesToEmail = true;
				browserProfile.SendMoreInfoToEmail = false;
				browserProfile.SendSiteMessagesToEmail = true;
				browserProfile.SendSiteMessagesToSms = false;
				browserProfile.SubscribeToNewsletterEmail = true;
				browserProfile.UserName = browserUserName;
				browserProfile.Active = true;
				browserProfile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				browserProfile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				storeDb.UserProfiles.Add(browserProfile);
				storeDb.SaveChanges();
			}

		}
	}
}