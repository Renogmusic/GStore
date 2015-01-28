using GStore.Identity;
using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using GStore.AppHtmlHelpers;
using GStore.Models.ViewModels;
using GStore.Areas.StoreAdmin.ViewModels;
using GStore.Models.BaseClasses;

namespace GStore.Data
{
	public static class StoreFrontExtensions
	{
		/// <summary>
		/// Gets the current store front, or null if anonymous; uses CachedStoreFront from context if available
		/// </summary>
		/// <param name="db"></param>
		/// <param name="request"></param>
		/// <param name="throwErrorIfNotFound">If true, throws an error when storefront is not found</param>
		/// <returns></returns>
		public static StoreFront GetCurrentStoreFront(this IGstoreDb db, HttpRequestBase request, bool throwErrorIfNotFound, bool returnInactiveIfFound, bool ignoreStoreFrontConfigCheck)
		{
			//if context has already set the current store front, return it
			if (db.CachedStoreFront != null && db.CachedStoreFrontConfig != null)
			{
				//note: only active storefront is cached, inactives are always queried from database
				return db.CachedStoreFront;
			}

			if (db.CachedStoreFront != null && db.CachedStoreFrontConfig == null)
			{
				//storefront is set, but not config, try to get current config
				StoreFrontConfiguration storeFrontActiveCurrentConfig = db.CachedStoreFront.CurrentConfig();
				if (storeFrontActiveCurrentConfig != null)
				{
					db.CachedStoreFrontConfig = storeFrontActiveCurrentConfig;
					return db.CachedStoreFront;
				}
				if (ignoreStoreFrontConfigCheck)
				{
					StoreFrontConfiguration storeFrontInactiveConfig = db.CachedStoreFront.CurrentConfigOrAny();
					if (storeFrontInactiveConfig != null)
					{
						return db.CachedStoreFront;
					}
				}
			}


			if (request == null)
			{
				if (throwErrorIfNotFound)
				{
					throw new ApplicationException("Request context is null, cannot get current store front");
				}
				return null;
			}

			//verify database has been seeded and not blank
			if (db.StoreBindings.IsEmpty())
			{
				if (throwErrorIfNotFound)
				{
					string error = "No Store bindings in database, be sure to run the Seed method or Set Settings.InitializeEFCodeFirstMigrateLatest or Settings.InitializeEFCodeFirstDropCreate";
					throw new Exceptions.NoMatchingBindingException(error, request.Url);
				}
				return null;
			}

			StoreBinding activeStoreBinding = db.GetCurrentStoreBindingOrNull(request);
			if (activeStoreBinding != null)
			{
				//active match found, update cache and return the active match
				db.CachedStoreFront = activeStoreBinding.StoreFront;
				StoreFrontConfiguration activeConfig = db.CachedStoreFront.CurrentConfig();
				if (activeConfig != null)
				{
					db.CachedStoreFrontConfig = activeConfig;
					return activeStoreBinding.StoreFront;
				}
				else
				{
					StoreFrontConfiguration inactiveConfig = db.CachedStoreFront.CurrentConfigOrAny();
					if (ignoreStoreFrontConfigCheck && inactiveConfig != null)
					{
						return activeStoreBinding.StoreFront;
					}
					string noConfigErrorMessage = "No active configuration found for store front id [" + db.CachedStoreFront.StoreFrontId + "]";
					if (inactiveConfig == null)
					{
						noConfigErrorMessage += "\n No configuration was found for this store front. Create One.";
					}
					else
					{
						noConfigErrorMessage += "\n Configuration '" + inactiveConfig.ConfigurationName + "' [" + inactiveConfig.StoreFrontConfigurationId + "] is inactive."
							+ "\n IsPending: " + inactiveConfig.IsPending + (inactiveConfig.IsPending ? " <-- Potential Issue" : "")
							+ "\n StartDateTimeUtc: " + inactiveConfig.StartDateTimeUtc.ToString() + (inactiveConfig.StartDateTimeUtc > DateTime.UtcNow ? " <-- Potential Issue" : "")
							+ "\n EndDateTimeUtc: " + inactiveConfig.EndDateTimeUtc.ToString() + (inactiveConfig.EndDateTimeUtc < DateTime.UtcNow ? " <-- Potential Issue" : "");

					}
					throw new Exceptions.StoreFrontInactiveException(noConfigErrorMessage, request.Url, activeStoreBinding.StoreFront);
				}
			}

			if ((throwErrorIfNotFound == false) && (returnInactiveIfFound == false))
			{
				//if throwErrorIfNotFound = false (means no error throw)
				//and
				//if returnInactiveIfFound = false (means I don't want an inactive record

				//so: if we're not throwing an exception and we're not returning an inactive, just quit with null
				return null;
			}

			string errorMessage = "No match found in active store bindings.\n"
				+ " \n BindingHostName: " + request.BindingHostName()
				+ " \n BindingRootPath:" + request.BindingRootPath()
				+ " \n BindingPort:" + request.BindingPort().ToString()
				+ " \n UrlStoreName: " + request.BindingUrlStoreName()
				+ " \n RawUrl: " + request.RawUrl
				+ " \n IP Address: " + request.UserHostAddress
				+ "\n You may want to add a binding catch-all such as HostName=*, RootPath=*, Port=0";

			//why can't we find an active binding? get inactive matches to find if it's an inactive record
			List<StoreBinding> inactiveBindings = db.GetInactiveStoreBindingMatches(request);

			if (inactiveBindings.Count == 0)
			{
				//No match in the database for this host name, root path, and port.  
				//throw error to show ("no store page: GStoreNotFound.html") applies also to hostname hackers and spoofs with host headers
				errorMessage = "Error! StoreFront not found. \nNo StoreBindings match the current host name, port, and RootPath.\n"
					+ "\n No matching bindings found in the inactive records. This is an unhandled URL or a hostname hack."
					+ "\n\n" + errorMessage;

				//we could not find an inactive record, so throw a no match exception or return null if throwErrorIfNotFound = false
				if (throwErrorIfNotFound)
				{
					throw new Exceptions.NoMatchingBindingException(errorMessage, request.Url);
				}
				return null;
			}

			if (returnInactiveIfFound)
			{
				//if returnInactiveIfFound = true; return the best matching inactive record (first)
				return inactiveBindings[0].StoreFront;
			}

			///build error message with details that might help find the inactive record for system admin to fix
			errorMessage = "Error! No ACTIVE StoreFront found in bindings match."
				+ " \n The best match for this URL is inactive."
				+ "\n\n " + errorMessage + "\n"
				+ inactiveBindings.StoreBindingsErrorHelper();

			throw new Exceptions.StoreFrontInactiveException(errorMessage, request.Url, inactiveBindings[0].StoreFront);
		}

		public static StoreBinding GetCurrentStoreBindingOrNull(this IGstoreDb db, HttpRequestBase request)
		{
			string urlStoreName = request.RequestContext.RouteData.UrlStoreName();
			return db.GetCurrentStoreBindingOrNull(urlStoreName, request.BindingHostName(), request.BindingRootPath(), request.BindingPort());
		}

		/// <summary>
		/// Gets the current storebinding record for a request.  Uses catch-alls in the database if available.
		/// Catch-alls are HostName = "*", RootPath = "*", Port = 0 (no quotes) 
		/// </summary>
		/// <param name="db"></param>
		/// <param name="bindingHostName"></param>
		/// <param name="bindingRootPath"></param>
		/// <param name="bindingPort"></param>
		/// <returns></returns>
		public static StoreBinding GetCurrentStoreBindingOrNull(this IGstoreDb db, string urlStoreName, string bindingHostName, string bindingRootPath, int bindingPort)
		{

			IQueryable<StoreBinding> queryBindings = db.StoreBindings.Where(
				sb => ((sb.HostName == "*") || (sb.HostName.ToLower() == bindingHostName))
					&& ((sb.Port == 0) || (sb.Port == bindingPort))
					&& ((sb.RootPath == "*") || (sb.RootPath.ToLower() == bindingRootPath))
				).WhereIsActive().OrderBy(sb => sb.Order).ThenBy(sb => sb.StoreBindingId);

			if (!string.IsNullOrEmpty(urlStoreName))
			{
				IQueryable<StoreBinding> queryByStoreName = queryBindings.Where(sb => sb.UseUrlStoreName && (sb.UrlStoreName.ToLower() == "*" || sb.UrlStoreName.ToLower() == urlStoreName.ToLower()));
				StoreBinding storeBindingByUrlStoreName = queryByStoreName.FirstOrDefault();
				return storeBindingByUrlStoreName;
			}

			IQueryable<StoreBinding> queryNoStoreName = queryBindings.Where(sb => sb.UseUrlStoreName == false);
				
			StoreBinding storeBinding = queryNoStoreName.FirstOrDefault();
			return storeBinding; // may be null
		}

		public static List<StoreBinding> GetInactiveStoreBindingMatches(this IGstoreDb db, HttpRequestBase request)
		{
			return db.GetInactiveStoreBindingMatches(request.BindingUrlStoreName(), request.BindingHostName(), request.BindingRootPath(), request.BindingPort());
		}
		public static List<StoreBinding> GetInactiveStoreBindingMatches(this IGstoreDb db, string urlStoreName, string bindingHostName, string bindingRootPath, int bindingPort)
		{
			IQueryable<StoreBinding> queryBindings = db.StoreBindings.Where(
				sb => ((sb.HostName == "*") || (sb.HostName.ToLower() == bindingHostName))
					&& ((sb.Port == 0) || (sb.Port == bindingPort))
					&& ((sb.RootPath == "*") || (sb.RootPath.ToLower() == bindingRootPath))
				).OrderBy(sb => sb.Order).ThenBy(sb => sb.StoreBindingId);

			if (!string.IsNullOrEmpty(urlStoreName))
			{
				IQueryable<StoreBinding> queryByStoreName = queryBindings.Where(sb => sb.UseUrlStoreName && (sb.UrlStoreName.ToLower() == "*" || sb.UrlStoreName.ToLower() == urlStoreName.ToLower()));
				return queryByStoreName.ToList();
			}

			IQueryable<StoreBinding> queryNoStoreName = queryBindings.Where(sb => sb.UseUrlStoreName == false);

			List<StoreBinding> results = queryNoStoreName.ToList();
			return results;
		}

		public static string BindingHostName(this HttpRequestBase request)
		{
			return request.Url.Host.Trim().ToLower();
		}
		public static string BindingRootPath(this HttpRequestBase request)
		{
			return request.ApplicationPath.ToLower();
		}
		public static int BindingPort(this HttpRequestBase request)
		{
			return request.Url.Port;
		}
		public static string BindingUrlStoreName(this HttpRequestBase request)
		{
			return request.RequestContext.RouteData.UrlStoreName();
		}


