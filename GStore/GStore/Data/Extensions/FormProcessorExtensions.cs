using GStore.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using GStore.Controllers.BaseClass;
using System.Collections.Generic;
using GStore.AppHtmlHelpers;
using System.Web.Mvc;

namespace GStore.Data
{
	public static class FormProcessorExtensions
	{
		public static bool ProcessWebForm(IGstoreDb db, ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, UserProfile userProfile, HttpRequestBase request, bool isRegisterPage)
		{
			if (modelStateDictionary == null)
			{
				throw new ArgumentNullException("modelStateDictionary");
			}
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (page == null && (!isRegisterPage))
			{
				throw new ArgumentNullException("page");
			}
			if (!isRegisterPage && !(page.WebFormSaveToDatabase || page.WebFormSaveToFile || page.WebFormSendToEmail))
			{
				throw new ArgumentException("No form processing specified in Page. One of the following must be enabled: page.WebFormSaveToDatabase or page.WebFormSaveToFile or page.WebFormSendToEmail", "page");
			}

			ValidateFields(modelStateDictionary, webForm, request);
			if (!modelStateDictionary.IsValid)
			{
				return false;
			}

			StoreFront storeFront = null;
			if (page != null)
			{
				storeFront = page.StoreFront;
			}
			else if ((userProfile != null) && (userProfile.StoreFront != null))
			{
				storeFront = userProfile.StoreFront;
			}
			else
			{
				throw new ApplicationException("Could not determine store front for Web Form. Page = null and User Profile = null, at least one must be valid to process a form.");
			}

			string formBodyText = BuildFormBodyText(modelStateDictionary, webForm, page, storeFront, userProfile, request);
			string formSubject = BuildFormSubject(modelStateDictionary, webForm, page, storeFront, userProfile, request);

			if (page == null || page.WebFormSaveToDatabase)
			{
				//register form always goes to database
				ProcessWebForm_ToDatabase(db, modelStateDictionary, webForm, page, userProfile, request, storeFront, formSubject, formBodyText);
			}

			if (isRegisterPage)
			{
				bool registered = ProcessWebForm_Register(modelStateDictionary, webForm, page, userProfile, request);
			}
			else
			{
				bool savedToFile = false;
				bool emailed = false;
				if (page.WebFormSaveToFile)
				{
					savedToFile = ProcessWebForm_ToFile(modelStateDictionary, webForm, page, storeFront, userProfile, request);
				}
				if (page.WebFormSendToEmail)
				{
					emailed = ProcessWebForm_ToEmail(modelStateDictionary, webForm, page, storeFront, userProfile, request);
				}
			}

			return true;

		}

		public static void ValidateFields(ModelStateDictionary modelStateDictionary, WebForm webForm, HttpRequestBase request)
		{
			List<WebFormField> fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();
			foreach (WebFormField field in fields)
			{
				string formFieldName = "Page.WebForm.item." + field.Name;
				string stringValue = null;
				if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
				{
					stringValue = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				}
				if (stringValue == null)
				{
					formFieldName = "WebForm.item." + field.Name;
					if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
					{
						stringValue = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
					}
				}
				if (stringValue == null)
				{
					formFieldName = "item." + field.Name;
					if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
					{
						stringValue = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
					}
				}
				
				if (!modelStateDictionary.ContainsKey(formFieldName))
				{
					modelStateDictionary.Add(formFieldName, new ModelState() { Value = new ValueProviderResult(stringValue, stringValue, System.Globalization.CultureInfo.CurrentCulture) });
				}


				if (field.IsRequired && string.IsNullOrWhiteSpace(stringValue))
				{
					modelStateDictionary.AddModelError(formFieldName, field.LabelText + " is required.");
				}


			}

		}

