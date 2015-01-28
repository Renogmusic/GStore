using GStore.Data;
using System;
using System.Web.Mvc;
using GStore.Models;
using System.Collections.Generic;
using GStore.AppHtmlHelpers;
using System.Linq;

namespace GStore.Areas.SystemAdmin.Controllers
{
    public class SystemAdminController : BaseClasses.SystemAdminBaseController
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
			Models.StoreFront storeFront = null;
			try
			{
				 storeFront = GStoreDb.GetCurrentStoreFront(Request, true, false, false);
			}
			catch(Exceptions.StoreFrontInactiveException exSFI)
			{
				hasErrorMessage = true;
				string sfiMessageHtml = "StoreFront is inactive.\n"
					+ "<a href=\"" + this.Url.Action("ActivateCurrentInactiveStoreFront") + "\">Click here to Activate StoreFront: " + (exSFI.StoreFront.CurrentConfigOrAny() == null ? "" : exSFI.StoreFront.CurrentConfigOrAny().Name.ToHtml()) + " [" + exSFI.StoreFront.StoreFrontId + "]" + "</a>\n"
					+ exSFI.Message.ToHtml();
				AddUserMessageBottom("Error getting current storefront: Inactive", sfiMessageHtml, AppHtmlHelpers.UserMessageType.Danger);
			}
			catch(Exceptions.NoMatchingBindingException exNMB)
			{
				hasErrorMessage = true;
				if (GStoreDb.StoreFronts.IsEmpty())
				{
					string nosfMessage = "There are no No Store Fronts in the database.\n"
						+ "<a href=\"" + this.Url.Action("ForceSeed") + "\">Click here to Create Force a Seed of the Database</a>\n"
						+ exNMB.Message.ToHtml();
					AddUserMessageBottom("Error getting current storefront: No matching bindings", nosfMessage, AppHtmlHelpers.UserMessageType.Danger);
				}
				else
				{
					StoreFront guessStoreFront = GStoreDb.SeedAutoMapStoreFrontBestGuess();
					string nmbMessageHtml = "No store front found for this Url.\n"
						+ "<a href=\"" + this.Url.Action("BindSeedBestGuessStoreFront") + "\">Click here to Create Store Binding for Store Front : " + guessStoreFront.CurrentConfigOrAny().Name.ToHtml() + " [" + guessStoreFront.StoreFrontId + "]" + "</a>\n"
						+ exNMB.Message.ToHtml();
					AddUserMessageBottom("Error getting current storefront: No matching bindings", nmbMessageHtml, AppHtmlHelpers.UserMessageType.Danger);
				}
				
			}
			catch (Exception ex)
			{
				hasErrorMessage = true;
				string exMessageHtml = ex.Message.ToHtml() + "\n" + ex.ToString().ToHtml();
				AddUserMessageBottom("Error getting current storefront: unknown error", exMessageHtml, AppHtmlHelpers.UserMessageType.Danger);
			}

			if (hasErrorMessage)
			{
				AddUserMessage("Errors found!!", "Errors were found in the system configuration. See bottom of this page for details or <a href=\"#UserMessagesBottom\">click here<a/>", AppHtmlHelpers.UserMessageType.Danger);
			}

			ViewBag.StoreFrontFilterList = StoreFrontFilterListWithAllAndNull(clientId, storeFrontId);

			this.BreadCrumbsFunc = ((html => this.TopBreadcrumb(html, false)));
			return View("Index");
        }

		public ActionResult ClearLogFolder(string folder)
		{
			if (string.IsNullOrEmpty(folder))
			{
				return HttpBadRequest("Folder is null");
			}

			//get folder
			string virtualPath = EventLogExtensions.GStoreLogConstantNameToPath(folder);
			string folderPath = Server.MapPath(virtualPath);

			if (!System.IO.Directory.Exists(folderPath))
			{
				AddUserMessage("Log Folder does not exist", "Log Folder for " + folder.ToHtml() + " does not exist, no need to delete", AppHtmlHelpers.UserMessageType.Info);
				return RedirectToAction("Index");
			}

			//delete folder too
			try
			{
				System.IO.Directory.Delete(folderPath, true);
				AddUserMessage("Log Folder Cleared!", "Log folder " + folder.ToHtml() + " cleared!", AppHtmlHelpers.UserMessageType.Success);
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				AddUserMessage("Error clearing Log Folder!", "Error clearing log folder. Exception: " + ex.ToString().ToHtml(), AppHtmlHelpers.UserMessageType.Danger);
			}

			return RedirectToAction("Index");
		}

		public ActionResult ActivateCurrentInactiveStoreFront()
		{
			if (CurrentStoreFrontOrNull != null)
			{
				AddUserMessage("Activate Current Inactive Store Front not needed!", "Current Store Front is Already Active.", AppHtmlHelpers.UserMessageType.Warning);
				return RedirectToAction("Index");
			}

			List<StoreBinding> inactiveBindings = GStoreDb.GetInactiveStoreBindingMatches(Request);
			if (inactiveBindings == null || inactiveBindings.Count == 0)
			{
				AddUserMessage("ActivateCurrentInactiveStoreFront Failed!", "No inactive bindings found", AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			StoreBinding binding = inactiveBindings[0];
			this.ActivateStoreFrontClientBindingAndConfig(binding);

			AddUserMessage("ActivateCurrentInactiveStoreFront Success!", "Re-activated store front '" + binding.StoreFront.CurrentConfigOrAny().Name.ToHtml() + "' [" + binding.StoreFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);

			return RedirectToAction("Index");
		}

		public ActionResult BindSeedBestGuessStoreFront()
		{
			StoreBinding binding = this.AutoMapBindingSeedBestGuessStoreFront();
			if (binding == null)
			{
				AddUserMessage("BindSeedBestGuessStoreFront Failed!", "BindSeedBestGuessStoreFront Failed!", AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			AddUserMessage("BindSeedBestGuessStoreFront Success!", "Auto-mapped Binding successfully!", AppHtmlHelpers.UserMessageType.Success);
			return RedirectToAction("Index");
		}

		public ActionResult ForceSeed()
		{
			GStoreDb.SeedDatabase(true);
			AddUserMessage("Database Seeded!", "The Database has been seeded successfully!", AppHtmlHelpers.UserMessageType.Success);
			return RedirectToAction("Index");
		}

		public PartialViewResult RecordSummary(int? clientId)
		{
			return PartialView("_RecordSummaryPartial", clientId);
		}
	}
}