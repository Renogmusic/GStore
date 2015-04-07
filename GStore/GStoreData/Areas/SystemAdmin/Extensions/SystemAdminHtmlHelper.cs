﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.Areas.SystemAdmin.ControllerBase;
using GStoreData.Models;

namespace GStoreData.Areas.SystemAdmin
{
	public static class SystemAdminHtmlHelper
	{

		public static IEnumerable<SelectListItem> ClientFilterList(this HtmlHelper htmlHelper, string labelForAll = "(ALL)", string labelForNull = "(NONE)")
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			return controller.ClientFilterList(labelForAll, labelForNull);
		}

		public static IEnumerable<SelectListItem> ClientFilterListForRecordSummary(this HtmlHelper htmlHelper, int? clientId, string labelForAll = "(ALL)", string labelForNull = "(NONE)")
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			return controller.ClientFilterListDirect(clientId, labelForAll, labelForNull);
		}

		public static IEnumerable<SelectListItem> StoreFrontFilterListForRecordSummary(this HtmlHelper htmlHelper, int? clientId, int? storeFrontId, string labelForAll = "(ALL)", string labelForNull = "(NONE)")
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}

			return controller.StoreFrontFilterList(clientId, storeFrontId, labelForAll, labelForNull);
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

		public static IEnumerable<SelectListItem> ClientList(this HtmlHelper htmlHelper, int? selectedClientId, bool onlyWithstoreFronts = false)
		{
			SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
			if (controller == null)
			{
				throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
			}
			IQueryable<Client> query = null;
			if (onlyWithstoreFronts)
			{
				query = controller.GStoreDb.Clients.Where(c => c.StoreFronts.Any());
			}
			else
			{
				query = controller.GStoreDb.Clients.All();
			}
			List<Client> clients = query.ApplyDefaultSort().ToList();
			return clients.Select(c => new SelectListItem() { 
				Value = c.ClientId.ToString(), 
				Selected = (selectedClientId.HasValue && selectedClientId.Value == c.ClientId), 
				Text = c.Name + " [" + c.ClientId + "]" + (c.IsActiveDirect() ? "" : " [INACTIVE]") + " Store Fronts: " + c.StoreFronts.Count() });
		}

		/// <summary>
		/// Will check for parameter client id, then viewdata["ClientId"] then parent record client id
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="clientId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> StoreFrontList(this HtmlHelper htmlHelper, int? clientId)
		{
			if (!clientId.HasValue)
			{
				clientId = htmlHelper.ViewData["ClientId"] as int?;
				if (!clientId.HasValue)
				{
					Models.BaseClasses.ClientRecord parentModel = htmlHelper.ViewData.ModelMetadata.Container as Models.BaseClasses.ClientRecord;
					clientId = 0;
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
				}
			}

			IQueryable<StoreFront> query = htmlHelper.GStoreDb().StoreFronts.Where(c => c.ClientId == clientId);

			int? selectedStoreFrontId = htmlHelper.ViewData.Model as int?;

			List<StoreFront> storeFronts = query.ApplyDefaultSort().ToList();

			return storeFronts.Select(s => new SelectListItem() { 
				Value = s.StoreFrontId.ToString(), 
				Selected = (selectedStoreFrontId.HasValue && selectedStoreFrontId.Value == s.StoreFrontId),
				Text = (s.CurrentConfigOrAny() == null ? "Store Front Id " + s.StoreFrontId : s.CurrentConfigOrAny().Name) + " [" + s.StoreFrontId + "]" + (s.IsActiveBubble() ? "" : " [INACTIVE]") + " - Client " + s.Client.Name + "[" + s.ClientId + "]" + (s.Client.IsActiveDirect() ? "" : " [INACTIVE]")}).ToList();
		}


		/// <summary>
		/// Will check for parameter client id, then viewdata["ClientId"] then parent record client id
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="clientId"></param>
		/// <returns></returns>
		public static IEnumerable<SelectListItem> StoreFrontConfigurationList(this HtmlHelper htmlHelper, int? storeFrontId = null, int? selectedStoreFrontConfigurationId = null)
		{
			if (!storeFrontId.HasValue)
			{
				storeFrontId = htmlHelper.ViewData["StoreFrontId"] as int?;
				if (!storeFrontId.HasValue)
				{
					Models.BaseClasses.StoreFrontRecord parentModel = htmlHelper.ViewData.ModelMetadata.Container as Models.BaseClasses.StoreFrontRecord;
					storeFrontId = 0;
					if (parentModel != null)
					{
						storeFrontId = parentModel.StoreFrontId;
					}
					else
					{
						UserProfile parentUserProfile = htmlHelper.ViewData.ModelMetadata.Container as UserProfile;
						if (parentUserProfile != null)
						{
							storeFrontId = parentUserProfile.StoreFrontId ?? 0;
						}
					}
				}
			}

			if (!selectedStoreFrontConfigurationId.HasValue)
			{
				selectedStoreFrontConfigurationId = htmlHelper.ViewData.Model as int?;
			}
			IQueryable<StoreFrontConfiguration> query = htmlHelper.GStoreDb().StoreFrontConfigurations.Where(sfc => sfc.StoreFrontId == storeFrontId);

			List<StoreFrontConfiguration> storeFrontConfigurations = query.ApplyDefaultSort().ToList();

			return storeFrontConfigurations.Select(s => new SelectListItem()
			{
				Value = s.StoreFrontConfigurationId.ToString(),
				Selected = (selectedStoreFrontConfigurationId.HasValue && s.StoreFrontConfigurationId == selectedStoreFrontConfigurationId.Value),
				Text = s.ConfigurationName + " [" + s.StoreFrontConfigurationId + "]" + (s.IsActiveBubble() ? "" : " [INACTIVE]")
					+ " - Store Front " + s.Name + "[" + s.StoreFrontId + "]" + (s.StoreFront.IsActiveDirect() ? "" : " [INACTIVE]")
					+ " - Client " + s.Client.Name + "[" + s.ClientId + "]" + (s.Client.IsActiveDirect() ? "" : " [INACTIVE]")
			}).ToList();
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