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
				+ " Store Front: '" + binding.StoreFront.Name + "' [" + binding.StoreFront.StoreFrontId  + "]" + " - Client '" + binding.Client.Name + "' [" + binding.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

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

		public static bool ActivateUserProfile(this BaseClasses.SystemAdminBaseController controller, int userProfileId)
		{
			UserProfile userProfile = controller.GStoreDb.UserProfiles.FindById(userProfileId);
			if (userProfile == null)
			{
				controller.AddUserMessage("User Profile Activation Failed!", "User Profile not found by id: " + userProfileId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (userProfile.IsActiveDirect())
			{
				controller.AddUserMessage("User Profile is already active.", "User Profile is already active. id: " + userProfileId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			userProfile.IsPending = false;
			userProfile.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			userProfile.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.UserProfiles.Update(userProfile);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated User Profile", "Activated User Profile '" + userProfile.UserName + "' [" + userProfile.UserProfileId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;

		}

		public static bool ActivateValueList(this BaseClasses.SystemAdminBaseController controller, int valueListId)
		{
			ValueList valueList = controller.GStoreDb.ValueLists.FindById(valueListId);
			if (valueList == null)
			{
				controller.AddUserMessage("Activate Value List Failed!", "Value List not found by id: " + valueListId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (valueList.IsActiveDirect())
			{
				controller.AddUserMessage("Value List is already active.", "Value List is already active. id: " + valueListId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			valueList.IsPending = false;
			valueList.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			valueList.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.ValueLists.Update(valueList);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Value List", "Activated Value List '" + valueList.Name + "' [" + valueList.ValueListId + "]" + " - Client '" + valueList.Client.Name + "' [" + valueList.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivateValueListItem(this BaseClasses.SystemAdminBaseController controller, int valueListItemId)
		{
			ValueListItem valueListItem = controller.GStoreDb.ValueListItems.FindById(valueListItemId);
			if (valueListItem == null)
			{
				controller.AddUserMessage("Activate Value List Item Failed!", "Value List Item not found by id: " + valueListItemId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (valueListItem.IsActiveDirect())
			{
				controller.AddUserMessage("Value List Item is already active.", "Value List Item is already active. id: " + valueListItemId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			valueListItem.IsPending = false;
			valueListItem.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			valueListItem.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.ValueListItems.Update(valueListItem);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Value List Item", "Activated Value List Item '" + valueListItem.Name + "' [" + valueListItem.ValueListItemId + "]" + " - Value List '" + valueListItem.ValueList.Name + "' [" + valueListItem.ValueList.ValueListId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static void CreateStoreFrontFolders(string basePath)
		{
			CreateFolderIfNotExists(basePath + "\\ErrorPages");
			CreateFolderIfNotExists(basePath + "\\Fonts");
			CreateFolderIfNotExists(basePath + "\\Images");
			CreateFolderIfNotExists(basePath + "\\Scripts");
			CreateFolderIfNotExists(basePath + "\\StoreFronts");
			CreateFolderIfNotExists(basePath + "\\Styles");
		}

		/// <summary>
		/// Creates a folder if it does not exist
		/// </summary>
		/// <param name="folder"></param>
		public static void CreateFolderIfNotExists(string folderPath)
		{
			if (!System.IO.Directory.Exists(folderPath))
			{
				System.IO.Directory.CreateDirectory(folderPath);
				System.Diagnostics.Trace.WriteLine("--File System: Created folder: " + folderPath);
			}
		}

		public static void CreateClientFolders(string basePath)
		{
			CreateFolderIfNotExists(basePath + "\\ErrorPages");
			CreateFolderIfNotExists(basePath + "\\Fonts");
			CreateFolderIfNotExists(basePath + "\\Images");
			CreateFolderIfNotExists(basePath + "\\Scripts");
			CreateFolderIfNotExists(basePath + "\\StoreFronts");
			CreateFolderIfNotExists(basePath + "\\Styles");
		}

	}
}