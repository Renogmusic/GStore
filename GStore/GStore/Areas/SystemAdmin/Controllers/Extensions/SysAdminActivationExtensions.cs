using GStore.Models;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GStore.AppHtmlHelpers;

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
			controller.AddUserMessage("Activated Client", "Activated Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

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
			controller.AddUserMessage("Activated Store Front", "Activated Store Front '" + (storeFront.CurrentConfig() == null ? "" : storeFront.CurrentConfig().Name.ToHtml()) + "' [" + storeFront.ClientId + "]" + " - Client '" + storeFront.Client.Name.ToHtml() + "' [" + storeFront.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

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
				+ " Store Front: '" + binding.StoreFront.CurrentConfigOrAny().Name.ToHtml() + "' [" + binding.StoreFront.StoreFrontId + "]" + " - Client '" + binding.Client.Name.ToHtml() + "' [" + binding.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivateStoreFrontConfigOnly(this BaseClasses.SystemAdminBaseController controller, int storeConfigId)
		{
			StoreFrontConfiguration config = controller.GStoreDb.StoreFrontConfigurations.FindById(storeConfigId);
			if (config == null)
			{
				controller.AddUserMessage("Activate Store Config Failed!", "Store Config not found by id: " + storeConfigId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (config.IsActiveDirect())
			{
				controller.AddUserMessage("Store Config is already active.", "Store Config is already active. id: " + storeConfigId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			config.IsPending = false;
			config.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			config.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			config = controller.GStoreDb.StoreFrontConfigurations.Update(config);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Store Config", "Activated Store Config '" + config.ConfigurationName + "' [" + config.StoreFrontConfigurationId + "] for Store Front: '" + config.Name.ToHtml() + "' [" + config.StoreFrontId + "]" + " - Client '" + config.Client.Name.ToHtml() + "' [" + config.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}


		public static bool ActivateStoreFrontClientBindingAndConfig(this BaseClasses.SystemAdminBaseController controller, StoreBinding binding)
		{
			if (controller == null)
			{
				throw new NullReferenceException("controller");
			}

			if (binding.StoreFront.CurrentConfigOrAny() == null)
			{
				controller.AddUserMessage("Store Front Config Not Found", "No Configuration was found for Store Front Id: " + binding.StoreFrontId, AppHtmlHelpers.UserMessageType.Warning);
			}
			else
			{
				StoreFrontConfiguration config = binding.StoreFront.CurrentConfigOrAny();
				if (!config.IsActiveDirect())
				{
					config.IsPending = false;
					config.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
					config.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
					config = controller.GStoreDb.StoreFrontConfigurations.Update(config);
					controller.GStoreDb.SaveChanges();
					controller.AddUserMessage("Activated Store Config", "Activated Store Front Configuration '" + config.ConfigurationName.ToHtml() + "' [" + config.StoreFrontConfigurationId +"]", AppHtmlHelpers.UserMessageType.Info);
				}
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
				controller.AddUserMessage("Activated Client", "Activated Client '" + client.Name.ToHtml() + "' [" + client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);
			}

			if (!binding.StoreFront.IsActiveDirect())
			{
				StoreFront storeFront = binding.StoreFront;
				storeFront.IsPending = false;
				storeFront.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				storeFront.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				controller.GStoreDb.StoreFronts.Update(storeFront);
				controller.GStoreDb.SaveChanges();
				controller.AddUserMessage("Activated Store Front", "Activated Store Front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);
			}

			return true;

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
			controller.AddUserMessage("Activated User Profile", "Activated User Profile '" + userProfile.UserName.ToHtml() + "' [" + userProfile.UserProfileId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;

		}

		public static bool ActivatePageTemplate(this BaseClasses.SystemAdminBaseController controller, int pageTemplateId)
		{
			PageTemplate pageTemplate = controller.GStoreDb.PageTemplates.FindById(pageTemplateId);
			if (pageTemplate == null)
			{
				controller.AddUserMessage("Activate Page Template Failed!", "Page Template not found by id: " + pageTemplateId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (pageTemplate.IsActiveDirect())
			{
				controller.AddUserMessage("Page Template is already active.", "Page Template is already active. id: " + pageTemplateId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			pageTemplate.IsPending = false;
			pageTemplate.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplate.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.PageTemplates.Update(pageTemplate);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Page Template", "Activated Page Template '" + pageTemplate.Name.ToHtml() + "' [" + pageTemplate.PageTemplateId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivatePageTemplateSection(this BaseClasses.SystemAdminBaseController controller, int pageTemplateSectionId)
		{
			PageTemplateSection pageTemplateSection = controller.GStoreDb.PageTemplateSections.FindById(pageTemplateSectionId);
			if (pageTemplateSection == null)
			{
				controller.AddUserMessage("Activate Page Template Section Failed!", "Page Template Section not found by id: " + pageTemplateSectionId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (pageTemplateSection.IsActiveDirect())
			{
				controller.AddUserMessage("Page Template Section is already active.", "Page Template Section is already active. id: " + pageTemplateSectionId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			pageTemplateSection.IsPending = false;
			pageTemplateSection.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			pageTemplateSection.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.PageTemplateSections.Update(pageTemplateSection);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Page Template Section", "Activated Page Template Section '" + pageTemplateSection.Name.ToHtml() + "' [" + pageTemplateSection.PageTemplateId + "] - Page Template '" + pageTemplateSection.PageTemplate.Name.ToHtml() + "' [" + pageTemplateSection.PageTemplate.PageTemplateId + "]", AppHtmlHelpers.UserMessageType.Info);

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
			controller.AddUserMessage("Activated Value List", "Activated Value List '" + valueList.Name.ToHtml() + "' [" + valueList.ValueListId + "]" + " - Client '" + valueList.Client.Name.ToHtml() + "' [" + valueList.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

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
			controller.AddUserMessage("Activated Value List Item", "Activated Value List Item '" + valueListItem.Name.ToHtml() + "' [" + valueListItem.ValueListItemId + "]" + " - Value List '" + valueListItem.ValueList.Name.ToHtml() + "' [" + valueListItem.ValueList.ValueListId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivateWebForm(this BaseClasses.SystemAdminBaseController controller, int webFormId)
		{
			WebForm webForm = controller.GStoreDb.WebForms.FindById(webFormId);
			if (webForm == null)
			{
				controller.AddUserMessage("Activate Web Form Failed!", "Web Form not found by id: " + webFormId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (webForm.IsActiveDirect())
			{
				controller.AddUserMessage("Web Form is already active.", "Web Form is already active. id: " + webFormId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			webForm.IsPending = false;
			webForm.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			webForm.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.WebForms.Update(webForm);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Web Form", "Activated Web Form '" + webForm.Name.ToHtml() + "' [" + webForm.WebFormId + "]" + " - Client '" + webForm.Client.Name.ToHtml() + "' [" + webForm.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivateWebFormField(this BaseClasses.SystemAdminBaseController controller, int webFormFieldId)
		{
			WebFormField webFormField = controller.GStoreDb.WebFormFields.FindById(webFormFieldId);
			if (webFormField == null)
			{
				controller.AddUserMessage("Activate Web Form Field Failed!", "Web Form Field not found by id: " + webFormFieldId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (webFormField.IsActiveDirect())
			{
				controller.AddUserMessage("Web Form Field is already active.", "Web Form Field is already active. id: " + webFormFieldId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			webFormField.IsPending = false;
			webFormField.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			webFormField.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.WebFormFields.Update(webFormField);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Web Form Field", "Activated Web Form Field '" + webFormField.Name.ToHtml() + "' [" + webFormField.WebFormFieldId + "]" + " - Web Form '" + webFormField.WebForm.Name.ToHtml() + "' [" + webFormField.WebForm.WebFormId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static bool ActivateTheme(this BaseClasses.SystemAdminBaseController controller, int themeId)
		{
			Theme theme = controller.GStoreDb.Themes.FindById(themeId);
			if (theme == null)
			{
				controller.AddUserMessage("Activate Theme Failed!", "Theme not found by id: " + themeId, AppHtmlHelpers.UserMessageType.Danger);
				return false;
			}

			if (theme.IsActiveDirect())
			{
				controller.AddUserMessage("Theme is already active.", "Theme is already active. id: " + themeId, AppHtmlHelpers.UserMessageType.Info);
				return false;
			}

			theme.IsPending = false;
			theme.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			theme.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			controller.GStoreDb.Themes.Update(theme);
			controller.GStoreDb.SaveChanges();
			controller.AddUserMessage("Activated Theme", "Activated Theme '" + theme.Name.ToHtml() + "' [" + theme.ThemeId + "]" + " - Client '" + theme.Client.Name.ToHtml() + "' [" + theme.Client.ClientId + "]", AppHtmlHelpers.UserMessageType.Info);

			return true;
		}

		public static StoreBinding AutoMapBindingSeedBestGuessStoreFront(this BaseClasses.SystemAdminBaseController controller)
		{

			StoreBinding binding = controller.GStoreDb.AutoMapBinding(controller);

			StoreFront storeFront = binding.StoreFront;
			string message = string.Empty;
			if (Settings.AppEnableBindingAutoMapCatchAll)
			{
				message = " to catch-all binding";
			}
			else
			{
				message = " to current Url";
			}
			controller.AddUserMessage("AutoMapBindingSeedBestGuessStoreFront Success!", "Auto-mapped" + message.ToHtml() + " store binding to best guess store front '" + storeFront.CurrentConfig().Name.ToHtml() + "' [" + storeFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);

			return binding;
		}

		/// <summary>
		/// Creates storefront folders if they don't exist. Uses physical file path as BasePath parameter
		/// </summary>
		/// <param name="basePath"></param>
		public static void CreateStoreFrontFolders(string basePath)
		{
			CreateFolderIfNotExists(basePath + "\\ErrorPages");
			CreateFolderIfNotExists(basePath + "\\Fonts");
			CreateFolderIfNotExists(basePath + "\\Forms");
			CreateFolderIfNotExists(basePath + "\\Images");
			CreateFolderIfNotExists(basePath + "\\Pages");
			CreateFolderIfNotExists(basePath + "\\Scripts");
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
			CreateFolderIfNotExists(basePath + "\\Pages");
			CreateFolderIfNotExists(basePath + "\\Scripts");
			CreateFolderIfNotExists(basePath + "\\StoreFronts");
			CreateFolderIfNotExists(basePath + "\\Styles");
		}

	}
}