using System;
using System.Linq;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreData
{
	/// <summary>
	/// Applies sort for a controller including sort descending/ascending from ActionSortLink displays
	/// Converts IQueryable to IOrderedQueryable
	/// </summary>
	public static class SortExtensions
	{
		public static IOrderedQueryable<Client> ApplyDefaultSort(this IQueryable<Client> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<Client> ApplySort(this IQueryable<Client> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<Client> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "folder":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Folder);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Folder);
					}
					break;

				case "enablepageviewlog":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EnablePageViewLog);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EnablePageViewLog);
					}
					break;

				case "enablenewuserregisteredbroadcast":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EnableNewUserRegisteredBroadcast);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EnableNewUserRegisteredBroadcast);
					}
					break;

				case "status" :
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "usesendgridemail":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UseSendGridEmail);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UseSendGridEmail);
					}
					break;

				case "usetwiliosms":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UseTwilioSms);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UseTwilioSms);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(c => c.Order).ThenBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(c => c.Order).ThenByDescending(c => c.ClientId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(c => c.Order).ThenBy(c => c.ClientId);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(c => c.Order).ThenByDescending(c => c.ClientId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<StoreFront> ApplyDefaultSort(this IQueryable<StoreFront> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<StoreFront> ApplySort(this IQueryable<StoreFront> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<StoreFront> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "client.ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.IsPending);
					}
					break;

				case "client.startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.StartDateTimeUtc);
					}
					break;

				case "client.enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.EndDateTimeUtc);
					}
					break;

				case "storefrontid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StoreFrontId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StoreFrontId);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(sf => sf.Client.Order).ThenBy(sf => sf.Client.ClientId).ThenBy(sf => sf.Order).ThenBy(sf => sf.StoreFrontId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(sf => sf.Client.Order).ThenByDescending(sf => sf.Client.ClientId).ThenByDescending(sf => sf.Order).ThenByDescending(sf => sf.StoreFrontId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(sf => sf.Client.Order).ThenBy(sf => sf.Client.ClientId).ThenBy(sf => sf.Order).ThenBy(sf => sf.StoreFrontId);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(sf => sf.Client.Order).ThenByDescending(sf => sf.Client.ClientId).ThenByDescending(sf => sf.Order).ThenByDescending(sf => sf.StoreFrontId);
			}
			return orderedQuery;

		}

		public static IOrderedQueryable<StoreBinding> ApplySort(this IQueryable<StoreBinding> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<StoreBinding> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "storefrontid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StoreFront.StoreFrontId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StoreFront.StoreFrontId);
					}
					break;

				case "storefront":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StoreFront.StoreFrontConfigurations.AsQueryable().WhereIsActive().OrderBy(cn => cn.Order).FirstOrDefault().Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StoreFront.StoreFrontConfigurations.AsQueryable().WhereIsActive().OrderBy(cn => cn.Order).FirstOrDefault().Name);
					}
					break;


				case "storefrontstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.StoreFront.IsPending || c.StoreFront.StartDateTimeUtc > DateTime.UtcNow || c.StoreFront.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.StoreFront.IsPending || c.StoreFront.StartDateTimeUtc > DateTime.UtcNow || c.StoreFront.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "hostname":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.HostName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.HostName);
					}
					break;

				case "port":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Port);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Port);
					}
					break;

				case "rootpath":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.RootPath);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.RootPath);
					}
					break;

				case "useurlstorename":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UseUrlStoreName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UseUrlStoreName);
					}
					break;

				case "urlstorename":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UrlStoreName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UrlStoreName);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(sb => sb.Client.Order)
							.ThenBy(sb => sb.Client.ClientId)
							.ThenBy(sb => sb.StoreFront.Order)
							.ThenBy(sb => sb.StoreFrontId)
							.ThenBy(sb => sb.Order)
							.ThenBy(sb => sb.StoreBindingId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(sb => sb.Client.Order)
							.ThenByDescending(sb => sb.Client.ClientId)
							.ThenByDescending(sb => sb.StoreFront.Order)
							.ThenByDescending(sb => sb.StoreFrontId)
							.ThenByDescending(sb => sb.Order)
							.ThenByDescending(sb => sb.StoreBindingId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(sb => sb.Client.Order)
							.ThenBy(sb => sb.Client.ClientId)
							.ThenBy(sb => sb.StoreFront.Order)
							.ThenBy(sb => sb.StoreFrontId)
							.ThenBy(sb => sb.Order)
							.ThenBy(sb => sb.StoreBindingId);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(sb => sb.Client.Order)
					.ThenByDescending(sb => sb.Client.ClientId)
					.ThenByDescending(sb => sb.StoreFront.Order)
					.ThenByDescending(sb => sb.StoreFrontId)
					.ThenByDescending(sb => sb.Order)
					.ThenByDescending(sb => sb.StoreBindingId);
			}
			return orderedQuery;
		}


		/// <summary>
		/// Default sort is by Order, then StoreFrontConfigurationID
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IOrderedQueryable<StoreFrontConfiguration> ApplyDefaultSort(this IQueryable<StoreFrontConfiguration> query)
		{
			return query.ApplySort(null, null, null);
		}

		/// <summary>
		/// Default sort is by Order, then StoreFrontConfigurationID
		/// </summary>
		/// <param name="query"></param>
		/// <param name="controller"></param>
		/// <param name="SortBy"></param>
		/// <param name="SortAscending"></param>
		/// <returns></returns>
		public static IOrderedQueryable<StoreFrontConfiguration> ApplySort(this IQueryable<StoreFrontConfiguration> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<StoreFrontConfiguration> orderedQuery = null;

			if (sortAscending)
			{
				orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
					.ThenBy(c => c.Client.Order)
					.ThenBy(c => c.Client.ClientId)
					.ThenBy(c => c.StoreFront.Order)
					.ThenBy(c => c.StoreFrontId)
					.ThenBy(c => c.Order)
					.ThenBy(c => c.StoreFrontConfigurationId);
			}
			else
			{
				orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
					.ThenByDescending(c => c.Client.Order)
					.ThenByDescending(c => c.Client.ClientId)
					.ThenByDescending(c => c.StoreFront.Order)
					.ThenByDescending(c => c.StoreFrontId)
					.ThenByDescending(c => c.Order)
					.ThenByDescending(c => c.StoreFrontConfigurationId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<UserProfile> ApplyDefaultSort(this IQueryable<UserProfile> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<UserProfile> ApplySort(this IQueryable<UserProfile> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<UserProfile> orderedQuery = null;
			
			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "storefrontid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StoreFront.StoreFrontId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StoreFront.StoreFrontId);
					}
					break;

				case "storefront":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StoreFront.StoreFrontConfigurations.AsQueryable().WhereIsActive().OrderBy(cn => cn.Order).FirstOrDefault().Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StoreFront.StoreFrontConfigurations.AsQueryable().WhereIsActive().OrderBy(cn => cn.Order).FirstOrDefault().Name);
					}
					break;


				case "storefrontstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.StoreFront.IsPending || c.StoreFront.StartDateTimeUtc > DateTime.UtcNow || c.StoreFront.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.StoreFront.IsPending || c.StoreFront.StartDateTimeUtc > DateTime.UtcNow || c.StoreFront.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "storefront.name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StoreFront.StoreFrontConfigurations.AsQueryable()
							.OrderBy(cn => cn.IsPending || cn.StartDateTimeUtc > DateTime.UtcNow || cn.EndDateTimeUtc < DateTime.UtcNow)
							.OrderBy(cn => cn.Order)
							.FirstOrDefault().Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StoreFront.StoreFrontConfigurations.AsQueryable()
							.OrderBy(cn => cn.IsPending || cn.StartDateTimeUtc > DateTime.UtcNow || cn.EndDateTimeUtc < DateTime.UtcNow)
							.OrderBy(cn => cn.Order)
							.FirstOrDefault().Name);
					}
					break;

				case "userprofileid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UserProfileId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UserProfileId);
					}
					break;

				case "username":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UserName);
					}
					break;

				case "email":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Email);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Email);
					}
					break;

				case "fullname":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.FullName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.FullName);
					}
					break;

				case "lastlogondatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.LastLogonDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.LastLogonDateTimeUtc);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.StoreFront.Order)
							.ThenBy(p => p.StoreFrontId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.StoreFront.Order)
							.ThenByDescending(p => p.StoreFrontId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.UserName);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.StoreFront.Order)
					.ThenBy(p => p.StoreFrontId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.UserName);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.StoreFront.Order)
					.ThenByDescending(p => p.StoreFrontId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.UserName);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<ValueList> ApplyDefaultSort(this IQueryable<ValueList> query)
		{
			return query.ApplySort(null, null, null);
		}
		public static IOrderedQueryable<ValueList> ApplySort(this IQueryable<ValueList> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<ValueList> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "description":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Description);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Description);
					}
					break;

				case "listitems":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ValueListItems.Count);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ValueListItems.Count);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "valuelistid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.ValueListId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.ValueListId);
					}
					break;


				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.ValueListId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.ValueListId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.ValueListId);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.ValueListId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<ValueListItem> ApplyDefaultSort(this IQueryable<ValueListItem> query)
		{
			return query.ApplySort(null, null, null);
		}
		public static IOrderedQueryable<ValueListItem> ApplySort(this IQueryable<ValueListItem> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<ValueListItem> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "valuelistid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.ValueListId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.ValueListId);
					}
					break;


				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(vl => vl.ValueList.Order)
							.ThenBy(vl => vl.ValueList.ValueListId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.ValueListItemId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(vl => vl.ValueList.Order)
							.ThenByDescending(vl => vl.ValueList.ValueListId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.ValueListItemId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(vl => vl.ValueList.Order)
					.ThenBy(vl => vl.ValueList.ValueListId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.ValueListItemId);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(vl => vl.ValueList.Order)
					.ThenByDescending(vl => vl.ValueList.ValueListId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.ValueListItemId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<WebForm> ApplyDefaultSort(this IQueryable<WebForm> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<WebForm> ApplySort(this IQueryable<WebForm> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<WebForm> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "title":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Title);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Title);
					}
					break;

				case "webformid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.WebFormId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.WebFormId);
					}
					break;


				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;
				case "responses":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.WebFormResponses.Count);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.WebFormResponses.Count);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.WebFormId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.WebFormId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.WebFormId);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.WebFormId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<WebFormField>ApplySortDefault(this IQueryable<WebFormField> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<WebFormField> ApplySort(this IQueryable<WebFormField> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<WebFormField> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "webformid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.WebFormId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.WebFormId);
					}
					break;

				case "webformfieldid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.WebFormFieldId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.WebFormFieldId);
					}
					break;

				case "valuelist":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => (vl.ValueList == null ? "" : vl.ValueList.Name) );
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => (vl.ValueList == null ? "" : vl.ValueList.Name));
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "labeltext":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.LabelText);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.LabelText);
					}
					break;

				case "datatypestring":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.DataTypeString);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.DataTypeString);
					}
					break;

				case "isrequired":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsRequired);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsRequired);
					}
					break;

				case "webformfieldresponses":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.WebFormFieldResponses.Count);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.WebFormFieldResponses.Count);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.WebForm.Order)
							.ThenBy(p => p.WebFormId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.WebFormFieldId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.WebForm.Order)
							.ThenByDescending(p => p.WebFormId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.WebFormFieldId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				return orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.WebForm.Order)
					.ThenBy(p => p.WebFormId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.WebFormFieldId);
			}
			else if (!defaultSort && !sortAscending)
			{
				return orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.WebForm.Order)
					.ThenByDescending(p => p.WebFormId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.WebFormFieldId);
			}
			return orderedQuery;
		}


		public static IOrderedQueryable<Page> ApplyDefaultSort(this IQueryable<Page> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<Page> ApplySort(this IQueryable<Page> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<Page> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "pagetemplate.name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.PageTemplate.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.PageTemplate.Name);
					}
					break;

				case "theme.name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Theme.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Theme.Name);
					}
					break;

				case "webform.name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => (p.WebForm == null ? string.Empty : p.WebForm.Name));
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => (p.WebForm == null ? string.Empty : p.WebForm.Name));
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "pageid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.PageId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.PageId);
					}
					break;

				case "pagetitle":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.PageTitle);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.PageTitle);
					}
					break;

				case "url":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(vl => vl.Url);
					}
					else
					{
						orderedQuery = query.OrderByDescending(vl => vl.Url);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p=> p.StoreFront.Order)
							.ThenBy(p=> p.StoreFront.StoreFrontId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.PageId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.StoreFront.Order)
							.ThenByDescending(p => p.StoreFront.StoreFrontId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.PageId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.StoreFront.Order)
					.ThenBy(p => p.StoreFront.StoreFrontId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.PageId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.StoreFront.Order)
					.ThenByDescending(p => p.StoreFront.StoreFrontId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.PageId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<PageTemplate> ApplyDefaultSort(this IQueryable<PageTemplate> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<PageTemplate> ApplySort(this IQueryable<PageTemplate> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<PageTemplate> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;


				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "pagetemplateid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.PageTemplateId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.PageTemplateId);
					}
					break;

				case "viewname":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ViewName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ViewName);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "sections":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Sections.Count);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Sections.Count);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.PageTemplateId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.PageTemplateId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.PageTemplateId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.OrderByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.PageTemplateId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<NavBarItem> ApplyDefaultSort(this IQueryable<NavBarItem> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<NavBarItem> ApplySort(this IQueryable<NavBarItem> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<NavBarItem> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;


				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "navbaritemid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.NavBarItemId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.NavBarItemId);
					}
					break;

				case "parentnavbaritemid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ParentNavBarItemId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ParentNavBarItemId);
					}
					break;

				case "parentnavbaritem.order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.ParentNavBarItem == null ? -1 : c.ParentNavBarItem.Order));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.ParentNavBarItem == null ? -1 : c.ParentNavBarItem.Order));
					}
					break;

				case "parentnavbaritem.name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.ParentNavBarItem == null ? string.Empty : c.ParentNavBarItem.Name));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.ParentNavBarItem == null ? string.Empty : c.ParentNavBarItem.Name));
					}
					break;

				case "page":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Page.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Page.Name);
					}
					break;

				case "forregisteredonly":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ForRegisteredOnly);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ForRegisteredOnly);
					}
					break;

				case "foranonymousonly":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ForAnonymousOnly);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ForAnonymousOnly);
					}
					break;


				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.NavBarItemId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.NavBarItemId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.NavBarItemId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.OrderByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.NavBarItemId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<Theme> ApplyDefaultSort(this IQueryable<Theme> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<Theme> ApplySort(this IQueryable<Theme> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<Theme> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "clientid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ClientId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ClientId);
					}
					break;

				case "client":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Client.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Client.Name);
					}
					break;

				case "clientstatus":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.Client.IsPending || c.Client.StartDateTimeUtc > DateTime.UtcNow || c.Client.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;


				case "themeid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ThemeId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ThemeId);
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "foldername":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.FolderName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.FolderName);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "ispending":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.IsPending);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.IsPending);
					}
					break;

				case "startdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.StartDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.StartDateTimeUtc);
					}
					break;

				case "enddatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.EndDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.EndDateTimeUtc);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.ThemeId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow))
							.ThenByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.ThemeId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.ThemeId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.OrderByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.ThemeId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<CartBundle> ApplyDefaultSort(this IQueryable<CartBundle> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<CartBundle> ApplySort(this IQueryable<CartBundle> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<CartBundle> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;


				case "quantity":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Quantity);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Quantity);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(c => c.ProductBundle.Order)
							.ThenBy(c => c.ProductBundle.ProductBundleId)
							.ThenBy(p => p.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(c => c.ProductBundle.Order)
							.ThenByDescending(c => c.ProductBundle.ProductBundleId)
							.ThenByDescending(p => p.CreateDateTimeUtc);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(c => c.ProductBundle.Order)
					.ThenBy(c => c.ProductBundle.ProductBundleId)
					.ThenBy(p => p.CreateDateTimeUtc);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.OrderByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(c => c.ProductBundle.Order)
					.ThenByDescending(c => c.ProductBundle.ProductBundleId)
					.ThenByDescending(p => p.CreateDateTimeUtc);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<OrderBundle> ApplyDefaultSort(this IQueryable<OrderBundle> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<OrderBundle> ApplySort(this IQueryable<OrderBundle> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<OrderBundle> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;


				case "quantity":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Quantity);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Quantity);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(c => c.ProductBundle.Order)
							.ThenBy(c => c.ProductBundle.ProductBundleId)
							.ThenBy(p => p.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(c => c.ProductBundle.Order)
							.ThenByDescending(c => c.ProductBundle.ProductBundleId)
							.ThenByDescending(p => p.CreateDateTimeUtc);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(c => c.ProductBundle.Order)
					.ThenBy(c => c.ProductBundle.ProductBundleId)
					.ThenBy(p => p.CreateDateTimeUtc);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.OrderByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(c => c.ProductBundle.Order)
					.ThenByDescending(c => c.ProductBundle.ProductBundleId)
					.ThenByDescending(p => p.CreateDateTimeUtc);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<CartItem> ApplyDefaultSort(this IQueryable<CartItem> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<CartItem> ApplySort(this IQueryable<CartItem> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<CartItem> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "productid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ProductId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ProductId);
					}
					break;

				case "product.name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Product.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Product.Name);
					}
					break;

				case "product.order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Product.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Product.Order);
					}
					break;

				case "quantity":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Quantity);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Quantity);
					}
					break;

				case "linetotal":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.LineTotal);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.LineTotal);
					}
					break;

				case "linediscount":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.LineDiscount);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.LineDiscount);
					}
					break;

				case "unitprice":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UnitPrice);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UnitPrice);
					}
					break;

				case "unitpriceext":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UnitPriceExt);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UnitPriceExt);
					}
					break;

				case "listprice":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ListPrice);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ListPrice);
					}
					break;

				case "listpriceext":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ListPriceExt);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ListPriceExt);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
							.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
							.ThenBy(p => p.CreateDateTimeUtc)
							.ThenBy(p => p.Product.ProductId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
							.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
							.ThenByDescending(p => p.CreateDateTimeUtc)
							.ThenByDescending(p => p.ProductId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
					.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
					.ThenBy(p => p.CreateDateTimeUtc)
					.ThenBy(p => p.ProductId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.OrderByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
					.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
					.ThenByDescending(p => p.CreateDateTimeUtc)
					.ThenByDescending(p => p.ProductId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<OrderItem> ApplyDefaultSort(this IQueryable<OrderItem> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<OrderItem> ApplySort(this IQueryable<OrderItem> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<OrderItem> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "productid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ProductId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ProductId);
					}
					break;

				case "product.name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Product.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Product.Name);
					}
					break;

				case "product.order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Product.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Product.Order);
					}
					break;

				case "quantity":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Quantity);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Quantity);
					}
					break;

				case "linetotal":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.LineTotal);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.LineTotal);
					}
					break;

				case "linediscount":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.LineDiscount);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.LineDiscount);
					}
					break;

				case "unitprice":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UnitPrice);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UnitPrice);
					}
					break;

				case "unitpriceext":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UnitPriceExt);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UnitPriceExt);
					}
					break;

				case "listprice":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ListPrice);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ListPrice);
					}
					break;

				case "listpriceext":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ListPriceExt);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ListPriceExt);
					}
					break;

				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
							.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
							.ThenBy(c => c.Product.Order)
							.ThenBy(p => p.Product.ProductId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
							.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
							.ThenByDescending(c => c.Product.Order)
							.ThenByDescending(p => p.Product.ProductId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
					.ThenBy(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
					.ThenBy(c => c.Product.Order)
					.ThenBy(p => p.Product.ProductId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.OrderByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.ProductBundle.Order : 9999999)
					.ThenByDescending(c => c.ProductBundleItemId.HasValue ? c.ProductBundleItem.Order : 9999999)
					.ThenByDescending(c => c.Product.Order)
					.ThenByDescending(p => p.Product.ProductId);
			}
			return orderedQuery;
		}

		public static IOrderedQueryable<Product> ApplyDefaultSort(this IQueryable<Product> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<Product> ApplySort(this IQueryable<Product> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<Product> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "productid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ProductId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ProductId);
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "urlname":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UrlName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UrlName);
					}
					break;

				case "productcategoryid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ProductCategoryId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ProductCategoryId);
					}
					break;

				case "category":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Category.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Category.Name);
					}
					break;


				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.Category.Order)
							.ThenBy(p => p.Category.ProductCategoryId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.ProductId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.Category.Order)
							.ThenByDescending(p => p.Category.ProductCategoryId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.ProductId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.Category.Order)
					.ThenBy(p => p.Category.ProductCategoryId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.ProductId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.Category.Order)
					.ThenByDescending(p => p.Category.ProductCategoryId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.ProductId);
			}
			return orderedQuery;
		}


		public static IOrderedQueryable<ProductBundle> ApplyDefaultSort(this IQueryable<ProductBundle> query)
		{
			return query.ApplySort(null, null, null);
		}

		public static IOrderedQueryable<ProductBundle> ApplySort(this IQueryable<ProductBundle> query, GStoreData.ControllerBase.BaseController controller, string SortBy, bool? SortAscending)
		{
			string sortBy = (string.IsNullOrEmpty(SortBy) ? string.Empty : SortBy.Trim().ToLower());
			bool sortAscending = (SortAscending.HasValue ? SortAscending.Value : true);
			IOrderedQueryable<ProductBundle> orderedQuery = null;

			bool defaultSort = false;
			switch (sortBy ?? string.Empty)
			{
				case "productbundleid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ProductBundleId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ProductBundleId);
					}
					break;

				case "name":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Name);
					}
					break;

				case "order":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Order);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Order);
					}
					break;

				case "updatedby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdatedBy.UserName);
					}
					break;

				case "updatedatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UpdateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UpdateDateTimeUtc);
					}
					break;

				case "createdby":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreatedBy.UserName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreatedBy.UserName);
					}
					break;

				case "createdatetimeutc":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.CreateDateTimeUtc);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.CreateDateTimeUtc);
					}
					break;

				case "status":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => (c.IsPending || c.StartDateTimeUtc > DateTime.UtcNow || c.EndDateTimeUtc < DateTime.UtcNow));
					}
					break;

				case "urlname":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.UrlName);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.UrlName);
					}
					break;

				case "productcategoryid":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.ProductCategoryId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.ProductCategoryId);
					}
					break;

				case "category":
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(c => c.Category.Name);
					}
					else
					{
						orderedQuery = query.OrderByDescending(c => c.Category.Name);
					}
					break;


				case "":
					//default sort
					defaultSort = true;
					if (sortAscending)
					{
						orderedQuery = query.OrderBy(p => p.Client.Order)
							.ThenBy(p => p.ClientId)
							.ThenBy(p => p.Order)
							.ThenBy(p => p.ProductBundleId);
					}
					else
					{
						orderedQuery = query.OrderByDescending(p => p.Client.Order)
							.ThenByDescending(p => p.ClientId)
							.ThenByDescending(p => p.Order)
							.ThenByDescending(p => p.ProductBundleId);
					}
					break;


				default:
					//unknown sort
					if (controller != null)
					{
						System.Diagnostics.Trace.WriteLine("Unknown sort: " + SortBy);
						controller.AddUserMessage("Unknown sort", "Unknown sort: " + SortBy.ToHtml(), AppHtmlHelpers.UserMessageType.Info);
					}
					goto case "";
			}

			if (!defaultSort && sortAscending)
			{
				orderedQuery = orderedQuery.ThenBy(p => p.Client.Order)
					.ThenBy(p => p.ClientId)
					.ThenBy(p => p.Order)
					.ThenBy(p => p.ProductBundleId);
			}
			else if (!defaultSort && !sortAscending)
			{
				orderedQuery = orderedQuery.ThenByDescending(p => p.Client.Order)
					.ThenByDescending(p => p.ClientId)
					.ThenByDescending(p => p.Order)
					.ThenByDescending(p => p.ProductBundleId);
			}
			return orderedQuery;
		}


		public static IOrderedQueryable<ProductBundleItem> ApplyDefaultSort(this IQueryable<ProductBundleItem> query)
		{
			return query
				.OrderBy(p => (p.IsPending || (p.StartDateTimeUtc > DateTime.UtcNow) || (p.EndDateTimeUtc < DateTime.UtcNow)))
				.ThenBy(p => p.Order)
				.ThenBy(p => p.ProductBundleItemId);
		}

		public static IOrderedQueryable<ProductCategory> ApplyDefaultSort(this IQueryable<ProductCategory> query)
		{
			return query.OrderBy(p => p.Order).ThenBy(p => p.ProductCategoryId);
		}

		public static IOrderedQueryable<Discount> ApplyDefaultSort(this IQueryable<Discount> query)
		{
			return query.OrderBy(p => p.Order).ThenBy(p => p.Code).ThenBy(p=>p.DiscountId);
		}

		public static IOrderedQueryable<GiftCard> ApplyDefaultSort(this IQueryable<GiftCard> query)
		{
			return query.OrderBy(p => p.Order).ThenBy(p => p.Code).ThenBy(p => p.GiftCardId);
		}

	}
}