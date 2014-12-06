using GStore.Areas.SystemAdmin.Controllers.BaseClasses;
using GStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
		
		//public static IEnumerable<SelectListItem> StoreFrontFilterList(this HtmlHelper htmlHelper)
		//{
		//	SystemAdminBaseController controller = htmlHelper.ViewContext.Controller as SystemAdminBaseController;
		//	if (controller == null)
		//	{
		//		throw new NullReferenceException("Controller does not inherit from SystemAdminBaseController");
		//	}

		//	return controller.StoreFrontFilterList();
		//}

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
		
		//GStore.Data.IGstoreDb gstoreDb = GStore.Data.RepositoryFactory.SystemWideRepository(currentUserProfile.UserName);




	}
}