using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.StoreAdmin.ViewModels;
using GStoreData.ControllerBase;
using GStoreData.Identity;
using GStoreData.Models;
using GStoreData.Models.BaseClasses;
using GStoreData.PayPal;
using GStoreData.PayPal.Models;

namespace GStoreData
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
					string error = "No Store bindings in database.";
					if (db.StoreFronts.IsEmpty())
					{
						error = "No Store Fronts in database. Be sure database is seeded or force a seed of the database." + "\n" + error;
					}
					if (db.Clients.IsEmpty())
					{
						error = "No Clients in database. Be sure database is seeded or force a seed of the database." + "\n" + error;
					}
					if (Settings.AppDoNotSeedDatabase)
					{
						error += "\nSettings.AppDoNotSeedDatabase = true. Change this settings to false to allow the system to seed the database with client and store front data.";
					}

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

			string bindingHostNameNoWww = null;
			if (bindingHostName.ToLower().StartsWith("www."))
			{
				bindingHostNameNoWww = bindingHostName.Substring(4);
			}
			IQueryable<StoreBinding> queryBindings = db.StoreBindings.Where(
				sb => ((sb.HostName == "*") || (sb.HostName.ToLower() == bindingHostName) || (sb.HostName.ToLower() == bindingHostNameNoWww))
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
				IQueryable<StoreBinding> queryByStoreName = queryBindings.Where(sb => sb.UseUrlStoreName && (sb.UrlStoreName.ToLower() == "*" || sb.UrlStoreName.ToLower() == urlStoreName));
				return queryByStoreName.ToList();
			}

			IQueryable<StoreBinding> queryNoStoreName = queryBindings.Where(sb => sb.UseUrlStoreName == false);

			List<StoreBinding> results = queryNoStoreName.ToList();
			return results;
		}

		/// <summary>
		/// Returns the binding host name to lower case
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static string BindingHostName(this HttpRequestBase request)
		{
			return request.Url.Host.Trim().ToLower();
		}

		/// <summary>
		/// Returns the binding root path to lowercase
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static string BindingRootPath(this HttpRequestBase request)
		{
			return request.ApplicationPath.ToLower();
		}

		/// <summary>
		/// Returns the binding port
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static int BindingPort(this HttpRequestBase request)
		{
			return request.Url.Port;
		}

		/// <summary>
		/// Returns the binding url store name to lower case
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
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

		public static List<TreeNode<ProductCategory>> CategoryTreeWhereActiveForNavBar(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<ProductCategory>>();
			}
			var query = storeFront.ProductCategories.AsQueryable()
				.WhereIsActive()
				.WhereRegisteredAnonymousCheck(isRegistered)
				.WhereShowInNavBarMenu(isRegistered)
				.OrderBy(cat => cat.Order)
				.ThenBy(cat => cat.Name)
				.AsTree(cat => cat.ProductCategoryId, cat => cat.ParentCategoryId);
			return query.ToList();
		}


		public static List<TreeNode<ProductCategory>> CategoryTreeWhereActiveForCatalogList(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<ProductCategory>>();
			}
			var query = storeFront.ProductCategories.AsQueryable()
				.WhereIsActive()
				.WhereRegisteredAnonymousCheck(isRegistered)
				.WhereShowInCatalogList(isRegistered)
				.OrderBy(cat => cat.Order)
				.ThenBy(cat => cat.Name)
				.AsTree(cat => cat.ProductCategoryId, cat => cat.ParentCategoryId);
			return query.ToList();
		}

		public static List<TreeNode<ProductCategory>> CategoryTreeWhereActiveForCatalogByName(this StoreFront storeFront, bool isRegistered)
		{
			if (storeFront == null)
			{
				return new List<TreeNode<ProductCategory>>();
			}
			var query = storeFront.ProductCategories.AsQueryable()
				.WhereIsActive()
				.WhereRegisteredAnonymousCheck(isRegistered)
				.WhereShowInCatalogByName(isRegistered)
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
					(!isRegistered || !nav.ForAnonymousOnly))
				.Where(nav => nav.Page == null || (!nav.Page.IsPending && nav.Page.StartDateTimeUtc < DateTime.UtcNow && nav.Page.EndDateTimeUtc > DateTime.UtcNow))
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
			return storeFront.ClientVirtualDirectoryToMapToStoreFronts(applicationPath) + "/" + System.Web.HttpUtility.UrlEncode(storeFront.CurrentConfig().Folder.ToFileName());
		}

		public static string StoreFrontVirtualDirectoryToMapAnyConfig(this StoreFront storeFront, string applicationPath)
		{
			if (storeFront.CurrentConfigOrAny() == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfigOrAny()");
			}
			return storeFront.ClientVirtualDirectoryToMapToStoreFronts(applicationPath) + "/" + System.Web.HttpUtility.UrlEncode(storeFront.CurrentConfigOrAny().Folder.ToFileName());
		}

		public static string CatalogCategoryContentVirtualDirectoryToMap(this StoreFront storeFront, string applicationPath)
		{
			if (storeFront.CurrentConfig() == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfig()");
			}
			return storeFront.StoreFrontVirtualDirectoryToMap(applicationPath) + "/CatalogContent/Categories";
		}

		public static string CatalogProductContentVirtualDirectoryToMap(this StoreFront storeFront, string applicationPath)
		{
			if (storeFront.CurrentConfig() == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfig()");
			}
			return storeFront.StoreFrontVirtualDirectoryToMap(applicationPath) + "/CatalogContent/Products";
		}

		public static string CatalogProductBundleContentVirtualDirectoryToMap(this StoreFront storeFront, string applicationPath)
		{
			if (storeFront.CurrentConfig() == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfig()");
			}
			return storeFront.StoreFrontVirtualDirectoryToMap(applicationPath) + "/CatalogContent/Bundles";
		}

		public static string ProductDigitalDownloadVirtualDirectoryToMap(this StoreFront storeFront, string applicationPath)
		{
			if (storeFront.CurrentConfig() == null)
			{
				throw new ArgumentNullException("storeFront.CurrentConfig()");
			}
			return storeFront.StoreFrontVirtualDirectoryToMap(applicationPath) + "/DigitalDownload/Products";
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
			string dateTimeString = DateTime.UtcNow.ToLocalTime().ToString("yyyy_MM_dd_HH_mm_ss");

			storeFrontConfig.ConfigurationName = "Default";
			storeFrontConfig.Name = "Store Front " + (storeFrontConfig.StoreFrontId == 0 ? dateTimeString : storeFrontConfig.StoreFrontId.ToString());
			storeFrontConfig.Folder = storeFrontConfig.Name;
			storeFrontConfig.HtmlFooter = storeFrontConfig.Name.ToHtml();
			storeFrontConfig.HomePageUseCatalog = true;
			storeFrontConfig.Order = 100;
			storeFrontConfig.PublicUrl = "http://www.gstore.renog.info";
			storeFrontConfig.TimeZoneId = (client == null ? Settings.AppDefaultTimeZoneId : client.TimeZoneId);
			storeFrontConfig.IsPending = false;
			storeFrontConfig.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			storeFrontConfig.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			storeFrontConfig.MetaApplicationName = storeFrontConfig.Name;
			storeFrontConfig.MetaApplicationTileColor = "#880088";
			storeFrontConfig.MetaDescription = "New GStore Storefront " + storeFrontConfig.Name;
			storeFrontConfig.MetaKeywords = "GStore Storefront " + storeFrontConfig.Name;
			storeFrontConfig.CatalogPageInitialLevels = 6;
			storeFrontConfig.CatalogTitle = storeFrontConfig.Name + " Catalog";
			storeFrontConfig.CatalogLayout = CatalogLayoutEnum.SimpleBlocked;
			storeFrontConfig.CatalogHeaderHtml = null;
			storeFrontConfig.CatalogFooterHtml = null;
			storeFrontConfig.CatalogRootListTemplate = CategoryListTemplateEnum.Default;
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

		public static void HandleNewUserRegisteredNotifications(this StoreFront storeFront, IGstoreDb db, HttpRequestBase request, UserProfile newProfile, string notificationBaseUrl, bool sendUserWelcome, bool sendRegisteredNotify, string customFields)
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
					+ "\n-Date/Time: " + DateTime.UtcNow.ToLocalTime().ToString()
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

		public static void HandleLockedOutNotification(this StoreFront storeFront, IGstoreDb db, HttpRequestBase request, UserProfile profile, string notificationBaseUrl, string forgotPasswordUrl)
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

				db.SendEmailFromDBContext(notification.Client, emailTo, emailToName, emailSubject, emailTextBody, emailHtmlBody, notification.UrlHost);

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

				AppHtmlHelpers.AppHtmlHelper.SendSmsFromDBContext(db, notification.Client, phoneTo, textBody, urlHostSms);

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
		public static void RecalculateProductCategoryActiveCount(this IGstoreDb storeDb, StoreFront storeFront)
		{
			List<ProductCategory> categories = storeFront.ProductCategories.ToList();

			var treeQuery = storeFront.ProductCategories.AsTree(prod => prod.ProductCategoryId, prod => prod.ParentCategoryId);
			List<TreeNode<ProductCategory>> categoriesTree = treeQuery.ToList();

			foreach (ProductCategory category in categories)
			{
				//foreach category, calculate direct activecount
				int activeCountForAnonymous = category.Products.AsQueryable().WhereIsActive().Where(p => !p.ForRegisteredOnly).Count() + category.ProductBundles.AsQueryable().WhereIsActive().Where(p => !p.ForRegisteredOnly).Count();
				category.DirectActiveCountForAnonymous = activeCountForAnonymous;

				int activeCountForRegistered = category.Products.AsQueryable().WhereIsActive().Where(p => !p.ForAnonymousOnly).Count() + category.ProductBundles.AsQueryable().WhereIsActive().Where(p => !p.ForAnonymousOnly).Count();
				category.DirectActiveCountForRegistered = activeCountForRegistered;

			}

			foreach (ProductCategory category in categories)
			{
				TreeNode<ProductCategory> categoryNode = categoriesTree.FindEntity(category);
				int childActiveCountForAnonymous = categoryNode.ActiveCountWithChildrenForAnonymous();
				int childActiveCountForRegistered = categoryNode.ActiveCountWithChildrenForRegistered();
				categoryNode.Entity.ChildActiveCountForAnonymous = childActiveCountForAnonymous;
				categoryNode.Entity.ChildActiveCountForRegistered = childActiveCountForRegistered;
			}
			storeDb.SaveChangesEx(false, false, false, false);
		}

		/// <summary>
		/// This is an expensive database and recursion operation, use with caution
		/// </summary>
		/// <param name="storeDb"></param>
		/// <param name="storeFront"></param>
		public static void RecalculateProductCategoryActiveCount(this IGstoreDb storeDb, int storeFrontId)
		{
			StoreFront storeFront = storeDb.StoreFronts.FindById(storeFrontId);
			storeDb.RecalculateProductCategoryActiveCount(storeFront);
		}

		private static int ActiveCountWithChildrenForAnonymous(this TreeNode<ProductCategory> categoryNode)
		{
			int count = categoryNode.Entity.DirectActiveCountForAnonymous;
			foreach (TreeNode<ProductCategory> childNode in categoryNode.ChildNodes)
			{
				count += childNode.ActiveCountWithChildrenForAnonymous();
			}

			return count;
		}

		private static int ActiveCountWithChildrenForRegistered(this TreeNode<ProductCategory> categoryNode)
		{
			int count = categoryNode.Entity.DirectActiveCountForRegistered;
			foreach (TreeNode<ProductCategory> childNode in categoryNode.ChildNodes)
			{
				count += childNode.ActiveCountWithChildrenForRegistered();
			}

			return count;
		}

		/// <summary>
		/// Returns a path of this category up to the top
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public static string CategoryPath(this ProductCategory category, string delimiter = " -> ", int maxLevels = 10)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			return category.CategoryPathRecurse(delimiter, maxLevels);
		}

		private static string CategoryPathRecurse(this ProductCategory category, string delimiter = " -> " , int maxLevels = 10, int currentLevel = 1)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}

			string text = category.Name;

			if (category.ParentCategoryId.HasValue && (currentLevel < maxLevels))
			{
				text = category.ParentCategory.CategoryPathRecurse(delimiter, maxLevels, currentLevel + 1) + delimiter + text;
			}
			return text;
		}

		/// <summary>
		/// Main category image
		/// </summary>
		/// <param name="category"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this ProductCategory category, string applicationPath, RouteData routeData, bool forCart = false)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			if (string.IsNullOrWhiteSpace(category.ImageName))
			{
				return null;
			}
			return category.StoreFront.ProductCategoryCatalogFileUrl(applicationPath, routeData, category.ImageName);
		}

		public static string ProductCategoryCatalogFileUrl(this StoreFront storeFront, string applicationPath, RouteData routeData, string fileName)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return null;
			}
			applicationPath = applicationPath.Trim('/');
			if (!string.IsNullOrEmpty(applicationPath))
			{
				applicationPath += "/";
			}
			if (routeData != null)
			{
				string storeName = routeData.UrlStoreName();
				if (!string.IsNullOrEmpty(storeName))
				{
					applicationPath += "Stores/" + HttpUtility.UrlEncode(storeName) + "/";
				}
			}
			return "/" + applicationPath + "CatalogContent/Categories/" + fileName;
		}

		public static string ImagePath(this ProductCategory category, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (category == null || string.IsNullOrEmpty(category.ImageName))
			{
				return null;
			}

			return category.StoreFront.ProductCategoryCatalogFilePath(applicationPath, routeData, server, category.ImageName);
		}

		public static string ProductCategoryCatalogFilePath(this StoreFront storeFront, string applicationPath, RouteData routeData, HttpServerUtilityBase server, string fileName)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}

			string path = "/CatalogContent/Categories/" + fileName;
			return storeFront.ChooseFilePath(storeFront.Client, path, applicationPath, server);
		}

		/// <summary>
		/// Main product image
		/// </summary>
		/// <param name="product"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this Product product, string applicationPath, RouteData routeData, bool forCart = false)
		{
			if (product == null)
			{
				return null; ;
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.ImageName);
		}

		public static string ImagePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.ImageName);
		}


		/// <summary>
		/// Main category image
		/// </summary>
		/// <param name="category"></param>
		/// <param name="applicationPath"></param>
		/// <returns></returns>
		public static string ImageUrl(this ProductBundle bundle, string applicationPath, RouteData routeData, bool forCart = false)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			if (string.IsNullOrWhiteSpace(bundle.ImageName))
			{
				return null;
			}
			return bundle.StoreFront.ProductBundleCatalogFileUrl(applicationPath, routeData, bundle.ImageName);
		}

		public static string ProductBundleCatalogFileUrl(this StoreFront storeFront, string applicationPath, RouteData routeData, string fileName)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return null;
			}
			applicationPath = applicationPath.Trim('/');
			if (!string.IsNullOrEmpty(applicationPath))
			{
				applicationPath += "/";
			}
			if (routeData != null)
			{
				string storeName = routeData.UrlStoreName();
				if (!string.IsNullOrEmpty(storeName))
				{
					applicationPath += "Stores/" + HttpUtility.UrlEncode(storeName) + "/";
				}
			}
			return "/" + applicationPath + "CatalogContent/Bundles/" + fileName;
		}

		public static string ProductBundleCatalogFilePath(this StoreFront storeFront, string applicationPath, RouteData routeData, HttpServerUtilityBase server, string fileName)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}

			string path = "/CatalogContent/Bundles/" + fileName;
			return storeFront.ChooseFilePath(storeFront.Client, path, applicationPath, server);
		}


		/// <summary>
		/// Returns the URL to the product sample audio file
		/// </summary>
		/// <param name="product"></param>
		/// <param name="applicationPath"></param>
		/// <param name="routeData"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public static string SampleAudioUrl(this Product product, string applicationPath, RouteData routeData)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.SampleAudioFileName);
		}

		/// <summary>
		/// Returns the full file path to the product sample audio file
		/// </summary>
		/// <param name="product"></param>
		/// <param name="applicationPath"></param>
		/// <param name="routeData"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public static string SampleAudioPath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.SampleAudioFileName);
		}

		public static string DigitalDownloadFilePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductDigitalDownloadFilePath(applicationPath, routeData, server, product.DigitalDownloadFileName);
		}

		public static string SampleDownloadFileUrl(this Product product, string applicationPath, RouteData routeData)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.SampleDownloadFileName);
		}

		public static string SampleDownloadFilePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.SampleDownloadFileName);
		}

		public static string SampleImageFileUrl(this Product product, string applicationPath, RouteData routeData)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			return product.StoreFront.ProductCatalogFileUrl(applicationPath, routeData, product.SampleImageFileName);
		}

		public static string SampleImageFilePath(this Product product, string applicationPath, RouteData routeData, HttpServerUtilityBase server)
		{
			if (product == null)
			{
				return null;
			}
			return product.StoreFront.ProductCatalogFilePath(applicationPath, routeData, server, product.SampleImageFileName);
		}

		public static string ProductCatalogFileUrl(this StoreFront storeFront, string applicationPath, RouteData routeData, string fileName)
		{
			if (string.IsNullOrEmpty(applicationPath))
			{
				throw new ArgumentNullException("applicationPath");
			}
			if (string.IsNullOrWhiteSpace(fileName))
			{
				return null;
			}
			applicationPath = applicationPath.Trim('/');
			if (!string.IsNullOrEmpty(applicationPath))
			{
				applicationPath += "/";
			}
			if (routeData != null)
			{
				string storeName = routeData.UrlStoreName();
				if (!string.IsNullOrEmpty(storeName))
				{
					applicationPath += "Stores/" + HttpUtility.UrlEncode(storeName) + "/";
				}
			}
			return "/" + applicationPath + "CatalogContent/Products/" + fileName;
		}

		public static string ProductCatalogFilePath(this StoreFront storeFront, string applicationPath, RouteData routeData, HttpServerUtilityBase server, string fileName)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}

			string path = "/CatalogContent/Products/" + fileName;
			return storeFront.ChooseFilePath(storeFront.Client, path, applicationPath, server);
		}

		public static string ProductDigitalDownloadFilePath(this StoreFront storeFront, string applicationPath, RouteData routeData, HttpServerUtilityBase server, string fileName)
		{
			if (routeData == null)
			{
				throw new ArgumentNullException("routeData");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}

			string path = "/DigitalDownload/Products/" + fileName;
			return storeFront.ChooseFilePath(storeFront.Client, path, applicationPath, server);
		}

		public static string ClientVirtualDirectoryToMap(this Models.BaseClasses.ClientRecord clientRecord, string applicationPath)
		{
			return clientRecord.Client.ClientVirtualDirectoryToMap(applicationPath);
		}

		public static string ClientVirtualDirectoryToMapToStoreFronts(this Models.BaseClasses.ClientRecord clientRecord, string applicationPath)
		{
			return clientRecord.Client.ClientVirtualDirectoryToMap(applicationPath) + "/StoreFronts";
		}

		public static string StoreFrontVirtualDirectoryToMap(this StoreFrontRecord record, string applicationPath)
		{
			return record.ClientVirtualDirectoryToMapToStoreFronts(applicationPath) + "/" + record.StoreFront.CurrentConfig().Folder.ToFileName();
		}

		public static string StoreFrontVirtualDirectoryToMapAnyConfig(this StoreFrontRecord record, string applicationPath)
		{
			if (record.StoreFront.CurrentConfigOrAny() == null)
			{
				throw new ArgumentNullException("record.CurrentConfigOrAny()");
			}
			return record.ClientVirtualDirectoryToMapToStoreFronts(applicationPath) + "/" + record.StoreFront.CurrentConfigOrAny().Folder.ToFileName();
		}

		public static string StoreFrontVirtualDirectoryToMapThisConfig(this StoreFrontConfiguration storeFrontConfig, string applicationPath)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}
			return storeFrontConfig.ClientVirtualDirectoryToMapToStoreFronts(applicationPath) + "/" + storeFrontConfig.Folder.ToFileName();
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

		public static string EmailConfirmationCodeMessageText(this StoreFront storeFront, string callbackUrl, Uri currentUrl)
		{
			return "Thank you for registering at " + currentUrl.Authority + "!"
				+ "\n\nPlease click this link to confirm your email address"
				+ "\n" + callbackUrl
				+ "\n\n" + (storeFront == null ? string.Empty : storeFront.OutgoingMessageSignature())
				+ "\n" + currentUrl.Authority
				+ "\n\n" + Settings.IdentitySendGridMailFromName + " - " + Settings.IdentitySendGridMailFromEmail;

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

		public static string ForgotPasswordMessageText(this StoreFront storeFront, string callbackUrl, Uri currentUrl)
		{
			return "Forgot your password??"
				+ "\n\nNo worries!"
				+ "\n\nClick here to reset your password"
				+ "\n" + callbackUrl
				+ "\n\n"
				+ (storeFront == null ? string.Empty : storeFront.OutgoingMessageSignature())
				+ "\n" + HttpUtility.HtmlEncode(currentUrl.Authority)
				+ "\n\n" + Settings.IdentitySendGridMailFromName + " - " + Settings.IdentitySendGridMailFromEmail;
		}

		/// <summary>
		/// Note: Handles null storefront with system default message
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="code"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
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

		public static PageTemplateSection CreatePageTemplateSection(this IGstoreDb db, int pageTemplateId, string sectionName, int order, string description, string defaultRawHtmlValue, string preTextHtml, string postTextHtml, string defaultTextCssClass, bool editInTop, bool editInBottom, int clientId, UserProfile userProfile)
		{
			db.UserName = userProfile.UserName;
			PageTemplateSection newSection = db.PageTemplateSections.Create();
			newSection.PageTemplateId = pageTemplateId;
			newSection.ClientId = clientId;
			newSection.Name = sectionName;
			newSection.Order = order;
			newSection.DefaultRawHtmlValue = defaultRawHtmlValue;
			newSection.PreTextHtml = preTextHtml;
			newSection.PostTextHtml = postTextHtml;
			newSection.DefaultTextCssClass = defaultTextCssClass;
			newSection.Description = description;
			newSection.IsPending = false;
			newSection.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			newSection.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			db.PageTemplateSections.Add(newSection);
			db.SaveChanges();
			return newSection;
		}


		public static PageSection CreatePageSection(this IGstoreDb db, ViewModels.PageSectionEditViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			if (!storeFront.Pages.Any(pg => pg.PageId == viewModel.PageId))
			{
				throw new ApplicationException("ID Map error. Page Id: " + viewModel.PageId + " not found in storefront pages. Make sure this storefront has a page with the passed PageId or fix the PageId parameter in model.");
			}

			if (!storeFront.Pages.Single(pg => pg.PageId == viewModel.PageId)
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

		public static bool ValidatePageUrl(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string url, int storeFrontId, int clientId, int? currentPageId)
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
			string[] blockedUrls = { "Account", "Bundles", "Category", "Catalog", "CatalogAdmin", "CatalogContent", "Cart", "Checkout", "Content", "Edit", "Fonts", "GStore", "Images", "JS", "Notifications", "Order", "OrderAdmin", "Pages", "Products", "Profile", "Styles", "Scripts", "StoreAdmin", "SubmitForm", "SystemAdmin", "Themes", "UpdatePageAjax", "UpdateSectionAjax", "View" };

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

		public static bool ValidateNavBarItemName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string name, int storeFrontId, int clientId, int? currentNavBarItemId)
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

		public static bool ValidateProductUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int? currentProductId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this product";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			Product conflict = db.Products.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.UrlName.ToLower() == urlName && (p.ProductId != currentProductId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Product '" + conflict.Name + "' [" + conflict.ProductId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Product URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateProductCategoryUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int? currentProductCategoryId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this category";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			ProductCategory conflict = db.ProductCategories.Where(pc => pc.ClientId == clientId && pc.StoreFrontId == storeFrontId && pc.UrlName.ToLower() == urlName && (pc.ProductCategoryId != currentProductCategoryId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Category '" + conflict.Name + "' [" + conflict.ProductCategoryId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Category URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);

			return false;

		}

		public static bool ValidateProductBundleUrlName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string urlName, int storeFrontId, int clientId, int? currentProductBundleId)
		{
			string nameField = "UrlName";

			if (string.IsNullOrWhiteSpace(urlName))
			{
				string errorMessage = "URL Name is required \n Please enter a unique URL name for this product Bundle";
				controller.ModelState.AddModelError(nameField, errorMessage);
				return false;
			}

			ProductBundle conflict = db.ProductBundles.Where(p => p.ClientId == clientId && p.StoreFrontId == storeFrontId && p.UrlName.ToLower() == urlName && (p.ProductBundleId != currentProductBundleId)).FirstOrDefault();

			if (conflict == null)
			{
				return true;
			}

			string errorConflictMessage = "URL Name '" + urlName + "' is already in use for Product Bundle '" + conflict.Name + "' [" + conflict.ProductBundleId + "] in Store Front '" + conflict.StoreFront.CurrentConfig().Name.ToHtml() + "' [" + conflict.StoreFrontId + "]. \n You must enter a unique URL Name or change the conflicting Product Bundle URL Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static bool ValidateValueListName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string name, int clientId, int? currentValueListId)
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

		public static bool ValidateValueListFastAddItemName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string name, ValueList valueList, int? currentValueListItemId)
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

		public static bool ValidateWebFormName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string name, int clientId, int? currentWebFormId)
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

		public static bool ValidateWebFormFieldName(this IGstoreDb db, GStoreData.ControllerBase.BaseController controller, string name, int clientId, int webFormId, int? currentWebFormFieldId)
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

			string errorConflictMessage = "Name '" + name + "' is already in use for field '" + conflict.Name + "' [" + conflict.WebFormFieldId + "] in Web Form '" + conflict.WebForm.Name + "' [" + conflict.WebFormId + "] for Client '" + conflict.Client.Name + "' [" + conflict.ClientId + "]. \n You must enter a unique Field Name or change the conflicting Field Name.";

			controller.ModelState.AddModelError(nameField, errorConflictMessage);
			return false;

		}

		public static Page CreatePage(this IGstoreDb db, ViewModels.PageEditViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
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

		public static Page UpdatePage(this IGstoreDb db, ViewModels.PageEditViewModel viewModel, GStoreData.ControllerBase.BaseController controller, StoreFront storeFront, UserProfile userProfile)
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

		public static PageSection UpdatePageSection(this IGstoreDb db, ViewModels.PageSectionEditViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
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

		public static Product CreateProduct(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			Product record = db.Products.Create();

			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.ImageName = viewModel.ImageName;
			record.UrlName = viewModel.UrlName;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.DigitalDownload = viewModel.DigitalDownload;
			record.MaxQuantityPerOrder = viewModel.MaxQuantityPerOrder;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;
			record.ProductCategoryId = viewModel.ProductCategoryId;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.AvailableForPurchase = viewModel.AvailableForPurchase;
			record.RequestAQuote_Show = viewModel.RequestAQuote_Show;
			record.RequestAQuote_Label = viewModel.RequestAQuote_Label;
			record.RequestAQuote_PageId = viewModel.RequestAQuote_PageId;
			record.BaseListPrice = viewModel.BaseListPrice;
			record.BaseUnitPrice = viewModel.BaseUnitPrice;
			record.ThemeId = viewModel.ThemeId;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;

			record.SummaryCaption = viewModel.SummaryCaption;
			record.SummaryHtml = viewModel.SummaryHtml;
			record.TopDescriptionCaption = viewModel.TopDescriptionCaption;
			record.TopDescriptionHtml = viewModel.TopDescriptionHtml;
			record.TopLinkHref = viewModel.TopLinkHref;
			record.TopLinkLabel = viewModel.TopLinkLabel;
			record.BottomDescriptionCaption = viewModel.BottomDescriptionCaption;
			record.BottomDescriptionHtml = viewModel.BottomDescriptionHtml;
			record.BottomLinkHref = viewModel.BottomLinkHref;
			record.BottomLinkLabel = viewModel.BottomLinkLabel;
			record.FooterHtml = viewModel.FooterHtml;

			record.DigitalDownloadFileName = viewModel.DigitalDownloadFileName;
			record.SampleAudioCaption = viewModel.SampleAudioCaption;
			record.SampleAudioFileName = viewModel.SampleAudioFileName;
			record.SampleDownloadCaption = viewModel.SampleDownloadCaption;
			record.SampleDownloadFileName = viewModel.SampleDownloadFileName;
			record.SampleImageCaption = viewModel.SampleImageCaption;
			record.SampleImageFileName = viewModel.SampleImageFileName;

			record.UpdateAuditFields(userProfile);

			db.Products.Add(record);
			db.SaveChanges();

			return record;

		}

		public static Product UpdateProduct(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			//find existing record, update it
			Product record = storeFront.Products.SingleOrDefault(p => p.ProductId == viewModel.ProductId);
			if (record == null)
			{
				throw new ApplicationException("Product not found in storefront Products. Product Id: " + viewModel.ProductId);
			}

			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.ImageName = viewModel.ImageName;
			record.UrlName = viewModel.UrlName;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.DigitalDownload = viewModel.DigitalDownload;
			record.MaxQuantityPerOrder = viewModel.MaxQuantityPerOrder;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;
			record.ProductCategoryId = viewModel.ProductCategoryId;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.UpdatedBy = userProfile;
			record.UpdateDateTimeUtc = DateTime.UtcNow;

			record.AvailableForPurchase = viewModel.AvailableForPurchase;
			record.RequestAQuote_Show = viewModel.RequestAQuote_Show;
			record.RequestAQuote_Label = viewModel.RequestAQuote_Label;
			record.RequestAQuote_PageId = viewModel.RequestAQuote_PageId;
			record.BaseListPrice = viewModel.BaseListPrice;
			record.BaseUnitPrice = viewModel.BaseUnitPrice;
			record.ThemeId = viewModel.ThemeId;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;

			record.SummaryCaption = viewModel.SummaryCaption;
			record.SummaryHtml = viewModel.SummaryHtml;
			record.TopDescriptionCaption = viewModel.TopDescriptionCaption;
			record.TopDescriptionHtml = viewModel.TopDescriptionHtml;
			record.TopLinkHref = viewModel.TopLinkHref;
			record.TopLinkLabel = viewModel.TopLinkLabel;
			record.BottomDescriptionCaption = viewModel.BottomDescriptionCaption;
			record.BottomDescriptionHtml = viewModel.BottomDescriptionHtml;
			record.BottomLinkHref = viewModel.BottomLinkHref;
			record.BottomLinkLabel = viewModel.BottomLinkLabel;

			record.FooterHtml = viewModel.FooterHtml;
			record.DigitalDownloadFileName = viewModel.DigitalDownloadFileName;
			record.SampleAudioCaption = viewModel.SampleAudioCaption;
			record.SampleAudioFileName = viewModel.SampleAudioFileName;
			record.SampleDownloadCaption = viewModel.SampleDownloadCaption;
			record.SampleDownloadFileName = viewModel.SampleDownloadFileName;
			record.SampleImageCaption = viewModel.SampleImageCaption;
			record.SampleImageFileName = viewModel.SampleImageFileName;

			db.Products.Update(record);
			db.SaveChanges();

			return record;

		}

		public static ProductCategory CreateProductCategory(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.CategoryEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			ProductCategory record = db.ProductCategories.Create();

			record.AllowChildCategoriesInMenu = viewModel.AllowChildCategoriesInMenu;
			record.ChildActiveCountForAnonymous = 0;
			record.ChildActiveCountForRegistered = 0;
			record.DirectActiveCountForAnonymous = 0;
			record.DirectActiveCountForRegistered = 0;

			record.ImageName = viewModel.ImageName;
			record.ParentCategoryId = viewModel.ParentCategoryId;
			record.ProductTypeSingle = viewModel.ProductTypeSingle;
			record.ProductTypePlural = viewModel.ProductTypePlural;
			record.BundleTypeSingle = viewModel.BundleTypeSingle;
			record.BundleTypePlural = viewModel.BundleTypePlural;

			record.HideInMenuIfEmpty = viewModel.HideInMenuIfEmpty;
			record.ShowInMenu = viewModel.ShowInMenu;
			record.DisplayForDirectLinks = viewModel.DisplayForDirectLinks;
			record.ShowInCatalogIfEmpty = viewModel.ShowInCatalogIfEmpty;
			record.UrlName = viewModel.UrlName;

			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.UseDividerAfterOnMenu = viewModel.UseDividerAfterOnMenu;
			record.UseDividerBeforeOnMenu = viewModel.UseDividerBeforeOnMenu;
			record.ThemeId = viewModel.ThemeId;
			record.CategoryDetailTemplate = viewModel.CategoryDetailTemplate;
			record.ProductListTemplate = viewModel.ProductListTemplate;
			record.BundleListTemplate = viewModel.BundleListTemplate;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;
			record.ChildCategoryHeaderHtml = viewModel.ChildCategoryHeaderHtml;
			record.ChildCategoryFooterHtml = viewModel.ChildCategoryFooterHtml;
			record.ProductHeaderHtml = viewModel.ProductHeaderHtml;
			record.ProductFooterHtml = viewModel.ProductFooterHtml;
			record.NoProductsMessageHtml = viewModel.NoProductsMessageHtml;

			record.DefaultSummaryCaption = viewModel.DefaultSummaryCaption;
			record.DefaultTopDescriptionCaption = viewModel.DefaultTopDescriptionCaption;
			record.DefaultBottomDescriptionCaption = viewModel.DefaultBottomDescriptionCaption;
			record.DefaultSampleImageCaption = viewModel.DefaultSampleImageCaption;
			record.DefaultSampleAudioCaption = viewModel.DefaultSampleAudioCaption;
			record.DefaultSampleDownloadCaption = viewModel.DefaultSampleDownloadCaption;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;

			record.UpdateAuditFields(userProfile);


			db.ProductCategories.Add(record);
			db.SaveChanges();

			return record;

		}

		public static ProductCategory UpdateProductCategory(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.CategoryEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			//find existing record, update it
			ProductCategory record = storeFront.ProductCategories.SingleOrDefault(pc => pc.ProductCategoryId == viewModel.ProductCategoryId);
			if (record == null)
			{
				throw new ApplicationException("Category not found in storefront Categories. Category Id: " + viewModel.ProductCategoryId);
			}

			record.AllowChildCategoriesInMenu = viewModel.AllowChildCategoriesInMenu;
			record.ImageName = viewModel.ImageName;
			record.ParentCategoryId = viewModel.ParentCategoryId;
			record.ProductTypeSingle = viewModel.ProductTypeSingle;
			record.ProductTypePlural = viewModel.ProductTypePlural;
			record.BundleTypeSingle = viewModel.BundleTypeSingle;
			record.BundleTypePlural = viewModel.BundleTypePlural;

			record.HideInMenuIfEmpty = viewModel.HideInMenuIfEmpty;
			record.ShowInMenu = viewModel.ShowInMenu;
			record.ShowInCatalogIfEmpty = viewModel.ShowInCatalogIfEmpty;
			record.DisplayForDirectLinks = viewModel.DisplayForDirectLinks;
			record.UrlName = viewModel.UrlName;

			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.UseDividerAfterOnMenu = viewModel.UseDividerAfterOnMenu;
			record.UseDividerBeforeOnMenu = viewModel.UseDividerBeforeOnMenu;
			record.ThemeId = viewModel.ThemeId;
			record.CategoryDetailTemplate = viewModel.CategoryDetailTemplate;
			record.ProductListTemplate = viewModel.ProductListTemplate;
			record.BundleListTemplate = viewModel.BundleListTemplate;
			record.ProductDetailTemplate = viewModel.ProductDetailTemplate;
			record.ChildCategoryHeaderHtml = viewModel.ChildCategoryHeaderHtml;
			record.ChildCategoryFooterHtml = viewModel.ChildCategoryFooterHtml;
			record.ProductHeaderHtml = viewModel.ProductHeaderHtml;
			record.ProductFooterHtml = viewModel.ProductFooterHtml;
			record.NoProductsMessageHtml = viewModel.NoProductsMessageHtml;

			record.DefaultSummaryCaption = viewModel.DefaultSummaryCaption;
			record.DefaultTopDescriptionCaption = viewModel.DefaultTopDescriptionCaption;
			record.DefaultBottomDescriptionCaption = viewModel.DefaultBottomDescriptionCaption;
			record.DefaultSampleImageCaption = viewModel.DefaultSampleImageCaption;
			record.DefaultSampleAudioCaption = viewModel.DefaultSampleAudioCaption;
			record.DefaultSampleDownloadCaption = viewModel.DefaultSampleDownloadCaption;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;

			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			record.UpdatedBy = userProfile;
			record.UpdateDateTimeUtc = DateTime.UtcNow;

			record = db.ProductCategories.Update(record);
			db.SaveChanges();

			return record;

		}

		public static ProductBundle CreateProductBundle(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductBundleEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			ProductBundle record = db.ProductBundles.Create();

			record.StoreFrontId = storeFront.StoreFrontId;
			record.ClientId = storeFront.ClientId;
			record.UpdateAuditFields(userProfile);
			record = record.MapFromViewModel(viewModel);

			record = db.ProductBundles.Add(record);
			db.SaveChanges();

			return record;
		}

		public static ProductBundle UpdateProductBundle(this IGstoreDb db, Areas.CatalogAdmin.ViewModels.ProductBundleEditAdminViewModel viewModel, StoreFront storeFront, UserProfile userProfile)
		{
			ProductBundle record = storeFront.ProductBundles.SingleOrDefault(pb => pb.ProductBundleId == viewModel.ProductBundleId);
			if (record == null)
			{
				throw new ApplicationException("Product Bundle not found in storefront Product Bundles. Product Bundle Id: " + viewModel.ProductBundleId);
			}

			record.UpdateAuditFields(userProfile);
			record = record.MapFromViewModel(viewModel);

			record = db.ProductBundles.Update(record);
			db.SaveChanges();

			return record;
		}

		public static int UpdateProductBundleItems(this IGstoreDb db, ProductBundle productBundle, List<Areas.CatalogAdmin.ViewModels.ProductBundleItemEditAdminViewModel> bundleItems, StoreFront storeFront, UserProfile userProfile)
		{
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}
			if (bundleItems == null)
			{
				throw new ArgumentNullException("bundleItems");
			}

			if (bundleItems.Count == 0)
			{
				return 0;
			}

			int counter = 0;
			foreach (Areas.CatalogAdmin.ViewModels.ProductBundleItemEditAdminViewModel itemModel in bundleItems)
			{
				ProductBundleItem item = productBundle.ProductBundleItems.SingleOrDefault(bi => bi.ProductBundleItemId == itemModel.ProductBundleItemId);
				if (item == null)
				{
					throw new ApplicationException("Product Bundle Item Id: " + itemModel.ProductBundleItemId + " not found in Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "]");
				}

				item.Order = itemModel.Order;
				item.ProductVariantInfo = itemModel.ProductVariantInfo;
				item.Quantity = itemModel.Quantity;
				item.BaseUnitPrice = itemModel.BaseUnitPrice;
				item.BaseListPrice = itemModel.BaseListPrice;
				item.IsPending = itemModel.IsPending;
				item.StartDateTimeUtc = itemModel.StartDateTimeUtc;
				item.EndDateTimeUtc = itemModel.EndDateTimeUtc;
				item = db.ProductBundleItems.Update(item);
				counter++;
			}

			db.SaveChanges();

			return counter;
		}

		private static ProductBundle MapFromViewModel(this ProductBundle record, GStoreData.Areas.CatalogAdmin.ViewModels.ProductBundleEditAdminViewModel viewModel)
		{
			record.Name = viewModel.Name;
			record.Order = viewModel.Order;
			record.ImageName = viewModel.ImageName;
			record.UrlName = viewModel.UrlName;
			record.ForAnonymousOnly = viewModel.ForAnonymousOnly;
			record.ForRegisteredOnly = viewModel.ForRegisteredOnly;
			record.MaxQuantityPerOrder = viewModel.MaxQuantityPerOrder;
			record.MetaDescription = viewModel.MetaDescription;
			record.MetaKeywords = viewModel.MetaKeywords;
			record.ProductCategoryId = viewModel.ProductCategoryId;
			record.IsPending = viewModel.IsPending;
			record.StartDateTimeUtc = viewModel.StartDateTimeUtc;
			record.EndDateTimeUtc = viewModel.EndDateTimeUtc;
			record.AvailableForPurchase = viewModel.AvailableForPurchase;
			record.RequestAQuote_Show = viewModel.RequestAQuote_Show;
			record.RequestAQuote_Label = viewModel.RequestAQuote_Label;
			record.RequestAQuote_PageId = viewModel.RequestAQuote_PageId;
			record.ThemeId = viewModel.ThemeId;
			record.ProductBundleDetailTemplate = viewModel.ProductBundleDetailTemplate;
			record.SummaryCaption = viewModel.SummaryCaption;
			record.SummaryHtml = viewModel.SummaryHtml;
			record.TopDescriptionCaption = viewModel.TopDescriptionCaption;
			record.TopDescriptionHtml = viewModel.TopDescriptionHtml;
			record.TopLinkHref = viewModel.TopLinkHref;
			record.TopLinkLabel = viewModel.TopLinkLabel;
			record.BottomDescriptionCaption = viewModel.BottomDescriptionCaption;
			record.BottomDescriptionHtml = viewModel.BottomDescriptionHtml;
			record.BottomLinkHref = viewModel.BottomLinkHref;
			record.BottomLinkLabel = viewModel.BottomLinkLabel;
			record.FooterHtml = viewModel.FooterHtml;
			record.ProductTypeSingle = viewModel.ProductTypeSingle;
			record.ProductTypePlural = viewModel.ProductTypePlural;

			return record;
		}

		/// <summary>
		/// re-orders siblings and puts them in order by 10's, and saves to database
		/// </summary>
		/// <param name="navBarItems"></param>
		public static void ProductCategoriesRenumberSiblings(this IGstoreDb db, IEnumerable<ProductCategory> productCategories)
		{
			List<ProductCategory> sortedItems = productCategories.AsQueryable().ApplyDefaultSort().ToList();

			int order = 100;
			foreach (ProductCategory item in sortedItems)
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
				throw new ApplicationException("Value List Item not found in existing values. Value List Id: " + viewModel.ValueListId + " Value List Item Id: " + viewModel.ValueListItemId + " Client '" + client.Name + " [" + client.ClientId + "]");
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


		/// <summary>
		/// Creates a Product Bundle item with basic settings for a fast add admin command
		/// This overload uses a Product entity and a ProductBundleId int value
		/// </summary>
		/// <param name="db"></param>
		/// <param name="productBundleId"></param>
		/// <param name="product"></param>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public static ProductBundleItem CreateProductBundleItemFastAdd(this IGstoreDb db, int productBundleId, Product product, StoreFront storeFront, UserProfile userProfile, int quantity = 1)
		{
			if (productBundleId == 0)
			{
				throw new ArgumentNullException("productBundleId cannot be 0");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}

			ProductBundle productBundle = storeFront.ProductBundles.Where(pb => pb.ProductBundleId == productBundleId).SingleOrDefault();
			if (productBundle == null)
			{
				throw new ApplicationException("Product Bundle not found by id: " + productBundleId);
			}

			return db.CreateProductBundleItemFastAdd(productBundle, product, storeFront, userProfile, quantity);
		}

		/// <summary>
		/// Creates a Product Bundle item with basic settings for a fast add admin command
		/// This overload uses a ProductBundle entity and a ProductId int value
		/// </summary>
		/// <param name="db"></param>
		/// <param name="bundle"></param>
		/// <param name="productId"></param>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public static ProductBundleItem CreateProductBundleItemFastAdd(this IGstoreDb db, ProductBundle bundle, int productId, StoreFront storeFront, UserProfile userProfile, int quantity = 1)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (productId == 0)
			{
				throw new ApplicationException("ProductId cannot be zero");
			}

			Product product = storeFront.Products.Where(p => p.ProductId == productId).SingleOrDefault();
			if (product == null)
			{
				throw new ApplicationException("Product not found by id: " + productId);
			}

			return db.CreateProductBundleItemFastAdd(bundle, product, storeFront, userProfile, quantity);

		}

		/// <summary>
		/// Creates a Product Bundle item with basic settings for a fast add admin command
		/// This is the prefered overload
		/// </summary>
		/// <param name="db"></param>
		/// <param name="bundle"></param>
		/// <param name="product"></param>
		/// <param name="storeFront"></param>
		/// <param name="userProfile"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public static ProductBundleItem CreateProductBundleItemFastAdd(this IGstoreDb db, ProductBundle bundle, Product product, StoreFront storeFront, UserProfile userProfile, int quantity = 1)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}

			ProductBundleItem bundleItem = db.ProductBundleItems.Create();
			bundleItem.SetDefaultsForNew(bundle);

			bundleItem.Quantity = quantity;
			bundleItem.ProductBundle = bundle;
			bundleItem.ProductBundleId = bundle.ProductBundleId;

			bundleItem.BaseListPrice = product.BaseListPrice;
			bundleItem.BaseUnitPrice = product.BaseUnitPrice;
			bundleItem.Product = product;
			bundleItem.ProductId = product.ProductId;
			bundleItem.ProductVariantInfo = "";

			bundleItem = db.ProductBundleItems.Add(bundleItem);
			db.SaveChanges();

			return bundleItem;

		}

		public static void SetDefaultsForNew(this ProductBundleItem item, ProductBundle bundle)
		{
			item.Client = bundle.Client;
			item.ClientId = bundle.ClientId;
			item.StoreFront = bundle.StoreFront;
			item.StoreFrontId = bundle.StoreFrontId;
			item.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			item.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			item.IsPending = false;
			item.Quantity = 1;
			item.ProductVariantInfo = "";
			item.ProductBundle = bundle;
			item.Order = bundle.ProductBundleItems.Count == 0 ? 100 : bundle.ProductBundleItems.Max(pbi => pbi.Order) + 10;
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

			return routeData.Values["urlstorename"].ToString().ToLower();
		}

		public static void ApplyDefaultCartConfig(this StoreFrontConfiguration storeFrontConfig)
		{
			storeFrontConfig.UseShoppingCart = true;
			storeFrontConfig.CartNavShowCartToAnonymous = true;
			storeFrontConfig.CartNavShowCartToRegistered = true;
			storeFrontConfig.CartNavShowCartWhenEmpty = true;
			storeFrontConfig.CartRequireLogin = false;

			if (storeFrontConfig.DefaultNewPageTheme != null)
			{
				storeFrontConfig.CartTheme = storeFrontConfig.DefaultNewPageTheme;
				storeFrontConfig.CartThemeId = storeFrontConfig.DefaultNewPageTheme.ThemeId;
			}
			storeFrontConfig.CartPageTitle = "Shopping Cart";
			storeFrontConfig.CartPageHeading = "Your Shopping Cart";
			storeFrontConfig.CartEmptyMessage = "Your Cart is Empty";
			storeFrontConfig.CartCheckoutButtonLabel = "Check Out";
			storeFrontConfig.CartItemColumnLabel = "Item";
			storeFrontConfig.CartItemVariantColumnShow = true;
			storeFrontConfig.CartItemVariantColumnLabel = "Type";
			storeFrontConfig.CartItemListPriceColumnShow = true;
			storeFrontConfig.CartItemListPriceColumnLabel = "List Price Each";
			storeFrontConfig.CartItemUnitPriceColumnShow = true;
			storeFrontConfig.CartItemUnitPriceColumnLabel = "Your Price Each";
			storeFrontConfig.CartItemQuantityColumnShow = true;
			storeFrontConfig.CartItemQuantityColumnLabel = "Quantity";
			storeFrontConfig.CartItemListPriceExtColumnShow = true;
			storeFrontConfig.CartItemListPriceExtColumnLabel = "List Price Ext";
			storeFrontConfig.CartItemUnitPriceExtColumnShow = true;
			storeFrontConfig.CartItemUnitPriceExtColumnLabel = "Your Price Ext";
			storeFrontConfig.CartItemDiscountColumnShow = true;
			storeFrontConfig.CartItemDiscountColumnLabel = "Your Savings";
			storeFrontConfig.CartItemTotalColumnShow = true;
			storeFrontConfig.CartItemTotalColumnLabel = "Line Total";
			storeFrontConfig.CartBundleShowIncludedItems = true;
			storeFrontConfig.CartBundleShowPriceOfIncludedItems = true;
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

		public static void ApplyDefaultPaymentMethodConfig(this StoreFrontConfiguration storeFrontConfig)
		{
			storeFrontConfig.PaymentMethod_PayPal_Client_Id = null;
			storeFrontConfig.PaymentMethod_PayPal_Client_Secret = null;
			storeFrontConfig.PaymentMethod_PayPal_Enabled = false;
			storeFrontConfig.PaymentMethod_PayPal_UseLiveServer = false;
			storeFrontConfig.Orders_AutoAcceptPaid = true;
		}

		public static void ApplyDefaultOrdersConfig(this StoreFrontConfiguration storeFrontConfig)
		{
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

		public static int ResetPagesToThemeId(this StoreFront storeFront, int themeId, IGstoreDb db)
		{
			int count = 0;
			IEnumerable<Page> pages = db.Pages.Where(p => p.StoreFrontId == storeFront.StoreFrontId && p.ThemeId != themeId);
			foreach (Page page in pages)
			{
				if (page.ThemeId != themeId)
				{
					page.ThemeId = themeId;
					db.Pages.Update(page);
				}
				count++;
			}
			if (count != 0)
			{
				db.SaveChanges();
			}

			return count;
		}

		/// <summary>
		/// Returns a queryable filter to check ForanonymousOnly and ForRegisteredOnly against the current user
		/// </summary>
		/// <param name="query"></param>
		/// <param name="isRegistered"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereRegisteredAnonymousCheck(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			return query.Where(pc =>
					(isRegistered || !pc.ForRegisteredOnly)
					&&
					(!isRegistered || !pc.ForAnonymousOnly));
		}

		/// <summary>
		/// Returns a queryable filter for whether to show this category in the navbar menu
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereShowInNavBarMenu(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			if (isRegistered)
			{
				return query.Where(cat => cat.ShowInMenu && (!cat.HideInMenuIfEmpty || cat.ChildActiveCountForRegistered > 0));
			}
			else
			{
				return query.Where(cat => cat.ShowInMenu && (!cat.HideInMenuIfEmpty || cat.ChildActiveCountForAnonymous > 0));
			}
		}

		/// <summary>
		/// Returns a queryable filter whether this category should be navigable in the catalog
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereShowInCatalogList(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			if (isRegistered)
			{
				return query.Where(cat =>
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForRegistered != 0)
					);
			}
			else
			{
				return query.Where(cat =>
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForAnonymous != 0)
					);
			}
		}

		/// <summary>
		/// Returns a queryable filter whether this category should show when targetted by name in category details
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereShowInCatalogByName(this IQueryable<ProductCategory> query, bool isRegistered)
		{
			if (isRegistered)
			{
				return query.Where(cat =>
					cat.DisplayForDirectLinks
					||
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForRegistered != 0)
					);
			}
			else
			{
				return query.Where(cat =>
					cat.DisplayForDirectLinks
					||
					(cat.ShowInCatalogIfEmpty)
					||
					(cat.ShowInMenu && (!cat.HideInMenuIfEmpty))
					||
					(cat.ChildActiveCountForAnonymous != 0)
					);
			}
		}

		/// <summary>
		/// Returns a queryable filter to check ForanonymousOnly and ForRegisteredOnly against the current user
		/// </summary>
		/// <param name="query"></param>
		/// <param name="isRegistered"></param>
		/// <returns></returns>
		public static IQueryable<ProductBundle> WhereRegisteredAnonymousCheck(this IQueryable<ProductBundle> query, bool isRegistered)
		{
			return query.Where(pc =>
					(isRegistered || !pc.ForRegisteredOnly)
					&&
					(!isRegistered || !pc.ForAnonymousOnly));
		}

		/// <summary>
		/// Checks for a file in storefront, client, or storefront folders. Returns path to first file found or Null if file does not exist in any location
		/// Example: for a path of /Images/File1.png   this function will check for [storefrontfolder]/Images/File1.png, if the file is found it will return the physical path
		/// if the file is not found, it will check [client folder]/Images/File1.png. if file is found in client folder, it will return that physical path
		/// If the file is not found in the client folder, it will check the server folder [/Content/Server]/Images/File1.png. If file is found that path will be returned.
		/// If file is not found in any of these locations, null is returned
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="path">Path to file, can start with / or not, / is implied</param>
		/// <param name="applicationPath"></param>
		/// <param name="client">Client to search if file is not found in the storefront, or storefront is null</param>
		/// <param name="server">HTTP Server</param>
		/// <returns></returns>
		public static string ChooseFilePath(this StoreFront storeFront, Client client, string path, string applicationPath, HttpServerUtilityBase server)
		{
			string fullVirtualPath = null;
			string fullPath = null;

			if (storeFront != null && storeFront.IsActiveBubble())
			{
				fullVirtualPath = storeFront.StoreFrontVirtualDirectoryToMap(applicationPath) + "/" + path.TrimStart('/');
				fullPath = server.MapPath(fullVirtualPath);
				if (System.IO.File.Exists(fullPath))
				{
					return fullPath;
				}
			}

			if (client != null && client.IsActiveDirect())
			{
				fullVirtualPath = client.ClientVirtualDirectoryToMap(applicationPath) + "/" + path.TrimStart('/');
				fullPath = server.MapPath(fullVirtualPath);
				if (System.IO.File.Exists(fullPath))
				{
					return fullPath;
				}
			}

			fullVirtualPath = "~/Content/Server/" + path.TrimStart('/');
			fullPath = server.MapPath(fullVirtualPath);
			if (!System.IO.File.Exists(fullPath))
			{
				return null;
			}
			return fullPath;

		}

		/// <summary>
		/// Checks for a file in storefront, client, or storefront folders. Returns path to first file found or Null if file does not exist in any location
		/// Example: for a virtua;l folder '/Images' and fileNameStart of 'File1'   this function will check for [storefrontfolder]/Images/File1.*, if the file is found it will return the physical path
		/// if the file is not found, it will check [client folder]/Images/File1.*. if file is found in client folder, it will return that physical path
		/// If the file is not found in the client folder, it will check the server folder [/Content/Server]/Images/File1.*. If file is found that path will be returned.
		/// If file is not found in any of these locations, null is returned
		/// </summary>
		/// <param name="storeFront"></param>
		/// <param name="path">Path to file, can start with / or not, / is implied</param>
		/// <param name="applicationPath"></param>
		/// <param name="client">Client to search if file is not found in the storefront, or storefront is null</param>
		/// <param name="server">HTTP Server</param>
		/// <returns></returns>
		public static string ChooseFileNameWildcard(this StoreFront storeFront, Client client, string virtualFolder, string fileNameStart, string applicationPath, HttpServerUtilityBase server)
		{
			string virtualPath = null;
			string folderPath = null;

			if (storeFront != null && storeFront.IsActiveBubble())
			{
				virtualPath = storeFront.StoreFrontVirtualDirectoryToMap(applicationPath) + "/" + virtualFolder.Trim('/');
				folderPath = server.MapPath(virtualPath);
				if (System.IO.Directory.Exists(folderPath))
				{
					IOrderedEnumerable<string> files = System.IO.Directory.GetFiles(folderPath, fileNameStart + ".*")
						.OrderByDescending(s => s.EndsWith(".png"))
						.ThenByDescending(s => s.EndsWith(".jpg"))
						.ThenByDescending(s => s.EndsWith(".jpeg"))
						.ThenByDescending(s => s.EndsWith(".gif"))
						.ThenByDescending(s => s.EndsWith(".pdf"))
						.ThenByDescending(s => s.EndsWith(".doc"))
						.ThenByDescending(s => s.EndsWith(".xls"))
						.ThenByDescending(s => s.EndsWith(".mp3"))
						.ThenByDescending(s => s.EndsWith(".txt"));

					if (files.Count() != 0)
					{
						return new System.IO.FileInfo(files.First()).Name;
					}
				}
			}

			if (client != null && client.IsActiveDirect())
			{
				virtualPath = client.ClientVirtualDirectoryToMap(applicationPath) + "/" + virtualFolder.Trim('/');
				folderPath = server.MapPath(virtualPath);
				if (System.IO.Directory.Exists(folderPath))
				{
					IOrderedEnumerable<string> files = System.IO.Directory.GetFiles(folderPath, fileNameStart + ".*")
						.OrderByDescending(s => s.EndsWith(".png"))
						.ThenByDescending(s => s.EndsWith(".jpg"))
						.ThenByDescending(s => s.EndsWith(".jpeg"))
						.ThenByDescending(s => s.EndsWith(".gif"))
						.ThenByDescending(s => s.EndsWith(".pdf"))
						.ThenByDescending(s => s.EndsWith(".doc"))
						.ThenByDescending(s => s.EndsWith(".xls"))
						.ThenByDescending(s => s.EndsWith(".mp3"))
						.ThenByDescending(s => s.EndsWith(".txt"));

					if (files.Count() != 0)
					{
						return new System.IO.FileInfo(files.First()).Name;
					}
				}
			}

			virtualPath = "~/Content/Server/" + virtualFolder.Trim('/');
			folderPath = server.MapPath(virtualPath);
			if (System.IO.Directory.Exists(folderPath))
			{
				IOrderedEnumerable<string> serverFiles = System.IO.Directory.GetFiles(folderPath, fileNameStart + ".*")
					.OrderByDescending(s => s.EndsWith(".png"))
					.ThenByDescending(s => s.EndsWith(".jpg"))
					.ThenByDescending(s => s.EndsWith(".jpeg"))
					.ThenByDescending(s => s.EndsWith(".gif"))
					.ThenByDescending(s => s.EndsWith(".pdf"))
					.ThenByDescending(s => s.EndsWith(".doc"))
					.ThenByDescending(s => s.EndsWith(".xls"))
					.ThenByDescending(s => s.EndsWith(".mp3"))
					.ThenByDescending(s => s.EndsWith(".txt"));

				if (serverFiles.Count() != 0)
				{
					return new System.IO.FileInfo(serverFiles.First()).Name;
				}
			}


			return null;
		}

		public static string SyncCatalogFile(this Product product, Expression<Func<Product, String>> expression, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			return product.SyncFileHelper(expression, false, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncDigitalDownloadFile(this Product product, Expression<Func<Product, String>> expression, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			return product.SyncFileHelper(expression, true, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		private static string SyncFileHelper(this Product product, Expression<Func<Product, String>> expression, bool digitalDownload, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			StoreFront storeFront = product.StoreFront;
			StoreFrontConfiguration storeFrontConfig = storeFront.CurrentConfigOrAny();
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("product.StoreFront.CurrentConfigOrAny");
			}

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<Product, String>(expression, new ViewDataDictionary<Product>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName ?? metadata.PropertyName;
			PropertyInfo property = typeof(Product).GetProperty(expressionText);
			string currentFileName = (property.GetValue(product) ?? "").ToString();


			hasDbChanges = false;
			StringBuilder results = new StringBuilder();

			if (!string.IsNullOrEmpty(currentFileName))
			{
				if (eraseFileNameIfNotFound)
				{
					//file name is set, verify file and set to null if not found
					string filePath = null;
					if (digitalDownload)
					{
						filePath = storeFront.ProductDigitalDownloadFilePath(applicationPath, routeData, server, currentFileName);
					}
					else
					{
						filePath = storeFront.ProductCatalogFilePath(applicationPath, routeData, server, currentFileName);
					}
					if (string.IsNullOrEmpty(filePath))
					{
						results.AppendLine("- - - - - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - '" + currentFileName + "' not found, setting to null - - - - -");
						currentFileName = null;
						property.SetValue(product, null);
						hasDbChanges = true;
					}
					else
					{
						if (verbose)
						{
							results.AppendLine("OK - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - '" + currentFileName + "' File Confirmed");
						}
					}
				}
			}

			if (string.IsNullOrEmpty(currentFileName))
			{
				if (searchForFileIfBlank)
				{
					//if file name is not set (or file was not found and set to blank), see if there is a suitable file in the file system
					string newFileName = null;
					if (digitalDownload)
					{
						newFileName = storeFront.ChooseFileNameWildcard(storeFrontConfig.Client, "DigitalDownload/Products", defaultFileName, applicationPath, server);
					}
					else
					{
						newFileName = storeFront.ChooseFileNameWildcard(storeFrontConfig.Client, "CatalogContent/Products", defaultFileName, applicationPath, server);
					}

					if (string.IsNullOrEmpty(newFileName))
					{
						if (verbose)
						{
							results.AppendLine("OK - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - File is null and no file found starting with '" + defaultFileName + "'");
						}
					}
					else
					{
						results.AppendLine("- - - - - Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - New File '" + newFileName + "' - - - - -");
						currentFileName = newFileName;
						property.SetValue(product, newFileName);
						hasDbChanges = true;
					}
				}
				else
				{
					if (verbose)
					{
						results.AppendLine("OK Product '" + product.Name + "' [" + product.ProductId + "] " + displayName + " - Ignoring Blank file link because searchForFileIfBlank = false");
					}
				}
			}
			return results.ToString();
		}

		public static string SyncImageFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_Image";
			return product.SyncCatalogFile(model => model.ImageName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncDigitalDownloadFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_DigitalDownload";
			return product.SyncDigitalDownloadFile(model => model.DigitalDownloadFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncSampleAudioFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_SampleAudio";
			return product.SyncCatalogFile(model => model.SampleAudioFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncSampleDownloadFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_SampleDownload";
			return product.SyncCatalogFile(model => model.SampleDownloadFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncSampleImageFile(this Product product, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			string defaultFileName = product.UrlName + "_SampleImage";
			return product.SyncCatalogFile(model => model.SampleImageFileName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		public static string SyncImageFile(this ProductBundle productBundle, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}
			string defaultFileName = productBundle.UrlName + "_Image";
			return productBundle.SyncCatalogFile(model => model.ImageName, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}


		public static string SyncCatalogFile(this ProductBundle productBundle, Expression<Func<ProductBundle, String>> expression, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			return productBundle.SyncFileHelper(expression, false, defaultFileName, eraseFileNameIfNotFound, searchForFileIfBlank, preview, verbose, applicationPath, routeData, server, out hasDbChanges);
		}

		private static string SyncFileHelper(this ProductBundle productBundle, Expression<Func<ProductBundle, String>> expression, bool digitalDownload, string defaultFileName, bool eraseFileNameIfNotFound, bool searchForFileIfBlank, bool preview, bool verbose, string applicationPath, RouteData routeData, HttpServerUtilityBase server, out bool hasDbChanges)
		{
			if (productBundle == null)
			{
				throw new ArgumentNullException("productBundle");
			}
			StoreFront storeFront = productBundle.StoreFront;
			StoreFrontConfiguration storeFrontConfig = storeFront.CurrentConfigOrAny();
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("productBundle.StoreFront.CurrentConfigOrAny");
			}

			ModelMetadata metadata = ModelMetadata.FromLambdaExpression<ProductBundle, String>(expression, new ViewDataDictionary<ProductBundle>());
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			string displayName = metadata.DisplayName ?? metadata.PropertyName;
			PropertyInfo property = typeof(ProductBundle).GetProperty(expressionText);
			string currentFileName = (property.GetValue(productBundle) ?? "").ToString();


			hasDbChanges = false;
			StringBuilder results = new StringBuilder();

			if (!string.IsNullOrEmpty(currentFileName))
			{
				if (eraseFileNameIfNotFound)
				{
					//file name is set, verify file and set to null if not found
					string filePath = storeFront.ProductBundleCatalogFilePath(applicationPath, routeData, server, currentFileName);
					if (string.IsNullOrEmpty(filePath))
					{
						results.AppendLine("- - - - - Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - '" + currentFileName + "' not found, setting to null - - - - -");
						currentFileName = null;
						property.SetValue(productBundle, null);
						hasDbChanges = true;
					}
					else
					{
						if (verbose)
						{
							results.AppendLine("OK - Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - '" + currentFileName + "' File Confirmed");
						}
					}
				}
			}

			if (string.IsNullOrEmpty(currentFileName))
			{
				if (searchForFileIfBlank)
				{
					//if file name is not set (or file was not found and set to blank), see if there is a suitable file in the file system
					string newFileName = storeFront.ChooseFileNameWildcard(storeFrontConfig.Client, "CatalogContent/Bundles", defaultFileName, applicationPath, server);

					if (string.IsNullOrEmpty(newFileName))
					{
						if (verbose)
						{
							results.AppendLine("OK - Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - File is null and no file found starting with '" + defaultFileName + "'");
						}
					}
					else
					{
						results.AppendLine("- - - - - Product 'Bundle " + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - New File '" + newFileName + "' - - - - -");
						currentFileName = newFileName;
						property.SetValue(productBundle, newFileName);
						hasDbChanges = true;
					}
				}
				else
				{
					if (verbose)
					{
						results.AppendLine("OK Product Bundle '" + productBundle.Name + "' [" + productBundle.ProductBundleId + "] " + displayName + " - Ignoring Blank file link because searchForFileIfBlank = false");
					}
				}
			}
			return results.ToString();
		}




		public static string DefaultSummaryCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSummaryCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSummaryCaption + "')";
				}
				return category.DefaultSummaryCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Summary')";
			}
			return "Summary";
		}

		public static string DefaultTopDescriptionCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultTopDescriptionCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultTopDescriptionCaption + "')";
				}
				return category.DefaultTopDescriptionCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Description for " + productName + "')";
			}
			return "Description for " + productName;
		}

		public static string DefaultBottomDescriptionCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultBottomDescriptionCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultBottomDescriptionCaption + "')";
				}
				return category.DefaultBottomDescriptionCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Details for " + productName + "')";
			}
			return "Details for " + productName;
		}

		public static string DefaultSampleImageCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSampleImageCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSampleImageCaption + "')";
				}
				return category.DefaultSampleImageCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Sample Image for " + productName + "')";
			}
			return "Sample Image for " + productName;
		}

		public static string DefaultSampleDownloadCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSampleDownloadCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSampleDownloadCaption + "')";
				}
				return category.DefaultSampleDownloadCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Sample Download for " + productName + "')";
			}
			return "Sample Download for " + productName;
		}

		public static string DefaultSampleAudioCaptionOrSystemDefault(this ProductCategory category, string productName, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.DefaultSampleAudioCaption)))
			{
				if (wrapForAdmin)
				{
					return "(Category Default: '" + category.DefaultSampleAudioCaption + "')";
				}
				return category.DefaultSampleAudioCaption;
			}
			if (wrapForAdmin)
			{
				return "(System Default: 'Sample Audio for " + productName + "')";
			}
			return "Sample Audio for " + productName;
		}

		public static MvcHtmlString NoProductsMessageHtmlOrSystemDefault(this ProductCategory category, bool wrapForAdmin = false)
		{
			if ((category != null) && (!string.IsNullOrEmpty(category.NoProductsMessageHtml)))
			{
				if (wrapForAdmin)
				{
					return new MvcHtmlString("(Category Default: '" + category.NoProductsMessageHtml + "')");
				}
				return new MvcHtmlString(category.NoProductsMessageHtml);
			}
			if (category != null)
			{
				if (wrapForAdmin)
				{
					return new MvcHtmlString("(System Default: <h2>There are no current " + category.ProductTypePlural.ToHtml() + " in this Category</h2>");
				}
				return new MvcHtmlString("<h2>There are no current " + category.ProductTypePlural.ToHtml() + " in this Category</h2>");
			}

			if (wrapForAdmin)
			{
				return new MvcHtmlString("(System Default: <h2>There are no current products in this Category</h2>");
			}
			return new MvcHtmlString("<h2>There are no current products in this Category</h2>");
		}

		public static string SummaryCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SummaryCaption))
			{
				return product.SummaryCaption;
			}
			return product.Category.DefaultSummaryCaptionOrSystemDefault(product.Name);
		}

		public static string TopDescriptionCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.TopDescriptionCaption))
			{
				return product.TopDescriptionCaption;
			}
			return product.Category.DefaultTopDescriptionCaptionOrSystemDefault(product.Name);
		}

		public static string BottomDescriptionCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.BottomDescriptionCaption))
			{
				return product.BottomDescriptionCaption;
			}
			return product.Category.DefaultBottomDescriptionCaptionOrSystemDefault(product.Name);
		}

		public static string SampleImageCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SampleImageCaption))
			{
				return product.SampleImageCaption;
			}
			return product.Category.DefaultSampleImageCaptionOrSystemDefault(product.Name);
		}

		public static string SampleDownloadCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SampleDownloadCaption))
			{
				return product.SampleDownloadCaption;
			}
			return product.Category.DefaultSampleDownloadCaptionOrSystemDefault(product.Name);
		}

		public static string SampleAudioCaptionOrDefault(this Product product, bool wrapForAdmin = false)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (!string.IsNullOrEmpty(product.SampleAudioCaption))
			{
				return product.SampleAudioCaption;
			}
			return product.Category.DefaultSampleAudioCaptionOrSystemDefault(product.Name);
		}

		public static string SummaryCaptionOrDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.SummaryCaption))
			{
				return bundle.SummaryCaption;
			}
			return bundle.Category.DefaultSummaryCaptionOrSystemDefault(bundle.Name);
		}

		public static string TopDescriptionCaptionOrDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.TopDescriptionCaption))
			{
				return bundle.TopDescriptionCaption;
			}
			return bundle.Category.DefaultTopDescriptionCaptionOrSystemDefault(bundle.Name);
		}

		public static string BottomDescriptionCaptionOrDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.BottomDescriptionCaption))
			{
				return bundle.BottomDescriptionCaption;
			}
			return bundle.Category.DefaultBottomDescriptionCaptionOrSystemDefault(bundle.Name);
		}

		public static string ProductTypeSingleOrSystemDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.ProductTypeSingle))
			{
				return bundle.ProductTypeSingle;
			}
			return bundle.Category.ProductTypeSingle.OrDefault("Item");
		}

		public static string ProductTypePluralOrSystemDefault(this ProductBundle bundle, bool wrapForAdmin = false)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (!string.IsNullOrEmpty(bundle.ProductTypePlural))
			{
				return bundle.ProductTypePlural;
			}
			return bundle.Category.ProductTypePlural.OrDefault("Items");
		}

		public static void SetDefaultsForNew(this Payment payment, StoreFrontConfiguration storeFrontConfig)
		{
			if (storeFrontConfig == null)
			{
				throw new ArgumentNullException("storeFrontConfig");
			}

			StoreFront storeFront = storeFrontConfig.StoreFront;

			payment.Client = storeFront.Client;
			payment.ClientId = storeFront.ClientId;
			payment.StoreFront = storeFront;
			payment.StoreFrontId = storeFront.StoreFrontId;

			payment.AmountPaid = 0M;
			payment.IsProcessed = false;
			payment.Json = null;
			payment.ProcessDateTimeUtc = null;
			payment.TransactionId = null;

			payment.IsPending = false;
			payment.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			payment.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);

		}

		public static string ToStringDetails(this Discount discount)
		{
			if (discount == null)
			{
				return "(none)";
			}

			StringBuilder details = new StringBuilder();
			details.Append("Code: " + discount.Code + " [" + discount.DiscountId + "]");
			if (discount.FlatDiscount > 0M)
			{
				details.Append(" Flat Discount: $" + discount.FlatDiscount.ToString("N2"));
			}
			if (discount.FreeProduct != null)
			{
				details.Append(" Free Product: " + discount.FreeProduct.Name + " [" + discount.FreeProduct.ProductId + "]");
			}
			if (discount.FreeShipping)
			{
				details.Append(" Free Shipping");
			}
			if (discount.MinSubtotal > 0M)
			{
				details.Append(" Min Order $" + discount.MinSubtotal.ToString("N2"));
			}
			if (discount.PercentOff > 0M)
			{
				details.Append(" Percent Off: " + discount.PercentOff.ToString("N0") + "%");
			}

			return details.ToString();
		}

		public static bool TestPayPal(this StoreFrontConfiguration config, BaseController controller)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			if (!config.PaymentMethod_PayPal_Enabled)
			{
				controller.AddUserMessage("PayPal Test Failed!", "PayPal is not enabled in your store front configuration. Make sure to enable it on the Payments tab.", UserMessageType.Danger);
				return false;
			}
			else
			{
				bool useSandbox = !config.PaymentMethod_PayPal_UseLiveServer;
				PayPalPaymentClient client = new PayPalPaymentClient();
				try
				{
					PayPalOAuthTokenData token = client.TestPayPalOAuthToken(config.PaymentMethod_PayPal_Client_Id, config.PaymentMethod_PayPal_Client_Secret, useSandbox);
					if (!string.IsNullOrWhiteSpace(token.access_token))
					{
						//token appears good
						controller.AddUserMessage("PayPal Test Passed!", "PayPal tested successfully using the " + (useSandbox ? " TEST" : " LIVE") + " server", UserMessageType.Success);
						return true;
					}
					controller.AddUserMessage("PayPal Test Failed!", "PayPal test failed using the " + (useSandbox ? " TEST" : " LIVE") + " server", UserMessageType.Danger);
					return false;
				}
				catch (Exception ex)
				{
					controller.AddUserMessage("PayPal Test Failed!", "PayPal test failed. with error " + ex.GetType().FullName + " using the " + (useSandbox ? " TEST" : " LIVE") + " server.\nPlease verify your PayPal configuration in the store front edit page on the Payments Tab.\nError Message: " + ex.Message, UserMessageType.Danger);
					return false;
				}
			}
		}

		public static string MetaDescriptionOrSystemDefault(this ProductCategory category, StoreFrontConfiguration storeFrontConfig)
		{
			if (category != null && (!string.IsNullOrEmpty(category.MetaDescription)))
			{
				return category.MetaDescription;
			}
			if (storeFrontConfig != null && (!string.IsNullOrEmpty(storeFrontConfig.MetaDescription)))
			{
				return storeFrontConfig.MetaDescription;
			}
			return BaseController.DefaultMetaDescription;
		}

		public static string MetaKeywordsOrSystemDefault(this ProductCategory category, StoreFrontConfiguration storeFrontConfig)
		{
			if (category != null && (!string.IsNullOrEmpty(category.MetaKeywords)))
			{
				return category.MetaKeywords;
			}
			if (storeFrontConfig != null && (!string.IsNullOrEmpty(storeFrontConfig.MetaKeywords)))
			{
				return storeFrontConfig.MetaKeywords;
			}
			return BaseController.DefaultMetaKeywords;
		}

		public static string MetaDescriptionOrSystemDefault(this Product product, StoreFrontConfiguration storeFrontConfig)
		{
			if (product != null && (!string.IsNullOrEmpty(product.MetaDescription)))
			{
				return product.MetaDescription;
			}
			return product.Category.MetaDescriptionOrSystemDefault(storeFrontConfig);
		}

		public static string MetaKeywordsOrSystemDefault(this Product product, StoreFrontConfiguration storeFrontConfig)
		{
			if (product != null && (!string.IsNullOrEmpty(product.MetaKeywords)))
			{
				return product.MetaKeywords;
			}
			return product.Category.MetaKeywordsOrSystemDefault(storeFrontConfig);
		}

		public static string MetaDescriptionOrSystemDefault(this ProductBundle bundle, StoreFrontConfiguration storeFrontConfig)
		{
			if (bundle != null && (!string.IsNullOrEmpty(bundle.MetaDescription)))
			{
				return bundle.MetaDescription;
			}
			return bundle.Category.MetaDescriptionOrSystemDefault(storeFrontConfig);
		}

		public static string MetaKeywordsOrSystemDefault(this ProductBundle bundle, StoreFrontConfiguration storeFrontConfig)
		{
			if (bundle != null && (!string.IsNullOrEmpty(bundle.MetaKeywords)))
			{
				return bundle.MetaKeywords;
			}
			return bundle.Category.MetaKeywordsOrSystemDefault(storeFrontConfig);
		}

		public static ProductDetailTemplateEnum ProductDetailTemplateOrCategory(this Product product)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}
			if (product.ProductDetailTemplate.HasValue)
			{
				return product.ProductDetailTemplate.Value;
			}

			return product.Category.ProductDetailTemplate;
		}

		public static ProductBundleDetailTemplateEnum ProductBundleDetailTemplateOrCategory(this ProductBundle bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}
			if (bundle.ProductBundleDetailTemplate.HasValue)
			{
				return bundle.ProductBundleDetailTemplate.Value;
			}

			return bundle.Category.ProductBundleDetailTemplate;
		}

		public static decimal? UnitPrice(this ProductBundle bundle, int qty)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.ProductBundleItems == null || bundle.ProductBundleItems.Count == 0)
			{
				return null;
			}

			if (!bundle.ProductBundleItems.AsQueryable().WhereIsActive().All(bi => bi.BaseUnitPrice.HasValue))
			{
				//if any items don't have a unit price, unit price of this item is null (call for price)
				return null;
			}
			return bundle.ProductBundleItems.AsQueryable().WhereIsActive().Sum(bi => bi.UnitPriceExt(bi.Quantity));
		}

		public static decimal? ListPrice(this ProductBundle bundle, int qty)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException("bundle");
			}

			if (bundle.ProductBundleItems == null || bundle.ProductBundleItems.Count == 0)
			{
				return null;
			}

			if (!bundle.ProductBundleItems.AsQueryable().WhereIsActive().All(bi => bi.BaseListPrice.HasValue))
			{
				//if any items don't have a list price, list price of this item is null (call for price)
				return null;
			}
			return bundle.ProductBundleItems.AsQueryable().WhereIsActive().Sum(bi => bi.ListPriceExt(bi.Quantity));
		}

		public static string RandomImageName(this ProductCategory category)
		{
			string folderPath = MapPathToGStore("~/Content/Server/CatalogContent/Categories");
			return FolderRandomImageFileName(folderPath);
		}

		public static string RandomImageName(this Product product)
		{
			string folderPath = MapPathToGStore("~/Content/Server/CatalogContent/Products");
			return FolderRandomImageFileName(folderPath);
		}

		public static string RandomImageName(this ProductBundle bundle)
		{
			string folderPath = MapPathToGStore("~/Content/Server/CatalogContent/Bundles");
			return FolderRandomImageFileName(folderPath);
		}

		public static string FolderRandomImageFileName(string folderPath, string excludeFileNameStartsWith = "RenoG")
		{
			if (!System.IO.Directory.Exists(folderPath))
			{
				return null;
			}

			DirectoryInfo folderInfo = new System.IO.DirectoryInfo(folderPath);

			IEnumerable<FileInfo> query = null;
			if (string.IsNullOrEmpty(excludeFileNameStartsWith))
			{
				query = folderInfo.GetFiles().Where(f => f.Name.FileExtensionIsImage()).ToList();
			}
			else
			{
				query = folderInfo.GetFiles().Where(f => f.Name.FileExtensionIsImage() && !f.Name.StartsWith(excludeFileNameStartsWith));
			}

			List<FileInfo> files = query.ToList();
			if (files.Count == 0)
			{
				return null;
			}

			Random rndNumber = new Random();
			int randomIndex = rndNumber.Next(1, files.Count);

			return files[randomIndex - 1].Name;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="virtualPath">Example: "~/Views/Page"</param>
		/// <returns></returns>
		public static string MapPathToGStore(string virtualPath)
		{
			string path = string.Empty;
			if (HttpContext.Current == null)
			{
				string assemblyPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).AbsolutePath;
				string directoryName = System.IO.Path.GetDirectoryName(assemblyPath).Replace("GStore\\GStoreData\\", "GStore\\GStoreWeb\\");
				path = System.IO.Path.Combine(directoryName, "..\\.." + virtualPath.TrimStart('~').Replace('/', '\\')).Replace("%20", " ");
			}
			else
			{
				path = HttpContext.Current.Server.MapPath(virtualPath);
			}

			return path;
		}

		public static void CloneFromParentForNew(this ProductCategory productCategory, ProductCategory parentProductCategory)
		{
			productCategory.ParentCategory = parentProductCategory;
			productCategory.ParentCategoryId = parentProductCategory.ProductCategoryId;

			productCategory.AllowChildCategoriesInMenu = parentProductCategory.AllowChildCategoriesInMenu;
			productCategory.BundleListTemplate = parentProductCategory.BundleListTemplate;
			productCategory.BundleTypePlural = parentProductCategory.BundleTypePlural;
			productCategory.BundleTypeSingle = parentProductCategory.BundleTypeSingle;
			productCategory.CategoryDetailTemplate = parentProductCategory.CategoryDetailTemplate;
			productCategory.ChildCategoryFooterHtml = parentProductCategory.ChildCategoryFooterHtml;
			productCategory.ChildCategoryHeaderHtml = parentProductCategory.ChildCategoryHeaderHtml;
			productCategory.DefaultBottomDescriptionCaption = parentProductCategory.DefaultBottomDescriptionCaption;
			productCategory.DefaultSampleAudioCaption = parentProductCategory.DefaultSampleAudioCaption;
			productCategory.DefaultSampleDownloadCaption = parentProductCategory.DefaultSampleDownloadCaption;
			productCategory.DefaultSampleImageCaption = parentProductCategory.DefaultSampleImageCaption;
			productCategory.DefaultSummaryCaption = parentProductCategory.DefaultSummaryCaption;
			productCategory.DefaultTopDescriptionCaption = parentProductCategory.DefaultTopDescriptionCaption;
			productCategory.DisplayForDirectLinks = parentProductCategory.DisplayForDirectLinks;
			productCategory.ForAnonymousOnly = parentProductCategory.ForAnonymousOnly;
			productCategory.ForRegisteredOnly = parentProductCategory.ForRegisteredOnly;
			productCategory.HideInMenuIfEmpty = parentProductCategory.HideInMenuIfEmpty;
			productCategory.ImageName = parentProductCategory.ImageName;
			productCategory.MetaDescription = parentProductCategory.MetaDescription;
			productCategory.MetaKeywords = parentProductCategory.MetaKeywords;
			productCategory.NoProductsMessageHtml = parentProductCategory.NoProductsMessageHtml;
			productCategory.ProductBundleDetailTemplate = parentProductCategory.ProductBundleDetailTemplate;
			productCategory.ProductBundleFooterHtml = parentProductCategory.ProductBundleFooterHtml;
			productCategory.ProductBundleHeaderHtml = parentProductCategory.ProductBundleHeaderHtml;
			productCategory.ProductDetailTemplate = parentProductCategory.ProductDetailTemplate;
			productCategory.ProductFooterHtml = parentProductCategory.ProductFooterHtml;
			productCategory.ProductHeaderHtml = parentProductCategory.ProductHeaderHtml;
			productCategory.ProductListTemplate = parentProductCategory.ProductListTemplate;
			productCategory.ProductTypePlural = parentProductCategory.ProductTypePlural;
			productCategory.ProductTypeSingle = parentProductCategory.ProductTypeSingle;
			productCategory.ShowInCatalogIfEmpty = parentProductCategory.ShowInCatalogIfEmpty;
			productCategory.ShowInMenu = parentProductCategory.ShowInMenu;
			productCategory.Theme = parentProductCategory.Theme;
			productCategory.ThemeId = parentProductCategory.ThemeId;
			productCategory.UseDividerAfterOnMenu = parentProductCategory.UseDividerAfterOnMenu;
			productCategory.UseDividerBeforeOnMenu = parentProductCategory.UseDividerBeforeOnMenu;
		}
	}
}