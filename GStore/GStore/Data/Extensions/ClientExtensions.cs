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
			client.IsPending = true;
			client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			client.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			client.EnableNewUserRegisteredBroadcast = true;
			client.EnablePageViewLog = true;
			client.UseSendGridEmail = false;
			client.UseTwilioSms = false;
		}

		public static bool IsActiveDirect(this Models.BaseClasses.ClientLiveRecord record)
		{
			return record.IsActiveDirect(DateTime.UtcNow);
		}
		public static bool IsActiveDirect(this Models.BaseClasses.ClientLiveRecord record, DateTime dateTime)
		{
			if (!record.IsPending && (record.StartDateTimeUtc < dateTime) && (record.EndDateTimeUtc > dateTime))
			{
				return true;
			}
			return false;
		}


	}
}