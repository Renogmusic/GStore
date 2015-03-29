using System;
using System.Collections.Generic;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.SystemAdmin;
using GStoreData.Models;

namespace GStoreWeb.Areas.SystemAdmin.Controllers
{
	public class SystemAdminController : AreaBaseController.SystemAdminAreaBaseController
    {
        // GET: SystemAdmin/SystemAdmin
        public ActionResult Index(int? clientId, int? storeFrontId)
        {
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile != null && !Session.SystemAdminVisitLogged())
			{
				profile.LastSystemAdminVisitDateTimeUtc = DateTime.UtcNow;
				GStoreDb.UserProfiles.Update(profile);
				GStoreDb.SaveChangesDirect();
				Session.SystemAdminVisitLogged(true);
			}

			bool hasErrorMessage = false;
			StoreFront storeFront = null;
			try
			{
				 storeFront = GStoreDb.GetCurrentStoreFront(Request, true, false, false);
			}
			catch(GStoreData.Exceptions.StoreFrontInactiveException exSFI)
			{
				hasErrorMessage = true;
				StoreFrontConfiguration config = exSFI.StoreFront.CurrentConfigOrAny();
				if (config == null)
				{
					string sfiMessageHtml = "Store Front is inactive for this URL.\n"
						+ "\nStore Front Id: " + exSFI.StoreFront.StoreFrontId + " has no configuration defined."
						+ "\n<a href=\"" + this.Url.Action("CreateConfig", "StoreFrontSysAdmin", new { id = exSFI.StoreFront.StoreFrontId }) + "\">Click here to Create a Default Configuration for Store Front Id: " + exSFI.StoreFront.StoreFrontId.ToString().ToHtml() + " [" + exSFI.StoreFront.StoreFrontId + "]" + "</a>\n"
						+ exSFI.Message.ToHtml();
					AddUserMessageBottom("Error getting current storefront: Inactive", sfiMessageHtml, UserMessageType.Danger);
				}
				else if (!config.IsActiveDirect())
				{
					string configName = config.Name;
					string sfiMessageHtml = "Store Front is inactive for this Url.\n"
						+ "\nStore Front '" + config.Name.ToHtml() + "' [" + config.StoreFrontId + "] has an Inactive configuration."
						+ "\n<a href=\"" + this.Url.Action("ActivateConfig", "StoreFrontSysAdmin", new { id = config.StoreFrontConfigurationId }) + "\">Click here to Activate the inactive Configuration for Store Front " + configName.ToHtml() + " [" + config.StoreFrontId + "]" + "</a>\n"
						+ exSFI.Message.ToHtml();
					AddUserMessageBottom("Error getting current storefront: Inactive", sfiMessageHtml, UserMessageType.Danger);
				}
				else
				{
					string configName = config == null ? "No Store Front Config found for Store Front Id: " + exSFI.StoreFront.StoreFrontId : config.Name;
					string sfiMessageHtml = "StoreFront is inactive.\n"
						+ "<a href=\"" + this.Url.Action("ActivateCurrentInactiveStoreFront") + "\">Click here to Activate StoreFront: " + configName.ToHtml() + " [" + exSFI.StoreFront.StoreFrontId + "]" + "</a>\n"
						+ exSFI.Message.ToHtml();
					AddUserMessageBottom("Error getting current storefront: Inactive", sfiMessageHtml, UserMessageType.Danger);
				}
			}
			catch (GStoreData.Exceptions.NoMatchingBindingException exNMB)
			{
				hasErrorMessage = true;
				if (GStoreDb.Clients.IsEmpty())
				{
					string nosfMessage = "There are no No Clients in the database.\n"
						+ "<a href=\"" + this.Url.Action("ForceSeed") + "\">Click here to Create Force a Seed of the Database</a>\n"
						+ "\n<a href=\"" + this.Url.Action("Create", "ClientSysAdmin") + "\">Click here to Create a new Client</a>\n"
						+ exNMB.Message.ToHtml();
					AddUserMessageBottom("Error getting current storefront: No matching bindings", nosfMessage, UserMessageType.Danger);
				}
				else if (GStoreDb.StoreFronts.IsEmpty())
				{
					string nosfMessage = "There are no No Store Fronts in the database.\n"
						+ "<a href=\"" + this.Url.Action("ForceSeed") + "\">Click here to Create Force a Seed of the Database</a>\n"
						+ "\n<a href=\"" + this.Url.Action("Create", "StoreFrontSysAdmin") + "\">Click here to Create a new Store Front</a>\n"
						+ exNMB.Message.ToHtml();
					AddUserMessageBottom("Error getting current storefront: No matching bindings", nosfMessage, UserMessageType.Danger);
				}
				else
				{
					StoreFront guessStoreFront = GStoreDb.SeedAutoMapStoreFrontBestGuess();
					StoreFrontConfiguration config = guessStoreFront.CurrentConfigOrAny();
					if (config == null)
					{
						string nmbMessageHtml = "No store front found for this Url.\n"
							+ "\nThe best guess store front id: " + guessStoreFront.StoreFrontId + " has no configuration defined."
							+ "\n<a href=\"" + this.Url.Action("CreateConfig", "StoreFrontSysAdmin", new { id = guessStoreFront.StoreFrontId } ) + "\">Click here to Create a Default Configuration for Store Front Id: " + guessStoreFront.StoreFrontId.ToString().ToHtml() + " [" + guessStoreFront.StoreFrontId + "]" + "</a>\n"
							+ exNMB.Message.ToHtml();
						AddUserMessageBottom("Error getting current storefront: No matching bindings", nmbMessageHtml, UserMessageType.Danger);
					}
					else if (!config.IsActiveDirect())
					{
						string configName = config.Name;
						string nmbMessageHtml = "No store front found for this Url.\n"
							+ "\nThe best guess store front '" + config.Name.ToHtml() + "' [" + guessStoreFront.StoreFrontId + "] has an Inactive configuration."
							+ "\n<a href=\"" + this.Url.Action("ActivateConfig", "StoreFrontSysAdmin", new { id = config.StoreFrontConfigurationId }) + "\">Click here to Activate the inactive Configuration for Store Front " + configName.ToHtml() + " [" + guessStoreFront.StoreFrontId + "]" + "</a>\n"
							+ exNMB.Message.ToHtml();
						AddUserMessageBottom("Error getting current storefront: No matching bindings", nmbMessageHtml, UserMessageType.Danger);
					}
					else
					{
						string configName = config.Name;
						string nmbMessageHtml = "No store front found for this Url.\n"
							+ "<a href=\"" + this.Url.Action("BindSeedBestGuessStoreFront") + "\">Click here to Create Store Binding for Store Front : " + configName.ToHtml() + " [" + guessStoreFront.StoreFrontId + "]" + "</a>\n"
							+ exNMB.Message.ToHtml();
						AddUserMessageBottom("Error getting current storefront: No matching bindings", nmbMessageHtml, UserMessageType.Danger);
					}
				}
				
			}
			catch (Exception ex)
			{
				hasErrorMessage = true;
				string exMessageHtml = ex.Message.ToHtml() + "\n" + ex.ToString().ToHtml();
				AddUserMessageBottom("Error getting current storefront: unknown error", exMessageHtml, UserMessageType.Danger);
			}

