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
		public static IQueryable<UserProfile> WhereIsActiveOn(this IQueryable<UserProfile> query, DateTime dateTimeUtc, bool includeInactive = false)
		{
			
			return query.Where(data =>
				(includeInactive || data.Active)
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
		/// IEnumerable query extension to check where ProductCategory, Client, and StoreFront is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<ProductCategory> WhereIsActive(this IEnumerable<ProductCategory> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IEnumerable<ProductCategory> WhereIsActiveOn(this IEnumerable<ProductCategory> query, DateTime dateTimeUtc, bool includePending = false)
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
		/// IEnumerable query extension to check where Product, Client, and StoreFront is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<Product> WhereIsActive(this IEnumerable<Product> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IEnumerable<Product> WhereIsActiveOn(this IEnumerable<Product> query, DateTime dateTimeUtc, bool includePending = false)
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
		/// IEnumerable query extension to check where NavBarItem, Client, and StoreFront is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<NavBarItem> WhereIsActive(this IEnumerable<NavBarItem> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IEnumerable<NavBarItem> WhereIsActiveOn(this IEnumerable<NavBarItem> query, DateTime dateTimeUtc, bool includePending = false)
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
		/// IEnumerable query extension to check where ClientRole and Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<ClientRole> WhereIsActive(this IEnumerable<ClientRole> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IEnumerable<ClientRole> WhereIsActiveOn(this IEnumerable<ClientRole> query, DateTime dateTimeUtc, bool includePending = false)
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
		/// IEnumerable query extension to check where ClientUserRole, ClientRole, and Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<ClientUserRole> WhereIsActive(this IEnumerable<ClientUserRole> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IEnumerable<ClientUserRole> WhereIsActiveOn(this IEnumerable<ClientUserRole> query, DateTime dateTimeUtc, bool includePending = false)
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
		/// IEnumerable query extension to check where ClientRoleAction, ClientRole, and Client is currently active and not pending or expired
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IEnumerable<ClientRoleAction> WhereIsActive(this IEnumerable<ClientRoleAction> query)
		{
			return query.WhereIsActiveOn(DateTime.UtcNow);
		}
		public static IEnumerable<ClientRoleAction> WhereIsActiveOn(this IEnumerable<ClientRoleAction> query, DateTime dateTimeUtc, bool includePending = false)
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
	}
}