		public static string StoreBindingsErrorHelper(this List<StoreBinding> inactiveBindings)
		{
			if (inactiveBindings == null)
			{
				return "InactiveBindings are null. No matches found. You need to create new bindings, such as a catch-all HostName=*, RootPath=*, Port=0";
			}
			if (inactiveBindings.Count == 0)
			{
				return "InactiveBindings are empty. No matches found. You need to create new bindings for this url, such as a catch-all HostName=*, RootPath=*, Port=0";
			}

			string error = "\n-Matching Inactive Bindings found: " + inactiveBindings.Count
				+ "\n---First Match---"
				+ "\n - StoreBindingId: " + inactiveBindings[0].StoreBindingId
				+ "\n - StoreBinding.IsActiveDirect: " + inactiveBindings[0].IsActiveDirect()
				+ (inactiveBindings[0].IsActiveDirect() ? string.Empty : " <--- potential issue")
				+ "\n - StoreBinding.IsPending: " + inactiveBindings[0].IsPending.ToString()
				+ (inactiveBindings[0].IsPending ? " <--- potential issue" : string.Empty)
				+ "\n - StoreBinding.StartDateTimeUtc(local): " + inactiveBindings[0].StartDateTimeUtc.ToLocalTime()
				+ (inactiveBindings[0].StartDateTimeUtc > DateTime.UtcNow ? " <--- potential issue" : string.Empty)
				+ "\n - StoreBinding.EndDateTimeUtc(local): " + inactiveBindings[0].EndDateTimeUtc.ToLocalTime()
				+ (inactiveBindings[0].EndDateTimeUtc < DateTime.UtcNow ? " <--- potential issue" : string.Empty)
				+ "\n - StoreFrontId: " + inactiveBindings[0].StoreFront.StoreFrontId
				+ "\n - StoreFrontId.IsActiveDirect: " + inactiveBindings[0].StoreFront.IsActiveDirect()
				+ (inactiveBindings[0].StoreFront.IsActiveDirect() ? string.Empty : " <--- potential issue")
				+ "\n - StoreFrontId.IsPending: " + inactiveBindings[0].StoreFront.IsPending.ToString()
				+ (inactiveBindings[0].StoreFront.IsPending ? " <--- potential issue" : string.Empty)
				+ "\n - StoreFrontId.StartDateTimeUtc(local): " + inactiveBindings[0].StoreFront.StartDateTimeUtc.ToLocalTime()
				+ (inactiveBindings[0].StoreFront.StartDateTimeUtc > DateTime.UtcNow ? " <--- potential issue" : string.Empty)
				+ "\n - StoreFrontId.EndDateTimeUtc(local): " + inactiveBindings[0].StoreFront.EndDateTimeUtc.ToLocalTime()
				+ (inactiveBindings[0].StoreFront.EndDateTimeUtc < DateTime.UtcNow ? " <--- potential issue" : string.Empty)
				+ "\n - ClientId: " + inactiveBindings[0].Client.ClientId
				+ "\n - Client.IsActiveDirect: " + inactiveBindings[0].Client.IsActiveDirect()
				+ (inactiveBindings[0].Client.IsActiveDirect() ? string.Empty : " <--- potential issue")
				+ "\n - Client.IsPending: " + inactiveBindings[0].Client.IsPending.ToString()
				+ (inactiveBindings[0].Client.IsPending ? " <--- potential issue" : string.Empty)
				+ "\n - Client.StartDateTimeUtc(local): " + inactiveBindings[0].Client.StartDateTimeUtc.ToLocalTime()
				+ (inactiveBindings[0].Client.StartDateTimeUtc > DateTime.UtcNow ? " <--- potential issue" : string.Empty)
				+ "\n - Client.EndDateTimeUtc(local): " + inactiveBindings[0].Client.EndDateTimeUtc.ToLocalTime()
				+ (inactiveBindings[0].Client.EndDateTimeUtc < DateTime.UtcNow ? " <--- potential issue" : string.Empty);

			return error;
		}


		public static Page GetCurrentPage(this StoreFront storeFront, HttpRequestBase request, bool throwErrorIfNotFound = true)
		{
			string url = request.Url.AbsolutePath.Trim('/').ToLower();
			string appPath = request.ApplicationPath.Trim('/').ToLower();

			//remove app path for virtual directories running
			if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(appPath) && url.StartsWith(appPath))
			{
				url = url.Remove(0, appPath.Length).Trim('/');
			}

			string urlStoreName = request.BindingUrlStoreName();
			if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(urlStoreName) && url.StartsWith("stores/" + urlStoreName))
			{
				url = url.Remove(0, ("stores/" + urlStoreName).Length).Trim('/');
			}

			url = "/" + url;

