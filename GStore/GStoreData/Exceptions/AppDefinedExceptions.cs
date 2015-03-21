using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using GStoreData.ControllerBase;
using GStoreData.Models;

namespace GStoreData.Exceptions
{
	/// <summary>
	/// Exception containing a database error or query error
	/// </summary>
	[Serializable]
	public class DatabaseErrorException : ApplicationException
	{
		public DatabaseErrorException() { }
		public DatabaseErrorException(string message, Exception innerException)
			: base(message, innerException) { }

		protected DatabaseErrorException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Exception meaning a specific dynamic page is inactive, it does not mean it doesn't exists, this explicitly means it is found and is inactive
	/// </summary>
	[Serializable]
	public class DynamicPageInactiveException : DynamicPageNotFoundException
	{
		public DynamicPageInactiveException() { }
		public DynamicPageInactiveException(string message, string url, StoreFront storeFront)
			: base(message, url, storeFront)
		{
		}
	}

	/// <summary>
	/// Exception meaning a dynamic page is not found for the current url. This means the URL is bad, wrong, a hack, or the page has been deleted
	/// </summary>
	[Serializable]
	public class DynamicPageNotFoundException : ApplicationException
	{
		public DynamicPageNotFoundException() { }
		public DynamicPageNotFoundException(string message, string url, StoreFront storeFront)
			: base(message)
		{
			this.Url = url;
			this.StoreFront = storeFront;
		}

		public string Url { get; protected set; }
		public StoreFront StoreFront { get; protected set; }

		public bool IsHomePage
		{
			get
			{
				if (string.IsNullOrEmpty(Url))
				{
					return true;
				}
				if (Url.Trim(' ', '/', '\\').Length == 0)
				{
					return true;
				}
				return false;
			}
		}

		protected DynamicPageNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Exception meaning a web form used for this post is inactive
	/// </summary>
	[Serializable]
	public class WebFormInactiveException : WebFormNotFoundException
	{
		public WebFormInactiveException() { }
		public WebFormInactiveException(string message, string url, StoreFront storeFront)
			: base(message, url, storeFront)
		{
		}
	}

	/// <summary>
	/// Exception meaning a web form used for this post is not found and does not exist any more
	/// </summary>
	[Serializable]
	public class WebFormNotFoundException : ApplicationException
	{
		public WebFormNotFoundException() { }
		public WebFormNotFoundException(string message, string url, StoreFront storeFront)
			: base(message)
		{
			this.Url = url;
			this.StoreFront = storeFront;
		}

		public string Url { get; protected set; }
		public StoreFront StoreFront { get; protected set; }

		public bool IsHomePage
		{
			get
			{
				if (string.IsNullOrEmpty(Url))
				{
					return true;
				}
				if (Url.Trim(' ', '/', '\\').Length == 0)
				{
					return true;
				}
				return false;
			}
		}

		protected WebFormNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Exception meaning there is no matching binding for this URL to a store front. Usually this means the current URL is new and not configured yet
	/// </summary>
	[Serializable]
	public class NoMatchingBindingException : ApplicationException
	{
		public NoMatchingBindingException() { }
		public NoMatchingBindingException(string message, Uri uri)
			: base(message)
		{
			this.Uri = uri;
		}

		public Uri Uri { get; protected set; }

		protected NoMatchingBindingException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Exception meaning the store front this url points to is inactive
	/// </summary>
	[Serializable]
	public class StoreFrontInactiveException : ApplicationException
	{
		public StoreFrontInactiveException() { }
		public StoreFrontInactiveException(string message, Uri uri, StoreFront storeFront)
			: base(message)
		{
			this.Uri = uri;
			this.StoreFront = storeFront;
		}

		public StoreFront StoreFront { get; protected set; }
		public Uri Uri { get; protected set; }

		protected StoreFrontInactiveException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Exception for 404 File Not Found errors. Can also be thrown if a specific record is not found
	/// </summary>
	[Serializable]
	public class HttpFileNotFoundException : HttpExceptionBase
	{
		public HttpFileNotFoundException(string message, string area, BaseController gstoreControllerForArea)
			: base(404, message, area, gstoreControllerForArea)
		{
		}
	}

	/// <summary>
	/// Exception for 400 Bad Request errors. Usually means a required url parameter is missing or this page is being accessed by a hack
	/// </summary>
	[Serializable]
	public class HttpBadRequestException : HttpExceptionBase
	{
		public HttpBadRequestException(string message, string area, BaseController gstoreControllerForArea)
			: base(400, message, area, gstoreControllerForArea)
		{
		}
	}

