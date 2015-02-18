using GStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using GStore.Data;
using System.Web.Mvc;
using GStore.AppHtmlHelpers;

namespace GStore.Areas.CatalogAdmin.ViewModels
{
	public class DigitalDownloadFileError
	{
		public DigitalDownloadFileError(Product product, string errorMessage)
		{
			if (product == null)
			{
				throw new ArgumentNullException("product");
			}

			this.ErrorMessage = errorMessage;
		}

		public string ErrorMessage { get; set; }

	}
}