			if (hasErrorMessage)
			{
				AddUserMessage("Errors found!!", "Errors were found in the system configuration for the current URL. See bottom of this page for details or <a href=\"#UserMessagesBottom\">click here<a/>", UserMessageType.Danger);
			}

			ViewBag.StoreFrontFilterList = StoreFrontFilterListWithAllAndNull(clientId, storeFrontId);

			this.BreadCrumbsFunc = ((html => this.TopBreadcrumb(html, false)));
			return View("Index");
        }

		public PartialViewResult ClearAllLogs(int? filterClientId, int? filterStoreFrontId)
		{
			ClearLogFolderHelper("GStoreFolder_BadRequests");
			ClearLogFolderHelper("GStoreFolder_EmailSent");
			ClearLogFolderHelper("GStoreFolder_FileNotFoundLogs");
			ClearLogFolderHelper("GStoreFolder_LogExceptions");
			ClearLogFolderHelper("GStoreFolder_PageViewEvents");
			ClearLogFolderHelper("GStoreFolder_SecurityEvents");
			ClearLogFolderHelper("GStoreFolder_SmsSent");
			ClearLogFolderHelper("GStoreFolder_SystemEvents");
			ClearLogFolderHelper("GStoreFolder_UserActionEvents");

			return RecordSummary(filterClientId, filterStoreFrontId, false);
		}

