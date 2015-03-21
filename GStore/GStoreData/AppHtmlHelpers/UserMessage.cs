using System;
using System.Collections.Generic;

namespace GStoreData.AppHtmlHelpers
{
	public enum UserMessageType
	{
		Info = 1,
		Warning = 2,
		Success = 3,
		Danger = 4
	}

	public class UserMessage
	{
		public UserMessage()
		{
			this.CreatedDateTimeUtc = DateTime.UtcNow;
		}

		public UserMessage(string title, string message, UserMessageType messageType)
		{
			this.CreatedDateTimeUtc = DateTime.UtcNow;
			this.Title = title;
			this.Message = message;
			this.MessageType = messageType;
		}

		public string Title { get; set; }

		public string Message { get; set; }

		public UserMessageType MessageType { get; set; }

		public DateTime CreatedDateTimeUtc { get; set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Message))
			{
				return "(no message)";
			}
			else
			{
				return "(" + MessageType.ToString() + ") " + Title + ": " + Message;
			}
		}

	}

	public static class AddUserMessageExtension
	{
		/// <summary>
		/// Adds a message to the user's user message alerts (top)
		/// </summary>
		/// <param name="controller"></param>
		/// <param name="title"></param>
		/// <param name="message"></param>
		/// <param name="userMessageType"></param>
		public static void AddUserMessage(this System.Web.Mvc.TempDataDictionary tempData, string title, string message, UserMessageType userMessageType)
		{
			if (!tempData.ContainsKey("UserMessages"))
			{
				tempData.Add("UserMessages", new List<UserMessage>());
			}

			List<UserMessage> userMessages = (List<UserMessage>)tempData["UserMessages"];
			userMessages.Add(new UserMessage(title, message, userMessageType));
		}
	}
}
