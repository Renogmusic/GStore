using GStore.Areas.StoreAdmin.Controllers.BaseClasses;
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
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace GStore.AppHtmlHelpers
{
	public static class StoreAdminHtmlHelper
	{
		public static SelectList UserProfileList(this HtmlHelper htmlHelper)
		{
			StoreFront storeFront = htmlHelper.CurrentStoreFront(true);
			Client client = htmlHelper.CurrentClient(true);

			IQueryable<UserProfile> query = htmlHelper.GStoreDb().UserProfiles
				.Where(
					c => (!c.ClientId.HasValue || c.ClientId.Value == client.ClientId)
						&& (!c.StoreFrontId.HasValue || c.StoreFrontId == storeFront.StoreFrontId)
					).ApplyDefaultSort();

			List<UserProfile> userProfiles = query.ToList();

			IEnumerable<SelectListItem> items = userProfiles.Select(p => new SelectListItem
			{
				Value = p.UserProfileId.ToString(),
				Text = p.FullName + " <" + p.Email + ">"
				+ (p.StoreFrontId.HasValue ? " - Store '" + p.StoreFront.CurrentConfigOrAny().Name + "' [" + p.StoreFrontId + "]" : " (no store)")
				+ (p.ClientId.HasValue ? " - Client '" + p.Client.Name + "' [" + p.ClientId + "]" : " (no client)")
			});

			return new SelectList(items, "Value", "Text");
		}

		public static IEnumerable<SelectListItem> PageList(this HtmlHelper htmlHelper)
		{
			int selectedPageId = (htmlHelper.ViewData.Model as int?) ?? 0;

			List<Page> pages = htmlHelper.CurrentStoreFront(true).Pages.AsQueryable().ApplyDefaultSort().ToList();
			return pages.Select(p =>
				new SelectListItem()
				{
					Value = p.PageId.ToString(),
					Text = p.Name + " [" + p.PageId + "]" + (p.IsActiveBubble() ? "" : " [INACTIVE]") + " Url: " + p.Url,
					Selected = p.PageId == selectedPageId
				});
		}

		public static IEnumerable<SelectListItem> ParentNavBarItemList(this HtmlHelper htmlHelper)
		{
			int selectedNavBarItemId = (htmlHelper.ViewData.Model as int?) ?? 0;

			List<NavBarItem> navBarItems = htmlHelper.CurrentStoreFront(true).NavBarItems.AsQueryable()
				.Where(n => (!n.ParentNavBarItemId.HasValue) || (n.ParentNavBarItemId.Value != selectedNavBarItemId)).ApplyDefaultSort().ToList();

			//todo: need to exclude descendants (grandchildren)

			return  navBarItems.Select(n =>
				new SelectListItem()
				{
					Value = n.NavBarItemId.ToString(),
					Text = n.Name + " [" + n.NavBarItemId + "]" + (n.IsActiveBubble() ? "" : " [INACTIVE]"),
					Selected = n.NavBarItemId == selectedNavBarItemId
				});
		}

		public static IEnumerable<SelectListItem> ThemeList(this HtmlHelper htmlHelper)
		{
			int selectedThemeId = (htmlHelper.ViewData.Model as int?) ?? 0;

			List<Theme> themes = htmlHelper.CurrentClient(true).Themes.AsQueryable().ApplyDefaultSort().ToList();
			return themes.Select(t =>
				new SelectListItem()
				{
					Value = t.ThemeId.ToString(),
					Text = t.Name + " [" + t.ThemeId + "]" + (t.IsActiveBubble() ? "" : " [INACTIVE]"),
					Selected = t.ThemeId == selectedThemeId
				});
		}

		public static IEnumerable<SelectListItem> ValueListSelect(this HtmlHelper htmlHelper)
		{
			int selectedValueListId = (htmlHelper.ViewData.Model as int?) ?? 0;

			List<ValueList> valueLists = htmlHelper.CurrentClient(true).ValueLists.AsQueryable().ApplyDefaultSort().ToList();
			return valueLists.Select(vl =>
				new SelectListItem()
				{
					Value = vl.ValueListId.ToString(),
					Text = vl.Name + " [" + vl.ValueListId + "]" + (vl.IsActiveBubble() ? "" : " [INACTIVE]") + " List Items: " + vl.ValueListItems.Count(),
					Selected = vl.ValueListId == selectedValueListId
				});
		}

		public static IEnumerable<SelectListItem> PageTemplateList(this HtmlHelper htmlHelper)
		{
			int selectedPageTemplateId = (htmlHelper.ViewData.Model as int?) ?? 0;

			List<PageTemplate> pageTemplates = htmlHelper.CurrentClient(true).PageTemplates.AsQueryable().ApplyDefaultSort().ToList();
			return pageTemplates.Select(pt =>
				new SelectListItem()
				{
					Value = pt.PageTemplateId.ToString(),
					Text = pt.Name + " [" + pt.PageTemplateId + "]" + (pt.IsActiveBubble() ? "" : " [INACTIVE]") + " Sections: " + pt.Sections.Count(),
					Selected = pt.PageTemplateId == selectedPageTemplateId
				});
		}

		public static IEnumerable<SelectListItem> ProductCategoryList(this HtmlHelper htmlHelper, bool showProductCount = true)
		{
			int selectedProductCategoryId = (htmlHelper.ViewData.Model as int?) ?? 0;

			List<ProductCategory> categories = htmlHelper.CurrentStoreFront(true).ProductCategories.AsQueryable().ApplyDefaultSort().ToList();
			return categories.Select(c =>
				new SelectListItem()
				{
					Value = c.ProductCategoryId.ToString(),
					Text = c.Name + " [" + c.ProductCategoryId + "]" + (c.IsActiveBubble() ? "" : " [INACTIVE]") + " Url Name: " + c.UrlName + (showProductCount ? " Products: " + c.Products.Count : ""),
					Selected = c.ProductCategoryId == selectedProductCategoryId
				});
		}

		public static IEnumerable<SelectListItem> WebFormList(this HtmlHelper htmlHelper)
		{
			int selectedWebFormId = (htmlHelper.ViewData.Model as int?) ?? 0;

			List<WebForm> webForms = htmlHelper.CurrentClient(true).WebForms.AsQueryable().ApplyDefaultSort().ToList();
			return webForms.Select(wf =>
				new SelectListItem()
				{
					Value = wf.WebFormId.ToString(),
					Text = wf.Name + " [" + wf.WebFormId + "]" + (wf.IsActiveBubble() ? "" : " [INACTIVE]") + " Fields: " + wf.WebFormFields.Count(),
					Selected = wf.WebFormId == selectedWebFormId
				});
		}

		public static DateTime ToStartDateTimeUtc(this DashboardDateTimeRange value)
		{
			switch (value)
			{
				case DashboardDateTimeRange.Past15Minutes:
					return DateTime.UtcNow.AddMinutes(-15);
				case DashboardDateTimeRange.PastHour:
					return DateTime.UtcNow.AddHours(-1);
				case DashboardDateTimeRange.Past7Days:
					return DateTime.UtcNow.AddDays(-7);
				case DashboardDateTimeRange.Past24Hours:
					return DateTime.UtcNow.AddHours(-24);
				case DashboardDateTimeRange.Past48Hours:
					return DateTime.UtcNow.AddHours(-48);
				case DashboardDateTimeRange.Past72Hours:
					return DateTime.UtcNow.AddHours(-72);
				case DashboardDateTimeRange.Past30Days:
					return DateTime.UtcNow.AddDays(-30);
				case DashboardDateTimeRange.Past60Days:
					return DateTime.UtcNow.AddDays(-60);
				case DashboardDateTimeRange.Past90Days:
					return DateTime.UtcNow.AddDays(-90);
				case DashboardDateTimeRange.Past6Months:
					return DateTime.UtcNow.AddMonths(-6);
				case DashboardDateTimeRange.PastYear:
					return DateTime.UtcNow.AddYears(-1);
				case DashboardDateTimeRange.AllTime:
					return new DateTime(2000, 1, 1);
				default:
					throw new ApplicationException("Unknown DashboardDateTimeRange: " + value.ToString());
			}

		}

		public static DateTime ToEndDateTimeUtc(this DashboardDateTimeRange value)
		{
			return DateTime.UtcNow;
		}

		public static Product ProductFromUrlName(this HtmlHelper htmlHelper, string productUrlName)
		{
			if (string.IsNullOrWhiteSpace(productUrlName))
			{
				throw new ArgumentNullException("productUrlName");
			}
			return htmlHelper.CurrentStoreFront(true).Products.SingleOrDefault(p => p.UrlName.ToLower() == productUrlName.ToLower());
		}

		public static ProductCategory ProductCategoryFromUrlName(this HtmlHelper htmlHelper, string categoryUrlName)
		{
			if (string.IsNullOrWhiteSpace(categoryUrlName))
			{
				throw new ArgumentNullException("categoryUrlName");
			}
			return htmlHelper.CurrentStoreFront(true).ProductCategories.SingleOrDefault(p => p.UrlName.ToLower() == categoryUrlName.ToLower());
		}

	}

	public enum DashboardDateTimeRange
	{
		[Display(Name = "the Past 15 Minutes")]
		Past15Minutes = 1,

		[Display(Name = "the Past 60 Minutes")]
		PastHour,

		[Display(Name = "the Past Day (24 hours)")]
		Past24Hours,

		[Display(Name = "the Past 2 days (48 hours)")]
		Past48Hours,

		[Display(Name = "the Past 3 days (72 hours)")]
		Past72Hours,

		[Display(Name = "the Past 7 Days")]
		Past7Days,

		[Display(Name = "the past 30 days")]
		Past30Days,

		[Display(Name = "the past 2 months (60 days)")]
		Past60Days,

		[Display(Name = "the past 3 months (90 days)")]
		Past90Days,

		[Display(Name = "the past 6 months")]
		Past6Months,

		[Display(Name = "the Past Year")]
		PastYear,

		[Display(Name = "All Time")]
		AllTime
	}
}