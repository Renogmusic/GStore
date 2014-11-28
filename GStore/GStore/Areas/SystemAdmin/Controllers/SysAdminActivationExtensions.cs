using GStore.Models;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GStore.Areas.SystemAdmin.Controllers
{
	/// <summary>
	/// Runs activation commands for a controller
	/// </summary>
	public static class SysAdminActivationExtensions
	{
		public static bool ActivateClientOnly(this BaseClasses.SystemAdminBaseController controller, int clientId)
		{
			Client client = controller.GStoreDb.Clients.FindById(clientId);
			if (client == null)
			{
				controller.AddUserMessage("Activate Client Failed!", "Client not found by id: " + clientId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (client.IsActiveDirect())
			{
				controller.AddUserMessage("Client is already active.", "Client is already active. id: " + clientId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			client.IsPending = false;
			client.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.Clients.Update(client);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Client", "Activated Client '" + client.Name + "' [" + client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivateStoreFrontOnly(this BaseClasses.SystemAdminBaseController controller, int storeFrontId)
		{
			StoreFront storeFront = controller.GStoreDb.StoreFronts.FindById(storeFrontId);
			if (storeFront == null)
			{
				controller.AddUserMessage("Activate Store Front Failed!", "Store Front not found by id: " + storeFrontId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (storeFront.IsActiveDirect())
			{
				controller.AddUserMessage("Store Front is already active.", "Store Front is already active. id: " + storeFrontId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			storeFront.IsPending = false;
			storeFront.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			storeFront.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.StoreFronts.Update(storeFront);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Store Front", "Activated Store Front '" + storeFront.Name + "' [" + storeFront.ClientId + "]" + " - Client '" + storeFront.Client.Name + "' [" + storeFront.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivateStoreBindingOnly(this BaseClasses.SystemAdminBaseController controller, int storeBindingId)
		{
			StoreBinding binding = controller.GStoreDb.StoreBindings.FindById(storeBindingId);
			if (binding == null)
			{
				controller.AddUserMessage("Activate Store Binding Failed!", "Store Binding not found by id: " + storeBindingId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (binding.IsActiveDirect())
			{
				controller.AddUserMessage("Store Binding is already active.", "Store Binding is already active. id: " + storeBindingId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			binding.IsPending = false;
			binding.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			binding.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.StoreBindings.Update(binding);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Store Front", "Activated Store Binding [" + binding.StoreBindingId + "] "
				+ " Store Front: '" + binding.StoreFront.Name + "' [" + binding.StoreFront.ClientId + "]" + " - Client '" + binding.Client.Name + "' [" + binding.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}


		public static bool ActivateStoreFrontClientAndBinding(this BaseClasses.SystemAdminBaseController controller, StoreBinding binding)
		{
			if (controller == null)
			{
				throw new NullReferenceException("controller");
			}

			if (!binding.IsActiveDirect())
			{
				binding.IsPending = false;
				binding.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				binding.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				controller.GStoreDb.StoreBindings.Update(binding);
				controller.GStoreDb.SaveChanges();
				controller.AddUserMessage("Activated Store Binding", "Activated Store Binding Id: " + binding.StoreBindingId, AppHtmlHelpers.UserMessageType.Info);
			}

			if (!binding.Client.IsActiveDirect())
			{
				Client client = binding.Client;
				client.IsPending = false;
				client.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				client.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				controller.GStoreDb.Clients.Update(client);
				controller.GStoreDb.SaveChanges();
				controller.AddUserMessage("Activated Client", "Activated Client '" + client.Name + "' [" + client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);
			}

			if (!binding.StoreFront.IsActiveDirect())
			{
				StoreFront storeFront = binding.StoreFront;
				storeFront.IsPending = false;
				storeFront.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				storeFront.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				controller.GStoreDb.StoreFronts.Update(storeFront);
				controller.GStoreDb.SaveChanges();
				controller.AddUserMessage("Activated Store Front", "Activated Store Front '" + storeFront.Name + "' [" + storeFront.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);
			}

			return true;

		}

		public static StoreBinding AutoMapBindingSeedBestGuessStoreFront(this BaseClasses.SystemAdminBaseController controller)
		{

			StoreBinding binding = controller.GStoreDb.AutoMapBinding(controller);

			StoreFront storeFront = binding.StoreFront;
			string message = string.Empty;
			if (Properties.Settings.Current.AppEnableBindingAutoMapCatchAll)
			{
				message = " to catch-all binding";
			}
			else
			{
				message = " to current Url";
			}
			controller.AddUserMessage("AutoMapBindingSeedBestGuessStoreFront Success!", "Auto-mapped" + message + " store binding to best guess store front '" + storeFront.Name + "' [" + storeFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);

			return binding;
		}

	}
}