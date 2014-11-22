using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GStore.Exceptions
{
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

	[Serializable]
	public class DynamicPageInactiveException : DynamicPageNotFoundException
	{
		public DynamicPageInactiveException() { }
		public DynamicPageInactiveException(string message, string url, StoreFront storeFront)
			: base(message, url, storeFront)
		{
		}
	}

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


}