		private static void ProcessWebForm_ToDatabase(IGstoreDb db, ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, UserProfile userProfile, HttpRequestBase request, StoreFront storeFront, string formSubject, string formBodyText)
		{
			WebFormResponse webFormResponse = db.WebFormResponses.Create();
			webFormResponse.StoreFrontId = storeFront.StoreFrontId;
			webFormResponse.StoreFront = storeFront;
			webFormResponse.ClientId = storeFront.ClientId;
			webFormResponse.Client = storeFront.Client;
			webFormResponse.PageId = (page == null ? null : (int?)page.PageId);
			webFormResponse.Page = page;
			webFormResponse.WebFormId = webForm.WebFormId;
			webFormResponse.WebForm = webForm;
			webFormResponse.IsPending = false;
			webFormResponse.StartDateTimeUtc = DateTime.UtcNow;
			webFormResponse.EndDateTimeUtc = DateTime.UtcNow;
			webFormResponse.BodyText = formBodyText;
			webFormResponse.Subject = formSubject;
			webFormResponse.SetDefaults(userProfile);

			FillWebFormResponses(db, webFormResponse, modelStateDictionary, webForm, storeFront, userProfile, request);

			db.WebFormResponses.Add(webFormResponse);
			db.SaveChanges();

		}

		private static bool ProcessWebForm_Register(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, UserProfile userProfile, HttpRequestBase request)
		{
			//register data is already added to register notification form and linked up to user
			if (!modelStateDictionary.IsValid)
			{
				return false;
			}

			return true;
		}

		private static bool ProcessWebForm_ToFile(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFront storeFront, UserProfile userProfile, HttpRequestBase request)
		{
			if (!modelStateDictionary.IsValid)
			{
				return false;
			}

			string subject = BuildFormSubject(modelStateDictionary, webForm, page, storeFront, userProfile, request);
			string bodyText = BuildFormBodyText(modelStateDictionary, webForm, page, storeFront, userProfile, request);

			string virtualDir = page.StoreFront.StoreFrontVirtualDirectoryToMap(request.ApplicationPath) + "\\Forms\\" + webForm.Name.ToFileName();
			string fileDir = request.RequestContext.HttpContext.Server.MapPath(virtualDir);
			if (!System.IO.Directory.Exists(fileDir))
			{
				System.IO.Directory.CreateDirectory(fileDir);
			}

			string fileName = DateTime.UtcNow.ToFileSafeString() + System.Guid.NewGuid().ToString() + ".txt";

			System.IO.File.AppendAllText(fileDir + "\\" + fileName, bodyText);

			return true;
		}

		private static bool ProcessWebForm_ToEmail(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFront storeFront, UserProfile userProfile, HttpRequestBase request)
		{

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

			string subject = BuildFormSubject(modelStateDictionary, webForm, page, storeFront, userProfile, request);
			string bodyText = BuildFormBodyText(modelStateDictionary, webForm, page, storeFront, userProfile, request);
			string bodyHtml = bodyText.ToHtmlLines();

			return AppHtmlHelper.SendEmail(page.Client, toEmail, toName, subject, bodyText, bodyHtml, request.Url.Host);

		}

		private static string BuildFormSubject(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFront storeFront, UserProfile userProfile, HttpRequestBase request)
		{
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}

			if (!modelStateDictionary.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormSubject");
			}

			return request.Url.Host + " Form '" + webForm.Name + "' [" + webForm.WebFormId + "] submitted at " + DateTime.Now;
		}

		private static string BuildFormBodyText(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFront storeFront, UserProfile userProfile, HttpRequestBase request)
		{
			if (modelStateDictionary == null)
			{
				throw new ArgumentNullException("modelStateDictionary");
			}
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}
			if (!modelStateDictionary.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormBodyText");
			}

			StringBuilder textBody = new StringBuilder();
			textBody.AppendLine(BuildFormSubject(modelStateDictionary, webForm, page, storeFront, userProfile, request));
			textBody.AppendLine();

			List<WebFormField> fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();

