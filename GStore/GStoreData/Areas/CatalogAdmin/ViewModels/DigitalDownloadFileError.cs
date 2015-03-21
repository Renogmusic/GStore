using System;
using GStoreData.Models;

namespace GStoreData.Areas.CatalogAdmin.ViewModels
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