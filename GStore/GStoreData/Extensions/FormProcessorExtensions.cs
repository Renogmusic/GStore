using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using GStoreData.AppHtmlHelpers;
using GStoreData.ControllerBase;
using GStoreData.Models;
using GStoreData.Models.BaseClasses;

namespace GStoreData
{
	public static class FormProcessorExtensions
	{
		/// <summary>
		/// Processes a web form and saves result to file, database, or email based on web form settings. 
		/// Also handles newly registered user form
		/// </summary>
		public static bool ProcessWebForm(this BaseController controller, WebForm webForm, Page page, bool isRegisterPage, WebFormResponse oldResponseToUpdateOrNull)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}
			if (page == null && (!isRegisterPage))
			{
				throw new ArgumentNullException("page");
			}
			if (!isRegisterPage && !(page.WebFormSaveToDatabase || page.WebFormSaveToFile || page.WebFormSendToEmail))
			{
				throw new ArgumentException("No form processing specified in Page. One of the following must be enabled: page.WebFormSaveToDatabase or page.WebFormSaveToFile or page.WebFormSendToEmail", "page");
			}

			bool isValid = ValidateFields(controller, webForm);
			if (!controller.ModelState.IsValid || !isValid)
			{
				return false;
			}

			string formBodyText = BuildFormBodyText(controller, webForm, page, false);
			string formSubject = BuildFormSubject(controller, webForm, page);

			if (page == null || page.WebFormSaveToDatabase)
			{
				//register forms always go to database
				ProcessWebForm_ToDatabase(controller, webForm, page, formSubject, formBodyText, isRegisterPage, oldResponseToUpdateOrNull);
			}

			if (!isRegisterPage)
			{
				bool savedToFile = false;
				bool emailed = false;
				if (page.WebFormSaveToFile)
				{
					savedToFile = ProcessWebForm_ToFile(controller, webForm, page);
				}
				if (page.WebFormSendToEmail)
				{
					emailed = ProcessWebForm_ToEmail(controller, webForm, page);
				}
			}