			foreach (WebFormField field in fields)
			{
				string value1 = request.GetFormFieldValue1String(field);
				string value2 = request.GetFormFieldValue2String(field);

				if (string.IsNullOrEmpty(value1))
				{
					value1 = "(blank)";
				}

				textBody.AppendLine(field.Name + " = " + value1 + (string.IsNullOrEmpty(value2) ? string.Empty : ", " + value2) + "\n");
			}
			textBody.AppendLine();
			textBody.AppendLine("- - - - - - - -");
			//header info
			textBody.AppendLine("User: " + (userProfile == null ? "(anonymous)" : "'" + userProfile.FullName + "' <" + userProfile.Email + ">"));
			textBody.AppendLine("Url: " + request.Url.ToString());
			textBody.AppendLine("Store Front: " + storeFront.Name + " [" + storeFront.StoreFrontId + "] Url: " + storeFront.PublicUrl);
			textBody.AppendLine("Client: " + storeFront.Client.Name + " [" + storeFront.ClientId + "]");
			textBody.AppendLine("Host: " + request.Url.Host);
			textBody.AppendLine("Raw Url: " + request.RawUrl);
			textBody.AppendLine("IP Address: " + request.UserHostAddress);
			textBody.AppendLine();


			return textBody.ToString();
		}

		public static void FillWebFormResponses(IGstoreDb db, WebFormResponse webFormResponse, ModelStateDictionary modelStateDictionary, WebForm webForm, StoreFront storeFront, UserProfile userProfile, HttpRequestBase request)
		{
			if (db == null)
			{
				throw new ArgumentNullException("db");
			}
			if (webFormResponse == null)
			{
				throw new ArgumentNullException("webFormResponse");
			}
			if (webForm == null)
			{
				throw new ArgumentNullException("webFormResponse");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			List<WebFormField> fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();

			foreach (WebFormField field in fields)
			{
				WebFormFieldResponse fieldResponse = db.WebFormFieldResponses.Create();
				fieldResponse.WebFormField = field;
				fieldResponse.WebFormFieldId = field.WebFormFieldId;
				fieldResponse.ClientId = storeFront.ClientId;
				fieldResponse.Client = storeFront.Client;
				fieldResponse.StoreFrontId = storeFront.StoreFrontId;
				fieldResponse.StoreFront = storeFront;
				fieldResponse.DataType = field.DataType;
				fieldResponse.DataTypeString = field.DataTypeString;
				fieldResponse.IsPending = false;
				fieldResponse.StartDateTimeUtc = DateTime.UtcNow;
				fieldResponse.EndDateTimeUtc = DateTime.UtcNow;
				fieldResponse.WebFormFieldLabelText = field.LabelText;
				fieldResponse.WebFormFieldName = field.Name;
				fieldResponse.WebFormFieldOrder = field.Order;
				fieldResponse.WebFormName = field.WebForm.Name;
				fieldResponse.WebFormOrder = field.WebForm.Order;

				fieldResponse.SetValueFieldsFromFormValues(request);

				fieldResponse.SetDefaults(userProfile);


				db.WebFormFieldResponses.Add(fieldResponse);
			}

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

		public static void SetValueFieldsFromFormValues(this WebFormFieldResponse data, HttpRequestBase request)
		{
			if (data == null)
			{
				throw new ArgumentNullException("webFormFieldResponse");
			}
			if (request == null)
			{
				throw new ArgumentNullException("webFormFieldResponse");
			}

			if (data.WebFormField == null)
			{
				throw new ArgumentException("webFormFieldResponse.WebFormField is null. Be sure to set webFormFieldResponse.WebFormField object before calling this method", "webFormFieldResponse.WebFormField");
			}
			if (!Enum.IsDefined(typeof(GStoreValueDataType), data.DataType))
			{
				throw new ArgumentException("webFormFieldResponse.DataType value '" + data.DataType.ToString() + "' is invalid.  is null. Be sure to set this value on the webFormFieldResponse object before calling this method", "webFormFieldResponse.DataType");
			}

			data.Value1String = request.GetFormFieldValue1String(data.WebFormField);
			data.Value2String = request.GetFormFieldValue2String(data.WebFormField);
			
			switch (data.DataType)
			{
				case GStoreValueDataType.CheckboxYesNo:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						if (
								data.Value1String.Trim().ToLower() == "1"
								|| data.Value1String.Trim().ToLower() == "true"
								|| data.Value1String.Trim().ToLower() == "t"
								|| data.Value1String.Trim().ToLower() == "y"
								|| data.Value1String.Trim().ToLower() == "yes"
								|| data.Value1String.Trim().ToLower() == "checked"
								|| data.Value1String.Trim().ToLower() == "check"
							)
							data.Value1Bool = true;
						else
						{
							data.Value1Bool = false;
						}
					}
					break;

				case GStoreValueDataType.Integer:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						int result;
						if (int.TryParse(data.Value1String, out result))
						{
							data.Value1Int = result;
						}
					}
					break;

				case GStoreValueDataType.Decimal:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						decimal result;
						if (decimal.TryParse(data.Value1String, out result))
						{
							data.Value1Decimal = result;
						}
					}
					break;

				case GStoreValueDataType.IntegerRange:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						int result1;
						if (int.TryParse(data.Value1String, out result1))
						{
							data.Value1Int = result1;
						}
					}
					if (!string.IsNullOrWhiteSpace(data.Value2String))
					{
						int result2;
						if (int.TryParse(data.Value2String, out result2))
						{
							data.Value2Int = result2;
						}
					}
					break;

				case GStoreValueDataType.DecimalRange:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						decimal result1;
						if (decimal.TryParse(data.Value1String, out result1))
						{
							data.Value1Decimal = result1;
						}
					}
					if (!string.IsNullOrWhiteSpace(data.Value2String))
					{
						decimal result2;
						if (decimal.TryParse(data.Value2String, out result2))
						{
							data.Value2Decimal = result2;
						}
					}
					break;

				case GStoreValueDataType.SingleLineText:
					//do nothing string value already set
					break;
				case GStoreValueDataType.MultiLineText:
					//do nothing string value already set
					break;
				case GStoreValueDataType.Html:
					//do nothing string value already set
					break;
				case GStoreValueDataType.ValueListItemDropdown:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						int result;
						if (int.TryParse(data.Value1String, out result))
						{
							if (result != 0)
							{
								data.Value1ValueListItemId = result;
							}
						}
					}
					break;
				case GStoreValueDataType.ValueListItemRadio:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						int result;
						if (int.TryParse(data.Value1String, out result))
						{
							if (result != 0)
							{
								data.Value1ValueListItemId = result;
							}
						}
					}
					break;
				case GStoreValueDataType.ValueListItemMultiCheckbox:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						int result;
						string[] split = data.Value1String.Split(',');
						foreach(string value in split)
						{
							if (!string.IsNullOrWhiteSpace(value))
							{
								if (int.TryParse(value.Trim(), out result))
								{
									if (result != 0)
									{
										data.Value1ValueListItemId = result;
									}
								}
							}
						}
					}
					break;
				case GStoreValueDataType.ExternalLinkToPage:
					//do nothing link url will be value 1, link text will be value 2
					break;
				case GStoreValueDataType.ExternalLinkToImage:
					//do nothing link url will be value 1, link text will be value 2
					break;
				case GStoreValueDataType.InternalLinkToAction:
					//do nothing route values will be value 1, link text will be value 2
					break;
				case GStoreValueDataType.InternalLinkToPageById:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						int result;
						if (int.TryParse(data.Value1String, out result))
						{
							if (result != 0)
							{
								data.Value1PageId = result;
							}
						}
					}
					break;
				case GStoreValueDataType.InternalLinkToPageByUrl:
					//do nothing route values will be value 1, link text will be value 2
					break;
				case GStoreValueDataType.InternalLinkToImageByUrl:
					//do nothing route values will be value 1, link text will be value 2
					break;
				default:
					break;
			}

		}

		public static string GetFormFieldValue1String(this HttpRequestBase request, WebFormField webFormField)
		{
			string formFieldName = "Page.WebForm.item." + webFormField.Name;

			string value1 = string.Empty;
			if (request.Form.AllKeys.Contains(formFieldName))
			{
				value1 = request.Unvalidated.Form.Get(formFieldName);
			}

			return value1;
		}

		public static string GetFormFieldValue2String(this HttpRequestBase request, WebFormField webFormField)
		{
			string formFieldName = "Page.WebForm.item." + webFormField.Name;

			string value2 = string.Empty;
			if (request.Form.AllKeys.Contains(formFieldName + "_Value2"))
			{
				value2 = request.Unvalidated.Form.Get(formFieldName + "_Value2");
			}

			return value2;
		}

	}
}