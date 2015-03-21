using System;
using GStoreData;
using GStoreData.Models;
using Microsoft.AspNet.SignalR;

namespace GStoreWeb.Hubs
{
	public class NotifyHub : Hub
	{
		public Int64 ActiveUsers()
		{
			return _activeUsers;
		}

		public static void IncrementActiveUsers()
		{
			lock (_lockObject)
			{
				_activeUsers++;
			}
		}

		public static void DecrementActiveUsers()
		{
			lock (_lockObject)
			{
				_activeUsers--;
			}
		}

		private static Int64 _activeUsers = 0;
		private static object _lockObject = new object();

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

			IncrementActiveUsers();
			return base.OnConnected();
		}

		public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
		{
			DecrementActiveUsers();
			return base.OnDisconnected(stopCalled);
		}

		public override System.Threading.Tasks.Task OnReconnected()
		{
			return base.OnReconnected();
		}

		protected string UserName()
		{
			string userName = string.Empty;

			if (base.Context.User != null && base.Context.User.Identity != null && base.Context.User.Identity.IsAuthenticated)
			{
				IGstoreDb ctx = RepositoryFactory.StoreFrontRepository(this.Context.Request.GetHttpContext());
				UserProfile profile = UserProfileExtensions.GetCurrentUserProfile(ctx, true, true);
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