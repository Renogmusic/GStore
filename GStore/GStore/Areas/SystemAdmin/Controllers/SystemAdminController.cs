using GStore.Data;
using System;
using System.Web.Mvc;
using GStore.Models;
using System.Collections.Generic;

namespace GStore.Areas.SystemAdmin.Controllers
{
    public class SystemAdminController : BaseClasses.SystemAdminBaseController
    {
        // GET: SystemAdmin/SystemAdmin
        public ActionResult Index(int? clientId, int? storeFrontId)
        {
			bool hasErrorMessage = false;
			Models.StoreFront storeFront = null;
			try
			{
				 storeFront = GStoreDb.GetCurrentStoreFront(Request, true, false);
			}
			catch(Exceptions.StoreFrontInactiveException exSFI)
			{
				hasErrorMessage = true;
				string sfiMessageHtml = "StoreFront is inactive.\n"
					+ "<a href=\"" + this.Url.Action("ActivateCurrentInactiveStoreFront") + "\">Click here to Activate StoreFront: " + Server.HtmlEncode(exSFI.StoreFront.Name + " [" + exSFI.StoreFront.StoreFrontId + "]") + "</a>\n"
					+ exSFI.Message;
				AddUserMessageBottom("Error getting current storefront: Inactive", sfiMessageHtml, AppHtmlHelpers.UserMessageType.Danger);
			}
			catch(Exceptions.NoMatchingBindingException exNMB)
			{
				hasErrorMessage = true;
				StoreFront guessStoreFront = GStoreDb.SeedAutoMapStoreFrontBestGuess();
				string nmbMessageHtml = "No store front found for this Url.\n"
					+ "<a href=\"" + this.Url.Action("BindSeedBestGuessStoreFront") + "\">Click here to Create Store Binding for Store Front : " + Server.HtmlEncode(guessStoreFront.Name + " [" + guessStoreFront.StoreFrontId + "]") + "</a>\n"
					+ exNMB.Message;
				AddUserMessageBottom("Error getting current storefront: No matching bindings", nmbMessageHtml, AppHtmlHelpers.UserMessageType.Danger);
			}
			catch (Exception ex)
			{
				hasErrorMessage = true;
				string exMessageHtml = ex.Message + "\n" + ex.ToString();
				AddUserMessageBottom("Error getting current storefront: unknown error", exMessageHtml, AppHtmlHelpers.UserMessageType.Danger);
			}

			if (hasErrorMessage)
			{
				AddUserMessage("Errors found!!", "Errors were found in the system configuration. See bottom of this page for details or <a href=\"#UserMessagesBottom\">click here<a/>", AppHtmlHelpers.UserMessageType.Danger);
			}

			ViewBag.ClientFilterList = ClientFilterListEx(clientId, true, true, true) ;
			ViewBag.StoreFrontFilterList = StoreFrontFilterList(clientId, storeFrontId);

			return View("Index");
        }

		public ActionResult ActivateCurrentInactiveStoreFront()
		{
			if (CurrentStoreFrontOrNull != null)
			{
				return RedirectToAction("Index");
			}

			List<StoreBinding> inactiveBindings = GStoreDb.GetInactiveStoreBindingMatches(Request);
			if (inactiveBindings == null || inactiveBindings.Count == 0)
			{
				AddUserMessage("ActivateCurrentInactiveStoreFront Failed!", "No inactive bindings found", AppHtmlHelpers.UserMessageType.Danger);
				return RedirectToAction("Index");
			}

			StoreBinding binding = inactiveBindings[0];
			this.ActivateStoreFrontClientAndBinding(binding);

			AddUserMessage("ActivateCurrentInactiveStoreFront Success!", "Re-activated store front '" + binding.StoreFront.Name + "' [" + binding.StoreFront.StoreFrontId + "]", AppHtmlHelpers.UserMessageType.Success);

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

	}
}