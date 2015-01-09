using GStore.Areas.SystemAdmin.Controllers.BaseClasses;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GStore.AppHtmlHelpers;
using GStore.Models;

namespace GStore.Areas.SystemAdmin
{
	public static class SysAdminHtmlHelper
	{

		public static IEnumerable<SelectListItem> ClientFilterList(this HtmlHelper htmlHelper)
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			return controller.ClientFilterList();
		}
		
		public static bool ShowAllClients(this HtmlHelper htmlHelper)
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			return controller.ShowAllClients();
		}

		public static bool ClientIsFiltered(this HtmlHelper htmlHelper)
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			return controller.ClientIsFiltered();
		}

		public static int? FilterClientId(this HtmlHelper htmlHelper)
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			return controller.FilterClientId();
		}

		public static MvcHtmlString Breadcrumbs(this HtmlHelper htmlHelper)
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				return new MvcHtmlString("");
			}
			return controller.Breadcrumbs(htmlHelper);
		}

		public static IEnumerable<SelectListItem> ClientList(this HtmlHelper htmlHelper, int? selectedClientId)
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			List<Client> clients = controller.GStoreDb.Clients.All().ApplyDefaultSort().ToList();
			return clients.Select(c => new SelectListItem() { 
				Value = c.ClientId.ToString(), 
				Selected = (selectedClientId.HasValue && selectedClientId.Value == c.ClientId), 
				Text = c.Name + " [" + c.ClientId + "]" + (c.IsActiveDirect() ? "" : " [INACTIVE]") + " Store Fronts: " + c.StoreFronts.Count() });
		}

		public static IEnumerable<SelectListItem> StoreFrontList(this HtmlHelper htmlHelper)
		{
			Models.BaseClasses.ClientRecord parentModel = htmlHelper.ViewData.ModelMetadata.Container as Models.BaseClasses.ClientRecord;
			int clientId = 0;
			if (parentModel != null)
			{
				clientId = parentModel.ClientId;
			}
			else
			{
				UserProfile parentUserProfile = htmlHelper.ViewData.ModelMetadata.Container as UserProfile;
				if (parentUserProfile != null)
				{
					clientId = parentUserProfile.ClientId ?? 0;
				}
			}

			IQueryable<StoreFront> query = htmlHelper.GStoreDb().StoreFronts.Where(c => c.ClientId == clientId);

			int? selectedStoreFrontId = null;
			Models.BaseClasses.StoreFrontRecord parentStoreFrontModel = htmlHelper.ViewData.ModelMetadata.Container as Models.BaseClasses.StoreFrontRecord;
			if (parentStoreFrontModel != null && parentStoreFrontModel.StoreFrontId != 0)
			{
				selectedStoreFrontId = parentStoreFrontModel.StoreFrontId;
			}

			List<StoreFront> storeFronts = query.ApplyDefaultSort().ToList();

			return storeFronts.Select(s => new SelectListItem() { 
				Value = s.StoreFrontId.ToString(), 
				Selected = (selectedStoreFrontId.HasValue && selectedStoreFrontId.Value == s.StoreFrontId),
				Text = (s.CurrentConfigOrAny() == null ? "Store Front Id " + s.StoreFrontId : s.CurrentConfigOrAny().Name) + " [" + s.StoreFrontId + "]" + (s.IsActiveBubble() ? "" : " [INACTIVE]") + " - Client " + s.Client.Name + "[" + s.ClientId + "]" + (s.Client.IsActiveDirect() ? "" : " [INACTIVE]")}).ToList();
		}

		/// <summary>
		/// Returns a list of theme folders from the file system for selection
		/// </summary>
		/// <param name="clientId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> ThemeFolderList(this HtmlHelper htmlHelper)
		{
			HttpServerUtilityBase server = htmlHelper.ViewContext.HttpContext.Server;

			string virtualPath = "~/Content/Server/Themes/";
			if (!System.IO.Directory.Exists(server.MapPath(virtualPath)))
			{
				throw new ApplicationException("Themes folder does not exist in file system at '" + virtualPath + "'");
			}
			System.IO.DirectoryInfo themesFolder = new System.IO.DirectoryInfo(server.MapPath(virtualPath));
			IEnumerable<System.IO.DirectoryInfo> themeFolders = themesFolder.EnumerateDirectories("*", System.IO.SearchOption.TopDirectoryOnly);

			string selectedName = htmlHelper.ViewData.Model as string;
			return themeFolders.Select(t => new SelectListItem
			{
				Value = t.Name,
				Text = t.Name + " [" + (System.IO.File.Exists(t.FullName + "\\bootstrap.min.css") ? "bootstrap.min.css OK" : "WARNING: no bootstrap.min.css") + "]",
				Selected = t.Name == selectedName
			});

		}

		public static IEnumerable<SelectListItem> ValueListSelect(this HtmlHelper htmlHelper)
		{
			ValueList valueListModel = htmlHelper.ViewData.Model as ValueList;
			int selectedValueListId = 0;
			if (valueListModel != null)
			{
				selectedValueListId = valueListModel.ValueListId;
			}

			IQueryable<ValueList> query = null;
			Models.BaseClasses.ClientRecord parentModel = htmlHelper.ViewData.ModelMetadata.Container as Models.BaseClasses.ClientRecord;
			if (parentModel != null && parentModel.ClientId != 0)
			{
				query = htmlHelper.GStoreDb().ValueLists.Where(vl => vl.ClientId == parentModel.ClientId);
			}
			else
			{
				query = htmlHelper.GStoreDb().ValueLists.All();
			}

			List<ValueList> valueLists = query.ApplyDefaultSort().ToList();
			return valueLists.Select(vl =>
				new SelectListItem()
				{
					Value = vl.ValueListId.ToString(),
					Text = vl.Name + " [" + vl.ValueListId + "]" + (vl.IsActiveBubble() ? "" : " [INACTIVE]") + " Client '" + vl.Client.Name + "' [" + vl.ClientId + "]" + " items: " + vl.ValueListItems.Count(),
					Selected = vl.ValueListId == selectedValueListId
				});
		}

		public static SelectList UserProfileList(this HtmlHelper htmlHelper, int? clientId, int? storeFrontId)
		{
			var query = htmlHelper.GStoreDb().UserProfiles.All();

			if (clientId.HasValue)
			{
				query = query.Where(p => !p.ClientId.HasValue || p.ClientId.Value == clientId);
			}
			if (storeFrontId.HasValue)
			{
				query = query.Where(p => !p.StoreFrontId.HasValue || p.StoreFrontId.Value == storeFrontId);
			}
			query = query.OrderBy(p => p.Order).ThenBy(p => p.UserProfileId).ThenBy(p => p.UserName);

			IQueryable<SelectListItem> items = query.Select(p => new SelectListItem
			{
				Value = p.UserProfileId.ToString(),
				Text = p.FullName + " <" + p.Email + ">"
				+ (p.StoreFrontId.HasValue ? " - Store '" + p.StoreFront.CurrentConfigOrAny().Name + "' [" + p.StoreFrontId + "]" : " (no store)")
				+ (p.ClientId.HasValue ? " - Client '" + p.Client.Name + "' [" + p.ClientId + "]" : " (no client)")
			});

			return new SelectList(items, "Value", "Text");
		}



	}
}