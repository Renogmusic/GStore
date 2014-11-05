using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GStore.Models.Extensions;

namespace GStore.Controllers.BaseClass
{
	/// <summary>
	/// 
	/// Dynamic page controller base class for common functionality
	/// </summary>
    public abstract class PageBaseController : BaseController
    {
		Models.Page _page = null;
		public Models.Page CurrentPage
		{
			get
			{
				if (_page == null)
				{
					try
					{
						_page = CurrentStoreFront.GetCurrentPage(Request);
					}
					catch(Exceptions.DynamicPageNotFoundException dnfException)
					{
						string message = dnfException.Message;
						HttpNotFound("Page Not Found for url: " + dnfException.Url);
					}
					catch (Exception ex)
					{
						throw new ApplicationException("Error getting active dynamic page", ex);
					}
				}
				return _page;
			}
		}
    }
}

