﻿using System;
using System.Web;
using System.Web.Mvc;
using GStoreData;
using GStoreData.AppHtmlHelpers;
using GStoreData.Models;

namespace GStoreWeb.Controllers
{
	/// <summary>
	/// Dynamic Url Rewriteer for file requests to the storefront folder, or the client folder, server folder, or file not found pages
	/// </summary>
	public class StoreFrontFileController : AreaBaseController.RootAreaBaseController
    {
		public StoreFrontFileController()
		{
			this.LogActionsAsPageViews = false;
		}

		public ActionResult Images(string path)
		{
			return StoreFile("/Images/" + path, true);
		}

		public ActionResult Audio(string path)
		{
			return StoreFile("/Audio/" + path);
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

		public ActionResult Pages(string path, bool useNullContentType = false)
		{
			return StoreFile("/Pages/" + path);
		}

		public ActionResult CatalogContent(string path)
		{
			bool isImage = path.FileExtensionIsImage();
			return StoreFile("/CatalogContent/" + path, isImage);
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
				return HttpForbidden("Directory Listing Denied: /");
			}

			StoreFront storeFront = null;
			try
			{
				storeFront = base.CurrentStoreFrontOrThrow;
			}
			catch (Exception ex)
			{
				if (ex is GStoreData.Exceptions.StoreFrontInactiveException || ex.GetBaseException() is GStoreData.Exceptions.StoreFrontInactiveException)
				{
					//storefront is inactive, just show server file
					System.Diagnostics.Debug.Print("--StoreFile - StoreFront is inactive for file: " + path);
				}
				else if (ex is GStoreData.Exceptions.NoMatchingBindingException || ex.GetBaseException() is GStoreData.Exceptions.NoMatchingBindingException)
				{
					//no matching binding, just show server file
					System.Diagnostics.Debug.Print("--StoreFile - No matching binding found; for file: " + path);
				}
				else if (ex is GStoreData.Exceptions.DatabaseErrorException || ex.GetBaseException() is GStoreData.Exceptions.DatabaseErrorException)
				{
					//databse error. just show server file
					System.Diagnostics.Debug.Print("--StoreFile - Database Error: " + ex.Message + " - for file: " + path);
				}
				else
				{
					throw new ApplicationException("Error getting CurrentStoreFront for StoreFile controller.", ex);
				}
			}

			Client client = null;
			if (storeFront != null)
			{
				client = storeFront.Client;
			}

			string fullPath = storeFront.ChooseFilePath(client, path, Request.ApplicationPath, Server);

			if (string.IsNullOrEmpty(fullPath) && isImage)
			{
				path = "Images/NotFound.png";
				fullPath = storeFront.ChooseFilePath(client, path, Request.ApplicationPath, Server);
			}

			if (string.IsNullOrEmpty(fullPath))
			{
				return HttpNotFound((isImage ? "Image" : "Store File") + " not found: " + path);
			}

			string mimeType = MimeMapping.GetMimeMapping(fullPath);
			return new FilePathResult(fullPath, mimeType);
		}

		protected override string ThemeFolderName
		{
			get
			{
				return CurrentStoreFrontConfigOrThrow.DefaultNewPageTheme.FolderName;
			}
		}

	}
}
