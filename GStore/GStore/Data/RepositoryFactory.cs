using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.Data;

namespace GStore.Data
{
	public enum RepositoryProviderEnum
	{
		EntityFrameworkCodeFirstProvider = 100,
		ListProvider = 200
	}

	public static class RepositoryFactory
	{
		private static IGstoreDb _listDb = null;

		/// <summary>
		/// Returns a repository for the current store front and sets clientid and storefrontid
		/// </summary>
		/// <param name="httpContext"></param>
		/// <param name="throwErrorIfStoreFrontNotFound"></param>
		/// <returns></returns>
		public static IGstoreDb StoreFrontRepository(HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ApplicationException("HTTP Context is null, call repository method with valid context");
			}
			string userName = httpContext.User.Identity.Name;

			IGstoreDb db = NewRepository(userName);

			return db;
		}


		/// <summary>
		/// Returns a repository with no client id or store front set. used for configuring new clients or managing system fields not client-specific
		/// </summary>
		/// <returns></returns>
		public static IGstoreDb SystemWideRepository(System.Security.Principal.IPrincipal user)
		{
			string userName = string.Empty;
			if (user != null)
			{
				userName = user.Identity.Name;
			}
			return NewRepository(userName);
		}

		/// <summary>
		/// Returns a repository with no client id or store front set. used for configuring new clients or managing system fields not client-specific
		/// </summary>
		/// <returns></returns>
		public static IGstoreDb SystemWideRepository(string userName)
		{
			return NewRepository(userName);
		}

		private static IGstoreDb NewRepository(string userName)
		{
			//get setting for repositorysource and create repository
			switch (RepositoryProvider())
			{
				case RepositoryProviderEnum.EntityFrameworkCodeFirstProvider:
					return new Data.EntityFrameworkCodeFirstProvider.GStoreEFDbContext(userName);
				case RepositoryProviderEnum.ListProvider :
					//todo: note ListProvider uses a single static list, no copies or real separate contexts
					_listDb = new Data.ListProvider.ListContext();
					SeedDataExtensions.AddSeedData(_listDb);
					return _listDb;
				//todo: allow for other repositories, perhaps by class or project name; and config mapping here
				//Example: pull in a provider that takes some lists, some ef, some web services, some whoknowswhats
				default:
					break;
			}

			throw new ApplicationException("Unable to create repository:" + RepositoryProvider().ToString());
		}

		/// <summary>
		/// Returns the enum translations of setting RepositorySource
		/// </summary>
		/// <returns></returns>
		public static RepositoryProviderEnum RepositoryProvider()
		{
			switch (GStore.Properties.Settings.Current.RepositoryProvider.ToLower())
			{
				case "entityframeworkcodefirstprovider":
				case "entityframeworkcodefirst":
					return RepositoryProviderEnum.EntityFrameworkCodeFirstProvider;
				case "listprovider":
				case "list":
					return RepositoryProviderEnum.ListProvider;
				default:
					//todo: allow for other repositories, perhaps by class or project name; and config mapping here
					//Example: pull in a provider that takes some lists, some ef, some web services, some whoknowswhats
					break;
			}

			//default repository
			return RepositoryProviderEnum.EntityFrameworkCodeFirstProvider;
		}
	}
}