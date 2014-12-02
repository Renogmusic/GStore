using GStore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Areas.StoreAdmin.Controllers
{
	public class ValueListAdminController : BaseClasses.StoreAdminBaseController
    {
		//ValueList_Create = 7220,
		//ValueList_ViewLists = 7200,
		//ValueList_EditLists = 7250,
		//ValueList_DeleteLists = 7250,
		//ValueList_ViewValues = 7300,
		//ValueList_EditValues = 7310,
		//ValueList_AddValues = 7320,
		//ValueList_DeleteValues = 7330,


		[AuthorizeGStoreAction(GStoreAction.ValueList_Manager)]
		public ActionResult Manager()
        {
			return View("Manager", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(true, GStoreAction.ValueList_Add)]
		public ActionResult Add()
		{
			return View("Add", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ValueList_Edit)]
		public ActionResult Edit(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ValueList id cannot be null");
			}
			return View("Edit", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ValueList_View)]
		public ActionResult View(int? id)
        {
			if (!id.HasValue)
			{
				return HttpBadRequest("ValueList id cannot be null");
			}
			return View("View", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(GStoreAction.ValueList_Delete)]
		public ActionResult Delete()
        {
			return View("Delete", this.StoreAdminViewModel);
        }

		[AuthorizeGStoreAction(true, GStoreAction.ValueList_AddValue)]
		public ActionResult AddValue()
		{
			return View("AddValue", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ValueList_EditValue)]
		public ActionResult EditValue(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ValueList id cannot be null");
			}
			return View("EditValue", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(true, GStoreAction.ValueList_ViewValue)]
		public ActionResult ViewValue(int? id)
		{
			if (!id.HasValue)
			{
				return HttpBadRequest("ValueList id cannot be null");
			}
			return View("ViewValue", this.StoreAdminViewModel);
		}

		[AuthorizeGStoreAction(GStoreAction.ValueList_DeleteValue)]
		public ActionResult DeleteValue()
		{
			return View("DeleteValue", this.StoreAdminViewModel);
		}

    }
}