	/// <summary>
	/// Exception for 403 Forbidden errors. Usually means directory listing is denied, not serious
	/// </summary>
	[Serializable]
	public class HttpForbiddenException : HttpExceptionBase
	{
		public HttpForbiddenException() { }
		public HttpForbiddenException(string message, string area, BaseController gstoreControllerForArea)
			: base(403, message, area, gstoreControllerForArea)
		{
		}
	}

	/// <summary>
	/// Base class for HTTP Exceptions
	/// </summary>
	[Serializable]
	public abstract class HttpExceptionBase : HttpException
	{
		public HttpExceptionBase() { }
		public HttpExceptionBase(int code, string message, string area, BaseController gstoreControllerForArea)
			: base(code, message)
		{
			this.Area = area;
			this.GStoreControllerForArea = gstoreControllerForArea;
		}

		public string Area { get; set; }

		public BaseController GStoreControllerForArea { get; set; }

		protected HttpExceptionBase(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	/// <summary>
	/// Exception thrown when OAuth failed. Usually a problem with the client_Id and client_secret. Be sure to verify sandbox setting
	/// </summary>
	[Serializable]
	public class PayPalExceptionOAuthFailed : PayPalExceptionBase
	{
		public PayPalExceptionOAuthFailed(bool isSandbox, string message, HttpResponseMessage httpResponse, Exception innerException)
			: base(isSandbox, message, httpResponse, innerException)
		{
		}
	}

	/// <summary>
	/// Exception thrown when a Create Payment API call has failed. This can be for a variety of reasons. Be sure to verify sandbox setting
	/// </summary>
	[Serializable]
	public class PayPalExceptionCreatePaymentFailed : PayPalExceptionBase
	{
		public PayPalExceptionCreatePaymentFailed(bool isSandbox, string message, HttpResponseMessage httpResponse, Exception innerException)
			: base(isSandbox, message, httpResponse, innerException)
		{
		}

	}

	/// <summary>
	/// Exception thrown when a Complete Payment API call has failed. This can be for a variety of reasons like the user does not have funds. Be sure to verify sandbox setting
	/// </summary>
	[Serializable]
	public class PayPalExceptionExecutePaymentFailed : PayPalExceptionBase
	{
		public PayPalExceptionExecutePaymentFailed(bool isSandbox, string message, HttpResponseMessage httpResponse, Exception innerException)
			: base(isSandbox, message, httpResponse, innerException)
		{
		}
	}


	/// <summary>
	/// Base class for PayPal exceptions
	/// </summary>
	[Serializable]
	public abstract class PayPalExceptionBase : PaymentExceptionBase
	{
		public PayPalExceptionBase(bool isSandbox, string message, HttpResponseMessage httpResponse, Exception innerException)
			: base(message, innerException)
		{
			this.IsSandbox = isSandbox;
			this.httpResponse = httpResponse;
			if (httpResponse != null && httpResponse.Content != null)
			{
				try
				{
					HttpResponseHeaders headers = httpResponse.Headers;
					foreach (var header in headers)
					{
						this.ResponseHeaders += header.Key + " = " + String.Join(", ", header.Value.ToArray()) + "\n";
					}
				}
				catch (Exception)
				{
				}

				if (httpResponse.Content != null)
				{
					try
					{
						this.ResponseString = httpResponse.Content.ReadAsStringAsync().Result;
					}
					catch (Exception)
					{
					}
				}
			}
		}

		/// <summary>
		/// If true, this API call ran against the sandbox test environment
		/// </summary>
		public bool IsSandbox { get; set; }

		/// <summary>
		/// HTTP Response message if there was an API call
		/// </summary>
		public HttpResponseMessage httpResponse { get; set; }


		/// <summary>
		/// Response String or JSON data if set from response
		/// </summary>
		public string ResponseString { get; set; }

		/// <summary>
		/// Response Headers if set from response
		/// </summary>
		public string ResponseHeaders { get; set; }

		public PayPalExceptionBase() { }
	}

	/// <summary>
	/// Base class for payment exceptions for any payment provider
	/// </summary>
	[Serializable]
	public abstract class PaymentExceptionBase : ApplicationException
	{
		public PaymentExceptionBase(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public PaymentExceptionBase() { }

		protected PaymentExceptionBase(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}