		public PartialViewResult ClearLogFolder(string folder, int? filterClientId, int? filterStoreFrontId)
		{
			if (string.IsNullOrEmpty(folder))
			{
				HttpBadRequest("Folder is null");
				return null;
			}

			ClearLogFolderHelper(folder);
			return RecordSummary(filterClientId, filterStoreFrontId, false);
		}

		protected void ClearLogFolderHelper(string folder)
		{
			if (string.IsNullOrEmpty(folder))
			{
				throw new ArgumentException("folder");
			}

			//get folder
			string virtualPath = EventLogExtensions.GStoreLogConstantNameToPath(folder);
			string folderPath = Server.MapPath(virtualPath);

			if (!System.IO.Directory.Exists(folderPath))
			{
				AddUserMessage("Log Folder does not exist", "Log Folder for " + folder.ToHtml() + " does not exist, no need to delete", UserMessageType.Info);
				return;
			}

			//delete folder too
			try
			{
				System.IO.Directory.Delete(folderPath, true);
				AddUserMessage("Log Folder Cleared!", "Log folder " + folder.ToHtml() + " cleared!", UserMessageType.Success);
				return;
			}
			catch (Exception ex)
			{
				AddUserMessage("Error clearing Log Folder!", "Error clearing log folder. Exception: " + ex.ToString().ToHtml(), UserMessageType.Danger);
			}
		}

