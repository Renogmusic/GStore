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
			StoreFrontConfiguration storeFrontConfiguration = storeFront.CurrentConfig();
			if (storeFrontConfiguration == null)
			{
				throw new ApplicationException("Could not determine current configuration of store front for Web Form. Store Front must be active and have an active configuration to process a form.");
			}

			string formBodyText = BuildFormBodyText(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request, false);
			string formSubject = BuildFormSubject(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request);

			if (page == null || page.WebFormSaveToDatabase)
			{
				//register form always goes to database
				ProcessWebForm_ToDatabase(db, modelStateDictionary, webForm, page, userProfile, request, storeFrontConfiguration, formSubject, formBodyText, isRegisterPage);
			}

			if (!isRegisterPage)
			{
				bool savedToFile = false;
				bool emailed = false;
				if (page.WebFormSaveToFile)
				{
					savedToFile = ProcessWebForm_ToFile(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request);
				}
				if (page.WebFormSendToEmail)
				{
					emailed = ProcessWebForm_ToEmail(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request);
				}
			}

			return true;

		}

		public static string BodyTextCustomFieldsOnly(WebForm webForm, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile, HttpRequestBase request)
		{
			return BuildFormBodyText(null, webForm, null, storeFrontConfiguration, userProfile, request, true);

		}

		public static bool ValidateFields(ModelStateDictionary modelStateDictionary, WebForm webForm, HttpRequestBase request)
		{
			bool isValid = true;
			List<WebFormField> fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();
			foreach (WebFormField field in fields)
			{
				string formFieldName = "Page.WebForm.item." + field.Name;
				string stringValue1 = null;
				if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
				{
					stringValue1 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				}
				if (stringValue1 == null)
				{
					formFieldName = "WebForm.item." + field.Name;
					if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
					{
						stringValue1 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
					}
				}
				if (stringValue1 == null)
				{
					formFieldName = "item." + field.Name;
					if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
					{
						stringValue1 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
					}
				}
				if (!modelStateDictionary.ContainsKey(formFieldName))
				{
					modelStateDictionary.Add(formFieldName, new ModelState() { Value = new ValueProviderResult(stringValue1, stringValue1, System.Globalization.CultureInfo.CurrentCulture) });
				}

				string formFieldNameValue2 = formFieldName + "_Value2";
				string stringValue2 = null;
				stringValue2 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				if (!modelStateDictionary.ContainsKey(formFieldNameValue2))
				{
					modelStateDictionary.Add(formFieldNameValue2, new ModelState() { Value = new ValueProviderResult(stringValue2, stringValue2, System.Globalization.CultureInfo.CurrentCulture) });
				}


				if (field.IsRequired && string.IsNullOrWhiteSpace(stringValue1))
				{
					isValid = false;
					modelStateDictionary.AddModelError(formFieldName, field.LabelText + " is required.");
				}
				else
				{
					bool result = false;
					switch (field.DataType)
					{
						case GStoreValueDataType.EmailAddress:
							result = ValidateEmailAddress(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.Url:
							result = ValidateUrl(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.SingleLineText:
							result = ValidateSingleLineText(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.MultiLineText:
							result = ValidateMultiLineText(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.CheckboxYesNo:
							result = ValidateCheckboxYesNo(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.ValueListItemDropdown:
							result = ValidateValueListItemDropdown(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.ValueListItemRadio:
							result = ValidateValueListItemRadio(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.ValueListItemMultiCheckbox:
							result = ValidateValueListItemMultiCheckbox(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.Integer:
							result = ValidateInteger(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.Decimal:
							result = ValidateDecimal(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.IntegerRange:
							result = ValidateIntegerRange(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.DecimalRange:
							result = ValidateDecimalRange(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.Html:
							result = ValidateHtml(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.ExternalLinkToPage:
							result = ValidateExternalLinkToPage(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.ExternalLinkToImage:
							result = ValidateExternalLinkToImage(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.InternalLinkToPageById:
							result = ValidateInternalLinkToPageById(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.InternalLinkToPageByUrl:
							result = ValidateInternalLinkToPageByUrl(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						case GStoreValueDataType.InternalLinkToImageByUrl:
							result = ValidateInternalLinkToImageByUrl(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, modelStateDictionary);
							break;
						default:
							modelStateDictionary.AddModelError("", "Unknown data type: " + field.DataType.ToString() + "[" + (int)field.DataType + "]");
							break;
					}

					if (result == false)
					{
						isValid = false;
					}
				}

			}

			return isValid;

		}

		private static void ProcessWebForm_ToDatabase(IGstoreDb db, ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, UserProfile userProfile, HttpRequestBase request, StoreFrontConfiguration storeFrontConfiguration, string formSubject, string formBodyText, bool isRegisterPage)
		{
			WebFormResponse webFormResponse = db.WebFormResponses.Create();
			webFormResponse.StoreFrontId = storeFrontConfiguration.StoreFrontId;
			webFormResponse.StoreFront = storeFrontConfiguration.StoreFront;
			webFormResponse.ClientId = storeFrontConfiguration.ClientId;
			webFormResponse.Client = storeFrontConfiguration.Client;
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

			FillWebFormResponses(db, webFormResponse, modelStateDictionary, webForm, storeFrontConfiguration, userProfile, request);

			webFormResponse = db.WebFormResponses.Add(webFormResponse);
			db.SaveChanges();

			if (isRegisterPage)
			{
				userProfile.RegisterWebFormResponseId = webFormResponse.WebFormResponseId;
				db.UserProfiles.Update(userProfile);
				db.SaveChangesDirect();
			}
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

		private static bool ProcessWebForm_ToFile(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile, HttpRequestBase request)
		{
			if (!modelStateDictionary.IsValid)
			{
				return false;
			}

			string subject = BuildFormSubject(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request);
			string bodyText = BuildFormBodyText(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request, false);

			string virtualDir = storeFrontConfiguration.StoreFront.StoreFrontVirtualDirectoryToMap(request.ApplicationPath) + "\\Forms\\" + webForm.Name.ToFileName();
			string fileDir = request.RequestContext.HttpContext.Server.MapPath(virtualDir);
			if (!System.IO.Directory.Exists(fileDir))
			{
				System.IO.Directory.CreateDirectory(fileDir);
			}

			string fileName = DateTime.UtcNow.ToFileSafeString() + System.Guid.NewGuid().ToString() + ".txt";

			System.IO.File.AppendAllText(fileDir + "\\" + fileName, bodyText);

			return true;
		}

		private static bool ProcessWebForm_ToEmail(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile, HttpRequestBase request)
		{

			if (!Settings.AppEnableEmail || !page.StoreFront.Client.UseSendGridEmail)
			{
				//email not enabled, don't send and fail silently
				return true;
			}

			string toEmail = page.WebFormEmailToAddress;
			if (string.IsNullOrWhiteSpace(toEmail))
			{
				toEmail = storeFrontConfiguration.RegisteredNotify.Email;
			}
			string toName = page.WebFormEmailToName;
			if (string.IsNullOrWhiteSpace(toName))
			{
				toName = storeFrontConfiguration.RegisteredNotify.FullName;
			}

			string subject = BuildFormSubject(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request);
			string bodyText = BuildFormBodyText(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request, false);
			string bodyHtml = bodyText.ToHtmlLines();

			return AppHtmlHelper.SendEmail(page.Client, toEmail, toName, subject, bodyText, bodyHtml, request.Url.Host);

		}

		private static string BuildFormSubject(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile, HttpRequestBase request)
		{
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}

			if (!modelStateDictionary.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormSubject");
			}

			return request.Url.Host + " Form '" + webForm.Name + "' [" + webForm.WebFormId + "] submitted at " + DateTime.UtcNow.ToLocalTime();
		}

		private static string BuildFormBodyText(ModelStateDictionary modelStateDictionary, WebForm webForm, Page page, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile, HttpRequestBase request, bool fieldDataOnly)
		{
			if (!fieldDataOnly && modelStateDictionary == null)
			{
				throw new ArgumentNullException("modelStateDictionary");
			}
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}
			if (!fieldDataOnly && !modelStateDictionary.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormBodyText");
			}

			StringBuilder textBody = new StringBuilder();
			if (!fieldDataOnly)
			{
				textBody.AppendLine(BuildFormSubject(modelStateDictionary, webForm, page, storeFrontConfiguration, userProfile, request));
			}
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
				else if (field.DataType == GStoreValueDataType.ValueListItemDropdown || field.DataType == GStoreValueDataType.ValueListItemRadio)
				{
					value1 = field.GetValueListItemName(value1);
				}
				else if (field.DataType == GStoreValueDataType.ValueListItemMultiCheckbox)
				{
					value1 = field.GetValueListItemNameList(value1);
				}


				textBody.AppendLine(field.Name + " = " + value1 + (string.IsNullOrEmpty(value2) ? string.Empty : ", " + value2) + "\n");
			}
			if (!fieldDataOnly)
			{
				textBody.AppendLine();
				textBody.AppendLine("- - - - - - - -");
				//header info
				textBody.AppendLine("User: " + (userProfile == null ? "(anonymous)" : "'" + userProfile.FullName + "' <" + userProfile.Email + ">"));
				textBody.AppendLine("Url: " + request.Url.ToString());
				textBody.AppendLine("Store Front: " + storeFrontConfiguration.Name + " [" + storeFrontConfiguration.StoreFrontId + "] Url: " + storeFrontConfiguration.PublicUrl);
				textBody.AppendLine("Client: " + storeFrontConfiguration.Client.Name + " [" + storeFrontConfiguration.ClientId + "]");
				textBody.AppendLine("Host: " + request.Url.Host);
				textBody.AppendLine("Raw Url: " + request.RawUrl);
				textBody.AppendLine("IP Address: " + request.UserHostAddress);
				textBody.AppendLine("User Agent: " + request.UserAgent);
				textBody.AppendLine();

				HttpSessionStateBase session = request.RequestContext.HttpContext.Session;
				textBody.AppendLine("Session Start Date Time: " + (session.EntryDateTime().HasValue ? session.EntryDateTime().ToString() : "(unknown)"));
				textBody.AppendLine("Session Entry Raw Url: " + (session.EntryRawUrl() ?? "(unknown)"));
				textBody.AppendLine("Session Entry Url: " + (session.EntryUrl() ?? "(unknown)"));
				textBody.AppendLine("Session Referrer: " + (session.EntryReferrer() ?? "(unknown)"));
				textBody.AppendLine();
			}

			return textBody.ToString();
		}

		public static void FillWebFormResponses(IGstoreDb db, WebFormResponse webFormResponse, ModelStateDictionary modelStateDictionary, WebForm webForm, StoreFrontConfiguration storeFrontConfiguration, UserProfile userProfile, HttpRequestBase request)
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
				fieldResponse.ClientId = storeFrontConfiguration.ClientId;
				fieldResponse.Client = storeFrontConfiguration.Client;
				fieldResponse.StoreFrontId = storeFrontConfiguration.StoreFrontId;
				fieldResponse.StoreFront = storeFrontConfiguration.StoreFront;
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
				case GStoreValueDataType.EmailAddress:
				//do nothing string value already set

				case GStoreValueDataType.Url:
				//do nothing string value already set

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
								data.Value1ValueListItemName = data.WebFormField.GetValueListItemName(data.Value1String);
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
								data.Value1ValueListItemName = data.WebFormField.GetValueListItemName(data.Value1String);
							}
						}
					}
					break;
				case GStoreValueDataType.ValueListItemMultiCheckbox:
					if (!string.IsNullOrWhiteSpace(data.Value1String))
					{
						int result;
						string[] split = data.Value1String.Split(',');
						foreach (string value in split)
						{
							if (!string.IsNullOrWhiteSpace(value))
							{
								if (int.TryParse(value.Trim(), out result))
								{
									if (result != 0)
									{
										if (!string.IsNullOrEmpty(data.Value1ValueListItemIdList))
										{
											data.Value1ValueListItemIdList += ",";
										}
										if (!string.IsNullOrEmpty(data.Value1ValueListItemNameList))
										{
											data.Value1ValueListItemNameList += ",";
										}
										data.Value1ValueListItemIdList += result;
										data.Value1ValueListItemNameList += data.WebFormField.GetValueListItemName(value);
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
			string stringValue1 = null;
			if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
			{
				stringValue1 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
			}
			if (stringValue1 == null)
			{
				formFieldName = "WebForm.item." + webFormField.Name;
				if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
				{
					stringValue1 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				}
			}
			if (stringValue1 == null)
			{
				formFieldName = "item." + webFormField.Name;
				if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
				{
					stringValue1 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				}
			}

			return stringValue1;

		}

		public static string GetFormFieldValue2String(this HttpRequestBase request, WebFormField webFormField)
		{
			string formFieldName = "Page.WebForm.item." + webFormField.Name + "_Value2";
			string stringValue2 = null;
			if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
			{
				stringValue2 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
			}
			if (stringValue2 == null)
			{
				formFieldName = "WebForm.item." + webFormField.Name + "_Value2"; ;
				if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
				{
					stringValue2 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				}
			}
			if (stringValue2 == null)
			{
				formFieldName = "item." + webFormField.Name + "_Value2"; ;
				if (request.Unvalidated.Form.AllKeys.Contains(formFieldName))
				{
					stringValue2 = request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				}
			}

			return stringValue2;

		}

		#region Validation Routines

		private static bool ValidateEmailAddress(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			bool isValid = new EmailAddressAttribute().IsValid(stringValue1);
			if (!isValid)
			{
				modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid Email Address.");
			}
			return isValid;
		}
		private static bool ValidateUrl(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			bool isValid = new UrlAttribute().IsValid(stringValue1);
			if (!isValid)
			{
				modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid URL.");
			}
			return isValid;
		}
		private static bool ValidateSingleLineText(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			return true;
		}
		private static bool ValidateMultiLineText(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			return true;
		}
		private static bool ValidateCheckboxYesNo(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			return true;
		}
		private static bool ValidateValueListItemDropdown(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			return true;
		}
		private static bool ValidateValueListItemRadio(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			return true;
		}
		private static bool ValidateValueListItemMultiCheckbox(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			return true;
		}
		private static bool ValidateInteger(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			if (string.IsNullOrEmpty(stringValue1))
			{
				return true;
			}
			int value;
			if (int.TryParse(stringValue1, out value))
			{
				return true;
			}

			modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid whole number.");
			return false;
		}
		private static bool ValidateDecimal(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			if (string.IsNullOrEmpty(stringValue1))
			{
				return true;
			}
			decimal value;
			if (decimal.TryParse(stringValue1, out value))
			{
				return true;
			}

			modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid decimal number.");
			return false;
		}
		private static bool ValidateIntegerRange(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			bool isValid = true;
			if (!string.IsNullOrEmpty(stringValue1))
			{
				int value;
				if (!int.TryParse(stringValue1, out value))
				{
					isValid = false;
					modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid whole number.");
				}
			}
			if (!string.IsNullOrEmpty(stringValue2))
			{
				int value2;
				if (!int.TryParse(stringValue2, out value2))
				{
					isValid = false;
					modelStateDictionary.AddModelError(formFieldNameValue2, labelText + " value '" + stringValue2 + "' is invalid. You must enter a valid whole number.");
				}
			}
			return isValid;
		}
		private static bool ValidateDecimalRange(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			bool isValid = true;
			if (!string.IsNullOrEmpty(stringValue1))
			{
				decimal value;
				if (!decimal.TryParse(stringValue1, out value))
				{
					isValid = false;
					modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid decimal number.");
				}
			}
			if (!string.IsNullOrEmpty(stringValue2))
			{
				decimal value2;
				if (!decimal.TryParse(stringValue2, out value2))
				{
					isValid = false;
					modelStateDictionary.AddModelError(formFieldNameValue2, labelText + " value '" + stringValue2 + "' is invalid. You must enter a valid decimal number.");
				}
			}
			return isValid;
		}
		private static bool ValidateHtml(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			return true;
		}
		private static bool ValidateExternalLinkToPage(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			bool isValid = new UrlAttribute().IsValid(stringValue1);
			if (!isValid)
			{
				modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid URL.");
			}
			return isValid;
		}
		private static bool ValidateExternalLinkToImage(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			bool isValid = new UrlAttribute().IsValid(stringValue1);
			if (!isValid)
			{
				modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid URL.");
			}
			return isValid;
		}
		private static bool ValidateInternalLinkToPageById(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			if (string.IsNullOrEmpty(stringValue1))
			{
				return true;
			}
			int value;
			if (int.TryParse(stringValue1, out value))
			{
				return true;
			}
			modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. You must enter a valid page Id.");
			return false;
		}
		private static bool ValidateInternalLinkToPageByUrl(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			if (string.IsNullOrEmpty(stringValue1))
			{
				return true;
			}
			if (!stringValue1.StartsWith("/"))
			{
				modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. Internal URL must start with a slash character.");
				return false;
			}
			return true;
		}
		private static bool ValidateInternalLinkToImageByUrl(string formFieldName, string labelText, string formFieldNameValue2, string stringValue1, string stringValue2, ModelStateDictionary modelStateDictionary)
		{
			if (string.IsNullOrEmpty(stringValue1))
			{
				return true;
			}
			if (!stringValue1.StartsWith("/"))
			{
				modelStateDictionary.AddModelError(formFieldName, labelText + " value '" + stringValue1 + "' is invalid. Internal URL must start with a slash character.");
				return false;
			}
			return true;
		}

		#endregion

		private static string GetValueListItemName(this WebFormField field, string stringValue)
		{
			//convert id value to text
			int value;
			if (!int.TryParse(stringValue, out value))
			{
				return "invalid value (not int): '" + stringValue + "'";
			}
			if (value == 0)
			{
				return "invalid value: " + stringValue;
			}
			if (field.ValueList == null)
			{
				return "unknown value (no value list): '" + stringValue + "'";
			}
			ValueListItem listItem = field.ValueList.ValueListItems.AsQueryable().WhereIsActive().SingleOrDefault(vli => vli.ValueListItemId == value);
			if (listItem == null)
			{
				return "unknown value (not found): '" + stringValue + "'";
			}
			return "'" + listItem.Name + "'";
		}

		private static string GetValueListItemNameList(this WebFormField field, string valueIdList)
		{
			//convert id values to text
			string[] stringValues = valueIdList.Split(',');
			string returnString = string.Empty;
			foreach (string value in stringValues)
			{
				if (!string.IsNullOrEmpty(returnString))
				{
					returnString += ",";
				}
				returnString += field.GetValueListItemName(value);
			}
			return returnString;
		}

	}
}