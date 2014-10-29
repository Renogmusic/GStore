using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Security.Principal;
using GStore.Models;

namespace GStore.Hubs
{
	public class NotifyHub : Hub
	{
		public static Int64 activeUsers = 0;

		[Authorize(Roles = "SystemAdmin")]
		public void SendMessage(string message)
		{
			string title = "Message from " + UserName();
			Clients.All.addNewMessageToPage(title, message);
		}

		/// <summary>
		/// Sends a message to a specific logged on user by user name. This only works for authenticated users
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="message"></param>
		[Authorize(Roles = "SystemAdmin")]
		public void SendMessageToLoggedOnUser(string userName, string message)
		{
			string title = "Message from " + UserName();
			Clients.User(userName).addNewMessageToPage(title, message);
		}

		/// <summary>
		/// Sends a message to a specific guest user (not authenticated). names have the format "Guest ID#" + Context.ConnectionId;
		/// </summary>
		/// <param name="guestName"></param>
		/// <param name="message"></param>
		[Authorize(Roles = "SystemAdmin")]
		public void SendMessageToGuest(string guestName, string message)
		{
			string title = "Message from " + UserName();
			Clients.Group(guestName).addNewMessageToPage(title, message);
		}

		public Int64 ActiveUsers()
		{
			return activeUsers;
		}

		public override System.Threading.Tasks.Task OnConnected()
		{
			//string title = "New user";
			//string message = "Visiting..." + UserName();
			//Clients.Others.addNewMessageToPage(title, message);
			string name = string.Empty;
			if (Context.User.Identity.IsAuthenticated)
			{
				name = Context.User.Identity.Name;
			}
			else
			{
				name = "Guest ID#" + Context.ConnectionId;
			}

			Groups.Add(Context.ConnectionId, name);

			activeUsers++;
			return base.OnConnected();
		}

		public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
		{
			activeUsers--;
			return base.OnDisconnected(stopCalled);
		}

		public override System.Threading.Tasks.Task OnReconnected()
		{
			//activeUsers++;
			return base.OnReconnected();
		}

		protected string UserName()
		{
			string userName = string.Empty;

			if (base.Context.User != null && base.Context.User.Identity != null && base.Context.User.Identity.IsAuthenticated)
			{
				Data.IGstoreDb ctx = Data.RepositoryFactory.StoreFrontRepository(this.Context.Request.GetHttpContext(), false);
				Models.UserProfile profile = Models.Extensions.GStoreDBExtensions.GetCurrentUserProfile(ctx, true);
				userName = profile.FullName;
			}
			else
			{
				userName = "Guest ID#" + base.Context.ConnectionId;
			}

			return userName;
		}
	}
}