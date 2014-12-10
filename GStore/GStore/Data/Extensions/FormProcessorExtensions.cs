using GStore.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using GStore.Controllers.BaseClass;
using System.Collections.Generic;
using GStore.AppHtmlHelpers;


namespace GStore.Data
{
	public enum WebFormProcessorType
	{
		[Display(Name = "Save to Store Front Forms Folder and Email Results")]
		FileAndEmailFormProcessor = 100,

		[Display(Name = "Save to Store Front Forms Folder")]
		FileFormProcessor = 120,

		[Display(Name = "Register User Processor")]
		RegisterUserFormProcessor = 200,

	}

	public static class FormProcessorExtensions
	{
		public static bool ProcessWebForm(BaseController controller, WebForm webForm, Page page, UserProfile profile, HttpRequestBase request)
		{
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			ValidateFields(controller, webForm, page, profile, request);
			if (!controller.ModelState.IsValid)
			{
				return false;
			}

			switch (page.WebFormProcessorType)
			{
				case WebFormProcessorType.FileAndEmailFormProcessor:
					return ProcessWebForm_FileAndEmail(controller, webForm, page, profile, request);
				case WebFormProcessorType.FileFormProcessor:
					return ProcessWebForm_File(controller, webForm, page, profile, request);
				case WebFormProcessorType.RegisterUserFormProcessor:
					return ProcessWebForm_Register(controller, webForm, page, profile, request);
				default:
					throw new ApplicationException("Unknown Web Form Processor Type: " + page.WebFormProcessorType.ToString() + " StringValue: " + page.WebFormProcessorType.ToDisplayName());
			}
		}

		private static void ValidateFields(BaseController controller, WebForm webForm, Page page, UserProfile profile, HttpRequestBase request)
		{
			List<WebFormField> requiredFields = webForm.WebFormFields.AsQueryable().Where(f => f.IsRequired).WhereIsActive().ApplySortDefault().ToList();
			foreach (WebFormField field in requiredFields)
			{
				string formFieldName = "Page.WebForm.item." + field.Name;
				if (!request.Form.AllKeys.Contains(formFieldName) || string.IsNullOrWhiteSpace(request.Form.Get(formFieldName)))
				{
					controller.ModelState.AddModelError(formFieldName, field.LabelText + " is required.");
				}
			}
		}

		private static bool ProcessWebForm_Register(BaseController controller, WebForm webForm, Page page, UserProfile profile, HttpRequestBase request)
		{
			if (!controller.ModelState.IsValid)
			{
				return false;
			}

			throw new NotImplementedException();
		}

		private static bool ProcessWebForm_File(BaseController controller, WebForm webForm, Page page, UserProfile profile, HttpRequestBase request)
		{
			if (!controller.ModelState.IsValid)
			{
				return false;
			}

			string subject = BuildFormSubject(controller, webForm, page, profile, request);
			string bodyText = BuildFormBodyText(controller, webForm, page, profile, request);

			string virtualDir = page.StoreFront.StoreFrontVirtualDirectoryToMap(request.ApplicationPath) + "\\Forms\\" + webForm.Name.ToFileName();
			string fileDir = controller.Server.MapPath(virtualDir);
			if (!System.IO.Directory.Exists(fileDir))
			{
				System.IO.Directory.CreateDirectory(fileDir);
			}

			string fileName = DateTime.UtcNow.ToFileSafeString() + System.Guid.NewGuid().ToString() + ".txt";

			System.IO.File.AppendAllText(fileDir + "\\" + fileName, bodyText);

			return true;
		}

		private static bool ProcessWebForm_FileAndEmail(BaseController controller, WebForm webForm, Page page, UserProfile profile, HttpRequestBase request)
		{

			if (!ProcessWebForm_File(controller, webForm, page, profile, request))
			{
				//file log failed, return false
				return false;
			}

			if (!Properties.Settings.Current.AppEnableEmail || !page.StoreFront.Client.UseSendGridEmail)
			{
				//email not enabled, don't send and fail silently
				return true;
			}

			string toEmail = page.WebFormEmailToAddress;
			if (string.IsNullOrWhiteSpace(toEmail))
			{
				toEmail = page.StoreFront.RegisteredNotify.Email;
			}
			string toName = page.WebFormEmailToName;
			if (string.IsNullOrWhiteSpace(toName))
			{
				toName = page.StoreFront.RegisteredNotify.FullName;
			}

			string subject = BuildFormSubject(controller, webForm, page, profile, request);
			string bodyText = BuildFormBodyText(controller, webForm, page, profile, request);
			string bodyHtml = bodyText.ToHtmlLines();

			return AppHtmlHelper.SendEmail(page.Client, toEmail, toName, subject, bodyText, bodyHtml, request.Url.Host);

		}

		private static string BuildFormSubject(BaseController controller, WebForm webForm, Page page, UserProfile profile, HttpRequestBase request)
		{
			if (!controller.ModelState.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormSubject");
			}

			return request.Url.Host + " Form '" + webForm.Name + "' [" + webForm.WebFormId + "] submitted at " + DateTime.Now;
		}

		private static string BuildFormBodyText(BaseController controller, WebForm webForm, Page page, UserProfile profile, HttpRequestBase request)
		{
			if (!controller.ModelState.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormBodyText");
			}

			StringBuilder textBody = new StringBuilder();
			textBody.AppendLine(BuildFormSubject(controller, webForm, page, profile, request));
			textBody.AppendLine();

			//header info
			textBody.AppendLine("Client: " + page.Client.Name + " [" + page.ClientId + "]");
			textBody.AppendLine("Store Front: " + page.StoreFront.Name + " [" + page.StoreFrontId + "]");
			textBody.AppendLine("Store Front Url: " + page.StoreFront.PublicUrl);
			textBody.AppendLine("Host: " + request.Url.Host);
			textBody.AppendLine("Url: " + request.Url.ToString());
			textBody.AppendLine("Raw Url: " + request.RawUrl);
			textBody.AppendLine("User Email: " + (profile == null ? "(blank)" : profile.Email));
			textBody.AppendLine("User Name: " + (profile == null ? "(blank)" : profile.FullName));
			textBody.AppendLine("IP Address: " + request.UserHostAddress);
			textBody.AppendLine();

			var fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();

			foreach (WebFormField field in fields)
			{
				string formFieldName = "Page.WebForm.item." + field.Name;
				string value = "(blank)";
				if (request.Form.AllKeys.Contains(formFieldName))
				{
					value = request.Form.Get(formFieldName);
				}
				textBody.AppendLine(field.Name + " = " + value + "\n");
			}
			textBody.AppendLine("-- end of form --");

			return textBody.ToString();
		}

		private static string ToFileName(this string value)
		{
			return value.Replace('/', '_')
				.Replace('\\', '_')
				.Replace(':', '_')
				.Replace('*', '_')
				.Replace('?', '_')
				.Replace('"', '_')
				.Replace('<', '_')
				.Replace('>', '_')
				.Replace('|', '_')
				.Replace('.', '_');
		}

	}
}