			return storeFront.GetCurrentPage(url, throwErrorIfNotFound);
		}
		public static Page GetCurrentPage(this StoreFront storeFront, string url, bool throwErrorIfNotFound = true)
		{
			string urlLower = url.ToLower();
			var query = storeFront.Pages.Where(p => p.Url.ToLower() == urlLower).AsQueryable().WhereIsActive().OrderBy(p => p.Order).ThenByDescending(p => p.UpdateDateTimeUtc);
			Page page = query.FirstOrDefault();

			if (page == null && urlLower.TrimStart('/').StartsWith("edit"))
			{
				urlLower = "/" + urlLower.TrimStart('/').Substring(4).TrimStart('/');
				var editQuery = storeFront.Pages.Where(p => p.Url.ToLower() == urlLower).AsQueryable().WhereIsActive().OrderBy(p => p.Order).ThenByDescending(p => p.UpdateDateTimeUtc);
				page = query.FirstOrDefault();
			}
			else if (page == null && urlLower.TrimStart('/').StartsWith("view"))
			{
				urlLower = "/" + urlLower.TrimStart('/').Substring(4).TrimStart('/');
				var editQuery = storeFront.Pages.Where(p => p.Url.ToLower() == urlLower).AsQueryable().WhereIsActive().OrderBy(p => p.Order).ThenByDescending(p => p.UpdateDateTimeUtc);
				page = query.FirstOrDefault();
			}
			else if (page == null && urlLower.TrimStart('/').StartsWith("submitform"))
			{
				urlLower = "/" + urlLower.TrimStart('/').Substring(10).TrimStart('/');
				var editQuery = storeFront.Pages.Where(p => p.Url.ToLower() == urlLower).AsQueryable().WhereIsActive().OrderBy(p => p.Order).ThenByDescending(p => p.UpdateDateTimeUtc);
				page = query.FirstOrDefault();
			}

			if (throwErrorIfNotFound && page == null)
			{
				string errorMessage = "Active Page not found for url: " + url
					+ "\n-Store Front [" + storeFront.StoreFrontId + "]: " + storeFront.CurrentConfig().Name
					+ "\n-Client [" + storeFront.Client.ClientId + "]: " + storeFront.Client.Name;

				var inactivePagesQuery = storeFront.Pages.Where(p => p.Url.ToLower() == urlLower).AsQueryable().OrderBy(p => p.Order).ThenByDescending(p => p.UpdateDateTimeUtc);
				List<Page> inactivePages = inactivePagesQuery.ToList();

				if (inactivePages.Count == 0)
				{
					throw new Exceptions.DynamicPageNotFoundException(errorMessage, url, storeFront);
				}
				errorMessage += "\n-Matching Pages found: " + inactivePages.Count
					+ "\n---First (most likely to be selected)---"
					+ "\n - PageId: " + inactivePages[0].PageId
					+ "\n - Page.Name: " + inactivePages[0].Name
					+ "\n - Page.IsActiveDirect: " + inactivePages[0].IsActiveDirect()
					+ "\n - Page.IsPending: " + inactivePages[0].IsPending.ToString()
					+ "\n - Page.StartDateTimeUtc(local): " + inactivePages[0].StartDateTimeUtc.ToLocalTime()
					+ "\n - Page.EndDateTimeUtc(local): " + inactivePages[0].EndDateTimeUtc.ToLocalTime()
					+ "\n - StoreFront [" + inactivePages[0].StoreFront.StoreFrontId + "]: " + inactivePages[0].StoreFront.CurrentConfigOrAny().Name
					+ "\n - Client [" + inactivePages[0].StoreFront.Client.ClientId + "]: " + inactivePages[0].StoreFront.Client.Name;

				throw new Exceptions.DynamicPageInactiveException(errorMessage, url, storeFront);

			}
			return page;
		}

		/// <summary>
		/// Handles null storeFront by returning false for permissions; unless user is sysadmin (then always true)
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <returns></returns>
		public static bool ShowStoreAdminLink(this StoreFront storeFront, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				return false;
			}
			return storeFront.Authorization_IsAuthorized(userProfile, GStoreAction.Admin_StoreAdminArea);
		}

		public static bool ShowOrderAdminLink(this StoreFront storeFront, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				return false;
			}
			return storeFront.Authorization_IsAuthorized(userProfile, GStoreAction.Admin_OrderAdminArea);
		}

		public static bool ShowCatalogAdminLink(this StoreFront storeFront, UserProfile userProfile)
		{
			if (userProfile == null)
			{
				return false;
			}
			return storeFront.Authorization_IsAuthorized(userProfile, GStoreAction.Admin_CatalogAdminArea);
		}

		public static List<TreeNode<ProductCategory>> CategoryTreeWhereActive(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<ProductCategory>>();
			}
			var query = storeFront.ProductCategories.AsQueryable()
				.WhereIsActive()
				.Where(cat => isRegistered || !cat.ForRegisteredOnly)
				.Where(cat => cat.ShowInMenu && (cat.ShowIfEmpty || cat.ChildActiveCount > 0))
				.OrderBy(cat => cat.Order)
				.ThenBy(cat => cat.Name)
				.AsTree(cat => cat.ProductCategoryId, cat => cat.ParentCategoryId);
			return query.ToList();
		}

		public static List<TreeNode<NavBarItem>> NavBarTreeWhereActive(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<NavBarItem>>();
			}

			var query = storeFront.NavBarItems.AsQueryable()
				.WhereIsActive()
				.Where(nav => 
					(isRegistered || !nav.ForRegisteredOnly) 
					&& 
					(!isRegistered || !nav.ForAnonymousOnly ))

				.OrderBy(nav => nav.Order)
				.ThenBy(nav => nav.Name)
				.AsTree(nav => nav.NavBarItemId, nav => nav.ParentNavBarItemId);
			return query.ToList();
		}

		public static bool HasChildNodes(this TreeNode<ProductCategory> category)
		{
			return category.ChildNodes.Any();
		}

		public static bool HasChildMenuItems(this TreeNode<ProductCategory> category, int maxLevels)
		{
			return (maxLevels > category.Depth && category.Entity.AllowChildCategoriesInMenu && category.ChildNodes.Any());
		}

		public static bool HasChildMenuItems(this TreeNode<NavBarItem> navBarItem, int maxLevels)
		{
			return (maxLevels > navBarItem.Depth && navBarItem.ChildNodes.Any());
		}

		public static string OutgoingMessageSignature(this StoreFront storeFront)
		{
			return "\n-Sent From " + storeFront.CurrentConfig().Name + " \n " + storeFront.CurrentConfigOrAny().PublicUrl;
		}

		public static string StoreFrontVirtualDirectoryToMap(this StoreFront storeFront, string applicationPath)
		{
			if (storeFront.CurrentConfig() == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfig()");
			}
			return storeFront.ClientVirtualDirectoryToMap(applicationPath) + "/StoreFronts/" + System.Web.HttpUtility.UrlEncode(storeFront.CurrentConfig().Folder.ToFileName());
		}

		public static string StoreFrontVirtualDirectoryToMapAnyConfig(this StoreFront storeFront, string applicationPath)
		{
			if (storeFront.CurrentConfigOrAny() == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfigOrAny()");
			}
			return storeFront.ClientVirtualDirectoryToMap(applicationPath) + "/StoreFronts/" + System.Web.HttpUtility.UrlEncode(storeFront.CurrentConfigOrAny().Folder.ToFileName());
		}

		public static void SetDefaultsForNew(this StoreFront storeFront, Client client)
		{
			if (client != null)
			{
				storeFront.Client = client;
				storeFront.ClientId = client.ClientId;
				storeFront.Order = (client.StoreFronts.Count == 0 ? 1000 : client.StoreFronts.Max(sf => sf.Order) + 10);
			}
			else
			{
				storeFront.Order = 1000;
			}
			storeFront.IsPending = false;
			storeFront.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeFront.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this StoreFrontConfiguration storeFrontConfig, Client client)
		{
			if (client != null)
			{
				storeFrontConfig.Client = client;
				storeFrontConfig.ClientId = client.ClientId;
			}
			string dateTimeString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");

			storeFrontConfig.ConfigurationName = "Default";
			storeFrontConfig.Name = "Store Front " + (storeFrontConfig.StoreFrontId == 0 ? dateTimeString : storeFrontConfig.StoreFrontId.ToString());
			storeFrontConfig.Folder = storeFrontConfig.Name;
			storeFrontConfig.HtmlFooter = storeFrontConfig.Name.ToHtml();
			storeFrontConfig.Order = 100;
			storeFrontConfig.PublicUrl = "http://www.gstore.renog.info";
			storeFrontConfig.IsPending = false;
			storeFrontConfig.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeFrontConfig.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			storeFrontConfig.MetaApplicationName = storeFrontConfig.Name;
			storeFrontConfig.MetaApplicationTileColor = "#880088";
			storeFrontConfig.MetaDescription = "New GStore Storefront " + storeFrontConfig.Name;
			storeFrontConfig.MetaKeywords = "GStore Storefront " + storeFrontConfig.Name;
			storeFrontConfig.AdminLayoutName = "Default";
			storeFrontConfig.AccountLayoutName = "Default";
			storeFrontConfig.ProfileLayoutName = "Default";
			storeFrontConfig.NotificationsLayoutName = "Default";
			storeFrontConfig.CatalogLayoutName = "Default";
			storeFrontConfig.CatalogAdminLayoutName = "Default";
			storeFrontConfig.CartLayoutName = "Default";
			storeFrontConfig.CheckoutLayoutName = "Default";
			storeFrontConfig.OrdersLayoutName = "Default";
			storeFrontConfig.OrderAdminLayoutName = "Default";
			storeFrontConfig.DefaultNewPageLayoutName = "Default";
			storeFrontConfig.CatalogPageInitialLevels = 6;
			storeFrontConfig.NavBarCatalogMaxLevels = 6;
			storeFrontConfig.NavBarItemsMaxLevels = 6;
			storeFrontConfig.CatalogCategoryColLg = 3;
			storeFrontConfig.CatalogCategoryColMd = 4;
			storeFrontConfig.CatalogCategoryColSm = 6;
			storeFrontConfig.CatalogProductColLg = 2;
			storeFrontConfig.CatalogProductColMd = 3;
			storeFrontConfig.CatalogProductColSm = 6;
			storeFrontConfig.UseShoppingCart = true;
			storeFrontConfig.CartNavShowCartToAnonymous = true;
			storeFrontConfig.CartNavShowCartToRegistered = true;
			storeFrontConfig.CartNavShowCartWhenEmpty = true;
			storeFrontConfig.CartRequireLogin = false;
			storeFrontConfig.NavBarShowRegisterLink = true;
			storeFrontConfig.NavBarRegisterLinkText = "Sign-Up";
			storeFrontConfig.AccountLoginShowRegisterLink = true;
			storeFrontConfig.AccountLoginRegisterLinkText = "Sign-Up";

			storeFrontConfig.ApplyDefaultCartConfig();
			storeFrontConfig.Order = 1000;
		}

		public static void SetDefaultsForNew(this StoreBinding storeBinding, HttpRequestBase request, int? clientId, int? storeFrontId)
		{
			if (clientId.HasValue)
			{
				storeBinding.ClientId = clientId.Value;
			}
			if (storeFrontId.HasValue)
			{
				storeBinding.StoreFrontId = storeFrontId.Value;
			}
			storeBinding.Order = 1000;
			storeBinding.HostName = request.BindingHostName();
			storeBinding.Port = request.BindingPort();
			storeBinding.RootPath = request.BindingRootPath();
			storeBinding.UrlStoreName = request.BindingUrlStoreName();
			storeBinding.UseUrlStoreName = !string.IsNullOrEmpty(storeBinding.UrlStoreName);
			storeBinding.IsPending = false;
			storeBinding.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			storeBinding.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
		}

		public static void HandleNewUserRegisteredNotifications(this StoreFront storeFront, Data.IGstoreDb db, HttpRequestBase request, UserProfile newProfile, string notificationBaseUrl, bool sendUserWelcome, bool sendRegisteredNotify, string customFields)
		{
			UserProfile welcomeUserProfile = storeFront.CurrentConfig().WelcomePerson;
			//HttpRequestBase request = controller.Request;
			
			Notification notification = db.Notifications.Create();
			notification.StoreFront = storeFront;
			notification.Client = storeFront.Client;
			notification.From = (welcomeUserProfile == null ? "System Administrator" : welcomeUserProfile.FullName);
			notification.FromUserProfileId = (welcomeUserProfile == null ? 0 : welcomeUserProfile.UserProfileId);
			notification.ToUserProfileId = newProfile.UserProfileId;
			notification.To = newProfile.FullName;
			notification.Importance = "Normal";
			notification.Subject = "Welcome!";
			notification.UrlHost = request.Url.Host;
			notification.IsPending = false;
			notification.StartDateTimeUtc = DateTime.UtcNow;
			notification.EndDateTimeUtc = DateTime.UtcNow;

			if (!request.Url.IsDefaultPort)
			{
				notification.UrlHost += ":" + request.Url.Port;
			}

			notification.BaseUrl = notificationBaseUrl;
			notification.Message = "Welcome to " + storeFront.CurrentConfig().Name + "!"
				+ "\nEnjoy your stay, and email us if you have any questions, suggestions, feedback, or anything!"
				+ "\n\n" + Settings.IdentitySendGridMailFromName + " - " + Settings.IdentitySendGridMailFromEmail;

			//NotificationLink link1 = db.NotificationLinks.Create();
			//link1.StoreFront = storeFront;
			//link1.Client = storeFront.Client;
			//link1.Order = 1;
			//link1.IsExternal = false;
			//link1.LinkText = "My Songs";
			//link1.Url = "/Songs";
			//link1.Notification = notification;
			//db.NotificationLinks.Add(link1);

			//NotificationLink link2 = db.NotificationLinks.Create();
			//link2.StoreFront = storeFront;
			//link2.Client = storeFront.Client;
			//link2.Order = 2;
			//link2.IsExternal = false;
			//link2.LinkText = "Kids Apps";
			//link2.Url = "/KidApps";
			//link2.Notification = notification;
			//db.NotificationLinks.Add(link2);

			//NotificationLink link3 = db.NotificationLinks.Create();
			//link3.StoreFront = storeFront;
			//link3.Client = storeFront.Client;
			//link3.Order = 3;
			//link3.IsExternal = false;
			//link3.LinkText = "My Experimental Apps";
			//link3.Url = "/PlayApps";
			//link3.Notification = notification;
			//db.NotificationLinks.Add(link3);

			//NotificationLink link4 = db.NotificationLinks.Create();
			//link4.StoreFront = storeFront;
			//link4.Client = storeFront.Client;
			//link4.Order = 4;
			//link4.IsExternal = true;
			//link4.LinkText = "Click Here to Email Me";
			//link4.Url = "mailto:renogmusic@yahoo.com";
			//link4.Notification = notification;
			//db.NotificationLinks.Add(link4);

			db.Notifications.Add(notification);
			db.SaveChanges();

			UserProfile registeredNotify = storeFront.CurrentConfig().RegisteredNotify;
			if (registeredNotify != null)
			{
				HttpSessionStateBase session = request.RequestContext.HttpContext.Session;

				string messageBody = "New User Registered on " + storeFront.CurrentConfig().Name + "!"
					+ "\n-Name: " + newProfile.FullName
					+ "\n-Email: " + newProfile.Email
					+ "\n-Date/Time: " + DateTime.Now.ToString()
					+ "\n-Send Me More Info: " + newProfile.SendMoreInfoToEmail.ToString()
					+ "\n-Notify Of Site Updates: " + newProfile.NotifyOfSiteUpdatesToEmail.ToString()
					+ "\n-Sign-up Notes: " + newProfile.SignupNotes;

				if (!string.IsNullOrEmpty(customFields))
				{
					messageBody += "\n-" + customFields;
				}

				messageBody += "\n - - - - - - - - - -"
					+ "\n-User Profile Id: " + newProfile.UserProfileId
					+ "\n-Url: " + request.Url.ToString()
					+ "\n-Store Front: " + storeFront.CurrentConfig().Name
					+ "\n-Client: " + storeFront.Client.Name + " [" + storeFront.ClientId + "]"
					+ "\n-Host: " + request.Url.Host
					+ "\n-Raw Url: " + request.RawUrl
					+ "\n-IP Address: " + request.UserHostAddress
					+ "\n-User Agent: " + request.UserAgent
					+ "\n-Session Start Date Time: " + session.EntryDateTime().Value.ToString()
					+ "\n-Session Entry Raw Url: " + session.EntryRawUrl() ?? "(none)"
					+ "\n-Session Entry Url: " + session.EntryUrl() ?? "(none)"
					+ "\n-Session Referrer: " + session.EntryReferrer() ?? "(none)";


				Notification newUserNotify = db.Notifications.Create();
				newUserNotify.StoreFront = storeFront;
				newUserNotify.Client = storeFront.Client;
				newUserNotify.From = (welcomeUserProfile == null ? "System Administrator" : welcomeUserProfile.FullName);
				newUserNotify.FromUserProfileId = welcomeUserProfile.UserProfileId;
				newUserNotify.ToUserProfileId = registeredNotify.UserProfileId;
				newUserNotify.To = registeredNotify.FullName;
				newUserNotify.Importance = "Normal";
				newUserNotify.Subject = "New User Registered on " + storeFront.CurrentConfig().Name + " - " + newProfile.FullName + " <" + newProfile.Email + ">";
				newUserNotify.UrlHost = request.Url.Host;
				newUserNotify.IsPending = false;
				newUserNotify.StartDateTimeUtc = DateTime.UtcNow;
				newUserNotify.EndDateTimeUtc = DateTime.UtcNow;

				if (!request.Url.IsDefaultPort)
				{
					newUserNotify.UrlHost += ":" + request.Url.Port;
				}

				newUserNotify.BaseUrl = notificationBaseUrl;
				newUserNotify.Message = messageBody;
				db.Notifications.Add(newUserNotify);
				db.SaveChanges();
			}
		}

		public static void HandleLockedOutNotification(this StoreFront storeFront, Data.IGstoreDb db, HttpRequestBase request, UserProfile profile, string notificationBaseUrl, string forgotPasswordUrl)
		{

			//notify user through site message if their account is locked out, and it has been at least an hour since they were last told they are locked out
			if (profile.LastLockoutFailureNoticeDateTimeUtc != null
				&& (DateTime.UtcNow - profile.LastLockoutFailureNoticeDateTimeUtc.Value).Hours < 1)
			{
				//user has been notified in the past hour, do not re-notify
				return;
			}

			UserProfile accountAdmin = storeFront.CurrentConfig().AccountAdmin;

			Notification notification = db.Notifications.Create();
			notification.StoreFront = storeFront;
			notification.Client = storeFront.Client;
			notification.ToUserProfileId = profile.UserProfileId;
			notification.From = (accountAdmin == null ? "System Administrator" : accountAdmin.FullName);
			notification.FromUserProfileId = (accountAdmin == null ? 0 : accountAdmin.UserProfileId);
			notification.To = profile.FullName;
			notification.Importance = "Low";
			notification.Subject = "Login failure for " + request.Url.Host;
			notification.IsPending = false;
			notification.EndDateTimeUtc = DateTime.UtcNow;
			notification.StartDateTimeUtc = DateTime.UtcNow;
			notification.UrlHost = request.Url.Host;
			if (!request.Url.IsDefaultPort)
			{
				notification.UrlHost += ":" + request.Url.Port;
			}

			notification.BaseUrl = notificationBaseUrl;
			notification.Message = "Somebody tried to log on as you with the wrong password at " + request.Url.Host
				+ " \n\nFor security reasons, your account has been locked out for 5 minutes."
				+ " \n\nIf this was you, please disregard this message and try again in 5 minutes."
				+ " \n\nIf you forgot your password, you can reset it at " + forgotPasswordUrl
				+ " \n\nIf this was not you, the below information may be helpful."
				+ " \n\nIP Address: " + request.UserHostAddress
				+ " \nHost Name: " + request.UserHostName;

			db.Notifications.Add(notification);
			db.SaveChanges();

			UserProfile profileUpdate = db.UserProfiles.FindById(profile.UserProfileId);
			profileUpdate.LastLockoutFailureNoticeDateTimeUtc = DateTime.UtcNow;
			db.SaveChangesDirect();

		}

		public static void CreatePasswordChangedNotification(this IGstoreDb db, StoreFront storeFront, UserProfile userProfile, Uri requestUrl, string notificationBaseUrl)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			UserProfile profile = userProfile;
			UserProfile accountAdmin = storeFront.CurrentConfig().AccountAdmin;
			Notification notification = db.Notifications.Create();

			notification.StoreFront = storeFront;
			notification.ClientId = storeFront.ClientId;
			notification.From = (accountAdmin == null ? "System Administrator" : accountAdmin.FullName);
			notification.FromUserProfileId = (accountAdmin == null ? 0 : accountAdmin.UserProfileId);
			notification.ToUserProfileId = profile.UserProfileId;
			notification.To = profile.FullName;
			notification.Importance = "Normal";
			notification.Subject = "FYI - Your password has been changed";
			notification.UrlHost = requestUrl.Host;
			if (!requestUrl.IsDefaultPort)
			{
				notification.UrlHost += ":" + requestUrl.Port;
			}
			notification.BaseUrl = notificationBaseUrl;
			notification.Message = "FYI - Your password was changed on " + requestUrl.Host
				+ " \n - This is just a courtesy message to let you know.";

			notification.IsPending = false;
			notification.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			notification.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			db.Notifications.Add(notification);
			db.SaveChanges();
		}

		public static void ProcessEmailAndSmsNotifications(this IGstoreDb db, Models.Notification notification, bool runEmailNotifications, bool runSmsNotifications)
		{
			Models.UserProfile profileTo = notification.ToUserProfile;
			if (profileTo == null)
			{
				throw new ApplicationException("Profile not found for new notification! ToUserProfileId: " + notification.ToUserProfileId + " FromUserProfileId: " + notification.FromUserProfileId);
			}
			Identity.AspNetIdentityUser aspNetUserTo = profileTo.AspNetIdentityUser();
			if (aspNetUserTo == null)
			{
				throw new ApplicationException("AspNetUser not found for new notification! ToUserProfileId: " + notification.ToUserProfileId + " FromUserProfileId: " + notification.FromUserProfileId);
			}

			if (runEmailNotifications && profileTo.SendSiteMessagesToEmail && aspNetUserTo.EmailConfirmed)
			{
				string emailTo = profileTo.Email;
				string emailToName = profileTo.FullName;
				string emailSubject = "Msg from " + notification.From + " at GStore - " + notification.Subject;
				string url = "http://" + notification.UrlHost.Trim() + notification.BaseUrl.Trim() + "/" + notification.NotificationId.ToString().Trim();
				string emailTextBody = "There is a new site message for you at GStore!"
					+ "\n\n-From " + notification.From
					+ "\n-Subject: " + notification.Subject
					+ "\n-Priority: " + notification.Importance
					+ "\n-Sent: " + notification.CreateDateTimeUtc.ToLocalTime().ToString()
					+ "\n\n-Link: " + url
					+ "\n\nMessage: \n" + notification.Message;

				string emailHtmlBody = System.Web.HttpUtility.HtmlEncode(emailTextBody).Replace("\n", "<br/>\n");

				emailHtmlBody += "<hr/><a href=\"" + url + "\">Click here to view this message on " + System.Web.HttpUtility.HtmlEncode(notification.UrlHost) + "</a><hr/>";

				int linkCounter = 0;
				foreach (NotificationLink link in notification.NotificationLinks)
				{
					linkCounter++;
					emailHtmlBody += link.FullNotificationLinkTag(linkCounter, notification.UrlHost) + "<br/>";
				}

				AppHtmlHelpers.AppHtmlHelper.SendEmail(notification.Client, emailTo, emailToName, emailSubject, emailTextBody, emailHtmlBody, notification.UrlHost);

				IGstoreDb ctxEmail = db.NewContext();
				UserProfile profileUpdateEmailSent = ctxEmail.UserProfiles.FindById(profileTo.UserProfileId);
				profileUpdateEmailSent.LastSiteMessageSentToEmailDateTimeUtc = DateTime.UtcNow;
				ctxEmail.SaveChangesDirect();
			}

			if (runSmsNotifications && profileTo.SendSiteMessagesToSms && aspNetUserTo.PhoneNumberConfirmed)
			{
				string phoneTo = aspNetUserTo.PhoneNumber;
				string urlHostSms = notification.UrlHost;
				string textBody = "Msg from " + notification.From + " at GStore!"
					+ "\n\n-From " + notification.From
					+ "\n-Subject: " + notification.Subject
					+ "\n-Priority: " + notification.Importance
					+ "\n-Sent: " + notification.CreateDateTimeUtc.ToLocalTime().ToString()
					+ "\n\n-Link: http://" + notification.UrlHost.Trim() + notification.BaseUrl.Trim() + "/" + notification.NotificationId.ToString().Trim()
					+ "\n\nMessage: \n" + (notification.Message.Length < 1200 ? notification.Message : notification.Message.Substring(0, 1200) + "...<more>");

				int linkCounter = 0;
				foreach (NotificationLink link in notification.NotificationLinks)
				{
					linkCounter++;
					textBody += "\n-Link " + linkCounter + ": " + link.FullNotificationLinkUrl(notification.UrlHost);
				}


				AppHtmlHelpers.AppHtmlHelper.SendSms(notification.Client, phoneTo, textBody, urlHostSms);

				IGstoreDb ctxSmsUpdate = db.NewContext();
				UserProfile profileUpdateEmailSent = ctxSmsUpdate.UserProfiles.FindById(profileTo.UserProfileId);
				profileUpdateEmailSent.LastSiteMessageSentToEmailDateTimeUtc = DateTime.UtcNow;
				ctxSmsUpdate.SaveChangesDirect();
			}
		}


		/// <summary>
		/// This is an expensive database and recursion operation, use with caution
		/// </summary>
		/// <param name="storeDb"></param>
		/// <param name="storeFront"></param>
		public static void RecalculateProductCategoryActiveCount(this Data.IGstoreDb storeDb, StoreFront storeFront)
		{
			List<ProductCategory> categories = storeFront.ProductCategories.ToList();

			var treeQuery = storeFront.ProductCategories.AsTree(prod => prod.ProductCategoryId, prod => prod.ParentCategoryId);
			List<TreeNode<ProductCategory>> categoriesTree = treeQuery.ToList();

			foreach (ProductCategory category in categories)
			{
				//foreach category, calculate direct activecount
				int activeCount = category.Products.AsQueryable().WhereIsActive().Count();
				category.DirectActiveCount = activeCount;
			}

			foreach (ProductCategory category in categories)
			{
				TreeNode<ProductCategory> categoryNode = categoriesTree.FindEntity(category);
				int childActiveCount = categoryNode.ActiveCountWithChildren();
				categoryNode.Entity.ChildActiveCount = childActiveCount;
			}
			storeDb.SaveChangesEx(false, false, false, false);
		}

		private static int ActiveCountWithChildren(this TreeNode<ProductCategory> categoryNode)
		{
			int count = categoryNode.Entity.DirectActiveCount;
			foreach (TreeNode<ProductCategory> childNode in categoryNode.ChildNodes)
			{
				count += childNode.ActiveCountWithChildren();
			}

			return count;
		}

		/// <summary>
		/// Main catagory image
		/// </summary>
		/// <param name="category"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this ProductCategory category, string applicationPath, bool forCart = false)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			applicationPath = applicationPath.Trim('/');
			if (!string.IsNullOrEmpty(applicationPath))
			{
				applicationPath += "/";
			}
			return "/" + applicationPath + "Images/Categories/" + category.ImageName;
		}

		/// <summary>
		/// Main product image
		/// </summary>
		/// <param name="product"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this Product product, string applicationPath, bool forCart = false)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			applicationPath = applicationPath.Trim('/');
			if (!string.IsNullOrEmpty(applicationPath))
			{
				applicationPath += "/";
			}
			return "/" + applicationPath + "Images/Products/" + product.ImageName;
		}


		public static string ClientVirtualDirectoryToMap(this Models.BaseClasses.ClientRecord clientRecord, string applicationPath)
		{
			return clientRecord.Client.ClientVirtualDirectoryToMap(applicationPath);
		}

		public static string StoreFrontVirtualDirectoryToMap(this StoreFrontRecord record, string applicationPath)
		{
			return record.ClientVirtualDirectoryToMap(applicationPath) + "/StoreFronts/" + record.StoreFront.CurrentConfig().Folder.ToFileName();
		}

		public static string StoreFrontVirtualDirectoryToMapAnyConfig(this StoreFrontRecord record, string applicationPath)
		{
			if (record.StoreFront.CurrentConfigOrAny() == null)
			{
				throw new ArgumentNullException("record.CurrentConfigOrAny()");
			}
			return record.ClientVirtualDirectoryToMap(applicationPath) + "/StoreFronts/" + record.StoreFront.CurrentConfigOrAny().Folder.ToFileName();
		}

		public static string EmailConfirmationCodeSubject(this StoreFront storeFront, string callbackUrl, Uri currentUrl)
		{
			return "Please confirm your Email account for " + (storeFront == null ? currentUrl.Authority : storeFront.CurrentConfig().Name + " - " + currentUrl.Authority);
		}


		public static string EmailConfirmationCodeMessageHtml(this StoreFront storeFront, string callbackUrl, Uri currentUrl)
		{
			string messageHtml = string.Empty;
			messageHtml = "Thank you for registering at " + currentUrl.Authority + "!<br/><br/>"
				+ "<a href=\"" + callbackUrl + "\">Please click this link to confirm your email address</a>"
				+ "<br/><br/>"
				+ (storeFront == null ? string.Empty : HttpUtility.HtmlEncode(storeFront.OutgoingMessageSignature()).Replace("\n", " \n<br/>") + "<br/><br/>")
				+ "<a href=\"" + HttpUtility.HtmlAttributeEncode(currentUrl.Authority) + "\">" + HttpUtility.HtmlEncode(currentUrl.Authority) + "</a>"
				+ "<br/><br/>" + Settings.IdentitySendGridMailFromName + " - " + Settings.IdentitySendGridMailFromEmail;

			return messageHtml;

		}

		public static string ForgotPasswordSubject(this StoreFront storeFront, string callbackUrl, Uri currentUrl)
		{
			return "Reset Password for " + (storeFront == null ? currentUrl.Authority : storeFront.CurrentConfig().Name + " - " + currentUrl.Authority);
		}

		public static string ForgotPasswordMessageHtml(this StoreFront storeFront, string callbackUrl, Uri currentUrl)
		{
			string messageHtml = "Forgot your password??<br/><br/>"
				+ "No worries! <br/><br/><a href=\"" + callbackUrl + "\">Click here to reset your password</a>"
				+ "<br/><br/>"
				+ (storeFront == null ? string.Empty : HttpUtility.HtmlEncode(storeFront.OutgoingMessageSignature()).Replace("\n", " \n<br/>") + "<br/><br/>")
				+ "<a href=\"" + HttpUtility.HtmlAttributeEncode(currentUrl.Authority) + "\">" + HttpUtility.HtmlEncode(currentUrl.Authority) + "</a>"
				+ "<br/><br/>" + Settings.IdentitySendGridMailFromName + " - " + Settings.IdentitySendGridMailFromEmail;

			return messageHtml;

		}


		public static string AddPhoneNumberMessage(StoreFront storeFront, string code, Uri uri)
		{
			string messageBody = "Your security code is: " + code + " \n";
			if (storeFront != null)
			{
				messageBody += "\n" + storeFront.OutgoingMessageSignature();
			}
			messageBody += "\n\n" + Settings.IdentityTwoFactorSignature;

			return messageBody;
		}

		public static PageTemplateSection CreatePageTemplateSection(this IGstoreDb db, int pageTemplateId, string sectionName, int order, string description, string defaultRawHtmlValue, int clientId, bool editInTop, bool editInBottom, UserProfile userProfile)
		{
			db.UserName = userProfile.UserName;
			PageTemplateSection newSection = db.PageTemplateSections.Create();
			newSection.PageTemplateId = pageTemplateId;
			newSection.ClientId = clientId;
			newSection.Name = sectionName;
			newSection.Order = order;
			newSection.DefaultRawHtmlValue = defaultRawHtmlValue;
			newSection.Description = description;
			newSection.IsPending = false;
			newSection.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newSection.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			db.PageTemplateSections.Add(newSection);
			db.SaveChanges();
			return newSection;
		}


		public static PageSection CreatePageSection(this IGstoreDb db, Models.ViewModels.PageSectionEditViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			if (!storeFront.Pages.Any(pg => pg.PageId == viewModel.PageId))
			{
				throw new ApplicationException("ID Map error. Page Id: " + viewModel.PageId + " not found in storefront pages. Make sure this storefront has a page with the passed PageId or fix the PageId parameter in model.");
			}

			if (!storeFront.Pages.Single(pg => pg.PageId ==  viewModel.PageId)
				.PageTemplate.Sections.Any(pts => pts.PageTemplateSectionId == viewModel.PageTemplateSectionId))
			{
				throw new ApplicationException("ID Map Error. Page Template Section Id: " + viewModel.PageTemplateSectionId + " not found in pages template. Make sure this storefront has the passed PageTemplateSectionId or fix the PageTemplateSectionId in model.");
			}

			PageSection newRecord = db.PageSections.Create();
			newRecord.SetDefaults(userProfile);
			newRecord.ClientId = storeFront.ClientId;
			newRecord.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			newRecord.UseDefaultFromTemplate = viewModel.UseDefaultFromTemplate;
			newRecord.HasNothing = viewModel.HasNothing;
			newRecord.HasPlainText = viewModel.HasPlainText;
			newRecord.HasRawHtml = viewModel.HasRawHtml;
			newRecord.IsPending = viewModel.IsPending;
			newRecord.Order = viewModel.Order;
			newRecord.PageId = viewModel.PageId;
			newRecord.PageTemplateSectionId = viewModel.PageTemplateSectionId;
			newRecord.PlainText = viewModel.PlainText;
			newRecord.RawHtml = viewModel.RawHtml;
			newRecord.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			newRecord.StoreFrontId = storeFront.StoreFrontId;

			db.PageSections.Add(newRecord);
			db.SaveChanges();

			return newRecord;
			
		}

		public static bool ValidatePageUrl(this IGstoreDb db, Controllers.BaseClass.BaseController controller, string url, int storeFrontId, int clientId, int? currentPageId)
		{
			string urlField = (controller.ModelState.ContainsKey("PageEditViewModel_Url") ? "PageEditViewModel_Url" : "Url");

			if (string.IsNullOrWhiteSpace(url))
			{
				string errorMessage = "Url is required \n Please enter a url starting with /";
				controller.ModelState.AddModelError(urlField, errorMessage);
				return false;
			}

			if (!url.StartsWith("/"))
			{
				string errorMessage = "Invalid Url: '" + url + "'. Url must start with a slash. Example / for home page or /Food";
				controller.ModelState.AddModelError(urlField, errorMessage);
				return false;
			}

			if (url.Contains(" "))
			{
				string errorMessage = "Invalid Url: '" + url + "'. Url Cannot have spaces. Be sure to remove spaces from Url. You may replace spaces with underscore _ ";
				controller.ModelState.AddModelError(urlField, errorMessage);
				return false;
			}

			if (url.Contains("?"))
			{
				string errorMessage = "Invalid Url: '" + url + "'. Url Cannot have a question Mark ? in it. You may might choose to replace it with an underscore _ or dash -";
				controller.ModelState.AddModelError(urlField, errorMessage);
				return false;
			}

			if (url.Contains('~') || url.Contains('|') || url.Contains(':') || url.Contains("*") || url.Contains('\"') || url.Contains('<') || url.Contains('>'))
			{
				string errorMessage = "Invalid Url: '" + url + "'. These characters are not allowed in Urls. ~ | : * \\ < > . You might choose to replace these characters with underscore or dash -";
				controller.ModelState.AddModelError(urlField, errorMessage);
				return false;
			}

			if (!System.Uri.IsWellFormedUriString("http://www.test.com" + url, UriKind.Absolute))
			{
				string errorMessage = "Invalid Url: '" + url + "'. Url is not a valid URL. Example: /food   or /food/page1";
				controller.ModelState.AddModelError(urlField, errorMessage);
				return false;
			}

			string trimUrl = "/" + url.Trim().Trim('~').Trim('/').ToLower();
			string[] blockedUrls = { "Account", "Category", "Catalog", "CatalogAdmin", "Cart", "Checkout", "Content", "Edit", "Fonts", "GStore", "Images", "JS", "Notifications", "Order", "OrderAdmin", "Pages", "Products", "Profile", "Styles", "Scripts", "StoreAdmin", "SubmitForm", "SystemAdmin", "Themes", "UpdatePageAjax", "UpdateSectionAjax", "View" };

			foreach (string blockedUrl in blockedUrls)
			{
				if (trimUrl.StartsWith(blockedUrl.ToLower()))
				{
					string errorMessage = "Url '" + url + "' is invalid. Url cannot start with '" + blockedUrl + "' because the system already has built-in " + blockedUrl + " pages. \n Please choose a different url";
					controller.ModelState.AddModelError(urlField, errorMessage);
					return false;
				}
			}

			if (Settings.AppEnableStoresVirtualFolders)
			{
				if (trimUrl.StartsWith("stores"))
				{
					string errorMessage = "Url '" + url + "' is invalid. Url cannot start with 'Stores' because the system already has built-in Stores pages. \n Please choose a different url";
					controller.ModelState.AddModelError(urlField, errorMessage);
					return false;
				}
			}

			Page conflict = db.Pages.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.Url.ToLower() == trimUrl && (p.PageId != currentPageId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "Url '" + url + "' is already in use for page '" + conflict.Name + "' [" + conflict.PageId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique Url or change the conflicting page Url.";

			controller.ModelState.AddModelError(urlField, errorConflictMessage);
			return false;

		}

		public static bool ValidateNavBarItemName(this IGstoreDb db, Controllers.BaseClass.BaseController controller, string name, int storeFrontId, int clientId, int? currentNavBarItemId)
		{
			string nameField = "Name";

			if (string.IsNullOrWhiteSpace(name))
			{
				string errorMessage = "Name is required \n Please enter a unique name for this menu item";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			NavBarItem conflict = db.NavBarItems.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.Name.ToLower() == name && (p.NavBarItemId != currentNavBarItemId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "Name '" + name + "' is already in use for Menu Item '" + conflict.Name + "' [" + conflict.NavBarItemId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique Name or change the conflicting Menu Item Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateValueListName(this IGstoreDb db, Controllers.BaseClass.BaseController controller, string name, int clientId, int? currentValueListId)
		{
			string nameField = "Name";

			if (string.IsNullOrWhiteSpace(name))
			{
				string errorMessage = "Name is required \n Please enter a unique name for this Value List";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			ValueList conflict = db.ValueLists.Where(p => p.ClientId == clientId && p.Name.ToLower() == name.ToLower() && (p.ValueListId != currentValueListId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "Name '" + name + "' is already in use for Value List '" + conflict.Name + "' [" + conflict.ValueListId + "] in Client '" + conflict.Client.Name.ToHtml() + "' [" + conflict.ClientId + "]. \n You must enter a unique Name or change the conflicting Value List Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateValueListFastAddItemName(this IGstoreDb db, Controllers.BaseClass.BaseController controller, string name, ValueList valueList, int? currentValueListItemId)
		{
			string nameField = "fastAddValueListItem";

			if (string.IsNullOrWhiteSpace(name))
			{
				string errorMessage = "Name is required \n Please enter a unique name for this Value List Item";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			ValueListItem conflict = valueList.ValueListItems.Where(p => p.Name.ToLower() == name.ToLower() && (p.ValueListId != currentValueListItemId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "Name '" + name + "' is already in use for Value List Item '" + conflict.Name + "' [" + conflict.ValueListItemId + "] in Client '" + conflict.Client.Name.ToHtml() + "' [" + conflict.ClientId + "]. \n You must enter a unique Name or change the conflicting Value List Item Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateWebFormName(this IGstoreDb db, Controllers.BaseClass.BaseController controller, string name, int clientId, int? currentWebFormId)
		{
			string nameField = "Name";
			if (string.IsNullOrWhiteSpace(name))
			{
				controller.ModelState.AddModelError(nameField, "Name is required. Please enter a name for this web form.");
				return false;
			}

			WebForm conflict = db.WebForms.Where(wf => wf.ClientId == clientId && wf.Name.ToLower() == name && (wf.WebFormId != currentWebFormId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "Name '" + name + "' is already in use for Web Form '" + conflict.Name + "' [" + conflict.WebFormId + "] in Client '" + conflict.Client.Name.ToHtml() + "' [" + conflict.ClientId + "]. \n You must enter a unique Name or change the conflicting Web Form name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateWebFormFieldName(this IGstoreDb db, Controllers.BaseClass.BaseController controller, string name, int clientId, int webFormId, int? currentWebFormFieldId)
		{
			string nameField = "Web Form Field Name";
			if (string.IsNullOrWhiteSpace(name))
			{
				controller.ModelState.AddModelError(nameField, "Field Name is required. Please enter a name for this field.");
				return false;
			}

			WebFormField conflict = db.WebFormFields.Where(wf => wf.ClientId == clientId && wf.WebFormId == webFormId && wf.Name.ToLower() == name && (wf.WebFormId != currentWebFormFieldId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "Name '" + name + "' is already in use for field '" + conflict.Name + "' [" + conflict.WebFormFieldId + "] in Web Form '" + conflict.WebForm.Name  + "' [" + conflict.WebFormId + "] for Client '" + conflict.Client.Name + "' [" + conflict.ClientId + "]. \n You must enter a unique Field Name or change the conflicting Field Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static Page CreatePage(this IGstoreDb db, Models.ViewModels.PageEditViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			Page page = db.Pages.Create();
			page.StoreFrontId = storeFront.StoreFrontId;
			page.ClientId = storeFront.ClientId;

			page.BodyBottomScriptTag = viewModel.BodyBottomScriptTag;
			page.BodyTopScriptTag = viewModel.BodyTopScriptTag;
			page.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			page.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			page.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			page.IsPending = viewModel.IsPending;
			page.MetaDescription = viewModel.MetaDescription;
			page.MetaKeywords = viewModel.MetaKeywords;
			page.MetaApplicationName = viewModel.MetaApplicationName;
			page.MetaApplicationTileColor = viewModel.MetaApplicationTileColor;
			page.Name = viewModel.Name;
			page.Order = viewModel.Order;
			page.PageTitle = viewModel.PageTitle;
			page.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			page.ThemeId = viewModel.ThemeId;
			page.Url = viewModel.Url;
			page.PageTemplateId = viewModel.PageTemplateId;
			page.WebFormId = viewModel.WebFormId;
			page.WebFormSaveToDatabase = viewModel.WebFormSaveToDatabase;
			page.WebFormSaveToFile = viewModel.WebFormSaveToFile;
			page.WebFormSendToEmail = viewModel.WebFormSendToEmail;
			page.WebFormEmailToAddress = viewModel.WebFormEmailToAddress;
			page.WebFormEmailToName = viewModel.WebFormEmailToName;
			page.WebFormSuccessPageId = viewModel.WebFormSuccessPageId;
			page.WebFormThankYouTitle = viewModel.WebFormThankYouTitle;
			page.WebFormThankYouMessage = viewModel.WebFormThankYouMessage;
			page.UpdateAuditFields(userProfile);

			db.Pages.Add(page);
			db.SaveChanges();

			return page;

		}

		public static Page UpdatePage(this IGstoreDb db, Models.ViewModels.PageEditViewModel viewModel, Controllers.BaseClass.BaseController controller, StoreFront storeFront, UserProfile userProfile)
		{
			//find existing record, update it
			Page page = storeFront.Pages.SingleOrDefault(p => p.PageId == viewModel.PageId);
			if (page == null)
			{
				throw new ApplicationException("Page not found in storefront pages. PageId: " + viewModel.PageId);
			}

			page.BodyBottomScriptTag = viewModel.BodyBottomScriptTag;
			page.BodyTopScriptTag = viewModel.BodyTopScriptTag;
			page.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			page.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			page.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			page.IsPending = viewModel.IsPending;
			page.MetaDescription = viewModel.MetaDescription;
			page.MetaKeywords = viewModel.MetaKeywords;
			page.MetaApplicationName = viewModel.MetaApplicationName;
			page.MetaApplicationTileColor = viewModel.MetaApplicationTileColor;
			page.Name = viewModel.Name;
			page.Order = viewModel.Order;
			page.PageTitle = viewModel.PageTitle;
			page.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			page.ThemeId = viewModel.ThemeId;
			page.Url = viewModel.Url;
			if (page.PageTemplateId != viewModel.PageTemplateId)
			{
				if (controller != null)
				{
					controller.AddUserMessage("Page Template Changed", "Page Template has been changed. Be sure to edit the new template sections for template '" + page.PageTemplate.Name.ToHtml() + "' [" + page.PageTemplateId + "].", AppHtmlHelpers.UserMessageType.Info);
				}
				page.PageTemplateId = viewModel.PageTemplateId;
			}

			page.WebFormId = viewModel.WebFormId;
			page.WebFormSaveToDatabase = viewModel.WebFormSaveToDatabase;
			page.WebFormSaveToFile = viewModel.WebFormSaveToFile;
			page.WebFormSendToEmail = viewModel.WebFormSendToEmail;
			page.WebFormEmailToAddress = viewModel.WebFormEmailToAddress;
			page.WebFormEmailToName = viewModel.WebFormEmailToName;
			page.WebFormSuccessPageId = viewModel.WebFormSuccessPageId;
			page.WebFormThankYouTitle = viewModel.WebFormThankYouTitle;
			page.WebFormThankYouMessage = viewModel.WebFormThankYouMessage;
			page.WebFormSaveToDatabase = viewModel.WebFormSaveToDatabase;

			db.Pages.Update(page);
			db.SaveChanges();

			return page;

		}

		public static PageSection UpdatePageSection(this IGstoreDb db, Models.ViewModels.PageSectionEditViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			//find existing record, update it
			PageSection pageSection = storeFront.Pages.Single(p => p.PageId == viewModel.PageId)
				.Sections.Where(ps => ps.PageSectionId == viewModel.PageSectionId).OrderBy(s => s.Order).ThenBy(s => s.PageSectionId).FirstOrDefault();
			if (pageSection == null)
			{
				throw new ApplicationException("Page section not found in storefront page sections. PageId: " + viewModel.PageId + " PageSectionId: " + viewModel.PageSectionId + " PageTemplateSectionId: " + viewModel.PageTemplateSectionId);
			}

			pageSection.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			pageSection.UseDefaultFromTemplate = viewModel.UseDefaultFromTemplate;
			pageSection.HasPlainText = viewModel.HasPlainText;
			pageSection.HasRawHtml = viewModel.HasRawHtml;
			pageSection.HasNothing = viewModel.HasNothing;
			pageSection.IsPending = viewModel.IsPending;
			pageSection.Order = viewModel.Order;
			pageSection.PlainText = viewModel.PlainText;
			pageSection.RawHtml = viewModel.RawHtml;
			pageSection.StartDateTimeUtc = viewModel.StartDateTimeUtc;

			db.PageSections.Update(pageSection);
			db.SaveChanges();

			return pageSection;

		}

		public static NavBarItem CreateNavBarItem(this IGstoreDb db, Areas.StoreAdmin.ViewModels.NavBarItemEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			NavBarItem record = db.NavBarItems.Create();

			record.Action = viewModel.Action;
			record.ActionIdParam = viewModel.ActionIdParam;
			record.Area = viewModel.Area;
			record.Controller = viewModel.Controller;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.htmlAttributes = viewModel.htmlAttributes;
			record.IsAction = viewModel.IsAction;
			record.IsLocalHRef = viewModel.IsLocalHRef;
			record.IsPage = viewModel.IsPage;
			record.IsRemoteHRef = viewModel.IsRemoteHRef;
			record.LocalHRef = viewModel.LocalHRef;
			record.Name = viewModel.Name;
			record.OpenInNewWindow = viewModel.OpenInNewWindow;
			record.Order = viewModel.Order;
			record.PageId = viewModel.PageId;
			record.ParentNavBarItemId = viewModel.ParentNavBarItemId;
			record.RemoteHRef = viewModel.RemoteHRef;
			record.UseDividerAfterOnMenu = viewModel.UseDividerAfterOnMenu;
			record.UseDividerBeforeOnMenu = viewModel.UseDividerBeforeOnMenu;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.UpdateAuditFields(userProfile);

			db.NavBarItems.Add(record);
			db.SaveChanges();

			return record;

		}

		public static NavBarItem CreateNavBarItemForPage(this IGstoreDb db, Page page, StoreFront storeFront, UserProfile userProfile)
		{
			NavBarItem record = db.NavBarItems.Create();

			string navBarItemName = page.Name;

			bool nameIsValid = db.ValidateNavBarItemName(null, navBarItemName, storeFront.StoreFrontId, storeFront.ClientId, null);
			if (!nameIsValid)
			{
				int index = 1;
				do
				{
					navBarItemName = page.Name + "_" + index;
					nameIsValid = db.ValidateNavBarItemName(null, navBarItemName, storeFront.StoreFrontId, storeFront.ClientId, null);
				} while (!nameIsValid);
			}

			record.Name = navBarItemName;
			record.Order = (storeFront.Pages == null ? 100 : storeFront.Pages.Max(pg => pg.Order) + 10);
			record.ForAnonymousOnly = page.ForAnonymousOnly;
			record.ForRegisteredOnly = page.ForRegisteredOnly;
			record.IsPage = true;
			record.PageId = page.PageId;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = false;
			record.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			record.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);

			record.UpdateAuditFields(userProfile);

			db.NavBarItems.Add(record);
			db.SaveChanges();

			return record;

		}

		public static NavBarItem UpdateNavBarItem(this IGstoreDb db, Areas.StoreAdmin.ViewModels.NavBarItemEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			//find existing record, update it
			NavBarItem record = storeFront.NavBarItems.SingleOrDefault(p => p.NavBarItemId == viewModel.NavBarItemId);
			if (record == null)
			{
				throw new ApplicationException("Nav Bar Item not found in storefront Nav Bar Items . Nav Bar Item Id: " + viewModel.NavBarItemId);
			}

			record.Action = viewModel.Action;
			record.ActionIdParam = viewModel.ActionIdParam;
			record.Area = viewModel.Area;
			record.Controller = viewModel.Controller;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.htmlAttributes = viewModel.htmlAttributes;
			record.IsAction = viewModel.IsAction;
			record.IsLocalHRef = viewModel.IsLocalHRef;
			record.IsPage = viewModel.IsPage;
			record.IsRemoteHRef = viewModel.IsRemoteHRef;
			record.LocalHRef = viewModel.LocalHRef;
			record.Name = viewModel.Name;
			record.OpenInNewWindow = viewModel.OpenInNewWindow;
			record.Order = viewModel.Order;
			record.PageId = viewModel.PageId;
			record.ParentNavBarItemId = viewModel.ParentNavBarItemId;
			record.RemoteHRef = viewModel.RemoteHRef;
			record.UseDividerAfterOnMenu = viewModel.UseDividerAfterOnMenu;
			record.UseDividerBeforeOnMenu = viewModel.UseDividerBeforeOnMenu;

			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			record.UpdatedBy = userProfile;
			record.UpdateDateTimeUtc = DateTime.UtcNow;

			db.NavBarItems.Update(record);
			db.SaveChanges();

			return record;

		}

		/// <summary>
		/// re-orders siblings and puts them in order by 10's, and saves to database
		/// </summary>
		/// <param name="navBarItems"></param>
		public static void NavBarItemsRenumberSiblings(this IGstoreDb db, IEnumerable<NavBarItem> navBarItems)
		{
			List<NavBarItem> sortedItems = navBarItems.AsQueryable().ApplyDefaultSort().ToList();

			int order = 100;
			foreach (NavBarItem item in sortedItems)
			{
				item.Order = order;
				order += 10;
			}

			db.SaveChanges();
		}

		public static ValueList CreateValueList(this IGstoreDb db, Areas.StoreAdmin.ViewModels.ValueListEditAdminViewModel viewModel, Client client, UserProfile userProfile)
		{
			ValueList record = db.ValueLists.Create();

			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.Description = viewModel.Description;
			record.ClientId = client.ClientId;
			record.Client = client;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.UpdateAuditFields(userProfile);

			db.ValueLists.Add(record);
			db.SaveChanges();

			return record;

		}

		public static ValueList UpdateValueList(this IGstoreDb db, ValueListEditAdminViewModel viewModel, Client client, UserProfile userProfile)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			//find existing record, update it
			ValueList valueList = client.ValueLists.SingleOrDefault(p => p.ValueListId == viewModel.ValueListId);
			if (valueList == null)
			{
				throw new ApplicationException("Value List not found in client Value Lists. Value List Id: " + viewModel.ValueListId + " Client '" + client.Name + " [" + client.ClientId + "]");
			}

			valueList.Description = viewModel.Description;
			valueList.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			valueList.IsPending = viewModel.IsPending;
			valueList.Name = viewModel.Name;
			valueList.Order = viewModel.Order;
			valueList.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			valueList.UpdateDateTimeUtc = DateTime.UtcNow;
			valueList.UpdatedBy = userProfile;
			valueList.UpdatedBy_UserProfileId = userProfile.UserProfileId;

			valueList.UpdateAuditFields(userProfile);

			valueList = db.ValueLists.Update(valueList);
			db.SaveChanges();

			return valueList;

		}

		public static ValueListItem CreateValueListItemFastAdd(this IGstoreDb db, ValueListEditAdminViewModel viewModel, string fastAddName, Client client, UserProfile userProfile)
		{
			if (string.IsNullOrWhiteSpace(fastAddName))
			{
				throw new ArgumentNullException("fastAddName");
			}
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			ValueListItem newRecord = db.ValueListItems.Create();

			newRecord.Client = client;
			newRecord.ClientId = client.ClientId;
			newRecord.CreateDateTimeUtc = DateTime.UtcNow;
			newRecord.CreatedBy = userProfile;
			newRecord.CreatedBy_UserProfileId = userProfile.UserProfileId;
			newRecord.Description = fastAddName;
			newRecord.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			newRecord.IsPending = false;
			newRecord.Name = fastAddName;
			newRecord.IsInteger = false;
			newRecord.IntegerValue = null;
			newRecord.IsString = true;
			newRecord.StringValue = fastAddName;
			newRecord.Order = (viewModel.ValueListItems == null ? 100 : (viewModel.ValueListItems.Count == 0 ? 100 : (viewModel.ValueListItems.Max(vli => vli.Order) + 100)));
			newRecord.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newRecord.ValueList = viewModel.ValueList;
			newRecord.ValueListId = viewModel.ValueListId;
			newRecord.UpdateAuditFields(userProfile);

			newRecord = db.ValueListItems.Add(newRecord);
			db.SaveChanges();

			return newRecord;

		}

		public static ValueListItem UpdateValueListItem(this IGstoreDb db, ValueListItemEditAdminViewModel viewModel, Client client, UserProfile userProfile)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			if (client == null)
			{
				throw new ArgumentNullException("client");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			//find existing record, update it
			ValueList valueList = client.ValueLists.SingleOrDefault(p => p.ValueListId == viewModel.ValueListId);
			if (valueList == null)
			{
				throw new ApplicationException("Value List not found in client Value Lists. Value List Id: " + viewModel.ValueListId + " Client '" + client.Name + " [" + client.ClientId + "]");
			}
			ValueListItem listItem = valueList.ValueListItems.Where(vli => vli.ValueListItemId == viewModel.ValueListItemId).SingleOrDefault();
			if (listItem == null)
			{
				throw new ApplicationException("Value List Item not found in existing values. Value List Id: " + viewModel.ValueListId + " Value List Item Id: " + viewModel.ValueListItemId  + " Client '" + client.Name + " [" + client.ClientId + "]");
			}

			listItem.Description = viewModel.Description;
			listItem.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			listItem.IsPending = viewModel.IsPending;
			listItem.Name = viewModel.Name;
			listItem.Order = viewModel.Order;
			listItem.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			listItem.UpdateDateTimeUtc = DateTime.UtcNow;
			listItem.UpdatedBy = userProfile;
			listItem.UpdatedBy_UserProfileId = userProfile.UserProfileId;

			listItem.UpdateAuditFields(userProfile);

			listItem = db.ValueListItems.Update(listItem);
			db.SaveChanges();

			return listItem;

		}

		public static WebForm CreateWebForm(this IGstoreDb db, WebFormEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			WebForm webForm = db.WebForms.Create();

			webForm.Client = storeFront.Client;
			webForm.ClientId = storeFront.ClientId;
			webForm.CreateDateTimeUtc = DateTime.UtcNow;
			webForm.CreatedBy = userProfile;
			webForm.CreatedBy_UserProfileId = userProfile.UserProfileId;
			webForm.Description = viewModel.Description;
			webForm.DisplayTemplateName = viewModel.DisplayTemplateName;
			webForm.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			webForm.FieldMdColSpan = viewModel.FieldMdColSpan;
			webForm.FormFooterAfterSubmitHtml = viewModel.FormFooterAfterSubmitHtml;
			webForm.FormFooterBeforeSubmitHtml = viewModel.FormFooterBeforeSubmitHtml;
			webForm.FormHeaderHtml = viewModel.FormHeaderHtml;
			webForm.IsPending = viewModel.IsPending;
			webForm.LabelMdColSpan = viewModel.LabelMdColSpan;
			webForm.Name = viewModel.Name;
			webForm.Order = viewModel.Order;
			webForm.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			webForm.SubmitButtonClass = viewModel.SubmitButtonClass;
			webForm.SubmitButtonText = viewModel.SubmitButtonText;
			webForm.Title = viewModel.Title;
			webForm.UpdateDateTimeUtc = DateTime.UtcNow;
			webForm.UpdatedBy = userProfile;
			webForm.UpdatedBy_UserProfileId = userProfile.UserProfileId;

			webForm.UpdateAuditFields(userProfile);

			webForm = db.WebForms.Add(webForm);
			db.SaveChanges();

			return webForm;

		}

		public static WebForm UpdateWebForm(this IGstoreDb db, WebFormEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			//find existing record, update it
			WebForm webForm = storeFront.Client.WebForms.SingleOrDefault(p => p.WebFormId == viewModel.WebFormId);
			if (webForm == null)
			{
				throw new ApplicationException("Web Form not found in client web forms. Web Form Id: " + viewModel.WebFormId + " Client '" + storeFront.Client.Name + " [" + storeFront.ClientId + "]");
			}

			webForm.Description = viewModel.Description;
			webForm.DisplayTemplateName = viewModel.DisplayTemplateName;
			webForm.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			webForm.FieldMdColSpan = viewModel.FieldMdColSpan;
			webForm.FormFooterAfterSubmitHtml = viewModel.FormFooterAfterSubmitHtml;
			webForm.FormFooterBeforeSubmitHtml = viewModel.FormFooterBeforeSubmitHtml;
			webForm.FormHeaderHtml = viewModel.FormHeaderHtml;
			webForm.IsPending = viewModel.IsPending;
			webForm.LabelMdColSpan = viewModel.LabelMdColSpan;
			webForm.Name = viewModel.Name;
			webForm.Order = viewModel.Order;
			webForm.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			webForm.SubmitButtonClass = viewModel.SubmitButtonClass;
			webForm.SubmitButtonText = viewModel.SubmitButtonText;
			webForm.Title = viewModel.Title;
			webForm.UpdateDateTimeUtc = DateTime.UtcNow;
			webForm.UpdatedBy = userProfile;
			webForm.UpdatedBy_UserProfileId = userProfile.UserProfileId;

			webForm.UpdateAuditFields(userProfile);

			webForm = db.WebForms.Update(webForm);
			db.SaveChanges();

			return webForm;

		}

		public static WebFormField UpdateWebFormField(this IGstoreDb db, WebFormFieldEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}

			if (viewModel.WebFormId == 0)
			{
				throw new ArgumentNullException("viewModel.WebFormId", "viewModel.WebFormId cannot be 0. Make sure it's set in the form");
			}

			if (viewModel.WebFormFieldId == 0)
			{
				throw new ArgumentNullException("viewModel.WebFormFieldId", "viewModel.WebFormFieldId cannot be 0. Make sure it's set in the form");
			}

			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			//find existing record, update it
			WebFormField webFormFieldToUpdate = db.WebFormFields.Where(wf => wf.ClientId == storeFront.ClientId && (wf.WebFormId == viewModel.WebFormId) && (wf.WebFormFieldId == viewModel.WebFormFieldId)).SingleOrDefault();
			if (webFormFieldToUpdate == null)
			{
				throw new ApplicationException("Web Form Field not found in client web form fields. Web Form Field Id: " + viewModel.WebFormFieldId + " Web Form Id: " + viewModel.WebFormId + " Client '" + storeFront.Client.Name + " [" + storeFront.ClientId + "]");
			}

			webFormFieldToUpdate.DataType = viewModel.DataType;
			webFormFieldToUpdate.DataTypeString = viewModel.DataType.ToDisplayName();
			webFormFieldToUpdate.Description = viewModel.Description;
			webFormFieldToUpdate.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			webFormFieldToUpdate.HelpLabelBottomText = viewModel.HelpLabelBottomText;
			webFormFieldToUpdate.HelpLabelTopText = viewModel.HelpLabelTopText;
			webFormFieldToUpdate.IsPending = viewModel.IsPending;
			webFormFieldToUpdate.IsRequired = viewModel.IsRequired;
			webFormFieldToUpdate.LabelText = viewModel.LabelText;
			webFormFieldToUpdate.Watermark = viewModel.Watermark;
			webFormFieldToUpdate.Name = viewModel.Name;
			webFormFieldToUpdate.Order = viewModel.Order;
			webFormFieldToUpdate.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			webFormFieldToUpdate.TextAreaColumns = viewModel.TextAreaColumns;
			webFormFieldToUpdate.TextAreaRows = viewModel.TextAreaRows;
			webFormFieldToUpdate.UpdateDateTimeUtc = DateTime.UtcNow;
			webFormFieldToUpdate.UpdatedBy = userProfile;
			webFormFieldToUpdate.UpdatedBy_UserProfileId = userProfile.UserProfileId;
			webFormFieldToUpdate.ValueListId = viewModel.ValueListId;

			webFormFieldToUpdate.UpdateAuditFields(userProfile);
			try
			{
				webFormFieldToUpdate = db.WebFormFields.Update(webFormFieldToUpdate);
				db.SaveChanges();
				return webFormFieldToUpdate;
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Database update failed", ex);
			}
		}

		public static WebFormField CreateWebFormFieldFastAdd(this IGstoreDb db, WebFormEditAdminViewModel viewModel, string FastAddField, StoreFront storeFront, UserProfile userProfile)
		{
			if (string.IsNullOrWhiteSpace(FastAddField))
			{
				throw new ArgumentNullException("FastAddField");
			}
			if (viewModel == null)
			{
				throw new ArgumentNullException("viewModel");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			if (viewModel.WebForm != null)
			{

			}

			WebFormField webFormField = db.WebFormFields.Create();
			webFormField.SetDefaultsForNew(viewModel.WebForm);

			webFormField.Client = storeFront.Client;
			webFormField.ClientId = storeFront.ClientId;
			webFormField.CreateDateTimeUtc = DateTime.UtcNow;
			webFormField.CreatedBy = userProfile;
			webFormField.CreatedBy_UserProfileId = userProfile.UserProfileId;
			webFormField.Description = FastAddField;
			webFormField.HelpLabelBottomText = null;
			webFormField.HelpLabelTopText = null;
			webFormField.IsRequired = false;
			webFormField.LabelText = FastAddField;
			webFormField.Watermark = FastAddField;
			webFormField.Name = FastAddField;
			webFormField.TextAreaColumns = null;
			webFormField.TextAreaRows = null;
			webFormField.WebForm = viewModel.WebForm;
			webFormField.WebFormId = viewModel.WebFormId;
			webFormField.UpdateAuditFields(userProfile);

			webFormField = db.WebFormFields.Add(webFormField);
			db.SaveChanges();

			return webFormField;

		}



		public static RouteData RouteData(this HttpRequest request)
		{
			if (request == null)
			{
				return null;
			}
			return request.RequestContext.RouteData;
		}

		public static string UrlStoreName(this RouteData routeData)
		{
			if (routeData == null)
			{
				return null;
			}

			if (!routeData.Values.ContainsKey("urlstorename"))
			{
				return null;
			}

			return routeData.Values["urlstorename"].ToString();
		}

		public static void ApplyDefaultCartConfig(this StoreFrontConfiguration storeFrontConfig)
		{
			storeFrontConfig.UseShoppingCart = true;
			storeFrontConfig.CartLayoutName = "Default";
			storeFrontConfig.CartNavShowCartToAnonymous = true;
			storeFrontConfig.CartNavShowCartToRegistered = true;
			storeFrontConfig.CartNavShowCartWhenEmpty = true;
			storeFrontConfig.CartRequireLogin = false;

			if (storeFrontConfig.DefaultNewPageTheme != null)
			{
				storeFrontConfig.CartTheme = storeFrontConfig.DefaultNewPageTheme;
				storeFrontConfig.CartThemeId = storeFrontConfig.DefaultNewPageTheme.ThemeId;
			}
			storeFrontConfig.CartPageTitle  = "Shopping Cart";
			storeFrontConfig.CartPageHeading = "Your Shopping Cart";
			storeFrontConfig.CartEmptyMessage = "Your Cart is Empty";
			storeFrontConfig.CartItemColumnLabel = "Item";
			storeFrontConfig.CartItemVariantColumnShow = true;
			storeFrontConfig.CartItemVariantColumnLabel = "Type";
			storeFrontConfig.CartItemListPriceColumnShow = true;
			storeFrontConfig.CartItemListPriceColumnLabel = "List Price";
			storeFrontConfig.CartItemUnitPriceColumnShow = true;
			storeFrontConfig.CartItemUnitPriceColumnLabel = "Unit Price";
			storeFrontConfig.CartItemQuantityColumnShow = true;
			storeFrontConfig.CartItemQuantityColumnLabel = "Quantity";
			storeFrontConfig.CartItemListPriceExtColumnShow = true;
			storeFrontConfig.CartItemListPriceExtColumnLabel = "List Price Ext";
			storeFrontConfig.CartItemUnitPriceExtColumnShow = true;
			storeFrontConfig.CartItemUnitPriceExtColumnLabel = "Unit Price Ext";
			storeFrontConfig.CartItemDiscountColumnShow = true;
			storeFrontConfig.CartItemDiscountColumnLabel = "Your Savings";
			storeFrontConfig.CartItemTotalColumnShow = true;
			storeFrontConfig.CartItemTotalColumnLabel = "Total";
			storeFrontConfig.CartOrderDiscountCodeSectionShow = true;
			storeFrontConfig.CartOrderDiscountCodeLabel = "Discount Code";
			storeFrontConfig.CartOrderDiscountCodeApplyButtonText = "Apply Code";
			storeFrontConfig.CartOrderDiscountCodeRemoveButtonText = "Remove Code";
			storeFrontConfig.CartOrderItemCountShow = true;
			storeFrontConfig.CartOrderItemCountLabel = "Total Items in Cart";
			storeFrontConfig.CartOrderSubtotalShow = true;
			storeFrontConfig.CartOrderSubtotalLabel = "Sub-Total";
			storeFrontConfig.CartOrderTaxShow = true;
			storeFrontConfig.CartOrderTaxLabel = "Tax";
			storeFrontConfig.CartOrderShippingShow = true;
			storeFrontConfig.CartOrderShippingLabel = "Shipping";
			storeFrontConfig.CartOrderHandlingShow = true;
			storeFrontConfig.CartOrderHandlingLabel = "Handling";
			storeFrontConfig.CartOrderDiscountShow = true;
			storeFrontConfig.CartOrderDiscountLabel = "Order Discount";
			storeFrontConfig.CartOrderTotalLabel = "Order Total";
		}

		public static void ApplyDefaultCheckoutConfig(this StoreFrontConfiguration storeFrontConfig)
		{
			storeFrontConfig.CheckoutLayoutName = "Default";
			if (storeFrontConfig.DefaultNewPageTheme != null)
			{
				storeFrontConfig.CheckoutTheme = storeFrontConfig.DefaultNewPageTheme;
				storeFrontConfig.CheckoutThemeId = storeFrontConfig.DefaultNewPageTheme.ThemeId;
			}
			storeFrontConfig.CheckoutLogInOrGuestWebForm = null;
			storeFrontConfig.CheckoutLogInOrGuestWebFormId = null;
			storeFrontConfig.CheckoutDeliveryInfoDigitalOnlyWebForm = null;
			storeFrontConfig.CheckoutDeliveryInfoDigitalOnlyWebFormId = null;
			storeFrontConfig.CheckoutDeliveryInfoShippingWebForm = null;
			storeFrontConfig.CheckoutDeliveryInfoShippingWebFormId = null;
			storeFrontConfig.CheckoutDeliveryMethodWebForm = null;
			storeFrontConfig.CheckoutDeliveryMethodWebFormId = null;
			storeFrontConfig.CheckoutPaymentInfoWebForm = null;
			storeFrontConfig.CheckoutPaymentInfoWebFormId = null;
			storeFrontConfig.CheckoutConfirmOrderWebForm = null;
			storeFrontConfig.CheckoutConfirmOrderWebFormId = null;
		}

		public static void ApplyDefaultOrdersConfig(this StoreFrontConfiguration storeFrontConfig)
		{
			storeFrontConfig.OrdersLayoutName = "Default";
			if (storeFrontConfig.DefaultNewPageTheme != null)
			{
				storeFrontConfig.OrdersTheme = storeFrontConfig.DefaultNewPageTheme;
				storeFrontConfig.OrdersThemeId = storeFrontConfig.DefaultNewPageTheme.ThemeId;
			}
		}

		public static StoreFrontConfiguration CloneStoreFrontConfiguration(this IGstoreDb db, StoreFrontConfiguration storeFrontConfig, UserProfile userProfile)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			StoreFrontConfiguration newConfig = db.StoreFrontConfigurations.Create(storeFrontConfig);
			newConfig.ConfigurationName = "New Configuration Created " + DateTime.UtcNow.ToString();
			newConfig.StoreFrontConfigurationId = 0;
			newConfig.Order = storeFrontConfig.StoreFront.StoreFrontConfigurations.Max(c => c.Order) + 10;
			newConfig.SetDefaults(userProfile);

			return newConfig;
		}
	}
}