		public ActionResult ActivateCurrentInactiveStoreFront()
		{
			if (CurrentStoreFrontOrNull != null)
			{
				AddUserMessage("Activate Current Inactive Store Front not needed!", "Current Store Front is Already Active.", UserMessageType.Warning);
				return RedirectToAction("Index");
			}

			List<StoreBinding> inactiveBindings = GStoreDb.GetInactiveStoreBindingMatches(Request);
			if (inactiveBindings == null || inactiveBindings.Count == 0)
			{
				AddUserMessage("ActivateCurrentInactiveStoreFront Failed!", "No inactive bindings found", UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			StoreBinding binding = inactiveBindings[0];

			bool result = this.ActivateStoreFrontClientBindingAndConfig(binding);
			if (result)
			{
				AddUserMessage("ActivateCurrentInactiveStoreFront Success!", "Re-activated store front '" + binding.StoreFront.CurrentConfigOrAny().Name.ToHtml() + "' [" + binding.StoreFront.StoreFrontId + "]", UserMessageType.Success);
			}
			else
			{
				AddUserMessage("ActivateCurrentInactiveStoreFront Failed!", "Could not re-activate store front [" + binding.StoreFront.StoreFrontId + "]\n<br/><a href=\"" + Url.Action("CreateConfig", "StoreFrontSysAdmin", new { id = binding.StoreFrontId }) + "\">Click here to create a new config for this store front</a>", UserMessageType.Danger);
			}

			return RedirectToAction("Index");
		}

		public ActionResult BindSeedBestGuessStoreFront()
		{
			StoreBinding binding = this.AutoMapBindingSeedBestGuessStoreFront();
			if (binding == null)
			{
				AddUserMessage("BindSeedBestGuessStoreFront Failed!", "BindSeedBestGuessStoreFront Failed!", UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			AddUserMessage("BindSeedBestGuessStoreFront Success!", "Auto-mapped Binding successfully!", UserMessageType.Success);
			return RedirectToAction("Index");
		}

		public ActionResult ForceSeed()
		{
			GStoreDb.SeedDatabase(true);
			AddUserMessage("Database Seeded!", "The Database has been seeded successfully!", UserMessageType.Success);
			return RedirectToAction("Index");
		}

		public ActionResult SendTestEmail()
		{
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile == null)
			{
				AddUserMessage("Profile not active", "Cannot send test Email Message!", UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			string message = "Test Message from " + Request.Url.Host;

			bool result;
			try
			{
				result = this.SendEmail(profile.Email, profile.FullName, message, message, message);
			}
			catch (Exception ex)
			{
				AddUserMessage("Test Email Exception", "Test Email encountered an exception sending to '" + profile.Email + "'.\n<br/>" + ex.ToString(), UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			if (result)
			{
				AddUserMessage("Test Email Sent", "Test Email sent to '" + profile.Email + "' successfully.", UserMessageType.Info);
			}
			else
			{
				AddUserMessage("Test Email Failed", "Test Email failed sending to '" + profile.Email + "'", UserMessageType.Danger);
				if (!Settings.AppEnableEmail)
				{
					AddUserMessage("App Setting", "App Setting 'AppEnableEmail' = false. Set this setting to true to enable email from this server.", UserMessageType.Danger);
				}
				else if (CurrentClientOrNull == null)
				{
					AddUserMessage("Client Not Active", "No Current client; cannot send email. You Must use an active site for email test.", UserMessageType.Danger);
				}
				else if (!CurrentClientOrThrow.UseSendGridEmail)
				{
					AddUserMessage("Client Email not Enabled", "Current Client does not have Email enabled. Set Client Configuration to enable Email.", UserMessageType.Danger);
				}
				else if (string.IsNullOrEmpty(CurrentClientOrThrow.SendGridMailAccount) || string.IsNullOrEmpty(CurrentClientOrThrow.SendGridMailFromEmail) || string.IsNullOrEmpty(CurrentClientOrThrow.SendGridMailPassword))
				{
					AddUserMessage("Client Email Config Missing", "Current Client does not have a valid sendgrid configuration. Set Client Configuration to SendGrid settings.", UserMessageType.Danger);
				}
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		public ActionResult SendTestSms()
		{
			UserProfile profile = CurrentUserProfileOrNull;
			if (profile == null)
			{
				AddUserMessage("Profile not active", "Cannot send test SMS Message!", UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			string phone = profile.AspNetIdentityUser().PhoneNumber;
			bool result;
			try
			{
				result = this.SendSms(phone, "This is a test SMS Text.");
			}
			catch (Exception ex)
			{
				AddUserMessage("Test SMS Exception", "Test SMS encountered an exception sending to '" + phone + "'.\n<br/>" + ex.ToString(), UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			if (result)
			{
				AddUserMessage("Test SMS Sent", "Test SMS sent to '" + phone + "' successfully.", UserMessageType.Info);
			}
			else
			{
				AddUserMessage("Test SMS Failed", "Test SMS failed sending to '" + phone + "'", UserMessageType.Danger);
				if (!Settings.AppEnableSMS)
				{
					AddUserMessage("App Setting", "App Setting 'AppEnableSMS' = false. Set this setting to true to enable SMS from this server.", UserMessageType.Danger);
				}
				else if (CurrentClientOrNull == null)
				{
					AddUserMessage("Client Not Active", "No Current client; cannot send SMS. You Must use an active site for SMS test.", UserMessageType.Danger);
				}
				else if (!CurrentClientOrThrow.UseTwilioSms)
				{
					AddUserMessage("Client SMS not Enabled", "Current Client does not have SMS enabled. Set Client Configuration to enable SMS.", UserMessageType.Danger);
				}
				else if (string.IsNullOrEmpty(CurrentClientOrThrow.TwilioFromPhone) || string.IsNullOrEmpty(CurrentClientOrThrow.TwilioSid) || string.IsNullOrEmpty(CurrentClientOrThrow.TwilioToken))
				{
					AddUserMessage("Client SMS Config Missing", "Current Client does not have a valid Twilio configuration. Set Client Configuration to Twilio settings.", UserMessageType.Danger);
				}
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		public PartialViewResult RecordSummary(int? clientId, int? storeFrontId, bool? clientIdChanged)
		{
			if (clientIdChanged ?? false)
			{
				storeFrontId = null;
			}
			if (ViewData.ModelState.ContainsKey("StoreFrontId"))
			{
				ViewData.ModelState.Remove("StoreFrontId");
			}
			Tuple<int?, int?> viewModel = new Tuple<int?, int?>(clientId, storeFrontId);
			return PartialView("_RecordSummaryPartial", viewModel);
		}

		public PartialViewResult FileSystemSummary(bool showEmptyFolders = false)
		{
			return PartialView("_FileSystemSummaryPartial", showEmptyFolders);
		}

	}
}
