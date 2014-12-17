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

		public static IQueryable<Page> WhereIsActive(this IQueryable<Page> query)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, 0);
		}
		public static IQueryable<Page> WhereIsActiveOrSelected(this IQueryable<Page> query, int? selectedId)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, selectedId);
		}
		public static IQueryable<Page> WhereIsActiveOnOrSelected(this IQueryable<Page> query, DateTime dateTimeUtc, int? selectedId, bool includePending = false)
		{
			int selectedValue = selectedId ?? 0;
			return query.Where(data => data.PageTemplateId == selectedValue
				||
				(
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
				)
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
				&& (includePending || !data.Client.IsPending)
				&& (data.Client.StartDateTimeUtc < dateTimeUtc)
				&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				);
		}

		public static IQueryable<PageTemplate> WhereIsActive(this IQueryable<PageTemplate> query)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, 0);
		}
		public static IQueryable<PageTemplate> WhereIsActiveOrSelected(this IQueryable<PageTemplate> query, int? selectedId)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, selectedId);
		}
		public static IQueryable<PageTemplate> WhereIsActiveOnOrSelected(this IQueryable<PageTemplate> query, DateTime dateTimeUtc, int? selectedId, bool includePending = false)
		{
			int selectedValue = selectedId ?? 0;
			return query.Where(data => data.PageTemplateId == selectedValue
				||
				(
					(includePending || !data.IsPending)
					&& (data.StartDateTimeUtc < dateTimeUtc)
					&& (data.EndDateTimeUtc > dateTimeUtc)
					&& (includePending || !data.Client.IsPending)
					&& (data.Client.StartDateTimeUtc < dateTimeUtc)
					&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				)
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
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, 0);
		}
		public static IQueryable<Theme> WhereIsActiveOrSelected(this IQueryable<Theme> query, int? selectedId)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, selectedId);
		}
		public static IQueryable<Theme> WhereIsActiveOnOrSelected(this IQueryable<Theme> query, DateTime dateTimeUtc, int? selectedId, bool includePending = false)
		{
			int selectedValue = selectedId ?? 0;

			return query.Where(data => data.ThemeId == selectedValue
				||
				(
					(includePending || !data.IsPending)
					&& (data.StartDateTimeUtc < dateTimeUtc)
					&& (data.EndDateTimeUtc > dateTimeUtc)
					&& (includePending || !data.Client.IsPending)
					&& (data.Client.StartDateTimeUtc < dateTimeUtc)
					&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				)
				);
		}

		public static IQueryable<ValueList> WhereIsActive(this IQueryable<ValueList> query)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, 0);
		}
		public static IQueryable<ValueList> WhereIsActiveOrSelected(this IQueryable<ValueList> query, int? selectedId)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, selectedId);
		}
		public static IQueryable<ValueList> WhereIsActiveOnOrSelected(this IQueryable<ValueList> query, DateTime dateTimeUtc, int? selectedId, bool includePending = false)
		{
			int selectedValue = selectedId ?? 0;

			return query.Where(data => data.ValueListId == selectedValue
				||
				(
					(includePending || !data.IsPending)
					&& (data.StartDateTimeUtc < dateTimeUtc)
					&& (data.EndDateTimeUtc > dateTimeUtc)
					&& (includePending || !data.Client.IsPending)
					&& (data.Client.StartDateTimeUtc < dateTimeUtc)
					&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				)
				);
		}


		public static IQueryable<WebForm> WhereIsActive(this IQueryable<WebForm> query)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, 0);
		}
		public static IQueryable<WebForm> WhereIsActiveOrSelected(this IQueryable<WebForm> query, int? selectedId)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, selectedId);
		}
		public static IQueryable<WebForm> WhereIsActiveOnOrSelected(this IQueryable<WebForm> query, DateTime dateTimeUtc, int? selectedId, bool includePending = false)
		{
			int selectedValue = selectedId ?? 0;

			return query.Where(data => data.WebFormId == selectedValue
				||
				(
					(includePending || !data.IsPending)
					&& (data.StartDateTimeUtc < dateTimeUtc)
					&& (data.EndDateTimeUtc > dateTimeUtc)
					&& (includePending || !data.Client.IsPending)
					&& (data.Client.StartDateTimeUtc < dateTimeUtc)
					&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				)
				);
		}

		public static IQueryable<WebFormField> WhereIsActive(this IQueryable<WebFormField> query)
		{
			return query.WhereIsActiveOnOrSelected(DateTime.UtcNow, 0);
		}
		public static IQueryable<WebFormField> WhereIsActiveOnOrSelected(this IQueryable<WebFormField> query, DateTime dateTimeUtc, int? selectedId, bool includePending = false)
		{

			int selectedValue = selectedId ?? 0;

			return query.Where(data => data.WebFormFieldId == selectedValue
				||
				(
					(includePending || !data.IsPending)
					&& (data.StartDateTimeUtc < dateTimeUtc)
					&& (data.EndDateTimeUtc > dateTimeUtc)
					&& (includePending || !data.WebForm.IsPending)
					&& (data.WebForm.StartDateTimeUtc < dateTimeUtc)
					&& (data.WebForm.EndDateTimeUtc > dateTimeUtc)
					&& (includePending || !data.Client.IsPending)
					&& (data.Client.StartDateTimeUtc < dateTimeUtc)
					&& (data.Client.EndDateTimeUtc > dateTimeUtc)
				)
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
		public static bool IsActiveBubble(this PageTemplate pageTemplate)
		{
			if (pageTemplate == null)
			{
				throw new ArgumentNullException("pageTemplate");
			}

			return pageTemplate.IsActiveDirect() && pageTemplate.Client.IsActiveDirect();
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

			return pageTemplateSection.IsActiveDirect() && pageTemplateSection.PageTemplate.IsActiveBubble();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this Theme theme)
		{
			if (theme == null)
			{
				throw new ArgumentNullException("theme");
			}

			return theme.IsActiveDirect() && theme.Client.IsActiveDirect();
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
		public static bool IsActiveBubble(this WebForm webForm)
		{
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}

			return webForm.IsActiveDirect() && webForm.Client.IsActiveDirect();
		}

		/// <summary>
		/// Returns true if store front and client (parent record) are both active
		/// </summary>
		/// <param name="storeFront"></param>
		/// <returns></returns>
		public static bool IsActiveBubble(this WebFormField webFormField)
		{
			if (webFormField == null)
			{
				throw new ArgumentNullException("webFormField");
			}

			return webFormField.IsActiveDirect() && webFormField.WebForm.IsActiveBubble();
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

			if (page.PageId == default(int))
			{
				return false;
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

			if (pageSection.PageSectionId == default(int))
			{
				return false;
			}
			return pageSection.IsActiveDirect() && pageSection.Page.IsActiveBubble()
				&& pageSection.PageTemplateSection.IsActiveDirect()
				&& pageSection.StoreFront.IsActiveBubble();
		}


	}
}