using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Controllers
{
	/// <summary>
	/// Dynamic Url Rewriteer for file requests to the storefront folder, or the client folder, server folder, or file not found pages
	/// </summary>
    public class StoreFrontFileController : BaseClass.BaseController
    {
		public ActionResult Images(string path)
		{
			return StoreFile("/Images/" + path, true);
		}

		public ActionResult Styles(string path)
		{
			return StoreFile("/Styles/" + path);
		}

		public ActionResult Scripts(string path)
		{
			return StoreFile("/Scripts/" + path);
		}

		public ActionResult Themes(string path)
		{
			return StoreFile("/Themes/" + path);
		}

		public ActionResult Fonts(string path)
		{
			return StoreFile("/Fonts/" + path);
		}


		/// <summary>
		/// Checks if a file exists in storefront folder, then client folder, then server folder, otherwise returns 404
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		protected ActionResult StoreFile(string path, bool isImage = false)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return HttpForbidden("Directory Listing Denied: /Store");
			}

			Models.StoreFront storeFront = base.CurrentStoreFront;
			string fullPath = ChooseFilePath(storeFront, path);

			if (string.IsNullOrEmpty(fullPath) && isImage)
			{
				path = "Images/NotFound.png";
				fullPath = ChooseFilePath(storeFront, path);
			}

			if (string.IsNullOrEmpty(fullPath))
			{
				return HttpNotFound((isImage ? "Image" : "Store File") + " not found: " + path);
			}

			string mimeType = MimeMapping.GetMimeMapping(fullPath);
			return new FilePathResult(fullPath, mimeType);
		}

		private string ChooseFilePath(Models.StoreFront storeFront, string path)
		{
			string fullVirtualPath = storeFront.StoreFrontVirtualDirectoryToMap() + "/" + path;
			string fullPath = Server.MapPath(fullVirtualPath);
			if (!System.IO.File.Exists(fullPath))
			{
				fullVirtualPath = storeFront.ClientVirtualDirectoryToMap() + "/" + path;
				fullPath = Server.MapPath(fullVirtualPath);
				if (!System.IO.File.Exists(fullPath))
				{
					fullVirtualPath = "~/Content/Server/" + path;
					fullPath = Server.MapPath(fullVirtualPath);
					if (!System.IO.File.Exists(fullPath))
					{
						return null;
					}
				}
			}

			return fullPath;
		}

	}
}