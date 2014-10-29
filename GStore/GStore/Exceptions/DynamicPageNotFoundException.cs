using GStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GStore.Exceptions
{
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

		protected DynamicPageNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}

	[Serializable]
	public class DynamicPageInactiveException : ApplicationException
	{
		public DynamicPageInactiveException() { }
		public DynamicPageInactiveException(string message, string url, StoreFront storeFront)
			: base(message)
		{
			this.Url = url;
			this.StoreFront = storeFront;
		}

		public string Url { get; protected set; }
		public StoreFront StoreFront { get; protected set; }

		protected DynamicPageInactiveException(
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

		public StoreFront StoreFront { get; protected set;}
		public Uri Uri { get; protected set; }

		protected StoreFrontInactiveException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}


}

