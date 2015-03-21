using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GStoreData.Models;

namespace GStoreData.ViewModels
{
	public class NotificationCreateViewModel: IValidatableObject
	{
		public int ToUserProfileId { get; set; }

		[Required]
		public string Subject { get; set; }

		[Display(Name = "Priority")]
		[Required]
		public string Importance { get; set; }

		public string OrderEmail { get; set; }

		public int? OrderId { get; set; }

		public Order Order { get; protected set; }

		[AllowHtml]
		[Required]
		[DataType(DataType.Html)]
		public string Message { get; set; }

		[Display(Name = "Label", Description = "Label Text for Link 1 or leave blank if no link 1.\nExample: 'Link to Google'")]
		public string Link1Text { get; set; }

		[Display(Name = "Url", Description = "Url for Link 1 or leave blank if no link 1.\nExample: http://www.google.com")]
		public string Link1Url { get; set; }

		[Display(Name = "Label", Description = "Label Text for Link 2 or leave blank if no link 2.\nExample: 'Link to Google'")]
		public string Link2Text { get; set; }

		[Display(Name = "Url", Description = "Url for Link 2 or leave blank if no link 2.\nExample: http://www.google.com")]
		public string Link2Url { get; set; }

		[Display(Name = "Label", Description = "Label Text for Link 3 or leave blank if no link 3.\nExample: 'Link to Google'")]
		public string Link3Text { get; set; }

		[Display(Name = "Url", Description = "Url for Link 3 or leave blank if no link 3.\nExample: http://www.google.com")]
		public string Link3Url { get; set; }

		[Display(Name = "Label", Description="Label Text for Link 4 or leave blank if no link 4.\nExample: 'Link to Google'")]
		public string Link4Text { get; set; }

		[Display(Name = "Url", Description="Url for Link 4 or leave blank if no link 4.\nExample: http://www.google.com")]
		public string Link4Url { get; set; }

		public NotificationCreateViewModel()
		{
		}

		public void StartReply(Notification notification)
		{
			this.OrderId = notification.OrderId;
			this.Order = notification.Order;
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

		public void UpdateOrder(Order order)
		{
			if (order != null)
			{
				this.Order = order;
				this.OrderId = order.OrderId;
			}
		}

		#region IValidatableObject Members

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> result = new List<ValidationResult>();

			if (string.IsNullOrEmpty(this.Link1Text) ^ string.IsNullOrEmpty(this.Link1Url))
			{
				//one or the other is specified
				if (string.IsNullOrEmpty(this.Link1Text))
				{
					result.Add(new ValidationResult("You must enter a label for Link 1.", new string[] { "Link1Text" }));
				}
				else
				{
					result.Add(new ValidationResult("You must enter a URL for Link 1.", new string[] { "Link1Url" }));
				}
			}

			if (string.IsNullOrEmpty(this.Link2Text) ^ string.IsNullOrEmpty(this.Link2Url))
			{
				//one or the other is specified
				if (string.IsNullOrEmpty(this.Link2Text))
				{
					result.Add(new ValidationResult("You must enter a label for Link 2.", new string[] { "Link2Text" }));
				}
				else
				{
					result.Add(new ValidationResult("You must enter a URL for Link 2.", new string[] { "Link2Url" }));
				}
			}

			if (string.IsNullOrEmpty(this.Link3Text) ^ string.IsNullOrEmpty(this.Link3Url))
			{
				//one or the other is specified
				if (string.IsNullOrEmpty(this.Link3Text))
				{
					result.Add(new ValidationResult("You must enter a label for Link 3.", new string[] { "Link3Text" }));
				}
				else
				{
					result.Add(new ValidationResult("You must enter a URL for Link 3.", new string[] { "Link3Url" }));
				}
			}

			if (string.IsNullOrEmpty(this.Link4Text) ^ string.IsNullOrEmpty(this.Link4Url))
			{
				//one or the other is specified
				if (string.IsNullOrEmpty(this.Link4Text))
				{
					result.Add(new ValidationResult("You must enter a label for Link 4.", new string[] { "Link4Text" }));
				}
				else
				{
					result.Add(new ValidationResult("You must enter a URL for Link 4.", new string[] { "Link4Url" }));
				}
			}
			
			return result;
		}

		#endregion
	}
}