			return true;

		}

		/// <summary>
		/// Processes a web form for a checkout page. saves response to database and return WebFormResponse object.
		/// If no data to save, returns null.
		/// </summary>
		/// <param name="db"></param>
		/// <param name="modelStateDictionary"></param>
		/// <param name="webForm"></param>
		/// <param name="storeFrontConfig"></param>
		/// <param name="userProfile"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public static WebFormResponse ProcessWebFormForCheckout(BaseController controller, WebForm webForm, WebFormResponse oldResponseToUpdateOrNull)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}

			bool isValid = ValidateFields(controller, webForm);
			if (!controller.ModelState.IsValid || !isValid)
			{
				return null;
			}

			string formBodyText = BuildFormBodyText(controller, webForm, null, false);
			string formSubject = BuildFormSubject(controller, webForm, null);

			//checkout forms always go to database
			WebFormResponse response = ProcessWebForm_ToDatabase(controller, webForm, null, formSubject, formBodyText, false, oldResponseToUpdateOrNull);

			return response;

		}

		public static string BodyTextCustomFieldsOnly(BaseController controller, WebForm webForm)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			return BuildFormBodyText(controller, webForm, null, true);
		}

		/// <summary>
		/// Runs validation on web form fields and sets model state errors if fields do not validate
		/// </summary>
		/// <param name="modelStateDictionary"></param>
		/// <param name="webForm"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public static bool ValidateFields(BaseController controller, WebForm webForm)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			bool isValid = true;
			List<WebFormField> fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();
			foreach (WebFormField field in fields)
			{
				string formFieldName = "Page.WebForm.item." + field.Name;
				string stringValue1 = null;
				if (controller.Request.Unvalidated.Form.AllKeys.Contains(formFieldName))
				{
					stringValue1 = controller.Request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				}
				if (stringValue1 == null)
				{
					formFieldName = "WebForm.item." + field.Name;
					if (controller.Request.Unvalidated.Form.AllKeys.Contains(formFieldName))
					{
						stringValue1 = controller.Request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
					}
				}
				if (stringValue1 == null)
				{
					formFieldName = "item." + field.Name;
					if (controller.Request.Unvalidated.Form.AllKeys.Contains(formFieldName))
					{
						stringValue1 = controller.Request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
					}
				}
				if (!controller.ModelState.ContainsKey(formFieldName))
				{
					controller.ModelState.Add(formFieldName, new ModelState() { Value = new ValueProviderResult(stringValue1, stringValue1, System.Globalization.CultureInfo.CurrentCulture) });
				}

				string formFieldNameValue2 = formFieldName + "_Value2";
				string stringValue2 = null;
				stringValue2 = controller.Request.Unvalidated.Form.Get(formFieldName) ?? string.Empty;
				if (!controller.ModelState.ContainsKey(formFieldNameValue2))
				{
					controller.ModelState.Add(formFieldNameValue2, new ModelState() { Value = new ValueProviderResult(stringValue2, stringValue2, System.Globalization.CultureInfo.CurrentCulture) });
				}


				if (field.IsRequired && string.IsNullOrWhiteSpace(stringValue1))
				{
					isValid = false;
					controller.ModelState.AddModelError(formFieldName, field.LabelText + " is required.");
				}
				else
				{
					bool result = false;
					switch (field.DataType)
					{
						case GStoreValueDataType.EmailAddress:
							result = ValidateEmailAddress(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.Url:
							result = ValidateUrl(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.SingleLineText:
							result = ValidateSingleLineText(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.MultiLineText:
							result = ValidateMultiLineText(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.CheckboxYesNo:
							result = ValidateCheckboxYesNo(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.ValueListItemDropdown:
							result = ValidateValueListItemDropdown(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.ValueListItemRadio:
							result = ValidateValueListItemRadio(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.ValueListItemMultiCheckbox:
							result = ValidateValueListItemMultiCheckbox(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.Integer:
							result = ValidateInteger(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.Decimal:
							result = ValidateDecimal(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.IntegerRange:
							result = ValidateIntegerRange(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.DecimalRange:
							result = ValidateDecimalRange(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.Html:
							result = ValidateHtml(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.ExternalLinkToPage:
							result = ValidateExternalLinkToPage(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.ExternalLinkToImage:
							result = ValidateExternalLinkToImage(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.InternalLinkToPageById:
							result = ValidateInternalLinkToPageById(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.InternalLinkToPageByUrl:
							result = ValidateInternalLinkToPageByUrl(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						case GStoreValueDataType.InternalLinkToImageByUrl:
							result = ValidateInternalLinkToImageByUrl(formFieldName, field.LabelText, formFieldNameValue2, stringValue1, stringValue2, controller.ModelState);
							break;
						default:
							controller.ModelState.AddModelError("", "Unknown data type: " + field.DataType.ToString() + "[" + (int)field.DataType + "]");
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

		/// <summary>
		/// saves form data to database, is isRegisterPage = true, also updates user profile
		/// </summary>
		/// <param name="db"></param>
		/// <param name="modelStateDictionary"></param>
		/// <param name="webForm"></param>
		/// <param name="page"></param>
		/// <param name="userProfile"></param>
		/// <param name="request"></param>
		/// <param name="storeFrontConfiguration"></param>
		/// <param name="formSubject"></param>
		/// <param name="formBodyText"></param>
		/// <param name="isRegisterPage"></param>
		/// <returns></returns>
		private static WebFormResponse ProcessWebForm_ToDatabase(BaseController controller, WebForm webForm, Page page, string formSubject, string formBodyText, bool isRegisterPage, WebFormResponse oldResponseToUpdateOrNull)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			IGstoreDb db = controller.GStoreDb;
			UserProfile userProfile = controller.CurrentUserProfileOrNull;
			StoreFrontConfiguration storeFrontConfiguration = controller.CurrentStoreFrontConfigOrThrow;

			WebFormResponse webFormResponse = null;
			if (oldResponseToUpdateOrNull == null)
			{
				webFormResponse = db.WebFormResponses.Create();
				webFormResponse.SetDefaults(userProfile);
			}
			else
			{
				webFormResponse = oldResponseToUpdateOrNull;
			}
			webFormResponse.StoreFrontId = storeFrontConfiguration.StoreFrontId;
			webFormResponse.StoreFront = storeFrontConfiguration.StoreFront;
			webFormResponse.ClientId = storeFrontConfiguration.ClientId;
			webFormResponse.Client = storeFrontConfiguration.Client;
			webFormResponse.PageId = (page == null ? null : (int?)page.PageId);
			webFormResponse.Page = page;
			webFormResponse.WebFormId = webForm.WebFormId;
			webFormResponse.WebForm = webForm;
			webFormResponse.IsPending = false;
			webFormResponse.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
			webFormResponse.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
			webFormResponse.BodyText = formBodyText;
			webFormResponse.Subject = formSubject;

			FillWebFormResponses(controller, webFormResponse, webForm, oldResponseToUpdateOrNull);

			if (oldResponseToUpdateOrNull == null)
			{
				webFormResponse = db.WebFormResponses.Add(webFormResponse);
			}
			else
			{
				webFormResponse = db.WebFormResponses.Update(oldResponseToUpdateOrNull);
			}
			db.SaveChanges();

			if (isRegisterPage && (userProfile != null))
			{
				userProfile.RegisterWebFormResponseId = webFormResponse.WebFormResponseId;
				db.UserProfiles.Update(userProfile);
				db.SaveChangesDirect();
			}

			//for checkout page and other pages return object
			return webFormResponse;
		}

		private static bool ProcessWebForm_ToFile(BaseController controller, WebForm webForm, Page page)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			StoreFront storeFront = controller.CurrentStoreFrontOrThrow;

			if (!controller.ModelState.IsValid)
			{
				return false;
			}

			string subject = BuildFormSubject(controller, webForm, page);
			string bodyText = BuildFormBodyText(controller, webForm, page, false);

			string virtualDir = storeFront.StoreFrontVirtualDirectoryToMap(controller.Request.ApplicationPath) + "\\Forms\\" + webForm.Name.ToFileName();
			string fileDir = controller.Server.MapPath(virtualDir);
			if (!System.IO.Directory.Exists(fileDir))
			{
				System.IO.Directory.CreateDirectory(fileDir);
			}

			string fileName = DateTime.UtcNow.ToFileSafeString() + System.Guid.NewGuid().ToString() + ".txt";

			System.IO.File.AppendAllText(fileDir + "\\" + fileName, bodyText);

			return true;
		}

		private static bool ProcessWebForm_ToEmail(BaseController controller, WebForm webForm, Page page)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			StoreFrontConfiguration storeFrontConfiguration = controller.CurrentStoreFrontConfigOrThrow;
			UserProfile userProfile = controller.CurrentUserProfileOrNull;

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

			string subject = BuildFormSubject(controller, webForm, page);
			string bodyText = BuildFormBodyText(controller, webForm, page, false);
			string bodyHtml = bodyText.ToHtmlLines();

			return controller.SendEmail(toEmail, toName, subject, bodyText, bodyHtml);

		}

		private static string BuildFormSubject(BaseController controller, WebForm webForm, Page page)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}

			if (!controller.ModelState.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormSubject");
			}

			return controller.Request.Url.Host + " Form '" + webForm.Name + "' [" + webForm.WebFormId + "] submitted at " + DateTime.UtcNow.ToLocalTime();
		}

		private static string BuildFormBodyText(BaseController controller, WebForm webForm, Page page, bool fieldDataOnly)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}

			if (webForm == null)
			{
				throw new ArgumentNullException("webForm");
			}
			if (!fieldDataOnly && !controller.ModelState.IsValid)
			{
				throw new ApplicationException("Model State is invalid. Be sure model state is valid before calling BuildFormBodyText");
			}

			StringBuilder textBody = new StringBuilder();
			if (!fieldDataOnly)
			{
				textBody.AppendLine(BuildFormSubject(controller, webForm, page));
			}
			textBody.AppendLine();

			List<WebFormField> fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();

			foreach (WebFormField field in fields)
			{
				string value1 = controller.Request.GetFormFieldValue1String(field);
				string value2 = controller.Request.GetFormFieldValue2String(field);

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
				UserProfile userProfile = controller.CurrentUserProfileOrNull;
				StoreFrontConfiguration storeFrontConfiguration = controller.CurrentStoreFrontConfigOrThrow;

				textBody.AppendLine();
				textBody.AppendLine("- - - - - - - -");
				//header info
				textBody.AppendLine("User: " + (userProfile == null ? "(anonymous)" : "'" + userProfile.FullName + "' <" + userProfile.Email + ">"));
				textBody.AppendLine("Url: " + controller.Request.Url.ToString());
				textBody.AppendLine("Store Front: " + storeFrontConfiguration.Name + " [" + storeFrontConfiguration.StoreFrontId + "] Url: " + storeFrontConfiguration.PublicUrl);
				textBody.AppendLine("Client: " + storeFrontConfiguration.Client.Name + " [" + storeFrontConfiguration.ClientId + "]");
				textBody.AppendLine("Host: " + controller.Request.Url.Host);
				textBody.AppendLine("Raw Url: " + controller.Request.RawUrl);
				textBody.AppendLine("IP Address: " + controller.Request.UserHostAddress);
				textBody.AppendLine("User Agent: " + controller.Request.UserAgent);
				textBody.AppendLine();

				HttpSessionStateBase session = controller.Session;
				textBody.AppendLine("Session Start Date Time: " + (session.EntryDateTime().HasValue ? session.EntryDateTime().ToString() : "(unknown)"));
				textBody.AppendLine("Session Entry Raw Url: " + (session.EntryRawUrl() ?? "(unknown)"));
				textBody.AppendLine("Session Entry Url: " + (session.EntryUrl() ?? "(unknown)"));
				textBody.AppendLine("Session Referrer: " + (session.EntryReferrer() ?? "(unknown)"));
				textBody.AppendLine();
			}

			return textBody.ToString();
		}

		private static void FillWebFormResponses(BaseController controller, WebFormResponse webFormResponse, WebForm webForm, WebFormResponse oldResponseOrNull)
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			IGstoreDb db = controller.GStoreDb;
			UserProfile userProfile = controller.CurrentUserProfileOrNull;
			StoreFrontConfiguration storeFrontConfiguration = controller.CurrentStoreFrontConfigOrThrow;

			if (webFormResponse == null)
			{
				throw new ArgumentNullException("webFormResponse");
			}
			if (webForm == null)
			{
				throw new ArgumentNullException("webFormResponse");
			}

			List<WebFormField> fields = webForm.WebFormFields.AsQueryable().WhereIsActive().ApplySortDefault().ToList();

			foreach (WebFormField field in fields)
			{
				WebFormFieldResponse oldFieldResponse = null;
				WebFormFieldResponse fieldResponse = null;
				if (oldResponseOrNull != null)
				{
					oldFieldResponse = oldResponseOrNull.WebFormFieldResponses.SingleOrDefault(wffr => wffr.WebFormFieldId == field.WebFormFieldId);
				}
				if (oldFieldResponse != null)
				{
					fieldResponse = oldFieldResponse;
				}
				else
				{
					fieldResponse = db.WebFormFieldResponses.Create();
					fieldResponse.WebFormField = field;
					fieldResponse.WebFormFieldId = field.WebFormFieldId;
					fieldResponse.SetDefaults(userProfile);
				}

				fieldResponse.ClientId = storeFrontConfiguration.ClientId;
				fieldResponse.Client = storeFrontConfiguration.Client;
				fieldResponse.StoreFrontId = storeFrontConfiguration.StoreFrontId;
				fieldResponse.StoreFront = storeFrontConfiguration.StoreFront;
				fieldResponse.DataType = field.DataType;
				fieldResponse.DataTypeString = field.DataTypeString;
				fieldResponse.IsPending = false;
				fieldResponse.StartDateTimeUtc = DateTime.UtcNow.AddMinutes(-1);
				fieldResponse.EndDateTimeUtc = DateTime.UtcNow.AddYears(100);
				fieldResponse.WebFormFieldLabelText = field.LabelText;
				fieldResponse.WebFormFieldName = field.Name;
				fieldResponse.WebFormFieldOrder = field.Order;
				fieldResponse.WebFormName = field.WebForm.Name;
				fieldResponse.WebFormOrder = field.WebForm.Order;

				fieldResponse.SetValueFieldsFromFormValues(controller.Request);

				if (oldFieldResponse == null)
				{
					db.WebFormFieldResponses.Add(fieldResponse);
				}
				else
				{
					db.WebFormFieldResponses.Update(fieldResponse);
				}

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

		/// <summary>
		/// gets a display text value from WebForm value fields using their data type and returns a user readable string
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		public static string ValueText(this WebFormFieldResponse response)
		{
			if (response == null)
			{
				return null;
			}

			string text = string.Empty;
			switch (response.DataType)
			{
				case GStoreValueDataType.CheckboxYesNo:
					text = response.Value1Bool.HasValue ? response.Value1Bool.ToString() : "(blank)";
					break;
				case GStoreValueDataType.ValueListItemDropdown:
				case GStoreValueDataType.ValueListItemRadio:
					text = response.Value1ValueListItemName;
					break;
				case GStoreValueDataType.ValueListItemMultiCheckbox:
					text = response.Value1ValueListItemNameList;
					break;
				case GStoreValueDataType.Integer:
					text = response.Value1Int.HasValue ? response.Value1Int.ToString() : "(blank)";
					break;
				case GStoreValueDataType.Decimal:
					text = response.Value1Decimal.HasValue ? response.Value1Decimal.ToString() : "(blank)";
					break;
				case GStoreValueDataType.IntegerRange:
					text = (response.Value1Int.HasValue ? response.Value1Int.ToString() : "(blank)")
						+ " to "
						+ (response.Value2Int.HasValue ? response.Value2Int.ToString() : "(blank)");
					break;
				case GStoreValueDataType.DecimalRange:
					text = (response.Value1Decimal.HasValue ? response.Value1Decimal.ToString() : "(blank)")
						+ " to "
						+ (response.Value2Decimal.HasValue ? response.Value2Decimal.ToString() : "(blank)");
					break;
				default:
					text = string.IsNullOrEmpty(response.Value1String) ? "(blank)" : response.Value1String;
					break;
			}

			return text;
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