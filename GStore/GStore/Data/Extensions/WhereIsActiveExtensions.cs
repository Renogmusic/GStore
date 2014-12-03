using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GStore.Data
{
	public static class WhereIsActiveExtensions
	{
		/// <summary>
		/// IQueryable query extension to check where UserProfile is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<UserProfile> WhereIsActive(this IQueryable<UserProfile> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<UserProfile> WhereIsActiveOn(this IQueryable<UserProfile> query, DateTime dateTimeUtc, bool includePending = false)
		{
			
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				);
		}


		/// <summary>
		/// IQueryable query extension to check where Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<Client> WhereIsActive(this IQueryable<Client> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<Client> WhereIsActiveOn(this IQueryable<Client> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where StoreFront and Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<StoreFront> WhereIsActive(this IQueryable<StoreFront> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<StoreFront> WhereIsActiveOn(this IQueryable<StoreFront> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where StoreBinding, Client, and StoreFront s currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<StoreBinding> WhereIsActive(this IQueryable<StoreBinding> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<StoreBinding> WhereIsActiveOn(this IQueryable<StoreBinding> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where Page, Client, and StoreFront is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<Page> WhereIsActive(this IQueryable<Page> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<Page> WhereIsActiveOn(this IQueryable<Page> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.PageTemplate.IsPending)
				&& (data.PageTemplate.StartDateTimeUtc < dateTimeUtc)
				&& (data.PageTemplate.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where ProductCategory, Client, and StoreFront is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ProductCategory> WhereIsActive(this IQueryable<ProductCategory> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<ProductCategory> WhereIsActiveOn(this IQueryable<ProductCategory> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where Product, Client, and StoreFront is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<Product> WhereIsActive(this IQueryable<Product> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<Product> WhereIsActiveOn(this IQueryable<Product> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where NavBarItem, Client, and StoreFront is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<NavBarItem> WhereIsActive(this IQueryable<NavBarItem> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<NavBarItem> WhereIsActiveOn(this IQueryable<NavBarItem> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<PageSection> WhereIsActive(this IQueryable<PageSection> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<PageSection> WhereIsActiveOn(this IQueryable<PageSection> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.PageTemplateSection.IsPending)
				&& (data.PageTemplateSection.StartDateTimeUtc < dateTimeUtc)
				&& (data.PageTemplateSection.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.PageTemplateSection.PageTemplate.IsPending)
				&& (data.PageTemplateSection.PageTemplate.StartDateTimeUtc < dateTimeUtc)
				&& (data.PageTemplateSection.PageTemplate.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Page.IsPending)
				&& (data.Page.StartDateTimeUtc < dateTimeUtc)
				&& (data.Page.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Page.StoreFront.IsPending)
				&& (data.Page.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.Page.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}


		/// <summary>
		/// IQueryable query extension to check where ClientRole and Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ClientRole> WhereIsActive(this IQueryable<ClientRole> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<ClientRole> WhereIsActiveOn(this IQueryable<ClientRole> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where ClientUserRole, ClientRole, and Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ClientUserRole> WhereIsActive(this IQueryable<ClientUserRole> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<ClientUserRole> WhereIsActiveOn(this IQueryable<ClientUserRole> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.ClientRole.IsPending)
				&& (data.ClientRole.StartDateTimeUtc < dateTimeUtc)
				&& (data.ClientRole.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (
						data.ScopeStoreFrontId == null 
						|| 
						(
							(includePending || !data.ScopeStoreFront.IsPending) 
							&& data.ScopeStoreFront.StartDateTimeUtc < dateTimeUtc 
							&& data.ScopeStoreFront.EndDateTimeUtc > dateTimeUtc
						)
					)
				);
		}

		/// <summary>
		/// IQueryable query extension to check where ClientRoleAction, ClientRole, and Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<ClientRoleAction> WhereIsActive(this IQueryable<ClientRoleAction> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<ClientRoleAction> WhereIsActiveOn(this IQueryable<ClientRoleAction> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.ClientRole.IsPending)
				&& (data.ClientRole.StartDateTimeUtc < dateTimeUtc)
				&& (data.ClientRole.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<ValueListItem> WhereIsActive(this IQueryable<ValueListItem> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<ValueListItem> WhereIsActiveOn(this IQueryable<ValueListItem> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.ValueList.IsPending)
				&& (data.ValueList.StartDateTimeUtc < dateTimeUtc)
				&& (data.ValueList.EndDateTimeUtc > dateTimeUtc)
				);
		}


		public static IQueryable<PageTemplate> WhereIsActive(this IQueryable<PageTemplate> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<PageTemplate> WhereIsActiveOn(this IQueryable<PageTemplate> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<PageTemplateSection> WhereIsActive(this IQueryable<PageTemplateSection> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<PageTemplateSection> WhereIsActiveOn(this IQueryable<PageTemplateSection> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (includePending || !data.PageTemplate.IsPending)
				&& (data.PageTemplate.StartDateTimeUtc < dateTimeUtc)
				&& (data.PageTemplate.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<Theme> WhereIsActive(this IQueryable<Theme> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IQueryable<Theme> WhereIsActiveOn(this IQueryable<Theme> query, DateTime dateTimeUtc, bool includePending = false)
		{
			return query.Where(data =>
				(includePending || !data.IsPending)
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				);
		}


		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this StoreFront storeFront)
		{
			if (storeFront == null)
			{
				throw new ArgumentNullException("storeFront");
			}

			return storeFront.IsActiveDirect() && storeFront.Client.IsActiveDirect();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this StoreBinding storeBinding)
		{
			if (storeBinding == null)
			{
				throw new ArgumentNullException("storeBinding");
			}

			return storeBinding.IsActiveDirect() && storeBinding.StoreFront.IsActiveBubble();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this UserProfile userProfile)
		{
			if (userProfile == null)
			{
				throw new ArgumentNullException("userProfile");
			}

			return userProfile.IsActiveDirect()
				&& (userProfile.ClientId == null || userProfile.Client.IsActiveDirect())
				&& (userProfile.StoreFrontId == null || userProfile.StoreFront.IsActiveDirect());
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this PageTemplateSection pageTemplateSection)
		{
			if (pageTemplateSection == null)
			{
				throw new ArgumentNullException("pageTemplateSection");
			}

			return pageTemplateSection.IsActiveDirect() && pageTemplateSection.PageTemplate.IsActiveDirect();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this ValueList valueList)
		{
			if (valueList == null)
			{
				throw new ArgumentNullException("valueList");
			}

			return valueList.IsActiveDirect() && valueList.Client.IsActiveDirect();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this ValueListItem valueListItem)
		{
			if (valueListItem == null)
			{
				throw new ArgumentNullException("valueListItem");
			}

			return valueListItem.IsActiveDirect() && valueListItem.ValueList.IsActiveBubble();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this Page page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}

			return page.IsActiveDirect() && page.PageTemplate.IsActiveDirect() && page.StoreFront.IsActiveBubble();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this PageSection pageSection)
		{
			if (pageSection == null)
			{
				throw new ArgumentNullException("pageSection");
			}

			return pageSection.IsActiveDirect() && pageSection.Page.IsActiveBubble()
				&& pageSection.PageTemplateSection.IsActiveDirect()
				&& pageSection.StoreFront.IsActiveBubble();
		}


	}
}