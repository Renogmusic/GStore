using GStore.Models;
using System;

namespace GStore.Data
{
	public static class ClientExtensions
	{
		public static bool IsActiveDirect(this Client client)
		{
			return client.IsActiveDirect(DateTime.UtcNow);
		}

		public static bool IsActiveDirect(this Client client, DateTime dateTimeUtc)
		{
			if ((!client.IsPending) && (client.StartDateTimeUtc < dateTimeUtc) && (client.EndDateTimeUtc > dateTimeUtc))
			{
				return true;
			}
			return false;
		}

		public static string ClientVirtualDirectoryToMap(this Client client)
		{
			return "/Content/Clients/" + System.Web.HttpUtility.UrlEncode(client.Folder);
		}

		public static void SetDefaultsForNew(this Client client)
		{
			client.Name = "New Client " + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
			client.Folder = client.Name;
			client.IsPending = false;
			client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			client.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			client.EnableNewUserRegisteredBroadcast = true;
			client.EnablePageViewLog = true;
			client.UseSendGridEmail = false;
			client.UseTwilioSms = false;
		}

		public static void SetDefaultsForNew(this ValueList valueList, int? clientId)
		{
			valueList.AllowDelete = true;
			valueList.AllowEdit = true;
			valueList.IsMultiSelect = true;
			if (clientId.HasValue)
			{
				valueList.ClientId = clientId.Value;
			}
			valueList.IsPending = false;
			valueList.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			valueList.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

		public static void SetDefaultsForNew(this ValueListItem valueListItem, ValueList valueList)
		{
			if (valueList != null)
			{
				valueListItem.ValueList = valueList;
				valueListItem.ValueListId = valueList.ValueListId;
				valueListItem.ClientId = valueList.ClientId;
			}

			valueListItem.IsPending = false;
			valueListItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			valueListItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
		}

	}
}