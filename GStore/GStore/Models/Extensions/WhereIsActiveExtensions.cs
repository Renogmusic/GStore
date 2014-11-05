using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Models.Extensions
{
	public static class WhereIsActiveExtensions
	{
		public static IQueryable<UserProfile> WhereIsActive(this IQueryable<UserProfile> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<UserProfile> WhereIsActive(this IQueryable<UserProfile> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				data.Active
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				);
		}


		public static IQueryable<Client> WhereIsActive(this IQueryable<Client> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<Client> WhereIsActive(this IQueryable<Client> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<StoreFront> WhereIsActive(this IQueryable<StoreFront> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<StoreFront> WhereIsActive(this IQueryable<StoreFront> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<StoreBinding> WhereIsActive(this IQueryable<StoreBinding> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<StoreBinding> WhereIsActive(this IQueryable<StoreBinding> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<Page> WhereIsActive(this IQueryable<Page> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<Page> WhereIsActive(this IQueryable<Page> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<ProductCategory> WhereIsActive(this IQueryable<ProductCategory> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IQueryable<ProductCategory> WhereIsActive(this IQueryable<ProductCategory> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IEnumerable<ProductCategory> WhereIsActive(this IEnumerable<ProductCategory> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IEnumerable<ProductCategory> WhereIsActive(this IEnumerable<ProductCategory> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IEnumerable<Product> WhereIsActive(this IEnumerable<Product> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IEnumerable<Product> WhereIsActive(this IEnumerable<Product> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IEnumerable<NavBarItem> WhereIsActive(this IEnumerable<NavBarItem> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IEnumerable<NavBarItem> WhereIsActive(this IEnumerable<NavBarItem> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& (!data.StoreFront.IsPending)
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IEnumerable<ClientUserRole> WhereIsActive(this IEnumerable<ClientUserRole> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IEnumerable<ClientUserRole> WhereIsActive(this IEnumerable<ClientUserRole> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IEnumerable<StoreFrontUserRole> WhereIsActive(this IEnumerable<StoreFrontUserRole> query)
		{
			return query.WhereIsActive(DateTime.UtcNow);
		}
		public static IEnumerable<StoreFrontUserRole> WhereIsActive(this IEnumerable<StoreFrontUserRole> query, DateTime dateTimeUtc)
		{
			return query.Where(data =>
				!data.IsPending
				&& (data.StartDateTimeUtc < dateTimeUtc)
				&& (data.EndDateTimeUtc > dateTimeUtc)
				&& (!data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				&& !data.StoreFront.IsPending
				&& (data.StoreFront.StartDateTimeUtc < dateTimeUtc)
				&& (data.StoreFront.EndDateTimeUtc > dateTimeUtc)
				);
		}

	}
}