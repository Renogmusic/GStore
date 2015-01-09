using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GStore.Models.ViewModels
{
	public class NotificationCreateViewModel
	{
		public int ToUserProfileId { get; set; }

		[Required]
		public string Subject { get; set; }

		[Display(Name = "Priority")]
		[Required]
		public string Importance { get; set; }

		[AllowHtml]
		[Required]
		[DataType(DataType.Html)]
		public string Message { get; set; }

		public string Link1Text { get; set; }

		public string Link1Url { get; set; }

		public string Link2Text { get; set; }

		public string Link2Url { get; set; }

		public string Link3Text { get; set; }

		public string Link3Url { get; set; }

		public string Link4Text { get; set; }
		public string Link4Url { get; set; }

		public NotificationCreateViewModel()
		{
		}

		public void StartReply(Notification notification)
		{
			this.ToUserProfileId = notification.FromUserProfileId;
			this.Subject = "Re: " + notification.Subject;
			this.Importance = notification.Importance;
			this.Message = "\n\n\n\n"
				+ "---Original Message---\n"
				+ "  -Date: " + notification.CreateDateTimeUtc.ToLocalTime() + "\n"
				+ "  -From: " + notification.From + "\n"
				+ "  -To: " + notification.To + "\n"
				+ "  -Subject: " + notification.To + "\n\n"
				+ notification.Message;
		}
	}
}